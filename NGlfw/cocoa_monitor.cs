namespace NGlfw;

public static unsafe partial class Glfw
{
    static int cocoa_modeIsGood(void* mode)
    {
        var flags = CGDisplayModeGetIOFlags(mode);

        if ((flags & kDisplayModeValidFlag) == 0 ||
            (flags & kDisplayModeSafeFlag) == 0)
        {
            return GLFW_FALSE;
        }

        if ((flags & kDisplayModeInterlacedFlag) != 0)
            return GLFW_FALSE;
        if ((flags & kDisplayModeStretchedFlag) != 0)
            return GLFW_FALSE;

        return GLFW_TRUE;
    }

    static GLFWvidmode cocoa_vidmodeFromDisplayMode(void* mode, double fallbackRefreshRate)
    {
        var refreshRate = CGDisplayModeGetRefreshRate(mode);
        if (refreshRate == 0.0)
            refreshRate = fallbackRefreshRate;

        return new GLFWvidmode
        {
            width = (int)CGDisplayModeGetWidth(mode),
            height = (int)CGDisplayModeGetHeight(mode),
            redBits = 8,
            greenBits = 8,
            blueBits = 8,
            refreshRate = (int)(refreshRate + 0.5)
        };
    }

    static uint cocoa_beginFadeReservation()
    {
        uint token = 0;
        if (CGAcquireDisplayFadeReservation(5.0, &token) == 0)
        {
            CGDisplayFade(token,
                0.3,
                0.0,
                1.0,
                0.0,
                0.0,
                0.0,
                1);
        }

        return token;
    }

    static void cocoa_endFadeReservation(uint token)
    {
        if (token == 0)
            return;

        CGDisplayFade(token,
            0.5,
            1.0,
            0.0,
            0.0,
            0.0,
            0.0,
            0);
        CGReleaseDisplayFadeReservation(token);
    }

    static void* cocoa_iokitCopyProperty(uint service, string propertyName)
    {
        var property = cocoa_stringFromUTF8(propertyName);
        if (property == null)
            return null;

        var value = IORegistryEntryCreateCFProperty(service, property, null, 0);
        CFRelease(property);
        return value;
    }

    static double cocoa_getFallbackRefreshRate(uint displayID)
    {
        var refreshRate = 60.0;
        var serviceName = cocoa_ascii("IOFramebuffer");

        uint iterator = 0;
        fixed (byte* serviceNamePtr = serviceName)
        {
            var matching = IOServiceMatching(serviceNamePtr);
            if (matching == null ||
                IOServiceGetMatchingServices(0, matching, &iterator) != 0)
            {
                return refreshRate;
            }
        }

        for (;;)
        {
            var service = IOIteratorNext(iterator);
            if (service == 0)
                break;

            var indexRef = cocoa_iokitCopyProperty(service, "IOFramebufferOpenGLIndex");
            if (indexRef == null)
            {
                IOObjectRelease(service);
                continue;
            }

            uint index = 0;
            CFNumberGetValue(indexRef, kCFNumberIntType, &index);
            CFRelease(indexRef);

            if (index >= 32 || CGOpenGLDisplayMaskToDisplayID(1u << (int)index) != displayID)
            {
                IOObjectRelease(service);
                continue;
            }

            var clockRef = cocoa_iokitCopyProperty(service, "IOFBCurrentPixelClock");
            var countRef = cocoa_iokitCopyProperty(service, "IOFBCurrentPixelCount");
            uint clock = 0;
            uint count = 0;

            if (clockRef != null)
            {
                CFNumberGetValue(clockRef, kCFNumberIntType, &clock);
                CFRelease(clockRef);
            }

            if (countRef != null)
            {
                CFNumberGetValue(countRef, kCFNumberIntType, &count);
                CFRelease(countRef);
            }

            if (clock > 0 && count > 0)
                refreshRate = clock / (double)count;

            IOObjectRelease(service);
            break;
        }

        IOObjectRelease(iterator);
        return refreshRate;
    }

