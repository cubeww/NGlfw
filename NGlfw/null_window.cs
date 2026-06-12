namespace NGlfw;

public static unsafe partial class Glfw
{
    static void null_applySizeLimits(_GLFWwindow* window, int* width, int* height)
    {
        if (window->numer != GLFW_DONT_CARE && window->denom != GLFW_DONT_CARE)
        {
            var ratio = (float)window->numer / window->denom;
            *height = (int)(*width / ratio);
        }

        if (window->minwidth != GLFW_DONT_CARE)
            *width = _glfw_max(*width, window->minwidth);
        else if (window->maxwidth != GLFW_DONT_CARE)
            *width = _glfw_min(*width, window->maxwidth);

        if (window->minheight != GLFW_DONT_CARE)
            *height = _glfw_min(*height, window->minheight);
        else if (window->maxheight != GLFW_DONT_CARE)
            *height = _glfw_max(*height, window->maxheight);
    }

    static void null_fitToMonitor(_GLFWwindow* window)
    {
        GLFWvidmode mode;
        _glfwGetVideoModeNull(window->monitor, &mode);
        _glfwGetMonitorPosNull(window->monitor, &window->@null.xpos, &window->@null.ypos);
        window->@null.width = mode.width;
        window->@null.height = mode.height;
    }

    static void null_acquireMonitor(_GLFWwindow* window)
    {
        _glfwInputMonitorWindow(window->monitor, window);
    }

    static void null_releaseMonitor(_GLFWwindow* window)
    {
        if (window->monitor->window != window)
            return;

        _glfwInputMonitorWindow(window->monitor, null);
    }

    static int null_createNativeWindow(_GLFWwindow* window,
                                       _GLFWwndconfig* wndconfig,
                                       _GLFWfbconfig* fbconfig)
    {
        if (window->monitor != null)
            null_fitToMonitor(window);
        else
        {
            if (wndconfig->xpos == GLFW_ANY_POSITION && wndconfig->ypos == GLFW_ANY_POSITION)
            {
                window->@null.xpos = 17;
                window->@null.ypos = 17;
            }
            else
            {
                window->@null.xpos = wndconfig->xpos;
                window->@null.ypos = wndconfig->ypos;
            }

            window->@null.width = wndconfig->width;
            window->@null.height = wndconfig->height;
        }

        window->@null.visible = wndconfig->visible;
        window->@null.decorated = wndconfig->decorated;
        window->@null.maximized = wndconfig->maximized;
        window->@null.floating = wndconfig->floating;
        window->@null.transparent = fbconfig->transparent;
        window->@null.opacity = 1f;

        return GLFW_TRUE;
    }

