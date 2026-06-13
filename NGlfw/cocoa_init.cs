namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwConnectCocoa(int platformID, _GLFWplatform* platform)
    {
        *platform = default;
        platform->platformID = GLFW_PLATFORM_COCOA;
        platform->init = &_glfwInitCocoa;
        platform->terminate = &_glfwTerminateCocoa;
        platform->getCursorPos = &_glfwGetCursorPosCocoa;
        platform->setCursorPos = &_glfwSetCursorPosCocoa;
        platform->setCursorMode = &_glfwSetCursorModeCocoa;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionCocoa;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedCocoa;
        platform->createCursor = &_glfwCreateCursorCocoa;
        platform->createStandardCursor = &_glfwCreateStandardCursorCocoa;
        platform->destroyCursor = &_glfwDestroyCursorCocoa;
        platform->setCursor = &_glfwSetCursorCocoa;
        platform->getScancodeName = &_glfwGetScancodeNameCocoa;
        platform->getKeyScancode = &_glfwGetKeyScancodeCocoa;
        platform->setClipboardString = &_glfwSetClipboardStringCocoa;
        platform->getClipboardString = &_glfwGetClipboardStringCocoa;
        platform->initJoysticks = &_glfwInitJoysticksCocoa;
        platform->terminateJoysticks = &_glfwTerminateJoysticksCocoa;
        platform->pollJoystick = &_glfwPollJoystickCocoa;
        platform->getMappingName = &_glfwGetMappingNameCocoa;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDCocoa;
        platform->freeMonitor = &_glfwFreeMonitorCocoa;
        platform->getMonitorPos = &_glfwGetMonitorPosCocoa;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleCocoa;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaCocoa;
        platform->getVideoModes = &_glfwGetVideoModesCocoa;
        platform->getVideoMode = &_glfwGetVideoModeCocoa;
        platform->getGammaRamp = &_glfwGetGammaRampCocoa;
        platform->setGammaRamp = &_glfwSetGammaRampCocoa;
        platform->createWindow = &_glfwCreateWindowCocoa;
        platform->destroyWindow = &_glfwDestroyWindowCocoa;
        platform->setWindowTitle = &_glfwSetWindowTitleCocoa;
        platform->setWindowIcon = &_glfwSetWindowIconCocoa;
        platform->getWindowPos = &_glfwGetWindowPosCocoa;
        platform->setWindowPos = &_glfwSetWindowPosCocoa;
        platform->getWindowSize = &_glfwGetWindowSizeCocoa;
        platform->setWindowSize = &_glfwSetWindowSizeCocoa;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsCocoa;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioCocoa;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeCocoa;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeCocoa;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleCocoa;
        platform->iconifyWindow = &_glfwIconifyWindowCocoa;
        platform->restoreWindow = &_glfwRestoreWindowCocoa;
        platform->maximizeWindow = &_glfwMaximizeWindowCocoa;
        platform->showWindow = &_glfwShowWindowCocoa;
        platform->hideWindow = &_glfwHideWindowCocoa;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionCocoa;
        platform->focusWindow = &_glfwFocusWindowCocoa;
        platform->setWindowMonitor = &_glfwSetWindowMonitorCocoa;
        platform->windowFocused = &_glfwWindowFocusedCocoa;
        platform->windowIconified = &_glfwWindowIconifiedCocoa;
        platform->windowVisible = &_glfwWindowVisibleCocoa;
        platform->windowMaximized = &_glfwWindowMaximizedCocoa;
        platform->windowHovered = &_glfwWindowHoveredCocoa;
        platform->framebufferTransparent = &_glfwFramebufferTransparentCocoa;
        platform->getWindowOpacity = &_glfwGetWindowOpacityCocoa;
        platform->setWindowResizable = &_glfwSetWindowResizableCocoa;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedCocoa;
        platform->setWindowFloating = &_glfwSetWindowFloatingCocoa;
        platform->setWindowOpacity = &_glfwSetWindowOpacityCocoa;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughCocoa;
        platform->pollEvents = &_glfwPollEventsCocoa;
        platform->waitEvents = &_glfwWaitEventsCocoa;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutCocoa;
        platform->postEmptyEvent = &_glfwPostEmptyEventCocoa;
        platform->getEGLPlatform = &_glfwGetEGLPlatformCocoa;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayCocoa;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowCocoa;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsCocoa;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportCocoa;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceCocoa;
        return GLFW_TRUE;
    }

    static void cocoa_registerUserDefaults()
    {
        var defaults = cocoa_msgSend_id(cocoa_getClass("NSUserDefaults"), "standardUserDefaults");
        if (defaults == null)
            return;

        var key = cocoa_stringFromUTF8("ApplePressAndHoldEnabled");
        if (key == null)
            return;

        var value = cocoa_msgSend_id_bool(cocoa_getClass("NSNumber"), "numberWithBool:", GLFW_FALSE);
        var dictionary = objc_msgSend_id_ptr_ptr(cocoa_getClass("NSDictionary"),
            cocoa_sel("dictionaryWithObject:forKey:"),
            value,
            key);
        if (dictionary != null)
            cocoa_msgSend_void_ptr(defaults, "registerDefaults:", dictionary);

        cocoa_releaseTemporaryString(key);
    }

    static void cocoa_createKeyTables()
    {
        fixed (short* keycodes = _glfw.ns.keycodes)
        fixed (short* scancodes = _glfw.ns.scancodes)
        {
            _glfw_memset(keycodes, 0xff, 256 * (nuint)sizeof(short));
            _glfw_memset(scancodes, 0xff, (GLFW_KEY_LAST + 1) * (nuint)sizeof(short));

            keycodes[0x1D] = GLFW_KEY_0;
            keycodes[0x12] = GLFW_KEY_1;
            keycodes[0x13] = GLFW_KEY_2;
            keycodes[0x14] = GLFW_KEY_3;
            keycodes[0x15] = GLFW_KEY_4;
            keycodes[0x17] = GLFW_KEY_5;
            keycodes[0x16] = GLFW_KEY_6;
            keycodes[0x1A] = GLFW_KEY_7;
            keycodes[0x1C] = GLFW_KEY_8;
            keycodes[0x19] = GLFW_KEY_9;
            keycodes[0x00] = GLFW_KEY_A;
            keycodes[0x0B] = GLFW_KEY_B;
            keycodes[0x08] = GLFW_KEY_C;
            keycodes[0x02] = GLFW_KEY_D;
            keycodes[0x0E] = GLFW_KEY_E;
            keycodes[0x03] = GLFW_KEY_F;
            keycodes[0x05] = GLFW_KEY_G;
            keycodes[0x04] = GLFW_KEY_H;
            keycodes[0x22] = GLFW_KEY_I;
            keycodes[0x26] = GLFW_KEY_J;
            keycodes[0x28] = GLFW_KEY_K;
            keycodes[0x25] = GLFW_KEY_L;
            keycodes[0x2E] = GLFW_KEY_M;
            keycodes[0x2D] = GLFW_KEY_N;
            keycodes[0x1F] = GLFW_KEY_O;
            keycodes[0x23] = GLFW_KEY_P;
            keycodes[0x0C] = GLFW_KEY_Q;
            keycodes[0x0F] = GLFW_KEY_R;
            keycodes[0x01] = GLFW_KEY_S;
            keycodes[0x11] = GLFW_KEY_T;
            keycodes[0x20] = GLFW_KEY_U;
            keycodes[0x09] = GLFW_KEY_V;
            keycodes[0x0D] = GLFW_KEY_W;
            keycodes[0x07] = GLFW_KEY_X;
            keycodes[0x10] = GLFW_KEY_Y;
            keycodes[0x06] = GLFW_KEY_Z;

            keycodes[0x27] = GLFW_KEY_APOSTROPHE;
            keycodes[0x2A] = GLFW_KEY_BACKSLASH;
            keycodes[0x2B] = GLFW_KEY_COMMA;
            keycodes[0x18] = GLFW_KEY_EQUAL;
            keycodes[0x32] = GLFW_KEY_GRAVE_ACCENT;
            keycodes[0x21] = GLFW_KEY_LEFT_BRACKET;
            keycodes[0x1B] = GLFW_KEY_MINUS;
            keycodes[0x2F] = GLFW_KEY_PERIOD;
            keycodes[0x1E] = GLFW_KEY_RIGHT_BRACKET;
            keycodes[0x29] = GLFW_KEY_SEMICOLON;
            keycodes[0x2C] = GLFW_KEY_SLASH;
            keycodes[0x0A] = GLFW_KEY_WORLD_1;

            keycodes[0x33] = GLFW_KEY_BACKSPACE;
            keycodes[0x39] = GLFW_KEY_CAPS_LOCK;
            keycodes[0x75] = GLFW_KEY_DELETE;
            keycodes[0x7D] = GLFW_KEY_DOWN;
            keycodes[0x77] = GLFW_KEY_END;
            keycodes[0x24] = GLFW_KEY_ENTER;
            keycodes[0x35] = GLFW_KEY_ESCAPE;
            keycodes[0x7A] = GLFW_KEY_F1;
            keycodes[0x78] = GLFW_KEY_F2;
            keycodes[0x63] = GLFW_KEY_F3;
            keycodes[0x76] = GLFW_KEY_F4;
            keycodes[0x60] = GLFW_KEY_F5;
            keycodes[0x61] = GLFW_KEY_F6;
            keycodes[0x62] = GLFW_KEY_F7;
            keycodes[0x64] = GLFW_KEY_F8;
            keycodes[0x65] = GLFW_KEY_F9;
            keycodes[0x6D] = GLFW_KEY_F10;
            keycodes[0x67] = GLFW_KEY_F11;
            keycodes[0x6F] = GLFW_KEY_F12;
            keycodes[0x69] = GLFW_KEY_PRINT_SCREEN;
            keycodes[0x6B] = GLFW_KEY_F14;
            keycodes[0x71] = GLFW_KEY_F15;
            keycodes[0x6A] = GLFW_KEY_F16;
            keycodes[0x40] = GLFW_KEY_F17;
            keycodes[0x4F] = GLFW_KEY_F18;
            keycodes[0x50] = GLFW_KEY_F19;
            keycodes[0x5A] = GLFW_KEY_F20;
            keycodes[0x73] = GLFW_KEY_HOME;
            keycodes[0x72] = GLFW_KEY_INSERT;
            keycodes[0x7B] = GLFW_KEY_LEFT;
            keycodes[0x3A] = GLFW_KEY_LEFT_ALT;
            keycodes[0x3B] = GLFW_KEY_LEFT_CONTROL;
            keycodes[0x38] = GLFW_KEY_LEFT_SHIFT;
            keycodes[0x37] = GLFW_KEY_LEFT_SUPER;
            keycodes[0x6E] = GLFW_KEY_MENU;
            keycodes[0x47] = GLFW_KEY_NUM_LOCK;
            keycodes[0x79] = GLFW_KEY_PAGE_DOWN;
            keycodes[0x74] = GLFW_KEY_PAGE_UP;
            keycodes[0x7C] = GLFW_KEY_RIGHT;
            keycodes[0x3D] = GLFW_KEY_RIGHT_ALT;
            keycodes[0x3E] = GLFW_KEY_RIGHT_CONTROL;
            keycodes[0x3C] = GLFW_KEY_RIGHT_SHIFT;
            keycodes[0x36] = GLFW_KEY_RIGHT_SUPER;
            keycodes[0x31] = GLFW_KEY_SPACE;
            keycodes[0x30] = GLFW_KEY_TAB;
            keycodes[0x7E] = GLFW_KEY_UP;

            keycodes[0x52] = GLFW_KEY_KP_0;
            keycodes[0x53] = GLFW_KEY_KP_1;
            keycodes[0x54] = GLFW_KEY_KP_2;
            keycodes[0x55] = GLFW_KEY_KP_3;
            keycodes[0x56] = GLFW_KEY_KP_4;
            keycodes[0x57] = GLFW_KEY_KP_5;
            keycodes[0x58] = GLFW_KEY_KP_6;
            keycodes[0x59] = GLFW_KEY_KP_7;
            keycodes[0x5B] = GLFW_KEY_KP_8;
            keycodes[0x5C] = GLFW_KEY_KP_9;
            keycodes[0x45] = GLFW_KEY_KP_ADD;
            keycodes[0x41] = GLFW_KEY_KP_DECIMAL;
            keycodes[0x4B] = GLFW_KEY_KP_DIVIDE;
            keycodes[0x4C] = GLFW_KEY_KP_ENTER;
            keycodes[0x51] = GLFW_KEY_KP_EQUAL;
            keycodes[0x43] = GLFW_KEY_KP_MULTIPLY;
            keycodes[0x4E] = GLFW_KEY_KP_SUBTRACT;

            for (var scancode = 0; scancode < 256; scancode++)
            {
                if (keycodes[scancode] >= 0)
                    scancodes[keycodes[scancode]] = (short)scancode;
            }
        }
    }

    static int _glfwInitCocoa()
    {
        if (!OperatingSystem.IsMacOS())
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not available on this system");
            return GLFW_FALSE;
        }

        _glfw.ns.autoreleasePool = cocoa_createAutoreleasePool();
        if (_glfw.ns.autoreleasePool == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create autorelease pool");
            return GLFW_FALSE;
        }

        if (cocoa_registerWindowClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register window class");
            return GLFW_FALSE;
        }

        if (cocoa_registerWindowDelegateClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register window delegate class");
            return GLFW_FALSE;
        }

        if (cocoa_registerContentViewClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register content view class");
            return GLFW_FALSE;
        }

        var app = cocoa_getNSApp();
        var appDelegateClass = cocoa_registerApplicationDelegateClass();
        if (app == null || appDelegateClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to initialize application delegate class");
            return GLFW_FALSE;
        }

        _glfw.ns.delegateObject = cocoa_msgSend_id(cocoa_msgSend_id(appDelegateClass, "alloc"), "init");
        if (_glfw.ns.delegateObject == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create application delegate");
            return GLFW_FALSE;
        }

        cocoa_msgSend_void_ptr(app, "setDelegate:", _glfw.ns.delegateObject);

        cocoa_registerUserDefaults();
        cocoa_createKeyTables();
        _glfwPollMonitorsCocoa();
        return GLFW_TRUE;
    }

    static void _glfwTerminateCocoa()
    {
        cocoa_showCursor();
        _glfwTerminateNSGL();

        _glfw_free(_glfw.ns.clipboardString);
        _glfw.ns.clipboardString = null;

        var app = cocoa_getNSApp();
        cocoa_msgSend_void_ptr(app, "setDelegate:", null);
        cocoa_msgSend_void(_glfw.ns.delegateObject, "release");
        _glfw.ns.delegateObject = null;

        cocoa_drainAutoreleasePool(_glfw.ns.autoreleasePool);
        _glfw.ns.autoreleasePool = null;
    }
}
