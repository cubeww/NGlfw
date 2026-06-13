namespace NGlfw;

public static unsafe partial class Glfw
{
    static void cocoa_reportUnimplemented(string feature)
    {
        _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED, "Cocoa: {0} has not yet been ported", feature);
    }

    static void cocoa_acquireMonitor(_GLFWwindow* window)
    {
        if (window->monitor == null || window->ns.@object == null)
            return;

        _glfwSetVideoModeCocoa(window->monitor, &window->videoMode);

        var bounds = CGDisplayBounds(window->monitor->ns.displayID);
        var frame = cocoa_makeRect(bounds.origin.x,
            cocoa_transformY(bounds.origin.y + bounds.size.height - 1),
            bounds.size.width,
            bounds.size.height);

        objc_msgSend_void_rect_bool(window->ns.@object,
            cocoa_sel("setFrame:display:"),
            frame,
            1);

        _glfwInputMonitorWindow(window->monitor, window);
    }

    static void cocoa_releaseMonitor(_GLFWwindow* window)
    {
        if (window->monitor == null || window->monitor->window != window)
            return;

        _glfwInputMonitorWindow(window->monitor, null);
        _glfwRestoreVideoModeCocoa(window->monitor);
    }

    static double cocoa_transformY(double y)
    {
        if (!OperatingSystem.IsMacOS())
            return y;

        var bounds = CGDisplayBounds(CGMainDisplayID());
        return bounds.size.height - y - 1;
    }

    static int cocoa_createNativeWindow(_GLFWwindow* window,
                                        _GLFWwndconfig* wndconfig,
                                        _GLFWfbconfig* fbconfig)
    {
        var windowClass = cocoa_registerWindowClass();
        if (windowClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to retrieve window class");
            return GLFW_FALSE;
        }

        var delegateClass = cocoa_registerWindowDelegateClass();
        if (delegateClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to retrieve window delegate class");
            return GLFW_FALSE;
        }

        var contentViewClass = cocoa_registerContentViewClass();
        if (contentViewClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to retrieve content view class");
            return GLFW_FALSE;
        }

        NSRect contentRect;
        if (window->monitor != null)
        {
            GLFWvidmode mode;
            int xpos;
            int ypos;

            if (_glfwGetVideoModeCocoa(window->monitor, &mode) == 0)
            {
                mode.width = wndconfig->width;
                mode.height = wndconfig->height;
            }

            _glfwGetMonitorPosCocoa(window->monitor, &xpos, &ypos);
            contentRect = cocoa_makeRect(xpos, ypos, mode.width, mode.height);
        }
        else if (wndconfig->xpos == GLFW_ANY_POSITION ||
                 wndconfig->ypos == GLFW_ANY_POSITION)
        {
            contentRect = cocoa_makeRect(0, 0, wndconfig->width, wndconfig->height);
        }
        else
        {
            contentRect = cocoa_makeRect(wndconfig->xpos,
                cocoa_transformY(wndconfig->ypos + wndconfig->height - 1),
                wndconfig->width,
                wndconfig->height);
        }

        var styleMask = NSWindowStyleMaskMiniaturizable;
        if (window->monitor != null || window->decorated == 0)
            styleMask |= NSWindowStyleMaskBorderless;
        else
        {
            styleMask |= NSWindowStyleMaskTitled | NSWindowStyleMaskClosable;
            if (window->resizable != 0)
                styleMask |= NSWindowStyleMaskResizable;
        }

        var allocatedWindow = cocoa_msgSend_id(windowClass, "alloc");
        window->ns.@object = objc_msgSend_id_rect_ulong_long_bool(allocatedWindow,
            cocoa_sel("initWithContentRect:styleMask:backing:defer:"),
            contentRect,
            styleMask,
            NSBackingStoreBuffered,
            0);

        if (window->ns.@object == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create window");
            return GLFW_FALSE;
        }

        cocoa_setObjectWindow(window->ns.@object, window);

        var allocatedDelegate = cocoa_msgSend_id(delegateClass, "alloc");
        window->ns.delegateObject = cocoa_msgSend_id(allocatedDelegate, "init");
        if (window->ns.delegateObject == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create window delegate");
            return GLFW_FALSE;
        }

        cocoa_setObjectWindow(window->ns.delegateObject, window);

        var allocatedView = cocoa_msgSend_id(contentViewClass, "alloc");
        window->ns.view = objc_msgSend_id_rect(allocatedView,
            cocoa_sel("initWithFrame:"),
            cocoa_makeRect(0, 0, contentRect.size.width, contentRect.size.height));

        if (window->ns.view == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create content view");
            return GLFW_FALSE;
        }

        cocoa_setObjectWindow(window->ns.view, window);

        window->ns.markedText = cocoa_msgSend_id(cocoa_msgSend_id(cocoa_getClass("NSMutableAttributedString"), "alloc"), "init");
        cocoa_msgSend_void(window->ns.view, "updateTrackingAreas");

        var urlType = cocoa_stringFromUTF8(_glfwCocoaPasteboardTypeURL);
        var draggedTypes = cocoa_msgSend_id_ptr(cocoa_getClass("NSArray"), "arrayWithObject:", urlType);
        cocoa_msgSend_void_ptr(window->ns.view, "registerForDraggedTypes:", draggedTypes);
        cocoa_releaseTemporaryString(urlType);

        if (window->monitor != null)
            objc_msgSend_void_long(window->ns.@object, cocoa_sel("setLevel:"), NSMainMenuWindowLevel + 1);
        else
        {
            if (wndconfig->xpos == GLFW_ANY_POSITION ||
                wndconfig->ypos == GLFW_ANY_POSITION)
            {
                cocoa_msgSend_void(window->ns.@object, "center");
            }

            if (wndconfig->resizable != 0)
            {
                objc_msgSend_void_ulong(window->ns.@object,
                    cocoa_sel("setCollectionBehavior:"),
                    NSWindowCollectionBehaviorFullScreenPrimary |
                    NSWindowCollectionBehaviorManaged);
            }
            else
            {
                objc_msgSend_void_ulong(window->ns.@object,
                    cocoa_sel("setCollectionBehavior:"),
                    NSWindowCollectionBehaviorFullScreenNone);
            }

            if (wndconfig->floating != 0)
                objc_msgSend_void_long(window->ns.@object, cocoa_sel("setLevel:"), NSFloatingWindowLevel);

            if (wndconfig->maximized != 0)
                cocoa_msgSend_void_ptr(window->ns.@object, "zoom:", null);
        }

        var frameName = wndconfig->ns.frameName;
        if (_glfw_strlen(frameName) != 0)
        {
            var frameNameObject = cocoa_stringFromUTF8(frameName);
            cocoa_msgSend_void_ptr(window->ns.@object, "setFrameAutosaveName:", frameNameObject);
            cocoa_releaseTemporaryString(frameNameObject);
        }

        window->ns.retina = wndconfig->scaleFramebuffer;

        if (fbconfig->transparent != 0)
        {
            window->ns.transparent = GLFW_TRUE;
            cocoa_msgSend_void_bool(window->ns.@object, "setOpaque:", GLFW_FALSE);
            cocoa_msgSend_void_bool(window->ns.@object, "setHasShadow:", GLFW_FALSE);

            var colorClass = cocoa_getClass("NSColor");
            var clearColor = cocoa_msgSend_id(colorClass, "clearColor");
            cocoa_msgSend_void_ptr(window->ns.@object, "setBackgroundColor:", clearColor);
        }

        cocoa_msgSend_void_ptr(window->ns.@object, "setContentView:", window->ns.view);
        cocoa_msgSend_void_ptr(window->ns.@object, "makeFirstResponder:", window->ns.view);
        cocoa_msgSend_void_ptr(window->ns.@object, "setDelegate:", window->ns.delegateObject);

        var title = cocoa_stringFromUTF8(wndconfig->title);
        cocoa_msgSend_void_ptr(window->ns.@object, "setTitle:", title);
        cocoa_msgSend_void_ptr(window->ns.@object, "setMiniwindowTitle:", title);
        cocoa_releaseTemporaryString(title);

        cocoa_msgSend_void_bool(window->ns.@object, "setAcceptsMouseMovedEvents:", GLFW_TRUE);
        cocoa_msgSend_void_bool(window->ns.@object, "setRestorable:", GLFW_FALSE);

        _glfwGetWindowSizeCocoa(window, &window->ns.width, &window->ns.height);
        _glfwGetFramebufferSizeCocoa(window, &window->ns.fbWidth, &window->ns.fbHeight);
        _glfwGetWindowContentScaleCocoa(window, &window->ns.xscale, &window->ns.yscale);
        window->ns.xpos = wndconfig->xpos == GLFW_ANY_POSITION ? 0 : wndconfig->xpos;
        window->ns.ypos = wndconfig->ypos == GLFW_ANY_POSITION ? 0 : wndconfig->ypos;
        window->ns.visible = wndconfig->visible;
        window->ns.focused = GLFW_FALSE;
        window->ns.opacity = 1f;

        return GLFW_TRUE;
    }

    static int _glfwCreateWindowCocoa(_GLFWwindow* window,
                                      _GLFWwndconfig* wndconfig,
                                      _GLFWctxconfig* ctxconfig,
                                      _GLFWfbconfig* fbconfig)
    {
        if (cocoa_createNativeWindow(window, wndconfig, fbconfig) == 0)
            return GLFW_FALSE;

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API)
            {
                if (_glfwInitNSGL() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextNSGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                cocoa_msgSend_void_bool(window->ns.view, "setWantsLayer:", GLFW_TRUE);
                window->ns.layer = cocoa_msgSend_id(window->ns.view, "layer");

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
            _glfwSetWindowMousePassthroughCocoa(window, GLFW_TRUE);

        if (window->monitor != null)
        {
            _glfwShowWindowCocoa(window);
            _glfwFocusWindowCocoa(window);
            cocoa_acquireMonitor(window);

            if (wndconfig->centerCursor != 0)
                _glfwCenterCursorInContentArea(window);
        }
        else if (wndconfig->visible != 0)
        {
            _glfwShowWindowCocoa(window);
            if (wndconfig->focused != 0)
                _glfwFocusWindowCocoa(window);
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowCocoa(_GLFWwindow* window)
    {
        if (_glfw.ns.disabledCursorWindow == window)
            _glfw.ns.disabledCursorWindow = null;

        cocoa_msgSend_void_ptr(window->ns.@object, "orderOut:", null);

        if (window->monitor != null)
            cocoa_releaseMonitor(window);

        if (window->context.destroy != null)
            window->context.destroy(window);

        cocoa_msgSend_void_ptr(window->ns.@object, "setDelegate:", null);
        cocoa_clearObjectWindow(window->ns.delegateObject);
        cocoa_msgSend_void(window->ns.delegateObject, "release");
        window->ns.delegateObject = null;

        if (window->ns.trackingArea != null)
        {
            cocoa_msgSend_void_ptr(window->ns.view, "removeTrackingArea:", window->ns.trackingArea);
            cocoa_msgSend_void(window->ns.trackingArea, "release");
            window->ns.trackingArea = null;
        }

        cocoa_clearObjectWindow(window->ns.view);
        cocoa_msgSend_void(window->ns.view, "release");
        window->ns.view = null;

        cocoa_msgSend_void(window->ns.markedText, "release");
        window->ns.markedText = null;

        cocoa_clearObjectWindow(window->ns.@object);
        cocoa_msgSend_void(window->ns.@object, "close");
        window->ns = default;

        _glfwPollEventsCocoa();
    }

    static void _glfwSetWindowTitleCocoa(_GLFWwindow* window, byte* title)
    {
        if (window->ns.@object == null)
            return;

        var stringObject = cocoa_stringFromUTF8(title);
        cocoa_msgSend_void_ptr(window->ns.@object, "setTitle:", stringObject);
        cocoa_msgSend_void_ptr(window->ns.@object, "setMiniwindowTitle:", stringObject);
        cocoa_msgSend_void(stringObject, "release");
    }

    static void _glfwSetWindowIconCocoa(_GLFWwindow* window, int count, GLFWimage* images)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE,
            "Cocoa: Regular windows do not have icons on macOS");
    }

    static void _glfwGetWindowPosCocoa(_GLFWwindow* window, int* xpos, int* ypos)
    {
        if (window->ns.@object != null)
        {
            var frame = objc_msgSend_rect(window->ns.@object, cocoa_sel("frame"));
            var contentRect = objc_msgSend_rect_rect(window->ns.@object,
                cocoa_sel("contentRectForFrameRect:"),
                frame);

            window->ns.xpos = (int)contentRect.origin.x;
            window->ns.ypos = (int)cocoa_transformY(contentRect.origin.y + contentRect.size.height - 1);
        }

        if (xpos != null)
            *xpos = window->ns.xpos;
        if (ypos != null)
            *ypos = window->ns.ypos;
    }

    static void _glfwSetWindowPosCocoa(_GLFWwindow* window, int xpos, int ypos)
    {
        if (window->ns.@object != null)
        {
            var contentRect = window->ns.view != null
                ? objc_msgSend_rect(window->ns.view, cocoa_sel("frame"))
                : cocoa_makeRect(0, 0, window->ns.width, window->ns.height);
            var dummyRect = cocoa_makeRect(xpos,
                cocoa_transformY(ypos + contentRect.size.height - 1),
                0,
                0);
            var frameRect = objc_msgSend_rect_rect(window->ns.@object,
                cocoa_sel("frameRectForContentRect:"),
                dummyRect);
            objc_msgSend_void_point(window->ns.@object,
                cocoa_sel("setFrameOrigin:"),
                frameRect.origin);
        }

        window->ns.xpos = xpos;
        window->ns.ypos = ypos;
    }

    static void _glfwGetWindowSizeCocoa(_GLFWwindow* window, int* width, int* height)
    {
        if (window->ns.view != null)
        {
            var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            window->ns.width = (int)contentRect.size.width;
            window->ns.height = (int)contentRect.size.height;
        }

        if (width != null)
            *width = window->ns.width;
        if (height != null)
            *height = window->ns.height;
    }

    static void _glfwSetWindowSizeCocoa(_GLFWwindow* window, int width, int height)
    {
        if (window->ns.@object != null)
        {
            if (window->monitor != null)
            {
                if (window->monitor->window == window)
                    cocoa_acquireMonitor(window);
            }
            else
            {
                var contentRect = window->ns.@object != null
                    ? objc_msgSend_rect_rect(window->ns.@object,
                        cocoa_sel("contentRectForFrameRect:"),
                        objc_msgSend_rect(window->ns.@object, cocoa_sel("frame")))
                    : cocoa_makeRect(window->ns.xpos, window->ns.ypos, window->ns.width, window->ns.height);

                contentRect.origin.y += contentRect.size.height - height;
                contentRect.size = cocoa_makeSize(width, height);

                var frameRect = objc_msgSend_rect_rect(window->ns.@object,
                    cocoa_sel("frameRectForContentRect:"),
                    contentRect);
                objc_msgSend_void_rect_bool(window->ns.@object,
                    cocoa_sel("setFrame:display:"),
                    frameRect,
                    1);
            }
        }

        window->ns.width = width;
        window->ns.height = height;
    }

    static void _glfwSetWindowSizeLimitsCocoa(_GLFWwindow* window,
                                              int minwidth, int minheight,
                                              int maxwidth, int maxheight)
    {
        if (window->ns.@object == null)
            return;

        objc_msgSend_void_size(window->ns.@object,
            cocoa_sel("setContentMinSize:"),
            minwidth == GLFW_DONT_CARE || minheight == GLFW_DONT_CARE
                ? cocoa_makeSize(0, 0)
                : cocoa_makeSize(minwidth, minheight));

        objc_msgSend_void_size(window->ns.@object,
            cocoa_sel("setContentMaxSize:"),
            maxwidth == GLFW_DONT_CARE || maxheight == GLFW_DONT_CARE
                ? cocoa_makeSize(double.MaxValue, double.MaxValue)
                : cocoa_makeSize(maxwidth, maxheight));
    }

    static void _glfwSetWindowAspectRatioCocoa(_GLFWwindow* window, int numer, int denom)
    {
        if (window->ns.@object == null)
            return;

        if (numer == GLFW_DONT_CARE || denom == GLFW_DONT_CARE)
            objc_msgSend_void_size(window->ns.@object, cocoa_sel("setResizeIncrements:"), cocoa_makeSize(1, 1));
        else
            objc_msgSend_void_size(window->ns.@object, cocoa_sel("setContentAspectRatio:"), cocoa_makeSize(numer, denom));
    }

    static void _glfwGetFramebufferSizeCocoa(_GLFWwindow* window, int* width, int* height)
    {
        if (window->ns.view != null)
        {
            var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            var fbRect = objc_msgSend_rect_rect(window->ns.view,
                cocoa_sel("convertRectToBacking:"),
                contentRect);

            if (width != null)
                *width = (int)fbRect.size.width;
            if (height != null)
                *height = (int)fbRect.size.height;
            return;
        }

        _glfwGetWindowSizeCocoa(window, width, height);
    }

    static void _glfwGetWindowFrameSizeCocoa(_GLFWwindow* window,
                                             int* left, int* top,
                                             int* right, int* bottom)
    {
        if (window->ns.@object != null && window->ns.view != null)
        {
            var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            var frameRect = objc_msgSend_rect_rect(window->ns.@object,
                cocoa_sel("frameRectForContentRect:"),
                contentRect);

            if (left != null)
                *left = (int)(contentRect.origin.x - frameRect.origin.x);
            if (top != null)
                *top = (int)(frameRect.origin.y + frameRect.size.height -
                             contentRect.origin.y - contentRect.size.height);
            if (right != null)
                *right = (int)(frameRect.origin.x + frameRect.size.width -
                              contentRect.origin.x - contentRect.size.width);
            if (bottom != null)
                *bottom = (int)(contentRect.origin.y - frameRect.origin.y);
            return;
        }

        if (left != null) *left = 0;
        if (top != null) *top = 0;
        if (right != null) *right = 0;
        if (bottom != null) *bottom = 0;
    }

    static void _glfwGetWindowContentScaleCocoa(_GLFWwindow* window, float* xscale, float* yscale)
    {
        if (window->ns.view != null)
        {
            var points = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            var pixels = objc_msgSend_rect_rect(window->ns.view,
                cocoa_sel("convertRectToBacking:"),
                points);

            if (points.size.width > 0 && points.size.height > 0)
            {
                if (xscale != null)
                    *xscale = (float)(pixels.size.width / points.size.width);
                if (yscale != null)
                    *yscale = (float)(pixels.size.height / points.size.height);
                return;
            }
        }

        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwIconifyWindowCocoa(_GLFWwindow* window)
    {
        cocoa_msgSend_void_ptr(window->ns.@object, "miniaturize:", null);
        window->ns.iconified = GLFW_TRUE;
    }

    static void _glfwRestoreWindowCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null)
        {
            if (objc_msgSend_bool(window->ns.@object, cocoa_sel("isMiniaturized")) != 0)
                cocoa_msgSend_void_ptr(window->ns.@object, "deminiaturize:", null);
            else if (objc_msgSend_bool(window->ns.@object, cocoa_sel("isZoomed")) != 0)
                cocoa_msgSend_void_ptr(window->ns.@object, "zoom:", null);
        }

        window->ns.iconified = GLFW_FALSE;
        window->ns.maximized = GLFW_FALSE;
    }

    static void _glfwMaximizeWindowCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null &&
            objc_msgSend_bool(window->ns.@object, cocoa_sel("isZoomed")) == 0)
        {
            cocoa_msgSend_void_ptr(window->ns.@object, "zoom:", null);
        }

        window->ns.maximized = GLFW_TRUE;
    }

    static void _glfwShowWindowCocoa(_GLFWwindow* window)
    {
        cocoa_msgSend_void_ptr(window->ns.@object, "orderFront:", null);
        window->ns.visible = GLFW_TRUE;
    }

    static void _glfwHideWindowCocoa(_GLFWwindow* window)
    {
        cocoa_msgSend_void_ptr(window->ns.@object, "orderOut:", null);
        window->ns.visible = GLFW_FALSE;
        window->ns.focused = GLFW_FALSE;
    }

    static void _glfwRequestWindowAttentionCocoa(_GLFWwindow* window)
    {
        var app = cocoa_getNSApp();
        if (app != null)
            objc_msgSend_id_int(app, cocoa_sel("requestUserAttention:"), 10);
    }

    static void _glfwFocusWindowCocoa(_GLFWwindow* window)
    {
        var app = cocoa_getNSApp();
        cocoa_msgSend_id_bool(app, "activateIgnoringOtherApps:", GLFW_TRUE);
        cocoa_msgSend_void_ptr(window->ns.@object, "makeKeyAndOrderFront:", null);
        window->ns.focused = GLFW_TRUE;
    }

    static void _glfwSetWindowMonitorCocoa(_GLFWwindow* window,
                                           _GLFWmonitor* monitor,
                                           int xpos, int ypos,
                                           int width, int height,
                                           int refreshRate)
    {
        if (window->monitor == monitor)
        {
            if (monitor != null)
            {
                if (monitor->window == window)
                    cocoa_acquireMonitor(window);
            }
            else
            {
                if (window->ns.@object != null)
                {
                    var contentRect = cocoa_makeRect(xpos,
                        cocoa_transformY(ypos + height - 1),
                        width,
                        height);
                    var frameRect = objc_msgSend_rect_rect(window->ns.@object,
                        cocoa_sel("frameRectForContentRect:"),
                        contentRect);
                    objc_msgSend_void_rect_bool(window->ns.@object,
                        cocoa_sel("setFrame:display:"),
                        frameRect,
                        1);
                }

                window->ns.xpos = xpos;
                window->ns.ypos = ypos;
                window->ns.width = width;
                window->ns.height = height;
            }

            return;
        }

        if (window->monitor != null)
            cocoa_releaseMonitor(window);

        _glfwInputWindowMonitor(window, monitor);

        if (window->ns.@object != null)
        {
            var styleMask = (ulong)objc_msgSend_int(window->ns.@object, cocoa_sel("styleMask"));
            if (window->monitor != null)
            {
                styleMask &= ~(NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable);
                styleMask |= NSWindowStyleMaskBorderless;
                objc_msgSend_void_ulong(window->ns.@object, cocoa_sel("setStyleMask:"), styleMask);
                cocoa_msgSend_void_ptr(window->ns.@object, "makeFirstResponder:", window->ns.view);
                objc_msgSend_void_long(window->ns.@object, cocoa_sel("setLevel:"), NSMainMenuWindowLevel + 1);
                cocoa_msgSend_void_bool(window->ns.@object, "setHasShadow:", GLFW_FALSE);
                cocoa_acquireMonitor(window);

                int mx;
                int my;
                GLFWvidmode mode;
                _glfwGetMonitorPosCocoa(window->monitor, &mx, &my);
                _glfwGetVideoModeCocoa(window->monitor, &mode);
                window->ns.xpos = mx;
                window->ns.ypos = my;
                window->ns.width = mode.width;
                window->ns.height = mode.height;
            }
            else
            {
                if (window->decorated != 0)
                {
                    styleMask &= ~NSWindowStyleMaskBorderless;
                    styleMask |= NSWindowStyleMaskTitled | NSWindowStyleMaskClosable;
                }

                if (window->resizable != 0)
                    styleMask |= NSWindowStyleMaskResizable;
                else
                    styleMask &= ~NSWindowStyleMaskResizable;

                objc_msgSend_void_ulong(window->ns.@object, cocoa_sel("setStyleMask:"), styleMask);
                cocoa_msgSend_void_ptr(window->ns.@object, "makeFirstResponder:", window->ns.view);

                var contentRect = cocoa_makeRect(xpos, cocoa_transformY(ypos + height - 1), width, height);
                var frameRect = objc_msgSend_rect_rect(window->ns.@object,
                    cocoa_sel("frameRectForContentRect:"),
                    contentRect);
                objc_msgSend_void_rect_bool(window->ns.@object,
                    cocoa_sel("setFrame:display:"),
                    frameRect,
                    1);

                objc_msgSend_void_long(window->ns.@object,
                    cocoa_sel("setLevel:"),
                    window->floating != 0 ? NSFloatingWindowLevel : NSNormalWindowLevel);

                if (window->numer != GLFW_DONT_CARE &&
                    window->denom != GLFW_DONT_CARE)
                {
                    objc_msgSend_void_size(window->ns.@object,
                        cocoa_sel("setContentAspectRatio:"),
                        cocoa_makeSize(window->numer, window->denom));
                }

                if (window->minwidth != GLFW_DONT_CARE &&
                    window->minheight != GLFW_DONT_CARE)
                {
                    objc_msgSend_void_size(window->ns.@object,
                        cocoa_sel("setContentMinSize:"),
                        cocoa_makeSize(window->minwidth, window->minheight));
                }

                if (window->maxwidth != GLFW_DONT_CARE &&
                    window->maxheight != GLFW_DONT_CARE)
                {
                    objc_msgSend_void_size(window->ns.@object,
                        cocoa_sel("setContentMaxSize:"),
                        cocoa_makeSize(window->maxwidth, window->maxheight));
                }

                objc_msgSend_void_ulong(window->ns.@object,
                    cocoa_sel("setCollectionBehavior:"),
                    window->resizable != 0
                        ? NSWindowCollectionBehaviorFullScreenPrimary | NSWindowCollectionBehaviorManaged
                        : NSWindowCollectionBehaviorFullScreenNone);
                cocoa_msgSend_void_bool(window->ns.@object, "setHasShadow:", GLFW_TRUE);
                cocoa_msgSend_void_ptr(window->ns.@object,
                    "setTitle:",
                    cocoa_msgSend_id(window->ns.@object, "miniwindowTitle"));
            }
        }

        if (window->monitor == null)
        {
            window->ns.xpos = xpos;
            window->ns.ypos = ypos;
            window->ns.width = width;
            window->ns.height = height;
        }
    }

    static int _glfwWindowFocusedCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null)
            return objc_msgSend_bool(window->ns.@object, cocoa_sel("isKeyWindow")) != 0 ? GLFW_TRUE : GLFW_FALSE;
        return window->ns.focused;
    }

    static int _glfwWindowIconifiedCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null)
            return objc_msgSend_bool(window->ns.@object, cocoa_sel("isMiniaturized")) != 0 ? GLFW_TRUE : GLFW_FALSE;
        return window->ns.iconified;
    }

    static int _glfwWindowVisibleCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null)
            return objc_msgSend_bool(window->ns.@object, cocoa_sel("isVisible")) != 0 ? GLFW_TRUE : GLFW_FALSE;
        return window->ns.visible;
    }

    static int _glfwWindowMaximizedCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null && window->resizable != 0)
            return objc_msgSend_bool(window->ns.@object, cocoa_sel("isZoomed")) != 0 ? GLFW_TRUE : GLFW_FALSE;
        return window->ns.maximized;
    }

    static int _glfwWindowHoveredCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object == null || window->ns.view == null)
            return window->ns.hovered;

        var point = objc_msgSend_point(cocoa_getClass("NSEvent"), cocoa_sel("mouseLocation"));
        var topWindow = objc_msgSend_long_point_long(cocoa_getClass("NSWindow"),
            cocoa_sel("windowNumberAtPoint:belowWindowWithWindowNumber:"),
            point,
            0);
        if (topWindow != objc_msgSend_long(window->ns.@object, cocoa_sel("windowNumber")))
            return GLFW_FALSE;

        var rect = objc_msgSend_rect_rect(window->ns.@object,
            cocoa_sel("convertRectToScreen:"),
            objc_msgSend_rect(window->ns.view, cocoa_sel("frame")));

        return point.x >= rect.origin.x &&
               point.y >= rect.origin.y &&
               point.x < rect.origin.x + rect.size.width &&
               point.y < rect.origin.y + rect.size.height
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static int _glfwFramebufferTransparentCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null && window->ns.view != null)
        {
            return objc_msgSend_bool(window->ns.@object, cocoa_sel("isOpaque")) == 0 &&
                   objc_msgSend_bool(window->ns.view, cocoa_sel("isOpaque")) == 0
                ? GLFW_TRUE
                : GLFW_FALSE;
        }

        return window->ns.transparent;
    }

    static float _glfwGetWindowOpacityCocoa(_GLFWwindow* window)
    {
        if (window->ns.@object != null)
            window->ns.opacity = (float)objc_msgSend_double(window->ns.@object, cocoa_sel("alphaValue"));
        return window->ns.opacity;
    }

    static void _glfwSetWindowResizableCocoa(_GLFWwindow* window, int enabled)
    {
        if (window->ns.@object != null)
        {
            var styleMask = (ulong)objc_msgSend_int(window->ns.@object, cocoa_sel("styleMask"));
            if (enabled != 0)
                styleMask |= NSWindowStyleMaskResizable;
            else
                styleMask &= ~NSWindowStyleMaskResizable;

            objc_msgSend_void_ulong(window->ns.@object, cocoa_sel("setStyleMask:"), styleMask);
            objc_msgSend_void_ulong(window->ns.@object,
                cocoa_sel("setCollectionBehavior:"),
                enabled != 0
                    ? NSWindowCollectionBehaviorFullScreenPrimary | NSWindowCollectionBehaviorManaged
                    : NSWindowCollectionBehaviorFullScreenNone);
        }

        window->resizable = enabled;
    }

    static void _glfwSetWindowDecoratedCocoa(_GLFWwindow* window, int enabled)
    {
        if (window->ns.@object != null)
        {
            var styleMask = (ulong)objc_msgSend_int(window->ns.@object, cocoa_sel("styleMask"));
            if (enabled != 0)
            {
                styleMask &= ~NSWindowStyleMaskBorderless;
                styleMask |= NSWindowStyleMaskTitled | NSWindowStyleMaskClosable;
            }
            else
            {
                styleMask &= ~(NSWindowStyleMaskTitled | NSWindowStyleMaskClosable);
                styleMask |= NSWindowStyleMaskBorderless;
            }

            objc_msgSend_void_ulong(window->ns.@object, cocoa_sel("setStyleMask:"), styleMask);
            cocoa_msgSend_void_ptr(window->ns.@object, "makeFirstResponder:", window->ns.view);
        }

        window->decorated = enabled;
    }

    static void _glfwSetWindowFloatingCocoa(_GLFWwindow* window, int enabled)
    {
        if (window->ns.@object != null)
        {
            objc_msgSend_void_long(window->ns.@object,
                cocoa_sel("setLevel:"),
                enabled != 0 ? NSFloatingWindowLevel : NSNormalWindowLevel);
        }

        window->floating = enabled;
    }

    static void _glfwSetWindowOpacityCocoa(_GLFWwindow* window, float opacity)
    {
        if (window->ns.@object != null)
            objc_msgSend_void_double(window->ns.@object, cocoa_sel("setAlphaValue:"), opacity);
        window->ns.opacity = opacity;
    }

    static void _glfwSetWindowMousePassthroughCocoa(_GLFWwindow* window, int enabled)
    {
        if (window->ns.@object != null)
            cocoa_msgSend_void_bool(window->ns.@object, "setIgnoresMouseEvents:", enabled);

        window->mousePassthrough = enabled;
    }

    static void _glfwPollEventsCocoa()
    {
        var app = cocoa_getNSApp();
        if (app == null)
            return;

        var dateClass = cocoa_getClass("NSDate");
        var date = cocoa_msgSend_id(dateClass, "distantPast");
        var mode = cocoa_getDefaultRunLoopMode();

        for (;;)
        {
            var eventObject = objc_msgSend_id_ulong_ptr_ptr_bool(app,
                cocoa_sel("nextEventMatchingMask:untilDate:inMode:dequeue:"),
                NSEventMaskAny,
                date,
                mode,
                1);
            if (eventObject == null)
                break;

            cocoa_msgSend_void_ptr(app, "sendEvent:", eventObject);
        }

        cocoa_releaseTemporaryString(mode);
    }

    static void _glfwWaitEventsCocoa()
    {
        var app = cocoa_getNSApp();
        if (app == null)
            return;

        var dateClass = cocoa_getClass("NSDate");
        var date = cocoa_msgSend_id(dateClass, "distantFuture");
        var mode = cocoa_getDefaultRunLoopMode();
        var eventObject = objc_msgSend_id_ulong_ptr_ptr_bool(app,
            cocoa_sel("nextEventMatchingMask:untilDate:inMode:dequeue:"),
            NSEventMaskAny,
            date,
            mode,
            1);
        if (eventObject != null)
            cocoa_msgSend_void_ptr(app, "sendEvent:", eventObject);

        cocoa_releaseTemporaryString(mode);
        _glfwPollEventsCocoa();
    }

    static void _glfwWaitEventsTimeoutCocoa(double timeout)
    {
        var app = cocoa_getNSApp();
        if (app == null)
            return;

        var dateClass = cocoa_getClass("NSDate");
        var date = objc_msgSend_id_double(dateClass, cocoa_sel("dateWithTimeIntervalSinceNow:"), timeout);
        var mode = cocoa_getDefaultRunLoopMode();
        var eventObject = objc_msgSend_id_ulong_ptr_ptr_bool(app,
            cocoa_sel("nextEventMatchingMask:untilDate:inMode:dequeue:"),
            NSEventMaskAny,
            date,
            mode,
            1);
        if (eventObject != null)
            cocoa_msgSend_void_ptr(app, "sendEvent:", eventObject);

        cocoa_releaseTemporaryString(mode);
        _glfwPollEventsCocoa();
    }

    static void _glfwPostEmptyEventCocoa()
    {
        var eventClass = cocoa_getClass("NSEvent");
        var eventObject = objc_msgSend_id_long_point_ulong_double_long_ptr_long_long_long(eventClass,
            cocoa_sel("otherEventWithType:location:modifierFlags:timestamp:windowNumber:context:subtype:data1:data2:"),
            NSEventTypeApplicationDefined,
            new NSPoint { x = 0, y = 0 },
            0,
            0,
            0,
            null,
            0,
            0,
            0);

        if (eventObject != null)
            objc_msgSend_void_ptr_bool(cocoa_getNSApp(), cocoa_sel("postEvent:atStart:"), eventObject, 1);
    }

    static void _glfwGetCursorPosCocoa(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (window->ns.@object != null && window->ns.view != null)
        {
            var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            var pos = objc_msgSend_point(window->ns.@object, cocoa_sel("mouseLocationOutsideOfEventStream"));
            window->virtualCursorPosX = pos.x;
            window->virtualCursorPosY = contentRect.size.height - pos.y;
        }

        if (xpos != null)
            *xpos = window->virtualCursorPosX;
        if (ypos != null)
            *ypos = window->virtualCursorPosY;
    }

    static void cocoa_updateCursorMode(_GLFWwindow* window)
    {
        if (window->cursorMode == GLFW_CURSOR_DISABLED)
        {
            _glfw.ns.disabledCursorWindow = window;
            double restoreX;
            double restoreY;
            _glfwGetCursorPosCocoa(window, &restoreX, &restoreY);
            _glfw.ns.restoreCursorPosX = restoreX;
            _glfw.ns.restoreCursorPosY = restoreY;
            _glfwCenterCursorInContentArea(window);
            CGAssociateMouseAndMouseCursorPosition(0);
        }
        else if (_glfw.ns.disabledCursorWindow == window)
        {
            _glfw.ns.disabledCursorWindow = null;
            _glfwSetCursorPosCocoa(window,
                _glfw.ns.restoreCursorPosX,
                _glfw.ns.restoreCursorPosY);
        }

        if (cocoa_cursorInContentArea(window) != 0)
            cocoa_updateCursorImage(window);
    }

    static void _glfwSetCursorPosCocoa(_GLFWwindow* window, double x, double y)
    {
        cocoa_updateCursorImage(window);

        if (window->ns.@object != null && window->ns.view != null)
        {
            var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
            var pos = objc_msgSend_point(window->ns.@object, cocoa_sel("mouseLocationOutsideOfEventStream"));

            window->ns.cursorWarpDeltaX += x - pos.x;
            window->ns.cursorWarpDeltaY += y - contentRect.size.height + pos.y;

            if (window->monitor != null)
            {
                CGDisplayMoveCursorToPoint(window->monitor->ns.displayID,
                    new NSPoint { x = x, y = y });
            }
            else
            {
                var localRect = cocoa_makeRect(x, contentRect.size.height - y - 1, 0, 0);
                var globalRect = objc_msgSend_rect_rect(window->ns.@object,
                    cocoa_sel("convertRectToScreen:"),
                    localRect);

                CGWarpMouseCursorPosition(new NSPoint
                {
                    x = globalRect.origin.x,
                    y = cocoa_transformY(globalRect.origin.y)
                });
            }

            if (window->cursorMode != GLFW_CURSOR_DISABLED)
                CGAssociateMouseAndMouseCursorPosition(1);
        }

        window->virtualCursorPosX = x;
        window->virtualCursorPosY = y;
    }

    static void _glfwSetCursorModeCocoa(_GLFWwindow* window, int mode)
    {
        if (mode == GLFW_CURSOR_CAPTURED)
        {
            _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED,
                "Cocoa: Captured cursor mode not yet implemented");
        }

        if (_glfwWindowFocusedCocoa(window) != 0)
            cocoa_updateCursorMode(window);
    }

    static void _glfwSetRawMouseMotionCocoa(_GLFWwindow* window, int enabled)
    {
        _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED,
            "Cocoa: Raw mouse motion not yet implemented");
    }

    static int _glfwRawMouseMotionSupportedCocoa()
    {
        return GLFW_FALSE;
    }

    static int _glfwCreateCursorCocoa(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        var colorSpaceName = cocoa_stringFromUTF8("NSCalibratedRGBColorSpace");
        var repClass = cocoa_getClass("NSBitmapImageRep");
        var allocatedRep = cocoa_msgSend_id(repClass, "alloc");
        var rep = objc_msgSend_id_ptr_long_long_long_long_bool_bool_ptr_ulong_long_long(allocatedRep,
            cocoa_sel("initWithBitmapDataPlanes:pixelsWide:pixelsHigh:bitsPerSample:samplesPerPixel:hasAlpha:isPlanar:colorSpaceName:bitmapFormat:bytesPerRow:bitsPerPixel:"),
            null,
            image->width,
            image->height,
            8,
            4,
            1,
            0,
            colorSpaceName,
            NSBitmapFormatAlphaNonpremultiplied,
            image->width * 4,
            32);
        cocoa_releaseTemporaryString(colorSpaceName);

        if (rep == null)
            return GLFW_FALSE;

        var bitmapData = objc_msgSend_bytes(rep, cocoa_sel("bitmapData"));
        if (bitmapData == null)
        {
            cocoa_msgSend_void(rep, "release");
            return GLFW_FALSE;
        }

        _glfw_memcpy(bitmapData, image->pixels, (nuint)(image->width * image->height * 4));

        var nativeClass = cocoa_getClass("NSImage");
        var native = objc_msgSend_id_size(cocoa_msgSend_id(nativeClass, "alloc"),
            cocoa_sel("initWithSize:"),
            cocoa_makeSize(image->width, image->height));
        if (native == null)
        {
            cocoa_msgSend_void(rep, "release");
            return GLFW_FALSE;
        }

        cocoa_msgSend_void_ptr(native, "addRepresentation:", rep);

        cursor->ns.@object = objc_msgSend_id_ptr_point(cocoa_msgSend_id(cocoa_getClass("NSCursor"), "alloc"),
            cocoa_sel("initWithImage:hotSpot:"),
            native,
            new NSPoint { x = xhot, y = yhot });

        cocoa_msgSend_void(native, "release");
        cocoa_msgSend_void(rep, "release");

        return cursor->ns.@object != null ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwCreateStandardCursorCocoa(_GLFWcursor* cursor, int shape)
    {
        var cursorClass = cocoa_getClass("NSCursor");
        nint privateSelector = 0;

        switch (shape)
        {
            case GLFW_RESIZE_EW_CURSOR:
                privateSelector = cocoa_sel("_windowResizeEastWestCursor");
                break;
            case GLFW_RESIZE_NS_CURSOR:
                privateSelector = cocoa_sel("_windowResizeNorthSouthCursor");
                break;
            case GLFW_RESIZE_NWSE_CURSOR:
                privateSelector = cocoa_sel("_windowResizeNorthWestSouthEastCursor");
                break;
            case GLFW_RESIZE_NESW_CURSOR:
                privateSelector = cocoa_sel("_windowResizeNorthEastSouthWestCursor");
                break;
        }

        if (privateSelector != 0 &&
            objc_msgSend_bool_nint(cursorClass, cocoa_sel("respondsToSelector:"), privateSelector) != 0)
        {
            cursor->ns.@object = objc_msgSend_id_nint(cursorClass, cocoa_sel("performSelector:"), privateSelector);
        }

        var selector = shape switch
        {
            GLFW_ARROW_CURSOR => "arrowCursor",
            GLFW_IBEAM_CURSOR => "IBeamCursor",
            GLFW_CROSSHAIR_CURSOR => "crosshairCursor",
            GLFW_POINTING_HAND_CURSOR => "pointingHandCursor",
            GLFW_RESIZE_EW_CURSOR => "resizeLeftRightCursor",
            GLFW_RESIZE_NS_CURSOR => "resizeUpDownCursor",
            GLFW_RESIZE_ALL_CURSOR => "closedHandCursor",
            GLFW_NOT_ALLOWED_CURSOR => "operationNotAllowedCursor",
            _ => null
        };

        if (cursor->ns.@object == null && selector == null)
        {
            _glfwInputError(GLFW_CURSOR_UNAVAILABLE, "Cocoa: Standard cursor shape unavailable");
            return GLFW_FALSE;
        }

        if (cursor->ns.@object == null)
            cursor->ns.@object = cocoa_msgSend_id(cursorClass, selector!);
        if (cursor->ns.@object == null)
        {
            _glfwInputError(GLFW_CURSOR_UNAVAILABLE, "Cocoa: Standard cursor shape unavailable");
            return GLFW_FALSE;
        }

        cocoa_msgSend_void(cursor->ns.@object, "retain");
        return GLFW_TRUE;
    }

    static void _glfwDestroyCursorCocoa(_GLFWcursor* cursor)
    {
        cocoa_msgSend_void(cursor->ns.@object, "release");
        cursor->ns.@object = null;
    }

    static void _glfwSetCursorCocoa(_GLFWwindow* window, _GLFWcursor* cursor)
    {
        cocoa_updateCursorImage(window);
    }

    static byte* _glfwGetScancodeNameCocoa(int scancode)
    {
        if (scancode < 0 || scancode > 0xff)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid scancode {0}", scancode);
            return null;
        }

        int key;
        fixed (short* keycodes = _glfw.ns.keycodes)
            key = keycodes[scancode];

        if (key == GLFW_KEY_UNKNOWN)
            return null;

        if (_glfw.ns.unicodeData == null ||
            _glfw.ns.tis.GetKbdType == null ||
            _glfw.ns.tis.UCKeyTranslate == null)
        {
            return null;
        }

        var keyLayout = objc_msgSend_bytes(_glfw.ns.unicodeData, cocoa_sel("bytes"));
        if (keyLayout == null)
            return null;

        uint deadKeyState = 0;
        var characters = stackalloc ushort[4];
        uint characterCount = 0;

        if (_glfw.ns.tis.UCKeyTranslate(keyLayout,
                (ushort)scancode,
                kUCKeyActionDisplay,
                0,
                _glfw.ns.tis.GetKbdType(),
                kUCKeyTranslateNoDeadKeysBit,
                &deadKeyState,
                4,
                &characterCount,
                characters) != 0)
        {
            return null;
        }

        if (characterCount == 0)
            return null;

        var stringObject = CFStringCreateWithCharacters(null, characters, (nint)characterCount);
        if (stringObject == null)
            return null;

        fixed (byte* keynames = _glfw.ns.keynames)
        {
            var buffer = keynames + key * _GLFW_COCOA_KEYNAME_LENGTH;
            buffer[0] = 0;

            if (CFStringGetCString(stringObject,
                    buffer,
                    _GLFW_COCOA_KEYNAME_LENGTH,
                    kCFStringEncodingUTF8) == 0)
            {
                CFRelease(stringObject);
                return null;
            }

            CFRelease(stringObject);
            return buffer;
        }
    }

    static int _glfwGetKeyScancodeCocoa(int key)
    {
        if (key < 0 || key > GLFW_KEY_LAST)
            return -1;

        fixed (short* scancodes = _glfw.ns.scancodes)
            return scancodes[key];
    }

    static void _glfwSetClipboardStringCocoa(byte* value)
    {
        var pasteboard = cocoa_msgSend_id(cocoa_getClass("NSPasteboard"), "generalPasteboard");
        if (pasteboard != null)
        {
            var type = cocoa_stringFromUTF8(_glfwCocoaPasteboardTypeString);
            var array = cocoa_msgSend_id_ptr(cocoa_getClass("NSArray"), "arrayWithObject:", type);
            objc_msgSend_long_ptr_ptr(pasteboard, cocoa_sel("declareTypes:owner:"), array, null);

            var stringObject = cocoa_stringFromUTF8(value);
            objc_msgSend_bool_ptr_ptr(pasteboard,
                cocoa_sel("setString:forType:"),
                stringObject,
                type);

            cocoa_releaseTemporaryString(stringObject);
            cocoa_releaseTemporaryString(type);
        }

        var copy = _glfw_strdup(value);
        _glfw_free(_glfw.ns.clipboardString);
        _glfw.ns.clipboardString = copy;
    }

    static byte* _glfwGetClipboardStringCocoa()
    {
        var pasteboard = cocoa_msgSend_id(cocoa_getClass("NSPasteboard"), "generalPasteboard");
        if (pasteboard != null)
        {
            var type = cocoa_stringFromUTF8(_glfwCocoaPasteboardTypeString);
            var types = cocoa_msgSend_id(pasteboard, "types");

            if (types == null || objc_msgSend_bool_ptr(types, cocoa_sel("containsObject:"), type) == 0)
            {
                cocoa_releaseTemporaryString(type);
                _glfwInputError(GLFW_FORMAT_UNAVAILABLE,
                    "Cocoa: Failed to retrieve string from pasteboard");
                return null;
            }

            var objectString = cocoa_msgSend_id_ptr(pasteboard, "stringForType:", type);
            cocoa_releaseTemporaryString(type);

            if (objectString == null)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR,
                    "Cocoa: Failed to retrieve object from pasteboard");
                return null;
            }

            var utf8 = objc_msgSend_UTF8String(objectString, cocoa_sel("UTF8String"));
            if (utf8 == null)
                return null;

            _glfw_free(_glfw.ns.clipboardString);
            _glfw.ns.clipboardString = _glfw_strdup(utf8);
        }

        return _glfw.ns.clipboardString;
    }

    static int _glfwGetEGLPlatformCocoa(int** attribs)
    {
        if (_glfw.egl.ANGLE_platform_angle != 0)
        {
            var type = 0;

            if (_glfw.egl.ANGLE_platform_angle_opengl != 0)
            {
                if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_OPENGL)
                    type = EGL_PLATFORM_ANGLE_TYPE_OPENGL_ANGLE;
            }

            if (_glfw.egl.ANGLE_platform_angle_metal != 0)
            {
                if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_METAL)
                    type = EGL_PLATFORM_ANGLE_TYPE_METAL_ANGLE;
            }

            if (type != 0)
            {
                *attribs = (int*)_glfw_calloc(3, (nuint)sizeof(int));
                if (*attribs == null)
                    return 0;

                (*attribs)[0] = EGL_PLATFORM_ANGLE_TYPE_ANGLE;
                (*attribs)[1] = type;
                (*attribs)[2] = EGL_NONE;
                return EGL_PLATFORM_ANGLE_ANGLE;
            }
        }

        return 0;
    }

    static void* _glfwGetEGLNativeDisplayCocoa()
    {
        return null;
    }

    static void* _glfwGetEGLNativeWindowCocoa(_GLFWwindow* window)
    {
        return window->ns.layer;
    }

    static void _glfwGetRequiredInstanceExtensionsCocoa(byte** extensions)
    {
        if (_glfw.vk.KHR_surface == 0)
            return;

        extensions[0] = _glfwVkKHRSurfaceExtensionName;

        if (_glfw.vk.EXT_metal_surface != 0)
            extensions[1] = _glfwVkEXTMetalSurfaceExtensionName;
        else if (_glfw.vk.MVK_macos_surface != 0)
            extensions[1] = _glfwVkMVKMacOSSurfaceExtensionName;
        else
            extensions[0] = null;
    }

    static int _glfwGetPhysicalDevicePresentationSupportCocoa(void* instance, void* device, uint queuefamily)
    {
        return GLFW_TRUE;
    }

    static int cocoa_createMetalLayer(_GLFWwindow* window)
    {
        var path = cocoa_stringFromUTF8("/System/Library/Frameworks/QuartzCore.framework");
        var bundle = cocoa_msgSend_id_ptr(cocoa_getClass("NSBundle"), "bundleWithPath:", path);
        cocoa_releaseTemporaryString(path);
        if (bundle == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Cocoa: Failed to find QuartzCore.framework");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        var className = cocoa_stringFromUTF8("CAMetalLayer");
        var layerClass = cocoa_msgSend_id_ptr(bundle, "classNamed:", className);
        cocoa_releaseTemporaryString(className);
        if (layerClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Cocoa: Failed to find CAMetalLayer");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        window->ns.layer = cocoa_msgSend_id(layerClass, "layer");
        if (window->ns.layer == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Cocoa: Failed to create layer for view");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        if (window->ns.retina != 0)
        {
            var scale = objc_msgSend_double(window->ns.@object, cocoa_sel("backingScaleFactor"));
            objc_msgSend_void_double(window->ns.layer, cocoa_sel("setContentsScale:"), scale);
        }

        cocoa_msgSend_void_ptr(window->ns.view, "setLayer:", window->ns.layer);
        cocoa_msgSend_void_bool(window->ns.view, "setWantsLayer:", GLFW_TRUE);

        return VK_SUCCESS;
    }

    static int _glfwCreateWindowSurfaceCocoa(void* instance,
                                             _GLFWwindow* window,
                                             void* allocator,
                                             ulong* surface)
    {
        if (surface != null)
            *surface = 0;

        var err = cocoa_createMetalLayer(window);
        if (err != VK_SUCCESS)
            return err;

        if (_glfw.vk.EXT_metal_surface != 0)
        {
            var vkCreateMetalSurfaceEXT =
                (delegate* unmanaged<void*, VkMetalSurfaceCreateInfoEXT*, void*, ulong*, int>)
                vulkan_getInstanceProcAddress(instance, "vkCreateMetalSurfaceEXT");
            if (vkCreateMetalSurfaceEXT == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "Cocoa: Vulkan instance missing VK_EXT_metal_surface extension");
                return VK_ERROR_EXTENSION_NOT_PRESENT;
            }

            var sci = new VkMetalSurfaceCreateInfoEXT
            {
                sType = VK_STRUCTURE_TYPE_METAL_SURFACE_CREATE_INFO_EXT,
                pLayer = window->ns.layer
            };

            err = vkCreateMetalSurfaceEXT(instance, &sci, allocator, surface);
        }
        else
        {
            var vkCreateMacOSSurfaceMVK =
                (delegate* unmanaged<void*, VkMacOSSurfaceCreateInfoMVK*, void*, ulong*, int>)
                vulkan_getInstanceProcAddress(instance, "vkCreateMacOSSurfaceMVK");
            if (vkCreateMacOSSurfaceMVK == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "Cocoa: Vulkan instance missing VK_MVK_macos_surface extension");
                return VK_ERROR_EXTENSION_NOT_PRESENT;
            }

            var sci = new VkMacOSSurfaceCreateInfoMVK
            {
                sType = VK_STRUCTURE_TYPE_MACOS_SURFACE_CREATE_INFO_MVK,
                pView = window->ns.view
            };

            err = vkCreateMacOSSurfaceMVK(instance, &sci, allocator, surface);
        }

        if (err != VK_SUCCESS)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Cocoa: Failed to create Vulkan surface: {0}",
                vulkan_resultString(err));
        }

        return err;
    }

    public static void* glfwGetCocoaWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_COCOA)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not initialized");
            return null;
        }

        return ((_GLFWwindow*)window)->ns.@object;
    }

    public static void* glfwGetCocoaView(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_COCOA)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not initialized");
            return null;
        }

        return ((_GLFWwindow*)window)->ns.view;
    }
}