    static void* cocoa_dictionaryGetValue(void* dictionary, string keyName)
    {
        if (dictionary == null)
            return null;

        var key = cocoa_stringFromUTF8(keyName);
        if (key == null)
            return null;

        var value = CFDictionaryGetValue(dictionary, key);
        cocoa_releaseTemporaryString(key);
        return value;
    }

    static void* cocoa_dictionaryGetValueIfPresent(void* dictionary, string keyName)
    {
        if (dictionary == null)
            return null;

        var key = cocoa_stringFromUTF8(keyName);
        if (key == null)
            return null;

        void* value = null;
        CFDictionaryGetValueIfPresent(dictionary, key, &value);
        cocoa_releaseTemporaryString(key);
        return value;
    }

    static string? cocoa_stringFromCFString(void* value)
    {
        if (value == null)
            return null;

        var length = CFStringGetLength(value);
        var size = CFStringGetMaximumSizeForEncoding(length, kCFStringEncodingUTF8);
        if (size < 0)
            return null;

        var buffer = (byte*)_glfw_calloc((nuint)size + 1, 1);
        if (buffer == null)
            return null;

        string? result = null;
        if (CFStringGetCString(value, buffer, size + 1, kCFStringEncodingUTF8) != 0)
            result = System.Runtime.InteropServices.Marshal.PtrToStringUTF8((nint)buffer);

        _glfw_free(buffer);
        return result;
    }

    static string? cocoa_getMonitorNameFromIOKit(uint displayID)
    {
        var serviceName = cocoa_ascii("IODisplayConnect");

        uint iterator = 0;
        fixed (byte* serviceNamePtr = serviceName)
        {
            var matching = IOServiceMatching(serviceNamePtr);
            if (matching == null ||
                IOServiceGetMatchingServices(0, matching, &iterator) != 0)
            {
                return null;
            }
        }

        void* info = null;

        for (;;)
        {
            var service = IOIteratorNext(iterator);
            if (service == 0)
                break;

            info = IODisplayCreateInfoDictionary(service, kIODisplayOnlyPreferredName);
            if (info == null)
            {
                IOObjectRelease(service);
                continue;
            }

            var vendorIDRef = cocoa_dictionaryGetValue(info, "DisplayVendorID");
            var productIDRef = cocoa_dictionaryGetValue(info, "DisplayProductID");
            if (vendorIDRef == null || productIDRef == null)
            {
                CFRelease(info);
                info = null;
                IOObjectRelease(service);
                continue;
            }

            uint vendorID = 0;
            uint productID = 0;
            CFNumberGetValue(vendorIDRef, kCFNumberIntType, &vendorID);
            CFNumberGetValue(productIDRef, kCFNumberIntType, &productID);

            IOObjectRelease(service);

            if (CGDisplayVendorNumber(displayID) == vendorID &&
                CGDisplayModelNumber(displayID) == productID)
            {
                break;
            }

            CFRelease(info);
            info = null;
        }

        IOObjectRelease(iterator);

        if (info == null)
            return null;

        string? name = null;
        var names = cocoa_dictionaryGetValue(info, "DisplayProductName");
        var nameRef = cocoa_dictionaryGetValueIfPresent(names, "en_US");
        if (nameRef != null)
            name = cocoa_stringFromCFString(nameRef);

        CFRelease(info);
        return string.IsNullOrEmpty(name) ? null : name;
    }

    static void* cocoa_getScreenForDisplay(uint displayID)
    {
        var screens = cocoa_msgSend_id(cocoa_getClass("NSScreen"), "screens");
        if (screens == null)
            return null;

        var screenCount = (nint)objc_msgSend_ulong(screens, cocoa_sel("count"));
        var screenNumberKey = cocoa_stringFromUTF8("NSScreenNumber");

        for (nint i = 0; i < screenCount; i++)
        {
            var screen = objc_msgSend_id_nint(screens, cocoa_sel("objectAtIndex:"), i);
            if (screen == null)
                continue;

            var description = cocoa_msgSend_id(screen, "deviceDescription");
            if (description == null)
                continue;

            var screenNumber = cocoa_msgSend_id_ptr(description, "objectForKey:", screenNumberKey);
            if (screenNumber == null)
                continue;

            var screenDisplayID = objc_msgSend_uint(screenNumber, cocoa_sel("unsignedIntValue"));
            if (CGDisplayUnitNumber(screenDisplayID) == CGDisplayUnitNumber(displayID))
            {
                cocoa_releaseTemporaryString(screenNumberKey);
                return screen;
            }
        }

        cocoa_releaseTemporaryString(screenNumberKey);
        return null;
    }

