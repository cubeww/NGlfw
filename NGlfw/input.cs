using System;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int _GLFW_STICK = 3;
    const int _GLFW_JOYSTICK_AXIS = 1;
    const int _GLFW_JOYSTICK_BUTTON = 2;
    const int _GLFW_JOYSTICK_HATBIT = 3;
    const int GLFW_MOD_MASK = GLFW_MOD_SHIFT |
                              GLFW_MOD_CONTROL |
                              GLFW_MOD_ALT |
                              GLFW_MOD_SUPER |
                              GLFW_MOD_CAPS_LOCK |
                              GLFW_MOD_NUM_LOCK;

    static void _glfwInputChar(_GLFWwindow* window, uint codepoint, int mods, int plain)
    {
        if (codepoint < 32 || (codepoint > 126 && codepoint < 160))
            return;

        if (window->lockKeyMods == 0)
            mods &= ~(GLFW_MOD_CAPS_LOCK | GLFW_MOD_NUM_LOCK);

        if (window->callbacks.charmods != null)
            window->callbacks.charmods((GLFWwindow*)window, codepoint, mods);

        if (plain != 0 && window->callbacks.character != null)
            window->callbacks.character((GLFWwindow*)window, codepoint);
    }

    static void _glfwInputScroll(_GLFWwindow* window, double xoffset, double yoffset)
    {
        if (window->callbacks.scroll != null)
            window->callbacks.scroll((GLFWwindow*)window, xoffset, yoffset);
    }

    static void _glfwInputCursorPos(_GLFWwindow* window, double xpos, double ypos)
    {
        if (window->virtualCursorPosX == xpos && window->virtualCursorPosY == ypos)
            return;

        window->virtualCursorPosX = xpos;
        window->virtualCursorPosY = ypos;

        if (window->callbacks.cursorPos != null)
            window->callbacks.cursorPos((GLFWwindow*)window, xpos, ypos);
    }

    static void _glfwInputCursorEnter(_GLFWwindow* window, int entered)
    {
        if (window->callbacks.cursorEnter != null)
            window->callbacks.cursorEnter((GLFWwindow*)window, entered);
    }

    static void _glfwInputDrop(_GLFWwindow* window, int count, byte** paths)
    {
        if (window->callbacks.drop != null)
            window->callbacks.drop((GLFWwindow*)window, count, paths);
    }

    static int input_initJoysticks()
    {
        if (_glfw.joysticksInitialized == 0)
        {
            if (_glfw.platform.initJoysticks() == 0)
            {
                _glfw.platform.terminateJoysticks();
                return GLFW_FALSE;
            }
        }

        _glfw.joysticksInitialized = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static int input_strcspn(byte* value, byte reject)
    {
        var length = 0;
        while (value[length] != 0 && value[length] != reject)
            length++;
        return length;
    }

    static int input_strcspn(byte* value, byte rejectA, byte rejectB)
    {
        var length = 0;
        while (value[length] != 0 && value[length] != rejectA && value[length] != rejectB)
            length++;
        return length;
    }

    static int input_strspn(byte* value, byte accept)
    {
        var length = 0;
        while (value[length] == accept)
            length++;
        return length;
    }

    static int input_strspn(byte* value, byte acceptA, byte acceptB)
    {
        var length = 0;
        while (value[length] == acceptA || value[length] == acceptB)
            length++;
        return length;
    }

    static int input_strncmp(byte* a, byte* b, int length)
    {
        for (var i = 0; i < length; i++)
        {
            if (a[i] != b[i])
                return a[i] - b[i];
        }

        return 0;
    }

    static int input_strcmp(byte* a, byte* b)
    {
        var i = 0;
        while (a[i] != 0 && b[i] != 0 && a[i] == b[i])
            i++;
        return a[i] - b[i];
    }

    static int input_isHex(byte c)
    {
        return (c >= '0' && c <= '9') ||
               (c >= 'a' && c <= 'f') ||
               (c >= 'A' && c <= 'F')
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static uint input_parseUInt(byte* value, ref byte* end)
    {
        var c = value;
        var result = 0u;

        while (*c >= '0' && *c <= '9')
        {
            result = result * 10 + (uint)(*c - '0');
            c++;
        }

        end = c;
        return result;
    }

    static int input_matchMappingField(ref byte* c, string name)
    {
        var start = c;

        for (var i = 0; i < name.Length; i++)
        {
            if (start[i] != (byte)name[i])
                return GLFW_FALSE;
        }

        if (start[name.Length] != ':')
            return GLFW_FALSE;

        c = start + name.Length + 1;
        return GLFW_TRUE;
    }

    static _GLFWmapelement* input_getMappingElement(_GLFWmapping* mapping, ref byte* c, out int matched, out int platform)
    {
        matched = GLFW_TRUE;
        platform = GLFW_FALSE;

        if (input_matchMappingField(ref c, "platform") != 0)
        {
            platform = GLFW_TRUE;
            return null;
        }
        if (input_matchMappingField(ref c, "a") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_A];
        if (input_matchMappingField(ref c, "b") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_B];
        if (input_matchMappingField(ref c, "x") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_X];
        if (input_matchMappingField(ref c, "y") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_Y];
        if (input_matchMappingField(ref c, "back") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_BACK];
        if (input_matchMappingField(ref c, "start") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_START];
        if (input_matchMappingField(ref c, "guide") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_GUIDE];
        if (input_matchMappingField(ref c, "leftshoulder") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_LEFT_BUMPER];
        if (input_matchMappingField(ref c, "rightshoulder") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER];
        if (input_matchMappingField(ref c, "leftstick") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_LEFT_THUMB];
        if (input_matchMappingField(ref c, "rightstick") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_RIGHT_THUMB];
        if (input_matchMappingField(ref c, "dpup") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_DPAD_UP];
        if (input_matchMappingField(ref c, "dpright") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_DPAD_RIGHT];
        if (input_matchMappingField(ref c, "dpdown") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_DPAD_DOWN];
        if (input_matchMappingField(ref c, "dpleft") != 0)
            return &mapping->buttons[GLFW_GAMEPAD_BUTTON_DPAD_LEFT];
        if (input_matchMappingField(ref c, "lefttrigger") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_LEFT_TRIGGER];
        if (input_matchMappingField(ref c, "righttrigger") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER];
        if (input_matchMappingField(ref c, "leftx") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_LEFT_X];
        if (input_matchMappingField(ref c, "lefty") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_LEFT_Y];
        if (input_matchMappingField(ref c, "rightx") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_RIGHT_X];
        if (input_matchMappingField(ref c, "righty") != 0)
            return &mapping->axes[GLFW_GAMEPAD_AXIS_RIGHT_Y];

        matched = GLFW_FALSE;
        return null;
    }

    static _GLFWmapping* input_findMapping(byte* guid)
    {
        for (var i = 0; i < _glfw.mappingCount; i++)
        {
            if (input_strcmp(_glfw.mappings[i].guid, guid) == 0)
                return _glfw.mappings + i;
        }

        return null;
    }

    static int input_isValidElementForJoystick(_GLFWmapelement* e, _GLFWjoystick* js)
    {
        if (e->type == _GLFW_JOYSTICK_HATBIT && (e->index >> 4) >= js->hatCount)
            return GLFW_FALSE;
        if (e->type == _GLFW_JOYSTICK_BUTTON && e->index >= js->buttonCount)
            return GLFW_FALSE;
        if (e->type == _GLFW_JOYSTICK_AXIS && e->index >= js->axisCount)
            return GLFW_FALSE;

        return GLFW_TRUE;
    }

    static _GLFWmapping* input_findValidMapping(_GLFWjoystick* js)
    {
        var mapping = input_findMapping(js->guid);
        if (mapping != null)
        {
            for (var i = 0; i <= GLFW_GAMEPAD_BUTTON_LAST; i++)
            {
                if (input_isValidElementForJoystick(&mapping->buttons[i], js) == 0)
                    return null;
            }

            for (var i = 0; i <= GLFW_GAMEPAD_AXIS_LAST; i++)
            {
                if (input_isValidElementForJoystick(&mapping->axes[i], js) == 0)
                    return null;
            }
        }

        return mapping;
    }

    static int input_parseMapping(_GLFWmapping* mapping, byte* value)
    {
        var c = value;
        var length = input_strcspn(c, (byte)',');

        if (length != 32 || c[length] != ',')
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return GLFW_FALSE;
        }

        for (var i = 0; i < length; i++)
            mapping->guid[i] = c[i];
        mapping->guid[length] = 0;
        c += length + 1;

        length = input_strcspn(c, (byte)',');
        if (length >= 128 || c[length] != ',')
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return GLFW_FALSE;
        }

        for (var i = 0; i < length; i++)
            mapping->name[i] = c[i];
        mapping->name[length] = 0;
        c += length + 1;

        while (*c != 0)
        {
            if (*c == '+' || *c == '-')
                return GLFW_FALSE;

            var start = c;
            var e = input_getMappingElement(mapping, ref c, out var matched, out var platform);

            if (matched != 0)
            {
                if (platform != 0)
                {
                    var name = _glfw.platform.getMappingName();
                    if (name == null)
                        return GLFW_FALSE;

                    length = _glfw_strlen(name);
                    if (input_strncmp(c, name, length) != 0)
                        return GLFW_FALSE;
                }
                else if (e != null)
                {
                    sbyte minimum = -1;
                    sbyte maximum = 1;

                    if (*c == '+')
                    {
                        minimum = 0;
                        c++;
                    }
                    else if (*c == '-')
                    {
                        maximum = 0;
                        c++;
                    }

                    if (*c == 'a')
                        e->type = _GLFW_JOYSTICK_AXIS;
                    else if (*c == 'b')
                        e->type = _GLFW_JOYSTICK_BUTTON;
                    else if (*c == 'h')
                        e->type = _GLFW_JOYSTICK_HATBIT;
                    else
                    {
                        c = start;
                        c += input_strcspn(c, (byte)',');
                        c += input_strspn(c, (byte)',');
                        continue;
                    }

                    if (e->type == _GLFW_JOYSTICK_HATBIT)
                    {
                        var hat = input_parseUInt(c + 1, ref c);
                        var bit = input_parseUInt(c + 1, ref c);
                        e->index = (byte)((hat << 4) | bit);
                    }
                    else
                        e->index = (byte)input_parseUInt(c + 1, ref c);

                    if (e->type == _GLFW_JOYSTICK_AXIS)
                    {
                        e->axisScale = (sbyte)(2 / (maximum - minimum));
                        e->axisOffset = (sbyte)-(maximum + minimum);

                        if (*c == '~')
                        {
                            e->axisScale = (sbyte)-e->axisScale;
                            e->axisOffset = (sbyte)-e->axisOffset;
                        }
                    }
                }
            }

            c += input_strcspn(c, (byte)',');
            c += input_strspn(c, (byte)',');
        }

        for (var i = 0; i < 32; i++)
        {
            if (mapping->guid[i] >= 'A' && mapping->guid[i] <= 'F')
                mapping->guid[i] += 'a' - 'A';
        }

        _glfw.platform.updateGamepadGUID(mapping->guid);
        return GLFW_TRUE;
    }

    static int input_parseMapping(_GLFWmapping* mapping, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + '\0');
        fixed (byte* text = bytes)
            return input_parseMapping(mapping, text);
    }

    static void _glfwInitGamepadMappings()
    {
        var count = _glfwDefaultMappings.Length;
        _glfw.mappings = (_GLFWmapping*)_glfw_calloc((nuint)count, (nuint)sizeof(_GLFWmapping));
        if (_glfw.mappings == null)
            return;

        for (var i = 0; i < count; i++)
        {
            if (input_parseMapping(&_glfw.mappings[_glfw.mappingCount], _glfwDefaultMappings[i]) != 0)
                _glfw.mappingCount++;
        }
    }

    static void _glfwInputJoystick(_GLFWjoystick* js, int @event)
    {
        if (@event == GLFW_CONNECTED)
            js->connected = GLFW_TRUE;
        else if (@event == GLFW_DISCONNECTED)
            js->connected = GLFW_FALSE;

        if (_glfw.callbacks.joystick == null)
            return;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                if (&glfw->joysticks[jid] == js)
                {
                    _glfw.callbacks.joystick(jid, @event);
                    return;
                }
            }
        }
    }

    static void _glfwInputJoystickAxis(_GLFWjoystick* js, int axis, float value)
    {
        js->axes[axis] = value;
    }

    static void _glfwInputJoystickButton(_GLFWjoystick* js, int button, int value)
    {
        js->buttons[button] = (byte)value;
    }

    static void _glfwInputJoystickHat(_GLFWjoystick* js, int hat, int value)
    {
        var baseIndex = js->buttonCount + hat * 4;

        js->buttons[baseIndex + 0] = (value & 0x01) != 0 ? (byte)GLFW_PRESS : (byte)GLFW_RELEASE;
        js->buttons[baseIndex + 1] = (value & 0x02) != 0 ? (byte)GLFW_PRESS : (byte)GLFW_RELEASE;
        js->buttons[baseIndex + 2] = (value & 0x04) != 0 ? (byte)GLFW_PRESS : (byte)GLFW_RELEASE;
        js->buttons[baseIndex + 3] = (value & 0x08) != 0 ? (byte)GLFW_PRESS : (byte)GLFW_RELEASE;

        js->hats[hat] = (byte)value;
    }

    static void input_copyString(byte* destination, int capacity, string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);
        var count = Math.Min(bytes.Length, capacity - 1);

        for (var i = 0; i < count; i++)
            destination[i] = bytes[i];
        destination[count] = 0;
    }

    static _GLFWjoystick* _glfwAllocJoystick(string name, string guid, int axisCount, int buttonCount, int hatCount)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var jid = 0;
            for (; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                if (glfw->joysticks[jid].allocated == 0)
                    break;
            }

            if (jid > GLFW_JOYSTICK_LAST)
                return null;

            var js = &glfw->joysticks[jid];
            js->allocated = GLFW_TRUE;
            js->axes = (float*)_glfw_calloc((nuint)axisCount, (nuint)sizeof(float));
            js->buttons = (byte*)_glfw_calloc((nuint)(buttonCount + hatCount * 4), 1);
            js->hats = (byte*)_glfw_calloc((nuint)hatCount, 1);
            js->axisCount = axisCount;
            js->buttonCount = buttonCount;
            js->hatCount = hatCount;

            if ((axisCount != 0 && js->axes == null) ||
                (buttonCount + hatCount * 4 != 0 && js->buttons == null) ||
                (hatCount != 0 && js->hats == null))
            {
                _glfwFreeJoystick(js);
                return null;
            }

            input_copyString(js->name, 128, name);
            input_copyString(js->guid, 33, guid);

            js->mapping = input_findValidMapping(js);
            return js;
        }
    }

    static void _glfwFreeJoystick(_GLFWjoystick* js)
    {
        _glfw_free(js->axes);
        _glfw_free(js->buttons);
        _glfw_free(js->hats);
        *js = default;
    }

    public static int glfwGetInputMode(GLFWwindow* window, int mode)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        var internalWindow = (_GLFWwindow*)window;

        switch (mode)
        {
            case GLFW_CURSOR:
                return internalWindow->cursorMode;
            case GLFW_STICKY_KEYS:
                return internalWindow->stickyKeys;
            case GLFW_STICKY_MOUSE_BUTTONS:
                return internalWindow->stickyMouseButtons;
            case GLFW_LOCK_KEY_MODS:
                return internalWindow->lockKeyMods;
            case GLFW_RAW_MOUSE_MOTION:
                return internalWindow->rawMouseMotion;
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid input mode 0x%08X", mode);
        return 0;
    }

    public static void glfwSetInputMode(GLFWwindow* window, int mode, int value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;

        switch (mode)
        {
            case GLFW_CURSOR:
                if (value != GLFW_CURSOR_NORMAL &&
                    value != GLFW_CURSOR_HIDDEN &&
                    value != GLFW_CURSOR_DISABLED &&
                    value != GLFW_CURSOR_CAPTURED)
                {
                    _glfwInputError(GLFW_INVALID_ENUM, "Invalid cursor mode 0x%08X", value);
                    return;
                }

                if (internalWindow->cursorMode == value)
                    return;

                internalWindow->cursorMode = value;
                _glfw.platform.getCursorPos(internalWindow,
                    &internalWindow->virtualCursorPosX,
                    &internalWindow->virtualCursorPosY);
                _glfw.platform.setCursorMode(internalWindow, value);
                return;

            case GLFW_STICKY_KEYS:
                value = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                if (internalWindow->stickyKeys == value)
                    return;

                if (value == 0)
                {
                    for (var i = 0; i <= GLFW_KEY_LAST; i++)
                    {
                        if (internalWindow->keys[i] == _GLFW_STICK)
                            internalWindow->keys[i] = GLFW_RELEASE;
                    }
                }

                internalWindow->stickyKeys = value;
                return;

            case GLFW_STICKY_MOUSE_BUTTONS:
                value = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                if (internalWindow->stickyMouseButtons == value)
                    return;

                if (value == 0)
                {
                    for (var i = 0; i <= GLFW_MOUSE_BUTTON_LAST; i++)
                    {
                        if (internalWindow->mouseButtons[i] == _GLFW_STICK)
                            internalWindow->mouseButtons[i] = GLFW_RELEASE;
                    }
                }

                internalWindow->stickyMouseButtons = value;
                return;

            case GLFW_LOCK_KEY_MODS:
                internalWindow->lockKeyMods = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                return;

            case GLFW_RAW_MOUSE_MOTION:
                if (_glfw.platform.rawMouseMotionSupported() == 0)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Raw mouse motion is not supported on this system");
                    return;
                }

                value = value != 0 ? GLFW_TRUE : GLFW_FALSE;
                if (internalWindow->rawMouseMotion == value)
                    return;

                internalWindow->rawMouseMotion = value;
                _glfw.platform.setRawMouseMotion(internalWindow, value);
                return;
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid input mode 0x%08X", mode);
    }

    public static int glfwRawMouseMotionSupported()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        return _glfw.platform.rawMouseMotionSupported();
    }

    public static byte* glfwGetKeyName(int key, int scancode)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (key != GLFW_KEY_UNKNOWN)
        {
            if (key < GLFW_KEY_SPACE || key > GLFW_KEY_LAST)
            {
                _glfwInputError(GLFW_INVALID_ENUM, "Invalid key {0}", key);
                return null;
            }

            if (key != GLFW_KEY_KP_EQUAL &&
                (key < GLFW_KEY_KP_0 || key > GLFW_KEY_KP_ADD) &&
                (key < GLFW_KEY_APOSTROPHE || key > GLFW_KEY_WORLD_2))
                return null;

            scancode = _glfw.platform.getKeyScancode(key);
        }

        return _glfw.platform.getScancodeName(scancode);
    }

    public static int glfwGetKeyScancode(int key)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (key < GLFW_KEY_SPACE || key > GLFW_KEY_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid key {0}", key);
            return -1;
        }

        return _glfw.platform.getKeyScancode(key);
    }

    public static int glfwGetKey(GLFWwindow* window, int key)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_RELEASE;
        }

        if (key < GLFW_KEY_SPACE || key > GLFW_KEY_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid key {0}", key);
            return GLFW_RELEASE;
        }

        var internalWindow = (_GLFWwindow*)window;

        if (internalWindow->keys[key] == _GLFW_STICK)
        {
            internalWindow->keys[key] = GLFW_RELEASE;
            return GLFW_PRESS;
        }

        return internalWindow->keys[key];
    }

    public static int glfwGetMouseButton(GLFWwindow* window, int button)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_RELEASE;
        }

        if (button < GLFW_MOUSE_BUTTON_1 || button > GLFW_MOUSE_BUTTON_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid mouse button {0}", button);
            return GLFW_RELEASE;
        }

        var internalWindow = (_GLFWwindow*)window;

        if (internalWindow->mouseButtons[button] == _GLFW_STICK)
        {
            internalWindow->mouseButtons[button] = GLFW_RELEASE;
            return GLFW_PRESS;
        }

        return internalWindow->mouseButtons[button];
    }

    public static void glfwGetCursorPos(GLFWwindow* window, double* xpos, double* ypos)
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

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->cursorMode == GLFW_CURSOR_DISABLED)
        {
            if (xpos != null)
                *xpos = internalWindow->virtualCursorPosX;
            if (ypos != null)
                *ypos = internalWindow->virtualCursorPosY;
        }
        else
            _glfw.platform.getCursorPos(internalWindow, xpos, ypos);
    }

    public static void glfwSetCursorPos(GLFWwindow* window, double xpos, double ypos)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (double.IsNaN(xpos) || double.IsNaN(ypos) ||
            double.IsInfinity(xpos) || double.IsInfinity(ypos))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid cursor position {0} {1}", xpos, ypos);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (_glfw.platform.windowFocused(internalWindow) == 0)
            return;

        if (internalWindow->cursorMode == GLFW_CURSOR_DISABLED)
        {
            internalWindow->virtualCursorPosX = xpos;
            internalWindow->virtualCursorPosY = ypos;
        }
        else
            _glfw.platform.setCursorPos(internalWindow, xpos, ypos);
    }

    public static GLFWcursor* glfwCreateCursor(GLFWimage* image, int xhot, int yhot)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (image == null || image->pixels == null || image->width <= 0 || image->height <= 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid image dimensions for cursor");
            return null;
        }

        var cursor = (_GLFWcursor*)_glfw_calloc(1, (nuint)sizeof(_GLFWcursor));
        if (cursor == null)
            return null;

        cursor->next = _glfw.cursorListHead;
        _glfw.cursorListHead = cursor;

        if (_glfw.platform.createCursor(cursor, image, xhot, yhot) == 0)
        {
            glfwDestroyCursor((GLFWcursor*)cursor);
            return null;
        }

        return (GLFWcursor*)cursor;
    }

    public static GLFWcursor* glfwCreateStandardCursor(int shape)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (shape != GLFW_ARROW_CURSOR &&
            shape != GLFW_IBEAM_CURSOR &&
            shape != GLFW_CROSSHAIR_CURSOR &&
            shape != GLFW_POINTING_HAND_CURSOR &&
            shape != GLFW_RESIZE_EW_CURSOR &&
            shape != GLFW_RESIZE_NS_CURSOR &&
            shape != GLFW_RESIZE_NWSE_CURSOR &&
            shape != GLFW_RESIZE_NESW_CURSOR &&
            shape != GLFW_RESIZE_ALL_CURSOR &&
            shape != GLFW_NOT_ALLOWED_CURSOR)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid standard cursor 0x%08X", shape);
            return null;
        }

        var cursor = (_GLFWcursor*)_glfw_calloc(1, (nuint)sizeof(_GLFWcursor));
        if (cursor == null)
            return null;

        cursor->next = _glfw.cursorListHead;
        _glfw.cursorListHead = cursor;

        if (_glfw.platform.createStandardCursor(cursor, shape) == 0)
        {
            glfwDestroyCursor((GLFWcursor*)cursor);
            return null;
        }

        return (GLFWcursor*)cursor;
    }

    public static void glfwSetCursor(GLFWwindow* window, GLFWcursor* cursor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        var internalCursor = (_GLFWcursor*)cursor;

        internalWindow->cursor = internalCursor;
        _glfw.platform.setCursor(internalWindow, internalCursor);
    }

    public static void glfwPollEvents()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.pollEvents();
    }

    public static void glfwWaitEvents()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.waitEvents();
    }

    public static void glfwWaitEventsTimeout(double timeout)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (double.IsNaN(timeout) || timeout < 0.0 || double.IsInfinity(timeout))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid time {0}", timeout);
            return;
        }

        _glfw.platform.waitEventsTimeout(timeout);
    }

    public static void glfwPostEmptyEvent()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.postEmptyEvent();
    }

    public static int glfwJoystickPresent(int jid)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return GLFW_FALSE;
        }

        if (input_initJoysticks() == 0)
            return GLFW_FALSE;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0)
                return GLFW_FALSE;

            return glfw->platform.pollJoystick(js, _GLFW_POLL_PRESENCE);
        }
    }

    public static float* glfwGetJoystickAxes(int jid, int* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0 || glfw->platform.pollJoystick(js, _GLFW_POLL_AXES) == 0)
                return null;

            if (count != null)
                *count = js->axisCount;
            return js->axes;
        }
    }

    public static byte* glfwGetJoystickButtons(int jid, int* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0 || glfw->platform.pollJoystick(js, _GLFW_POLL_BUTTONS) == 0)
                return null;

            if (count != null)
                *count = glfw->hints.init.hatButtons != 0 ? js->buttonCount + js->hatCount * 4 : js->buttonCount;
            return js->buttons;
        }
    }

    public static byte* glfwGetJoystickHats(int jid, int* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0 || glfw->platform.pollJoystick(js, _GLFW_POLL_BUTTONS) == 0)
                return null;

            if (count != null)
                *count = js->hatCount;
            return js->hats;
        }
    }

    public static byte* glfwGetJoystickName(int jid)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0 || glfw->platform.pollJoystick(js, _GLFW_POLL_PRESENCE) == 0)
                return null;

            return js->name;
        }
    }

    public static byte* glfwGetJoystickGUID(int jid)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->connected == 0 || glfw->platform.pollJoystick(js, _GLFW_POLL_PRESENCE) == 0)
                return null;

            return js->guid;
        }
    }

    public static void glfwSetJoystickUserPointer(int jid, void* pointer)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (js->allocated != 0)
                js->userPointer = pointer;
        }
    }

    public static void* glfwGetJoystickUserPointer(int jid)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (jid < GLFW_JOYSTICK_1 || jid > GLFW_JOYSTICK_LAST)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid joystick ID {0}", jid);
            return null;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            return js->allocated != 0 ? js->userPointer : null;
        }
    }

    public static delegate*<int, int, void> glfwSetJoystickCallback(delegate*<int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (input_initJoysticks() == 0)
            return null;

        var previous = _glfw.callbacks.joystick;
        _glfw.callbacks.joystick = cbfun;
        return previous;
    }

    public static int glfwUpdateGamepadMappings(byte* value)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        if (value == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return GLFW_FALSE;
        }

        var c = value;
        while (*c != 0)
        {
            if (input_isHex(*c) != 0)
            {
                var length = input_strcspn(c, (byte)'\r', (byte)'\n');
                if (length < 1024)
                {
                    var lineBytes = new byte[1024];
                    for (var i = 0; i < length; i++)
                    {
                        lineBytes[i] = c[i];
                    }

                    fixed (byte* line = lineBytes)
                    {
                        var mapping = new _GLFWmapping();
                        if (input_parseMapping(&mapping, line) != 0)
                        {
                            var previous = input_findMapping(mapping.guid);
                            if (previous != null)
                                *previous = mapping;
                            else
                            {
                                var mappings = (_GLFWmapping*)_glfw_realloc(_glfw.mappings,
                                    (nuint)((_glfw.mappingCount + 1) * sizeof(_GLFWmapping)));
                                if (mappings == null)
                                    return GLFW_FALSE;

                                _glfw.mappings = mappings;
                                _glfw.mappings[_glfw.mappingCount] = mapping;
                                _glfw.mappingCount++;
                            }
                        }
                    }
                }

                c += length;
            }
            else
            {
                c += input_strcspn(c, (byte)'\r', (byte)'\n');
                c += input_strspn(c, (byte)'\r', (byte)'\n');
            }
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0)
                    js->mapping = input_findValidMapping(js);
            }
        }

        return GLFW_TRUE;
    }

    public static int glfwJoystickIsGamepad(int jid)
    {
        if (glfwJoystickPresent(jid) == 0)
            return GLFW_FALSE;

        fixed (_GLFWlibrary* glfw = &_glfw)
            return glfw->joysticks[jid].mapping != null ? GLFW_TRUE : GLFW_FALSE;
    }

    public static byte* glfwGetGamepadName(int jid)
    {
        if (glfwJoystickPresent(jid) == 0)
            return null;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            return js->mapping != null ? js->mapping->name : null;
        }
    }

    public static int glfwGetGamepadState(int jid, GLFWgamepadstate* state)
    {
        if (state == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE);
            return GLFW_FALSE;
        }

        *state = default;

        if (glfwJoystickPresent(jid) == 0)
            return GLFW_FALSE;

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var js = &glfw->joysticks[jid];
            if (glfw->platform.pollJoystick(js, _GLFW_POLL_ALL) == 0)
                return GLFW_FALSE;

            if (js->mapping == null)
                return GLFW_FALSE;

            for (var i = 0; i <= GLFW_GAMEPAD_BUTTON_LAST; i++)
            {
                var e = &js->mapping->buttons[i];
                if (e->type == _GLFW_JOYSTICK_AXIS)
                {
                    var buttonValue = js->axes[e->index] * e->axisScale + e->axisOffset;
                    if (e->axisOffset < 0 || (e->axisOffset == 0 && e->axisScale > 0))
                    {
                        if (buttonValue >= 0f)
                            state->buttons[i] = GLFW_PRESS;
                    }
                    else
                    {
                        if (buttonValue <= 0f)
                            state->buttons[i] = GLFW_PRESS;
                    }
                }
                else if (e->type == _GLFW_JOYSTICK_HATBIT)
                {
                    var hat = e->index >> 4;
                    var bit = e->index & 0xf;
                    if ((js->hats[hat] & bit) != 0)
                        state->buttons[i] = GLFW_PRESS;
                }
                else if (e->type == _GLFW_JOYSTICK_BUTTON)
                    state->buttons[i] = js->buttons[e->index];
            }

            for (var i = 0; i <= GLFW_GAMEPAD_AXIS_LAST; i++)
            {
                var e = &js->mapping->axes[i];
                if (e->type == _GLFW_JOYSTICK_AXIS)
                {
                    var axisValue = js->axes[e->index] * e->axisScale + e->axisOffset;
                    state->axes[i] = MathF.Min(MathF.Max(axisValue, -1f), 1f);
                }
                else if (e->type == _GLFW_JOYSTICK_HATBIT)
                {
                    var hat = e->index >> 4;
                    var bit = e->index & 0xf;
                    state->axes[i] = (js->hats[hat] & bit) != 0 ? 1f : -1f;
                }
                else if (e->type == _GLFW_JOYSTICK_BUTTON)
                    state->axes[i] = js->buttons[e->index] * 2f - 1f;
            }

            return GLFW_TRUE;
        }
    }

    public static void glfwSetClipboardString(GLFWwindow* window, byte* value)
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

        _glfw.platform.setClipboardString(value);
    }

    public static byte* glfwGetClipboardString(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return _glfw.platform.getClipboardString();
    }

    public static double glfwGetTime()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0.0;
        }

        return (_glfwPlatformGetTimerValue() - _glfw.timer.offset) /
               (double)_glfwPlatformGetTimerFrequency();
    }

    public static void glfwSetTime(double time)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (double.IsNaN(time) || time < 0.0 || time > 18446744073.0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid time {0}", time);
            return;
        }

        _glfw.timer.offset = _glfwPlatformGetTimerValue() -
                             (ulong)(time * _glfwPlatformGetTimerFrequency());
    }

    public static ulong glfwGetTimerValue()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        return _glfwPlatformGetTimerValue();
    }

    public static ulong glfwGetTimerFrequency()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        return _glfwPlatformGetTimerFrequency();
    }

    public static delegate*<GLFWwindow*, int, int, int, int, void> glfwSetKeyCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, int, int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.key;
        internalWindow->callbacks.key = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, uint, void> glfwSetCharCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, uint, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.character;
        internalWindow->callbacks.character = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, uint, int, void> glfwSetCharModsCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, uint, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.charmods;
        internalWindow->callbacks.charmods = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, int, int, void> glfwSetMouseButtonCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, int, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.mouseButton;
        internalWindow->callbacks.mouseButton = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, double, double, void> glfwSetCursorPosCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, double, double, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.cursorPos;
        internalWindow->callbacks.cursorPos = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, void> glfwSetCursorEnterCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.cursorEnter;
        internalWindow->callbacks.cursorEnter = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, double, double, void> glfwSetScrollCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, double, double, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.scroll;
        internalWindow->callbacks.scroll = cbfun;
        return previous;
    }

    public static delegate*<GLFWwindow*, int, byte**, void> glfwSetDropCallback(
        GLFWwindow* window,
        delegate*<GLFWwindow*, int, byte**, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        var previous = internalWindow->callbacks.drop;
        internalWindow->callbacks.drop = cbfun;
        return previous;
    }
}
