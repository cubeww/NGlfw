namespace NGlfw;

public static unsafe partial class Glfw
{
    static void _glfwInputWindowFocus(_GLFWwindow* window, int focused)
    {
        if (window->callbacks.focus != null)
            window->callbacks.focus((GLFWwindow*)window, focused);

        if (focused == 0)
        {
            for (var key = 0; key <= GLFW_KEY_LAST; key++)
            {
                if (window->keys[key] == GLFW_PRESS)
                {
                    var scancode = _glfw.platform.getKeyScancode != null ?
                        _glfw.platform.getKeyScancode(key) : 0;
                    _glfwInputKey(window, key, scancode, GLFW_RELEASE, 0);
                }
            }

            for (var button = 0; button <= GLFW_MOUSE_BUTTON_LAST; button++)
            {
                if (window->mouseButtons[button] == GLFW_PRESS)
                    _glfwInputMouseClick(window, button, GLFW_RELEASE, 0);
            }
        }
    }

    static void _glfwInputWindowPos(_GLFWwindow* window, int x, int y)
    {
        if (window->callbacks.pos != null)
            window->callbacks.pos((GLFWwindow*)window, x, y);
    }

    static void _glfwInputWindowSize(_GLFWwindow* window, int width, int height)
    {
        if (window->callbacks.size != null)
            window->callbacks.size((GLFWwindow*)window, width, height);
    }

    static void _glfwInputFramebufferSize(_GLFWwindow* window, int width, int height)
    {
        if (window->callbacks.fbsize != null)
            window->callbacks.fbsize((GLFWwindow*)window, width, height);
    }

    static void _glfwInputWindowContentScale(_GLFWwindow* window, float xscale, float yscale)
    {
        if (window->callbacks.scale != null)
            window->callbacks.scale((GLFWwindow*)window, xscale, yscale);
    }

    static void _glfwInputWindowIconify(_GLFWwindow* window, int iconified)
    {
        if (window->callbacks.iconify != null)
            window->callbacks.iconify((GLFWwindow*)window, iconified);
    }

    static void _glfwInputWindowMaximize(_GLFWwindow* window, int maximized)
    {
        if (window->callbacks.maximize != null)
            window->callbacks.maximize((GLFWwindow*)window, maximized);
    }

    static void _glfwInputWindowDamage(_GLFWwindow* window)
    {
        if (window->callbacks.refresh != null)
            window->callbacks.refresh((GLFWwindow*)window);
    }

    static void _glfwInputWindowCloseRequest(_GLFWwindow* window)
    {
        window->shouldClose = GLFW_TRUE;

        if (window->callbacks.close != null)
            window->callbacks.close((GLFWwindow*)window);
    }

    static void _glfwInputWindowMonitor(_GLFWwindow* window, _GLFWmonitor* monitor)
    {
        window->monitor = monitor;
    }

    public static GLFWwindow* glfwCreateWindow(int width, int height,
                                               byte* title,
                                               GLFWmonitor* monitor,
                                               GLFWwindow* share)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (title == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window title");
            return null;
        }

        if (width <= 0 || height <= 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window size {0}x{1}", width, height);
            return null;
        }

        var fbconfig = _glfw.hints.framebuffer;
        var ctxconfig = _glfw.hints.context;
        var wndconfig = _glfw.hints.window;

        wndconfig.width = width;
        wndconfig.height = height;
        wndconfig.title = title;
        ctxconfig.share = (_GLFWwindow*)share;

        if (_glfwIsValidContextConfig(&ctxconfig) == 0)
            return null;

        var window = (_GLFWwindow*)_glfw_calloc(1, (nuint)sizeof(_GLFWwindow));
        if (window == null)
            return null;

        window->next = _glfw.windowListHead;
        _glfw.windowListHead = window;

        window->videoMode.width = width;
        window->videoMode.height = height;
        window->videoMode.redBits = fbconfig.redBits;
        window->videoMode.greenBits = fbconfig.greenBits;
        window->videoMode.blueBits = fbconfig.blueBits;
        window->videoMode.refreshRate = _glfw.hints.refreshRate;