    static string cocoa_getMonitorName(uint displayID, void* screen)
    {
        if (screen != null && objc_msgSend_bool_nint(screen, cocoa_sel("respondsToSelector:"), cocoa_sel("localizedName")) != 0)
        {
            var name = cocoa_msgSend_id(screen, "localizedName");
            if (name != null)
            {
                var utf8 = objc_msgSend_UTF8String(name, cocoa_sel("UTF8String"));
                var text = System.Runtime.InteropServices.Marshal.PtrToStringUTF8((nint)utf8);
                if (!string.IsNullOrEmpty(text))
                    return text;
            }
        }

        var fallbackName = cocoa_getMonitorNameFromIOKit(displayID);
        if (!string.IsNullOrEmpty(fallbackName))
            return fallbackName;

        return "Display";
    }

    static void _glfwPollMonitorsCocoa()
    {
        uint displayCount = 0;
        if (CGGetOnlineDisplayList(0, null, &displayCount) != 0)
            return;

        uint* displays = null;
        if (displayCount != 0)
        {
            displays = (uint*)_glfw_calloc(displayCount, sizeof(uint));
            if (displays == null)
                return;

            if (CGGetOnlineDisplayList(displayCount, displays, &displayCount) != 0)
            {
                _glfw_free(displays);
                return;
            }
        }

        for (var i = 0; i < _glfw.monitorCount; i++)
            _glfw.monitors[i]->ns.screen = null;

        var disconnectedCount = _glfw.monitorCount;
        _GLFWmonitor** disconnected = null;
        if (disconnectedCount != 0)
        {
            disconnected = (_GLFWmonitor**)_glfw_calloc((nuint)disconnectedCount, (nuint)sizeof(_GLFWmonitor*));
            if (disconnected != null)
            {
                _glfw_memcpy(disconnected,
                    _glfw.monitors,
                    (nuint)(disconnectedCount * sizeof(_GLFWmonitor*)));
            }
            else
                disconnectedCount = 0;
        }

        for (uint i = 0; i < displayCount; i++)
        {
            if (CGDisplayIsAsleep(displays[i]) != 0)
                continue;

            var unitNumber = CGDisplayUnitNumber(displays[i]);
            var screen = cocoa_getScreenForDisplay(displays[i]);

            var existing = -1;
            for (var j = 0; j < disconnectedCount; j++)
            {
                if (disconnected[j] != null && disconnected[j]->ns.unitNumber == unitNumber)
                {
                    existing = j;
                    break;
                }
            }

            if (existing >= 0)
            {
                disconnected[existing]->ns.screen = screen;
                disconnected[existing] = null;
                continue;
            }

            var size = CGDisplayScreenSize(displays[i]);
            var widthMM = (int)size.width;
            var heightMM = (int)size.height;

            if (widthMM <= 0 || heightMM <= 0)
            {
                var bounds = CGDisplayBounds(displays[i]);
                widthMM = (int)(bounds.size.width * 25.4 / 96.0);
                heightMM = (int)(bounds.size.height * 25.4 / 96.0);
            }

            var monitor = _glfwAllocMonitor(cocoa_getMonitorName(displays[i], screen), widthMM, heightMM);
            if (monitor == null)
                continue;

            monitor->ns.displayID = displays[i];
            monitor->ns.unitNumber = unitNumber;
            monitor->ns.screen = screen;

            var nativeMode = CGDisplayCopyDisplayMode(displays[i]);
            if (nativeMode != null)
            {
                var refreshRate = CGDisplayModeGetRefreshRate(nativeMode);
                if (refreshRate == 0.0)
                    monitor->ns.fallbackRefreshRate = cocoa_getFallbackRefreshRate(displays[i]);

                monitor->currentMode = cocoa_vidmodeFromDisplayMode(nativeMode, monitor->ns.fallbackRefreshRate);
                CGDisplayModeRelease(nativeMode);
            }

            _glfwInputMonitor(monitor, GLFW_CONNECTED, _GLFW_INSERT_LAST);
        }

        for (var i = 0; i < disconnectedCount; i++)
        {
            if (disconnected[i] != null)
                _glfwInputMonitor(disconnected[i], GLFW_DISCONNECTED, 0);
        }

        _glfw_free(disconnected);
        _glfw_free(displays);
    }

