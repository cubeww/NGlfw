using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int _GLFW_FIND_LOADER = 1;
    const int _GLFW_REQUIRE_LOADER = 2;

    [StructLayout(LayoutKind.Sequential)]
    struct VkExtensionProperties
    {
        public fixed byte extensionName[256];
        public uint specVersion;
    }

    static int _glfwInitVulkan(int mode)
    {
        delegate* unmanaged<byte*, uint*, VkExtensionProperties*, int> vkEnumerateInstanceExtensionProperties;
        uint count = 0;

        if (_glfw.vk.available != 0)
            return GLFW_TRUE;

        if (_glfw.hints.init.vulkanLoader != null)
            _glfw.vk.GetInstanceProcAddr = _glfw.hints.init.vulkanLoader;
        else if (_glfw.hints.init.vulkanLoaderNative != null)
            _glfw.vk.GetInstanceProcAddrNative = _glfw.hints.init.vulkanLoaderNative;
        else
        {
            void* handle = null;
            if (_glfw.platform.loadLocalVulkanLoader != null)
                handle = _glfw.platform.loadLocalVulkanLoader();
            if (handle == null)
                handle = vulkan_loadModule(vulkan_getLoaderName());

            if (handle == null)
            {
                if (mode == _GLFW_REQUIRE_LOADER)
                    _glfwInputError(GLFW_API_UNAVAILABLE, "Vulkan: Loader not found");

                return GLFW_FALSE;
            }

            _glfw.vk.handle = handle;

            var vkGetInstanceProcAddr = vulkan_getModuleSymbol(handle, "vkGetInstanceProcAddr");
            if (vkGetInstanceProcAddr == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE, "Vulkan: Loader does not export vkGetInstanceProcAddr");

                _glfwTerminateVulkan();
                return GLFW_FALSE;
            }

            _glfw.vk.GetInstanceProcAddrNative =
                (delegate* unmanaged<void*, byte*, void*>)vkGetInstanceProcAddr;
        }

        vkEnumerateInstanceExtensionProperties =
            (delegate* unmanaged<byte*, uint*, VkExtensionProperties*, int>)
            vulkan_getInstanceProcAddress(null, "vkEnumerateInstanceExtensionProperties");
        if (vkEnumerateInstanceExtensionProperties == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Vulkan: Failed to retrieve vkEnumerateInstanceExtensionProperties");

            _glfwTerminateVulkan();
            return GLFW_FALSE;
        }

        var err = vkEnumerateInstanceExtensionProperties(null, &count, null);
        if (err != VK_SUCCESS)
        {
            if (mode == _GLFW_REQUIRE_LOADER)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "Vulkan: Failed to query instance extension count: {0}",
                    vulkan_resultString(err));
            }

            _glfwTerminateVulkan();
            return GLFW_FALSE;
        }

        VkExtensionProperties* ep = null;
        if (count != 0)
        {
            ep = (VkExtensionProperties*)_glfw_calloc(count, (nuint)sizeof(VkExtensionProperties));
            if (ep == null)
            {
                _glfwTerminateVulkan();
                return GLFW_FALSE;
            }
        }

        err = vkEnumerateInstanceExtensionProperties(null, &count, ep);
        if (err != VK_SUCCESS)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Vulkan: Failed to query instance extensions: {0}",
                vulkan_resultString(err));

            _glfw_free(ep);
            _glfwTerminateVulkan();
            return GLFW_FALSE;
        }

        for (uint i = 0; i < count; i++)
        {
            if (vulkan_stringEquals(ep[i].extensionName, "VK_KHR_surface") != 0)
                _glfw.vk.KHR_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_KHR_win32_surface") != 0)
                _glfw.vk.KHR_win32_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_MVK_macos_surface") != 0)
                _glfw.vk.MVK_macos_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_EXT_metal_surface") != 0)
                _glfw.vk.EXT_metal_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_KHR_xlib_surface") != 0)
                _glfw.vk.KHR_xlib_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_KHR_xcb_surface") != 0)
                _glfw.vk.KHR_xcb_surface = GLFW_TRUE;
            else if (vulkan_stringEquals(ep[i].extensionName, "VK_KHR_wayland_surface") != 0)
                _glfw.vk.KHR_wayland_surface = GLFW_TRUE;
        }

        _glfw_free(ep);

        _glfw.vk.extensions = (byte**)_glfw_calloc(2, (nuint)sizeof(byte*));
        if (_glfw.vk.extensions == null)
        {
            _glfwTerminateVulkan();
            return GLFW_FALSE;
        }

        _glfw.vk.available = GLFW_TRUE;
        _glfw.platform.getRequiredInstanceExtensions(_glfw.vk.extensions);

        return GLFW_TRUE;
    }

    static void _glfwTerminateVulkan()
    {
        if (_glfw.vk.handle != null)
            _glfwPlatformFreeModule(_glfw.vk.handle);

        _glfw_free(_glfw.vk.extensions);
        _glfw.vk = default;
    }

    static string vulkan_getLoaderName()
    {
        if (OperatingSystem.IsWindows())
            return "vulkan-1.dll";
        if (OperatingSystem.IsMacOS())
            return "libvulkan.1.dylib";

        return "libvulkan.so.1";
    }

    static void* vulkan_loadModule(string name)
    {
        var path = stackalloc byte[name.Length + 1];
        for (var i = 0; i < name.Length; i++)
            path[i] = (byte)name[i];
        path[name.Length] = 0;

        return _glfwPlatformLoadModule(path);
    }

    static void* vulkan_getModuleSymbol(void* module, string name)
    {
        var symbolName = stackalloc byte[name.Length + 1];
        for (var i = 0; i < name.Length; i++)
            symbolName[i] = (byte)name[i];
        symbolName[name.Length] = 0;

        return _glfwPlatformGetModuleSymbol(module, symbolName);
    }

    static int vulkan_stringEquals(byte* value, string expected)
    {
        for (var i = 0; i < expected.Length; i++)
        {
            if (value[i] != (byte)expected[i])
                return GLFW_FALSE;
        }

        return value[expected.Length] == 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void* vulkan_getInstanceProcAddress(void* instance, string procname)
    {
        var name = stackalloc byte[procname.Length + 1];
        for (var i = 0; i < procname.Length; i++)
            name[i] = (byte)procname[i];
        name[procname.Length] = 0;

        return vulkan_getInstanceProcAddress(instance, name);
    }

    static void* vulkan_getInstanceProcAddress(void* instance, byte* procname)
    {
        if (_glfw.vk.GetInstanceProcAddr != null)
            return _glfw.vk.GetInstanceProcAddr(instance, procname);

        if (_glfw.vk.GetInstanceProcAddrNative != null)
            return _glfw.vk.GetInstanceProcAddrNative(instance, procname);

        return null;
    }

    static void* vulkan_getModuleSymbol(byte* procname)
    {
        if (_glfw.vk.handle == null)
            return null;

        return _glfwPlatformGetModuleSymbol(_glfw.vk.handle, procname);
    }

    static void* vulkan_getLoaderProcAddress()
    {
        if (_glfw.vk.GetInstanceProcAddr != null)
            return _glfw.vk.GetInstanceProcAddr;

        if (_glfw.vk.GetInstanceProcAddrNative != null)
            return _glfw.vk.GetInstanceProcAddrNative;

        return null;
    }

    static string vulkan_resultString(int result)
    {
        return Marshal.PtrToStringUTF8((nint)_glfwGetVulkanResultString(result)) ??
               "ERROR: UNKNOWN VULKAN ERROR";
    }

    static byte* _glfwGetVulkanResultString(int result)
    {
        return result switch
        {
            VK_SUCCESS => _glfw_allocate_static_string("Success"),
            VK_NOT_READY => _glfw_allocate_static_string("A fence or query has not yet completed"),
            VK_TIMEOUT => _glfw_allocate_static_string("A wait operation has not completed in the specified time"),
            VK_EVENT_SET => _glfw_allocate_static_string("An event is signaled"),
            VK_EVENT_RESET => _glfw_allocate_static_string("An event is unsignaled"),
            VK_INCOMPLETE => _glfw_allocate_static_string("A return array was too small for the result"),
            VK_ERROR_OUT_OF_HOST_MEMORY => _glfw_allocate_static_string("A host memory allocation has failed"),
            VK_ERROR_OUT_OF_DEVICE_MEMORY => _glfw_allocate_static_string("A device memory allocation has failed"),
            VK_ERROR_INITIALIZATION_FAILED => _glfw_allocate_static_string("Initialization of an object could not be completed for implementation-specific reasons"),
            VK_ERROR_DEVICE_LOST => _glfw_allocate_static_string("The logical or physical device has been lost"),
            VK_ERROR_MEMORY_MAP_FAILED => _glfw_allocate_static_string("Mapping of a memory object has failed"),
            VK_ERROR_LAYER_NOT_PRESENT => _glfw_allocate_static_string("A requested layer is not present or could not be loaded"),
            VK_ERROR_EXTENSION_NOT_PRESENT => _glfw_allocate_static_string("A requested extension is not supported"),
            VK_ERROR_FEATURE_NOT_PRESENT => _glfw_allocate_static_string("A requested feature is not supported"),
            VK_ERROR_INCOMPATIBLE_DRIVER => _glfw_allocate_static_string("The requested version of Vulkan is not supported by the driver or is otherwise incompatible"),
            VK_ERROR_TOO_MANY_OBJECTS => _glfw_allocate_static_string("Too many objects of the type have already been created"),
            VK_ERROR_FORMAT_NOT_SUPPORTED => _glfw_allocate_static_string("A requested format is not supported on this device"),
            VK_ERROR_SURFACE_LOST_KHR => _glfw_allocate_static_string("A surface is no longer available"),
            VK_SUBOPTIMAL_KHR => _glfw_allocate_static_string("A swapchain no longer matches the surface properties exactly, but can still be used"),
            VK_ERROR_OUT_OF_DATE_KHR => _glfw_allocate_static_string("A surface has changed in such a way that it is no longer compatible with the swapchain"),
            VK_ERROR_INCOMPATIBLE_DISPLAY_KHR => _glfw_allocate_static_string("The display used by a swapchain does not use the same presentable image layout"),
            VK_ERROR_NATIVE_WINDOW_IN_USE_KHR => _glfw_allocate_static_string("The requested window is already connected to a VkSurfaceKHR, or to some other non-Vulkan API"),
            VK_ERROR_VALIDATION_FAILED_EXT => _glfw_allocate_static_string("A validation layer found an error"),
            _ => _glfw_allocate_static_string("ERROR: UNKNOWN VULKAN ERROR")
        };
    }

    public static int glfwVulkanSupported()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        return _glfwInitVulkan(_GLFW_FIND_LOADER);
    }

    public static byte** glfwGetRequiredInstanceExtensions(uint* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfwInitVulkan(_GLFW_REQUIRE_LOADER) == 0)
            return null;

        if (_glfw.vk.extensions == null || _glfw.vk.extensions[0] == null)
            return null;

        if (count != null)
            *count = 2;

        return _glfw.vk.extensions;
    }

    public static void* glfwGetInstanceProcAddress(void* instance, byte* procname)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (procname == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return null;
        }

        if (_glfwInitVulkan(_GLFW_REQUIRE_LOADER) == 0)
            return null;

        if (vulkan_stringEquals(procname, "vkGetInstanceProcAddr") != 0)
            return vulkan_getLoaderProcAddress();

        var proc = vulkan_getInstanceProcAddress(instance, procname);
        if (proc == null)
            proc = vulkan_getModuleSymbol(procname);

        return proc;
    }

    public static int glfwGetPhysicalDevicePresentationSupport(void* instance, void* device, uint queuefamily)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        if (_glfwInitVulkan(_GLFW_REQUIRE_LOADER) == 0)
            return GLFW_FALSE;

        if (_glfw.vk.extensions == null || _glfw.vk.extensions[0] == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "Vulkan: Window surface creation extensions not found");
            return GLFW_FALSE;
        }

        return _glfw.platform.getPhysicalDevicePresentationSupport(instance, device, queuefamily);
    }

    public static int glfwCreateWindowSurface(void* instance, GLFWwindow* window, void* allocator, ulong* surface)
    {
        if (surface != null)
            *surface = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return VK_ERROR_INITIALIZATION_FAILED;
        }

        if (_glfwInitVulkan(_GLFW_REQUIRE_LOADER) == 0)
            return VK_ERROR_INITIALIZATION_FAILED;

        if (_glfw.vk.extensions == null || _glfw.vk.extensions[0] == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "Vulkan: Window surface creation extensions not found");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.client != GLFW_NO_API)
        {
            _glfwInputError(GLFW_INVALID_VALUE,
                "Vulkan: Window surface creation requires the window to have the client API set to GLFW_NO_API");
            return VK_ERROR_NATIVE_WINDOW_IN_USE_KHR;
        }

        return _glfw.platform.createWindowSurface(instance, internalWindow, allocator, surface);
    }
}