        window->monitor = (_GLFWmonitor*)monitor;
        window->resizable = wndconfig.resizable;
        window->decorated = wndconfig.decorated;
        window->autoIconify = wndconfig.autoIconify;
        window->floating = wndconfig.floating;
        window->focusOnShow = wndconfig.focusOnShow;
        window->mousePassthrough = wndconfig.mousePassthrough;
        window->cursorMode = GLFW_CURSOR_NORMAL;
        window->doublebuffer = fbconfig.doublebuffer;

        window->minwidth = GLFW_DONT_CARE;
        window->minheight = GLFW_DONT_CARE;
        window->maxwidth = GLFW_DONT_CARE;
        window->maxheight = GLFW_DONT_CARE;
        window->numer = GLFW_DONT_CARE;
        window->denom = GLFW_DONT_CARE;
        window->title = _glfw_strdup(title);

        if (_glfw.platform.createWindow(window, &wndconfig, &ctxconfig, &fbconfig) == 0)
        {
            glfwDestroyWindow((GLFWwindow*)window);
            return null;
        }

        return (GLFWwindow*)window;
    }

    public static void glfwDefaultWindowHints()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.hints.context = default;
        _glfw.hints.context.client = GLFW_OPENGL_API;
        _glfw.hints.context.source = GLFW_NATIVE_CONTEXT_API;
        _glfw.hints.context.major = 1;
        _glfw.hints.context.minor = 0;

        _glfw.hints.window = default;
        _glfw.hints.window.resizable = GLFW_TRUE;
        _glfw.hints.window.visible = GLFW_TRUE;
        _glfw.hints.window.decorated = GLFW_TRUE;
        _glfw.hints.window.focused = GLFW_TRUE;
        _glfw.hints.window.autoIconify = GLFW_TRUE;
        _glfw.hints.window.centerCursor = GLFW_TRUE;
        _glfw.hints.window.focusOnShow = GLFW_TRUE;
        _glfw.hints.window.xpos = GLFW_ANY_POSITION;
        _glfw.hints.window.ypos = GLFW_ANY_POSITION;
        _glfw.hints.window.scaleFramebuffer = GLFW_TRUE;

        _glfw.hints.framebuffer = default;
        _glfw.hints.framebuffer.redBits = 8;
        _glfw.hints.framebuffer.greenBits = 8;
        _glfw.hints.framebuffer.blueBits = 8;
        _glfw.hints.framebuffer.alphaBits = 8;
        _glfw.hints.framebuffer.depthBits = 24;
        _glfw.hints.framebuffer.stencilBits = 8;
        _glfw.hints.framebuffer.doublebuffer = GLFW_TRUE;

        _glfw.hints.refreshRate = GLFW_DONT_CARE;
    }

    public static void glfwWindowHint(int hint, int value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        switch (hint)
        {
            case GLFW_RED_BITS:
                _glfw.hints.framebuffer.redBits = value;
                return;
            case GLFW_GREEN_BITS:
                _glfw.hints.framebuffer.greenBits = value;
                return;
            case GLFW_BLUE_BITS:
                _glfw.hints.framebuffer.blueBits = value;
                return;
            case GLFW_ALPHA_BITS:
                _glfw.hints.framebuffer.alphaBits = value;
                return;
            case GLFW_DEPTH_BITS:
                _glfw.hints.framebuffer.depthBits = value;
                return;
            case GLFW_STENCIL_BITS:
                _glfw.hints.framebuffer.stencilBits = value;
                return;
            case GLFW_ACCUM_RED_BITS:
                _glfw.hints.framebuffer.accumRedBits = value;
                return;
            case GLFW_ACCUM_GREEN_BITS:
                _glfw.hints.framebuffer.accumGreenBits = value;
                return;
            case GLFW_ACCUM_BLUE_BITS:
                _glfw.hints.framebuffer.accumBlueBits = value;
                return;
            case GLFW_ACCUM_ALPHA_BITS:
                _glfw.hints.framebuffer.accumAlphaBits = value;
                return;
            case GLFW_AUX_BUFFERS:
                _glfw.hints.framebuffer.auxBuffers = value;
                return;
            case GLFW_STEREO:
                _glfw.hints.framebuffer.stereo = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_DOUBLEBUFFER:
                _glfw.hints.framebuffer.doublebuffer = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_TRANSPARENT_FRAMEBUFFER:
                _glfw.hints.framebuffer.transparent = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_SAMPLES:
                _glfw.hints.framebuffer.samples = value;
                return;
            case GLFW_SRGB_CAPABLE:
                _glfw.hints.framebuffer.sRGB = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_CLIENT_API:
                _glfw.hints.context.client = value;
                return;
            case GLFW_CONTEXT_CREATION_API:
                _glfw.hints.context.source = value;
                return;
            case GLFW_CONTEXT_VERSION_MAJOR:
                _glfw.hints.context.major = value;
                return;
            case GLFW_CONTEXT_VERSION_MINOR:
                _glfw.hints.context.minor = value;
                return;
            case GLFW_CONTEXT_ROBUSTNESS:
                _glfw.hints.context.robustness = value;
                return;
            case GLFW_OPENGL_FORWARD_COMPAT:
                _glfw.hints.context.forward = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_CONTEXT_DEBUG:
                _glfw.hints.context.debug = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_CONTEXT_NO_ERROR:
                _glfw.hints.context.noerror = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_OPENGL_PROFILE:
                _glfw.hints.context.profile = value;
                return;
            case GLFW_CONTEXT_RELEASE_BEHAVIOR:
                _glfw.hints.context.release = value;
                return;
            case GLFW_RESIZABLE:
                _glfw.hints.window.resizable = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_VISIBLE:
                _glfw.hints.window.visible = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_DECORATED:
                _glfw.hints.window.decorated = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_FOCUSED:
                _glfw.hints.window.focused = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_AUTO_ICONIFY:
                _glfw.hints.window.autoIconify = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_FLOATING:
                _glfw.hints.window.floating = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_MAXIMIZED:
                _glfw.hints.window.maximized = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_POSITION_X:
                _glfw.hints.window.xpos = value;
                return;
            case GLFW_POSITION_Y:
                _glfw.hints.window.ypos = value;
                return;
            case GLFW_WIN32_KEYBOARD_MENU:
                _glfw.hints.window.win32.keymenu = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_WIN32_SHOWDEFAULT:
                _glfw.hints.window.win32.showDefault = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_COCOA_GRAPHICS_SWITCHING:
                _glfw.hints.context.nsgl.offline = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_SCALE_TO_MONITOR:
                _glfw.hints.window.scaleToMonitor = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_SCALE_FRAMEBUFFER:
            case GLFW_COCOA_RETINA_FRAMEBUFFER:
                _glfw.hints.window.scaleFramebuffer = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_CENTER_CURSOR:
                _glfw.hints.window.centerCursor = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_FOCUS_ON_SHOW:
                _glfw.hints.window.focusOnShow = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_MOUSE_PASSTHROUGH:
                _glfw.hints.window.mousePassthrough = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;
            case GLFW_REFRESH_RATE:
                _glfw.hints.refreshRate = value;
                return;
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid window hint 0x%08X", hint);
    }

    public static void glfwWindowHintString(int hint, byte* value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (value == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            switch (hint)
            {
                case GLFW_COCOA_FRAME_NAME:
                    _glfw_strncpy(glfw->hints.window.ns.frameName, value, 256);
                    return;
                case GLFW_X11_CLASS_NAME:
                    _glfw_strncpy(glfw->hints.window.x11.className, value, 256);
                    return;
                case GLFW_X11_INSTANCE_NAME:
                    _glfw_strncpy(glfw->hints.window.x11.instanceName, value, 256);
                    return;
                case GLFW_WAYLAND_APP_ID:
                    _glfw_strncpy(glfw->hints.window.wl.appId, value, 256);
                    return;
            }
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid window hint string 0x%08X", hint);
    }

    public static void glfwDestroyWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow == null)
            return;

        internalWindow->callbacks = default;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (_glfwPlatformGetTls(&glfw->contextSlot) == internalWindow)
                glfwMakeContextCurrent(null);
        }

        if (_glfw.platform.destroyWindow != null)
            _glfw.platform.destroyWindow(internalWindow);

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var prev = &glfw->windowListHead;
            while (*prev != null && *prev != internalWindow)
                prev = &(*prev)->next;

            if (*prev == internalWindow)
                *prev = internalWindow->next;
        }

        _glfw_free(internalWindow->title);
        _glfw_free(internalWindow);
    }

    public static void glfwDestroyCursor(GLFWcursor* cursor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (cursor == null)
            return;

        var internalCursor = (_GLFWcursor*)cursor;

        for (var window = _glfw.windowListHead; window != null; window = window->next)
        {
            if (window->cursor == internalCursor)
                glfwSetCursor((GLFWwindow*)window, null);
        }

        if (_glfw.platform.destroyCursor != null)
            _glfw.platform.destroyCursor(internalCursor);

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var previous = &glfw->cursorListHead;
            while (*previous != null && *previous != internalCursor)
                previous = &(*previous)->next;

            if (*previous == internalCursor)
                *previous = internalCursor->next;
        }

        _glfw_free(internalCursor);
    }

    public static int glfwWindowShouldClose(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        return ((_GLFWwindow*)window)->shouldClose;
    }

    public static void glfwSetWindowShouldClose(GLFWwindow* window, int value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        ((_GLFWwindow*)window)->shouldClose = value;
    }

    public static byte* glfwGetWindowTitle(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return ((_GLFWwindow*)window)->title;
    }

    public static void glfwSetWindowTitle(GLFWwindow* window, byte* title)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->title;
        internalWindow->title = _glfw_strdup(title);

        if (_glfw.platform.setWindowTitle != null)
            _glfw.platform.setWindowTitle(internalWindow, title);

        _glfw_free(previous);
    }

    public static void glfwSetWindowIcon(GLFWwindow* window, int count, GLFWimage* images)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (count < 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid image count for window icon");
            return;
        }

        for (var i = 0; i < count; i++)
        {
            if (images == null || images[i].pixels == null || images[i].width <= 0 || images[i].height <= 0)
            {
                _glfwInputError(GLFW_INVALID_VALUE, "Invalid image dimensions for window icon");
                return;
            }
        }

        _glfw.platform.setWindowIcon((_GLFWwindow*)window, count, images);
    }

    public static void glfwGetWindowPos(GLFWwindow* window, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getWindowPos((_GLFWwindow*)window, xpos, ypos);
    }

    public static void glfwSetWindowPos(GLFWwindow* window, int xpos, int ypos)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->monitor != null)
            return;

        _glfw.platform.setWindowPos(internalWindow, xpos, ypos);
    }

    public static void glfwGetWindowSize(GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = 0;
        if (height != null)
            *height = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getWindowSize((_GLFWwindow*)window, width, height);
    }

    public static void glfwSetWindowSize(GLFWwindow* window, int width, int height)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (width <= 0 || height <= 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window size {0}x{1}", width, height);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        internalWindow->videoMode.width = width;
        internalWindow->videoMode.height = height;
        _glfw.platform.setWindowSize(internalWindow, width, height);
    }

    public static void glfwSetWindowSizeLimits(GLFWwindow* window, int minwidth, int minheight, int maxwidth, int maxheight)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (minwidth != GLFW_DONT_CARE && minheight != GLFW_DONT_CARE && (minwidth < 0 || minheight < 0))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window minimum size {0}x{1}", minwidth, minheight);
            return;
        }

        if (maxwidth != GLFW_DONT_CARE && maxheight != GLFW_DONT_CARE &&
            (maxwidth < 0 || maxheight < 0 || maxwidth < minwidth || maxheight < minheight))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window maximum size {0}x{1}", maxwidth, maxheight);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        internalWindow->minwidth = minwidth;
        internalWindow->minheight = minheight;
        internalWindow->maxwidth = maxwidth;
        internalWindow->maxheight = maxheight;

        if (internalWindow->monitor != null || internalWindow->resizable == 0)
            return;

        _glfw.platform.setWindowSizeLimits(internalWindow, minwidth, minheight, maxwidth, maxheight);
    }

    public static void glfwSetWindowAspectRatio(GLFWwindow* window, int numer, int denom)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (numer != GLFW_DONT_CARE && denom != GLFW_DONT_CARE && (numer <= 0 || denom <= 0))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window aspect ratio {0}:{1}", numer, denom);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        internalWindow->numer = numer;
        internalWindow->denom = denom;

        if (internalWindow->monitor != null || internalWindow->resizable == 0)
            return;

        _glfw.platform.setWindowAspectRatio(internalWindow, numer, denom);
    }

    public static void glfwGetFramebufferSize(GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = 0;
        if (height != null)
            *height = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getFramebufferSize((_GLFWwindow*)window, width, height);
    }

    public static void glfwGetWindowFrameSize(GLFWwindow* window, int* left, int* top, int* right, int* bottom)
    {
        if (left != null) *left = 0;
        if (top != null) *top = 0;
        if (right != null) *right = 0;
        if (bottom != null) *bottom = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getWindowFrameSize((_GLFWwindow*)window, left, top, right, bottom);
    }

    public static void glfwGetWindowContentScale(GLFWwindow* window, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 0f;
        if (yscale != null)
            *yscale = 0f;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getWindowContentScale((_GLFWwindow*)window, xscale, yscale);
    }

    public static float glfwGetWindowOpacity(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0f;
        }

        return _glfw.platform.getWindowOpacity((_GLFWwindow*)window);
    }

    public static void glfwSetWindowOpacity(GLFWwindow* window, float opacity)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (float.IsNaN(opacity) || opacity < 0f || opacity > 1f)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window opacity {0}", opacity);
            return;
        }

        _glfw.platform.setWindowOpacity((_GLFWwindow*)window, opacity);
    }

    public static void glfwIconifyWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.iconifyWindow((_GLFWwindow*)window);
    }

    public static void glfwRestoreWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.restoreWindow((_GLFWwindow*)window);
    }

    public static void glfwMaximizeWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->monitor != null)
            return;

        _glfw.platform.maximizeWindow(internalWindow);
    }

    public static void glfwShowWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->monitor != null)
            return;

        _glfw.platform.showWindow(internalWindow);

        if (internalWindow->focusOnShow != 0)
            _glfw.platform.focusWindow(internalWindow);
    }

    public static void glfwRequestWindowAttention(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.requestWindowAttention((_GLFWwindow*)window);
    }

    public static void glfwHideWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->monitor != null)
            return;

        _glfw.platform.hideWindow(internalWindow);
    }

    public static void glfwFocusWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.focusWindow((_GLFWwindow*)window);
    }

    public static int glfwGetWindowAttrib(GLFWwindow* window, int attrib)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        var internalWindow = (_GLFWwindow*)window;
        return attrib switch
        {
            GLFW_FOCUSED => _glfw.platform.windowFocused(internalWindow),
            GLFW_ICONIFIED => _glfw.platform.windowIconified(internalWindow),
            GLFW_VISIBLE => _glfw.platform.windowVisible(internalWindow),
            GLFW_MAXIMIZED => _glfw.platform.windowMaximized(internalWindow),
            GLFW_HOVERED => _glfw.platform.windowHovered(internalWindow),
            GLFW_FOCUS_ON_SHOW => internalWindow->focusOnShow,
            GLFW_MOUSE_PASSTHROUGH => internalWindow->mousePassthrough,
            GLFW_TRANSPARENT_FRAMEBUFFER => _glfw.platform.framebufferTransparent(internalWindow),
            GLFW_RESIZABLE => internalWindow->resizable,
            GLFW_DECORATED => internalWindow->decorated,
            GLFW_FLOATING => internalWindow->floating,
            GLFW_AUTO_ICONIFY => internalWindow->autoIconify,
            GLFW_DOUBLEBUFFER => internalWindow->doublebuffer,
            GLFW_CLIENT_API => internalWindow->context.client,
            GLFW_CONTEXT_CREATION_API => internalWindow->context.source,
            GLFW_CONTEXT_VERSION_MAJOR => internalWindow->context.major,
            GLFW_CONTEXT_VERSION_MINOR => internalWindow->context.minor,
            GLFW_CONTEXT_REVISION => internalWindow->context.revision,
            GLFW_CONTEXT_ROBUSTNESS => internalWindow->context.robustness,
            GLFW_OPENGL_FORWARD_COMPAT => internalWindow->context.forward,
            GLFW_CONTEXT_DEBUG => internalWindow->context.debug,
            GLFW_OPENGL_PROFILE => internalWindow->context.profile,
            GLFW_CONTEXT_RELEASE_BEHAVIOR => internalWindow->context.release,
            GLFW_CONTEXT_NO_ERROR => internalWindow->context.noerror,
            _ => window_invalid_attrib(attrib)
        };
    }

    static int window_invalid_attrib(int attrib)
    {
        _glfwInputError(GLFW_INVALID_ENUM, "Invalid window attribute 0x%08X", attrib);
        return 0;
    }

    public static void glfwSetWindowAttrib(GLFWwindow* window, int attrib, int value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        value = value != 0 ? GLFW_TRUE : GLFW_FALSE;

        switch (attrib)
        {
            case GLFW_AUTO_ICONIFY:
                internalWindow->autoIconify = value;
                return;
            case GLFW_RESIZABLE:
                internalWindow->resizable = value;
                if (internalWindow->monitor == null)
                    _glfw.platform.setWindowResizable(internalWindow, value);
                return;
            case GLFW_DECORATED:
                internalWindow->decorated = value;
                if (internalWindow->monitor == null)
                    _glfw.platform.setWindowDecorated(internalWindow, value);
                return;
            case GLFW_FLOATING:
                internalWindow->floating = value;
                if (internalWindow->monitor == null)
                    _glfw.platform.setWindowFloating(internalWindow, value);
                return;
            case GLFW_FOCUS_ON_SHOW:
                internalWindow->focusOnShow = value;
                return;
            case GLFW_MOUSE_PASSTHROUGH:
                internalWindow->mousePassthrough = value;
                _glfw.platform.setWindowMousePassthrough(internalWindow, value);
                return;
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid window attribute 0x%08X", attrib);
    }

    public static GLFWmonitor* glfwGetWindowMonitor(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return (GLFWmonitor*)((_GLFWwindow*)window)->monitor;
    }

    public static void glfwSetWindowMonitor(GLFWwindow* window,
                                            GLFWmonitor* monitor,
                                            int xpos,
                                            int ypos,
                                            int width,
                                            int height,
                                            int refreshRate)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (width <= 0 || height <= 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid window size {0}x{1}", width, height);
            return;
        }

        if (refreshRate < 0 && refreshRate != GLFW_DONT_CARE)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid refresh rate {0}", refreshRate);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        internalWindow->videoMode.width = width;
        internalWindow->videoMode.height = height;
        internalWindow->videoMode.refreshRate = refreshRate;

        _glfw.platform.setWindowMonitor(internalWindow, (_GLFWmonitor*)monitor, xpos, ypos, width, height, refreshRate);
    }

    public static void glfwSetWindowUserPointer(GLFWwindow* window, void* pointer)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        ((_GLFWwindow*)window)->userPointer = pointer;
    }

    public static void* glfwGetWindowUserPointer(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return ((_GLFWwindow*)window)->userPointer;
    }

    public static delegate*<GLFWwindow*, int, int, void> glfwSetWindowPosCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.pos;
        internalWindow->callbacks.pos = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, int, void> glfwSetWindowSizeCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.size;
        internalWindow->callbacks.size = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, void> glfwSetWindowCloseCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.close;
        internalWindow->callbacks.close = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, void> glfwSetWindowRefreshCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.refresh;
        internalWindow->callbacks.refresh = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, void> glfwSetWindowFocusCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.focus;
        internalWindow->callbacks.focus = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, void> glfwSetWindowIconifyCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.iconify;
        internalWindow->callbacks.iconify = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, void> glfwSetWindowMaximizeCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.maximize;
        internalWindow->callbacks.maximize = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, int, void> glfwSetFramebufferSizeCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.fbsize;
        internalWindow->callbacks.fbsize = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, float, float, void> glfwSetWindowContentScaleCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, float, float, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.scale;
        internalWindow->callbacks.scale = cbfun;
        return previous;
    }
}