    static void _glfwFreeMonitorCocoa(_GLFWmonitor* monitor)
    {
        _glfwRestoreVideoModeCocoa(monitor);
    }

    static void _glfwSetVideoModeCocoa(_GLFWmonitor* monitor, GLFWvidmode* desired)
    {
        GLFWvidmode current;
        if (_glfwGetVideoModeCocoa(monitor, &current) == 0)
            return;

        var best = _glfwChooseVideoMode(monitor, desired);
        if (best == null || _glfwCompareVideoModes(&current, best) == 0)
            return;

        var modes = CGDisplayCopyAllDisplayModes(monitor->ns.displayID, null);
        if (modes == null)
            return;

        var count = CFArrayGetCount(modes);
        void* native = null;

        for (nint i = 0; i < count; i++)
        {
            var displayMode = CFArrayGetValueAtIndex(modes, i);
            if (displayMode == null || cocoa_modeIsGood(displayMode) == 0)
                continue;

            var mode = cocoa_vidmodeFromDisplayMode(displayMode, monitor->ns.fallbackRefreshRate);
            if (_glfwCompareVideoModes(best, &mode) == 0)
            {
                native = displayMode;
                break;
            }
        }

        if (native != null)
        {
            if (monitor->ns.previousMode == null)
                monitor->ns.previousMode = CGDisplayCopyDisplayMode(monitor->ns.displayID);

            var token = cocoa_beginFadeReservation();
            CGDisplaySetDisplayMode(monitor->ns.displayID, native, null);
            cocoa_endFadeReservation(token);
            monitor->currentMode = *best;
        }

        CFRelease(modes);
    }

    static void _glfwRestoreVideoModeCocoa(_GLFWmonitor* monitor)
    {
        if (monitor->ns.previousMode == null)
            return;

        var token = cocoa_beginFadeReservation();
        CGDisplaySetDisplayMode(monitor->ns.displayID, monitor->ns.previousMode, null);
        cocoa_endFadeReservation(token);
        CGDisplayModeRelease(monitor->ns.previousMode);
        monitor->ns.previousMode = null;

        _glfwGetVideoModeCocoa(monitor, &monitor->currentMode);
    }

    static void _glfwGetMonitorPosCocoa(_GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        var bounds = CGDisplayBounds(monitor->ns.displayID);
        if (xpos != null)
            *xpos = (int)bounds.origin.x;
        if (ypos != null)
            *ypos = (int)bounds.origin.y;
    }

