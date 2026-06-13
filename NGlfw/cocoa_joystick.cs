using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    [UnmanagedCallersOnly]
    static void cocoa_joystickMatchCallback(void* context, int result, void* sender, void* device)
    {
    }

    [UnmanagedCallersOnly]
    static void cocoa_joystickRemoveCallback(void* context, int result, void* sender, void* device)
    {
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
        if (usagePage != null && usageValue != null)
        {
            objc_msgSend_void_ptr_ptr(dictionary,
                cocoa_sel("setObject:forKey:"),
                usagePage,
                usagePageKey);
            objc_msgSend_void_ptr_ptr(dictionary,
                cocoa_sel("setObject:forKey:"),
                usageValue,
                usageKey);
        }

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
        if (_glfw.ns.hidManager != null)
        {
            CFRelease(_glfw.ns.hidManager);
            _glfw.ns.hidManager = null;
        }
    }

    static int _glfwPollJoystickCocoa(_GLFWjoystick* js, int mode)
    {
        return js->connected;
    }

    static byte* _glfwGetMappingNameCocoa()
    {
        return _glfwCocoaMappingName;
    }

    static void _glfwUpdateGamepadGUIDCocoa(byte* guid)
    {
    }
}