    static int _glfwCreateWindowNull(_GLFWwindow* window,
                                     _GLFWwndconfig* wndconfig,
                                     _GLFWctxconfig* ctxconfig,
                                     _GLFWfbconfig* fbconfig)
    {
        if (null_createNativeWindow(window, wndconfig, fbconfig) == 0)
            return GLFW_FALSE;

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API ||
                ctxconfig->source == GLFW_OSMESA_CONTEXT_API)
            {
                if (_glfwInitOSMesa() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextOSMesa(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                if (_glfwInitEGL() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextEGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }

            if (_glfwRefreshContextAttribs(window, ctxconfig) == 0)
                return GLFW_FALSE;
        }

        if (wndconfig->mousePassthrough != 0)
            _glfwSetWindowMousePassthroughNull(window, GLFW_TRUE);

        if (window->monitor != null)
        {
            _glfwShowWindowNull(window);
            _glfwFocusWindowNull(window);
            null_acquireMonitor(window);

            if (wndconfig->centerCursor != 0)
                _glfwCenterCursorInContentArea(window);
        }
        else if (wndconfig->visible != 0)
        {
            _glfwShowWindowNull(window);
            if (wndconfig->focused != 0)
                _glfwFocusWindowNull(window);
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowNull(_GLFWwindow* window)
    {
        if (window->monitor != null)
            null_releaseMonitor(window);

        if (_glfw.@null.focusedWindow == window)
            _glfw.@null.focusedWindow = null;

        if (window->context.destroy != null)
            window->context.destroy(window);
    }

    static void _glfwSetWindowTitleNull(_GLFWwindow* window, byte* title)
    {
    }

    static void _glfwSetWindowIconNull(_GLFWwindow* window, int count, GLFWimage* images)
    {
    }

    static void _glfwSetWindowMonitorNull(_GLFWwindow* window,
                                          _GLFWmonitor* monitor,
                                          int xpos, int ypos,
                                          int width, int height,
                                          int refreshRate)
    {
        if (window->monitor == monitor)
        {
            if (monitor == null)
            {
                _glfwSetWindowPosNull(window, xpos, ypos);
                _glfwSetWindowSizeNull(window, width, height);
            }

            return;
        }

        if (window->monitor != null)
            null_releaseMonitor(window);

        _glfwInputWindowMonitor(window, monitor);

        if (window->monitor != null)
        {
            window->@null.visible = GLFW_TRUE;
            null_acquireMonitor(window);
            null_fitToMonitor(window);
        }
        else
        {
            _glfwSetWindowPosNull(window, xpos, ypos);
            _glfwSetWindowSizeNull(window, width, height);
        }
    }

    static void _glfwGetWindowPosNull(_GLFWwindow* window, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = window->@null.xpos;
        if (ypos != null)
            *ypos = window->@null.ypos;
    }

    static void _glfwSetWindowPosNull(_GLFWwindow* window, int xpos, int ypos)
    {
        if (window->monitor != null)
            return;

        if (window->@null.xpos != xpos || window->@null.ypos != ypos)
        {
            window->@null.xpos = xpos;
            window->@null.ypos = ypos;
            _glfwInputWindowPos(window, xpos, ypos);
        }
    }

    static void _glfwGetWindowSizeNull(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->@null.width;
        if (height != null)
            *height = window->@null.height;
    }

    static void _glfwSetWindowSizeNull(_GLFWwindow* window, int width, int height)
    {
        if (window->monitor != null)
            return;

        if (window->@null.width != width || window->@null.height != height)
        {
            window->@null.width = width;
            window->@null.height = height;
            _glfwInputFramebufferSize(window, width, height);
            _glfwInputWindowDamage(window);
            _glfwInputWindowSize(window, width, height);
        }
    }

    static void _glfwSetWindowSizeLimitsNull(_GLFWwindow* window,
                                             int minwidth, int minheight,
                                             int maxwidth, int maxheight)
    {
        var width = window->@null.width;
        var height = window->@null.height;
        null_applySizeLimits(window, &width, &height);
        _glfwSetWindowSizeNull(window, width, height);
    }

    static void _glfwSetWindowAspectRatioNull(_GLFWwindow* window, int n, int d)
    {
        var width = window->@null.width;
        var height = window->@null.height;
        null_applySizeLimits(window, &width, &height);
        _glfwSetWindowSizeNull(window, width, height);
    }

    static void _glfwGetFramebufferSizeNull(_GLFWwindow* window, int* width, int* height)
    {
        _glfwGetWindowSizeNull(window, width, height);
    }

    static void _glfwGetWindowFrameSizeNull(_GLFWwindow* window,
                                            int* left, int* top,
                                            int* right, int* bottom)
    {
        if (window->@null.decorated != 0 && window->monitor == null)
        {
            if (left != null) *left = 1;
            if (top != null) *top = 10;
            if (right != null) *right = 1;
            if (bottom != null) *bottom = 1;
        }
        else
        {
            if (left != null) *left = 0;
            if (top != null) *top = 0;
            if (right != null) *right = 0;
            if (bottom != null) *bottom = 0;
        }
    }

    static void _glfwGetWindowContentScaleNull(_GLFWwindow* window, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwIconifyWindowNull(_GLFWwindow* window)
    {
        if (_glfw.@null.focusedWindow == window)
        {
            _glfw.@null.focusedWindow = null;
            _glfwInputWindowFocus(window, GLFW_FALSE);
        }

        if (window->@null.iconified == 0)
        {
            window->@null.iconified = GLFW_TRUE;
            _glfwInputWindowIconify(window, GLFW_TRUE);

            if (window->monitor != null)
                null_releaseMonitor(window);
        }
    }

    static void _glfwRestoreWindowNull(_GLFWwindow* window)
    {
        if (window->@null.iconified != 0)
        {
            window->@null.iconified = GLFW_FALSE;
            _glfwInputWindowIconify(window, GLFW_FALSE);

            if (window->monitor != null)
                null_acquireMonitor(window);
        }
        else if (window->@null.maximized != 0)
        {
            window->@null.maximized = GLFW_FALSE;
            _glfwInputWindowMaximize(window, GLFW_FALSE);
        }
    }

    static void _glfwMaximizeWindowNull(_GLFWwindow* window)
    {
        if (window->@null.maximized == 0)
        {
            window->@null.maximized = GLFW_TRUE;
            _glfwInputWindowMaximize(window, GLFW_TRUE);
        }
    }

    static int _glfwWindowMaximizedNull(_GLFWwindow* window)
    {
        return window->@null.maximized;
    }

    static int _glfwWindowHoveredNull(_GLFWwindow* window)
    {
        return _glfw.@null.xcursor >= window->@null.xpos &&
               _glfw.@null.ycursor >= window->@null.ypos &&
               _glfw.@null.xcursor <= window->@null.xpos + window->@null.width - 1 &&
               _glfw.@null.ycursor <= window->@null.ypos + window->@null.height - 1
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static int _glfwFramebufferTransparentNull(_GLFWwindow* window)
    {
        return window->@null.transparent;
    }

    static void _glfwSetWindowResizableNull(_GLFWwindow* window, int enabled)
    {
        window->@null.resizable = enabled;
    }

    static void _glfwSetWindowDecoratedNull(_GLFWwindow* window, int enabled)
    {
        window->@null.decorated = enabled;
    }

    static void _glfwSetWindowFloatingNull(_GLFWwindow* window, int enabled)
    {
        window->@null.floating = enabled;
    }

    static void _glfwSetWindowMousePassthroughNull(_GLFWwindow* window, int enabled)
    {
    }

    static float _glfwGetWindowOpacityNull(_GLFWwindow* window)
    {
        return window->@null.opacity;
    }

    static void _glfwSetWindowOpacityNull(_GLFWwindow* window, float opacity)
    {
        window->@null.opacity = opacity;
    }

    static void _glfwSetRawMouseMotionNull(_GLFWwindow* window, int enabled)
    {
    }

    static int _glfwRawMouseMotionSupportedNull()
    {
        return GLFW_TRUE;
    }

    static void _glfwShowWindowNull(_GLFWwindow* window)
    {
        window->@null.visible = GLFW_TRUE;
    }

    static void _glfwRequestWindowAttentionNull(_GLFWwindow* window)
    {
    }

    static void _glfwHideWindowNull(_GLFWwindow* window)
    {
        if (_glfw.@null.focusedWindow == window)
        {
            _glfw.@null.focusedWindow = null;
            _glfwInputWindowFocus(window, GLFW_FALSE);
        }

        window->@null.visible = GLFW_FALSE;
    }

    static void _glfwFocusWindowNull(_GLFWwindow* window)
    {
        if (_glfw.@null.focusedWindow == window)
            return;

        if (window->@null.visible == 0)
            return;

        var previous = _glfw.@null.focusedWindow;
        _glfw.@null.focusedWindow = window;

        if (previous != null)
        {
            _glfwInputWindowFocus(previous, GLFW_FALSE);
            if (previous->monitor != null && previous->autoIconify != 0)
                _glfwIconifyWindowNull(previous);
        }

        _glfwInputWindowFocus(window, GLFW_TRUE);
    }

    static int _glfwWindowFocusedNull(_GLFWwindow* window)
    {
        return _glfw.@null.focusedWindow == window ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowIconifiedNull(_GLFWwindow* window)
    {
        return window->@null.iconified;
    }

    static int _glfwWindowVisibleNull(_GLFWwindow* window)
    {
        return window->@null.visible;
    }

    static void _glfwPollEventsNull()
    {
    }

    static void _glfwWaitEventsNull()
    {
    }

    static void _glfwWaitEventsTimeoutNull(double timeout)
    {
    }

    static void _glfwPostEmptyEventNull()
    {
    }

    static void _glfwGetCursorPosNull(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (xpos != null)
            *xpos = _glfw.@null.xcursor - window->@null.xpos;
        if (ypos != null)
            *ypos = _glfw.@null.ycursor - window->@null.ypos;
    }

    static void _glfwSetCursorPosNull(_GLFWwindow* window, double x, double y)
    {
        _glfw.@null.xcursor = window->@null.xpos + (int)x;
        _glfw.@null.ycursor = window->@null.ypos + (int)y;
    }

    static void _glfwSetCursorModeNull(_GLFWwindow* window, int mode)
    {
    }

    static int _glfwCreateCursorNull(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        return GLFW_TRUE;
    }

    static int _glfwCreateStandardCursorNull(_GLFWcursor* cursor, int shape)
    {
        return GLFW_TRUE;
    }

    static void _glfwDestroyCursorNull(_GLFWcursor* cursor)
    {
    }

    static void _glfwSetCursorNull(_GLFWwindow* window, _GLFWcursor* cursor)
    {
    }

    static void _glfwSetClipboardStringNull(byte* value)
    {
        var copy = _glfw_strdup(value);
        _glfw_free(_glfw.@null.clipboardString);
        _glfw.@null.clipboardString = copy;
    }

    static byte* _glfwGetClipboardStringNull()
    {
        return _glfw.@null.clipboardString;
    }

    static int _glfwGetEGLPlatformNull(int** attribs)
    {
        return 0;
    }

    static void* _glfwGetEGLNativeDisplayNull()
    {
        return null;
    }

    static void* _glfwGetEGLNativeWindowNull(_GLFWwindow* window)
    {
        return null;
    }

    static byte* _glfwGetScancodeNameNull(int scancode)
    {
        return null;
    }

    static int _glfwGetKeyScancodeNull(int key)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
            return glfw->@null.scancodes[key];
    }

    static void _glfwGetRequiredInstanceExtensionsNull(byte** extensions)
    {
    }

    static int _glfwGetPhysicalDevicePresentationSupportNull(void* instance, void* device, uint queuefamily)
    {
        return GLFW_FALSE;
    }

    static int _glfwCreateWindowSurfaceNull(void* instance, _GLFWwindow* window, void* allocator, ulong* surface)
    {
        return VK_ERROR_EXTENSION_NOT_PRESENT;
    }
}
