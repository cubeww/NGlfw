using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static long cocoa_getJoystickElementValue(_GLFWjoystick* js, _GLFWjoyelementNS* element)
    {
        var value = 0L;

        if (js->ns.device != null)
        {
            void* valueRef;
            if (IOHIDDeviceGetValue(js->ns.device, element->native, &valueRef) == 0)
                value = IOHIDValueGetIntegerValue(valueRef);
        }

        return value;
    }

    static byte cocoa_nameByte(byte[] bytes, int index)
    {
        return index < bytes.Length ? bytes[index] : (byte)0;
    }

    static bool cocoa_asciiMatches(byte* value, string expected)
    {
        for (var i = 0; i < expected.Length; i++)
        {
            if (value[i] != (byte)expected[i])
                return false;
        }

        return true;
    }

    static void cocoa_writeAscii(byte* destination, string value)
    {
        for (var i = 0; i < value.Length; i++)
            destination[i] = (byte)value[i];
    }

    static string cocoa_getHIDStringProperty(void* device, string keyName)
    {
        var key = cocoa_stringFromUTF8(keyName);
        if (key == null)
            return "Unknown";

        var property = IOHIDDeviceGetProperty(device, key);
        cocoa_releaseTemporaryString(key);
        if (property == null)
            return "Unknown";

        var buffer = stackalloc byte[256];
        if (CFStringGetCString(property, buffer, 256, kCFStringEncodingUTF8) == 0)
            return "Unknown";

        return Marshal.PtrToStringUTF8((nint)buffer) ?? "Unknown";
    }

    static int cocoa_getHIDIntProperty(void* device, string keyName)
    {
        var key = cocoa_stringFromUTF8(keyName);
        if (key == null)
            return 0;

        var property = IOHIDDeviceGetProperty(device, key);
        cocoa_releaseTemporaryString(key);
        if (property == null)
            return 0;

        var value = 0;
        CFNumberGetValue(property, kCFNumberSInt32Type, &value);
        return value;
    }

    static string cocoa_createJoystickGUID(string name, int vendor, int product, int version)
    {
        if (vendor != 0 && product != 0)
        {
            return $"03000000{vendor & 0xff:x2}{(vendor >> 8) & 0xff:x2}0000" +
                   $"{product & 0xff:x2}{(product >> 8) & 0xff:x2}0000" +
                   $"{version & 0xff:x2}{(version >> 8) & 0xff:x2}0000";
        }

        var bytes = Encoding.UTF8.GetBytes(name ?? "Unknown");
        return "05000000" +
               $"{cocoa_nameByte(bytes, 0):x2}{cocoa_nameByte(bytes, 1):x2}" +
               $"{cocoa_nameByte(bytes, 2):x2}{cocoa_nameByte(bytes, 3):x2}" +
               $"{cocoa_nameByte(bytes, 4):x2}{cocoa_nameByte(bytes, 5):x2}" +
               $"{cocoa_nameByte(bytes, 6):x2}{cocoa_nameByte(bytes, 7):x2}" +
               $"{cocoa_nameByte(bytes, 8):x2}{cocoa_nameByte(bytes, 9):x2}" +
               $"{cocoa_nameByte(bytes, 10):x2}00";
    }

    [UnmanagedCallersOnly]
    static nint cocoa_compareJoystickElements(void* fp, void* sp, void* user)
    {
        var fe = (_GLFWjoyelementNS*)fp;
        var se = (_GLFWjoyelementNS*)sp;

        if (fe->usage < se->usage)
            return -1;
        if (fe->usage > se->usage)
            return 1;
        if (fe->index < se->index)
            return -1;
        if (fe->index > se->index)
            return 1;

        return 0;
    }

    static void* cocoa_targetForJoystickElement(void* axes, void* buttons, void* hats, uint page, uint usage)
    {
        if (page == kHIDPage_GenericDesktop)
        {
            switch (usage)
            {
                case kHIDUsage_GD_X:
                case kHIDUsage_GD_Y:
                case kHIDUsage_GD_Z:
                case kHIDUsage_GD_Rx:
                case kHIDUsage_GD_Ry:
                case kHIDUsage_GD_Rz:
                case kHIDUsage_GD_Slider:
                case kHIDUsage_GD_Dial:
                case kHIDUsage_GD_Wheel:
                    return axes;
                case kHIDUsage_GD_Hatswitch:
                    return hats;
                case kHIDUsage_GD_DPadUp:
                case kHIDUsage_GD_DPadRight:
                case kHIDUsage_GD_DPadDown:
                case kHIDUsage_GD_DPadLeft:
                case kHIDUsage_GD_SystemMainMenu:
                case kHIDUsage_GD_Select:
                case kHIDUsage_GD_Start:
                    return buttons;
            }
        }
        else if (page == kHIDPage_Simulation)
        {
            switch (usage)
            {
                case kHIDUsage_Sim_Accelerator:
                case kHIDUsage_Sim_Brake:
                case kHIDUsage_Sim_Throttle:
                case kHIDUsage_Sim_Rudder:
                case kHIDUsage_Sim_Steering:
                    return axes;
            }
        }
        else if (page == kHIDPage_Button || page == kHIDPage_Consumer)
            return buttons;

        return null;
    }

    static void cocoa_appendJoystickElement(void* target, void* native, uint usage)
    {
        var element = (_GLFWjoyelementNS*)_glfw_calloc(1, (nuint)sizeof(_GLFWjoyelementNS));
        if (element == null)
            return;

        element->native = native;
        element->usage = usage;
        element->index = (int)CFArrayGetCount(target);
        element->minimum = IOHIDElementGetLogicalMin(native);
        element->maximum = IOHIDElementGetLogicalMax(native);

        CFArrayAppendValue(target, element);
    }

    static void cocoa_sortJoystickElements(void* elements)
    {
        CFArraySortValues(elements,
            new CFRange { location = 0, length = CFArrayGetCount(elements) },
            &cocoa_compareJoystickElements,
            null);
    }

    static void cocoa_freeJoystickElements(void* elements)
    {
        if (elements == null)
            return;

        var count = CFArrayGetCount(elements);
        for (nint i = 0; i < count; i++)
            _glfw_free(CFArrayGetValueAtIndex(elements, i));

        CFRelease(elements);
    }

    static void cocoa_closeJoystick(_GLFWjoystick* js)
    {
        if (js == null)
            return;

        _glfwInputJoystick(js, GLFW_DISCONNECTED);

        cocoa_freeJoystickElements(js->ns.axes);
        cocoa_freeJoystickElements(js->ns.buttons);
        cocoa_freeJoystickElements(js->ns.hats);

        _glfwFreeJoystick(js);
    }

    [UnmanagedCallersOnly]
    static void cocoa_joystickMatchCallback(void* context, int result, void* sender, void* device)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                if (glfw->joysticks[jid].ns.device == device)
                    return;
            }
        }

        var elements = IOHIDDeviceCopyMatchingElements(device, null, kIOHIDOptionsTypeNone);
        if (elements == null)
            return;

        var axes = CFArrayCreateMutable(null, 0, null);
        var buttons = CFArrayCreateMutable(null, 0, null);
        var hats = CFArrayCreateMutable(null, 0, null);
        if (axes == null || buttons == null || hats == null)
        {
            cocoa_freeJoystickElements(axes);
            cocoa_freeJoystickElements(buttons);
            cocoa_freeJoystickElements(hats);
            CFRelease(elements);
            return;
        }

        var name = cocoa_getHIDStringProperty(device, "Product");
        var vendor = cocoa_getHIDIntProperty(device, "VendorID");
        var product = cocoa_getHIDIntProperty(device, "ProductID");
        var version = cocoa_getHIDIntProperty(device, "VersionNumber");
        var guid = cocoa_createJoystickGUID(name, vendor, product, version);

        var elementType = IOHIDElementGetTypeID();
        for (nint i = 0; i < CFArrayGetCount(elements); i++)
        {
            var native = CFArrayGetValueAtIndex(elements, i);
            if (CFGetTypeID(native) != elementType)
                continue;

            var type = IOHIDElementGetType(native);
            if (type != kIOHIDElementTypeInput_Axis &&
                type != kIOHIDElementTypeInput_Button &&
                type != kIOHIDElementTypeInput_Misc)
            {
                continue;
            }

            var usage = IOHIDElementGetUsage(native);
            var page = IOHIDElementGetUsagePage(native);
            var target = cocoa_targetForJoystickElement(axes, buttons, hats, page, usage);
            if (target != null)
                cocoa_appendJoystickElement(target, native, usage);
        }

        CFRelease(elements);

        cocoa_sortJoystickElements(axes);
        cocoa_sortJoystickElements(buttons);
        cocoa_sortJoystickElements(hats);

        var js = _glfwAllocJoystick(name,
            guid,
            (int)CFArrayGetCount(axes),
            (int)CFArrayGetCount(buttons),
            (int)CFArrayGetCount(hats));
        if (js == null)
        {
            cocoa_freeJoystickElements(axes);
            cocoa_freeJoystickElements(buttons);
            cocoa_freeJoystickElements(hats);
            return;
        }

        js->ns.device = device;
        js->ns.axes = axes;
        js->ns.buttons = buttons;
        js->ns.hats = hats;

        _glfwInputJoystick(js, GLFW_CONNECTED);
    }

    [UnmanagedCallersOnly]
    static void cocoa_joystickRemoveCallback(void* context, int result, void* sender, void* device)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0 && js->ns.device == device)
                {
                    cocoa_closeJoystick(js);
                    break;
                }
            }
        }
    }

    static void* cocoa_createJoystickMatchingDictionary(void* usagePageKey, void* usageKey, int usage)
    {
        if (usagePageKey == null || usageKey == null)
            return null;

        var dictionary = cocoa_msgSend_id(cocoa_msgSend_id(cocoa_getClass("NSMutableDictionary"), "alloc"), "init");
        if (dictionary == null)
            return null;

        var usagePage = objc_msgSend_id_int(cocoa_getClass("NSNumber"), cocoa_sel("numberWithInt:"), kHIDPage_GenericDesktop);
        var usageValue = objc_msgSend_id_int(cocoa_getClass("NSNumber"), cocoa_sel("numberWithInt:"), usage);
        if (usagePage == null || usageValue == null)
        {
            cocoa_msgSend_void(dictionary, "release");
            return null;
        }

        objc_msgSend_void_ptr_ptr(dictionary,
            cocoa_sel("setObject:forKey:"),
            usagePage,
            usagePageKey);
        objc_msgSend_void_ptr_ptr(dictionary,
            cocoa_sel("setObject:forKey:"),
            usageValue,
            usageKey);

        return dictionary;
    }

    static int _glfwInitJoysticksCocoa()
    {
        _glfw.ns.hidManager = IOHIDManagerCreate(null, kIOHIDOptionsTypeNone);
        if (_glfw.ns.hidManager == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create HID manager");
            return GLFW_FALSE;
        }

        var matching = cocoa_msgSend_id(cocoa_msgSend_id(cocoa_getClass("NSMutableArray"), "alloc"), "init");
        if (matching == null)
        {
            CFRelease(_glfw.ns.hidManager);
            _glfw.ns.hidManager = null;
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create joystick matching array");
            return GLFW_FALSE;
        }

        var usagePageKey = cocoa_stringFromUTF8("DeviceUsagePage");
        var usageKey = cocoa_stringFromUTF8("DeviceUsage");
        int* usages = stackalloc int[3]
        {
            kHIDUsage_GD_Joystick,
            kHIDUsage_GD_GamePad,
            kHIDUsage_GD_MultiAxisController
        };

        for (var i = 0; i < 3; i++)
        {
            var dictionary = cocoa_createJoystickMatchingDictionary(usagePageKey, usageKey, usages[i]);
            if (dictionary == null)
                continue;

            cocoa_msgSend_void_ptr(matching, "addObject:", dictionary);
            cocoa_msgSend_void(dictionary, "release");
        }

        cocoa_releaseTemporaryString(usageKey);
        cocoa_releaseTemporaryString(usagePageKey);

        IOHIDManagerSetDeviceMatchingMultiple(_glfw.ns.hidManager, matching);
        cocoa_msgSend_void(matching, "release");

        IOHIDManagerRegisterDeviceMatchingCallback(_glfw.ns.hidManager,
            &cocoa_joystickMatchCallback,
            null);
        IOHIDManagerRegisterDeviceRemovalCallback(_glfw.ns.hidManager,
            &cocoa_joystickRemoveCallback,
            null);

        var mode = cocoa_getDefaultRunLoopMode();
        IOHIDManagerScheduleWithRunLoop(_glfw.ns.hidManager, CFRunLoopGetMain(), mode);
        IOHIDManagerOpen(_glfw.ns.hidManager, kIOHIDOptionsTypeNone);
        CFRunLoopRunInMode(mode, 0.0, 0);
        cocoa_releaseTemporaryString(mode);

        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksCocoa()
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0)
                    cocoa_closeJoystick(js);
            }
        }

        if (_glfw.ns.hidManager != null)
        {
            CFRelease(_glfw.ns.hidManager);
            _glfw.ns.hidManager = null;
        }
    }

    static int _glfwPollJoystickCocoa(_GLFWjoystick* js, int mode)
    {
        if ((mode & _GLFW_POLL_AXES) != 0)
        {
            for (nint i = 0; i < CFArrayGetCount(js->ns.axes); i++)
            {
                var axis = (_GLFWjoyelementNS*)CFArrayGetValueAtIndex(js->ns.axes, i);
                var raw = cocoa_getJoystickElementValue(js, axis);

                if (raw < axis->minimum)
                    axis->minimum = raw;
                if (raw > axis->maximum)
                    axis->maximum = raw;

                var size = axis->maximum - axis->minimum;
                if (size == 0)
                {
                    _glfwInputJoystickAxis(js, (int)i, 0f);
                }
                else
                {
                    var value = 2f * (raw - axis->minimum) / size - 1f;
                    _glfwInputJoystickAxis(js, (int)i, value);
                }
            }
        }

        if ((mode & _GLFW_POLL_BUTTONS) != 0)
        {
            for (nint i = 0; i < CFArrayGetCount(js->ns.buttons); i++)
            {
                var button = (_GLFWjoyelementNS*)CFArrayGetValueAtIndex(js->ns.buttons, i);
                var value = cocoa_getJoystickElementValue(js, button) - button->minimum;
                var state = value > 0 ? GLFW_PRESS : GLFW_RELEASE;
                _glfwInputJoystickButton(js, (int)i, state);
            }

            int* states = stackalloc int[9]
            {
                GLFW_HAT_UP,
                GLFW_HAT_RIGHT_UP,
                GLFW_HAT_RIGHT,
                GLFW_HAT_RIGHT_DOWN,
                GLFW_HAT_DOWN,
                GLFW_HAT_LEFT_DOWN,
                GLFW_HAT_LEFT,
                GLFW_HAT_LEFT_UP,
                GLFW_HAT_CENTERED
            };

            for (nint i = 0; i < CFArrayGetCount(js->ns.hats); i++)
            {
                var hat = (_GLFWjoyelementNS*)CFArrayGetValueAtIndex(js->ns.hats, i);
                var state = cocoa_getJoystickElementValue(js, hat) - hat->minimum;
                if (state < 0 || state > 8)
                    state = 8;

                _glfwInputJoystickHat(js, (int)i, states[(int)state]);
            }
        }

        return js->connected;
    }

    static byte* _glfwGetMappingNameCocoa()
    {
        return _glfwCocoaMappingName;
    }

    static void _glfwUpdateGamepadGUIDCocoa(byte* guid)
    {
        if (cocoa_asciiMatches(guid + 4, "000000000000") == false ||
            cocoa_asciiMatches(guid + 20, "000000000000") == false)
        {
            return;
        }

        var original = stackalloc byte[33];
        for (var i = 0; i < 33; i++)
            original[i] = guid[i];

        cocoa_writeAscii(guid, "03000000");
        for (var i = 0; i < 4; i++)
            guid[8 + i] = original[i];
        cocoa_writeAscii(guid + 12, "0000");
        for (var i = 0; i < 4; i++)
            guid[16 + i] = original[16 + i];
        cocoa_writeAscii(guid + 20, "000000000000");
        guid[32] = 0;
    }
}
