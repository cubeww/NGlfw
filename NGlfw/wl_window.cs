using System.Runtime.InteropServices;

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

    struct wl_surface_listener
    {
        public delegate* unmanaged<void*, void*, void*, void> enter;
        public delegate* unmanaged<void*, void*, void*, void> leave;
        public delegate* unmanaged<void*, void*, int, void> preferred_buffer_scale;
        public delegate* unmanaged<void*, void*, uint, void> preferred_buffer_transform;
    }

    struct xdg_surface_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> configure;
    }

    struct xdg_toplevel_listener
    {
        public delegate* unmanaged<void*, void*, int, int, void*, void> configure;
        public delegate* unmanaged<void*, void*, void> close;
    }

    static wl_surface_listener* _glfwWaylandSurfaceListener;
    static xdg_surface_listener* _glfwWaylandXdgSurfaceListener;
    static xdg_toplevel_listener* _glfwWaylandXdgToplevelListener;

    static wl_surface_listener* wayland_getSurfaceListener()
    {
        if (_glfwWaylandSurfaceListener == null)
        {
            _glfwWaylandSurfaceListener = (wl_surface_listener*)_glfw_calloc(1, (nuint)sizeof(wl_surface_listener));
            if (_glfwWaylandSurfaceListener != null)
            {
                _glfwWaylandSurfaceListener->enter = &wayland_surfaceHandleEnter;
                _glfwWaylandSurfaceListener->leave = &wayland_surfaceHandleLeave;
                _glfwWaylandSurfaceListener->preferred_buffer_scale = &wayland_surfaceHandlePreferredBufferScale;
                _glfwWaylandSurfaceListener->preferred_buffer_transform = &wayland_surfaceHandlePreferredBufferTransform;
            }
        }

        return _glfwWaylandSurfaceListener;
    }

    static xdg_surface_listener* wayland_getXdgSurfaceListener()
    {
        if (_glfwWaylandXdgSurfaceListener == null)
        {
            _glfwWaylandXdgSurfaceListener = (xdg_surface_listener*)_glfw_calloc(1, (nuint)sizeof(xdg_surface_listener));
            if (_glfwWaylandXdgSurfaceListener != null)
                _glfwWaylandXdgSurfaceListener->configure = &wayland_xdgSurfaceHandleConfigure;
        }

        return _glfwWaylandXdgSurfaceListener;
    }

    static xdg_toplevel_listener* wayland_getXdgToplevelListener()
    {
        if (_glfwWaylandXdgToplevelListener == null)
        {
            _glfwWaylandXdgToplevelListener = (xdg_toplevel_listener*)_glfw_calloc(1, (nuint)sizeof(xdg_toplevel_listener));
            if (_glfwWaylandXdgToplevelListener != null)
            {
                _glfwWaylandXdgToplevelListener->configure = &wayland_xdgToplevelHandleConfigure;
                _glfwWaylandXdgToplevelListener->close = &wayland_xdgToplevelHandleClose;
            }
        }

        return _glfwWaylandXdgToplevelListener;
    }

    static void _glfwUpdateBufferScaleFromOutputsWayland(_GLFWwindow* window)
    {
        if (window->wl.surface == null ||
            _glfw.wl.client.proxy_marshal_int == null ||
            _glfw.wl.client.proxy_get_version == null ||
            _glfw.wl.client.proxy_get_version(window->wl.surface) < WL_SURFACE_SET_BUFFER_SCALE_SINCE_VERSION)
        {
            return;
        }

        var scale = 1;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
            scale = _glfw_max(scale, window->wl.outputScales[i].factor);

        if (window->wl.scaleFramebuffer == 0 || window->wl.fractionalScale != null)
            return;

        if (window->wl.bufferScale == scale)
            return;

        window->wl.bufferScale = scale;
        _glfw.wl.client.proxy_marshal_int(window->wl.surface, WL_SURFACE_SET_BUFFER_SCALE, scale);

        window->wl.fbWidth = window->wl.width * scale;
        window->wl.fbHeight = window->wl.height * scale;

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        _glfwInputWindowContentScale(window, scale, scale);
        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    static void wayland_surfaceCommit(void* surface)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(surface, WL_SURFACE_COMMIT);
    }

    static int wayland_proxyHasTag(void* proxy)
    {
        if (proxy == null || _glfw.wl.client.proxy_get_tag == null)
            return GLFW_FALSE;

        fixed (_GLFWlibrary* glfw = &_glfw)
            return _glfw.wl.client.proxy_get_tag(proxy) == &glfw->wl.tag ? GLFW_TRUE : GLFW_FALSE;
    }

    static void wayland_surfaceAddOutputScale(_GLFWwindow* window, void* output, int factor)
    {
        if (window == null || output == null)
            return;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
        {
            if (window->wl.outputScales[i].output == output)
                return;
        }

        if (window->wl.outputScaleCount + 1 > window->wl.outputScaleSize)
        {
            var size = window->wl.outputScaleSize + 1;
            var scales = (_GLFWscaleWayland*)_glfw_realloc(window->wl.outputScales,
                (nuint)(size * (nuint)sizeof(_GLFWscaleWayland)));
            if (scales == null)
                return;

            window->wl.outputScales = scales;
            window->wl.outputScaleSize = size;
        }

        window->wl.outputScales[window->wl.outputScaleCount++] = new _GLFWscaleWayland
        {
            output = output,
            factor = factor
        };

        _glfwUpdateBufferScaleFromOutputsWayland(window);
    }

    static void wayland_surfaceRemoveOutputScale(_GLFWwindow* window, void* output)
    {
        if (window == null || output == null)
            return;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
        {
            if (window->wl.outputScales[i].output == output)
            {
                window->wl.outputScales[i] = window->wl.outputScales[window->wl.outputScaleCount - 1];
                window->wl.outputScaleCount--;
                break;
            }
        }

        _glfwUpdateBufferScaleFromOutputsWayland(window);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandleEnter(void* userData, void* surface, void* output)
    {
        if (wayland_proxyHasTag(output) == 0 || _glfw.wl.client.proxy_get_user_data == null)
            return;

        var window = (_GLFWwindow*)userData;
        var monitor = (_GLFWmonitor*)_glfw.wl.client.proxy_get_user_data(output);
        if (window == null || monitor == null)
            return;

        wayland_surfaceAddOutputScale(window, output, monitor->wl.scale != 0 ? monitor->wl.scale : 1);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandleLeave(void* userData, void* surface, void* output)
    {
        if (wayland_proxyHasTag(output) == 0)
            return;

        wayland_surfaceRemoveOutputScale((_GLFWwindow*)userData, output);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandlePreferredBufferScale(void* userData, void* surface, int factor)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandlePreferredBufferTransform(void* userData, void* surface, uint transform)
    {
    }

    static void* wayland_xdgWmBaseGetXdgSurface(void* wmBase, void* surface)
    {
        if (wmBase == null || surface == null || _glfwWaylandXdgSurfaceInterface == null)
            return null;

        var xdgSurface = _glfw.wl.client.proxy_marshal_constructor_object(wmBase,
            XDG_WM_BASE_GET_XDG_SURFACE,
            _glfwWaylandXdgSurfaceInterface,
            null,
            surface);

        wayland_tagProxy(xdgSurface);
        return xdgSurface;
    }

    static void* wayland_xdgSurfaceGetToplevel(void* xdgSurface)
    {
        if (xdgSurface == null || _glfwWaylandXdgToplevelInterface == null)
            return null;

        var toplevel = _glfw.wl.client.proxy_marshal_constructor(xdgSurface,
            XDG_SURFACE_GET_TOPLEVEL,
            _glfwWaylandXdgToplevelInterface,
            null);

        wayland_tagProxy(toplevel);
        return toplevel;
    }

    static void wayland_xdgSurfaceAckConfigure(void* xdgSurface, uint serial)
    {
        if (xdgSurface != null && _glfw.wl.client.proxy_marshal_uint != null)
            _glfw.wl.client.proxy_marshal_uint(xdgSurface, XDG_SURFACE_ACK_CONFIGURE, serial);
    }

    static void wayland_xdgSurfaceSetWindowGeometry(void* xdgSurface, int x, int y, int width, int height)
    {
        if (xdgSurface != null && _glfw.wl.client.proxy_marshal_int_int_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int_int_int(xdgSurface, XDG_SURFACE_SET_WINDOW_GEOMETRY, x, y, width, height);
    }

    static void wayland_xdgToplevelSetTitle(void* toplevel, byte* title)
    {
        if (toplevel != null && title != null && _glfw.wl.client.proxy_marshal_string != null)
            _glfw.wl.client.proxy_marshal_string(toplevel, XDG_TOPLEVEL_SET_TITLE, title);
    }

    static void wayland_xdgToplevelSetAppId(void* toplevel, byte* appId)
    {
        if (toplevel != null && appId != null && _glfw.wl.client.proxy_marshal_string != null)
            _glfw.wl.client.proxy_marshal_string(toplevel, XDG_TOPLEVEL_SET_APP_ID, appId);
    }

    static void wayland_xdgToplevelSetMinSize(void* toplevel, int width, int height)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int(toplevel, XDG_TOPLEVEL_SET_MIN_SIZE, width, height);
    }

    static void wayland_xdgToplevelSetMaxSize(void* toplevel, int width, int height)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int(toplevel, XDG_TOPLEVEL_SET_MAX_SIZE, width, height);
    }

    static void wayland_xdgToplevelSetMaximized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_SET_MAXIMIZED);
    }

    static void wayland_xdgToplevelUnsetMaximized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_UNSET_MAXIMIZED);
    }

    static void wayland_xdgToplevelSetFullscreen(void* toplevel, void* output)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_object != null)
            _glfw.wl.client.proxy_marshal_object(toplevel, XDG_TOPLEVEL_SET_FULLSCREEN, output);
    }

    static void wayland_xdgToplevelUnsetFullscreen(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_UNSET_FULLSCREEN);
    }

    static void wayland_xdgToplevelSetMinimized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_SET_MINIMIZED);
    }

    static void wayland_applyPendingSize(_GLFWwindow* window)
    {
        var width = window->wl.pending.width;
        var height = window->wl.pending.height;

        window->wl.pending.width = 0;
        window->wl.pending.height = 0;

        if (width <= 0 || height <= 0)
            return;

        if (width == window->wl.width && height == window->wl.height)
            return;

        window->wl.width = width;
        window->wl.height = height;
        window->wl.fbWidth = width * (window->wl.bufferScale != 0 ? window->wl.bufferScale : 1);
        window->wl.fbHeight = height * (window->wl.bufferScale != 0 ? window->wl.bufferScale : 1);

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        wayland_xdgSurfaceSetWindowGeometry(window->wl.xdg.surface, 0, 0, width, height);
        wayland_surfaceCommit(window->wl.surface);

        _glfwInputWindowSize(window, width, height);
        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgSurfaceHandleConfigure(void* userData, void* xdgSurface, uint serial)
    {
        var window = (_GLFWwindow*)userData;

        wayland_xdgSurfaceAckConfigure(xdgSurface, serial);
        wayland_applyPendingSize(window);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgToplevelHandleConfigure(void* userData,
                                                  void* toplevel,
                                                  int width,
                                                  int height,
                                                  void* states)
    {
        var window = (_GLFWwindow*)userData;

        if (width > 0 && height > 0)
        {
            window->wl.pending.width = width;
            window->wl.pending.height = height;
        }
        else
        {
            window->wl.pending.width = window->wl.width;
            window->wl.pending.height = window->wl.height;
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgToplevelHandleClose(void* userData, void* toplevel)
    {
        _glfwInputWindowCloseRequest((_GLFWwindow*)userData);
    }

    static int wayland_createXdgShellObjects(_GLFWwindow* window)
    {
        window->wl.xdg.surface = wayland_xdgWmBaseGetXdgSurface(_glfw.wl.wmBase, window->wl.surface);
        if (window->wl.xdg.surface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create xdg-surface for window");
            return GLFW_FALSE;
        }

        var surfaceListener = wayland_getXdgSurfaceListener();
        if (surfaceListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.xdg.surface, surfaceListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add xdg-surface listener");
            return GLFW_FALSE;
        }

        window->wl.xdg.toplevel = wayland_xdgSurfaceGetToplevel(window->wl.xdg.surface);
        if (window->wl.xdg.toplevel == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create xdg-toplevel for window");
            return GLFW_FALSE;
        }

        var toplevelListener = wayland_getXdgToplevelListener();
        if (toplevelListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.xdg.toplevel, toplevelListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add xdg-toplevel listener");
            return GLFW_FALSE;
        }

        if (window->wl.appId != null)
            wayland_xdgToplevelSetAppId(window->wl.xdg.toplevel, window->wl.appId);

        wayland_xdgToplevelSetTitle(window->wl.xdg.toplevel, window->title);
        wayland_xdgSurfaceSetWindowGeometry(window->wl.xdg.surface, 0, 0, window->wl.width, window->wl.height);

        if (window->monitor != null)
        {
            window->wl.fullscreen = GLFW_TRUE;
            wayland_xdgToplevelSetFullscreen(window->wl.xdg.toplevel, window->monitor->wl.output);
        }
        else if (window->wl.maximized != 0)
            wayland_xdgToplevelSetMaximized(window->wl.xdg.toplevel);

        wayland_surfaceCommit(window->wl.surface);

        if (_glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to process initial xdg-shell events");
            return GLFW_FALSE;
        }

        window->wl.visible = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void wayland_destroyShellObjects(_GLFWwindow* window)
    {
        wayland_proxyDestroyWithOpcode(window->wl.xdg.toplevel, XDG_TOPLEVEL_DESTROY);
        wayland_proxyDestroyWithOpcode(window->wl.xdg.surface, XDG_SURFACE_DESTROY);

        window->wl.xdg.toplevel = null;
        window->wl.xdg.surface = null;
        window->wl.xdg.decoration = null;
        window->wl.xdg.decorationMode = 0;
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

        window->wl.maximized = wndconfig->maximized;

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

        var surfaceListener = wayland_getSurfaceListener();
        if (surfaceListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.surface, surfaceListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add surface listener");
            return GLFW_FALSE;
        }

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API ||
                ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                if (_glfw.wl.egl.window_create == null)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-egl entry point");
                    return GLFW_FALSE;
                }

                window->wl.egl.window = _glfw.wl.egl.window_create(window->wl.surface,
                    window->wl.fbWidth,
                    window->wl.fbHeight);
                if (window->wl.egl.window == null)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create EGL window");
                    return GLFW_FALSE;
                }

                if (_glfwInitEGL() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextEGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_OSMESA_CONTEXT_API)
            {
                if (_glfwInitOSMesa() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextOSMesa(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }

            if (_glfwRefreshContextAttribs(window, ctxconfig) == 0)
                return GLFW_FALSE;
        }

        if (wndconfig->mousePassthrough != 0)
            _glfwSetWindowMousePassthroughWayland(window, GLFW_TRUE);

        if (window->monitor != null || wndconfig->visible != 0)
        {
            if (wayland_createXdgShellObjects(window) == 0)
                return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowWayland(_GLFWwindow* window)
    {
        if (window == _glfw.wl.pointerFocus)
            _glfw.wl.pointerFocus = null;
        if (window == _glfw.wl.keyboardFocus)
            _glfw.wl.keyboardFocus = null;

        if (window->context.destroy != null)
            window->context.destroy(window);

        wayland_destroyShellObjects(window);

        if (window->wl.egl.window != null && _glfw.wl.egl.window_destroy != null)
            _glfw.wl.egl.window_destroy(window->wl.egl.window);

        wayland_proxyDestroyWithOpcode(window->wl.surface, WL_SURFACE_DESTROY);

        _glfw_free(window->wl.appId);
        _glfw_free(window->wl.outputScales);
        window->wl = default;
    }

    static void _glfwSetWindowTitleWayland(_GLFWwindow* window, byte* title)
    {
        wayland_xdgToplevelSetTitle(window->wl.xdg.toplevel, title);
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
        if (minwidth == GLFW_DONT_CARE || minheight == GLFW_DONT_CARE)
            wayland_xdgToplevelSetMinSize(window->wl.xdg.toplevel, 0, 0);
        else
            wayland_xdgToplevelSetMinSize(window->wl.xdg.toplevel, minwidth, minheight);

        if (maxwidth == GLFW_DONT_CARE || maxheight == GLFW_DONT_CARE)
            wayland_xdgToplevelSetMaxSize(window->wl.xdg.toplevel, 0, 0);
        else
            wayland_xdgToplevelSetMaxSize(window->wl.xdg.toplevel, maxwidth, maxheight);
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
        wayland_xdgToplevelSetMinimized(window->wl.xdg.toplevel);
    }

    static void _glfwRestoreWindowWayland(_GLFWwindow* window)
    {
        if (window->wl.maximized != 0)
            wayland_xdgToplevelUnsetMaximized(window->wl.xdg.toplevel);

        window->wl.maximized = GLFW_FALSE;
    }

    static void _glfwMaximizeWindowWayland(_GLFWwindow* window)
    {
        window->wl.maximized = GLFW_TRUE;
        wayland_xdgToplevelSetMaximized(window->wl.xdg.toplevel);
    }

    static void _glfwShowWindowWayland(_GLFWwindow* window)
    {
        if (window->wl.xdg.surface == null &&
            wayland_createXdgShellObjects(window) == 0)
        {
            return;
        }

        window->wl.visible = GLFW_TRUE;
    }

    static void _glfwHideWindowWayland(_GLFWwindow* window)
    {
        wayland_destroyShellObjects(window);
        wayland_surfaceCommit(window->wl.surface);
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
        {
            if (window->wl.fullscreen != 0)
                wayland_xdgToplevelUnsetFullscreen(window->wl.xdg.toplevel);
            window->wl.fullscreen = GLFW_FALSE;
            _glfwSetWindowSizeWayland(window, width, height);
        }
        else
        {
            window->wl.fullscreen = GLFW_TRUE;
            wayland_xdgToplevelSetFullscreen(window->wl.xdg.toplevel, monitor->wl.output);
        }
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