    static void _glfwGetMonitorContentScaleCocoa(_GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        if (monitor->ns.screen != null)
        {
            var points = objc_msgSend_rect(monitor->ns.screen, cocoa_sel("frame"));
            var pixels = objc_msgSend_rect_rect(monitor->ns.screen, cocoa_sel("convertRectToBacking:"), points);

            if (points.size.width > 0 && points.size.height > 0)
            {
                if (xscale != null)
                    *xscale = (float)(pixels.size.width / points.size.width);
                if (yscale != null)
                    *yscale = (float)(pixels.size.height / points.size.height);
                return;
            }
        }
        else
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Cannot query content scale without screen");

        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwGetMonitorWorkareaCocoa(_GLFWmonitor* monitor,
                                             int* xpos, int* ypos,
                                             int* width, int* height)
    {
        if (monitor->ns.screen != null)
        {
            var frame = objc_msgSend_rect(monitor->ns.screen, cocoa_sel("visibleFrame"));

            if (xpos != null)
                *xpos = (int)frame.origin.x;
            if (ypos != null)
                *ypos = (int)cocoa_transformY(frame.origin.y + frame.size.height - 1);
            if (width != null)
                *width = (int)frame.size.width;
            if (height != null)
                *height = (int)frame.size.height;
            return;
        }

        _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Cannot query workarea without screen");

        var bounds = CGDisplayBounds(monitor->ns.displayID);

        if (xpos != null)
            *xpos = (int)bounds.origin.x;
        if (ypos != null)
            *ypos = (int)bounds.origin.y;
        if (width != null)
            *width = (int)bounds.size.width;
        if (height != null)
            *height = (int)bounds.size.height;
    }

    static GLFWvidmode* _glfwGetVideoModesCocoa(_GLFWmonitor* monitor, int* found)
    {
        if (found != null)
            *found = 0;

        var modes = CGDisplayCopyAllDisplayModes(monitor->ns.displayID, null);
        if (modes == null)
            return null;

        var nativeCount = CFArrayGetCount(modes);
        var result = (GLFWvidmode*)_glfw_calloc((nuint)nativeCount, (nuint)sizeof(GLFWvidmode));
        if (result == null)
        {
            CFRelease(modes);
            return null;
        }

        var resultCount = 0;

        for (nint i = 0; i < nativeCount; i++)
        {
            var native = CFArrayGetValueAtIndex(modes, i);
            if (native == null || cocoa_modeIsGood(native) == 0)
                continue;

            var mode = cocoa_vidmodeFromDisplayMode(native, monitor->ns.fallbackRefreshRate);
            var duplicate = GLFW_FALSE;

            for (var j = 0; j < resultCount; j++)
            {
                if (_glfwCompareVideoModes(result + j, &mode) == 0)
                {
                    duplicate = GLFW_TRUE;
                    break;
                }
            }

            if (duplicate != 0)
                continue;

            result[resultCount++] = mode;
        }

        CFRelease(modes);

        if (resultCount == 0)
        {
            _glfw_free(result);
            return null;
        }

        if (found != null)
            *found = resultCount;
        return result;
    }

    static int _glfwGetVideoModeCocoa(_GLFWmonitor* monitor, GLFWvidmode* mode)
    {
        var native = CGDisplayCopyDisplayMode(monitor->ns.displayID);
        if (native == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to query display mode");
            return GLFW_FALSE;
        }

        if (mode != null)
            *mode = cocoa_vidmodeFromDisplayMode(native, monitor->ns.fallbackRefreshRate);

        CGDisplayModeRelease(native);
        return GLFW_TRUE;
    }

    static int _glfwGetGammaRampCocoa(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        var size = CGDisplayGammaTableCapacity(monitor->ns.displayID);
        if (size == 0)
            return GLFW_FALSE;

        var values = (float*)_glfw_calloc(size * 3, sizeof(float));
        if (values == null)
            return GLFW_FALSE;

        uint sampleCount = 0;
        CGGetDisplayTransferByTable(monitor->ns.displayID,
            size,
            values,
            values + size,
            values + size * 2,
            &sampleCount);

        _glfwAllocGammaArrays(ramp, sampleCount);

        for (uint i = 0; i < sampleCount; i++)
        {
            ramp->red[i] = (ushort)(values[i] * 65535f);
            ramp->green[i] = (ushort)(values[i + sampleCount] * 65535f);
            ramp->blue[i] = (ushort)(values[i + sampleCount * 2] * 65535f);
        }

        _glfw_free(values);
        return GLFW_TRUE;
    }

    static void _glfwSetGammaRampCocoa(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        var values = (float*)_glfw_calloc(ramp->size * 3, sizeof(float));
        if (values == null)
            return;

        for (uint i = 0; i < ramp->size; i++)
        {
            values[i] = ramp->red[i] / 65535f;
            values[i + ramp->size] = ramp->green[i] / 65535f;
            values[i + ramp->size * 2] = ramp->blue[i] / 65535f;
        }

        CGSetDisplayTransferByTable(monitor->ns.displayID,
            ramp->size,
            values,
            values + ramp->size,
            values + ramp->size * 2);

        _glfw_free(values);
    }

    public static uint glfwGetCocoaMonitor(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_COCOA)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not initialized");
            return 0;
        }

        return ((_GLFWmonitor*)monitor)->ns.displayID;
    }
}
