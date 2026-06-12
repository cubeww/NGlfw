using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static readonly Guid GUID_DEVINTERFACE_HID = new("4D1E55B2-F16F-11CF-88CB-001111000030");

    static int _glfwConnectWin32(int platformID, _GLFWplatform* platform)
    {
        *platform = default;
        platform->platformID = GLFW_PLATFORM_WIN32;
        platform->init = &_glfwInitWin32;
        platform->terminate = &_glfwTerminateWin32;
        platform->getCursorPos = &_glfwGetCursorPosWin32;
        platform->setCursorPos = &_glfwSetCursorPosWin32;
        platform->setCursorMode = &_glfwSetCursorModeWin32;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionWin32;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedWin32;
        platform->createCursor = &_glfwCreateCursorWin32;
        platform->createStandardCursor = &_glfwCreateStandardCursorWin32;
        platform->destroyCursor = &_glfwDestroyCursorWin32;
        platform->setCursor = &_glfwSetCursorWin32;
        platform->getScancodeName = &_glfwGetScancodeNameWin32;
        platform->getKeyScancode = &_glfwGetKeyScancodeWin32;
        platform->setClipboardString = &_glfwSetClipboardStringWin32;
        platform->getClipboardString = &_glfwGetClipboardStringWin32;
        platform->initJoysticks = &_glfwInitJoysticksWin32;
        platform->terminateJoysticks = &_glfwTerminateJoysticksWin32;
        platform->pollJoystick = &_glfwPollJoystickWin32;
        platform->getMappingName = &_glfwGetMappingNameWin32;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDWin32;
        platform->freeMonitor = &_glfwFreeMonitorWin32;
        platform->getMonitorPos = &_glfwGetMonitorPosWin32;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleWin32;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaWin32;
        platform->getVideoModes = &_glfwGetVideoModesWin32;
        platform->getVideoMode = &_glfwGetVideoModeWin32;
        platform->getGammaRamp = &_glfwGetGammaRampWin32;
        platform->setGammaRamp = &_glfwSetGammaRampWin32;
        platform->createWindow = &_glfwCreateWindowWin32;
        platform->destroyWindow = &_glfwDestroyWindowWin32;
        platform->setWindowTitle = &_glfwSetWindowTitleWin32;
        platform->setWindowIcon = &_glfwSetWindowIconWin32;
        platform->getWindowPos = &_glfwGetWindowPosWin32;
        platform->setWindowPos = &_glfwSetWindowPosWin32;
        platform->getWindowSize = &_glfwGetWindowSizeWin32;
        platform->setWindowSize = &_glfwSetWindowSizeWin32;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsWin32;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioWin32;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeWin32;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeWin32;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleWin32;
        platform->iconifyWindow = &_glfwIconifyWindowWin32;
        platform->restoreWindow = &_glfwRestoreWindowWin32;
        platform->maximizeWindow = &_glfwMaximizeWindowWin32;
        platform->showWindow = &_glfwShowWindowWin32;
        platform->hideWindow = &_glfwHideWindowWin32;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionWin32;
        platform->focusWindow = &_glfwFocusWindowWin32;
        platform->setWindowMonitor = &_glfwSetWindowMonitorWin32;
        platform->windowFocused = &_glfwWindowFocusedWin32;
        platform->windowIconified = &_glfwWindowIconifiedWin32;
        platform->windowVisible = &_glfwWindowVisibleWin32;
        platform->windowMaximized = &_glfwWindowMaximizedWin32;
        platform->windowHovered = &_glfwWindowHoveredWin32;
        platform->framebufferTransparent = &_glfwFramebufferTransparentWin32;
        platform->getWindowOpacity = &_glfwGetWindowOpacityWin32;
        platform->setWindowResizable = &_glfwSetWindowResizableWin32;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedWin32;
        platform->setWindowFloating = &_glfwSetWindowFloatingWin32;
        platform->setWindowOpacity = &_glfwSetWindowOpacityWin32;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughWin32;
        platform->pollEvents = &_glfwPollEventsWin32;
        platform->waitEvents = &_glfwWaitEventsWin32;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutWin32;
        platform->postEmptyEvent = &_glfwPostEmptyEventWin32;
        platform->getEGLPlatform = &_glfwGetEGLPlatformWin32;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayWin32;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowWin32;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsWin32;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportWin32;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceWin32;
        return GLFW_TRUE;
    }

    static int _glfwInitWin32()
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Win32: Platform not available on this system");
            return GLFW_FALSE;
        }

        _glfw.win32.instance = GetModuleHandleW(null);
        if (_glfw.win32.instance == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to retrieve module handle");
            return GLFW_FALSE;
        }

        _glfw.win32.mainThreadId = GetCurrentThreadId();
        win32_createKeyTables();
        win32_setProcessDpiAwareness();
        win32_loadXInput();

        MSG msg;
        PeekMessageW(out msg, 0, WM_USER, WM_USER, PM_NOREMOVE);

        if (win32_createHelperWindow() == 0)
            return GLFW_FALSE;

        var wc = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(win32_windowProcDelegate),
            hInstance = _glfw.win32.instance,
            hIcon = LoadIconW(0, IDI_APPLICATION),
            hCursor = LoadCursorW(0, IDC_ARROW),
            lpszClassName = _GLFW_WIN32_WINDOW_CLASS,
            hIconSm = LoadIconW(0, IDI_APPLICATION)
        };

        _glfw.win32.mainWindowClass = RegisterClassExW(ref wc);
        if (_glfw.win32.mainWindowClass == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to register window class");
            return GLFW_FALSE;
        }

        _glfwPollMonitorsWin32();
        return GLFW_TRUE;
    }

    static void _glfwTerminateWin32()
    {
        lock (win32_windowLock)
            win32_windows.Clear();

        if (_glfw.win32.deviceNotificationHandle != 0)
        {
            UnregisterDeviceNotification(_glfw.win32.deviceNotificationHandle);
            _glfw.win32.deviceNotificationHandle = 0;
        }

        if (_glfw.win32.helperWindowHandle != 0)
        {
            DestroyWindow(_glfw.win32.helperWindowHandle);
            _glfw.win32.helperWindowHandle = 0;
        }

        if (_glfw.win32.helperWindowClass != 0)
        {
            UnregisterClassW(_GLFW_WIN32_HELPER_WINDOW_CLASS, _glfw.win32.instance);
            _glfw.win32.helperWindowClass = 0;
        }

        if (_glfw.win32.mainWindowClass != 0)
            UnregisterClassW(_GLFW_WIN32_WINDOW_CLASS, _glfw.win32.instance);

        _glfw_free(_glfw.win32.clipboardString);
        win32_freeXInput();
        _glfwTerminateWGL();
        _glfwTerminateOSMesa();
        _glfwTerminateEGL();
    }

    static void _glfwInputErrorWin32(int error, string description)
    {
        _glfwInputError(error, description);
    }

    static nint win32_getModuleHandle(string name)
    {
        fixed (char* chars = name)
            return GetModuleHandleW(chars);
    }

    static nint win32_getUser32Symbol(string name)
    {
        var module = win32_getModuleHandle("user32.dll");
        if (module == 0)
            return 0;

        return NativeLibrary.TryGetExport(module, name, out var symbol) ? symbol : 0;
    }

    static void win32_setProcessDpiAwareness()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 15063))
        {
            var setProcessDpiAwarenessContext = win32_getUser32Symbol("SetProcessDpiAwarenessContext");
            if (setProcessDpiAwarenessContext != 0)
            {
                ((delegate* unmanaged[Stdcall]<nint, int>)setProcessDpiAwarenessContext)((nint)(-4));
                return;
            }
        }

        var setProcessDPIAware = win32_getUser32Symbol("SetProcessDPIAware");
        if (setProcessDPIAware != 0)
            ((delegate* unmanaged[Stdcall]<int>)setProcessDPIAware)();
    }

    static nint win32_helperWindowProc(nint hWnd, uint uMsg, nuint wParam, nint lParam)
    {
        if (uMsg == WM_DEVICECHANGE && _glfw.joysticksInitialized != 0)
        {
            if (wParam == DBT_DEVICEARRIVAL)
            {
                var header = (DEV_BROADCAST_HDR*)lParam;
                if (header != null && header->dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
                    _glfwDetectJoystickConnectionWin32();
            }
            else if (wParam == DBT_DEVICEREMOVECOMPLETE)
            {
                var header = (DEV_BROADCAST_HDR*)lParam;
                if (header != null && header->dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
                    _glfwDetectJoystickDisconnectionWin32();
            }
        }

        return DefWindowProcW(hWnd, uMsg, wParam, lParam);
    }

    static int win32_createHelperWindow()
    {
        var wc = new WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
            style = CS_OWNDC,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(win32_helperWindowProcDelegate),
            hInstance = _glfw.win32.instance,
            lpszClassName = _GLFW_WIN32_HELPER_WINDOW_CLASS
        };

        _glfw.win32.helperWindowClass = RegisterClassExW(ref wc);
        if (_glfw.win32.helperWindowClass == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to register helper window class");
            return GLFW_FALSE;
        }

        _glfw.win32.helperWindowHandle = CreateWindowExW(0,
            _GLFW_WIN32_HELPER_WINDOW_CLASS,
            "GLFW message window",
            WS_CLIPSIBLINGS | WS_CLIPCHILDREN,
            0,
            0,
            1,
            1,
            0,
            0,
            _glfw.win32.instance,
            0);

        if (_glfw.win32.helperWindowHandle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create helper window");
            return GLFW_FALSE;
        }

        ShowWindow(_glfw.win32.helperWindowHandle, SW_HIDE);

        var dbi = new DEV_BROADCAST_DEVICEINTERFACE_W
        {
            dbcc_size = (uint)sizeof(DEV_BROADCAST_DEVICEINTERFACE_W),
            dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE,
            dbcc_classguid = GUID_DEVINTERFACE_HID
        };

        _glfw.win32.deviceNotificationHandle = RegisterDeviceNotificationW(
            _glfw.win32.helperWindowHandle,
            &dbi,
            DEVICE_NOTIFY_WINDOW_HANDLE);

        if (_glfw.win32.deviceNotificationHandle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to register device notification");
            return GLFW_FALSE;
        }

        while (PeekMessageW(out var msg, _glfw.win32.helperWindowHandle, 0, 0, PM_REMOVE) != 0)
        {
            TranslateMessage(ref msg);
            DispatchMessageW(ref msg);
        }

        return GLFW_TRUE;
    }

    static void win32_createKeyTables()
    {
        fixed (short* keycodes = _glfw.win32.keycodes)
        fixed (short* scancodes = _glfw.win32.scancodes)
        {
            for (var i = 0; i < 512; i++)
                keycodes[i] = -1;
            for (var i = 0; i <= GLFW_KEY_LAST; i++)
                scancodes[i] = -1;

            win32_mapKey(keycodes, 0x00B, GLFW_KEY_0);
            win32_mapKey(keycodes, 0x002, GLFW_KEY_1);
            win32_mapKey(keycodes, 0x003, GLFW_KEY_2);
            win32_mapKey(keycodes, 0x004, GLFW_KEY_3);
            win32_mapKey(keycodes, 0x005, GLFW_KEY_4);
            win32_mapKey(keycodes, 0x006, GLFW_KEY_5);
            win32_mapKey(keycodes, 0x007, GLFW_KEY_6);
            win32_mapKey(keycodes, 0x008, GLFW_KEY_7);
            win32_mapKey(keycodes, 0x009, GLFW_KEY_8);
            win32_mapKey(keycodes, 0x00A, GLFW_KEY_9);
            win32_mapKey(keycodes, 0x01E, GLFW_KEY_A);
            win32_mapKey(keycodes, 0x030, GLFW_KEY_B);
            win32_mapKey(keycodes, 0x02E, GLFW_KEY_C);
            win32_mapKey(keycodes, 0x020, GLFW_KEY_D);
            win32_mapKey(keycodes, 0x012, GLFW_KEY_E);
            win32_mapKey(keycodes, 0x021, GLFW_KEY_F);
            win32_mapKey(keycodes, 0x022, GLFW_KEY_G);
            win32_mapKey(keycodes, 0x023, GLFW_KEY_H);
            win32_mapKey(keycodes, 0x017, GLFW_KEY_I);
            win32_mapKey(keycodes, 0x024, GLFW_KEY_J);
            win32_mapKey(keycodes, 0x025, GLFW_KEY_K);
            win32_mapKey(keycodes, 0x026, GLFW_KEY_L);
            win32_mapKey(keycodes, 0x032, GLFW_KEY_M);
            win32_mapKey(keycodes, 0x031, GLFW_KEY_N);
            win32_mapKey(keycodes, 0x018, GLFW_KEY_O);
            win32_mapKey(keycodes, 0x019, GLFW_KEY_P);
            win32_mapKey(keycodes, 0x010, GLFW_KEY_Q);
            win32_mapKey(keycodes, 0x013, GLFW_KEY_R);
            win32_mapKey(keycodes, 0x01F, GLFW_KEY_S);
            win32_mapKey(keycodes, 0x014, GLFW_KEY_T);
            win32_mapKey(keycodes, 0x016, GLFW_KEY_U);
            win32_mapKey(keycodes, 0x02F, GLFW_KEY_V);
            win32_mapKey(keycodes, 0x011, GLFW_KEY_W);
            win32_mapKey(keycodes, 0x02D, GLFW_KEY_X);
            win32_mapKey(keycodes, 0x015, GLFW_KEY_Y);
            win32_mapKey(keycodes, 0x02C, GLFW_KEY_Z);

            win32_mapKey(keycodes, 0x028, GLFW_KEY_APOSTROPHE);
            win32_mapKey(keycodes, 0x02B, GLFW_KEY_BACKSLASH);
            win32_mapKey(keycodes, 0x033, GLFW_KEY_COMMA);
            win32_mapKey(keycodes, 0x00D, GLFW_KEY_EQUAL);
            win32_mapKey(keycodes, 0x029, GLFW_KEY_GRAVE_ACCENT);
            win32_mapKey(keycodes, 0x01A, GLFW_KEY_LEFT_BRACKET);
            win32_mapKey(keycodes, 0x00C, GLFW_KEY_MINUS);
            win32_mapKey(keycodes, 0x034, GLFW_KEY_PERIOD);
            win32_mapKey(keycodes, 0x01B, GLFW_KEY_RIGHT_BRACKET);
            win32_mapKey(keycodes, 0x027, GLFW_KEY_SEMICOLON);
            win32_mapKey(keycodes, 0x035, GLFW_KEY_SLASH);
            win32_mapKey(keycodes, 0x056, GLFW_KEY_WORLD_2);

            win32_mapKey(keycodes, 0x00E, GLFW_KEY_BACKSPACE);
            win32_mapKey(keycodes, 0x153, GLFW_KEY_DELETE);
            win32_mapKey(keycodes, 0x14F, GLFW_KEY_END);
            win32_mapKey(keycodes, 0x01C, GLFW_KEY_ENTER);
            win32_mapKey(keycodes, 0x001, GLFW_KEY_ESCAPE);
            win32_mapKey(keycodes, 0x147, GLFW_KEY_HOME);
            win32_mapKey(keycodes, 0x152, GLFW_KEY_INSERT);
            win32_mapKey(keycodes, 0x15D, GLFW_KEY_MENU);
            win32_mapKey(keycodes, 0x151, GLFW_KEY_PAGE_DOWN);
            win32_mapKey(keycodes, 0x149, GLFW_KEY_PAGE_UP);
            win32_mapKey(keycodes, 0x045, GLFW_KEY_PAUSE);
            win32_mapKey(keycodes, 0x039, GLFW_KEY_SPACE);
            win32_mapKey(keycodes, 0x00F, GLFW_KEY_TAB);
            win32_mapKey(keycodes, 0x03A, GLFW_KEY_CAPS_LOCK);
            win32_mapKey(keycodes, 0x145, GLFW_KEY_NUM_LOCK);
            win32_mapKey(keycodes, 0x046, GLFW_KEY_SCROLL_LOCK);
            win32_mapKey(keycodes, 0x03B, GLFW_KEY_F1);
            win32_mapKey(keycodes, 0x03C, GLFW_KEY_F2);
            win32_mapKey(keycodes, 0x03D, GLFW_KEY_F3);
            win32_mapKey(keycodes, 0x03E, GLFW_KEY_F4);
            win32_mapKey(keycodes, 0x03F, GLFW_KEY_F5);
            win32_mapKey(keycodes, 0x040, GLFW_KEY_F6);
            win32_mapKey(keycodes, 0x041, GLFW_KEY_F7);
            win32_mapKey(keycodes, 0x042, GLFW_KEY_F8);
            win32_mapKey(keycodes, 0x043, GLFW_KEY_F9);
            win32_mapKey(keycodes, 0x044, GLFW_KEY_F10);
            win32_mapKey(keycodes, 0x057, GLFW_KEY_F11);
            win32_mapKey(keycodes, 0x058, GLFW_KEY_F12);
            win32_mapKey(keycodes, 0x064, GLFW_KEY_F13);
            win32_mapKey(keycodes, 0x065, GLFW_KEY_F14);
            win32_mapKey(keycodes, 0x066, GLFW_KEY_F15);
            win32_mapKey(keycodes, 0x067, GLFW_KEY_F16);
            win32_mapKey(keycodes, 0x068, GLFW_KEY_F17);
            win32_mapKey(keycodes, 0x069, GLFW_KEY_F18);
            win32_mapKey(keycodes, 0x06A, GLFW_KEY_F19);
            win32_mapKey(keycodes, 0x06B, GLFW_KEY_F20);
            win32_mapKey(keycodes, 0x06C, GLFW_KEY_F21);
            win32_mapKey(keycodes, 0x06D, GLFW_KEY_F22);
            win32_mapKey(keycodes, 0x06E, GLFW_KEY_F23);
            win32_mapKey(keycodes, 0x076, GLFW_KEY_F24);
            win32_mapKey(keycodes, 0x038, GLFW_KEY_LEFT_ALT);
            win32_mapKey(keycodes, 0x01D, GLFW_KEY_LEFT_CONTROL);
            win32_mapKey(keycodes, 0x02A, GLFW_KEY_LEFT_SHIFT);
            win32_mapKey(keycodes, 0x15B, GLFW_KEY_LEFT_SUPER);
            win32_mapKey(keycodes, 0x137, GLFW_KEY_PRINT_SCREEN);
            win32_mapKey(keycodes, 0x138, GLFW_KEY_RIGHT_ALT);
            win32_mapKey(keycodes, 0x11D, GLFW_KEY_RIGHT_CONTROL);
            win32_mapKey(keycodes, 0x036, GLFW_KEY_RIGHT_SHIFT);
            win32_mapKey(keycodes, 0x15C, GLFW_KEY_RIGHT_SUPER);
            win32_mapKey(keycodes, 0x150, GLFW_KEY_DOWN);
            win32_mapKey(keycodes, 0x14B, GLFW_KEY_LEFT);
            win32_mapKey(keycodes, 0x14D, GLFW_KEY_RIGHT);
            win32_mapKey(keycodes, 0x148, GLFW_KEY_UP);

            win32_mapKey(keycodes, 0x052, GLFW_KEY_KP_0);
            win32_mapKey(keycodes, 0x04F, GLFW_KEY_KP_1);
            win32_mapKey(keycodes, 0x050, GLFW_KEY_KP_2);
            win32_mapKey(keycodes, 0x051, GLFW_KEY_KP_3);
            win32_mapKey(keycodes, 0x04B, GLFW_KEY_KP_4);
            win32_mapKey(keycodes, 0x04C, GLFW_KEY_KP_5);
            win32_mapKey(keycodes, 0x04D, GLFW_KEY_KP_6);
            win32_mapKey(keycodes, 0x047, GLFW_KEY_KP_7);
            win32_mapKey(keycodes, 0x048, GLFW_KEY_KP_8);
            win32_mapKey(keycodes, 0x049, GLFW_KEY_KP_9);
            win32_mapKey(keycodes, 0x04E, GLFW_KEY_KP_ADD);
            win32_mapKey(keycodes, 0x053, GLFW_KEY_KP_DECIMAL);
            win32_mapKey(keycodes, 0x135, GLFW_KEY_KP_DIVIDE);
            win32_mapKey(keycodes, 0x11C, GLFW_KEY_KP_ENTER);
            win32_mapKey(keycodes, 0x059, GLFW_KEY_KP_EQUAL);
            win32_mapKey(keycodes, 0x037, GLFW_KEY_KP_MULTIPLY);
            win32_mapKey(keycodes, 0x04A, GLFW_KEY_KP_SUBTRACT);

            for (var scancode = 0; scancode < 512; scancode++)
            {
                if (keycodes[scancode] > 0)
                    scancodes[keycodes[scancode]] = (short)scancode;
            }
        }
    }

    static void win32_mapKey(short* keycodes, int scancode, int key)
    {
        keycodes[scancode] = (short)key;
    }

    static void _glfwUpdateKeyNamesWin32()
    {
    }
}
