using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static uint win32_getWindowStyle(_GLFWwindow* window)
    {
        var style = WS_CLIPSIBLINGS | WS_CLIPCHILDREN;

        if (window->monitor != null)
            style |= WS_POPUP;
        else
        {
            style |= WS_SYSMENU | WS_MINIMIZEBOX;

            if (window->decorated != 0)
            {
                style |= WS_CAPTION;

                if (window->resizable != 0)
                    style |= WS_MAXIMIZEBOX | WS_THICKFRAME;
            }
            else
                style |= WS_POPUP;
        }

        return style;
    }

    static uint win32_getWindowExStyle(_GLFWwindow* window)
    {
        var style = WS_EX_APPWINDOW;

        if (window->monitor != null || window->floating != 0)
            style |= WS_EX_TOPMOST;

        return style;
    }

    static string win32_utf8String(byte* value)
    {
        return value != null ? Marshal.PtrToStringUTF8((IntPtr)value) ?? string.Empty : string.Empty;
    }

    static nint win32_toLong(uint value)
    {
        return (nint)unchecked((int)value);
    }

    static void win32_changeWindowMessageFilterEx(nint handle, uint message)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
            return;

        var symbol = win32_getUser32Symbol("ChangeWindowMessageFilterEx");
        if (symbol != 0)
        {
            ((delegate* unmanaged[Stdcall]<nint, uint, uint, void*, int>)symbol)(
                handle, message, MSGFLT_ALLOW, null);
        }
    }

    static void win32_enableNonClientDpiScaling(nint handle)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
            return;

        var symbol = win32_getUser32Symbol("EnableNonClientDpiScaling");
        if (symbol != 0)
            ((delegate* unmanaged[Stdcall]<nint, int>)symbol)(handle);
    }

    static int win32_getX(nint lParam)
    {
        return (short)(ushort)((long)lParam & 0xffff);
    }

    static int win32_getY(nint lParam)
    {
        return (short)(ushort)(((long)lParam >> 16) & 0xffff);
    }

    static int win32_getLowWord(nint value)
    {
        return (ushort)((long)value & 0xffff);
    }

    static int win32_getHighWord(nint value)
    {
        return (ushort)(((long)value >> 16) & 0xffff);
    }

    static int win32_getSignedHighWord(nuint value)
    {
        return (short)(ushort)(((ulong)value >> 16) & 0xffff);
    }

    static _GLFWwindow* win32_windowFromHandle(nint handle)
    {
        lock (win32_windowLock)
        {
            return win32_windows.TryGetValue(handle, out var window) ? (_GLFWwindow*)window : null;
        }
    }

    static void win32_trackWindow(_GLFWwindow* window)
    {
        lock (win32_windowLock)
            win32_windows[window->win32.handle] = (nint)window;
    }

    static void win32_untrackWindow(_GLFWwindow* window)
    {
        lock (win32_windowLock)
            win32_windows.Remove(window->win32.handle);
    }

    static void win32_updateCursorImage(_GLFWwindow* window)
    {
        if (window->cursorMode == GLFW_CURSOR_NORMAL ||
            window->cursorMode == GLFW_CURSOR_CAPTURED)
        {
            if (window->cursor != null && window->cursor->win32.handle != 0)
                SetCursor(window->cursor->win32.handle);
            else
                SetCursor(LoadCursorW(0, IDC_ARROW));
        }
        else
            SetCursor(_glfw.win32.blankCursor);
    }

    static void win32_captureCursor(_GLFWwindow* window)
    {
        if (GetClientRect(window->win32.handle, out var rect) == 0)
            return;

        var upperLeft = new POINT { x = rect.left, y = rect.top };
        var lowerRight = new POINT { x = rect.right, y = rect.bottom };

        if (ClientToScreen(window->win32.handle, ref upperLeft) == 0 ||
            ClientToScreen(window->win32.handle, ref lowerRight) == 0)
        {
            return;
        }

        rect.left = upperLeft.x;
        rect.top = upperLeft.y;
        rect.right = lowerRight.x;
        rect.bottom = lowerRight.y;

        ClipCursor(&rect);
        _glfw.win32.capturedCursorWindow = window;
    }

    static void win32_releaseCursor()
    {
        ClipCursor(null);
        _glfw.win32.capturedCursorWindow = null;
    }

    static void win32_disableCursor(_GLFWwindow* window)
    {
        _glfw.win32.disabledCursorWindow = window;
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            _glfwGetCursorPosWin32(window,
                &glfw->win32.restoreCursorPosX,
                &glfw->win32.restoreCursorPosY);
        }

        win32_updateCursorImage(window);
        _glfwCenterCursorInContentArea(window);
        win32_captureCursor(window);

        if (window->rawMouseMotion != 0)
            win32_enableRawMouseMotion(window);
    }

    static void win32_enableCursor(_GLFWwindow* window)
    {
        if (window->rawMouseMotion != 0)
            win32_disableRawMouseMotion();

        _glfw.win32.disabledCursorWindow = null;
        win32_releaseCursor();
        _glfwSetCursorPosWin32(window,
            _glfw.win32.restoreCursorPosX,
            _glfw.win32.restoreCursorPosY);
        win32_updateCursorImage(window);
    }

    static int win32_cursorInContentArea(_GLFWwindow* window)
    {
        if (GetCursorPos(out var pos) == 0)
            return GLFW_FALSE;
        if (ScreenToClient(window->win32.handle, ref pos) == 0)
            return GLFW_FALSE;
        if (GetClientRect(window->win32.handle, out var rect) == 0)
            return GLFW_FALSE;

        return pos.x >= rect.left && pos.x < rect.right &&
               pos.y >= rect.top && pos.y < rect.bottom
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static void win32_updateFramebufferTransparency(_GLFWwindow* window)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 0))
            return;

        if (DwmIsCompositionEnabled(out var composition) < 0 || composition == 0)
            return;

        uint color;
        int opaque;
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 2) ||
            (DwmGetColorizationColor(&color, &opaque) >= 0 && opaque == 0))
        {
            var region = CreateRectRgn(0, 0, -1, -1);
            if (region == 0)
                return;

            var bb = new DWM_BLURBEHIND
            {
                dwFlags = DWM_BB_ENABLE | DWM_BB_BLURREGION,
                fEnable = GLFW_TRUE,
                hRgnBlur = region
            };

            DwmEnableBlurBehindWindow(window->win32.handle, &bb);
            DeleteObject(region);
        }
        else
        {
            var bb = new DWM_BLURBEHIND
            {
                dwFlags = DWM_BB_ENABLE
            };

            DwmEnableBlurBehindWindow(window->win32.handle, &bb);
        }
    }

    static int win32_getKeyMods()
    {
        var mods = 0;

        if ((GetKeyState(VK_SHIFT) & 0x8000) != 0)
            mods |= GLFW_MOD_SHIFT;
        if ((GetKeyState(VK_CONTROL) & 0x8000) != 0)
            mods |= GLFW_MOD_CONTROL;
        if ((GetKeyState(VK_MENU) & 0x8000) != 0)
            mods |= GLFW_MOD_ALT;
        if (((GetKeyState(VK_LWIN) | GetKeyState(VK_RWIN)) & 0x8000) != 0)
            mods |= GLFW_MOD_SUPER;
        if ((GetKeyState(VK_CAPITAL) & 1) != 0)
            mods |= GLFW_MOD_CAPS_LOCK;
        if ((GetKeyState(VK_NUMLOCK) & 1) != 0)
            mods |= GLFW_MOD_NUM_LOCK;

        return mods;
    }

    static int win32_anyMouseButtonPressed(_GLFWwindow* window)
    {
        for (var i = 0; i <= GLFW_MOUSE_BUTTON_LAST; i++)
        {
            if (window->mouseButtons[i] == GLFW_PRESS)
                return GLFW_TRUE;
        }

        return GLFW_FALSE;
    }

    static void win32_inputMouseClick(_GLFWwindow* window, nint hWnd, int button, int action)
    {
        if (action == GLFW_PRESS && win32_anyMouseButtonPressed(window) == 0)
            SetCapture(hWnd);

        _glfwInputMouseClick(window, button, action, win32_getKeyMods());

        if (action == GLFW_RELEASE && win32_anyMouseButtonPressed(window) == 0)
            ReleaseCapture();
    }

    static int win32_adjustWindowRectExForDpi(_GLFWwindow* window, ref RECT rect, uint style, uint exStyle)
    {
        if (window->win32.handle != 0 && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 14393))
            return AdjustWindowRectExForDpi(ref rect, style, 0, exStyle, GetDpiForWindow(window->win32.handle));

        return AdjustWindowRectEx(ref rect, style, 0, exStyle);
    }

    static void win32_applyAspectRatio(_GLFWwindow* window, int edge, RECT* area)
    {
        var frame = new RECT();
        var ratio = window->numer / (float)window->denom;
        var style = win32_getWindowStyle(window);
        var exStyle = win32_getWindowExStyle(window);

        win32_adjustWindowRectExForDpi(window, ref frame, style, exStyle);

        if (edge == WMSZ_LEFT || edge == WMSZ_BOTTOMLEFT ||
            edge == WMSZ_RIGHT || edge == WMSZ_BOTTOMRIGHT)
        {
            area->bottom = area->top + frame.bottom - frame.top +
                (int)(((area->right - area->left) - (frame.right - frame.left)) / ratio);
        }
        else if (edge == WMSZ_TOPLEFT || edge == WMSZ_TOPRIGHT)
        {
            area->top = area->bottom - (frame.bottom - frame.top) -
                (int)(((area->right - area->left) - (frame.right - frame.left)) / ratio);
        }
        else if (edge == WMSZ_TOP || edge == WMSZ_BOTTOM)
        {
            area->right = area->left + frame.right - frame.left +
                (int)(((area->bottom - area->top) - (frame.bottom - frame.top)) * ratio);
        }
    }

    static int win32_translateKey(nuint wParam)
    {
        var vk = (int)wParam;

        if (vk >= 0x30 && vk <= 0x39)
            return GLFW_KEY_0 + vk - 0x30;
        if (vk >= 0x41 && vk <= 0x5a)
            return GLFW_KEY_A + vk - 0x41;
        if (vk >= 0x70 && vk <= 0x87)
            return GLFW_KEY_F1 + vk - 0x70;

        return vk switch
        {
            0x08 => GLFW_KEY_BACKSPACE,
            0x09 => GLFW_KEY_TAB,
            0x0d => GLFW_KEY_ENTER,
            0x1b => GLFW_KEY_ESCAPE,
            0x20 => GLFW_KEY_SPACE,
            0x21 => GLFW_KEY_PAGE_UP,
            0x22 => GLFW_KEY_PAGE_DOWN,
            0x23 => GLFW_KEY_END,
            0x24 => GLFW_KEY_HOME,
            0x25 => GLFW_KEY_LEFT,
            0x26 => GLFW_KEY_UP,
            0x27 => GLFW_KEY_RIGHT,
            0x28 => GLFW_KEY_DOWN,
            0x2d => GLFW_KEY_INSERT,
            0x2e => GLFW_KEY_DELETE,
            0xa0 => GLFW_KEY_LEFT_SHIFT,
            0xa1 => GLFW_KEY_RIGHT_SHIFT,
            0xa2 => GLFW_KEY_LEFT_CONTROL,
            0xa3 => GLFW_KEY_RIGHT_CONTROL,
            0xa4 => GLFW_KEY_LEFT_ALT,
            0xa5 => GLFW_KEY_RIGHT_ALT,
            0x5b => GLFW_KEY_LEFT_SUPER,
            0x5c => GLFW_KEY_RIGHT_SUPER,
            0x60 => GLFW_KEY_KP_0,
            0x61 => GLFW_KEY_KP_1,
            0x62 => GLFW_KEY_KP_2,
            0x63 => GLFW_KEY_KP_3,
            0x64 => GLFW_KEY_KP_4,
            0x65 => GLFW_KEY_KP_5,
            0x66 => GLFW_KEY_KP_6,
            0x67 => GLFW_KEY_KP_7,
            0x68 => GLFW_KEY_KP_8,
            0x69 => GLFW_KEY_KP_9,
            0x6e => GLFW_KEY_KP_DECIMAL,
            0x6f => GLFW_KEY_KP_DIVIDE,
            0x6a => GLFW_KEY_KP_MULTIPLY,
            0x6d => GLFW_KEY_KP_SUBTRACT,
            0x6b => GLFW_KEY_KP_ADD,
            0x14 => GLFW_KEY_CAPS_LOCK,
            0x91 => GLFW_KEY_SCROLL_LOCK,
            0x90 => GLFW_KEY_NUM_LOCK,
            0x2c => GLFW_KEY_PRINT_SCREEN,
            0x13 => GLFW_KEY_PAUSE,
            _ => GLFW_KEY_UNKNOWN
        };
    }

    static GLFWimage* win32_chooseImage(int count, GLFWimage* images, int width, int height)
    {
        var leastDiff = int.MaxValue;
        GLFWimage* closest = null;

        for (var i = 0; i < count; i++)
        {
            var current = images + i;
            var currentDiff = Math.Abs(current->width * current->height - width * height);
            if (currentDiff < leastDiff)
            {
                closest = current;
                leastDiff = currentDiff;
            }
        }

        return closest;
    }

    static nint win32_createIcon(GLFWimage* image, int xhot, int yhot, int icon)
    {
        var bi = new BITMAPV5HEADER
        {
            bV5Size = (uint)sizeof(BITMAPV5HEADER),
            bV5Width = image->width,
            bV5Height = -image->height,
            bV5Planes = 1,
            bV5BitCount = 32,
            bV5Compression = BI_BITFIELDS,
            bV5RedMask = 0x00ff0000,
            bV5GreenMask = 0x0000ff00,
            bV5BlueMask = 0x000000ff,
            bV5AlphaMask = 0xff000000
        };

        var dc = GetDC(0);
        void* target = null;
        var color = CreateDIBSection(dc, &bi, DIB_RGB_COLORS, &target, 0, 0);
        ReleaseDC(0, dc);

        if (color == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create RGBA bitmap");
            return 0;
        }

        var mask = CreateBitmap(image->width, image->height, 1, 1, null);
        if (mask == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create mask bitmap");
            DeleteObject(color);
            return 0;
        }

        var source = image->pixels;
        var destination = (byte*)target;

        for (var i = 0; i < image->width * image->height; i++)
        {
            destination[0] = source[2];
            destination[1] = source[1];
            destination[2] = source[0];
            destination[3] = source[3];
            destination += 4;
            source += 4;
        }

        var ii = new ICONINFO
        {
            fIcon = icon,
            xHotspot = (uint)xhot,
            yHotspot = (uint)yhot,
            hbmMask = mask,
            hbmColor = color
        };

        var handle = CreateIconIndirect(ref ii);

        DeleteObject(color);
        DeleteObject(mask);

        if (handle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                icon != 0 ? "Win32: Failed to create icon" : "Win32: Failed to create cursor");
        }

        return handle;
    }

    static nint win32_windowProc(nint hWnd, uint uMsg, nuint wParam, nint lParam)
    {
        var window = win32_windowFromHandle(hWnd);
        if (window == null)
        {
            if (uMsg == WM_NCCREATE)
            {
                var cs = (CREATESTRUCTW*)lParam;
                var wndconfig = (_GLFWwndconfig*)cs->lpCreateParams;
                if (wndconfig != null && wndconfig->scaleToMonitor != 0)
                    win32_enableNonClientDpiScaling(hWnd);
            }

            return DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        switch (uMsg)
        {
            case WM_MOUSEACTIVATE:
                if (win32_getHighWord(lParam) == WM_LBUTTONDOWN &&
                    win32_getLowWord(lParam) != HTCLIENT)
                {
                    window->win32.frameAction = GLFW_TRUE;
                }

                break;

            case WM_CAPTURECHANGED:
                if (lParam == 0 && window->win32.frameAction != 0)
                {
                    if (window->cursorMode == GLFW_CURSOR_DISABLED)
                        win32_disableCursor(window);
                    else if (window->cursorMode == GLFW_CURSOR_CAPTURED)
                        win32_captureCursor(window);

                    window->win32.frameAction = GLFW_FALSE;
                }

                break;

            case WM_CLOSE:
                _glfwInputWindowCloseRequest(window);
                return 0;

            case WM_SETFOCUS:
                _glfwInputWindowFocus(window, GLFW_TRUE);

                if (window->win32.frameAction != 0)
                    break;

                if (window->cursorMode == GLFW_CURSOR_DISABLED)
                    win32_disableCursor(window);
                else if (window->cursorMode == GLFW_CURSOR_CAPTURED)
                    win32_captureCursor(window);

                return 0;

            case WM_KILLFOCUS:
                if (window->cursorMode == GLFW_CURSOR_DISABLED)
                    win32_enableCursor(window);
                else if (window->cursorMode == GLFW_CURSOR_CAPTURED)
                    win32_releaseCursor();

                if (window->monitor != null && window->autoIconify != 0)
                    _glfwIconifyWindowWin32(window);

                _glfwInputWindowFocus(window, GLFW_FALSE);
                return 0;

            case WM_SYSCOMMAND:
                switch (wParam & 0xfff0)
                {
                    case SC_SCREENSAVE:
                    case SC_MONITORPOWER:
                        if (window->monitor != null)
                            return 0;
                        break;

                    case SC_KEYMENU:
                        if (window->win32.keymenu == 0)
                            return 0;
                        break;
                }

                break;

            case WM_ENTERSIZEMOVE:
            case WM_ENTERMENULOOP:
                if (window->win32.frameAction != 0)
                    break;

                if (window->cursorMode == GLFW_CURSOR_DISABLED)
                    win32_enableCursor(window);
                else if (window->cursorMode == GLFW_CURSOR_CAPTURED)
                    win32_releaseCursor();
                break;

            case WM_EXITSIZEMOVE:
            case WM_EXITMENULOOP:
                if (window->win32.frameAction != 0)
                    break;

                if (window->cursorMode == GLFW_CURSOR_DISABLED)
                    win32_disableCursor(window);
                else if (window->cursorMode == GLFW_CURSOR_CAPTURED)
                    win32_captureCursor(window);
                break;

            case WM_MOVE:
                if (_glfw.win32.capturedCursorWindow == window)
                    win32_captureCursor(window);

                _glfwInputWindowPos(window, win32_getX(lParam), win32_getY(lParam));
                return 0;

            case WM_SIZE:
            {
                var iconified = (int)wParam == SIZE_MINIMIZED ? GLFW_TRUE : GLFW_FALSE;
                var maximized = (int)wParam == SIZE_MAXIMIZED ? GLFW_TRUE : GLFW_FALSE;
                var width = win32_getLowWord(lParam);
                var height = win32_getHighWord(lParam);

                if (_glfw.win32.capturedCursorWindow == window)
                    win32_captureCursor(window);

                if (window->win32.iconified != iconified)
                {
                    window->win32.iconified = iconified;
                    _glfwInputWindowIconify(window, iconified);
                }

                if (window->win32.maximized != maximized)
                {
                    window->win32.maximized = maximized;
                    _glfwInputWindowMaximize(window, maximized);
                }

                if (window->win32.width != width || window->win32.height != height)
                {
                    window->win32.width = width;
                    window->win32.height = height;
                    _glfwInputFramebufferSize(window, width, height);
                    _glfwInputWindowSize(window, width, height);
                }

                return 0;
            }

            case WM_SIZING:
                if (window->numer == GLFW_DONT_CARE ||
                    window->denom == GLFW_DONT_CARE)
                {
                    break;
                }

                win32_applyAspectRatio(window, (int)wParam, (RECT*)lParam);
                return 1;

            case WM_GETMINMAXINFO:
            {
                if (window->monitor != null)
                    break;

                var frame = new RECT();
                win32_adjustWindowRectExForDpi(window, ref frame,
                    win32_getWindowStyle(window),
                    win32_getWindowExStyle(window));

                var mmi = (MINMAXINFO*)lParam;

                if (window->minwidth != GLFW_DONT_CARE &&
                    window->minheight != GLFW_DONT_CARE)
                {
                    mmi->ptMinTrackSize.x = window->minwidth + frame.right - frame.left;
                    mmi->ptMinTrackSize.y = window->minheight + frame.bottom - frame.top;
                }

                if (window->maxwidth != GLFW_DONT_CARE &&
                    window->maxheight != GLFW_DONT_CARE)
                {
                    mmi->ptMaxTrackSize.x = window->maxwidth + frame.right - frame.left;
                    mmi->ptMaxTrackSize.y = window->maxheight + frame.bottom - frame.top;
                }

                if (window->decorated == 0)
                {
                    var mi = win32_createMonitorInfo();
                    var mh = MonitorFromWindow(window->win32.handle, MONITOR_DEFAULTTONEAREST);

                    if (GetMonitorInfoW(mh, ref mi) != 0)
                    {
                        mmi->ptMaxPosition.x = mi.rcWork.left - mi.rcMonitor.left;
                        mmi->ptMaxPosition.y = mi.rcWork.top - mi.rcMonitor.top;
                        mmi->ptMaxSize.x = mi.rcWork.right - mi.rcWork.left;
                        mmi->ptMaxSize.y = mi.rcWork.bottom - mi.rcWork.top;
                    }
                }

                return 0;
            }

            case WM_PAINT:
                _glfwInputWindowDamage(window);
                break;

            case WM_ERASEBKGND:
                return 1;

            case WM_NCACTIVATE:
            case WM_NCPAINT:
                if (window->decorated == 0)
                    return 1;

                break;

            case WM_DWMCOMPOSITIONCHANGED:
            case WM_DWMCOLORIZATIONCOLORCHANGED:
                if (window->win32.transparent != 0)
                    win32_updateFramebufferTransparency(window);
                return 0;

            case WM_GETDPISCALEDSIZE:
            {
                if (window->win32.scaleToMonitor != 0)
                    break;

                if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 15063))
                {
                    var size = (SIZE*)lParam;
                    var source = new RECT();
                    var target = new RECT();
                    var style = win32_getWindowStyle(window);
                    var exStyle = win32_getWindowExStyle(window);

                    AdjustWindowRectExForDpi(ref source, style, 0, exStyle, GetDpiForWindow(window->win32.handle));
                    AdjustWindowRectExForDpi(ref target, style, 0, exStyle, (uint)win32_getLowWord((nint)wParam));

                    size->cx += (target.right - target.left) - (source.right - source.left);
                    size->cy += (target.bottom - target.top) - (source.bottom - source.top);
                    return 1;
                }

                break;
            }

            case WM_DPICHANGED:
            {
                var xscale = win32_getHighWord((nint)wParam) / (float)USER_DEFAULT_SCREEN_DPI;
                var yscale = win32_getLowWord((nint)wParam) / (float)USER_DEFAULT_SCREEN_DPI;

                if (window->monitor == null &&
                    (window->win32.scaleToMonitor != 0 ||
                     OperatingSystem.IsWindowsVersionAtLeast(10, 0, 15063)))
                {
                    var suggested = (RECT*)lParam;
                    SetWindowPos(window->win32.handle, HWND_TOP,
                        suggested->left,
                        suggested->top,
                        suggested->right - suggested->left,
                        suggested->bottom - suggested->top,
                        SWP_NOACTIVATE | SWP_NOZORDER);
                }

                _glfwInputWindowContentScale(window, xscale, yscale);
                break;
            }

            case WM_INPUTLANGCHANGE:
                _glfwUpdateKeyNamesWin32();
                break;

            case WM_SETCURSOR:
                if ((ushort)((long)lParam & 0xffff) == HTCLIENT)
                {
                    win32_updateCursorImage(window);
                    return 1;
                }

                break;

            case WM_DROPFILES:
            {
                var drop = (nint)wParam;
                var count = (int)DragQueryFileW(drop, uint.MaxValue, null, 0);
                var paths = (byte**)_glfw_calloc((nuint)count, (nuint)sizeof(byte*));

                if (paths == null && count > 0)
                {
                    DragFinish(drop);
                    return 0;
                }

                POINT pt;
                DragQueryPoint(drop, &pt);
                _glfwInputCursorPos(window, pt.x, pt.y);

                for (var i = 0; i < count; i++)
                {
                    var length = DragQueryFileW(drop, (uint)i, null, 0);
                    var buffer = (char*)_glfw_calloc((nuint)length + 1, (nuint)sizeof(char));
                    if (buffer == null)
                    {
                        for (var j = 0; j < i; j++)
                            _glfw_free(paths[j]);
                        _glfw_free(paths);
                        DragFinish(drop);
                        return 0;
                    }

                    DragQueryFileW(drop, (uint)i, buffer, length + 1);
                    paths[i] = win32_createUTF8String(new string(buffer));
                    _glfw_free(buffer);
                }

                _glfwInputDrop(window, count, paths);

                for (var i = 0; i < count; i++)
                    _glfw_free(paths[i]);
                _glfw_free(paths);

                DragFinish(drop);
                return 0;
            }

            case WM_MOUSEMOVE:
                if (window->win32.cursorTracked == 0)
                {
                    var tme = new TRACKMOUSEEVENT
                    {
                        cbSize = (uint)sizeof(TRACKMOUSEEVENT),
                        dwFlags = TME_LEAVE,
                        hwndTrack = window->win32.handle
                    };

                    TrackMouseEvent(ref tme);
                    window->win32.cursorTracked = GLFW_TRUE;
                    _glfwInputCursorEnter(window, GLFW_TRUE);
                }

                if (window->cursorMode == GLFW_CURSOR_DISABLED)
                {
                    var x = win32_getX(lParam);
                    var y = win32_getY(lParam);
                    var dx = x - window->win32.lastCursorPosX;
                    var dy = y - window->win32.lastCursorPosY;

                    if (_glfw.win32.disabledCursorWindow == window && window->rawMouseMotion == 0)
                        _glfwInputCursorPos(window, window->virtualCursorPosX + dx, window->virtualCursorPosY + dy);

                    window->win32.lastCursorPosX = x;
                    window->win32.lastCursorPosY = y;
                }
                else
                {
                    window->win32.lastCursorPosX = win32_getX(lParam);
                    window->win32.lastCursorPosY = win32_getY(lParam);
                    _glfwInputCursorPos(window, window->win32.lastCursorPosX, window->win32.lastCursorPosY);
                }

                return 0;

            case WM_INPUT:
            {
                if (_glfw.win32.disabledCursorWindow != window || window->rawMouseMotion == 0)
                    break;

                uint size = (uint)sizeof(RAWINPUT);
                RAWINPUT raw;
                if (GetRawInputData(lParam, RID_INPUT, &raw, &size, (uint)sizeof(RAWINPUTHEADER)) == uint.MaxValue)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to retrieve raw input data");
                    break;
                }

                if (raw.header.dwType == RIM_TYPEMOUSE)
                {
                    int dx;
                    int dy;

                    if ((raw.mouse.usFlags & MOUSE_MOVE_ABSOLUTE) != 0)
                    {
                        var pos = new POINT();
                        int width;
                        int height;

                        if ((raw.mouse.usFlags & MOUSE_VIRTUAL_DESKTOP) != 0)
                        {
                            pos.x += GetSystemMetrics(SM_XVIRTUALSCREEN);
                            pos.y += GetSystemMetrics(SM_YVIRTUALSCREEN);
                            width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
                            height = GetSystemMetrics(SM_CYVIRTUALSCREEN);
                        }
                        else
                        {
                            width = GetSystemMetrics(SM_CXSCREEN);
                            height = GetSystemMetrics(SM_CYSCREEN);
                        }

                        pos.x += (int)(raw.mouse.lLastX / 65535f * width);
                        pos.y += (int)(raw.mouse.lLastY / 65535f * height);
                        ScreenToClient(window->win32.handle, ref pos);

                        dx = pos.x - window->win32.lastCursorPosX;
                        dy = pos.y - window->win32.lastCursorPosY;
                    }
                    else
                    {
                        dx = raw.mouse.lLastX;
                        dy = raw.mouse.lLastY;
                    }

                    _glfwInputCursorPos(window,
                        window->virtualCursorPosX + dx,
                        window->virtualCursorPosY + dy);

                    window->win32.lastCursorPosX += dx;
                    window->win32.lastCursorPosY += dy;
                }

                return 0;
            }

            case WM_MOUSELEAVE:
                window->win32.cursorTracked = GLFW_FALSE;
                _glfwInputCursorEnter(window, GLFW_FALSE);
                return 0;

            case WM_LBUTTONDOWN:
                SetFocus(hWnd);
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_LEFT, GLFW_PRESS);
                return 0;

            case WM_LBUTTONUP:
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_LEFT, GLFW_RELEASE);
                return 0;

            case WM_RBUTTONDOWN:
                SetFocus(hWnd);
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_RIGHT, GLFW_PRESS);
                return 0;

            case WM_RBUTTONUP:
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_RIGHT, GLFW_RELEASE);
                return 0;

            case WM_MBUTTONDOWN:
                SetFocus(hWnd);
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_MIDDLE, GLFW_PRESS);
                return 0;

            case WM_MBUTTONUP:
                win32_inputMouseClick(window, hWnd, GLFW_MOUSE_BUTTON_MIDDLE, GLFW_RELEASE);
                return 0;

            case WM_XBUTTONDOWN:
                SetFocus(hWnd);
                win32_inputMouseClick(window,
                    hWnd,
                    win32_getSignedHighWord(wParam) == 1 ? GLFW_MOUSE_BUTTON_4 : GLFW_MOUSE_BUTTON_5,
                    GLFW_PRESS);
                return 1;

            case WM_XBUTTONUP:
                win32_inputMouseClick(window,
                    hWnd,
                    win32_getSignedHighWord(wParam) == 1 ? GLFW_MOUSE_BUTTON_4 : GLFW_MOUSE_BUTTON_5,
                    GLFW_RELEASE);
                return 1;

            case WM_MOUSEWHEEL:
                _glfwInputScroll(window, 0.0, win32_getSignedHighWord(wParam) / 120.0);
                return 0;

            case WM_MOUSEHWHEEL:
                _glfwInputScroll(window, -win32_getSignedHighWord(wParam) / 120.0, 0.0);
                return 0;

            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
            case WM_KEYUP:
            case WM_SYSKEYUP:
            {
                var hiword = (uint)(((long)lParam >> 16) & 0xffff);
                var action = (hiword & KF_UP) != 0 ? GLFW_RELEASE : GLFW_PRESS;
                var mods = win32_getKeyMods();
                var vk = (uint)wParam;
                var scancode = (int)(hiword & (KF_EXTENDED | 0xff));

                if (scancode == 0)
                    scancode = (int)MapVirtualKeyW(vk, MAPVK_VK_TO_VSC);

                if (scancode == 0x54)
                    scancode = 0x137;
                if (scancode == 0x146)
                    scancode = 0x45;
                if (scancode == 0x136)
                    scancode = 0x36;

                var key = scancode >= 0 && scancode < 512 ? _glfw.win32.keycodes[scancode] : GLFW_KEY_UNKNOWN;
                if (key < 0)
                    key = win32_translateKey(wParam);

                if (vk == VK_CONTROL)
                {
                    if ((hiword & KF_EXTENDED) != 0)
                    {
                        key = GLFW_KEY_RIGHT_CONTROL;
                    }
                    else
                    {
                        var time = GetMessageTime();

                        if (PeekMessageW(out var next, 0, 0, 0, PM_NOREMOVE) != 0)
                        {
                            if (next.message == WM_KEYDOWN ||
                                next.message == WM_SYSKEYDOWN ||
                                next.message == WM_KEYUP ||
                                next.message == WM_SYSKEYUP)
                            {
                                var nextHiword = (uint)(((long)next.lParam >> 16) & 0xffff);
                                if (next.wParam == VK_MENU &&
                                    (nextHiword & KF_EXTENDED) != 0 &&
                                    next.time == time)
                                {
                                    break;
                                }
                            }
                        }

                        key = GLFW_KEY_LEFT_CONTROL;
                    }
                }
                else if (vk == VK_PROCESSKEY)
                    break;

                if (action == GLFW_RELEASE && vk == VK_SHIFT)
                {
                    _glfwInputKey(window, GLFW_KEY_LEFT_SHIFT, scancode, action, mods);
                    _glfwInputKey(window, GLFW_KEY_RIGHT_SHIFT, scancode, action, mods);
                }
                else if (vk == VK_SNAPSHOT)
                {
                    _glfwInputKey(window, key, scancode, GLFW_PRESS, mods);
                    _glfwInputKey(window, key, scancode, GLFW_RELEASE, mods);
                }
                else if (key != GLFW_KEY_UNKNOWN)
                    _glfwInputKey(window, key, scancode, action, mods);

                break;
            }

            case WM_CHAR:
            case WM_SYSCHAR:
            {
                if (wParam >= 0xd800 && wParam <= 0xdbff)
                {
                    window->win32.highSurrogate = (char)wParam;
                }
                else
                {
                    var codepoint = 0u;

                    if (wParam >= 0xdc00 && wParam <= 0xdfff)
                    {
                        if (window->win32.highSurrogate != 0)
                        {
                            codepoint += (uint)(window->win32.highSurrogate - 0xd800) << 10;
                            codepoint += (uint)wParam - 0xdc00;
                            codepoint += 0x10000;
                        }
                    }
                    else
                        codepoint = (uint)wParam;

                    window->win32.highSurrogate = '\0';
                    if (codepoint != 0)
                        _glfwInputChar(window, codepoint, win32_getKeyMods(), uMsg != WM_SYSCHAR ? GLFW_TRUE : GLFW_FALSE);
                }

                if (uMsg == WM_SYSCHAR && window->win32.keymenu != 0)
                    break;

                return 0;
            }

            case WM_UNICHAR:
                if ((int)wParam == UNICODE_NOCHAR)
                    return 1;

                _glfwInputChar(window, (uint)wParam, win32_getKeyMods(), GLFW_TRUE);
                return 0;
        }

        return DefWindowProcW(hWnd, uMsg, wParam, lParam);
    }

    static int win32_createNativeWindow(_GLFWwindow* window, _GLFWwndconfig* wndconfig, _GLFWfbconfig* fbconfig)
    {
        var style = win32_getWindowStyle(window);
        var exStyle = win32_getWindowExStyle(window);
        int frameX;
        int frameY;
        int frameWidth;
        int frameHeight;

        if (window->monitor != null)
        {
            var mi = win32_createMonitorInfo();
            if (GetMonitorInfoW(window->monitor->win32.handle, ref mi) == 0)
            {
                _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to query monitor info");
                return GLFW_FALSE;
            }

            frameX = mi.rcMonitor.left;
            frameY = mi.rcMonitor.top;
            frameWidth = mi.rcMonitor.right - mi.rcMonitor.left;
            frameHeight = mi.rcMonitor.bottom - mi.rcMonitor.top;
        }
        else
        {
            var rect = new RECT { right = wndconfig->width, bottom = wndconfig->height };

            window->win32.maximized = wndconfig->maximized;
            if (wndconfig->maximized != 0)
                style |= WS_MAXIMIZE;

            AdjustWindowRectEx(ref rect, style, 0, exStyle);

            if (wndconfig->xpos == GLFW_ANY_POSITION && wndconfig->ypos == GLFW_ANY_POSITION)
            {
                frameX = CW_USEDEFAULT;
                frameY = CW_USEDEFAULT;
            }
            else
            {
                frameX = wndconfig->xpos + rect.left;
                frameY = wndconfig->ypos + rect.top;
            }

            frameWidth = rect.right - rect.left;
            frameHeight = rect.bottom - rect.top;
        }

        var title = win32_utf8String(wndconfig->title);

        window->win32.handle = CreateWindowExW(exStyle,
            _GLFW_WIN32_WINDOW_CLASS,
            title,
            style,
            frameX,
            frameY,
            frameWidth,
            frameHeight,
            0,
            0,
            _glfw.win32.instance,
            (nint)wndconfig);

        if (window->win32.handle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create window");
            return GLFW_FALSE;
        }

        window->win32.scaleToMonitor = wndconfig->scaleToMonitor;
        window->win32.keymenu = wndconfig->win32.keymenu;
        window->win32.showDefault = wndconfig->win32.showDefault;

        win32_trackWindow(window);
        win32_changeWindowMessageFilterEx(window->win32.handle, WM_DROPFILES);
        win32_changeWindowMessageFilterEx(window->win32.handle, WM_COPYDATA);
        win32_changeWindowMessageFilterEx(window->win32.handle, WM_COPYGLOBALDATA);
        DragAcceptFiles(window->win32.handle, GLFW_TRUE);
        if (fbconfig->transparent != 0)
        {
            win32_updateFramebufferTransparency(window);
            window->win32.transparent = GLFW_TRUE;
        }

        _glfwGetWindowSizeWin32(window, &window->win32.width, &window->win32.height);
        return GLFW_TRUE;
    }

    static int _glfwCreateWindowWin32(_GLFWwindow* window,
                                      _GLFWwndconfig* wndconfig,
                                      _GLFWctxconfig* ctxconfig,
                                      _GLFWfbconfig* fbconfig)
    {
        if (win32_createNativeWindow(window, wndconfig, fbconfig) == 0)
            return GLFW_FALSE;

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API)
            {
                if (_glfwInitWGL() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextWGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
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
            _glfwSetWindowMousePassthroughWin32(window, GLFW_TRUE);

        if (window->monitor != null)
        {
            _glfwShowWindowWin32(window);
            _glfwFocusWindowWin32(window);
            _glfwSetVideoModeWin32(window->monitor, &window->videoMode);
            _glfwInputMonitorWindow(window->monitor, window);

            if (wndconfig->centerCursor != 0)
                _glfwCenterCursorInContentArea(window);
        }
        else if (wndconfig->visible != 0)
        {
            _glfwShowWindowWin32(window);
            if (wndconfig->focused != 0)
                _glfwFocusWindowWin32(window);
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowWin32(_GLFWwindow* window)
    {
        if (window->monitor != null)
        {
            _glfwRestoreVideoModeWin32(window->monitor);
            _glfwInputMonitorWindow(window->monitor, null);
        }

        if (window->context.destroy != null)
            window->context.destroy(window);

        if (_glfw.win32.disabledCursorWindow == window)
            win32_enableCursor(window);

        if (_glfw.win32.capturedCursorWindow == window)
            win32_releaseCursor();

        if (window->win32.handle != 0)
        {
            win32_untrackWindow(window);
            DestroyWindow(window->win32.handle);
            window->win32.handle = 0;
        }

        if (window->win32.bigIcon != 0)
            DestroyIcon(window->win32.bigIcon);
        if (window->win32.smallIcon != 0)
            DestroyIcon(window->win32.smallIcon);
    }

    static void _glfwSetWindowTitleWin32(_GLFWwindow* window, byte* title)
    {
        SetWindowTextW(window->win32.handle, win32_utf8String(title));
    }

    static void _glfwSetWindowIconWin32(_GLFWwindow* window, int count, GLFWimage* images)
    {
        nint bigIcon;
        nint smallIcon;

        if (count != 0)
        {
            var bigImage = win32_chooseImage(count, images,
                GetSystemMetrics(SM_CXICON),
                GetSystemMetrics(SM_CYICON));
            var smallImage = win32_chooseImage(count, images,
                GetSystemMetrics(SM_CXSMICON),
                GetSystemMetrics(SM_CYSMICON));

            bigIcon = win32_createIcon(bigImage, 0, 0, GLFW_TRUE);
            smallIcon = win32_createIcon(smallImage, 0, 0, GLFW_TRUE);

            if (bigIcon == 0 || smallIcon == 0)
            {
                if (bigIcon != 0)
                    DestroyIcon(bigIcon);
                if (smallIcon != 0)
                    DestroyIcon(smallIcon);
                return;
            }
        }
        else
        {
            bigIcon = LoadIconW(0, IDI_APPLICATION);
            smallIcon = LoadIconW(0, IDI_APPLICATION);
        }

        SendMessageW(window->win32.handle, WM_SETICON, ICON_BIG, bigIcon);
        SendMessageW(window->win32.handle, WM_SETICON, ICON_SMALL, smallIcon);

        if (window->win32.bigIcon != 0)
            DestroyIcon(window->win32.bigIcon);
        if (window->win32.smallIcon != 0)
            DestroyIcon(window->win32.smallIcon);

        if (count != 0)
        {
            window->win32.bigIcon = bigIcon;
            window->win32.smallIcon = smallIcon;
        }
        else
        {
            window->win32.bigIcon = 0;
            window->win32.smallIcon = 0;
        }
    }

    static void _glfwGetWindowPosWin32(_GLFWwindow* window, int* xpos, int* ypos)
    {
        var pos = new POINT();
        ClientToScreen(window->win32.handle, ref pos);

        if (xpos != null)
            *xpos = pos.x;
        if (ypos != null)
            *ypos = pos.y;
    }

    static void _glfwSetWindowPosWin32(_GLFWwindow* window, int xpos, int ypos)
    {
        var rect = new RECT { left = xpos, top = ypos, right = xpos, bottom = ypos };
        AdjustWindowRectEx(ref rect, win32_getWindowStyle(window), 0, win32_getWindowExStyle(window));
        SetWindowPos(window->win32.handle, HWND_TOP, rect.left, rect.top, 0, 0,
            SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOSIZE);
    }

    static void _glfwGetWindowSizeWin32(_GLFWwindow* window, int* width, int* height)
    {
        GetClientRect(window->win32.handle, out var rect);

        if (width != null)
            *width = rect.right - rect.left;
        if (height != null)
            *height = rect.bottom - rect.top;
    }

    static void _glfwSetWindowSizeWin32(_GLFWwindow* window, int width, int height)
    {
        var rect = new RECT { right = width, bottom = height };
        AdjustWindowRectEx(ref rect, win32_getWindowStyle(window), 0, win32_getWindowExStyle(window));
        SetWindowPos(window->win32.handle, HWND_TOP, 0, 0,
            rect.right - rect.left, rect.bottom - rect.top,
            SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOMOVE | SWP_NOZORDER);
    }

    static void _glfwSetWindowSizeLimitsWin32(_GLFWwindow* window,
                                              int minwidth,
                                              int minheight,
                                              int maxwidth,
                                              int maxheight)
    {
        if ((minwidth == GLFW_DONT_CARE || minheight == GLFW_DONT_CARE) &&
            (maxwidth == GLFW_DONT_CARE || maxheight == GLFW_DONT_CARE))
        {
            return;
        }

        if (GetWindowRect(window->win32.handle, out var rect) != 0)
            MoveWindow(window->win32.handle, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, GLFW_TRUE);
    }

    static void _glfwSetWindowAspectRatioWin32(_GLFWwindow* window, int numer, int denom)
    {
        if (numer == GLFW_DONT_CARE || denom == GLFW_DONT_CARE)
            return;

        if (GetWindowRect(window->win32.handle, out var rect) == 0)
            return;

        win32_applyAspectRatio(window, WMSZ_BOTTOMRIGHT, &rect);
        MoveWindow(window->win32.handle, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, GLFW_TRUE);
    }

    static void _glfwGetFramebufferSizeWin32(_GLFWwindow* window, int* width, int* height)
    {
        _glfwGetWindowSizeWin32(window, width, height);
    }

    static void _glfwGetWindowFrameSizeWin32(_GLFWwindow* window, int* left, int* top, int* right, int* bottom)
    {
        var width = 0;
        var height = 0;
        _glfwGetWindowSizeWin32(window, &width, &height);

        var rect = new RECT { right = width, bottom = height };
        AdjustWindowRectEx(ref rect, win32_getWindowStyle(window), 0, win32_getWindowExStyle(window));

        if (left != null)
            *left = -rect.left;
        if (top != null)
            *top = -rect.top;
        if (right != null)
            *right = rect.right - width;
        if (bottom != null)
            *bottom = rect.bottom - height;
    }

    static void _glfwGetWindowContentScaleWin32(_GLFWwindow* window, float* xscale, float* yscale)
    {
        var handle = MonitorFromWindow(window->win32.handle, MONITOR_DEFAULTTONEAREST);
        _glfwGetHMONITORContentScaleWin32(handle, xscale, yscale);
    }

    static void _glfwIconifyWindowWin32(_GLFWwindow* window)
    {
        ShowWindow(window->win32.handle, SW_SHOWMINIMIZED);
    }

    static void _glfwRestoreWindowWin32(_GLFWwindow* window)
    {
        ShowWindow(window->win32.handle, SW_RESTORE);
    }

    static void _glfwMaximizeWindowWin32(_GLFWwindow* window)
    {
        ShowWindow(window->win32.handle, SW_SHOWMAXIMIZED);
    }

    static void _glfwShowWindowWin32(_GLFWwindow* window)
    {
        ShowWindow(window->win32.handle, window->win32.showDefault != 0 ? SW_SHOWDEFAULT : SW_SHOWNOACTIVATE);
        window->win32.showDefault = GLFW_FALSE;
        UpdateWindow(window->win32.handle);
    }

    static void _glfwHideWindowWin32(_GLFWwindow* window)
    {
        ShowWindow(window->win32.handle, SW_HIDE);
    }

    static void _glfwRequestWindowAttentionWin32(_GLFWwindow* window)
    {
        FlashWindow(window->win32.handle, GLFW_TRUE);
    }

    static void _glfwFocusWindowWin32(_GLFWwindow* window)
    {
        BringWindowToTop(window->win32.handle);
        SetForegroundWindow(window->win32.handle);
        SetFocus(window->win32.handle);
    }

    static void _glfwSetWindowMonitorWin32(_GLFWwindow* window,
                                           _GLFWmonitor* monitor,
                                           int xpos,
                                           int ypos,
                                           int width,
                                           int height,
                                           int refreshRate)
    {
        if (window->monitor == monitor)
        {
            if (monitor != null)
            {
            var current = win32_createMonitorInfo();
            if (GetMonitorInfoW(monitor->win32.handle, ref current) != 0)
            {
                _glfwSetVideoModeWin32(monitor, &window->videoMode);
                SetWindowPos(window->win32.handle, HWND_TOPMOST,
                        current.rcMonitor.left,
                        current.rcMonitor.top,
                        current.rcMonitor.right - current.rcMonitor.left,
                        current.rcMonitor.bottom - current.rcMonitor.top,
                        SWP_SHOWWINDOW | SWP_NOACTIVATE | SWP_NOCOPYBITS);
                }
            }
            else
            {
                _glfwSetWindowPosWin32(window, xpos, ypos);
                _glfwSetWindowSizeWin32(window, width, height);
            }

            return;
        }

        if (window->monitor != null)
        {
            _glfwRestoreVideoModeWin32(window->monitor);
            _glfwInputMonitorWindow(window->monitor, null);
        }

        _glfwInputWindowMonitor(window, monitor);

        if (window->monitor != null)
        {
            var mi = win32_createMonitorInfo();
            if (GetMonitorInfoW(window->monitor->win32.handle, ref mi) == 0)
                return;

            SetWindowLongPtrW(window->win32.handle, GWL_STYLE, win32_toLong(win32_getWindowStyle(window)));
            SetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE, win32_toLong(win32_getWindowExStyle(window)));
            _glfwSetVideoModeWin32(window->monitor, &window->videoMode);
            _glfwInputMonitorWindow(window->monitor, window);

            SetWindowPos(window->win32.handle, HWND_TOPMOST,
                mi.rcMonitor.left,
                mi.rcMonitor.top,
                mi.rcMonitor.right - mi.rcMonitor.left,
                mi.rcMonitor.bottom - mi.rcMonitor.top,
                SWP_SHOWWINDOW | SWP_FRAMECHANGED | SWP_NOACTIVATE | SWP_NOCOPYBITS);
        }
        else
        {
            var rect = new RECT { left = xpos, top = ypos, right = xpos + width, bottom = ypos + height };
            AdjustWindowRectEx(ref rect, win32_getWindowStyle(window), 0, win32_getWindowExStyle(window));

            SetWindowLongPtrW(window->win32.handle, GWL_STYLE, win32_toLong(win32_getWindowStyle(window)));
            SetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE, win32_toLong(win32_getWindowExStyle(window)));

            SetWindowPos(window->win32.handle, window->floating != 0 ? HWND_TOPMOST : HWND_NOTOPMOST,
                rect.left,
                rect.top,
                rect.right - rect.left,
                rect.bottom - rect.top,
                SWP_FRAMECHANGED | SWP_NOACTIVATE | SWP_NOCOPYBITS);
        }
    }

    static int _glfwWindowFocusedWin32(_GLFWwindow* window)
    {
        return GetActiveWindow() == window->win32.handle ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowIconifiedWin32(_GLFWwindow* window)
    {
        return IsIconic(window->win32.handle) != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowVisibleWin32(_GLFWwindow* window)
    {
        return IsWindowVisible(window->win32.handle) != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowMaximizedWin32(_GLFWwindow* window)
    {
        return IsZoomed(window->win32.handle) != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowHoveredWin32(_GLFWwindow* window)
    {
        return win32_cursorInContentArea(window);
    }

    static int _glfwFramebufferTransparentWin32(_GLFWwindow* window)
    {
        if (window->win32.transparent == 0)
            return GLFW_FALSE;

        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 0))
            return GLFW_FALSE;

        if (DwmIsCompositionEnabled(out var composition) < 0 || composition == 0)
            return GLFW_FALSE;

        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 2))
        {
            uint color;
            int opaque;
            if (DwmGetColorizationColor(&color, &opaque) < 0 || opaque != 0)
                return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static float _glfwGetWindowOpacityWin32(_GLFWwindow* window)
    {
        var exStyle = unchecked((uint)(long)GetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE));
        var flags = 0u;
        byte alpha = 255;

        if ((exStyle & WS_EX_LAYERED) != 0 &&
            GetLayeredWindowAttributes(window->win32.handle, null, &alpha, &flags) != 0 &&
            (flags & LWA_ALPHA) != 0)
        {
            return alpha / 255f;
        }

        return 1f;
    }

    static void win32_updateWindowStyles(_GLFWwindow* window)
    {
        var style = win32_getWindowStyle(window);
        if (IsWindowVisible(window->win32.handle) != 0)
            style |= WS_VISIBLE;
        if (IsZoomed(window->win32.handle) != 0)
            style |= WS_MAXIMIZE;

        SetWindowLongPtrW(window->win32.handle, GWL_STYLE, win32_toLong(style));
        SetWindowPos(window->win32.handle, HWND_TOP, 0, 0, 0, 0,
            SWP_FRAMECHANGED | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
    }

    static void _glfwSetWindowResizableWin32(_GLFWwindow* window, int enabled)
    {
        win32_updateWindowStyles(window);
    }

    static void _glfwSetWindowDecoratedWin32(_GLFWwindow* window, int enabled)
    {
        win32_updateWindowStyles(window);
    }

    static void _glfwSetWindowFloatingWin32(_GLFWwindow* window, int enabled)
    {
        SetWindowPos(window->win32.handle, enabled != 0 ? HWND_TOPMOST : HWND_NOTOPMOST,
            0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
    }

    static void _glfwSetWindowOpacityWin32(_GLFWwindow* window, float opacity)
    {
        var exStyle = unchecked((uint)(long)GetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE));

        if (opacity < 1f)
        {
            exStyle |= WS_EX_LAYERED;
            SetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE, win32_toLong(exStyle));
            SetLayeredWindowAttributes(window->win32.handle, 0, (byte)(255 * opacity), LWA_ALPHA);
        }
        else if (window->mousePassthrough == 0)
        {
            exStyle &= ~WS_EX_LAYERED;
            SetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE, win32_toLong(exStyle));
        }
    }

    static void _glfwSetWindowMousePassthroughWin32(_GLFWwindow* window, int enabled)
    {
        var exStyle = unchecked((uint)(long)GetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE));

        if (enabled != 0)
            exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
        else
            exStyle &= ~WS_EX_TRANSPARENT;

        SetWindowLongPtrW(window->win32.handle, GWL_EXSTYLE, win32_toLong(exStyle));
    }

    static void win32_releaseStuckKey(_GLFWwindow* window, int vk, int key)
    {
        if ((GetKeyState(vk) & 0x8000) != 0)
            return;
        if (window->keys[key] != GLFW_PRESS)
            return;

        _glfwInputKey(window, key, _glfw.win32.scancodes[key], GLFW_RELEASE, win32_getKeyMods());
    }

    static void _glfwPollEventsWin32()
    {
        while (PeekMessageW(out var msg, 0, 0, 0, PM_REMOVE) != 0)
        {
            if (msg.message == WM_QUIT)
            {
                for (var window = _glfw.windowListHead; window != null; window = window->next)
                    _glfwInputWindowCloseRequest(window);
            }
            else
            {
                TranslateMessage(ref msg);
                DispatchMessageW(ref msg);
            }
        }

        var handle = GetActiveWindow();
        if (handle != 0)
        {
            var window = win32_windowFromHandle(handle);
            if (window != null)
            {
                win32_releaseStuckKey(window, VK_LSHIFT, GLFW_KEY_LEFT_SHIFT);
                win32_releaseStuckKey(window, VK_RSHIFT, GLFW_KEY_RIGHT_SHIFT);
                win32_releaseStuckKey(window, VK_LWIN, GLFW_KEY_LEFT_SUPER);
                win32_releaseStuckKey(window, VK_RWIN, GLFW_KEY_RIGHT_SUPER);
            }
        }

        var disabledWindow = _glfw.win32.disabledCursorWindow;
        if (disabledWindow != null)
        {
            int width;
            int height;
            _glfwGetWindowSizeWin32(disabledWindow, &width, &height);

            if (disabledWindow->win32.lastCursorPosX != width / 2 ||
                disabledWindow->win32.lastCursorPosY != height / 2)
            {
                _glfwSetCursorPosWin32(disabledWindow, width / 2, height / 2);
            }
        }
    }

    static void _glfwWaitEventsWin32()
    {
        WaitMessage();
        _glfwPollEventsWin32();
    }

    static void _glfwWaitEventsTimeoutWin32(double timeout)
    {
        var milliseconds = timeout >= uint.MaxValue / 1000.0
            ? uint.MaxValue
            : (uint)Math.Ceiling(timeout * 1000.0);
        MsgWaitForMultipleObjects(0, 0, GLFW_FALSE, milliseconds, QS_ALLINPUT);
        _glfwPollEventsWin32();
    }

    static void _glfwPostEmptyEventWin32()
    {
        PostMessageW(_glfw.win32.helperWindowHandle, WM_NULL, 0, 0);
    }

    static void _glfwGetCursorPosWin32(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (GetCursorPos(out var pos) != 0)
        {
            ScreenToClient(window->win32.handle, ref pos);

            if (xpos != null)
                *xpos = pos.x;
            if (ypos != null)
                *ypos = pos.y;
        }
    }

    static void _glfwSetCursorPosWin32(_GLFWwindow* window, double xpos, double ypos)
    {
        var pos = new POINT { x = (int)xpos, y = (int)ypos };
        window->win32.lastCursorPosX = pos.x;
        window->win32.lastCursorPosY = pos.y;
        ClientToScreen(window->win32.handle, ref pos);
        SetCursorPos(pos.x, pos.y);
    }

    static void _glfwSetCursorModeWin32(_GLFWwindow* window, int mode)
    {
        if (_glfwWindowFocusedWin32(window) != 0)
        {
            if (mode == GLFW_CURSOR_DISABLED)
            {
                fixed (_GLFWlibrary* glfw = &_glfw)
                {
                    _glfwGetCursorPosWin32(window,
                        &glfw->win32.restoreCursorPosX,
                        &glfw->win32.restoreCursorPosY);
                }
                _glfwCenterCursorInContentArea(window);

                if (window->rawMouseMotion != 0)
                    win32_enableRawMouseMotion(window);
            }
            else if (_glfw.win32.disabledCursorWindow == window)
            {
                if (window->rawMouseMotion != 0)
                    win32_disableRawMouseMotion();
            }

            if (mode == GLFW_CURSOR_DISABLED || mode == GLFW_CURSOR_CAPTURED)
                win32_captureCursor(window);
            else
                win32_releaseCursor();

            if (mode == GLFW_CURSOR_DISABLED)
                _glfw.win32.disabledCursorWindow = window;
            else if (_glfw.win32.disabledCursorWindow == window)
            {
                _glfw.win32.disabledCursorWindow = null;
                _glfwSetCursorPosWin32(window,
                    _glfw.win32.restoreCursorPosX,
                    _glfw.win32.restoreCursorPosY);
            }
        }

        if (win32_cursorInContentArea(window) != 0)
            win32_updateCursorImage(window);
    }

    static void _glfwSetRawMouseMotionWin32(_GLFWwindow* window, int enabled)
    {
        if (_glfw.win32.disabledCursorWindow != window)
            return;

        if (enabled != 0)
            win32_enableRawMouseMotion(window);
        else
            win32_disableRawMouseMotion();
    }

    static int _glfwRawMouseMotionSupportedWin32()
    {
        return GLFW_TRUE;
    }

    static void win32_enableRawMouseMotion(_GLFWwindow* window)
    {
        var rid = new RAWINPUTDEVICE
        {
            usUsagePage = 0x01,
            usUsage = 0x02,
            hwndTarget = window->win32.handle
        };

        if (RegisterRawInputDevices(&rid, 1, (uint)sizeof(RAWINPUTDEVICE)) == 0)
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to register raw input device");
    }

    static void win32_disableRawMouseMotion()
    {
        var rid = new RAWINPUTDEVICE
        {
            usUsagePage = 0x01,
            usUsage = 0x02,
            dwFlags = RIDEV_REMOVE
        };

        if (RegisterRawInputDevices(&rid, 1, (uint)sizeof(RAWINPUTDEVICE)) == 0)
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to remove raw input device");
    }

    static int _glfwCreateCursorWin32(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        cursor->win32.handle = win32_createIcon(image, xhot, yhot, GLFW_FALSE);
        if (cursor->win32.handle == 0)
            return GLFW_FALSE;

        return GLFW_TRUE;
    }

    static int _glfwCreateStandardCursorWin32(_GLFWcursor* cursor, int shape)
    {
        var id = shape switch
        {
            GLFW_ARROW_CURSOR => IDC_ARROW,
            GLFW_IBEAM_CURSOR => IDC_IBEAM,
            GLFW_CROSSHAIR_CURSOR => IDC_CROSS,
            GLFW_POINTING_HAND_CURSOR => IDC_HAND,
            GLFW_RESIZE_EW_CURSOR => IDC_SIZEWE,
            GLFW_RESIZE_NS_CURSOR => IDC_SIZENS,
            GLFW_RESIZE_NWSE_CURSOR => IDC_SIZENWSE,
            GLFW_RESIZE_NESW_CURSOR => IDC_SIZENESW,
            GLFW_RESIZE_ALL_CURSOR => IDC_SIZEALL,
            GLFW_NOT_ALLOWED_CURSOR => IDC_NO,
            _ => 0
        };

        if (id == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Unknown standard cursor");
            return GLFW_FALSE;
        }

        cursor->win32.handle = LoadImageW(0, (nint)id, IMAGE_CURSOR, 0, 0, LR_DEFAULTSIZE | LR_SHARED);
        cursor->win32.standard = GLFW_TRUE;

        if (cursor->win32.handle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create standard cursor");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyCursorWin32(_GLFWcursor* cursor)
    {
        if (cursor->win32.handle != 0 && cursor->win32.standard == 0)
            DestroyCursor(cursor->win32.handle);
    }

    static void _glfwSetCursorWin32(_GLFWwindow* window, _GLFWcursor* cursor)
    {
        if (win32_cursorInContentArea(window) != 0)
            win32_updateCursorImage(window);
    }

    static byte* win32_createUTF8String(string value)
    {
        var byteCount = Encoding.UTF8.GetByteCount(value);
        var result = (byte*)_glfw_calloc((nuint)byteCount + 1, 1);
        if (result == null)
            return null;

        fixed (char* chars = value)
            Encoding.UTF8.GetBytes(chars, value.Length, result, byteCount);

        result[byteCount] = 0;
        return result;
    }

    static int win32_openClipboard()
    {
        for (var tries = 0; tries < 3; tries++)
        {
            if (OpenClipboard(_glfw.win32.helperWindowHandle) != 0)
                return GLFW_TRUE;

            Thread.Sleep(1);
        }

        _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to open clipboard");
        return GLFW_FALSE;
    }

    static void _glfwSetClipboardStringWin32(byte* value)
    {
        var text = win32_utf8String(value);
        var characterCount = text.Length + 1;
        var handle = GlobalAlloc(GMEM_MOVEABLE, (nuint)(characterCount * sizeof(char)));
        if (handle == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to allocate global handle for clipboard");
            return;
        }

        var buffer = (char*)GlobalLock(handle);
        if (buffer == null)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to lock global handle");
            GlobalFree(handle);
            return;
        }

        for (var i = 0; i < text.Length; i++)
            buffer[i] = text[i];
        buffer[text.Length] = '\0';

        GlobalUnlock(handle);

        if (win32_openClipboard() == 0)
        {
            GlobalFree(handle);
            return;
        }

        EmptyClipboard();
        if (SetClipboardData(CF_UNICODETEXT, handle) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to set clipboard data");
            CloseClipboard();
            GlobalFree(handle);
            return;
        }

        CloseClipboard();
    }

    static byte* _glfwGetClipboardStringWin32()
    {
        if (win32_openClipboard() == 0)
            return null;

        var handle = GetClipboardData(CF_UNICODETEXT);
        if (handle == 0)
        {
            _glfwInputErrorWin32(GLFW_FORMAT_UNAVAILABLE, "Win32: Failed to convert clipboard to string");
            CloseClipboard();
            return null;
        }

        var buffer = GlobalLock(handle);
        if (buffer == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to lock global handle");
            CloseClipboard();
            return null;
        }

        var text = Marshal.PtrToStringUni(buffer) ?? string.Empty;
        GlobalUnlock(handle);
        CloseClipboard();

        var result = win32_createUTF8String(text);
        if (result == null)
            return null;

        _glfw_free(_glfw.win32.clipboardString);
        _glfw.win32.clipboardString = result;
        return _glfw.win32.clipboardString;
    }

    static byte* _glfwGetScancodeNameWin32(int scancode)
    {
        if (scancode < 0 || scancode > 0x1ff)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid scancode {0}", scancode);
            return null;
        }

        var key = _glfw.win32.keycodes[scancode];
        if (key == GLFW_KEY_UNKNOWN)
            return null;

        if (key != GLFW_KEY_KP_EQUAL &&
            (key < GLFW_KEY_KP_0 || key > GLFW_KEY_KP_ADD) &&
            (key < GLFW_KEY_APOSTROPHE || key > GLFW_KEY_WORLD_2))
        {
            return null;
        }

        var chars = stackalloc char[32];
        var lParam = ((scancode & 0xff) << 16) |
                     (((scancode & 0x100) != 0) ? (1 << 24) : 0);
        var length = GetKeyNameTextW(lParam, chars, 32);
        if (length == 0)
            return null;

        fixed (byte* name = _glfw.win32.keyname)
        {
            var byteCount = Encoding.UTF8.GetBytes(chars, length, name, 63);
            name[byteCount] = 0;
            return name;
        }
    }

    static int _glfwGetKeyScancodeWin32(int key)
    {
        return _glfw.win32.scancodes[key];
    }

    static int _glfwGetEGLPlatformWin32(int** attribs)
    {
        if (_glfw.egl.ANGLE_platform_angle != 0)
        {
            var type = 0;

            if (_glfw.egl.ANGLE_platform_angle_opengl != 0)
            {
                if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_OPENGL)
                    type = EGL_PLATFORM_ANGLE_TYPE_OPENGL_ANGLE;
                else if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_OPENGLES)
                    type = EGL_PLATFORM_ANGLE_TYPE_OPENGLES_ANGLE;
            }

            if (_glfw.egl.ANGLE_platform_angle_d3d != 0)
            {
                if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_D3D9)
                    type = EGL_PLATFORM_ANGLE_TYPE_D3D9_ANGLE;
                else if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_D3D11)
                    type = EGL_PLATFORM_ANGLE_TYPE_D3D11_ANGLE;
            }

            if (_glfw.egl.ANGLE_platform_angle_vulkan != 0)
            {
                if (_glfw.hints.init.angleType == GLFW_ANGLE_PLATFORM_TYPE_VULKAN)
                    type = EGL_PLATFORM_ANGLE_TYPE_VULKAN_ANGLE;
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

    static void* _glfwGetEGLNativeDisplayWin32()
    {
        return (void*)GetDC(_glfw.win32.helperWindowHandle);
    }

    static void* _glfwGetEGLNativeWindowWin32(_GLFWwindow* window)
    {
        return (void*)window->win32.handle;
    }

    static void _glfwGetRequiredInstanceExtensionsWin32(byte** extensions)
    {
        if (_glfw.vk.KHR_surface == 0 || _glfw.vk.KHR_win32_surface == 0)
            return;

        extensions[0] = _glfwVkKHRSurfaceExtensionName;
        extensions[1] = _glfwVkKHRWin32SurfaceExtensionName;
    }

    static int _glfwGetPhysicalDevicePresentationSupportWin32(void* instance, void* device, uint queuefamily)
    {
        var vkGetPhysicalDeviceWin32PresentationSupportKHR =
            (delegate* unmanaged<void*, uint, int>)
            vulkan_getInstanceProcAddress(instance, "vkGetPhysicalDeviceWin32PresentationSupportKHR");
        if (vkGetPhysicalDeviceWin32PresentationSupportKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Win32: Vulkan instance missing VK_KHR_win32_surface extension");
            return GLFW_FALSE;
        }

        return vkGetPhysicalDeviceWin32PresentationSupportKHR(device, queuefamily);
    }

    static int _glfwCreateWindowSurfaceWin32(void* instance, _GLFWwindow* window, void* allocator, ulong* surface)
    {
        var vkCreateWin32SurfaceKHR =
            (delegate* unmanaged<void*, VkWin32SurfaceCreateInfoKHR*, void*, ulong*, int>)
            vulkan_getInstanceProcAddress(instance, "vkCreateWin32SurfaceKHR");
        if (vkCreateWin32SurfaceKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Win32: Vulkan instance missing VK_KHR_win32_surface extension");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        var sci = new VkWin32SurfaceCreateInfoKHR
        {
            sType = VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            hinstance = _glfw.win32.instance,
            hwnd = window->win32.handle
        };

        var err = vkCreateWin32SurfaceKHR(instance, &sci, allocator, surface);
        if (err != VK_SUCCESS)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Win32: Failed to create Vulkan surface: {0}",
                vulkan_resultString(err));
        }

        return err;
    }

    public static void* glfwGetWin32Window(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WIN32)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Win32: Platform not initialized");
            return null;
        }

        return (void*)((_GLFWwindow*)window)->win32.handle;
    }
}
