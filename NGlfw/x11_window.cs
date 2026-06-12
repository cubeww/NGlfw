using System.Threading;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int AllocNone = 0;
    const uint InputOutput = 1;
    const ulong CWBorderPixel = 1UL << 3;
    const ulong CWEventMask = 1UL << 11;
    const ulong CWColormap = 1UL << 13;
    const long KeyPressMask = 1L << 0;
    const long KeyReleaseMask = 1L << 1;
    const long ButtonPressMask = 1L << 2;
    const long ButtonReleaseMask = 1L << 3;
    const long EnterWindowMask = 1L << 4;
    const long LeaveWindowMask = 1L << 5;
    const long PointerMotionMask = 1L << 6;
    const long ExposureMask = 1L << 15;
    const long VisibilityChangeMask = 1L << 16;
    const long StructureNotifyMask = 1L << 17;
    const long SubstructureNotifyMask = 1L << 19;
    const long SubstructureRedirectMask = 1L << 20;
    const long FocusChangeMask = 1L << 21;
    const long PropertyChangeMask = 1L << 22;
    const int PropModeReplace = 0;
    const int Success = 0;
    const int XA_ATOM = 4;
    const int XA_CARDINAL = 6;
    const int XA_STRING = 31;
    const int PMinSize = 1 << 4;
    const int PMaxSize = 1 << 5;
    const int PAspect = 1 << 7;
    const int _NET_WM_STATE_REMOVE = 0;
    const int _NET_WM_STATE_ADD = 1;
    const int MWM_HINTS_DECORATIONS = 2;
    const int MWM_DECOR_ALL = 1;
    const int ShapeSet = 0;
    const int ShapeInput = 2;
    const int XIAllMasterDevices = 1;
    const int XI_RawMotion = 17;
    const uint XkbUseCoreKbd = 0x0100;
    const int XkbEventCode = 0;
    const uint XkbStateNotify = 2;
    const ulong XkbGroupStateMask = 1UL << 4;
    const int RevertToParent = 2;
    const ulong CurrentTime = 0;
    const long NoEventMask = 0;
    const nuint _GLFW_XDND_VERSION = 5;
    const int QueuedAfterReading = 1;
    const short POLLIN = 0x0001;

    const int KeyPress = 2;
    const int KeyRelease = 3;
    const int ButtonPress = 4;
    const int ButtonRelease = 5;
    const int MotionNotify = 6;
    const int EnterNotify = 7;
    const int LeaveNotify = 8;
    const int FocusIn = 9;
    const int FocusOut = 10;
    const int Expose = 12;
    const int DestroyNotify = 17;
    const int UnmapNotify = 18;
    const int MapNotify = 19;
    const int ReparentNotify = 21;
    const int ConfigureNotify = 22;
    const int PropertyNotify = 28;
    const int SelectionRequest = 30;
    const int SelectionNotify = 31;
    const int ClientMessage = 33;
    const int GenericEvent = 35;

    const int NormalState = 1;
    const int IconicState = 3;
    const int PropertyNewValue = 0;
    const int NotifyGrab = 1;
    const int NotifyUngrab = 2;

    const int ShiftMask = 1 << 0;
    const int LockMask = 1 << 1;
    const int ControlMask = 1 << 2;
    const int Mod1Mask = 1 << 3;
    const int Mod2Mask = 1 << 4;
    const int Mod4Mask = 1 << 6;

    const uint XC_arrow = 2;
    const uint XC_crosshair = 34;
    const uint XC_fleur = 52;
    const uint XC_hand2 = 60;
    const uint XC_sb_h_double_arrow = 108;
    const uint XC_sb_v_double_arrow = 116;
    const uint XC_top_left_corner = 134;
    const uint XC_top_right_corner = 136;
    const uint XC_xterm = 152;
    const uint XC_X_cursor = 0;

    static _GLFWwindow* x11_findWindow(nuint handle)
    {
        for (var window = _glfw.windowListHead; window != null; window = window->next)
        {
            if (window->x11.handle == handle)
                return window;
        }

        return null;
    }

    static _GLFWwindow* x11_findWindowForEvent(XEvent* @event)
    {
        var window = x11_findWindow(@event->anyWindow);
        if (window == null && @event->type == ConfigureNotify)
            window = x11_findWindow(@event->configureWindow);
        if (window == null && @event->type == ReparentNotify)
            window = x11_findWindow(@event->reparentWindow);
        return window;
    }

    static int x11_translateState(uint state)
    {
        var mods = 0;

        if ((state & ShiftMask) != 0)
            mods |= GLFW_MOD_SHIFT;
        if ((state & ControlMask) != 0)
            mods |= GLFW_MOD_CONTROL;
        if ((state & Mod1Mask) != 0)
            mods |= GLFW_MOD_ALT;
        if ((state & Mod4Mask) != 0)
            mods |= GLFW_MOD_SUPER;
        if ((state & LockMask) != 0)
            mods |= GLFW_MOD_CAPS_LOCK;
        if ((state & Mod2Mask) != 0)
            mods |= GLFW_MOD_NUM_LOCK;

        return mods;
    }

    static int x11_getWindowState(_GLFWwindow* window)
    {
        byte* data;
        var result = 0;
        var count = x11_getWindowProperty(window->x11.handle,
            _glfw.x11.WM_STATE,
            _glfw.x11.WM_STATE,
            GLFW_FALSE,
            &data);

        if (count >= 2 && data != null)
            result = (int)*(uint*)data;

        if (data != null && _glfw.x11.XFree != null)
            _glfw.x11.XFree(data);

        return result;
    }

    static int x11_queryWindowMaximized(_GLFWwindow* window)
    {
        if (_glfw.x11.NET_WM_STATE == 0 ||
            _glfw.x11.NET_WM_STATE_MAXIMIZED_VERT == 0 ||
            _glfw.x11.NET_WM_STATE_MAXIMIZED_HORZ == 0)
        {
            return GLFW_FALSE;
        }

        byte* data;
        var count = x11_getWindowProperty(window->x11.handle,
            _glfw.x11.NET_WM_STATE,
            XA_ATOM,
            GLFW_FALSE,
            &data);

        var maximized = GLFW_FALSE;
        var states = (nuint*)data;
        for (nuint i = 0; i < count; i++)
        {
            if (states[i] == _glfw.x11.NET_WM_STATE_MAXIMIZED_VERT ||
                states[i] == _glfw.x11.NET_WM_STATE_MAXIMIZED_HORZ)
            {
                maximized = GLFW_TRUE;
                break;
            }
        }

        if (data != null && _glfw.x11.XFree != null)
            _glfw.x11.XFree(data);

        return maximized;
    }

    static int x11_translateKeySym(nuint keysym)
    {
        if (keysym >= '0' && keysym <= '9')
            return GLFW_KEY_0 + (int)(keysym - '0');
        if (keysym >= 'A' && keysym <= 'Z')
            return GLFW_KEY_A + (int)(keysym - 'A');
        if (keysym >= 'a' && keysym <= 'z')
            return GLFW_KEY_A + (int)(keysym - 'a');
        if (keysym >= 0xffbe && keysym <= 0xffd5)
            return GLFW_KEY_F1 + (int)(keysym - 0xffbe);
        if (keysym >= 0xffb0 && keysym <= 0xffb9)
            return GLFW_KEY_KP_0 + (int)(keysym - 0xffb0);

        return keysym switch
        {
            0x020 => GLFW_KEY_SPACE,
            0x027 => GLFW_KEY_APOSTROPHE,
            0x02c => GLFW_KEY_COMMA,
            0x02d => GLFW_KEY_MINUS,
            0x02e => GLFW_KEY_PERIOD,
            0x02f => GLFW_KEY_SLASH,
            0x03b => GLFW_KEY_SEMICOLON,
            0x03d => GLFW_KEY_EQUAL,
            0x05b => GLFW_KEY_LEFT_BRACKET,
            0x05c => GLFW_KEY_BACKSLASH,
            0x05d => GLFW_KEY_RIGHT_BRACKET,
            0x060 => GLFW_KEY_GRAVE_ACCENT,
            0xff08 => GLFW_KEY_BACKSPACE,
            0xff09 => GLFW_KEY_TAB,
            0xff0d => GLFW_KEY_ENTER,
            0xff1b => GLFW_KEY_ESCAPE,
            0xffff => GLFW_KEY_DELETE,
            0xff50 => GLFW_KEY_HOME,
            0xff51 => GLFW_KEY_LEFT,
            0xff52 => GLFW_KEY_UP,
            0xff53 => GLFW_KEY_RIGHT,
            0xff54 => GLFW_KEY_DOWN,
            0xff55 => GLFW_KEY_PAGE_UP,
            0xff56 => GLFW_KEY_PAGE_DOWN,
            0xff57 => GLFW_KEY_END,
            0xff63 => GLFW_KEY_INSERT,
            0xff67 => GLFW_KEY_MENU,
            0xff61 => GLFW_KEY_PRINT_SCREEN,
            0xff13 => GLFW_KEY_PAUSE,
            0xff14 => GLFW_KEY_SCROLL_LOCK,
            0xff7f => GLFW_KEY_NUM_LOCK,
            0xffe1 => GLFW_KEY_LEFT_SHIFT,
            0xffe2 => GLFW_KEY_RIGHT_SHIFT,
            0xffe3 => GLFW_KEY_LEFT_CONTROL,
            0xffe4 => GLFW_KEY_RIGHT_CONTROL,
            0xffe5 => GLFW_KEY_CAPS_LOCK,
            0xffe9 => GLFW_KEY_LEFT_ALT,
            0xffea => GLFW_KEY_RIGHT_ALT,
            0xffeb => GLFW_KEY_LEFT_SUPER,
            0xffec => GLFW_KEY_RIGHT_SUPER,
            0xff8d => GLFW_KEY_KP_ENTER,
            0xffaa => GLFW_KEY_KP_MULTIPLY,
            0xffab => GLFW_KEY_KP_ADD,
            0xffad => GLFW_KEY_KP_SUBTRACT,
            0xffae => GLFW_KEY_KP_DECIMAL,
            0xffaf => GLFW_KEY_KP_DIVIDE,
            0xffbd => GLFW_KEY_KP_EQUAL,
            _ => GLFW_KEY_UNKNOWN
        };
    }

    static void x11_sendEventToWM(_GLFWwindow* window, nuint type, nint a, nint b, nint c, nint d, nint e)
    {
        if (window->x11.handle == 0 || type == 0 || _glfw.x11.XSendEvent == null)
            return;

        XEvent @event = default;
        @event.type = ClientMessage;
        @event.display = _glfw.x11.display;
        @event.anyWindow = window->x11.handle;
        @event.clientMessageType = type;
        @event.clientFormat = 32;
        @event.clientData0 = a;
        @event.clientData1 = b;
        @event.clientData2 = c;
        @event.clientData3 = d;
        @event.clientData4 = e;

        _glfw.x11.XSendEvent(_glfw.x11.display,
            _glfw.x11.root,
            GLFW_FALSE,
            (nint)(SubstructureNotifyMask | SubstructureRedirectMask),
            &@event);
    }

    static nuint x11_getWindowProperty(nuint window, nuint property, nuint type, int delete, byte** value)
    {
        if (value != null)
            *value = null;

        if (_glfw.x11.XGetWindowProperty == null)
            return 0;

        nuint actualType;
        int actualFormat;
        nuint itemCount;
        nuint bytesAfter;
        byte* data;

        var result = _glfw.x11.XGetWindowProperty(_glfw.x11.display,
            window,
            property,
            0,
            1024 * 1024,
            delete,
            type,
            &actualType,
            &actualFormat,
            &itemCount,
            &bytesAfter,
            &data);

        if (result != Success || data == null || actualType != type || bytesAfter != 0)
        {
            if (data != null && _glfw.x11.XFree != null)
                _glfw.x11.XFree(data);
            return 0;
        }

        if (value != null)
            *value = data;
        else if (_glfw.x11.XFree != null)
            _glfw.x11.XFree(data);

        return itemCount;
    }

    static int x11_writeSelectionString(nuint requestor, nuint property, nuint target, byte* selectionString)
    {
        if (_glfw.x11.XChangeProperty == null || property == 0 || selectionString == null)
            return GLFW_FALSE;

        if (target == _glfw.x11.TEXT)
            target = _glfw.x11.UTF8_STRING;

        if (target != _glfw.x11.UTF8_STRING && target != XA_STRING)
            return GLFW_FALSE;

        _glfw.x11.XChangeProperty(_glfw.x11.display,
            requestor,
            property,
            target,
            8,
            PropModeReplace,
            selectionString,
            _glfw_strlen(selectionString));

        return GLFW_TRUE;
    }

    static nuint x11_writeTargetToProperty(XEvent* @event)
    {
        if (_glfw.x11.XChangeProperty == null)
            return 0;

        var requestProperty = @event->selectionRequestProperty != 0
            ? @event->selectionRequestProperty
            : @event->selectionRequestTarget;
        var selectionString = @event->selectionRequestSelection == _glfw.x11.CLIPBOARD
            ? _glfw.x11.clipboardString
            : _glfw.x11.primarySelectionString;
        if (selectionString == null)
            return 0;

        if (@event->selectionRequestTarget == _glfw.x11.TARGETS)
        {
            var targets = stackalloc nuint[7];
            targets[0] = _glfw.x11.TARGETS;
            targets[1] = _glfw.x11.MULTIPLE;
            targets[2] = _glfw.x11.SAVE_TARGETS;
            targets[3] = _glfw.x11.UTF8_STRING;
            targets[4] = XA_STRING;
            targets[5] = _glfw.x11.TEXT;
            targets[6] = _glfw.x11.NULL_;

            _glfw.x11.XChangeProperty(_glfw.x11.display,
                @event->selectionRequestor,
                requestProperty,
                XA_ATOM,
                32,
                PropModeReplace,
                (byte*)targets,
                7);

            return requestProperty;
        }

        if (@event->selectionRequestTarget == _glfw.x11.MULTIPLE &&
            _glfw.x11.ATOM_PAIR != 0)
        {
            byte* data;
            var count = x11_getWindowProperty(@event->selectionRequestor,
                requestProperty,
                _glfw.x11.ATOM_PAIR,
                GLFW_FALSE,
                &data);
            if (count == 0 || data == null)
                return 0;

            var multipleTargets = stackalloc nuint[7];
            multipleTargets[0] = _glfw.x11.TARGETS;
            multipleTargets[1] = _glfw.x11.MULTIPLE;
            multipleTargets[2] = _glfw.x11.SAVE_TARGETS;
            multipleTargets[3] = _glfw.x11.UTF8_STRING;
            multipleTargets[4] = XA_STRING;
            multipleTargets[5] = _glfw.x11.TEXT;
            multipleTargets[6] = _glfw.x11.NULL_;

            var pairs = (nuint*)data;
            for (nuint i = 0; i + 1 < count; i += 2)
            {
                var target = pairs[i];
                var property = pairs[i + 1];

                if (target == _glfw.x11.SAVE_TARGETS)
                {
                    _glfw.x11.XChangeProperty(_glfw.x11.display,
                        @event->selectionRequestor,
                        property,
                        _glfw.x11.NULL_,
                        32,
                        PropModeReplace,
                        null,
                        0);
                    continue;
                }

                if (target == _glfw.x11.TARGETS)
                {
                    _glfw.x11.XChangeProperty(_glfw.x11.display,
                        @event->selectionRequestor,
                        property,
                        XA_ATOM,
                        32,
                        PropModeReplace,
                        (byte*)multipleTargets,
                        7);
                    continue;
                }

                if (x11_writeSelectionString(@event->selectionRequestor, property, target, selectionString) == 0)
                    pairs[i + 1] = 0;
            }

            _glfw.x11.XChangeProperty(_glfw.x11.display,
                @event->selectionRequestor,
                requestProperty,
                _glfw.x11.ATOM_PAIR,
                32,
                PropModeReplace,
                data,
                (int)count);

            if (_glfw.x11.XFree != null)
                _glfw.x11.XFree(data);

            return requestProperty;
        }

        if (@event->selectionRequestTarget == _glfw.x11.SAVE_TARGETS)
        {
            _glfw.x11.XChangeProperty(_glfw.x11.display,
                @event->selectionRequestor,
                requestProperty,
                _glfw.x11.NULL_,
                32,
                PropModeReplace,
                null,
                0);

            return requestProperty;
        }

        if (@event->selectionRequestTarget == _glfw.x11.UTF8_STRING ||
            @event->selectionRequestTarget == XA_STRING ||
            @event->selectionRequestTarget == _glfw.x11.TEXT)
        {
            return x11_writeSelectionString(@event->selectionRequestor,
                requestProperty,
                @event->selectionRequestTarget,
                selectionString) != 0
                    ? requestProperty
                    : 0;
        }

        return 0;
    }

    static void x11_handleSelectionRequest(XEvent* @event)
    {
        if (_glfw.x11.XSendEvent == null)
            return;

        XEvent reply = default;
        reply.type = SelectionNotify;
        reply.display = _glfw.x11.display;
        reply.selectionNotifyRequestor = @event->selectionRequestor;
        reply.selectionNotifySelection = @event->selectionRequestSelection;
        reply.selectionNotifyTarget = @event->selectionRequestTarget;
        reply.selectionNotifyProperty = x11_writeTargetToProperty(@event);
        reply.selectionNotifyTime = @event->selectionRequestTime;

        _glfw.x11.XSendEvent(_glfw.x11.display,
            @event->selectionRequestor,
            GLFW_FALSE,
            0,
            &reply);
        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void _glfwPushSelectionToManagerX11()
    {
        if (_glfw.x11.helperWindowHandle == 0 ||
            _glfw.x11.CLIPBOARD_MANAGER == 0 ||
            _glfw.x11.SAVE_TARGETS == 0 ||
            _glfw.x11.XConvertSelection == null ||
            _glfw.x11.XPending == null ||
            _glfw.x11.XNextEvent == null)
        {
            return;
        }

        _glfw.x11.XConvertSelection(_glfw.x11.display,
            _glfw.x11.CLIPBOARD_MANAGER,
            _glfw.x11.SAVE_TARGETS,
            0,
            _glfw.x11.helperWindowHandle,
            CurrentTime);
        _glfw.x11.XFlush(_glfw.x11.display);

        var deadline = _glfwPlatformGetTimerValue() + 5UL * _glfwPlatformGetTimerFrequency();
        while (_glfwPlatformGetTimerValue() < deadline)
        {
            if (_glfw.x11.XPending(_glfw.x11.display) == 0)
            {
                Thread.Sleep(1);
                continue;
            }

            XEvent @event;
            _glfw.x11.XNextEvent(_glfw.x11.display, &@event);

            if (@event.type == SelectionRequest)
            {
                x11_handleSelectionRequest(&@event);
            }
            else if (@event.type == SelectionNotify &&
                     @event.selectionNotifyRequestor == _glfw.x11.helperWindowHandle &&
                     @event.selectionNotifySelection == _glfw.x11.CLIPBOARD_MANAGER &&
                     @event.selectionNotifyTarget == _glfw.x11.SAVE_TARGETS)
            {
                return;
            }
            else
            {
                x11_processEvent(&@event);
            }
        }
    }

    static void x11_sendXdndFinished(_GLFWwindow* window, int accepted)
    {
        if (_glfw.x11.xdndVersion < 2 ||
            _glfw.x11.xdndSource == 0 ||
            _glfw.x11.XSendEvent == null)
        {
            return;
        }

        XEvent reply = default;
        reply.type = ClientMessage;
        reply.anyWindow = _glfw.x11.xdndSource;
        reply.clientMessageType = _glfw.x11.XdndFinished;
        reply.clientFormat = 32;
        reply.clientData0 = (nint)window->x11.handle;
        reply.clientData1 = accepted;
        reply.clientData2 = accepted != 0 ? (nint)_glfw.x11.XdndActionCopy : 0;

        _glfw.x11.XSendEvent(_glfw.x11.display,
            _glfw.x11.xdndSource,
            GLFW_FALSE,
            (nint)NoEventMask,
            &reply);
        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void x11_processXdndClientMessage(_GLFWwindow* window, XEvent* @event)
    {
        if (@event->clientMessageType == _glfw.x11.XdndEnter)
        {
            _glfw.x11.xdndSource = (nuint)@event->clientData0;
            _glfw.x11.xdndVersion = (int)((ulong)@event->clientData1 >> 24);
            _glfw.x11.xdndFormat = 0;

            if (_glfw.x11.xdndVersion > (int)_GLFW_XDND_VERSION)
                return;

            var list = ((ulong)@event->clientData1 & 1UL) != 0;
            byte* data = null;

            if (list)
            {
                var count = x11_getWindowProperty(_glfw.x11.xdndSource,
                    _glfw.x11.XdndTypeList,
                    XA_ATOM,
                    GLFW_FALSE,
                    &data);
                var formats = (nuint*)data;

                for (nuint i = 0; i < count; i++)
                {
                    if (formats[i] == _glfw.x11.text_uri_list)
                    {
                        _glfw.x11.xdndFormat = _glfw.x11.text_uri_list;
                        break;
                    }
                }
            }
            else if ((nuint)@event->clientData2 == _glfw.x11.text_uri_list ||
                     (nuint)@event->clientData3 == _glfw.x11.text_uri_list ||
                     (nuint)@event->clientData4 == _glfw.x11.text_uri_list)
            {
                _glfw.x11.xdndFormat = _glfw.x11.text_uri_list;
            }

            if (data != null && _glfw.x11.XFree != null)
                _glfw.x11.XFree(data);
        }
        else if (@event->clientMessageType == _glfw.x11.XdndDrop)
        {
            if (_glfw.x11.xdndVersion > (int)_GLFW_XDND_VERSION)
                return;

            if (_glfw.x11.xdndFormat != 0 && _glfw.x11.XConvertSelection != null)
            {
                var time = CurrentTime;
                if (_glfw.x11.xdndVersion >= 1)
                    time = (ulong)@event->clientData2;

                _glfw.x11.XConvertSelection(_glfw.x11.display,
                    _glfw.x11.XdndSelection,
                    _glfw.x11.xdndFormat,
                    _glfw.x11.XdndSelection,
                    window->x11.handle,
                    time);
            }
            else
            {
                x11_sendXdndFinished(window, GLFW_FALSE);
            }
        }
        else if (@event->clientMessageType == _glfw.x11.XdndPosition)
        {
            if (_glfw.x11.xdndVersion > (int)_GLFW_XDND_VERSION ||
                _glfw.x11.XSendEvent == null)
            {
                return;
            }

            var packed = (ulong)@event->clientData2;
            var xabs = (int)((packed >> 16) & 0xffff);
            var yabs = (int)(packed & 0xffff);
            var xpos = xabs;
            var ypos = yabs;
            nuint dummy;

            if (_glfw.x11.XTranslateCoordinates != null)
            {
                _glfw.x11.XTranslateCoordinates(_glfw.x11.display,
                    _glfw.x11.root,
                    window->x11.handle,
                    xabs,
                    yabs,
                    &xpos,
                    &ypos,
                    &dummy);
            }

            _glfwInputCursorPos(window, xpos, ypos);

            XEvent reply = default;
            reply.type = ClientMessage;
            reply.anyWindow = _glfw.x11.xdndSource;
            reply.clientMessageType = _glfw.x11.XdndStatus;
            reply.clientFormat = 32;
            reply.clientData0 = (nint)window->x11.handle;
            reply.clientData2 = 0;
            reply.clientData3 = 0;

            if (_glfw.x11.xdndFormat != 0)
            {
                reply.clientData1 = 1;
                if (_glfw.x11.xdndVersion >= 2)
                    reply.clientData4 = (nint)_glfw.x11.XdndActionCopy;
            }

            _glfw.x11.XSendEvent(_glfw.x11.display,
                _glfw.x11.xdndSource,
                GLFW_FALSE,
                (nint)NoEventMask,
                &reply);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    static void x11_processXdndSelection(_GLFWwindow* window, XEvent* @event)
    {
        if (@event->selectionNotifyProperty != _glfw.x11.XdndSelection)
            return;

        byte* data;
        var result = x11_getWindowProperty(@event->selectionNotifyRequestor,
            @event->selectionNotifyProperty,
            @event->selectionNotifyTarget,
            GLFW_FALSE,
            &data);

        if (result != 0 && data != null)
        {
            int count;
            var paths = _glfwParseUriList(data, &count);
            if (paths != null)
            {
                _glfwInputDrop(window, count, paths);

                for (var i = 0; i < count; i++)
                    _glfw_free(paths[i]);
                _glfw_free(paths);
            }
        }

        if (data != null && _glfw.x11.XFree != null)
            _glfw.x11.XFree(data);

        x11_sendXdndFinished(window, result != 0 ? GLFW_TRUE : GLFW_FALSE);
    }

    static void x11_setXIMask(byte* mask, int eventType)
    {
        mask[eventType >> 3] |= (byte)(1 << (eventType & 7));
    }

    static int x11_isXIMaskSet(byte* mask, int eventType)
    {
        return (mask[eventType >> 3] & (1 << (eventType & 7))) != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void x11_setRawMouseMotionEvents(int enabled)
    {
        if (_glfw.x11.xiAvailable == 0 || _glfw.x11.XISelectEvents == null)
            return;

        var mask = stackalloc byte[3];
        if (enabled != 0)
            x11_setXIMask(mask, XI_RawMotion);

        XIEventMask eventMask = default;
        eventMask.deviceid = XIAllMasterDevices;
        eventMask.mask_len = 3;
        eventMask.mask = mask;

        _glfw.x11.XISelectEvents(_glfw.x11.display, _glfw.x11.root, &eventMask, 1);
        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void x11_processRawMotion(XEvent* @event)
    {
        if (_glfw.x11.xiAvailable == 0 ||
            _glfw.x11.XGetEventData == null ||
            _glfw.x11.XFreeEventData == null ||
            @event->genericExtension != _glfw.x11.xiMajorOpcode)
        {
            return;
        }

        if (_glfw.x11.XGetEventData(_glfw.x11.display, @event) == 0)
            return;

        if (@event->genericEvType == XI_RawMotion)
        {
            var window = _glfw.x11.disabledCursorWindow;
            var raw = (XIRawEvent*)@event->genericData;
            if (window != null &&
                window->rawMouseMotion != 0 &&
                raw != null &&
                raw->valuators.mask_len > 0)
            {
                var values = raw->raw_values;
                var xpos = window->virtualCursorPosX;
                var ypos = window->virtualCursorPosY;

                if (x11_isXIMaskSet(raw->valuators.mask, 0) != 0)
                {
                    xpos += *values;
                    values++;
                }

                if (x11_isXIMaskSet(raw->valuators.mask, 1) != 0)
                    ypos += *values;

                _glfwInputCursorPos(window, xpos, ypos);
            }
        }

        _glfw.x11.XFreeEventData(_glfw.x11.display, @event);
    }

    static int x11_isKeyReleaseRepeat(XEvent* @event)
    {
        if (_glfw.x11.xkbDetectable != 0 ||
            _glfw.x11.XEventsQueued == null ||
            _glfw.x11.XPeekEvent == null ||
            _glfw.x11.XEventsQueued(_glfw.x11.display, QueuedAfterReading) == 0)
        {
            return GLFW_FALSE;
        }

        XEvent next;
        _glfw.x11.XPeekEvent(_glfw.x11.display, &next);

        if (next.type == KeyPress &&
            next.anyWindow == @event->anyWindow &&
            next.keycode == @event->keycode &&
            next.time - @event->time < 20)
        {
            return GLFW_TRUE;
        }

        return GLFW_FALSE;
    }

    static byte* x11_convertSelectionToString(nuint selection, nuint target)
    {
        if (_glfw.x11.helperWindowHandle == 0 ||
            _glfw.x11.GLFW_SELECTION == 0 ||
            _glfw.x11.XConvertSelection == null ||
            _glfw.x11.XGetWindowProperty == null)
        {
            return null;
        }

        if (_glfw.x11.XDeleteProperty != null)
            _glfw.x11.XDeleteProperty(_glfw.x11.display, _glfw.x11.helperWindowHandle, _glfw.x11.GLFW_SELECTION);

        _glfw.x11.XConvertSelection(_glfw.x11.display,
            selection,
            target,
            _glfw.x11.GLFW_SELECTION,
            _glfw.x11.helperWindowHandle,
            CurrentTime);
        _glfw.x11.XFlush(_glfw.x11.display);

        var deadline = _glfwPlatformGetTimerValue() + 5UL * _glfwPlatformGetTimerFrequency();
        while (_glfwPlatformGetTimerValue() < deadline)
        {
            if (_glfw.x11.XPending(_glfw.x11.display) == 0)
            {
                Thread.Sleep(1);
                continue;
            }

            XEvent @event;
            _glfw.x11.XNextEvent(_glfw.x11.display, &@event);

            if (@event.type == SelectionNotify &&
                @event.selectionNotifyRequestor == _glfw.x11.helperWindowHandle &&
                @event.selectionNotifySelection == selection &&
                @event.selectionNotifyTarget == target)
            {
                if (@event.selectionNotifyProperty == 0)
                    return null;

                byte* data;
                var itemCount = x11_getWindowProperty(_glfw.x11.helperWindowHandle,
                    @event.selectionNotifyProperty,
                    target,
                    GLFW_TRUE,
                    &data);
                if (itemCount == 0 || data == null)
                    return null;

                var result = (byte*)_glfw_calloc(itemCount + 1, 1);
                if (result != null)
                    _glfw_memcpy(result, data, itemCount);

                if (_glfw.x11.XFree != null)
                    _glfw.x11.XFree(data);

                return result;
            }

            x11_processEvent(&@event);
        }

        return null;
    }

    static void x11_updateNormalHints(_GLFWwindow* window, int width, int height)
    {
        if (window->x11.handle == 0 || _glfw.x11.XSetWMNormalHints == null)
            return;

        XSizeHints hints = default;

        if (window->monitor == null)
        {
            if (window->resizable != 0)
            {
                if (window->minwidth != GLFW_DONT_CARE &&
                    window->minheight != GLFW_DONT_CARE)
                {
                    hints.flags |= PMinSize;
                    hints.min_width = window->minwidth;
                    hints.min_height = window->minheight;
                }

                if (window->maxwidth != GLFW_DONT_CARE &&
                    window->maxheight != GLFW_DONT_CARE)
                {
                    hints.flags |= PMaxSize;
                    hints.max_width = window->maxwidth;
                    hints.max_height = window->maxheight;
                }

                if (window->numer != GLFW_DONT_CARE &&
                    window->denom != GLFW_DONT_CARE)
                {
                    hints.flags |= PAspect;
                    hints.min_aspect_x = hints.max_aspect_x = window->numer;
                    hints.min_aspect_y = hints.max_aspect_y = window->denom;
                }
            }
            else
            {
                hints.flags |= PMinSize | PMaxSize;
                hints.min_width = hints.max_width = width;
                hints.min_height = hints.max_height = height;
            }
        }

        _glfw.x11.XSetWMNormalHints(_glfw.x11.display, window->x11.handle, &hints);
    }

    static void x11_updateCursorImage(_GLFWwindow* window)
    {
        if (window->x11.handle == 0)
            return;

        if (window->cursorMode == GLFW_CURSOR_NORMAL ||
            window->cursorMode == GLFW_CURSOR_CAPTURED)
        {
            if (window->cursor != null && window->cursor->x11.handle != 0 && _glfw.x11.XDefineCursor != null)
                _glfw.x11.XDefineCursor(_glfw.x11.display, window->x11.handle, window->cursor->x11.handle);
            else if (_glfw.x11.XUndefineCursor != null)
                _glfw.x11.XUndefineCursor(_glfw.x11.display, window->x11.handle);
        }
        else if (_glfw.x11.hiddenCursorHandle != 0 && _glfw.x11.XDefineCursor != null)
        {
            _glfw.x11.XDefineCursor(_glfw.x11.display, window->x11.handle, _glfw.x11.hiddenCursorHandle);
        }

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void x11_updateWindowMode(_GLFWwindow* window)
    {
        if (window->x11.handle == 0)
            return;

        if (window->monitor != null)
        {
            if (_glfw.x11.NET_WM_FULLSCREEN_MONITORS != 0)
            {
                x11_sendEventToWM(window,
                    _glfw.x11.NET_WM_FULLSCREEN_MONITORS,
                    window->monitor->x11.index,
                    window->monitor->x11.index,
                    window->monitor->x11.index,
                    window->monitor->x11.index,
                    0);
            }

            if (_glfw.x11.NET_WM_STATE != 0 && _glfw.x11.NET_WM_STATE_FULLSCREEN != 0)
            {
                x11_sendEventToWM(window,
                    _glfw.x11.NET_WM_STATE,
                    _NET_WM_STATE_ADD,
                    (nint)_glfw.x11.NET_WM_STATE_FULLSCREEN,
                    0,
                    1,
                    0);
            }

            if (window->x11.transparent == 0 &&
                _glfw.x11.NET_WM_BYPASS_COMPOSITOR != 0 &&
                _glfw.x11.XChangeProperty != null)
            {
                var value = (nuint)1;
                _glfw.x11.XChangeProperty(_glfw.x11.display,
                    window->x11.handle,
                    _glfw.x11.NET_WM_BYPASS_COMPOSITOR,
                    XA_CARDINAL,
                    32,
                    PropModeReplace,
                    (byte*)&value,
                    1);
            }
        }
        else
        {
            if (_glfw.x11.NET_WM_FULLSCREEN_MONITORS != 0 && _glfw.x11.XDeleteProperty != null)
                _glfw.x11.XDeleteProperty(_glfw.x11.display, window->x11.handle, _glfw.x11.NET_WM_FULLSCREEN_MONITORS);

            if (_glfw.x11.NET_WM_STATE != 0 && _glfw.x11.NET_WM_STATE_FULLSCREEN != 0)
            {
                x11_sendEventToWM(window,
                    _glfw.x11.NET_WM_STATE,
                    _NET_WM_STATE_REMOVE,
                    (nint)_glfw.x11.NET_WM_STATE_FULLSCREEN,
                    0,
                    1,
                    0);
            }

            if (_glfw.x11.NET_WM_BYPASS_COMPOSITOR != 0 && _glfw.x11.XDeleteProperty != null)
                _glfw.x11.XDeleteProperty(_glfw.x11.display, window->x11.handle, _glfw.x11.NET_WM_BYPASS_COMPOSITOR);
        }

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void x11_inputText(_GLFWwindow* window, byte* chars, int count, int mods, int plain)
    {
        if (count <= 0)
            return;

        var text = System.Text.Encoding.UTF8.GetString(new ReadOnlySpan<byte>(chars, count));
        foreach (var ch in text)
        {
            if (!char.IsSurrogate(ch))
                _glfwInputChar(window, ch, mods, plain);
        }
    }

    static void x11_setWindowTitle(_GLFWwindow* window, byte* title)
    {
        if (window->x11.handle == 0)
            return;

        if (_glfw.x11.XStoreName != null)
            _glfw.x11.XStoreName(_glfw.x11.display, window->x11.handle, title);

        if (_glfw.x11.XChangeProperty != null &&
            _glfw.x11.NET_WM_NAME != 0 &&
            _glfw.x11.UTF8_STRING != 0)
        {
            _glfw.x11.XChangeProperty(_glfw.x11.display,
                window->x11.handle,
                _glfw.x11.NET_WM_NAME,
                _glfw.x11.UTF8_STRING,
                8,
                PropModeReplace,
                title,
                _glfw_strlen(title));

            if (_glfw.x11.NET_WM_ICON_NAME != 0)
            {
                _glfw.x11.XChangeProperty(_glfw.x11.display,
                    window->x11.handle,
                    _glfw.x11.NET_WM_ICON_NAME,
                    _glfw.x11.UTF8_STRING,
                    8,
                    PropModeReplace,
                    title,
                    _glfw_strlen(title));
            }
        }
    }

    static void x11_setWindowClass(_GLFWwindow* window, _GLFWwndconfig* wndconfig)
    {
        if (window->x11.handle == 0 || _glfw.x11.XSetClassHint == null)
            return;

        var instanceName = stackalloc byte[_GLFW_MESSAGE_SIZE];
        var className = stackalloc byte[_GLFW_MESSAGE_SIZE];

        if (wndconfig->x11.instanceName[0] != 0)
        {
            _glfw_strncpy(instanceName, wndconfig->x11.instanceName, (nuint)_GLFW_MESSAGE_SIZE);
        }
        else
        {
            var resourceName = System.Environment.GetEnvironmentVariable("RESOURCE_NAME");
            _glfw_strcpy(instanceName,
                !string.IsNullOrEmpty(resourceName) ? resourceName : "glfw-application");
        }

        if (wndconfig->x11.className[0] != 0)
            _glfw_strncpy(className, wndconfig->x11.className, (nuint)_GLFW_MESSAGE_SIZE);
        else
            _glfw_strcpy(className, "GLFW-Application");

        XClassHint hint = default;
        hint.res_name = instanceName;
        hint.res_class = className;

        _glfw.x11.XSetClassHint(_glfw.x11.display, window->x11.handle, &hint);
    }

    static void x11_processEvent(XEvent* @event)
    {
        if (@event->type == GenericEvent)
        {
            x11_processRawMotion(@event);
            return;
        }

        if (@event->type == SelectionRequest)
        {
            x11_handleSelectionRequest(@event);
            return;
        }

        if (_glfw.x11.randrAvailable != 0 &&
            @event->type == _glfw.x11.randrEventBase + RRNotify)
        {
            if (_glfw.x11.XRRUpdateConfiguration != null)
                _glfw.x11.XRRUpdateConfiguration(@event);

            _glfwPollMonitorsX11();
            return;
        }

        if (_glfw.x11.xkbAvailable != 0 &&
            @event->type == _glfw.x11.xkbEventBase + XkbEventCode)
        {
            if (@event->xkbType == XkbStateNotify &&
                (@event->xkbStateChanged & XkbGroupStateMask) != 0)
            {
                _glfw.x11.xkbGroup = (uint)@event->xkbStateGroup;
            }

            return;
        }

        if (@event->type == ClientMessage &&
            @event->anyWindow == _glfw.x11.helperWindowHandle &&
            @event->clientMessageType == _glfw.x11.GLFW_EMPTY_EVENT)
        {
            return;
        }

        var window = x11_findWindowForEvent(@event);
        if (window == null)
            return;

        switch (@event->type)
        {
            case KeyPress:
            case KeyRelease:
            {
                var mods = x11_translateState(@event->state);
                var plain = (mods & (GLFW_MOD_CONTROL | GLFW_MOD_ALT)) == 0 ? GLFW_TRUE : GLFW_FALSE;
                nuint keysym = 0;
                var buffer = stackalloc byte[64];
                var count = _glfw.x11.XLookupString != null
                    ? _glfw.x11.XLookupString(@event, buffer, 63, &keysym, null)
                    : 0;
                var scancode = (int)@event->keycode;
                var key = GLFW_KEY_UNKNOWN;
                if (scancode >= 0 && scancode <= _GLFW_X11_KEYCODE_LAST)
                {
                    fixed (short* keycodes = _glfw.x11.keycodes)
                        key = keycodes[scancode];
                }

                if (key == GLFW_KEY_UNKNOWN)
                    key = x11_translateKeySym(keysym);

                var action = @event->type == KeyPress ? GLFW_PRESS : GLFW_RELEASE;

                if (action == GLFW_RELEASE && x11_isKeyReleaseRepeat(@event) != 0)
                    return;

                _glfwInputKey(window, key, scancode, action, mods);

                if (@event->type == KeyPress)
                {
                    if (count > 0)
                        x11_inputText(window, buffer, count, mods, plain);
                    else
                    {
                        var codepoint = _glfwKeySym2Unicode((uint)keysym);
                        if (codepoint != GLFW_INVALID_CODEPOINT)
                            _glfwInputChar(window, codepoint, mods, plain);
                    }
                }

                return;
            }

            case ButtonPress:
            case ButtonRelease:
            {
                var mods = x11_translateState(@event->state);
                var action = @event->type == ButtonPress ? GLFW_PRESS : GLFW_RELEASE;
                var button = (int)@event->button;

                if (action == GLFW_PRESS)
                {
                    if (button == 4)
                    {
                        _glfwInputScroll(window, 0.0, 1.0);
                        return;
                    }
                    if (button == 5)
                    {
                        _glfwInputScroll(window, 0.0, -1.0);
                        return;
                    }
                    if (button == 6)
                    {
                        _glfwInputScroll(window, 1.0, 0.0);
                        return;
                    }
                    if (button == 7)
                    {
                        _glfwInputScroll(window, -1.0, 0.0);
                        return;
                    }
                }

                var glfwButton = button switch
                {
                    1 => GLFW_MOUSE_BUTTON_LEFT,
                    2 => GLFW_MOUSE_BUTTON_MIDDLE,
                    3 => GLFW_MOUSE_BUTTON_RIGHT,
                    > 7 => button - 5,
                    _ => -1
                };

                if (glfwButton >= 0)
                    _glfwInputMouseClick(window, glfwButton, action, mods);

                return;
            }

            case EnterNotify:
                window->x11.lastCursorPosX = @event->x;
                window->x11.lastCursorPosY = @event->y;
                _glfwInputCursorEnter(window, GLFW_TRUE);
                _glfwInputCursorPos(window, @event->x, @event->y);
                return;

            case LeaveNotify:
                _glfwInputCursorEnter(window, GLFW_FALSE);
                return;

            case MotionNotify:
                window->x11.lastCursorPosX = @event->x;
                window->x11.lastCursorPosY = @event->y;
                _glfwInputCursorPos(window, @event->x, @event->y);
                return;

            case ConfigureNotify:
                if (@event->configureWidth != window->x11.width ||
                    @event->configureHeight != window->x11.height)
                {
                    window->x11.width = @event->configureWidth;
                    window->x11.height = @event->configureHeight;
                    _glfwInputFramebufferSize(window, window->x11.width, window->x11.height);
                    _glfwInputWindowSize(window, window->x11.width, window->x11.height);
                }

                if (@event->configureX != window->x11.xpos ||
                    @event->configureY != window->x11.ypos)
                {
                    window->x11.xpos = @event->configureX;
                    window->x11.ypos = @event->configureY;
                    _glfwInputWindowPos(window, window->x11.xpos, window->x11.ypos);
                }
                return;

            case ReparentNotify:
                window->x11.parent = @event->reparentParent;
                return;

            case ClientMessage:
                if (@event->clientMessageType == _glfw.x11.WM_PROTOCOLS &&
                    (nuint)@event->clientData0 == _glfw.x11.WM_DELETE_WINDOW)
                {
                    _glfwInputWindowCloseRequest(window);
                }
                else if (@event->clientMessageType == _glfw.x11.WM_PROTOCOLS &&
                         (nuint)@event->clientData0 == _glfw.x11.NET_WM_PING)
                {
                    @event->anyWindow = _glfw.x11.root;
                    _glfw.x11.XSendEvent(_glfw.x11.display,
                        _glfw.x11.root,
                        GLFW_FALSE,
                        (nint)(SubstructureNotifyMask | SubstructureRedirectMask),
                        @event);
                }
                else if (@event->clientMessageType == _glfw.x11.XdndEnter ||
                         @event->clientMessageType == _glfw.x11.XdndPosition ||
                         @event->clientMessageType == _glfw.x11.XdndDrop)
                {
                    x11_processXdndClientMessage(window, @event);
                }
                return;

            case SelectionNotify:
                x11_processXdndSelection(window, @event);
                return;

            case FocusIn:
                if (@event->focusMode != NotifyGrab && @event->focusMode != NotifyUngrab)
                {
                    window->x11.focused = GLFW_TRUE;
                    _glfwInputWindowFocus(window, GLFW_TRUE);
                }
                return;

            case FocusOut:
                if (@event->focusMode != NotifyGrab && @event->focusMode != NotifyUngrab)
                {
                    window->x11.focused = GLFW_FALSE;
                    _glfwInputWindowFocus(window, GLFW_FALSE);
                }
                return;

            case Expose:
                _glfwInputWindowDamage(window);
                return;

            case PropertyNotify:
                if (@event->propertyState != PropertyNewValue)
                    return;

                if (@event->propertyAtom == _glfw.x11.WM_STATE)
                {
                    var state = x11_getWindowState(window);
                    if (state != IconicState && state != NormalState)
                        return;

                    var iconified = state == IconicState ? GLFW_TRUE : GLFW_FALSE;
                    if (window->x11.iconified != iconified)
                    {
                        if (window->monitor != null)
                        {
                            if (iconified != 0)
                            {
                                _glfwRestoreVideoModeX11(window->monitor);
                                _glfwInputMonitorWindow(window->monitor, null);
                            }
                            else
                            {
                                _glfwSetVideoModeX11(window->monitor, &window->videoMode);
                                _glfwInputMonitorWindow(window->monitor, window);
                            }
                        }

                        window->x11.iconified = iconified;
                        _glfwInputWindowIconify(window, iconified);
                    }
                }
                else if (@event->propertyAtom == _glfw.x11.NET_WM_STATE)
                {
                    var maximized = x11_queryWindowMaximized(window);
                    if (window->x11.maximized != maximized)
                    {
                        window->x11.maximized = maximized;
                        _glfwInputWindowMaximize(window, maximized);
                    }
                }

                return;

            case MapNotify:
                window->x11.visible = GLFW_TRUE;
                window->x11.iconified = GLFW_FALSE;
                _glfwInputWindowIconify(window, GLFW_FALSE);
                return;

            case UnmapNotify:
                window->x11.visible = GLFW_FALSE;
                window->x11.iconified = GLFW_TRUE;
                _glfwInputWindowIconify(window, GLFW_TRUE);
                return;

            case DestroyNotify:
                window->x11.handle = 0;
                return;
        }
    }

    static int x11_createNativeWindow(_GLFWwindow* window,
                                      _GLFWwndconfig* wndconfig,
                                      void* visual,
                                      int depth)
    {
        var width = wndconfig->width;
        var height = wndconfig->height;

        if (wndconfig->scaleToMonitor != 0)
        {
            width = (int)(width * _glfw.x11.contentScaleX);
            height = (int)(height * _glfw.x11.contentScaleY);
        }

        var xpos = 0;
        var ypos = 0;

        if (wndconfig->xpos != GLFW_ANY_POSITION && wndconfig->ypos != GLFW_ANY_POSITION)
        {
            xpos = wndconfig->xpos;
            ypos = wndconfig->ypos;
        }

        window->x11.colormap = _glfw.x11.XCreateColormap(_glfw.x11.display, _glfw.x11.root, visual, AllocNone);
        if (window->x11.colormap == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to create colormap");
            return GLFW_FALSE;
        }

        window->x11.transparent = _glfwIsVisualTransparentX11(visual);

        XSetWindowAttributes wa = default;
        wa.border_pixel = 0;
        wa.colormap = window->x11.colormap;
        wa.event_mask = (nint)(StructureNotifyMask |
                               KeyPressMask |
                               KeyReleaseMask |
                               PointerMotionMask |
                               ButtonPressMask |
                               ButtonReleaseMask |
                               ExposureMask |
                               FocusChangeMask |
                               VisibilityChangeMask |
                               EnterWindowMask |
                               LeaveWindowMask |
                               PropertyChangeMask);

        _glfwGrabErrorHandlerX11();

        window->x11.parent = _glfw.x11.root;
        window->x11.handle = _glfw.x11.XCreateWindow(_glfw.x11.display,
            _glfw.x11.root,
            xpos,
            ypos,
            (uint)width,
            (uint)height,
            0,
            depth,
            InputOutput,
            visual,
            (nuint)(CWBorderPixel | CWColormap | CWEventMask),
            &wa);

        _glfwReleaseErrorHandlerX11();

        if (window->x11.handle == 0)
        {
            _glfwInputErrorX11(GLFW_PLATFORM_ERROR, "X11: Failed to create window");
            return GLFW_FALSE;
        }

        window->x11.width = width;
        window->x11.height = height;
        window->x11.xpos = xpos;
        window->x11.ypos = ypos;
        window->x11.opacity = 1f;

        x11_setWindowClass(window, wndconfig);
        x11_setWindowTitle(window, wndconfig->title);
        x11_updateNormalHints(window, width, height);

        if (_glfw.x11.XdndAware != 0 && _glfw.x11.XChangeProperty != null)
        {
            var version = _GLFW_XDND_VERSION;
            _glfw.x11.XChangeProperty(_glfw.x11.display,
                window->x11.handle,
                _glfw.x11.XdndAware,
                XA_ATOM,
                32,
                PropModeReplace,
                (byte*)&version,
                1);
        }

        if (_glfw.x11.WM_DELETE_WINDOW != 0 || _glfw.x11.NET_WM_PING != 0)
        {
            var count = 0;
            var protocols = stackalloc nuint[2];

            if (_glfw.x11.WM_DELETE_WINDOW != 0)
                protocols[count++] = _glfw.x11.WM_DELETE_WINDOW;
            if (_glfw.x11.NET_WM_PING != 0)
                protocols[count++] = _glfw.x11.NET_WM_PING;

            _glfw.x11.XSetWMProtocols(_glfw.x11.display, window->x11.handle, protocols, count);
        }

        return GLFW_TRUE;
    }

    static void _glfwGetCursorPosX11(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (xpos != null)
            *xpos = window->x11.lastCursorPosX;
        if (ypos != null)
            *ypos = window->x11.lastCursorPosY;
    }

    static void _glfwSetCursorPosX11(_GLFWwindow* window, double xpos, double ypos)
    {
        window->x11.warpCursorPosX = (int)xpos;
        window->x11.warpCursorPosY = (int)ypos;
        window->x11.lastCursorPosX = (int)xpos;
        window->x11.lastCursorPosY = (int)ypos;

        if (window->x11.handle != 0 && _glfw.x11.XWarpPointer != null)
        {
            _glfw.x11.XWarpPointer(_glfw.x11.display,
                0,
                window->x11.handle,
                0,
                0,
                0,
                0,
                (int)xpos,
                (int)ypos);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    static void _glfwSetCursorModeX11(_GLFWwindow* window, int mode)
    {
        if (mode == GLFW_CURSOR_DISABLED)
        {
            double restoreX;
            double restoreY;
            _glfwGetCursorPosX11(window, &restoreX, &restoreY);
            _glfw.x11.restoreCursorPosX = restoreX;
            _glfw.x11.restoreCursorPosY = restoreY;
            _glfw.x11.disabledCursorWindow = window;
            _glfwSetCursorPosX11(window, window->x11.width / 2.0, window->x11.height / 2.0);
            if (window->rawMouseMotion != 0)
                x11_setRawMouseMotionEvents(GLFW_TRUE);
        }
        else if (_glfw.x11.disabledCursorWindow == window)
        {
            if (window->rawMouseMotion != 0)
                x11_setRawMouseMotionEvents(GLFW_FALSE);
            _glfw.x11.disabledCursorWindow = null;
            _glfwSetCursorPosX11(window, _glfw.x11.restoreCursorPosX, _glfw.x11.restoreCursorPosY);
        }

        x11_updateCursorImage(window);
    }

    static void _glfwSetRawMouseMotionX11(_GLFWwindow* window, int enabled)
    {
        if (_glfw.x11.disabledCursorWindow != window)
            return;

        x11_setRawMouseMotionEvents(enabled);
    }

    static int _glfwRawMouseMotionSupportedX11()
    {
        return _glfw.x11.xiAvailable;
    }

    static int _glfwCreateCursorX11(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        cursor->x11.handle = _glfwCreateNativeCursorX11(image, xhot, yhot);
        return cursor->x11.handle != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwCreateStandardCursorX11(_GLFWcursor* cursor, int shape)
    {
        if (_glfw.x11.XCreateFontCursor == null)
            return GLFW_FALSE;

        uint native = shape switch
        {
            GLFW_ARROW_CURSOR => XC_arrow,
            GLFW_IBEAM_CURSOR => XC_xterm,
            GLFW_CROSSHAIR_CURSOR => XC_crosshair,
            GLFW_POINTING_HAND_CURSOR => XC_hand2,
            GLFW_RESIZE_EW_CURSOR => XC_sb_h_double_arrow,
            GLFW_RESIZE_NS_CURSOR => XC_sb_v_double_arrow,
            GLFW_RESIZE_NWSE_CURSOR => XC_top_left_corner,
            GLFW_RESIZE_NESW_CURSOR => XC_top_right_corner,
            GLFW_RESIZE_ALL_CURSOR => XC_fleur,
            GLFW_NOT_ALLOWED_CURSOR => XC_X_cursor,
            _ => XC_arrow
        };

        cursor->x11.handle = _glfw.x11.XCreateFontCursor(_glfw.x11.display, native);
        return cursor->x11.handle != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void _glfwDestroyCursorX11(_GLFWcursor* cursor)
    {
        if (cursor->x11.handle != 0 && _glfw.x11.XFreeCursor != null)
        {
            _glfw.x11.XFreeCursor(_glfw.x11.display, cursor->x11.handle);
            cursor->x11.handle = 0;
        }
    }

    static void _glfwSetCursorX11(_GLFWwindow* window, _GLFWcursor* cursor)
    {
        x11_updateCursorImage(window);
    }

    static byte* _glfwGetScancodeNameX11(int scancode)
    {
        if (scancode < 0 || scancode > _GLFW_X11_KEYCODE_LAST)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid scancode {0}", scancode);
            return null;
        }

        var key = GLFW_KEY_UNKNOWN;
        fixed (short* keycodes = _glfw.x11.keycodes)
            key = keycodes[scancode];

        if (key == GLFW_KEY_UNKNOWN || _glfw.x11.XkbKeycodeToKeysym == null)
            return null;

        var keysym = _glfw.x11.XkbKeycodeToKeysym(_glfw.x11.display, (uint)scancode, (int)_glfw.x11.xkbGroup, 0);
        if (keysym == 0)
            return null;

        var codepoint = _glfwKeySym2Unicode((uint)keysym);
        if (codepoint == GLFW_INVALID_CODEPOINT)
            return null;

        fixed (byte* keyname = _glfw.x11.keyname)
        {
            var count = _glfwEncodeUTF8(keyname, codepoint);
            if (count == 0)
                return null;

            keyname[count] = 0;
            return keyname;
        }
    }

    static int _glfwGetKeyScancodeX11(int key)
    {
        if (key < GLFW_KEY_SPACE || key > GLFW_KEY_LAST)
            return -1;

        fixed (short* scancodes = _glfw.x11.scancodes)
            return scancodes[key];
    }

    static void _glfwSetClipboardStringX11(byte* value)
    {
        _glfw_free(_glfw.x11.clipboardString);
        _glfw.x11.clipboardString = _glfw_strdup(value);

        if (_glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.CLIPBOARD != 0 &&
            _glfw.x11.XSetSelectionOwner != null)
        {
            _glfw.x11.XSetSelectionOwner(_glfw.x11.display,
                _glfw.x11.CLIPBOARD,
                _glfw.x11.helperWindowHandle,
                CurrentTime);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    static byte* _glfwGetClipboardStringX11()
    {
        if (_glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.CLIPBOARD != 0 &&
            _glfw.x11.XGetSelectionOwner != null)
        {
            var owner = _glfw.x11.XGetSelectionOwner(_glfw.x11.display, _glfw.x11.CLIPBOARD);
            if (owner != 0 && owner != _glfw.x11.helperWindowHandle)
            {
                var stringValue = x11_convertSelectionToString(_glfw.x11.CLIPBOARD, _glfw.x11.UTF8_STRING);
                if (stringValue == null)
                    stringValue = x11_convertSelectionToString(_glfw.x11.CLIPBOARD, XA_STRING);

                if (stringValue != null)
                {
                    _glfw_free(_glfw.x11.clipboardString);
                    _glfw.x11.clipboardString = stringValue;
                }
            }
        }

        return _glfw.x11.clipboardString;
    }

    static int _glfwCreateWindowX11(_GLFWwindow* window,
                                    _GLFWwndconfig* wndconfig,
                                    _GLFWctxconfig* ctxconfig,
                                    _GLFWfbconfig* fbconfig)
    {
        void* visual = null;
        var depth = 0;

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API)
            {
                if (_glfwInitGLX() == 0)
                    return GLFW_FALSE;
                if (_glfwChooseVisualGLX(wndconfig, ctxconfig, fbconfig, &visual, &depth) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                if (_glfwInitEGL() == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_OSMESA_CONTEXT_API)
            {
                if (_glfwInitOSMesa() == 0)
                    return GLFW_FALSE;
            }
        }

        if (visual == null)
        {
            visual = _glfw.x11.XDefaultVisual(_glfw.x11.display, _glfw.x11.screen);
            depth = _glfw.x11.XDefaultDepth(_glfw.x11.display, _glfw.x11.screen);
        }

        if (visual == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "X11: Failed to retrieve default Visual");
            return GLFW_FALSE;
        }

        if (x11_createNativeWindow(window, wndconfig, visual, depth) == 0)
            return GLFW_FALSE;

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API)
            {
                if (_glfwCreateContextGLX(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                if (_glfwCreateContextEGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_OSMESA_CONTEXT_API)
            {
                if (_glfwCreateContextOSMesa(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }

            if (_glfwRefreshContextAttribs(window, ctxconfig) == 0)
                return GLFW_FALSE;
        }

        if (window->monitor != null)
        {
            _glfwSetVideoModeX11(window->monitor, &window->videoMode);
            _glfwInputMonitorWindow(window->monitor, window);
            x11_updateWindowMode(window);
            _glfwShowWindowX11(window);
            _glfwFocusWindowX11(window);
        }
        else if (wndconfig->visible != 0)
        {
            _glfwShowWindowX11(window);
        }

        _glfw.x11.XFlush(_glfw.x11.display);
        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowX11(_GLFWwindow* window)
    {
        if (window->monitor != null)
        {
            _glfwRestoreVideoModeX11(window->monitor);
            _glfwInputMonitorWindow(window->monitor, null);
        }

        if (window->context.destroy != null)
            window->context.destroy(window);

        if (window->x11.handle != 0)
        {
            if (_glfw.x11.XUnmapWindow != null)
                _glfw.x11.XUnmapWindow(_glfw.x11.display, window->x11.handle);

            _glfw.x11.XDestroyWindow(_glfw.x11.display, window->x11.handle);
            window->x11.handle = 0;
        }

        if (window->x11.colormap != 0)
        {
            _glfw.x11.XFreeColormap(_glfw.x11.display, window->x11.colormap);
            window->x11.colormap = 0;
        }
    }

    static void _glfwSetWindowTitleX11(_GLFWwindow* window, byte* title)
    {
        x11_setWindowTitle(window, title);
    }

    static void _glfwSetWindowIconX11(_GLFWwindow* window, int count, GLFWimage* images)
    {
        if (window->x11.handle == 0 || _glfw.x11.NET_WM_ICON == 0)
            return;

        if (count > 0)
        {
            var longCount = 0;
            for (var i = 0; i < count; i++)
                longCount += 2 + images[i].width * images[i].height;

            var icon = (nuint*)_glfw_calloc((nuint)longCount, (nuint)sizeof(nuint));
            if (icon == null)
                return;

            var target = icon;

            for (var i = 0; i < count; i++)
            {
                *target++ = (nuint)images[i].width;
                *target++ = (nuint)images[i].height;

                for (var j = 0; j < images[i].width * images[i].height; j++)
                {
                    var source = images[i].pixels + j * 4;
                    *target++ = ((nuint)source[0] << 16) |
                                ((nuint)source[1] << 8) |
                                source[2] |
                                ((nuint)source[3] << 24);
                }
            }

            if (_glfw.x11.XChangeProperty != null)
            {
                _glfw.x11.XChangeProperty(_glfw.x11.display,
                    window->x11.handle,
                    _glfw.x11.NET_WM_ICON,
                    XA_CARDINAL,
                    32,
                    PropModeReplace,
                    (byte*)icon,
                    longCount);
            }

            _glfw_free(icon);
        }
        else if (_glfw.x11.XDeleteProperty != null)
        {
            _glfw.x11.XDeleteProperty(_glfw.x11.display, window->x11.handle, _glfw.x11.NET_WM_ICON);
        }

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void _glfwGetWindowPosX11(_GLFWwindow* window, int* xpos, int* ypos)
    {
        if (window->x11.handle != 0 && _glfw.x11.XTranslateCoordinates != null)
        {
            int x;
            int y;
            nuint child;
            if (_glfw.x11.XTranslateCoordinates(_glfw.x11.display,
                    window->x11.handle,
                    _glfw.x11.root,
                    0,
                    0,
                    &x,
                    &y,
                    &child) != 0)
            {
                window->x11.xpos = x;
                window->x11.ypos = y;
            }
        }

        if (xpos != null)
            *xpos = window->x11.xpos;
        if (ypos != null)
            *ypos = window->x11.ypos;
    }

    static void _glfwSetWindowPosX11(_GLFWwindow* window, int xpos, int ypos)
    {
        window->x11.xpos = xpos;
        window->x11.ypos = ypos;
        if (window->x11.handle != 0 && _glfw.x11.XMoveWindow != null)
            _glfw.x11.XMoveWindow(_glfw.x11.display, window->x11.handle, xpos, ypos);
    }

    static void _glfwGetWindowSizeX11(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->x11.width;
        if (height != null)
            *height = window->x11.height;
    }

    static void _glfwSetWindowSizeX11(_GLFWwindow* window, int width, int height)
    {
        window->x11.width = width;
        window->x11.height = height;
        if (window->x11.handle != 0 && _glfw.x11.XResizeWindow != null)
            _glfw.x11.XResizeWindow(_glfw.x11.display, window->x11.handle, (uint)width, (uint)height);
    }

    static void _glfwSetWindowSizeLimitsX11(_GLFWwindow* window, int minwidth, int minheight, int maxwidth, int maxheight)
    {
        x11_updateNormalHints(window, window->x11.width, window->x11.height);
    }

    static void _glfwSetWindowAspectRatioX11(_GLFWwindow* window, int numer, int denom)
    {
        x11_updateNormalHints(window, window->x11.width, window->x11.height);
    }

    static void _glfwGetFramebufferSizeX11(_GLFWwindow* window, int* width, int* height)
    {
        _glfwGetWindowSizeX11(window, width, height);
    }

    static void _glfwGetWindowFrameSizeX11(_GLFWwindow* window, int* left, int* top, int* right, int* bottom)
    {
        if (_glfw.x11.NET_FRAME_EXTENTS != 0)
        {
            byte* data;
            var count = x11_getWindowProperty(window->x11.handle,
                _glfw.x11.NET_FRAME_EXTENTS,
                XA_CARDINAL,
                GLFW_FALSE,
                &data);
            if (count >= 4 && data != null)
            {
                var extents = (nuint*)data;
                if (left != null)
                    *left = (int)extents[0];
                if (right != null)
                    *right = (int)extents[1];
                if (top != null)
                    *top = (int)extents[2];
                if (bottom != null)
                    *bottom = (int)extents[3];

                if (_glfw.x11.XFree != null)
                    _glfw.x11.XFree(data);
                return;
            }

            if (data != null && _glfw.x11.XFree != null)
                _glfw.x11.XFree(data);
        }

        if (left != null)
            *left = 0;
        if (top != null)
            *top = 0;
        if (right != null)
            *right = 0;
        if (bottom != null)
            *bottom = 0;
    }

    static void _glfwGetWindowContentScaleX11(_GLFWwindow* window, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwIconifyWindowX11(_GLFWwindow* window)
    {
        window->x11.iconified = GLFW_TRUE;
        window->x11.visible = GLFW_FALSE;
        if (window->x11.handle != 0 && _glfw.x11.XIconifyWindow != null)
            _glfw.x11.XIconifyWindow(_glfw.x11.display, window->x11.handle, _glfw.x11.screen);
    }

    static void _glfwRestoreWindowX11(_GLFWwindow* window)
    {
        if (window->x11.maximized != 0)
        {
            x11_sendEventToWM(window,
                _glfw.x11.NET_WM_STATE,
                _NET_WM_STATE_REMOVE,
                (nint)_glfw.x11.NET_WM_STATE_MAXIMIZED_VERT,
                (nint)_glfw.x11.NET_WM_STATE_MAXIMIZED_HORZ,
                1,
                0);
            window->x11.maximized = GLFW_FALSE;
            _glfwInputWindowMaximize(window, GLFW_FALSE);
        }

        window->x11.iconified = GLFW_FALSE;
        _glfwShowWindowX11(window);
    }

    static void _glfwMaximizeWindowX11(_GLFWwindow* window)
    {
        x11_sendEventToWM(window,
            _glfw.x11.NET_WM_STATE,
            _NET_WM_STATE_ADD,
            (nint)_glfw.x11.NET_WM_STATE_MAXIMIZED_VERT,
            (nint)_glfw.x11.NET_WM_STATE_MAXIMIZED_HORZ,
            1,
            0);
        window->x11.maximized = GLFW_TRUE;
        _glfwInputWindowMaximize(window, GLFW_TRUE);
    }

    static void _glfwShowWindowX11(_GLFWwindow* window)
    {
        if (window->x11.handle != 0)
        {
            window->x11.visible = GLFW_TRUE;
            window->x11.iconified = GLFW_FALSE;
            _glfw.x11.XMapWindow(_glfw.x11.display, window->x11.handle);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    static void _glfwHideWindowX11(_GLFWwindow* window)
    {
        if (window->x11.handle != 0)
        {
            window->x11.visible = GLFW_FALSE;
            _glfw.x11.XUnmapWindow(_glfw.x11.display, window->x11.handle);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    static void _glfwRequestWindowAttentionX11(_GLFWwindow* window)
    {
        if (_glfw.x11.NET_WM_STATE != 0 && _glfw.x11.NET_WM_STATE_DEMANDS_ATTENTION != 0)
        {
            x11_sendEventToWM(window,
                _glfw.x11.NET_WM_STATE,
                _NET_WM_STATE_ADD,
                (nint)_glfw.x11.NET_WM_STATE_DEMANDS_ATTENTION,
                0,
                1,
                0);
        }
        else if (window->x11.handle != 0 && _glfw.x11.XRaiseWindow != null)
        {
            _glfw.x11.XRaiseWindow(_glfw.x11.display, window->x11.handle);
        }
    }

    static void _glfwFocusWindowX11(_GLFWwindow* window)
    {
        if (window->x11.handle != 0 && _glfw.x11.XSetInputFocus != null)
        {
            _glfw.x11.XSetInputFocus(_glfw.x11.display, window->x11.handle, RevertToParent, CurrentTime);
            window->x11.focused = GLFW_TRUE;
        }
    }

    static void _glfwSetWindowMonitorX11(_GLFWwindow* window, _GLFWmonitor* monitor, int xpos, int ypos, int width, int height, int refreshRate)
    {
        if (window->monitor == monitor)
        {
            if (monitor != null)
            {
                _glfwSetVideoModeX11(monitor, &window->videoMode);
                int mx;
                int my;
                GLFWvidmode mode;
                _glfwGetMonitorPosX11(monitor, &mx, &my);
                _glfwGetVideoModeX11(monitor, &mode);
                window->x11.xpos = mx;
                window->x11.ypos = my;
                window->x11.width = mode.width;
                window->x11.height = mode.height;

                if (window->x11.handle != 0 && _glfw.x11.XMoveResizeWindow != null)
                {
                    _glfw.x11.XMoveResizeWindow(_glfw.x11.display,
                        window->x11.handle,
                        mx,
                        my,
                        (uint)mode.width,
                        (uint)mode.height);
                }
            }
            else
            {
                _glfwSetWindowPosX11(window, xpos, ypos);
                _glfwSetWindowSizeX11(window, width, height);
            }

            return;
        }

        if (window->monitor != null)
        {
            _glfwRestoreVideoModeX11(window->monitor);
            _glfwInputMonitorWindow(window->monitor, null);
        }

        _glfwInputWindowMonitor(window, monitor);

        if (window->monitor != null)
        {
            _glfwSetVideoModeX11(window->monitor, &window->videoMode);
            _glfwInputMonitorWindow(window->monitor, window);
            x11_updateWindowMode(window);

            int mx;
            int my;
            GLFWvidmode mode;
            _glfwGetMonitorPosX11(window->monitor, &mx, &my);
            _glfwGetVideoModeX11(window->monitor, &mode);
            window->x11.xpos = mx;
            window->x11.ypos = my;
            window->x11.width = mode.width;
            window->x11.height = mode.height;

            if (window->x11.handle != 0 && _glfw.x11.XMoveResizeWindow != null)
            {
                _glfw.x11.XMoveResizeWindow(_glfw.x11.display,
                    window->x11.handle,
                    mx,
                    my,
                    (uint)mode.width,
                    (uint)mode.height);
                _glfwShowWindowX11(window);
            }
        }
        else
        {
            x11_updateWindowMode(window);
            window->x11.xpos = xpos;
            window->x11.ypos = ypos;
            window->x11.width = width;
            window->x11.height = height;

            if (window->x11.handle != 0 && _glfw.x11.XMoveResizeWindow != null)
            {
                _glfw.x11.XMoveResizeWindow(_glfw.x11.display,
                    window->x11.handle,
                    xpos,
                    ypos,
                    (uint)width,
                    (uint)height);
            }
        }

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static int _glfwWindowFocusedX11(_GLFWwindow* window)
    {
        return window->x11.focused;
    }

    static int _glfwWindowIconifiedX11(_GLFWwindow* window)
    {
        return window->x11.iconified;
    }

    static int _glfwWindowVisibleX11(_GLFWwindow* window)
    {
        return window->x11.visible;
    }

    static int _glfwWindowMaximizedX11(_GLFWwindow* window)
    {
        return window->x11.maximized;
    }

    static int _glfwWindowHoveredX11(_GLFWwindow* window)
    {
        if (window->x11.handle != 0 && _glfw.x11.XQueryPointer != null)
        {
            nuint root;
            nuint child;
            int rootX;
            int rootY;
            int winX;
            int winY;
            uint mask;

            if (_glfw.x11.XQueryPointer(_glfw.x11.display,
                    window->x11.handle,
                    &root,
                    &child,
                    &rootX,
                    &rootY,
                    &winX,
                    &winY,
                    &mask) != 0)
            {
                return winX >= 0 && winY >= 0 && winX < window->x11.width && winY < window->x11.height
                    ? GLFW_TRUE
                    : GLFW_FALSE;
            }
        }

        return GLFW_FALSE;
    }

    static int _glfwFramebufferTransparentX11(_GLFWwindow* window)
    {
        return window->x11.transparent;
    }

    static float _glfwGetWindowOpacityX11(_GLFWwindow* window)
    {
        return window->x11.opacity;
    }

    static void _glfwSetWindowResizableX11(_GLFWwindow* window, int enabled)
    {
        x11_updateNormalHints(window, window->x11.width, window->x11.height);
    }

    static void _glfwSetWindowDecoratedX11(_GLFWwindow* window, int enabled)
    {
        if (window->x11.handle == 0 ||
            _glfw.x11.MOTIF_WM_HINTS == 0 ||
            _glfw.x11.XChangeProperty == null)
        {
            return;
        }

        var hints = stackalloc nuint[5];
        hints[0] = MWM_HINTS_DECORATIONS;
        hints[1] = 0;
        hints[2] = enabled != 0 ? MWM_DECOR_ALL : 0u;
        hints[3] = 0;
        hints[4] = 0;

        _glfw.x11.XChangeProperty(_glfw.x11.display,
            window->x11.handle,
            _glfw.x11.MOTIF_WM_HINTS,
            _glfw.x11.MOTIF_WM_HINTS,
            32,
            PropModeReplace,
            (byte*)hints,
            5);

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void _glfwSetWindowFloatingX11(_GLFWwindow* window, int enabled)
    {
        if (_glfw.x11.NET_WM_STATE == 0 || _glfw.x11.NET_WM_STATE_ABOVE == 0)
            return;

        x11_sendEventToWM(window,
            _glfw.x11.NET_WM_STATE,
            enabled != 0 ? _NET_WM_STATE_ADD : _NET_WM_STATE_REMOVE,
            (nint)_glfw.x11.NET_WM_STATE_ABOVE,
            0,
            1,
            0);
    }

    static void _glfwSetWindowOpacityX11(_GLFWwindow* window, float opacity)
    {
        window->x11.opacity = opacity;

        if (window->x11.handle == 0 ||
            _glfw.x11.NET_WM_WINDOW_OPACITY == 0 ||
            _glfw.x11.XChangeProperty == null)
        {
            return;
        }

        var value = (nuint)(0xffffffffu * (double)opacity);
        _glfw.x11.XChangeProperty(_glfw.x11.display,
            window->x11.handle,
            _glfw.x11.NET_WM_WINDOW_OPACITY,
            XA_CARDINAL,
            32,
            PropModeReplace,
            (byte*)&value,
            1);

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void _glfwSetWindowMousePassthroughX11(_GLFWwindow* window, int enabled)
    {
        if (window->x11.handle == 0 ||
            _glfw.x11.XShapeCombineMask == null)
        {
            return;
        }

        if (enabled != 0)
        {
            if (_glfw.x11.XCreateRegion == null ||
                _glfw.x11.XDestroyRegion == null ||
                _glfw.x11.XShapeCombineRegion == null)
            {
                return;
            }

            var region = _glfw.x11.XCreateRegion();
            if (region == null)
                return;

            _glfw.x11.XShapeCombineRegion(_glfw.x11.display,
                window->x11.handle,
                ShapeInput,
                0,
                0,
                region,
                ShapeSet);
            _glfw.x11.XDestroyRegion(region);
        }
        else
        {
            _glfw.x11.XShapeCombineMask(_glfw.x11.display,
                window->x11.handle,
                ShapeInput,
                0,
                0,
                0,
                ShapeSet);
        }

        _glfw.x11.XFlush(_glfw.x11.display);
    }

    static void _glfwPollEventsX11()
    {
        while (_glfw.x11.XPending(_glfw.x11.display) != 0)
        {
            XEvent @event;
            _glfw.x11.XNextEvent(_glfw.x11.display, &@event);
            x11_processEvent(&@event);
        }
    }

    static void x11_waitForEvent(double* timeout)
    {
        if (_glfw.x11.XPending(_glfw.x11.display) != 0)
            return;

        if (_glfw.x11.XConnectionNumber != null)
        {
            var fd = _glfw.x11.XConnectionNumber(_glfw.x11.display);
            if (fd >= 0)
            {
                POLLFD fds = default;
                fds.fd = fd;
                fds.events = POLLIN;

                _glfw.x11.XFlush(_glfw.x11.display);
                _glfwPollPOSIX(&fds, 1, timeout);
                return;
            }
        }

        if (timeout == null)
        {
            while (_glfw.x11.XPending(_glfw.x11.display) == 0)
                Thread.Sleep(1);
        }
        else
        {
            var deadline = _glfwPlatformGetTimerValue() + (ulong)(*timeout * _glfwPlatformGetTimerFrequency());
            while (_glfw.x11.XPending(_glfw.x11.display) == 0 &&
                   _glfwPlatformGetTimerValue() < deadline)
            {
                Thread.Sleep(1);
            }
        }
    }

    static void _glfwWaitEventsX11()
    {
        x11_waitForEvent(null);
        _glfwPollEventsX11();
    }

    static void _glfwWaitEventsTimeoutX11(double timeout)
    {
        x11_waitForEvent(&timeout);
        _glfwPollEventsX11();
    }

    static void _glfwPostEmptyEventX11()
    {
        if (_glfw.x11.helperWindowHandle == 0 ||
            _glfw.x11.GLFW_EMPTY_EVENT == 0 ||
            _glfw.x11.XSendEvent == null)
        {
            return;
        }

        XEvent @event = default;
        @event.type = ClientMessage;
        @event.display = _glfw.x11.display;
        @event.anyWindow = _glfw.x11.helperWindowHandle;
        @event.clientMessageType = _glfw.x11.GLFW_EMPTY_EVENT;
        @event.clientFormat = 32;

        _glfw.x11.XSendEvent(_glfw.x11.display,
            _glfw.x11.helperWindowHandle,
            GLFW_FALSE,
            0,
            &@event);
        _glfw.x11.XFlush(_glfw.x11.display);
    }

    public static void glfwSetX11SelectionString(byte* value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return;
        }

        if (value == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return;
        }

        _glfw_free(_glfw.x11.primarySelectionString);
        _glfw.x11.primarySelectionString = _glfw_strdup(value);

        if (_glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.PRIMARY != 0 &&
            _glfw.x11.XSetSelectionOwner != null)
        {
            _glfw.x11.XSetSelectionOwner(_glfw.x11.display,
                _glfw.x11.PRIMARY,
                _glfw.x11.helperWindowHandle,
                CurrentTime);
            _glfw.x11.XFlush(_glfw.x11.display);
        }
    }

    public static byte* glfwGetX11SelectionString()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return null;
        }

        if (_glfw.x11.helperWindowHandle != 0 &&
            _glfw.x11.PRIMARY != 0 &&
            _glfw.x11.XGetSelectionOwner != null)
        {
            var owner = _glfw.x11.XGetSelectionOwner(_glfw.x11.display, _glfw.x11.PRIMARY);
            if (owner != 0 && owner != _glfw.x11.helperWindowHandle)
            {
                var stringValue = x11_convertSelectionToString(_glfw.x11.PRIMARY, _glfw.x11.UTF8_STRING);
                if (stringValue == null)
                    stringValue = x11_convertSelectionToString(_glfw.x11.PRIMARY, XA_STRING);

                if (stringValue != null)
                {
                    _glfw_free(_glfw.x11.primarySelectionString);
                    _glfw.x11.primarySelectionString = stringValue;
                }
            }
        }

        return _glfw.x11.primarySelectionString;
    }

    public static void* glfwGetX11Display()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return null;
        }

        return _glfw.x11.display;
    }

    public static nuint glfwGetX11Window(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return 0;
        }

        return ((_GLFWwindow*)window)->x11.handle;
    }
}
