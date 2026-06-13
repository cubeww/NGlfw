namespace NGlfw;

public static unsafe partial class Glfw
{
    const int VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR = 1000006000;

    struct VkWaylandSurfaceCreateInfoKHR
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* display;
        public void* surface;
    }

    static void _glfwUpdateBufferScaleFromOutputsWayland(_GLFWwindow* window)
    {
        var scale = 1;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
            scale = _glfw_max(scale, window->wl.outputScales[i].factor);

        if (window->wl.scaleFramebuffer == 0)
            scale = 1;

        if (window->wl.bufferScale == scale)
            return;

        window->wl.bufferScale = scale;
        window->wl.fbWidth = window->wl.width * scale;
        window->wl.fbHeight = window->wl.height * scale;

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        _glfwInputWindowContentScale(window, scale, scale);
        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    static int _glfwCreateWindowWayland(_GLFWwindow* window,
                                        _GLFWwndconfig* wndconfig,
                                        _GLFWctxconfig* ctxconfig,
                                        _GLFWfbconfig* fbconfig)
    {
        window->wl.width = wndconfig->width;
        window->wl.height = wndconfig->height;
        window->wl.fbWidth = wndconfig->width;
        window->wl.fbHeight = wndconfig->height;
        window->wl.transparent = fbconfig->transparent;
        window->wl.scaleFramebuffer = wndconfig->scaleFramebuffer;
        window->wl.bufferScale = 1;
        window->wl.scalingNumerator = 120;

        if (wndconfig->wl.appId[0] != 0)
            window->wl.appId = _glfw_strdup(wndconfig->wl.appId);

        if (_glfw.wl.compositor == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: No wl_compositor available");
            return GLFW_FALSE;
        }

        window->wl.surface = wayland_compositorCreateSurface();
        if (window->wl.surface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create window surface");
            return GLFW_FALSE;
        }

        _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED,
            "Wayland: xdg-shell window creation has not yet been ported");
        return GLFW_FALSE;
    }

    static void _glfwDestroyWindowWayland(_GLFWwindow* window)
    {
        if (window == _glfw.wl.pointerFocus)
            _glfw.wl.pointerFocus = null;
        if (window == _glfw.wl.keyboardFocus)
            _glfw.wl.keyboardFocus = null;

        if (window->context.destroy != null)
            window->context.destroy(window);

        if (window->wl.egl.window != null && _glfw.wl.egl.window_destroy != null)
            _glfw.wl.egl.window_destroy(window->wl.egl.window);

        wayland_proxyDestroyWithOpcode(window->wl.surface, WL_SURFACE_DESTROY);

        _glfw_free(window->wl.appId);
        _glfw_free(window->wl.outputScales);
        window->wl = default;
    }

    static void _glfwSetWindowTitleWayland(_GLFWwindow* window, byte* title)
    {
    }

    static void _glfwSetWindowIconWayland(_GLFWwindow* window, int count, GLFWimage* images)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window icon");
    }

    static void _glfwGetWindowPosWayland(_GLFWwindow* window, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;

        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not provide the window position");
    }

    static void _glfwSetWindowPosWayland(_GLFWwindow* window, int xpos, int ypos)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window position");
    }

    static void _glfwGetWindowSizeWayland(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->wl.width;
        if (height != null)
            *height = window->wl.height;
    }

    static void _glfwSetWindowSizeWayland(_GLFWwindow* window, int width, int height)
    {
        window->wl.width = width;
        window->wl.height = height;
        window->wl.fbWidth = width * (window->wl.bufferScale != 0 ? window->wl.bufferScale : 1);
        window->wl.fbHeight = height * (window->wl.bufferScale != 0 ? window->wl.bufferScale : 1);

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        _glfwInputWindowSize(window, width, height);
        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    static void _glfwSetWindowSizeLimitsWayland(_GLFWwindow* window,
                                                int minwidth,
                                                int minheight,
                                                int maxwidth,
                                                int maxheight)
    {
    }

    static void _glfwSetWindowAspectRatioWayland(_GLFWwindow* window, int numer, int denom)
    {
    }

    static void _glfwGetFramebufferSizeWayland(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->wl.fbWidth;
        if (height != null)
            *height = window->wl.fbHeight;
    }

    static void _glfwGetWindowFrameSizeWayland(_GLFWwindow* window,
                                               int* left,
                                               int* top,
                                               int* right,
                                               int* bottom)
    {
        if (left != null)
            *left = 0;
        if (top != null)
            *top = 0;
        if (right != null)
            *right = 0;
        if (bottom != null)
            *bottom = 0;
    }

    static void _glfwGetWindowContentScaleWayland(_GLFWwindow* window, float* xscale, float* yscale)
    {
        var scale = window->wl.fractionalScale != null
            ? window->wl.scalingNumerator / 120f
            : window->wl.bufferScale != 0 ? window->wl.bufferScale : 1f;

        if (xscale != null)
            *xscale = scale;
        if (yscale != null)
            *yscale = scale;
    }

    static void _glfwIconifyWindowWayland(_GLFWwindow* window)
    {
    }

    static void _glfwRestoreWindowWayland(_GLFWwindow* window)
    {
        window->wl.maximized = GLFW_FALSE;
    }

    static void _glfwMaximizeWindowWayland(_GLFWwindow* window)
    {
        window->wl.maximized = GLFW_TRUE;
    }

    static void _glfwShowWindowWayland(_GLFWwindow* window)
    {
        window->wl.visible = GLFW_TRUE;
    }

    static void _glfwHideWindowWayland(_GLFWwindow* window)
    {
        window->wl.visible = GLFW_FALSE;
    }

    static void _glfwRequestWindowAttentionWayland(_GLFWwindow* window)
    {
    }

    static void _glfwFocusWindowWayland(_GLFWwindow* window)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the input focus");
    }

    static void _glfwSetWindowMonitorWayland(_GLFWwindow* window,
                                             _GLFWmonitor* monitor,
                                             int xpos,
                                             int ypos,
                                             int width,
                                             int height,
                                             int refreshRate)
    {
        _glfwInputWindowMonitor(window, monitor);

        if (monitor == null)
            _glfwSetWindowSizeWayland(window, width, height);
        else
            window->wl.fullscreen = GLFW_TRUE;
    }

    static int _glfwWindowFocusedWayland(_GLFWwindow* window)
    {
        return _glfw.wl.keyboardFocus == window ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowIconifiedWayland(_GLFWwindow* window)
    {
        return GLFW_FALSE;
    }

    static int _glfwWindowVisibleWayland(_GLFWwindow* window)
    {
        return window->wl.visible;
    }

    static int _glfwWindowMaximizedWayland(_GLFWwindow* window)
    {
        return window->wl.maximized;
    }

    static int _glfwWindowHoveredWayland(_GLFWwindow* window)
    {
        return window->wl.hovered;
    }

    static int _glfwFramebufferTransparentWayland(_GLFWwindow* window)
    {
        return window->wl.transparent;
    }

    static void _glfwSetWindowResizableWayland(_GLFWwindow* window, int enabled)
    {
        window->resizable = enabled;
    }

    static void _glfwSetWindowDecoratedWayland(_GLFWwindow* window, int enabled)
    {
        window->decorated = enabled;
    }

    static void _glfwSetWindowFloatingWayland(_GLFWwindow* window, int enabled)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: Platform does not support making a window floating");
    }

    static void _glfwSetWindowMousePassthroughWayland(_GLFWwindow* window, int enabled)
    {
        window->mousePassthrough = enabled;
    }

    static float _glfwGetWindowOpacityWayland(_GLFWwindow* window)
    {
        return 1f;
    }

    static void _glfwSetWindowOpacityWayland(_GLFWwindow* window, float opacity)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window opacity");
    }

    static void _glfwSetRawMouseMotionWayland(_GLFWwindow* window, int enabled)
    {
    }

    static int _glfwRawMouseMotionSupportedWayland()
    {
        return _glfw.wl.relativePointerManager != null ? GLFW_TRUE : GLFW_FALSE;
    }

    static void _glfwPollEventsWayland()
    {
        if (_glfw.wl.client.display_dispatch_pending != null)
            _glfw.wl.client.display_dispatch_pending(_glfw.wl.display);
        if (_glfw.wl.client.display_flush != null)
            _glfw.wl.client.display_flush(_glfw.wl.display);
    }

    static void _glfwWaitEventsWayland()
    {
        _glfwPollEventsWayland();
    }

    static void _glfwWaitEventsTimeoutWayland(double timeout)
    {
        _glfwPollEventsWayland();
    }

    static void _glfwPostEmptyEventWayland()
    {
        if (_glfw.wl.client.display_flush != null)
            _glfw.wl.client.display_flush(_glfw.wl.display);
    }

    static void _glfwGetCursorPosWayland(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (xpos != null)
            *xpos = window->wl.cursorPosX;
        if (ypos != null)
            *ypos = window->wl.cursorPosY;
    }

    static void _glfwSetCursorPosWayland(_GLFWwindow* window, double x, double y)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the cursor position");
    }

    static void _glfwSetCursorModeWayland(_GLFWwindow* window, int mode)
    {
        _glfwSetCursorWayland(window, window->wl.currentCursor);
    }

    static byte* _glfwGetScancodeNameWayland(int scancode)
    {
        if (scancode < 0 || scancode > 255)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Wayland: Invalid scancode {0}", scancode);
            return null;
        }

        return null;
    }

    static int _glfwGetKeyScancodeWayland(int key)
    {
        return key >= 0 && key <= GLFW_KEY_LAST ? _glfw.wl.scancodes[key] : -1;
    }

    static int _glfwCreateCursorWayland(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        cursor->wl.width = image->width;
        cursor->wl.height = image->height;
        cursor->wl.xhot = xhot;
        cursor->wl.yhot = yhot;
        return GLFW_TRUE;
    }

    static int _glfwCreateStandardCursorWayland(_GLFWcursor* cursor, int shape)
    {
        return GLFW_TRUE;
    }

    static void _glfwDestroyCursorWayland(_GLFWcursor* cursor)
    {
        if (cursor->wl.buffer != null && _glfw.wl.client.proxy_destroy != null)
            _glfw.wl.client.proxy_destroy(cursor->wl.buffer);
        cursor->wl = default;
    }

    static void _glfwSetCursorWayland(_GLFWwindow* window, _GLFWcursor* cursor)
    {
        window->wl.currentCursor = cursor;
    }

    static void _glfwSetClipboardStringWayland(byte* @string)
    {
        _glfw_free(_glfw.wl.clipboardString);
        _glfw.wl.clipboardString = _glfw_strdup(@string);
    }

    static byte* _glfwGetClipboardStringWayland()
    {
        if (_glfw.wl.clipboardString == null)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "Wayland: No clipboard data available");
            return null;
        }

        return _glfw.wl.clipboardString;
    }

    static int _glfwGetEGLPlatformWayland(int** attribs)
    {
        if (attribs != null)
            *attribs = null;

        if (_glfw.egl.EXT_platform_base != 0 && _glfw.egl.EXT_platform_wayland != 0)
            return EGL_PLATFORM_WAYLAND_EXT;

        return 0;
    }

    static void* _glfwGetEGLNativeDisplayWayland()
    {
        return _glfw.wl.display;
    }

    static void* _glfwGetEGLNativeWindowWayland(_GLFWwindow* window)
    {
        return window->wl.egl.window;
    }

    static void _glfwGetRequiredInstanceExtensionsWayland(byte** extensions)
    {
        if (_glfw.vk.KHR_surface == 0 || _glfw.vk.KHR_wayland_surface == 0)
            return;

        extensions[0] = _glfwVkKHRSurfaceExtensionName;
        extensions[1] = _glfwVkKHRWaylandSurfaceExtensionName;
    }

    static int _glfwGetPhysicalDevicePresentationSupportWayland(void* instance, void* device, uint queuefamily)
    {
        var proc = (delegate* unmanaged<void*, uint, void*, int>)
            vulkan_getInstanceProcAddress(instance, "vkGetPhysicalDeviceWaylandPresentationSupportKHR");
        if (proc == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Wayland: Vulkan instance missing VK_KHR_wayland_surface extension");
            return GLFW_FALSE;
        }

        return proc(device, queuefamily, _glfw.wl.display);
    }

    static int _glfwCreateWindowSurfaceWayland(void* instance, _GLFWwindow* window, void* allocator, ulong* surface)
    {
        var vkCreateWaylandSurfaceKHR = (delegate* unmanaged<void*, VkWaylandSurfaceCreateInfoKHR*, void*, ulong*, int>)
            vulkan_getInstanceProcAddress(instance, "vkCreateWaylandSurfaceKHR");
        if (vkCreateWaylandSurfaceKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Wayland: Vulkan instance missing VK_KHR_wayland_surface extension");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        VkWaylandSurfaceCreateInfoKHR sci = default;
        sci.sType = VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR;
        sci.display = _glfw.wl.display;
        sci.surface = window->wl.surface;

        var err = vkCreateWaylandSurfaceKHR(instance, &sci, allocator, surface);
        if (err != VK_SUCCESS)
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create Vulkan surface: {0}", vulkan_resultString(err));

        return err;
    }

    public static void* glfwGetWaylandDisplay()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WAYLAND)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not initialized");
            return null;
        }

        return _glfw.wl.display;
    }

    public static void* glfwGetWaylandWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WAYLAND)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not initialized");
            return null;
        }

        return ((_GLFWwindow*)window)->wl.surface;
    }
}
