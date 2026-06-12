namespace NGlfw;

public static unsafe partial class Glfw
{
    static nuint x11_internAtom(string name)
    {
        if (_glfw.x11.XInternAtom == null)
            return 0;

        var bytes = System.Text.Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* atomName = bytes)
            return _glfw.x11.XInternAtom(_glfw.x11.display, atomName, GLFW_FALSE);
    }

    static void x11_createKeyTables()
    {
        fixed (short* keycodes = _glfw.x11.keycodes)
        fixed (short* scancodes = _glfw.x11.scancodes)
        {
            for (var i = 0; i <= _GLFW_X11_KEYCODE_LAST; i++)
                keycodes[i] = GLFW_KEY_UNKNOWN;
            for (var i = 0; i <= GLFW_KEY_LAST; i++)
                scancodes[i] = -1;

            var scancodeMin = 0;
            var scancodeMax = _GLFW_X11_KEYCODE_LAST;

            if (_glfw.x11.XDisplayKeycodes != null)
                _glfw.x11.XDisplayKeycodes(_glfw.x11.display, &scancodeMin, &scancodeMax);

            scancodeMin = _glfw_max(scancodeMin, 0);
            scancodeMax = _glfw_min(scancodeMax, _GLFW_X11_KEYCODE_LAST);

            for (var scancode = scancodeMin; scancode <= scancodeMax; scancode++)
            {
                var keysym = _glfw.x11.XkbKeycodeToKeysym != null
                    ? _glfw.x11.XkbKeycodeToKeysym(_glfw.x11.display, (uint)scancode, 0, 0)
                    : 0;
                var key = x11_translateKeySym(keysym);

                keycodes[scancode] = (short)key;

                if (key > 0 && key <= GLFW_KEY_LAST && scancodes[key] < 0)
                    scancodes[key] = (short)scancode;
            }
        }
    }

    static void x11_initXkb()
    {
        if (_glfw.x11.XkbQueryExtension == null)
            return;

        _glfw.x11.xkbMajor = 1;
        _glfw.x11.xkbMinor = 0;

        fixed (int* majorOpcode = &_glfw.x11.xkbMajorOpcode)
        fixed (int* eventBase = &_glfw.x11.xkbEventBase)
        fixed (int* errorBase = &_glfw.x11.xkbErrorBase)
        fixed (int* major = &_glfw.x11.xkbMajor)
        fixed (int* minor = &_glfw.x11.xkbMinor)
        {
            _glfw.x11.xkbAvailable =
                _glfw.x11.XkbQueryExtension(_glfw.x11.display,
                    majorOpcode,
                    eventBase,
                    errorBase,
                    major,
                    minor);
        }

        if (_glfw.x11.xkbAvailable == 0)
            return;

        if (_glfw.x11.XkbSetDetectableAutoRepeat != null)
        {
            var supported = 0;
            if (_glfw.x11.XkbSetDetectableAutoRepeat(_glfw.x11.display, GLFW_TRUE, &supported) != 0 &&
                supported != 0)
            {
                _glfw.x11.xkbDetectable = GLFW_TRUE;
            }
        }

        if (_glfw.x11.XkbGetState != null)
        {
            XkbStateRec state;
            if (_glfw.x11.XkbGetState(_glfw.x11.display, XkbUseCoreKbd, &state) == Success)
                _glfw.x11.xkbGroup = state.group;
        }

        if (_glfw.x11.XkbSelectEventDetails != null)
        {
            _glfw.x11.XkbSelectEventDetails(_glfw.x11.display,
                XkbUseCoreKbd,
                XkbStateNotify,
                XkbGroupStateMask,
                XkbGroupStateMask);
        }
    }

    static void x11_initXcursor()
    {
        _glfw.x11.xcursorHandle = x11_loadModule("libXcursor.so.1");
        if (_glfw.x11.xcursorHandle == null)
            _glfw.x11.xcursorHandle = x11_loadModule("libXcursor.so");

        if (_glfw.x11.xcursorHandle == null)
            return;

        _glfw.x11.XcursorImageCreate =
            (delegate* unmanaged<int, int, XcursorImage*>)x11_getModuleSymbol(_glfw.x11.xcursorHandle, "XcursorImageCreate");
        _glfw.x11.XcursorImageDestroy =
            (delegate* unmanaged<XcursorImage*, void>)x11_getModuleSymbol(_glfw.x11.xcursorHandle, "XcursorImageDestroy");
        _glfw.x11.XcursorImageLoadCursor =
            (delegate* unmanaged<void*, XcursorImage*, nuint>)x11_getModuleSymbol(_glfw.x11.xcursorHandle, "XcursorImageLoadCursor");
    }

    static void x11_initRandR()
    {
        _glfw.x11.randrHandle = x11_loadModule("libXrandr.so.2");
        if (_glfw.x11.randrHandle == null)
            _glfw.x11.randrHandle = x11_loadModule("libXrandr.so");

        if (_glfw.x11.randrHandle == null)
            return;

        _glfw.x11.XRRGetScreenResourcesCurrent =
            (delegate* unmanaged<void*, nuint, XRRScreenResources*>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetScreenResourcesCurrent");
        _glfw.x11.XRRFreeScreenResources =
            (delegate* unmanaged<XRRScreenResources*, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRFreeScreenResources");
        _glfw.x11.XRRGetOutputInfo =
            (delegate* unmanaged<void*, XRRScreenResources*, nuint, XRROutputInfo*>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetOutputInfo");
        _glfw.x11.XRRFreeOutputInfo =
            (delegate* unmanaged<XRROutputInfo*, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRFreeOutputInfo");
        _glfw.x11.XRRGetCrtcInfo =
            (delegate* unmanaged<void*, XRRScreenResources*, nuint, XRRCrtcInfo*>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetCrtcInfo");
        _glfw.x11.XRRFreeCrtcInfo =
            (delegate* unmanaged<XRRCrtcInfo*, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRFreeCrtcInfo");
        _glfw.x11.XRRGetOutputPrimary =
            (delegate* unmanaged<void*, nuint, nuint>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetOutputPrimary");
        _glfw.x11.XRRQueryExtension =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRQueryExtension");
        _glfw.x11.XRRQueryVersion =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRQueryVersion");
        _glfw.x11.XRRSelectInput =
            (delegate* unmanaged<void*, nuint, int, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRSelectInput");
        _glfw.x11.XRRSetCrtcConfig =
            (delegate* unmanaged<void*, XRRScreenResources*, nuint, ulong, int, int, nuint, ushort, nuint*, int, int>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRSetCrtcConfig");
        _glfw.x11.XRRGetCrtcGammaSize =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetCrtcGammaSize");
        _glfw.x11.XRRGetCrtcGamma =
            (delegate* unmanaged<void*, nuint, XRRCrtcGamma*>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRGetCrtcGamma");
        _glfw.x11.XRRAllocGamma =
            (delegate* unmanaged<int, XRRCrtcGamma*>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRAllocGamma");
        _glfw.x11.XRRFreeGamma =
            (delegate* unmanaged<XRRCrtcGamma*, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRFreeGamma");
        _glfw.x11.XRRSetCrtcGamma =
            (delegate* unmanaged<void*, nuint, XRRCrtcGamma*, void>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRSetCrtcGamma");
        _glfw.x11.XRRUpdateConfiguration =
            (delegate* unmanaged<XEvent*, int>)x11_getModuleSymbol(_glfw.x11.randrHandle, "XRRUpdateConfiguration");

        if (_glfw.x11.XRRQueryExtension == null ||
            _glfw.x11.XRRQueryVersion == null ||
            _glfw.x11.XRRGetScreenResourcesCurrent == null ||
            _glfw.x11.XRRFreeScreenResources == null ||
            _glfw.x11.XRRGetOutputInfo == null ||
            _glfw.x11.XRRFreeOutputInfo == null ||
            _glfw.x11.XRRGetCrtcInfo == null ||
            _glfw.x11.XRRFreeCrtcInfo == null)
        {
            return;
        }

        var eventBase = 0;
        var errorBase = 0;
        if (_glfw.x11.XRRQueryExtension(_glfw.x11.display, &eventBase, &errorBase) == 0)
            return;

        var major = 1;
        var minor = 3;
        if (_glfw.x11.XRRQueryVersion(_glfw.x11.display, &major, &minor) == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to query RandR version");
            return;
        }

        if (major < 1 || (major == 1 && minor < 3))
            return;

        _glfw.x11.randrAvailable = GLFW_TRUE;
        _glfw.x11.randrEventBase = eventBase;
        _glfw.x11.randrErrorBase = errorBase;
        _glfw.x11.randrMajor = major;
        _glfw.x11.randrMinor = minor;

        var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
        if (resources != null)
        {
            if (resources->ncrtc == 0)
            {
                _glfw.x11.randrMonitorBroken = GLFW_TRUE;
            }
            else if (_glfw.x11.XRRGetCrtcGammaSize == null ||
                     _glfw.x11.XRRGetCrtcGammaSize(_glfw.x11.display, resources->crtcs[0]) == 0)
            {
                _glfw.x11.randrGammaBroken = GLFW_TRUE;
            }

            _glfw.x11.XRRFreeScreenResources(resources);
        }
        else
        {
            _glfw.x11.randrMonitorBroken = GLFW_TRUE;
            _glfw.x11.randrGammaBroken = GLFW_TRUE;
        }

        if (_glfw.x11.randrMonitorBroken == 0 && _glfw.x11.XRRSelectInput != null)
        {
            _glfw.x11.XRRSelectInput(_glfw.x11.display,
                _glfw.x11.root,
                RROutputChangeNotifyMask);
        }
    }

    static void x11_initXinerama()
    {
        _glfw.x11.xineramaHandle = x11_loadModule("libXinerama.so.1");
        if (_glfw.x11.xineramaHandle == null)
            _glfw.x11.xineramaHandle = x11_loadModule("libXinerama.so");

        if (_glfw.x11.xineramaHandle == null)
            return;

        _glfw.x11.XineramaQueryExtension =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.xineramaHandle, "XineramaQueryExtension");
        _glfw.x11.XineramaIsActive =
            (delegate* unmanaged<void*, int>)x11_getModuleSymbol(_glfw.x11.xineramaHandle, "XineramaIsActive");
        _glfw.x11.XineramaQueryScreens =
            (delegate* unmanaged<void*, int*, XineramaScreenInfo*>)x11_getModuleSymbol(_glfw.x11.xineramaHandle, "XineramaQueryScreens");

        if (_glfw.x11.XineramaQueryExtension == null ||
            _glfw.x11.XineramaIsActive == null ||
            _glfw.x11.XineramaQueryScreens == null)
        {
            return;
        }

        var eventBase = 0;
        var errorBase = 0;
        if (_glfw.x11.XineramaQueryExtension(_glfw.x11.display, &eventBase, &errorBase) == 0)
            return;

        if (_glfw.x11.XineramaIsActive(_glfw.x11.display) == 0)
            return;

        _glfw.x11.xineramaAvailable = GLFW_TRUE;
    }

    static void x11_initX11XCB()
    {
        if (_glfwInitHints.x11.xcbVulkanSurface == 0)
            return;

        _glfw.x11.x11xcbHandle = x11_loadModule("libX11-xcb.so.1");
        if (_glfw.x11.x11xcbHandle == null)
            _glfw.x11.x11xcbHandle = x11_loadModule("libX11-xcb.so");

        if (_glfw.x11.x11xcbHandle == null)
            return;

        _glfw.x11.XGetXCBConnection =
            (delegate* unmanaged<void*, void*>)x11_getModuleSymbol(_glfw.x11.x11xcbHandle, "XGetXCBConnection");
        if (_glfw.x11.XGetXCBConnection == null)
            return;

        _glfw.x11.x11xcbConnection = _glfw.x11.XGetXCBConnection(_glfw.x11.display);
    }

    static void x11_initXIM()
    {
        if (_glfw.x11.XOpenIM == null)
            return;

        _glfw.x11.im = _glfw.x11.XOpenIM(_glfw.x11.display, null, null, null);
    }

    static void x11_initXShape()
    {
        _glfw.x11.xshapeHandle = x11_loadModule("libXext.so.6");
        if (_glfw.x11.xshapeHandle == null)
            _glfw.x11.xshapeHandle = x11_loadModule("libXext.so");

        if (_glfw.x11.xshapeHandle == null)
            return;

        _glfw.x11.XShapeCombineRegion =
            (delegate* unmanaged<void*, nuint, int, int, int, void*, int, void>)x11_getModuleSymbol(_glfw.x11.xshapeHandle, "XShapeCombineRegion");
        _glfw.x11.XShapeCombineMask =
            (delegate* unmanaged<void*, nuint, int, int, int, nuint, int, void>)x11_getModuleSymbol(_glfw.x11.xshapeHandle, "XShapeCombineMask");
    }

    static void x11_initXRender()
    {
        _glfw.x11.xrenderHandle = x11_loadModule("libXrender.so.1");
        if (_glfw.x11.xrenderHandle == null)
            _glfw.x11.xrenderHandle = x11_loadModule("libXrender.so");

        if (_glfw.x11.xrenderHandle == null)
            return;

        _glfw.x11.XRenderFindVisualFormat =
            (delegate* unmanaged<void*, void*, XRenderPictFormat*>)x11_getModuleSymbol(_glfw.x11.xrenderHandle, "XRenderFindVisualFormat");
    }

    static void x11_initXI()
    {
        if (_glfw.x11.XQueryExtension == null)
            return;

        _glfw.x11.xiHandle = x11_loadModule("libXi.so.6");
        if (_glfw.x11.xiHandle == null)
            _glfw.x11.xiHandle = x11_loadModule("libXi.so");

        if (_glfw.x11.xiHandle == null)
            return;

        _glfw.x11.XIQueryVersion =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.xiHandle, "XIQueryVersion");
        _glfw.x11.XISelectEvents =
            (delegate* unmanaged<void*, nuint, XIEventMask*, int, int>)x11_getModuleSymbol(_glfw.x11.xiHandle, "XISelectEvents");

        if (_glfw.x11.XIQueryVersion == null || _glfw.x11.XISelectEvents == null)
            return;

        var nameBytes = System.Text.Encoding.ASCII.GetBytes("XInputExtension\0");
        fixed (byte* name = nameBytes)
        {
            int majorOpcode;
            int eventBase;
            int errorBase;
            if (_glfw.x11.XQueryExtension(_glfw.x11.display, name, &majorOpcode, &eventBase, &errorBase) == 0)
                return;

            var major = 2;
            var minor = 0;
            if (_glfw.x11.XIQueryVersion(_glfw.x11.display, &major, &minor) != Success)
                return;

            _glfw.x11.xiAvailable = GLFW_TRUE;
            _glfw.x11.xiMajorOpcode = majorOpcode;
        }
    }

    static nuint _glfwCreateNativeCursorX11(GLFWimage* image, int xhot, int yhot)
    {
        if (_glfw.x11.xcursorHandle == null ||
            _glfw.x11.XcursorImageCreate == null ||
            _glfw.x11.XcursorImageDestroy == null ||
            _glfw.x11.XcursorImageLoadCursor == null)
        {
            return 0;
        }

        var native = _glfw.x11.XcursorImageCreate(image->width, image->height);
        if (native == null)
            return 0;

        native->xhot = (uint)xhot;
        native->yhot = (uint)yhot;

        var source = image->pixels;
        var target = native->pixels;
        var count = image->width * image->height;

        for (var i = 0; i < count; i++, source += 4, target++)
        {
            var alpha = source[3];
            *target = ((uint)alpha << 24) |
                      ((uint)((source[0] * alpha) / 255) << 16) |
                      ((uint)((source[1] * alpha) / 255) << 8) |
                      (uint)((source[2] * alpha) / 255);
        }

        var cursor = _glfw.x11.XcursorImageLoadCursor(_glfw.x11.display, native);
        _glfw.x11.XcursorImageDestroy(native);

        return cursor;
    }

    static void x11_createHiddenCursor()
    {
        var pixels = stackalloc byte[16 * 16 * 4];
        GLFWimage image = default;
        image.width = 16;
        image.height = 16;
        image.pixels = pixels;

        _glfw.x11.hiddenCursorHandle = _glfwCreateNativeCursorX11(&image, 0, 0);
    }

    static void x11_createHelperWindow()
    {
        if (_glfw.x11.XCreateSimpleWindow == null)
            return;

        _glfw.x11.helperWindowHandle = _glfw.x11.XCreateSimpleWindow(_glfw.x11.display,
            _glfw.x11.root,
            0,
            0,
            1,
            1,
            0,
            0,
            0);
    }

    static void x11_setPlatformCallbacks(_GLFWplatform* platform)
    {
        platform->platformID = GLFW_PLATFORM_X11;
        platform->init = &_glfwInitX11;
        platform->terminate = &_glfwTerminateX11;
        platform->getCursorPos = &_glfwGetCursorPosX11;
        platform->setCursorPos = &_glfwSetCursorPosX11;
        platform->setCursorMode = &_glfwSetCursorModeX11;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionX11;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedX11;
        platform->createCursor = &_glfwCreateCursorX11;
        platform->createStandardCursor = &_glfwCreateStandardCursorX11;
        platform->destroyCursor = &_glfwDestroyCursorX11;
        platform->setCursor = &_glfwSetCursorX11;
        platform->getScancodeName = &_glfwGetScancodeNameX11;
        platform->getKeyScancode = &_glfwGetKeyScancodeX11;
        platform->setClipboardString = &_glfwSetClipboardStringX11;
        platform->getClipboardString = &_glfwGetClipboardStringX11;
        platform->initJoysticks = &_glfwInitJoysticksLinux;
        platform->terminateJoysticks = &_glfwTerminateJoysticksLinux;
        platform->pollJoystick = &_glfwPollJoystickLinux;
        platform->getMappingName = &_glfwGetMappingNameLinux;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDLinux;
        platform->freeMonitor = &_glfwFreeMonitorX11;
        platform->getMonitorPos = &_glfwGetMonitorPosX11;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleX11;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaX11;
        platform->getVideoModes = &_glfwGetVideoModesX11;
        platform->getVideoMode = &_glfwGetVideoModeX11;
        platform->getGammaRamp = &_glfwGetGammaRampX11;
        platform->setGammaRamp = &_glfwSetGammaRampX11;
        platform->createWindow = &_glfwCreateWindowX11;
        platform->destroyWindow = &_glfwDestroyWindowX11;
        platform->setWindowTitle = &_glfwSetWindowTitleX11;
        platform->setWindowIcon = &_glfwSetWindowIconX11;
        platform->getWindowPos = &_glfwGetWindowPosX11;
        platform->setWindowPos = &_glfwSetWindowPosX11;
        platform->getWindowSize = &_glfwGetWindowSizeX11;
        platform->setWindowSize = &_glfwSetWindowSizeX11;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsX11;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioX11;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeX11;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeX11;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleX11;
        platform->iconifyWindow = &_glfwIconifyWindowX11;
        platform->restoreWindow = &_glfwRestoreWindowX11;
        platform->maximizeWindow = &_glfwMaximizeWindowX11;
        platform->showWindow = &_glfwShowWindowX11;
        platform->hideWindow = &_glfwHideWindowX11;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionX11;
        platform->focusWindow = &_glfwFocusWindowX11;
        platform->setWindowMonitor = &_glfwSetWindowMonitorX11;
        platform->windowFocused = &_glfwWindowFocusedX11;
        platform->windowIconified = &_glfwWindowIconifiedX11;
        platform->windowVisible = &_glfwWindowVisibleX11;
        platform->windowMaximized = &_glfwWindowMaximizedX11;
        platform->windowHovered = &_glfwWindowHoveredX11;
        platform->framebufferTransparent = &_glfwFramebufferTransparentX11;
        platform->getWindowOpacity = &_glfwGetWindowOpacityX11;
        platform->setWindowResizable = &_glfwSetWindowResizableX11;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedX11;
        platform->setWindowFloating = &_glfwSetWindowFloatingX11;
        platform->setWindowOpacity = &_glfwSetWindowOpacityX11;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughX11;
        platform->pollEvents = &_glfwPollEventsX11;
        platform->waitEvents = &_glfwWaitEventsX11;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutX11;
        platform->postEmptyEvent = &_glfwPostEmptyEventX11;
        platform->getEGLPlatform = &_glfwGetEGLPlatformX11;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayX11;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowX11;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsX11;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportX11;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceX11;
    }

    static int _glfwConnectX11(int platformID, _GLFWplatform* platform)
    {
        if (!OperatingSystem.IsLinux())
        {
            if (platformID == GLFW_PLATFORM_X11)
                _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not available on this system");

            return GLFW_FALSE;
        }

        var module = x11_loadModule("libX11.so.6");
        if (module == null)
        {
            if (platformID == GLFW_PLATFORM_X11)
                _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to load Xlib");

            return GLFW_FALSE;
        }

        var XInitThreads = (delegate* unmanaged<int>)x11_getModuleSymbol(module, "XInitThreads");
        var XrmInitialize = (delegate* unmanaged<void>)x11_getModuleSymbol(module, "XrmInitialize");
        var XOpenDisplay = (delegate* unmanaged<byte*, void*>)x11_getModuleSymbol(module, "XOpenDisplay");
        var XCloseDisplay = (delegate* unmanaged<void*, int>)x11_getModuleSymbol(module, "XCloseDisplay");
        var XConnectionNumber = (delegate* unmanaged<void*, int>)x11_getModuleSymbol(module, "XConnectionNumber");
        var XFree = (delegate* unmanaged<void*, int>)x11_getModuleSymbol(module, "XFree");
        var XDefaultScreen = (delegate* unmanaged<void*, int>)x11_getModuleSymbol(module, "XDefaultScreen");
        var XRootWindow = (delegate* unmanaged<void*, int, nuint>)x11_getModuleSymbol(module, "XRootWindow");

        if (XInitThreads == null || XrmInitialize == null || XOpenDisplay == null || XCloseDisplay == null)
        {
            if (platformID == GLFW_PLATFORM_X11)
                _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to load Xlib entry point");

            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        XInitThreads();
        XrmInitialize();

        var display = XOpenDisplay(null);
        if (display == null)
        {
            if (platformID == GLFW_PLATFORM_X11)
            {
                var name = Environment.GetEnvironmentVariable("DISPLAY");
                if (!string.IsNullOrEmpty(name))
                    _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Failed to open display {0}", name);
                else
                    _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: The DISPLAY environment variable is missing");
            }

            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        _glfw.x11.handle = module;
        _glfw.x11.display = display;
        _glfw.x11.XCloseDisplay = XCloseDisplay;
        _glfw.x11.XConnectionNumber = XConnectionNumber;
        _glfw.x11.XFree = XFree;
        _glfw.x11.XDefaultScreen = XDefaultScreen;
        _glfw.x11.XRootWindow = XRootWindow;
        x11_setPlatformCallbacks(platform);
        return GLFW_TRUE;
    }

    static int _glfwInitX11()
    {
        if (_glfw.x11.display == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Display connection is not open");
            return GLFW_FALSE;
        }

        if (_glfw.x11.XDefaultScreen != null)
            _glfw.x11.screen = _glfw.x11.XDefaultScreen(_glfw.x11.display);

        if (_glfw.x11.XRootWindow != null)
            _glfw.x11.root = _glfw.x11.XRootWindow(_glfw.x11.display, _glfw.x11.screen);

        _glfw.x11.XDefaultVisual =
            (delegate* unmanaged<void*, int, void*>)x11_getModuleSymbol(_glfw.x11.handle, "XDefaultVisual");
        _glfw.x11.XDefaultDepth =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDefaultDepth");
        _glfw.x11.XDisplayWidth =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDisplayWidth");
        _glfw.x11.XDisplayHeight =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDisplayHeight");
        _glfw.x11.XDisplayWidthMM =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDisplayWidthMM");
        _glfw.x11.XDisplayHeightMM =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDisplayHeightMM");
        _glfw.x11.XDisplayKeycodes =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDisplayKeycodes");
        _glfw.x11.XkbKeycodeToKeysym =
            (delegate* unmanaged<void*, uint, int, int, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XkbKeycodeToKeysym");
        _glfw.x11.XkbQueryExtension =
            (delegate* unmanaged<void*, int*, int*, int*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XkbQueryExtension");
        _glfw.x11.XkbSetDetectableAutoRepeat =
            (delegate* unmanaged<void*, int, int*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XkbSetDetectableAutoRepeat");
        _glfw.x11.XkbGetState =
            (delegate* unmanaged<void*, uint, XkbStateRec*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XkbGetState");
        _glfw.x11.XkbSelectEventDetails =
            (delegate* unmanaged<void*, uint, uint, ulong, ulong, int>)x11_getModuleSymbol(_glfw.x11.handle, "XkbSelectEventDetails");
        _glfw.x11.XQueryExtension =
            (delegate* unmanaged<void*, byte*, int*, int*, int*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XQueryExtension");
        _glfw.x11.XGetEventData =
            (delegate* unmanaged<void*, XEvent*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XGetEventData");
        _glfw.x11.XFreeEventData =
            (delegate* unmanaged<void*, XEvent*, void>)x11_getModuleSymbol(_glfw.x11.handle, "XFreeEventData");
        _glfw.x11.XCreateRegion =
            (delegate* unmanaged<void*>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateRegion");
        _glfw.x11.XDestroyRegion =
            (delegate* unmanaged<void*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDestroyRegion");
        _glfw.x11.XCreateColormap =
            (delegate* unmanaged<void*, nuint, void*, int, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateColormap");
        _glfw.x11.XCreateWindow =
            (delegate* unmanaged<void*, nuint, int, int, uint, uint, uint, int, uint, void*, nuint, XSetWindowAttributes*, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateWindow");
        _glfw.x11.XCreateSimpleWindow =
            (delegate* unmanaged<void*, nuint, int, int, uint, uint, uint, ulong, ulong, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateSimpleWindow");
        _glfw.x11.XDestroyWindow =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDestroyWindow");
        _glfw.x11.XFreeColormap =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XFreeColormap");
        _glfw.x11.XStoreName =
            (delegate* unmanaged<void*, nuint, byte*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XStoreName");
        _glfw.x11.Xutf8SetWMProperties =
            (delegate* unmanaged<void*, nuint, byte*, byte*, byte**, int, void*, void*, void*, void>)x11_getModuleSymbol(_glfw.x11.handle, "Xutf8SetWMProperties");
        _glfw.x11.XOpenIM =
            (delegate* unmanaged<void*, void*, byte*, byte*, void*>)x11_getModuleSymbol(_glfw.x11.handle, "XOpenIM");
        _glfw.x11.XCloseIM =
            (delegate* unmanaged<void*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XCloseIM");
        _glfw.x11.XCreateIC =
            (delegate* unmanaged<void*, byte*, nuint, byte*, nuint, byte*, nuint, void*, void*>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateIC");
        _glfw.x11.XDestroyIC =
            (delegate* unmanaged<void*, void>)x11_getModuleSymbol(_glfw.x11.handle, "XDestroyIC");
        _glfw.x11.XGetICValues =
            (delegate* unmanaged<void*, byte*, ulong*, void*, byte*>)x11_getModuleSymbol(_glfw.x11.handle, "XGetICValues");
        _glfw.x11.XSetICFocus =
            (delegate* unmanaged<void*, void>)x11_getModuleSymbol(_glfw.x11.handle, "XSetICFocus");
        _glfw.x11.XUnsetICFocus =
            (delegate* unmanaged<void*, void>)x11_getModuleSymbol(_glfw.x11.handle, "XUnsetICFocus");
        _glfw.x11.Xutf8LookupString =
            (delegate* unmanaged<void*, XEvent*, byte*, int, nuint*, int*, int>)x11_getModuleSymbol(_glfw.x11.handle, "Xutf8LookupString");
        _glfw.x11.XFilterEvent =
            (delegate* unmanaged<XEvent*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XFilterEvent");
        _glfw.x11.XChangeProperty =
            (delegate* unmanaged<void*, nuint, nuint, nuint, int, int, byte*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XChangeProperty");
        _glfw.x11.XGetWindowProperty =
            (delegate* unmanaged<void*, nuint, nuint, nint, nint, int, nuint, nuint*, int*, nuint*, nuint*, byte**, int>)x11_getModuleSymbol(_glfw.x11.handle, "XGetWindowProperty");
        _glfw.x11.XDeleteProperty =
            (delegate* unmanaged<void*, nuint, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDeleteProperty");
        _glfw.x11.XInternAtom =
            (delegate* unmanaged<void*, byte*, int, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XInternAtom");
        _glfw.x11.XConvertSelection =
            (delegate* unmanaged<void*, nuint, nuint, nuint, nuint, ulong, int>)x11_getModuleSymbol(_glfw.x11.handle, "XConvertSelection");
        _glfw.x11.XSetSelectionOwner =
            (delegate* unmanaged<void*, nuint, nuint, ulong, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSetSelectionOwner");
        _glfw.x11.XGetSelectionOwner =
            (delegate* unmanaged<void*, nuint, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XGetSelectionOwner");
        _glfw.x11.XSendEvent =
            (delegate* unmanaged<void*, nuint, int, nint, XEvent*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSendEvent");
        _glfw.x11.XSetWMNormalHints =
            (delegate* unmanaged<void*, nuint, XSizeHints*, void>)x11_getModuleSymbol(_glfw.x11.handle, "XSetWMNormalHints");
        _glfw.x11.XSetWMProtocols =
            (delegate* unmanaged<void*, nuint, nuint*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSetWMProtocols");
        _glfw.x11.XSetClassHint =
            (delegate* unmanaged<void*, nuint, XClassHint*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSetClassHint");
        _glfw.x11.XSelectInput =
            (delegate* unmanaged<void*, nuint, nint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSelectInput");
        _glfw.x11.XMapWindow =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XMapWindow");
        _glfw.x11.XUnmapWindow =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XUnmapWindow");
        _glfw.x11.XMoveWindow =
            (delegate* unmanaged<void*, nuint, int, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XMoveWindow");
        _glfw.x11.XResizeWindow =
            (delegate* unmanaged<void*, nuint, uint, uint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XResizeWindow");
        _glfw.x11.XMoveResizeWindow =
            (delegate* unmanaged<void*, nuint, int, int, uint, uint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XMoveResizeWindow");
        _glfw.x11.XRaiseWindow =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XRaiseWindow");
        _glfw.x11.XSetInputFocus =
            (delegate* unmanaged<void*, nuint, int, ulong, int>)x11_getModuleSymbol(_glfw.x11.handle, "XSetInputFocus");
        _glfw.x11.XIconifyWindow =
            (delegate* unmanaged<void*, nuint, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XIconifyWindow");
        _glfw.x11.XQueryPointer =
            (delegate* unmanaged<void*, nuint, nuint*, nuint*, int*, int*, int*, int*, uint*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XQueryPointer");
        _glfw.x11.XTranslateCoordinates =
            (delegate* unmanaged<void*, nuint, nuint, int, int, int*, int*, nuint*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XTranslateCoordinates");
        _glfw.x11.XWarpPointer =
            (delegate* unmanaged<void*, nuint, nuint, int, int, uint, uint, int, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XWarpPointer");
        _glfw.x11.XCreateFontCursor =
            (delegate* unmanaged<void*, uint, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XCreateFontCursor");
        _glfw.x11.XVisualIDFromVisual =
            (delegate* unmanaged<void*, nuint>)x11_getModuleSymbol(_glfw.x11.handle, "XVisualIDFromVisual");
        _glfw.x11.XFreeCursor =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XFreeCursor");
        _glfw.x11.XDefineCursor =
            (delegate* unmanaged<void*, nuint, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XDefineCursor");
        _glfw.x11.XUndefineCursor =
            (delegate* unmanaged<void*, nuint, int>)x11_getModuleSymbol(_glfw.x11.handle, "XUndefineCursor");
        _glfw.x11.XPending =
            (delegate* unmanaged<void*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XPending");
        _glfw.x11.XEventsQueued =
            (delegate* unmanaged<void*, int, int>)x11_getModuleSymbol(_glfw.x11.handle, "XEventsQueued");
        _glfw.x11.XNextEvent =
            (delegate* unmanaged<void*, XEvent*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XNextEvent");
        _glfw.x11.XPeekEvent =
            (delegate* unmanaged<void*, XEvent*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XPeekEvent");
        _glfw.x11.XLookupString =
            (delegate* unmanaged<XEvent*, byte*, int, nuint*, void*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XLookupString");
        _glfw.x11.XFlush =
            (delegate* unmanaged<void*, int>)x11_getModuleSymbol(_glfw.x11.handle, "XFlush");

        if (_glfw.x11.XDefaultVisual == null ||
            _glfw.x11.XDefaultDepth == null ||
            _glfw.x11.XDisplayWidth == null ||
            _glfw.x11.XDisplayHeight == null ||
            _glfw.x11.XCreateColormap == null ||
            _glfw.x11.XCreateWindow == null ||
            _glfw.x11.XDestroyWindow == null ||
            _glfw.x11.XFreeColormap == null ||
            _glfw.x11.XInternAtom == null ||
            _glfw.x11.XSetWMProtocols == null ||
            _glfw.x11.XMapWindow == null ||
            _glfw.x11.XUnmapWindow == null ||
            _glfw.x11.XPending == null ||
            _glfw.x11.XNextEvent == null ||
            _glfw.x11.XFlush == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to load required Xlib entry points");
            return GLFW_FALSE;
        }

        _glfw.x11.WM_PROTOCOLS = x11_internAtom("WM_PROTOCOLS");
        _glfw.x11.WM_DELETE_WINDOW = x11_internAtom("WM_DELETE_WINDOW");
        _glfw.x11.NET_WM_PING = x11_internAtom("_NET_WM_PING");
        _glfw.x11.WM_STATE = x11_internAtom("WM_STATE");
        _glfw.x11.NET_WM_NAME = x11_internAtom("_NET_WM_NAME");
        _glfw.x11.NET_WM_ICON_NAME = x11_internAtom("_NET_WM_ICON_NAME");
        _glfw.x11.NET_WM_ICON = x11_internAtom("_NET_WM_ICON");
        _glfw.x11.NET_WM_STATE = x11_internAtom("_NET_WM_STATE");
        _glfw.x11.NET_ACTIVE_WINDOW = x11_internAtom("_NET_ACTIVE_WINDOW");
        _glfw.x11.NET_WM_STATE_ABOVE = x11_internAtom("_NET_WM_STATE_ABOVE");
        _glfw.x11.NET_WM_STATE_DEMANDS_ATTENTION = x11_internAtom("_NET_WM_STATE_DEMANDS_ATTENTION");
        _glfw.x11.NET_WM_STATE_MAXIMIZED_VERT = x11_internAtom("_NET_WM_STATE_MAXIMIZED_VERT");
        _glfw.x11.NET_WM_STATE_MAXIMIZED_HORZ = x11_internAtom("_NET_WM_STATE_MAXIMIZED_HORZ");
        _glfw.x11.NET_WM_STATE_FULLSCREEN = x11_internAtom("_NET_WM_STATE_FULLSCREEN");
        _glfw.x11.NET_WM_FULLSCREEN_MONITORS = x11_internAtom("_NET_WM_FULLSCREEN_MONITORS");
        _glfw.x11.NET_WM_WINDOW_OPACITY = x11_internAtom("_NET_WM_WINDOW_OPACITY");
        _glfw.x11.NET_WM_BYPASS_COMPOSITOR = x11_internAtom("_NET_WM_BYPASS_COMPOSITOR");
        _glfw.x11.NET_FRAME_EXTENTS = x11_internAtom("_NET_FRAME_EXTENTS");
        _glfw.x11.NET_REQUEST_FRAME_EXTENTS = x11_internAtom("_NET_REQUEST_FRAME_EXTENTS");
        _glfw.x11.NET_WORKAREA = x11_internAtom("_NET_WORKAREA");
        _glfw.x11.NET_CURRENT_DESKTOP = x11_internAtom("_NET_CURRENT_DESKTOP");
        _glfw.x11.MOTIF_WM_HINTS = x11_internAtom("_MOTIF_WM_HINTS");
        _glfw.x11.PRIMARY = x11_internAtom("PRIMARY");
        _glfw.x11.CLIPBOARD = x11_internAtom("CLIPBOARD");
        _glfw.x11.CLIPBOARD_MANAGER = x11_internAtom("CLIPBOARD_MANAGER");
        _glfw.x11.TARGETS = x11_internAtom("TARGETS");
        _glfw.x11.MULTIPLE = x11_internAtom("MULTIPLE");
        _glfw.x11.ATOM_PAIR = x11_internAtom("ATOM_PAIR");
        _glfw.x11.SAVE_TARGETS = x11_internAtom("SAVE_TARGETS");
        _glfw.x11.TEXT = x11_internAtom("TEXT");
        _glfw.x11.NULL_ = x11_internAtom("NULL");
        _glfw.x11.INCR = x11_internAtom("INCR");
        _glfw.x11.GLFW_SELECTION = x11_internAtom("GLFW_SELECTION");
        _glfw.x11.GLFW_EMPTY_EVENT = x11_internAtom("GLFW_EMPTY_EVENT");
        _glfw.x11.UTF8_STRING = x11_internAtom("UTF8_STRING");
        _glfw.x11.XdndAware = x11_internAtom("XdndAware");
        _glfw.x11.XdndEnter = x11_internAtom("XdndEnter");
        _glfw.x11.XdndPosition = x11_internAtom("XdndPosition");
        _glfw.x11.XdndStatus = x11_internAtom("XdndStatus");
        _glfw.x11.XdndActionCopy = x11_internAtom("XdndActionCopy");
        _glfw.x11.XdndDrop = x11_internAtom("XdndDrop");
        _glfw.x11.XdndFinished = x11_internAtom("XdndFinished");
        _glfw.x11.XdndSelection = x11_internAtom("XdndSelection");
        _glfw.x11.XdndTypeList = x11_internAtom("XdndTypeList");
        _glfw.x11.text_uri_list = x11_internAtom("text/uri-list");

        _glfw.x11.contentScaleX = 1f;
        _glfw.x11.contentScaleY = 1f;
        x11_createHelperWindow();
        x11_initXcursor();
        x11_initRandR();
        x11_initXinerama();
        x11_initXShape();
        x11_initXRender();
        x11_initXI();
        x11_initXkb();
        x11_initX11XCB();
        x11_initXIM();
        x11_createHiddenCursor();
        x11_createKeyTables();
        _glfwPollMonitorsX11();
        return GLFW_TRUE;
    }

    static void _glfwTerminateX11()
    {
        _glfwTerminateGLX();

        if (_glfw.x11.display != null &&
            _glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.CLIPBOARD != 0 &&
            _glfw.x11.XGetSelectionOwner != null &&
            _glfw.x11.XGetSelectionOwner(_glfw.x11.display, _glfw.x11.CLIPBOARD) == _glfw.x11.helperWindowHandle)
        {
            _glfwPushSelectionToManagerX11();
        }

        if (_glfw.x11.im != null && _glfw.x11.XCloseIM != null)
        {
            _glfw.x11.XCloseIM(_glfw.x11.im);
            _glfw.x11.im = null;
        }

        if (_glfw.x11.display != null &&
            _glfw.x11.hiddenCursorHandle != 0 &&
            _glfw.x11.XFreeCursor != null)
        {
            _glfw.x11.XFreeCursor(_glfw.x11.display, _glfw.x11.hiddenCursorHandle);
        }

        if (_glfw.x11.display != null &&
            _glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.XDestroyWindow != null)
        {
            _glfw.x11.XDestroyWindow(_glfw.x11.display, _glfw.x11.helperWindowHandle);
        }

        if (_glfw.x11.display != null && _glfw.x11.XCloseDisplay != null)
            _glfw.x11.XCloseDisplay(_glfw.x11.display);

        _glfw_free(_glfw.x11.clipboardString);
        _glfw_free(_glfw.x11.primarySelectionString);

        if (_glfw.x11.xcursorHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.xcursorHandle);

        if (_glfw.x11.randrHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.randrHandle);

        if (_glfw.x11.xineramaHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.xineramaHandle);

        if (_glfw.x11.x11xcbHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.x11xcbHandle);

        if (_glfw.x11.xshapeHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.xshapeHandle);

        if (_glfw.x11.xrenderHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.xrenderHandle);

        if (_glfw.x11.xiHandle != null)
            _glfwPlatformFreeModule(_glfw.x11.xiHandle);

        if (_glfw.x11.handle != null)
            _glfwPlatformFreeModule(_glfw.x11.handle);

        _glfw.x11 = default;
    }

    static void _glfwInputErrorX11(int error, string message)
    {
        _glfwInputError(error, message);
    }

    static void _glfwGrabErrorHandlerX11()
    {
    }

    static void _glfwReleaseErrorHandlerX11()
    {
    }

    static void _glfwFreeMonitorX11(_GLFWmonitor* monitor)
    {
    }

    static void _glfwGetMonitorPosX11(_GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        if (monitor->x11.crtc != 0 && x11_randrAvailable() != 0)
        {
            var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
            if (resources != null)
            {
                var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);
                if (crtcInfo != null)
                {
                    if (xpos != null)
                        *xpos = crtcInfo->x;
                    if (ypos != null)
                        *ypos = crtcInfo->y;

                    _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                    _glfw.x11.XRRFreeScreenResources(resources);
                    return;
                }

                _glfw.x11.XRRFreeScreenResources(resources);
            }
        }

        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;
    }

    static void _glfwGetMonitorContentScaleX11(_GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwGetMonitorWorkareaX11(_GLFWmonitor* monitor, int* xpos, int* ypos, int* width, int* height)
    {
        var areaX = 0;
        var areaY = 0;
        var areaWidth = monitor->currentMode.width;
        var areaHeight = monitor->currentMode.height;

        if (monitor->x11.crtc != 0 && x11_randrAvailable() != 0)
        {
            var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
            if (resources != null)
            {
                var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);
                if (crtcInfo != null)
                {
                    areaX = crtcInfo->x;
                    areaY = crtcInfo->y;
                    areaWidth = (int)crtcInfo->width;
                    areaHeight = (int)crtcInfo->height;
                    _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                }

                _glfw.x11.XRRFreeScreenResources(resources);
            }
        }

        if (_glfw.x11.NET_WORKAREA != 0 && _glfw.x11.NET_CURRENT_DESKTOP != 0)
        {
            byte* extentsData = null;
            byte* desktopData = null;
            var extentCount = x11_getWindowProperty(_glfw.x11.root,
                _glfw.x11.NET_WORKAREA,
                XA_CARDINAL,
                GLFW_FALSE,
                &extentsData);
            var desktopCount = x11_getWindowProperty(_glfw.x11.root,
                _glfw.x11.NET_CURRENT_DESKTOP,
                XA_CARDINAL,
                GLFW_FALSE,
                &desktopData);

            if (extentCount >= 4 && desktopCount > 0 && extentsData != null && desktopData != null)
            {
                var extents = (nuint*)extentsData;
                var desktop = (int)*(nuint*)desktopData;
                if (desktop >= 0 && (nuint)(desktop * 4 + 3) < extentCount)
                {
                    var globalX = (int)extents[desktop * 4];
                    var globalY = (int)extents[desktop * 4 + 1];
                    var globalWidth = (int)extents[desktop * 4 + 2];
                    var globalHeight = (int)extents[desktop * 4 + 3];

                    if (areaX < globalX)
                    {
                        areaWidth -= globalX - areaX;
                        areaX = globalX;
                    }

                    if (areaY < globalY)
                    {
                        areaHeight -= globalY - areaY;
                        areaY = globalY;
                    }

                    if (areaX + areaWidth > globalX + globalWidth)
                        areaWidth = globalX - areaX + globalWidth;
                    if (areaY + areaHeight > globalY + globalHeight)
                        areaHeight = globalY - areaY + globalHeight;
                }
            }

            if (extentsData != null && _glfw.x11.XFree != null)
                _glfw.x11.XFree(extentsData);
            if (desktopData != null && _glfw.x11.XFree != null)
                _glfw.x11.XFree(desktopData);
        }

        if (xpos != null)
            *xpos = areaX;
        if (ypos != null)
            *ypos = areaY;
        if (width != null)
            *width = areaWidth;
        if (height != null)
            *height = areaHeight;
    }

    static GLFWvidmode* _glfwGetVideoModesX11(_GLFWmonitor* monitor, int* found)
    {
        if (monitor->x11.output != 0 && monitor->x11.crtc != 0 && x11_randrAvailable() != 0)
        {
            var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
            if (resources != null)
            {
                var outputInfo = _glfw.x11.XRRGetOutputInfo(_glfw.x11.display, resources, monitor->x11.output);
                var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);

                if (outputInfo != null && crtcInfo != null && outputInfo->nmode > 0)
                {
                    var randrModes = (GLFWvidmode*)_glfw_calloc((nuint)outputInfo->nmode, (nuint)sizeof(GLFWvidmode));
                    if (randrModes == null)
                    {
                        if (found != null)
                            *found = 0;
                        if (outputInfo != null)
                            _glfw.x11.XRRFreeOutputInfo(outputInfo);
                        if (crtcInfo != null)
                            _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                        _glfw.x11.XRRFreeScreenResources(resources);
                        return null;
                    }

                    var count = 0;
                    for (var i = 0; i < outputInfo->nmode; i++)
                    {
                        var modeInfo = x11_getModeInfo(resources, outputInfo->modes[i]);
                        if (x11_modeIsGood(modeInfo) == 0)
                            continue;

                        var mode = x11_vidmodeFromModeInfo(modeInfo, crtcInfo);
                        var duplicate = GLFW_FALSE;
                        for (var j = 0; j < count; j++)
                        {
                            if (_glfwCompareVideoModes(randrModes + j, &mode) == 0)
                            {
                                duplicate = GLFW_TRUE;
                                break;
                            }
                        }

                        if (duplicate != 0)
                            continue;

                        randrModes[count++] = mode;
                    }

                    _glfw.x11.XRRFreeOutputInfo(outputInfo);
                    _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                    _glfw.x11.XRRFreeScreenResources(resources);

                    if (count > 0)
                    {
                        if (found != null)
                            *found = count;
                        return randrModes;
                    }

                    _glfw_free(randrModes);
                }
                else
                {
                    if (outputInfo != null)
                        _glfw.x11.XRRFreeOutputInfo(outputInfo);
                    if (crtcInfo != null)
                        _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                    _glfw.x11.XRRFreeScreenResources(resources);
                }
            }
        }

        var modes = (GLFWvidmode*)_glfw_calloc(1, (nuint)sizeof(GLFWvidmode));
        if (modes == null)
        {
            if (found != null)
                *found = 0;
            return null;
        }

        *modes = monitor->currentMode;
        if (found != null)
            *found = 1;
        return modes;
    }

    static int _glfwGetVideoModeX11(_GLFWmonitor* monitor, GLFWvidmode* mode)
    {
        if (monitor->x11.crtc != 0 && x11_randrAvailable() != 0)
        {
            var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
            if (resources != null)
            {
                var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);
                if (crtcInfo != null)
                {
                    var modeInfo = x11_getModeInfo(resources, crtcInfo->mode);
                    if (modeInfo != null)
                    {
                        if (mode != null)
                            *mode = x11_vidmodeFromModeInfo(modeInfo, crtcInfo);

                        _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                        _glfw.x11.XRRFreeScreenResources(resources);
                        return GLFW_TRUE;
                    }

                    _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                }

                _glfw.x11.XRRFreeScreenResources(resources);
            }
        }

        if (mode != null)
            *mode = monitor->currentMode;
        return GLFW_TRUE;
    }

    static int _glfwGetGammaRampX11(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (monitor->x11.crtc == 0 ||
            _glfw.x11.XRRGetCrtcGammaSize == null ||
            _glfw.x11.XRRGetCrtcGamma == null ||
            _glfw.x11.XRRFreeGamma == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Gamma ramp access not supported by server");
            return GLFW_FALSE;
        }

        var size = _glfw.x11.XRRGetCrtcGammaSize(_glfw.x11.display, monitor->x11.crtc);
        if (size <= 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to query gamma ramp size");
            return GLFW_FALSE;
        }

        var gamma = _glfw.x11.XRRGetCrtcGamma(_glfw.x11.display, monitor->x11.crtc);
        if (gamma == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to query gamma ramp");
            return GLFW_FALSE;
        }

        _glfwAllocGammaArrays(ramp, (uint)size);
        _glfw_memcpy(ramp->red, gamma->red, (nuint)(size * sizeof(ushort)));
        _glfw_memcpy(ramp->green, gamma->green, (nuint)(size * sizeof(ushort)));
        _glfw_memcpy(ramp->blue, gamma->blue, (nuint)(size * sizeof(ushort)));

        _glfw.x11.XRRFreeGamma(gamma);
        return GLFW_TRUE;
    }

    static void _glfwSetGammaRampX11(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (monitor->x11.crtc == 0 ||
            _glfw.x11.XRRGetCrtcGammaSize == null ||
            _glfw.x11.XRRAllocGamma == null ||
            _glfw.x11.XRRSetCrtcGamma == null ||
            _glfw.x11.XRRFreeGamma == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Gamma ramp access not supported by server");
            return;
        }

        if (_glfw.x11.XRRGetCrtcGammaSize(_glfw.x11.display, monitor->x11.crtc) != ramp->size)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Gamma ramp size must match current ramp size");
            return;
        }

        var gamma = _glfw.x11.XRRAllocGamma((int)ramp->size);
        if (gamma == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to allocate gamma ramp");
            return;
        }

        _glfw_memcpy(gamma->red, ramp->red, (nuint)(ramp->size * sizeof(ushort)));
        _glfw_memcpy(gamma->green, ramp->green, (nuint)(ramp->size * sizeof(ushort)));
        _glfw_memcpy(gamma->blue, ramp->blue, (nuint)(ramp->size * sizeof(ushort)));

        _glfw.x11.XRRSetCrtcGamma(_glfw.x11.display, monitor->x11.crtc, gamma);
        _glfw.x11.XRRFreeGamma(gamma);
    }

    static int _glfwIsVisualTransparentX11(void* visual)
    {
        if (visual == null || _glfw.x11.XRenderFindVisualFormat == null)
            return GLFW_FALSE;

        var format = _glfw.x11.XRenderFindVisualFormat(_glfw.x11.display, visual);
        return format != null && format->direct.alphaMask != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwGetEGLPlatformX11(int** attribs)
    {
        if (attribs != null)
            *attribs = null;
        return 0;
    }

    static void* _glfwGetEGLNativeDisplayX11()
    {
        return _glfw.x11.display;
    }

    static void* _glfwGetEGLNativeWindowX11(_GLFWwindow* window)
    {
        return window != null ? (void*)window->x11.handle : null;
    }

    static int x11_xcbVulkanSurfaceAvailable()
    {
        return _glfwInitHints.x11.xcbVulkanSurface != 0 &&
               _glfw.x11.x11xcbConnection != null &&
               _glfw.vk.KHR_xcb_surface != 0
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static int x11_usingXcbVulkanSurface()
    {
        return _glfw.vk.extensions != null &&
               _glfw.vk.extensions[1] == _glfwVkKHRXcbSurfaceExtensionName
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static void _glfwGetRequiredInstanceExtensionsX11(byte** extensions)
    {
        if (extensions == null)
            return;

        if (_glfw.vk.KHR_surface == 0)
            return;

        byte* surfaceExtension = null;

        if (x11_xcbVulkanSurfaceAvailable() != 0)
            surfaceExtension = _glfwVkKHRXcbSurfaceExtensionName;
        else if (_glfw.vk.KHR_xlib_surface != 0)
            surfaceExtension = _glfwVkKHRXlibSurfaceExtensionName;

        if (surfaceExtension == null)
            return;

        extensions[0] = _glfwVkKHRSurfaceExtensionName;
        extensions[1] = surfaceExtension;
    }

    static int _glfwGetPhysicalDevicePresentationSupportX11(void* instance, void* device, uint queuefamily)
    {
        var visual = _glfw.x11.XDefaultVisual != null
            ? _glfw.x11.XDefaultVisual(_glfw.x11.display, _glfw.x11.screen)
            : null;
        var visualID = visual != null && _glfw.x11.XVisualIDFromVisual != null
            ? _glfw.x11.XVisualIDFromVisual(visual)
            : 0;

        if (x11_usingXcbVulkanSurface() != 0)
        {
            var vkGetPhysicalDeviceXcbPresentationSupportKHR =
                (delegate* unmanaged<void*, uint, void*, uint, int>)
                vulkan_getInstanceProcAddress(instance, "vkGetPhysicalDeviceXcbPresentationSupportKHR");
            if (vkGetPhysicalDeviceXcbPresentationSupportKHR == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "X11: Vulkan instance missing VK_KHR_xcb_surface extension");
                return GLFW_FALSE;
            }

            return vkGetPhysicalDeviceXcbPresentationSupportKHR(device,
                queuefamily,
                _glfw.x11.x11xcbConnection,
                (uint)visualID);
        }

        var vkGetPhysicalDeviceXlibPresentationSupportKHR =
            (delegate* unmanaged<void*, uint, void*, nuint, int>)
            vulkan_getInstanceProcAddress(instance, "vkGetPhysicalDeviceXlibPresentationSupportKHR");
        if (vkGetPhysicalDeviceXlibPresentationSupportKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "X11: Vulkan instance missing VK_KHR_xlib_surface extension");
            return GLFW_FALSE;
        }

        return vkGetPhysicalDeviceXlibPresentationSupportKHR(device,
            queuefamily,
            _glfw.x11.display,
            visualID);
    }

    static int _glfwCreateWindowSurfaceX11(void* instance, _GLFWwindow* window, void* allocator, ulong* surface)
    {
        if (x11_usingXcbVulkanSurface() != 0)
        {
            var vkCreateXcbSurfaceKHR =
                (delegate* unmanaged<void*, VkXcbSurfaceCreateInfoKHR*, void*, ulong*, int>)
                vulkan_getInstanceProcAddress(instance, "vkCreateXcbSurfaceKHR");
            if (vkCreateXcbSurfaceKHR == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "X11: Vulkan instance missing VK_KHR_xcb_surface extension");
                return VK_ERROR_EXTENSION_NOT_PRESENT;
            }

            var xcbSci = new VkXcbSurfaceCreateInfoKHR
            {
                sType = VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR,
                connection = _glfw.x11.x11xcbConnection,
                window = (uint)window->x11.handle
            };

            var xcbErr = vkCreateXcbSurfaceKHR(instance, &xcbSci, allocator, surface);
            if (xcbErr != VK_SUCCESS)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR,
                    "X11: Failed to create Vulkan XCB surface: {0}",
                    vulkan_resultString(xcbErr));
            }

            return xcbErr;
        }

        var vkCreateXlibSurfaceKHR =
            (delegate* unmanaged<void*, VkXlibSurfaceCreateInfoKHR*, void*, ulong*, int>)
            vulkan_getInstanceProcAddress(instance, "vkCreateXlibSurfaceKHR");
        if (vkCreateXlibSurfaceKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "X11: Vulkan instance missing VK_KHR_xlib_surface extension");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        var sci = new VkXlibSurfaceCreateInfoKHR
        {
            sType = VK_STRUCTURE_TYPE_XLIB_SURFACE_CREATE_INFO_KHR,
            dpy = _glfw.x11.display,
            window = window->x11.handle
        };

        var err = vkCreateXlibSurfaceKHR(instance, &sci, allocator, surface);
        if (err != VK_SUCCESS)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "X11: Failed to create Vulkan surface: {0}",
                vulkan_resultString(err));
        }

        return err;
    }
}
