using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint WL_DISPLAY_GET_REGISTRY = 1;
    const uint WL_REGISTRY_BIND = 0;
    const uint WL_COMPOSITOR_CREATE_SURFACE = 0;
    const uint WL_SURFACE_DESTROY = 0;
    const uint WL_OUTPUT_RELEASE = 0;
    const uint WL_MARSHAL_FLAG_DESTROY = 1;

    static readonly byte* _glfwWaylandWlCompositor = _glfw_allocate_static_string("wl_compositor");
    static readonly byte* _glfwWaylandWlSubcompositor = _glfw_allocate_static_string("wl_subcompositor");
    static readonly byte* _glfwWaylandWlShm = _glfw_allocate_static_string("wl_shm");
    static readonly byte* _glfwWaylandWlOutput = _glfw_allocate_static_string("wl_output");
    static readonly byte* _glfwWaylandWlSeat = _glfw_allocate_static_string("wl_seat");
    static readonly byte* _glfwWaylandWlDataDeviceManager = _glfw_allocate_static_string("wl_data_device_manager");

    struct wl_registry_listener
    {
        public delegate* unmanaged<void*, void*, uint, byte*, uint, void> global;
        public delegate* unmanaged<void*, void*, uint, void> global_remove;
    }

    static wl_registry_listener* _glfwWaylandRegistryListener;

    static uint wayland_min(uint a, uint b)
    {
        return a < b ? a : b;
    }

    static int wayland_stringEquals(byte* value, string expected)
    {
        if (value == null)
            return GLFW_FALSE;

        for (var i = 0; i < expected.Length; i++)
        {
            if (value[i] != (byte)expected[i])
                return GLFW_FALSE;
        }

        return value[expected.Length] == 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static wl_registry_listener* wayland_getRegistryListener()
    {
        if (_glfwWaylandRegistryListener == null)
        {
            _glfwWaylandRegistryListener = (wl_registry_listener*)_glfw_calloc(1, (nuint)sizeof(wl_registry_listener));
            if (_glfwWaylandRegistryListener != null)
            {
                _glfwWaylandRegistryListener->global = &wayland_registryHandleGlobal;
                _glfwWaylandRegistryListener->global_remove = &wayland_registryHandleGlobalRemove;
            }
        }

        return _glfwWaylandRegistryListener;
    }

    static void wayland_tagProxy(void* proxy)
    {
        if (proxy == null || _glfw.wl.client.proxy_set_tag == null)
            return;

        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfw.wl.client.proxy_set_tag(proxy, &glfw->wl.tag);
    }

    static void* wayland_displayGetRegistry(void* display)
    {
        if (display == null || _glfw.wl.client.proxy_marshal_constructor == null)
            return null;

        return _glfw.wl.client.proxy_marshal_constructor(display,
            WL_DISPLAY_GET_REGISTRY,
            _glfw.wl.client.registryInterface,
            null);
    }

    static void* wayland_registryBind(void* registry,
                                      uint name,
                                      void* interfacePtr,
                                      byte* interfaceName,
                                      uint version)
    {
        if (registry == null ||
            interfacePtr == null ||
            interfaceName == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind == null)
        {
            return null;
        }

        var proxy = _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind(registry,
            WL_REGISTRY_BIND,
            interfacePtr,
            version,
            name,
            interfaceName,
            version,
            null);

        wayland_tagProxy(proxy);
        return proxy;
    }

    static void* wayland_compositorCreateSurface()
    {
        if (_glfw.wl.compositor == null || _glfw.wl.client.surfaceInterface == null)
            return null;

        var surface = _glfw.wl.client.proxy_marshal_constructor(_glfw.wl.compositor,
            WL_COMPOSITOR_CREATE_SURFACE,
            _glfw.wl.client.surfaceInterface,
            null);

        wayland_tagProxy(surface);
        return surface;
    }

    static void wayland_proxyDestroyWithOpcode(void* proxy, uint opcode)
    {
        if (proxy == null)
            return;

        if (_glfw.wl.client.proxy_marshal_flags != null && _glfw.wl.client.proxy_get_version != null)
        {
            _glfw.wl.client.proxy_marshal_flags(proxy,
                opcode,
                null,
                _glfw.wl.client.proxy_get_version(proxy),
                WL_MARSHAL_FLAG_DESTROY);
            return;
        }

        if (_glfw.wl.client.proxy_marshal != null && _glfw.wl.client.proxy_destroy != null)
        {
            _glfw.wl.client.proxy_marshal(proxy, opcode);
            _glfw.wl.client.proxy_destroy(proxy);
            return;
        }

        if (_glfw.wl.client.proxy_destroy != null)
            _glfw.wl.client.proxy_destroy(proxy);
    }

    static void wayland_proxyDestroy(void* proxy)
    {
        if (proxy != null && _glfw.wl.client.proxy_destroy != null)
            _glfw.wl.client.proxy_destroy(proxy);
    }

    [UnmanagedCallersOnly]
    static void wayland_registryHandleGlobal(void* userData,
                                             void* registry,
                                             uint name,
                                             byte* interfaceName,
                                             uint version)
    {
        if (wayland_stringEquals(interfaceName, "wl_compositor") != 0)
        {
            if (_glfw.wl.compositor == null)
            {
                _glfw.wl.compositor = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.compositorInterface,
                    _glfwWaylandWlCompositor,
                    wayland_min(version, 3));
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_subcompositor") != 0)
        {
            if (_glfw.wl.subcompositor == null)
            {
                _glfw.wl.subcompositor = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.subcompositorInterface,
                    _glfwWaylandWlSubcompositor,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_shm") != 0)
        {
            if (_glfw.wl.shm == null)
            {
                _glfw.wl.shm = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.shmInterface,
                    _glfwWaylandWlShm,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_output") != 0)
        {
            _glfwAddOutputWayland(name, version);
        }
        else if (wayland_stringEquals(interfaceName, "wl_seat") != 0)
        {
            if (_glfw.wl.seat == null)
            {
                _glfw.wl.seat = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.seatInterface,
                    _glfwWaylandWlSeat,
                    wayland_min(version, 4));
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_data_device_manager") != 0)
        {
            if (_glfw.wl.dataDeviceManager == null)
            {
                _glfw.wl.dataDeviceManager = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.dataDeviceManagerInterface,
                    _glfwWaylandWlDataDeviceManager,
                    1);
            }
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_registryHandleGlobalRemove(void* userData, void* registry, uint name)
    {
        for (var i = 0; i < _glfw.monitorCount; i++)
        {
            var monitor = _glfw.monitors[i];
            if (monitor->wl.name == name)
            {
                _glfwInputMonitor(monitor, GLFW_DISCONNECTED, 0);
                return;
            }
        }
    }

    static int _glfwConnectWayland(int platformID, _GLFWplatform* platform)
    {
        if (!OperatingSystem.IsLinux())
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not available on this system");
            return GLFW_FALSE;
        }

        if (platformID != GLFW_PLATFORM_WAYLAND &&
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return GLFW_FALSE;
        }

        var module = wayland_loadModule("libwayland-client.so.0");
        if (module == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client");
            return GLFW_FALSE;
        }

        var wl_display_connect =
            (delegate* unmanaged<byte*, void*>)wayland_getModuleSymbol(module, "wl_display_connect");
        if (wl_display_connect == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client entry point");
            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        var display = wl_display_connect(null);
        if (display == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to connect to display");
            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        _glfw.wl.display = display;
        _glfw.wl.client.handle = module;
        _glfw.wl.client.display_connect = wl_display_connect;
        wayland_setPlatformCallbacks(platform);
        return GLFW_TRUE;
    }

    static int _glfwInitWayland()
    {
        _glfw.wl.keyRepeatTimerfd = -1;
        _glfw.wl.cursorTimerfd = -1;
        _glfw.wl.tag = glfwGetVersionString();

        _glfw.wl.client.display_flush =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_flush");
        _glfw.wl.client.display_cancel_read =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_cancel_read");
        _glfw.wl.client.display_dispatch_pending =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_dispatch_pending");
        _glfw.wl.client.display_read_events =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_read_events");
        _glfw.wl.client.display_disconnect =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_disconnect");
        _glfw.wl.client.display_roundtrip =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_roundtrip");
        _glfw.wl.client.display_get_fd =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_get_fd");
        _glfw.wl.client.display_prepare_read =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_prepare_read");
        _glfw.wl.client.proxy_marshal =
            (delegate* unmanaged<void*, uint, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_add_listener =
            (delegate* unmanaged<void*, void*, void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_add_listener");
        _glfw.wl.client.proxy_destroy =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_destroy");
        _glfw.wl.client.proxy_marshal_constructor =
            (delegate* unmanaged<void*, uint, void*, void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_versioned =
            (delegate* unmanaged<void*, uint, void*, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor_versioned");
        _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind =
            (delegate* unmanaged<void*, uint, void*, uint, uint, byte*, uint, void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor_versioned");
        _glfw.wl.client.proxy_get_user_data =
            (delegate* unmanaged<void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_user_data");
        _glfw.wl.client.proxy_set_user_data =
            (delegate* unmanaged<void*, void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_set_user_data");
        _glfw.wl.client.proxy_get_tag =
            (delegate* unmanaged<void*, byte**>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_tag");
        _glfw.wl.client.proxy_set_tag =
            (delegate* unmanaged<void*, byte**, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_set_tag");
        _glfw.wl.client.proxy_get_version =
            (delegate* unmanaged<void*, uint>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_version");
        _glfw.wl.client.proxy_marshal_flags =
            (delegate* unmanaged<void*, uint, void*, uint, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_flags");
        _glfw.wl.client.registryInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_registry_interface");
        _glfw.wl.client.compositorInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_compositor_interface");
        _glfw.wl.client.subcompositorInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_subcompositor_interface");
        _glfw.wl.client.shmInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_shm_interface");
        _glfw.wl.client.seatInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_seat_interface");
        _glfw.wl.client.outputInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_output_interface");
        _glfw.wl.client.dataDeviceManagerInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_data_device_manager_interface");
        _glfw.wl.client.surfaceInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_surface_interface");

        if (_glfw.wl.client.display_flush == null ||
            _glfw.wl.client.display_cancel_read == null ||
            _glfw.wl.client.display_dispatch_pending == null ||
            _glfw.wl.client.display_read_events == null ||
            _glfw.wl.client.display_disconnect == null ||
            _glfw.wl.client.display_roundtrip == null ||
            _glfw.wl.client.display_get_fd == null ||
            _glfw.wl.client.display_prepare_read == null ||
            _glfw.wl.client.proxy_marshal == null ||
            _glfw.wl.client.proxy_add_listener == null ||
            _glfw.wl.client.proxy_destroy == null ||
            _glfw.wl.client.proxy_marshal_constructor == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind == null ||
            _glfw.wl.client.proxy_get_user_data == null ||
            _glfw.wl.client.proxy_set_user_data == null ||
            _glfw.wl.client.proxy_get_tag == null ||
            _glfw.wl.client.proxy_set_tag == null ||
            _glfw.wl.client.registryInterface == null ||
            _glfw.wl.client.compositorInterface == null ||
            _glfw.wl.client.subcompositorInterface == null ||
            _glfw.wl.client.shmInterface == null ||
            _glfw.wl.client.seatInterface == null ||
            _glfw.wl.client.outputInterface == null ||
            _glfw.wl.client.dataDeviceManagerInterface == null ||
            _glfw.wl.client.surfaceInterface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client entry point");
            return GLFW_FALSE;
        }

        _glfw.wl.cursor.handle = wayland_loadModule("libwayland-cursor.so.0");
        if (_glfw.wl.cursor.handle != null)
        {
            _glfw.wl.cursor.theme_load =
                (delegate* unmanaged<byte*, int, void*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_load");
            _glfw.wl.cursor.theme_destroy =
                (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_destroy");
            _glfw.wl.cursor.theme_get_cursor =
                (delegate* unmanaged<void*, byte*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_get_cursor");
            _glfw.wl.cursor.image_get_buffer =
                (delegate* unmanaged<void*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_image_get_buffer");
        }

        _glfw.wl.egl.handle = wayland_loadModule("libwayland-egl.so.1");
        if (_glfw.wl.egl.handle != null)
        {
            _glfw.wl.egl.window_create =
                (delegate* unmanaged<void*, int, int, void*>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_create");
            _glfw.wl.egl.window_destroy =
                (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_destroy");
            _glfw.wl.egl.window_resize =
                (delegate* unmanaged<void*, int, int, int, int, void>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_resize");
        }

        _glfw.wl.xkb.handle = wayland_loadModule("libxkbcommon.so.0");

        _glfw.wl.registry = wayland_displayGetRegistry(_glfw.wl.display);
        if (_glfw.wl.registry == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create display registry");
            return GLFW_FALSE;
        }

        var registryListener = wayland_getRegistryListener();
        if (registryListener == null ||
            _glfw.wl.client.proxy_add_listener(_glfw.wl.registry, registryListener, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add registry listener");
            return GLFW_FALSE;
        }

        if (_glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0 ||
            _glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to process initial registry events");
            return GLFW_FALSE;
        }

        if (_glfw.wl.compositor == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to find wl_compositor in your compositor");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateWayland()
    {
        _glfwTerminateEGL();
        _glfwTerminateOSMesa();

        if (_glfw.wl.cursorTheme != null && _glfw.wl.cursor.theme_destroy != null)
            _glfw.wl.cursor.theme_destroy(_glfw.wl.cursorTheme);
        if (_glfw.wl.cursorThemeHiDPI != null && _glfw.wl.cursor.theme_destroy != null)
            _glfw.wl.cursor.theme_destroy(_glfw.wl.cursorThemeHiDPI);
        if (_glfw.wl.cursor.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.cursor.handle);
        if (_glfw.wl.egl.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.egl.handle);
        if (_glfw.wl.xkb.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.xkb.handle);

        wayland_proxyDestroy(_glfw.wl.dataDeviceManager);
        wayland_proxyDestroy(_glfw.wl.seat);
        wayland_proxyDestroy(_glfw.wl.shm);
        wayland_proxyDestroyWithOpcode(_glfw.wl.subcompositor, 0);
        wayland_proxyDestroy(_glfw.wl.compositor);
        wayland_proxyDestroy(_glfw.wl.registry);

        if (_glfw.wl.display != null && _glfw.wl.client.display_disconnect != null)
            _glfw.wl.client.display_disconnect(_glfw.wl.display);
        if (_glfw.wl.client.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.client.handle);

        _glfw_free(_glfw.wl.clipboardString);
        _glfw_free(_glfw.wl.offers);
        _glfw_free(_glfwWaylandRegistryListener);
        _glfw_free(_glfwWaylandOutputListener);
        _glfwWaylandRegistryListener = null;
        _glfwWaylandOutputListener = null;
        _glfw.wl = default;
    }
}
