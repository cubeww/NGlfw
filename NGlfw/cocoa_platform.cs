using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const ulong NSWindowStyleMaskBorderless = 0;
    const ulong NSWindowStyleMaskTitled = 1 << 0;
    const ulong NSWindowStyleMaskClosable = 1 << 1;
    const ulong NSWindowStyleMaskMiniaturizable = 1 << 2;
    const ulong NSWindowStyleMaskResizable = 1 << 3;
    const ulong NSWindowStyleMaskFullScreen = 1 << 14;
    const long NSBackingStoreBuffered = 2;
    const long NSNormalWindowLevel = 0;
    const long NSFloatingWindowLevel = 3;
    const long NSMainMenuWindowLevel = 24;
    const uint kDisplayModeValidFlag = 0x00000001;
    const uint kDisplayModeSafeFlag = 0x00000002;
    const uint kDisplayModeInterlacedFlag = 0x00000008;
    const uint kDisplayModeStretchedFlag = 0x00000010;
    const ulong NSWindowCollectionBehaviorManaged = 1 << 2;
    const ulong NSWindowCollectionBehaviorFullScreenPrimary = 1 << 7;
    const ulong NSWindowCollectionBehaviorFullScreenNone = 1 << 9;
    const ulong NSEventModifierFlagCapsLock = 1 << 16;
    const ulong NSEventModifierFlagShift = 1 << 17;
    const ulong NSEventModifierFlagControl = 1 << 18;
    const ulong NSEventModifierFlagOption = 1 << 19;
    const ulong NSEventModifierFlagCommand = 1 << 20;
    const ulong NSEventModifierFlagDeviceIndependentFlagsMask = 0xffff0000;
    const ulong NSEventMaskKeyUp = 1 << 11;
    const ulong NSEventMaskAny = ulong.MaxValue;
    const long NSEventTypeApplicationDefined = 15;
    const ulong NSBitmapFormatAlphaNonpremultiplied = 1;
    const ulong NSDragOperationGeneric = 4;
    const long NSApplicationActivationPolicyRegular = 0;
    const int kCGEventSourceStateHIDSystemState = 1;
    const ulong NSTrackingMouseEnteredAndExited = 1;
    const ulong NSTrackingCursorUpdate = 1 << 2;
    const ulong NSTrackingActiveInKeyWindow = 1 << 5;
    const ulong NSTrackingAssumeInside = 1 << 8;
    const ulong NSTrackingInVisibleRect = 1 << 9;
    const ulong NSTrackingEnabledDuringMouseDrag = 1 << 10;
    const ulong NSWindowOcclusionStateVisible = 1 << 1;
    const int kCFNumberIntType = 9;
    const int kCFNumberSInt32Type = 3;
    const int kIOHIDOptionsTypeNone = 0;
    const int kIOHIDElementTypeInput_Misc = 1;
    const int kIOHIDElementTypeInput_Button = 2;
    const int kIOHIDElementTypeInput_Axis = 3;
    const int kHIDPage_GenericDesktop = 0x01;
    const int kHIDPage_Simulation = 0x02;
    const int kHIDPage_Button = 0x09;
    const int kHIDPage_Consumer = 0x0c;
    const int kHIDUsage_GD_Joystick = 0x04;
    const int kHIDUsage_GD_GamePad = 0x05;
    const int kHIDUsage_GD_MultiAxisController = 0x08;
    const int kHIDUsage_GD_X = 0x30;
    const int kHIDUsage_GD_Y = 0x31;
    const int kHIDUsage_GD_Z = 0x32;
    const int kHIDUsage_GD_Rx = 0x33;
    const int kHIDUsage_GD_Ry = 0x34;
    const int kHIDUsage_GD_Rz = 0x35;
    const int kHIDUsage_GD_Slider = 0x36;
    const int kHIDUsage_GD_Dial = 0x37;
    const int kHIDUsage_GD_Wheel = 0x38;
    const int kHIDUsage_GD_Hatswitch = 0x39;
    const int kHIDUsage_GD_Start = 0x3d;
    const int kHIDUsage_GD_Select = 0x3e;
    const int kHIDUsage_GD_SystemMainMenu = 0x85;
    const int kHIDUsage_GD_DPadUp = 0x90;
    const int kHIDUsage_GD_DPadDown = 0x91;
    const int kHIDUsage_GD_DPadRight = 0x92;
    const int kHIDUsage_GD_DPadLeft = 0x93;
    const int kHIDUsage_Sim_Rudder = 0xba;
    const int kHIDUsage_Sim_Throttle = 0xbb;
    const int kHIDUsage_Sim_Accelerator = 0xc4;
    const int kHIDUsage_Sim_Brake = 0xc5;
    const int kHIDUsage_Sim_Steering = 0xc8;
    const int kUCKeyActionDisplay = 3;
    const uint kUCKeyTranslateNoDeadKeysBit = 0;
    const uint kCFStringEncodingUTF8 = 0x08000100;
    const uint kIODisplayOnlyPreferredName = 0x00000100;
    const int _GLFW_COCOA_KEYNAME_LENGTH = 17;
    const int BLOCK_IS_GLOBAL = 1 << 28;
    const int BLOCK_HAS_SIGNATURE = 1 << 30;

    static readonly byte* _glfwCocoaMappingName = _glfw_allocate_static_string("Mac OS X");
    static readonly byte* _glfwCocoaPasteboardTypeString = _glfw_allocate_static_string("public.utf8-plain-text");
    static readonly byte* _glfwCocoaPasteboardTypeURL = _glfw_allocate_static_string("public.url");
    static readonly byte* _glfwCocoaRunLoopDefaultMode = _glfw_allocate_static_string("kCFRunLoopDefaultMode");
    static readonly byte* _glfwCocoaOpenGLBundleID = _glfw_allocate_static_string("com.apple.opengl");
    static readonly byte* _glfwCocoaHIToolboxBundleID = _glfw_allocate_static_string("com.apple.HIToolbox");
    static readonly byte* _glfwCocoaVulkanLoaderName = _glfw_allocate_static_string("libvulkan.1.dylib");
    static readonly byte* _glfwCocoaEventBlockSignature = _glfw_allocate_static_string("@?@");
    static readonly object cocoaObjectMapLock = new();
    static readonly Dictionary<nint, nint> cocoaObjectWindows = new();

    public struct _GLFWlibraryNS
    {
        public void* autoreleasePool;
        public void* app;
        public void* delegateObject;
        public void* helper;
        public void* helperClass;
        public void* eventSource;
        public void* hidManager;
        public void* cursor;
        public void* keyUpMonitor;
        public void* keyUpMonitorBlock;
        public void* keyUpMonitorBlockDescriptor;
        public void* nibObjects;
        public void* windowClass;
        public void* contentViewClass;
        public void* windowDelegateClass;
        public void* applicationDelegateClass;
        public byte* clipboardString;
        public _GLFWwindow* disabledCursorWindow;
        public int cursorHidden;
        public double restoreCursorPosX;
        public double restoreCursorPosY;
        public void* inputSource;
        public void* unicodeData;
        public NSPoint cascadePoint;
        public _GLFWlibraryNS_tis tis;
        public fixed short keycodes[256];
        public fixed short scancodes[GLFW_KEY_LAST + 1];
        public fixed byte keynames[(GLFW_KEY_LAST + 1) * _GLFW_COCOA_KEYNAME_LENGTH];
    }

    public struct _GLFWlibraryNS_tis
    {
        public void* bundle;
        public void* kPropertyUnicodeKeyLayoutData;
        public delegate* unmanaged<void*> CopyCurrentKeyboardLayoutInputSource;
        public delegate* unmanaged<void*, void*, void*> GetInputSourceProperty;
        public delegate* unmanaged<byte> GetKbdType;
        public delegate* unmanaged<byte*, ushort, ushort, uint, uint, uint, uint*, uint, uint*, ushort*, int> UCKeyTranslate;
    }

    public struct _GLFWwindowNS
    {
        public void* @object;
        public void* delegateObject;
        public void* view;
        public void* layer;
        public void* markedText;
        public void* trackingArea;
        public int maximized;
        public int iconified;
        public int visible;
        public int focused;
        public int hovered;
        public int occluded;
        public int retina;
        public int transparent;
        public int width;
        public int height;
        public int fbWidth;
        public int fbHeight;
        public int xpos;
        public int ypos;
        public float xscale;
        public float yscale;
        public float opacity;
        public double cursorWarpDeltaX;
        public double cursorWarpDeltaY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSPoint
    {
        public double x;
        public double y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSSize
    {
        public double width;
        public double height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSRect
    {
        public NSPoint origin;
        public NSSize size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NSRange
    {
        public nuint location;
        public nuint length;
    }

    public struct CFRange
    {
        public nint location;
        public nint length;
    }

    public struct ObjCBlockDescriptor
    {
        public nuint reserved;
        public nuint size;
        public void* signature;
    }

    public struct ObjCBlockLiteral
    {
        public void* isa;
        public int flags;
        public int reserved;
        public delegate* unmanaged<void*, void*, void*> invoke;
        public ObjCBlockDescriptor* descriptor;
    }

    public struct _GLFWmonitorNS
    {
        public uint displayID;
        public uint unitNumber;
        public void* screen;
        public void* previousMode;
        public double fallbackRefreshRate;
    }

    public struct _GLFWcursorNS
    {
        public void* @object;
    }

    public struct _GLFWjoystickNS
    {
        public void* device;
        public void* axes;
        public void* buttons;
        public void* hats;
    }

    public struct _GLFWjoyelementNS
    {
        public void* native;
        public uint usage;
        public int index;
        public long minimum;
        public long maximum;
    }

    public struct _GLFWlibraryNSGL
    {
        public void* framework;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VkMacOSSurfaceCreateInfoMVK
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* pView;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VkMetalSurfaceCreateInfoEXT
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* pLayer;
    }

    static byte[] cocoa_ascii(string value)
    {
        return Encoding.ASCII.GetBytes(value + '\0');
    }

    static void* cocoa_getClass(string name)
    {
        var bytes = cocoa_ascii(name);
        fixed (byte* p = bytes)
            return objc_getClass(p);
    }

    static void* cocoa_lookUpClass(string name)
    {
        var bytes = cocoa_ascii(name);
        fixed (byte* p = bytes)
            return objc_lookUpClass(p);
    }

    static nint cocoa_sel(string name)
    {
        var bytes = cocoa_ascii(name);
        fixed (byte* p = bytes)
            return sel_registerName(p);
    }

    static void* cocoa_getProtocol(string name)
    {
        var bytes = cocoa_ascii(name);
        fixed (byte* p = bytes)
            return objc_getProtocol(p);
    }

    static void* cocoa_msgSend_id(void* receiver, string selector)
    {
        return receiver != null ? objc_msgSend_id(receiver, cocoa_sel(selector)) : null;
    }

    static void cocoa_msgSend_void(void* receiver, string selector)
    {
        if (receiver != null)
            objc_msgSend_void(receiver, cocoa_sel(selector));
    }

    static void cocoa_msgSend_void_bool(void* receiver, string selector, int value)
    {
        if (receiver != null)
            objc_msgSend_void_bool(receiver, cocoa_sel(selector), (byte)(value != 0 ? 1 : 0));
    }

    static void cocoa_msgSend_void_ptr(void* receiver, string selector, void* value)
    {
        if (receiver != null)
            objc_msgSend_void_ptr(receiver, cocoa_sel(selector), value);
    }

    static void* cocoa_msgSend_id_bool(void* receiver, string selector, int value)
    {
        return receiver != null ? objc_msgSend_id_bool(receiver, cocoa_sel(selector), (byte)(value != 0 ? 1 : 0)) : null;
    }

    static void* cocoa_msgSend_id_ptr(void* receiver, string selector, void* value)
    {
        return receiver != null ? objc_msgSend_id_ptr(receiver, cocoa_sel(selector), value) : null;
    }

    static int cocoa_translateFlags(ulong flags)
    {
        var mods = 0;

        if ((flags & NSEventModifierFlagShift) != 0)
            mods |= GLFW_MOD_SHIFT;
        if ((flags & NSEventModifierFlagControl) != 0)
            mods |= GLFW_MOD_CONTROL;
        if ((flags & NSEventModifierFlagOption) != 0)
            mods |= GLFW_MOD_ALT;
        if ((flags & NSEventModifierFlagCommand) != 0)
            mods |= GLFW_MOD_SUPER;
        if ((flags & NSEventModifierFlagCapsLock) != 0)
            mods |= GLFW_MOD_CAPS_LOCK;

        return mods;
    }

    static int cocoa_translateKey(uint scancode)
    {
        if (scancode > 0xff)
            return GLFW_KEY_UNKNOWN;

        fixed (short* keycodes = _glfw.ns.keycodes)
            return keycodes[scancode];
    }

    static ulong cocoa_translateKeyToModifierFlag(int key)
    {
        return key switch
        {
            GLFW_KEY_LEFT_SHIFT or GLFW_KEY_RIGHT_SHIFT => NSEventModifierFlagShift,
            GLFW_KEY_LEFT_CONTROL or GLFW_KEY_RIGHT_CONTROL => NSEventModifierFlagControl,
            GLFW_KEY_LEFT_ALT or GLFW_KEY_RIGHT_ALT => NSEventModifierFlagOption,
            GLFW_KEY_LEFT_SUPER or GLFW_KEY_RIGHT_SUPER => NSEventModifierFlagCommand,
            GLFW_KEY_CAPS_LOCK => NSEventModifierFlagCapsLock,
            _ => 0
        };
    }

    static void cocoa_setObjectWindow(void* @object, _GLFWwindow* window)
    {
        if (@object == null)
            return;

        lock (cocoaObjectMapLock)
            cocoaObjectWindows[(nint)@object] = (nint)window;
    }

    static void cocoa_clearObjectWindow(void* @object)
    {
        if (@object == null)
            return;

        lock (cocoaObjectMapLock)
            cocoaObjectWindows.Remove((nint)@object);
    }

    static _GLFWwindow* cocoa_getObjectWindow(void* @object)
    {
        if (@object == null)
            return null;

        lock (cocoaObjectMapLock)
            return cocoaObjectWindows.TryGetValue((nint)@object, out var window) ? (_GLFWwindow*)window : null;
    }

    static void* cocoa_getNSApp()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.app != null)
            return _glfw.ns.app;

        var appClass = cocoa_getClass("NSApplication");
        _glfw.ns.app = cocoa_msgSend_id(appClass, "sharedApplication");
        return _glfw.ns.app;
    }

    static void* cocoa_createAutoreleasePool()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        var poolClass = cocoa_getClass("NSAutoreleasePool");
        var pool = cocoa_msgSend_id(poolClass, "alloc");
        return cocoa_msgSend_id(pool, "init");
    }

    static void cocoa_drainAutoreleasePool(void* pool)
    {
        if (pool != null)
            cocoa_msgSend_void(pool, "drain");
    }

    static void* cocoa_stringFromUTF8(byte* value)
    {
        if (value == null)
            return null;

        var stringClass = cocoa_getClass("NSString");
        var allocated = cocoa_msgSend_id(stringClass, "alloc");
        return allocated != null ? objc_msgSend_id_ptr(allocated, cocoa_sel("initWithUTF8String:"), value) : null;
    }

    static void* cocoa_stringFromUTF8(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + '\0');
        fixed (byte* p = bytes)
            return cocoa_stringFromUTF8(p);
    }

    static NSRect cocoa_makeRect(double x, double y, double width, double height)
    {
        return new NSRect
        {
            origin = new NSPoint { x = x, y = y },
            size = new NSSize { width = width, height = height }
        };
    }

    static NSSize cocoa_makeSize(double width, double height)
    {
        return new NSSize { width = width, height = height };
    }

    static void* cocoa_getDefaultRunLoopMode()
    {
        return cocoa_stringFromUTF8(_glfwCocoaRunLoopDefaultMode);
    }

    static void cocoa_releaseTemporaryString(void* value)
    {
        cocoa_msgSend_void(value, "release");
    }

    static void cocoa_hideCursor()
    {
        if (_glfw.ns.cursorHidden != 0)
            return;

        cocoa_msgSend_void(cocoa_getClass("NSCursor"), "hide");
        _glfw.ns.cursorHidden = GLFW_TRUE;
    }

    static void cocoa_showCursor()
    {
        if (_glfw.ns.cursorHidden == 0)
            return;

        cocoa_msgSend_void(cocoa_getClass("NSCursor"), "unhide");
        _glfw.ns.cursorHidden = GLFW_FALSE;
    }

    static void cocoa_updateCursorImage(_GLFWwindow* window)
    {
        if (window == null)
            return;

        if (window->cursorMode == GLFW_CURSOR_NORMAL)
        {
            cocoa_showCursor();

            if (window->cursor != null && window->cursor->ns.@object != null)
                cocoa_msgSend_void(window->cursor->ns.@object, "set");
            else
                cocoa_msgSend_void(cocoa_msgSend_id(cocoa_getClass("NSCursor"), "arrowCursor"), "set");
        }
        else
            cocoa_hideCursor();
    }

    static int cocoa_cursorInContentArea(_GLFWwindow* window)
    {
        if (window == null || window->ns.@object == null || window->ns.view == null)
            return GLFW_FALSE;

        var pos = objc_msgSend_point(window->ns.@object, cocoa_sel("mouseLocationOutsideOfEventStream"));
        var rect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
        return objc_msgSend_bool_point_rect(window->ns.view, cocoa_sel("mouse:inRect:"), pos, rect) != 0
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static NSRange cocoa_emptyRange()
    {
        return new NSRange { location = nuint.MaxValue, length = 0 };
    }

    static void* cocoa_registerWindowClass()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.windowClass != null)
            return _glfw.ns.windowClass;

        var existing = cocoa_lookUpClass("GLFWWindow");
        if (existing != null)
        {
            _glfw.ns.windowClass = existing;
            return existing;
        }

        var superclass = cocoa_getClass("NSWindow");
        var name = cocoa_ascii("GLFWWindow");
        fixed (byte* namePtr = name)
        {
            var cls = objc_allocateClassPair(superclass, namePtr, 0);
            if (cls == null)
                return null;

            var types = cocoa_ascii("c@:");
            fixed (byte* typesPtr = types)
            {
                class_addMethod(cls,
                    cocoa_sel("canBecomeKeyWindow"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_canBecomeKeyWindow,
                    typesPtr);
                class_addMethod(cls,
                    cocoa_sel("canBecomeMainWindow"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_canBecomeMainWindow,
                    typesPtr);
            }

            objc_registerClassPair(cls);
            _glfw.ns.windowClass = cls;
            return cls;
        }
    }

    static void* cocoa_registerWindowDelegateClass()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.windowDelegateClass != null)
            return _glfw.ns.windowDelegateClass;

        var existing = cocoa_lookUpClass("GLFWWindowDelegate");
        if (existing != null)
        {
            _glfw.ns.windowDelegateClass = existing;
            return existing;
        }

        var superclass = cocoa_getClass("NSObject");
        var name = cocoa_ascii("GLFWWindowDelegate");
        fixed (byte* namePtr = name)
        {
            var cls = objc_allocateClassPair(superclass, namePtr, 0);
            if (cls == null)
                return null;

            var boolTypes = cocoa_ascii("c@:@");
            var voidTypes = cocoa_ascii("v@:@");
            fixed (byte* boolTypesPtr = boolTypes)
            fixed (byte* voidTypesPtr = voidTypes)
            {
                class_addMethod(cls, cocoa_sel("windowShouldClose:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, byte>)&cocoa_windowShouldClose,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidResize:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidResize,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidMove:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidMove,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidMiniaturize:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidMiniaturize,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidDeminiaturize:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidDeminiaturize,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidBecomeKey:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidBecomeKey,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidResignKey:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidResignKey,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("windowDidChangeOcclusionState:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_windowDidChangeOcclusionState,
                    voidTypesPtr);
            }

            objc_registerClassPair(cls);
            _glfw.ns.windowDelegateClass = cls;
            return cls;
        }
    }

    static void* cocoa_registerHelperClass()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.helperClass != null)
            return _glfw.ns.helperClass;

        var existing = cocoa_lookUpClass("GLFWHelper");
        if (existing != null)
        {
            _glfw.ns.helperClass = existing;
            return existing;
        }

        var superclass = cocoa_getClass("NSObject");
        var name = cocoa_ascii("GLFWHelper");
        fixed (byte* namePtr = name)
        {
            var cls = objc_allocateClassPair(superclass, namePtr, 0);
            if (cls == null)
                return null;

            var voidTypes = cocoa_ascii("v@:@");
            fixed (byte* voidTypesPtr = voidTypes)
            {
                class_addMethod(cls, cocoa_sel("selectedKeyboardInputSourceChanged:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_selectedKeyboardInputSourceChanged,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("doNothing:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_doNothing,
                    voidTypesPtr);
            }

            objc_registerClassPair(cls);
            _glfw.ns.helperClass = cls;
            return cls;
        }
    }

    static void* cocoa_registerApplicationDelegateClass()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.applicationDelegateClass != null)
            return _glfw.ns.applicationDelegateClass;

        var existing = cocoa_lookUpClass("GLFWApplicationDelegate");
        if (existing != null)
        {
            _glfw.ns.applicationDelegateClass = existing;
            return existing;
        }

        var superclass = cocoa_getClass("NSObject");
        var name = cocoa_ascii("GLFWApplicationDelegate");
        fixed (byte* namePtr = name)
        {
            var cls = objc_allocateClassPair(superclass, namePtr, 0);
            if (cls == null)
                return null;

            var applicationDelegateProtocol = cocoa_getProtocol("NSApplicationDelegate");
            if (applicationDelegateProtocol != null)
                class_addProtocol(cls, applicationDelegateProtocol);

            var terminateTypes = cocoa_ascii("q@:@");
            var voidTypes = cocoa_ascii("v@:@");
            fixed (byte* terminateTypesPtr = terminateTypes)
            fixed (byte* voidTypesPtr = voidTypes)
            {
                class_addMethod(cls, cocoa_sel("applicationShouldTerminate:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, long>)&cocoa_applicationShouldTerminate,
                    terminateTypesPtr);
                class_addMethod(cls, cocoa_sel("applicationDidChangeScreenParameters:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_applicationDidChangeScreenParameters,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("applicationWillFinishLaunching:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_applicationWillFinishLaunching,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("applicationDidFinishLaunching:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_applicationDidFinishLaunching,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("applicationDidHide:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_applicationDidHide,
                    voidTypesPtr);
            }

            objc_registerClassPair(cls);
            _glfw.ns.applicationDelegateClass = cls;
            return cls;
        }
    }

    static void* cocoa_registerContentViewClass()
    {
        if (!OperatingSystem.IsMacOS())
            return null;

        if (_glfw.ns.contentViewClass != null)
            return _glfw.ns.contentViewClass;

        var existing = cocoa_lookUpClass("GLFWContentView");
        if (existing != null)
        {
            _glfw.ns.contentViewClass = existing;
            return existing;
        }

        var superclass = cocoa_getClass("NSView");
        var name = cocoa_ascii("GLFWContentView");
        fixed (byte* namePtr = name)
        {
            var cls = objc_allocateClassPair(superclass, namePtr, 0);
            if (cls == null)
                return null;

            var textInputClientProtocol = cocoa_getProtocol("NSTextInputClient");
            if (textInputClientProtocol != null)
                class_addProtocol(cls, textInputClientProtocol);

            var boolTypes = cocoa_ascii("c@:");
            var eventBoolTypes = cocoa_ascii("c@:@");
            var dragOperationTypes = cocoa_ascii("Q@:@");
            var voidTypes = cocoa_ascii("v@:");
            var eventVoidTypes = cocoa_ascii("v@:@");
            var drawRectTypes = cocoa_ascii("v@:{CGRect={CGPoint=dd}{CGSize=dd}}");
            var rangeTypes = cocoa_ascii("{_NSRange=QQ}@:");
            var markedTextTypes = cocoa_ascii("v@:@{_NSRange=QQ}{_NSRange=QQ}");
            var objectTypes = cocoa_ascii("@@:");
            var substringTypes = cocoa_ascii("@@:{_NSRange=QQ}^{_NSRange=QQ}");
            var indexForPointTypes = cocoa_ascii("Q@:{CGPoint=dd}");
            var rectForRangeTypes = cocoa_ascii("{CGRect={CGPoint=dd}{CGSize=dd}}@:{_NSRange=QQ}^{_NSRange=QQ}");
            var insertTextTypes = cocoa_ascii("v@:@{_NSRange=QQ}");
            var selectorVoidTypes = cocoa_ascii("v@::");
            fixed (byte* boolTypesPtr = boolTypes)
            fixed (byte* eventBoolTypesPtr = eventBoolTypes)
            fixed (byte* dragOperationTypesPtr = dragOperationTypes)
            fixed (byte* voidTypesPtr = voidTypes)
            fixed (byte* eventVoidTypesPtr = eventVoidTypes)
            fixed (byte* drawRectTypesPtr = drawRectTypes)
            fixed (byte* rangeTypesPtr = rangeTypes)
            fixed (byte* markedTextTypesPtr = markedTextTypes)
            fixed (byte* objectTypesPtr = objectTypes)
            fixed (byte* substringTypesPtr = substringTypes)
            fixed (byte* indexForPointTypesPtr = indexForPointTypes)
            fixed (byte* rectForRangeTypesPtr = rectForRangeTypes)
            fixed (byte* insertTextTypesPtr = insertTextTypes)
            fixed (byte* selectorVoidTypesPtr = selectorVoidTypes)
            {
                class_addMethod(cls, cocoa_sel("isOpaque"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewIsOpaque,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("canBecomeKeyView"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewYes,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("acceptsFirstResponder"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewYes,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("acceptsFirstMouse:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, byte>)&cocoa_viewAcceptsFirstMouse,
                    eventBoolTypesPtr);
                class_addMethod(cls, cocoa_sel("wantsUpdateLayer"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewYes,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("updateLayer"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewUpdateLayer,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("cursorUpdate:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewCursorUpdate,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("viewDidChangeBackingProperties"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewDidChangeBackingProperties,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("drawRect:"),
                    (void*)(delegate* unmanaged<void*, nint, NSRect, void>)&cocoa_viewDrawRect,
                    drawRectTypesPtr);
                class_addMethod(cls, cocoa_sel("updateTrackingAreas"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewUpdateTrackingAreas,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseDown:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseDown,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseUp:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseUp,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("rightMouseDown:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewRightMouseDown,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("rightMouseUp:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewRightMouseUp,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("otherMouseDown:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewOtherMouseDown,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("otherMouseUp:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewOtherMouseUp,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseDragged:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseMoved,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("rightMouseDragged:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseMoved,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("otherMouseDragged:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseMoved,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseMoved:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseMoved,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseEntered:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseEntered,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("mouseExited:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewMouseExited,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("scrollWheel:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewScrollWheel,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("keyDown:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewKeyDown,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("keyUp:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewKeyUp,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("flagsChanged:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, void>)&cocoa_viewFlagsChanged,
                    eventVoidTypesPtr);
                class_addMethod(cls, cocoa_sel("draggingEntered:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, ulong>)&cocoa_viewDraggingEntered,
                    dragOperationTypesPtr);
                class_addMethod(cls, cocoa_sel("performDragOperation:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, byte>)&cocoa_viewPerformDragOperation,
                    eventBoolTypesPtr);
                class_addMethod(cls, cocoa_sel("hasMarkedText"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewHasMarkedText,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("markedRange"),
                    (void*)(delegate* unmanaged<void*, nint, NSRange>)&cocoa_viewMarkedRange,
                    rangeTypesPtr);
                class_addMethod(cls, cocoa_sel("selectedRange"),
                    (void*)(delegate* unmanaged<void*, nint, NSRange>)&cocoa_viewSelectedRange,
                    rangeTypesPtr);
                class_addMethod(cls, cocoa_sel("setMarkedText:selectedRange:replacementRange:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, NSRange, NSRange, void>)&cocoa_viewSetMarkedText,
                    markedTextTypesPtr);
                class_addMethod(cls, cocoa_sel("unmarkText"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewUnmarkText,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("validAttributesForMarkedText"),
                    (void*)(delegate* unmanaged<void*, nint, void*>)&cocoa_viewValidAttributesForMarkedText,
                    objectTypesPtr);
                class_addMethod(cls, cocoa_sel("attributedSubstringForProposedRange:actualRange:"),
                    (void*)(delegate* unmanaged<void*, nint, NSRange, void*, void*>)&cocoa_viewAttributedSubstringForProposedRange,
                    substringTypesPtr);
                class_addMethod(cls, cocoa_sel("characterIndexForPoint:"),
                    (void*)(delegate* unmanaged<void*, nint, NSPoint, ulong>)&cocoa_viewCharacterIndexForPoint,
                    indexForPointTypesPtr);
                class_addMethod(cls, cocoa_sel("firstRectForCharacterRange:actualRange:"),
                    (void*)(delegate* unmanaged<void*, nint, NSRange, void*, NSRect>)&cocoa_viewFirstRectForCharacterRange,
                    rectForRangeTypesPtr);
                class_addMethod(cls, cocoa_sel("insertText:replacementRange:"),
                    (void*)(delegate* unmanaged<void*, nint, void*, NSRange, void>)&cocoa_viewInsertText,
                    insertTextTypesPtr);
                class_addMethod(cls, cocoa_sel("doCommandBySelector:"),
                    (void*)(delegate* unmanaged<void*, nint, nint, void>)&cocoa_viewDoCommandBySelector,
                    selectorVoidTypesPtr);
            }

            objc_registerClassPair(cls);
            _glfw.ns.contentViewClass = cls;
            return cls;
        }
    }

    [UnmanagedCallersOnly]
    static byte cocoa_canBecomeKeyWindow(void* self, nint cmd)
    {
        return 1;
    }

    [UnmanagedCallersOnly]
    static byte cocoa_canBecomeMainWindow(void* self, nint cmd)
    {
        return 1;
    }

    [UnmanagedCallersOnly]
    static void cocoa_selectedKeyboardInputSourceChanged(void* self, nint cmd, void* notification)
    {
        cocoa_updateUnicodeData();
    }

    [UnmanagedCallersOnly]
    static void cocoa_doNothing(void* self, nint cmd, void* @object)
    {
    }

    static void cocoa_windowUpdateSizes(_GLFWwindow* window)
    {
        if (window == null || window->ns.view == null)
            return;

        var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
        var fbRect = objc_msgSend_rect_rect(window->ns.view, cocoa_sel("convertRectToBacking:"), contentRect);

        var fbWidth = (int)fbRect.size.width;
        var fbHeight = (int)fbRect.size.height;
        if (fbWidth != window->ns.fbWidth || fbHeight != window->ns.fbHeight)
        {
            window->ns.fbWidth = fbWidth;
            window->ns.fbHeight = fbHeight;
            _glfwInputFramebufferSize(window, fbWidth, fbHeight);
        }

        var width = (int)contentRect.size.width;
        var height = (int)contentRect.size.height;
        if (width != window->ns.width || height != window->ns.height)
        {
            window->ns.width = width;
            window->ns.height = height;
            _glfwInputWindowSize(window, width, height);
        }
    }

    [UnmanagedCallersOnly]
    static byte cocoa_windowShouldClose(void* self, nint cmd, void* sender)
    {
        var window = cocoa_getObjectWindow(self);
        if (window != null)
            _glfwInputWindowCloseRequest(window);
        return 0;
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidResize(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->context.source == GLFW_NATIVE_CONTEXT_API && window->context.nsgl.@object != null)
            cocoa_msgSend_void(window->context.nsgl.@object, "update");

        if (_glfw.ns.disabledCursorWindow == window)
            _glfwCenterCursorInContentArea(window);

        var maximized = _glfwWindowMaximizedCocoa(window);
        if (window->ns.maximized != maximized)
        {
            window->ns.maximized = maximized;
            _glfwInputWindowMaximize(window, maximized);
        }

        cocoa_windowUpdateSizes(window);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidMove(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->context.source == GLFW_NATIVE_CONTEXT_API && window->context.nsgl.@object != null)
            cocoa_msgSend_void(window->context.nsgl.@object, "update");

        if (_glfw.ns.disabledCursorWindow == window)
            _glfwCenterCursorInContentArea(window);

        int x;
        int y;
        _glfwGetWindowPosCocoa(window, &x, &y);
        _glfwInputWindowPos(window, x, y);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidMiniaturize(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->monitor != null)
            cocoa_releaseMonitor(window);

        window->ns.iconified = GLFW_TRUE;
        _glfwInputWindowIconify(window, GLFW_TRUE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidDeminiaturize(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->monitor != null)
            cocoa_acquireMonitor(window);

        window->ns.iconified = GLFW_FALSE;
        _glfwInputWindowIconify(window, GLFW_FALSE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidBecomeKey(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (_glfw.ns.disabledCursorWindow == window)
            _glfwCenterCursorInContentArea(window);

        window->ns.focused = GLFW_TRUE;
        _glfwInputWindowFocus(window, GLFW_TRUE);
        cocoa_updateCursorMode(window);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidResignKey(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->monitor != null && window->autoIconify != 0)
            _glfwIconifyWindowCocoa(window);

        window->ns.focused = GLFW_FALSE;
        _glfwInputWindowFocus(window, GLFW_FALSE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_windowDidChangeOcclusionState(void* self, nint cmd, void* notification)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.@object == null)
            return;

        var occlusionStateSelector = cocoa_sel("occlusionState");
        if (objc_msgSend_bool_nint(window->ns.@object, cocoa_sel("respondsToSelector:"), occlusionStateSelector) == 0)
            return;

        var state = objc_msgSend_ulong(window->ns.@object, occlusionStateSelector);
        window->ns.occluded = (state & NSWindowOcclusionStateVisible) != 0
            ? GLFW_FALSE
            : GLFW_TRUE;
    }

    [UnmanagedCallersOnly]
    static long cocoa_applicationShouldTerminate(void* self, nint cmd, void* sender)
    {
        for (var window = _glfw.windowListHead; window != null; window = window->next)
            _glfwInputWindowCloseRequest(window);

        return 0;
    }

    [UnmanagedCallersOnly]
    static void cocoa_applicationDidChangeScreenParameters(void* self, nint cmd, void* notification)
    {
        for (var window = _glfw.windowListHead; window != null; window = window->next)
        {
            if (window->context.client != GLFW_NO_API &&
                window->context.nsgl.@object != null)
            {
                cocoa_msgSend_void(window->context.nsgl.@object, "update");
            }
        }

        _glfwPollMonitorsCocoa();
    }

    [UnmanagedCallersOnly]
    static void cocoa_applicationWillFinishLaunching(void* self, nint cmd, void* notification)
    {
        if (_glfw.hints.init.ns.menubar == 0)
            return;

        var bundle = cocoa_msgSend_id(cocoa_getClass("NSBundle"), "mainBundle");
        if (bundle == null)
        {
            cocoa_createMenuBar();
            return;
        }

        var resource = cocoa_stringFromUTF8("MainMenu");
        var type = cocoa_stringFromUTF8("nib");
        var path = objc_msgSend_id_ptr_ptr(bundle,
            cocoa_sel("pathForResource:ofType:"),
            resource,
            type);

        if (path != null)
        {
            fixed (_GLFWlibrary* glfw = &_glfw)
            {
                objc_msgSend_bool_ptr_ptr_ptr(bundle,
                    cocoa_sel("loadNibNamed:owner:topLevelObjects:"),
                    resource,
                    cocoa_getNSApp(),
                    &glfw->ns.nibObjects);
            }
        }
        else
            cocoa_createMenuBar();

        cocoa_releaseTemporaryString(type);
        cocoa_releaseTemporaryString(resource);
    }

    [UnmanagedCallersOnly]
    static void cocoa_applicationDidFinishLaunching(void* self, nint cmd, void* notification)
    {
        _glfwPostEmptyEventCocoa();
        cocoa_msgSend_void_ptr(cocoa_getNSApp(), "stop:", null);
    }

    [UnmanagedCallersOnly]
    static void cocoa_applicationDidHide(void* self, nint cmd, void* notification)
    {
        for (var i = 0; i < _glfw.monitorCount; i++)
            _glfwRestoreVideoModeCocoa(_glfw.monitors[i]);
    }

    [UnmanagedCallersOnly]
    static byte cocoa_viewIsOpaque(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.@object == null)
            return 1;

        return objc_msgSend_bool(window->ns.@object, cocoa_sel("isOpaque"));
    }

    [UnmanagedCallersOnly]
    static byte cocoa_viewYes(void* self, nint cmd)
    {
        return 1;
    }

    [UnmanagedCallersOnly]
    static byte cocoa_viewAcceptsFirstMouse(void* self, nint cmd, void* eventObject)
    {
        return 1;
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewUpdateLayer(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->context.source == GLFW_NATIVE_CONTEXT_API && window->context.nsgl.@object != null)
            cocoa_msgSend_void(window->context.nsgl.@object, "update");

        _glfwInputWindowDamage(window);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewCursorUpdate(void* self, nint cmd, void* eventObject)
    {
        cocoa_updateCursorImage(cocoa_getObjectWindow(self));
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewDidChangeBackingProperties(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.view == null)
            return;

        var contentRect = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
        var fbRect = objc_msgSend_rect_rect(window->ns.view, cocoa_sel("convertRectToBacking:"), contentRect);

        if (contentRect.size.width > 0 && contentRect.size.height > 0)
        {
            var xscale = (float)(fbRect.size.width / contentRect.size.width);
            var yscale = (float)(fbRect.size.height / contentRect.size.height);
            if (xscale != window->ns.xscale || yscale != window->ns.yscale)
            {
                if (window->ns.retina != 0 && window->ns.layer != null)
                {
                    var scale = objc_msgSend_double(window->ns.@object, cocoa_sel("backingScaleFactor"));
                    objc_msgSend_void_double(window->ns.layer, cocoa_sel("setContentsScale:"), scale);
                }

                window->ns.xscale = xscale;
                window->ns.yscale = yscale;
                _glfwInputWindowContentScale(window, xscale, yscale);
            }
        }

        var fbWidth = (int)fbRect.size.width;
        var fbHeight = (int)fbRect.size.height;
        if (fbWidth != window->ns.fbWidth || fbHeight != window->ns.fbHeight)
        {
            window->ns.fbWidth = fbWidth;
            window->ns.fbHeight = fbHeight;
            _glfwInputFramebufferSize(window, fbWidth, fbHeight);
        }
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewDrawRect(void* self, nint cmd, NSRect rect)
    {
        var window = cocoa_getObjectWindow(self);
        if (window != null)
            _glfwInputWindowDamage(window);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewUpdateTrackingAreas(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->ns.trackingArea != null)
        {
            cocoa_msgSend_void_ptr(self, "removeTrackingArea:", window->ns.trackingArea);
            cocoa_msgSend_void(window->ns.trackingArea, "release");
            window->ns.trackingArea = null;
        }

        var options = NSTrackingMouseEnteredAndExited |
                      NSTrackingActiveInKeyWindow |
                      NSTrackingEnabledDuringMouseDrag |
                      NSTrackingCursorUpdate |
                      NSTrackingInVisibleRect |
                      NSTrackingAssumeInside;

        var allocated = cocoa_msgSend_id(cocoa_getClass("NSTrackingArea"), "alloc");
        if (allocated == null)
            return;

        window->ns.trackingArea = objc_msgSend_id_rect_ulong_ptr_ptr(allocated,
            cocoa_sel("initWithRect:options:owner:userInfo:"),
            objc_msgSend_rect(self, cocoa_sel("bounds")),
            options,
            self,
            null);
        if (window->ns.trackingArea != null)
            cocoa_msgSend_void_ptr(self, "addTrackingArea:", window->ns.trackingArea);
    }

    static int cocoa_eventMods(void* eventObject)
    {
        return cocoa_translateFlags((ulong)objc_msgSend_ulong(eventObject, cocoa_sel("modifierFlags")));
    }

    static void cocoa_viewMouseButton(void* self, void* eventObject, int button, int action)
    {
        var window = cocoa_getObjectWindow(self);
        if (window != null)
            _glfwInputMouseClick(window, button, action, cocoa_eventMods(eventObject));
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseDown(void* self, nint cmd, void* eventObject)
    {
        cocoa_viewMouseButton(self, eventObject, GLFW_MOUSE_BUTTON_LEFT, GLFW_PRESS);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseUp(void* self, nint cmd, void* eventObject)
    {
        cocoa_viewMouseButton(self, eventObject, GLFW_MOUSE_BUTTON_LEFT, GLFW_RELEASE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewRightMouseDown(void* self, nint cmd, void* eventObject)
    {
        cocoa_viewMouseButton(self, eventObject, GLFW_MOUSE_BUTTON_RIGHT, GLFW_PRESS);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewRightMouseUp(void* self, nint cmd, void* eventObject)
    {
        cocoa_viewMouseButton(self, eventObject, GLFW_MOUSE_BUTTON_RIGHT, GLFW_RELEASE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewOtherMouseDown(void* self, nint cmd, void* eventObject)
    {
        var button = (int)objc_msgSend_long(eventObject, cocoa_sel("buttonNumber"));
        cocoa_viewMouseButton(self, eventObject, button, GLFW_PRESS);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewOtherMouseUp(void* self, nint cmd, void* eventObject)
    {
        var button = (int)objc_msgSend_long(eventObject, cocoa_sel("buttonNumber"));
        cocoa_viewMouseButton(self, eventObject, button, GLFW_RELEASE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseMoved(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->cursorMode == GLFW_CURSOR_DISABLED)
        {
            var dx = objc_msgSend_double(eventObject, cocoa_sel("deltaX")) - window->ns.cursorWarpDeltaX;
            var dy = objc_msgSend_double(eventObject, cocoa_sel("deltaY")) - window->ns.cursorWarpDeltaY;
            _glfwInputCursorPos(window,
                window->virtualCursorPosX + dx,
                window->virtualCursorPosY + dy);
        }
        else
        {
            var contentRect = window->ns.view != null
                ? objc_msgSend_rect(window->ns.view, cocoa_sel("frame"))
                : cocoa_makeRect(0, 0, window->ns.width, window->ns.height);
            var pos = objc_msgSend_point(eventObject, cocoa_sel("locationInWindow"));
            _glfwInputCursorPos(window, pos.x, contentRect.size.height - pos.y);
        }

        window->ns.cursorWarpDeltaX = 0;
        window->ns.cursorWarpDeltaY = 0;
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseEntered(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->cursorMode == GLFW_CURSOR_HIDDEN)
            cocoa_hideCursor();

        window->ns.hovered = GLFW_TRUE;
        _glfwInputCursorEnter(window, GLFW_TRUE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseExited(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        if (window->cursorMode == GLFW_CURSOR_HIDDEN)
            cocoa_showCursor();

        window->ns.hovered = GLFW_FALSE;
        _glfwInputCursorEnter(window, GLFW_FALSE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewScrollWheel(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        var deltaX = objc_msgSend_double(eventObject, cocoa_sel("scrollingDeltaX"));
        var deltaY = objc_msgSend_double(eventObject, cocoa_sel("scrollingDeltaY"));

        if (objc_msgSend_bool(eventObject, cocoa_sel("hasPreciseScrollingDeltas")) != 0)
        {
            deltaX *= 0.1;
            deltaY *= 0.1;
        }

        if (deltaX != 0.0 || deltaY != 0.0)
            _glfwInputScroll(window, deltaX, deltaY);
    }

    static void cocoa_viewKey(void* self, void* eventObject, int action)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        var scancode = (int)objc_msgSend_uint(eventObject, cocoa_sel("keyCode"));
        var key = cocoa_translateKey((uint)scancode);
        _glfwInputKey(window, key, scancode, action, cocoa_eventMods(eventObject));
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewKeyDown(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        cocoa_viewKey(self, eventObject, GLFW_PRESS);

        var eventArray = cocoa_msgSend_id_ptr(cocoa_getClass("NSArray"), "arrayWithObject:", eventObject);
        if (eventArray != null)
            cocoa_msgSend_void_ptr(self, "interpretKeyEvents:", eventArray);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewKeyUp(void* self, nint cmd, void* eventObject)
    {
        cocoa_viewKey(self, eventObject, GLFW_RELEASE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewFlagsChanged(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        var scancode = (int)objc_msgSend_uint(eventObject, cocoa_sel("keyCode"));
        var key = cocoa_translateKey((uint)scancode);
        if (key == GLFW_KEY_UNKNOWN)
            return;

        var modifierFlags = (ulong)objc_msgSend_ulong(eventObject, cocoa_sel("modifierFlags")) &
                            NSEventModifierFlagDeviceIndependentFlagsMask;
        var keyFlag = cocoa_translateKeyToModifierFlag(key);
        var action = (keyFlag & modifierFlags) != 0
            ? window->keys[key] == GLFW_PRESS ? GLFW_RELEASE : GLFW_PRESS
            : GLFW_RELEASE;

        _glfwInputKey(window, key, scancode, action, cocoa_translateFlags(modifierFlags));
    }

    [UnmanagedCallersOnly]
    static ulong cocoa_viewDraggingEntered(void* self, nint cmd, void* sender)
    {
        return NSDragOperationGeneric;
    }

    [UnmanagedCallersOnly]
    static byte cocoa_viewPerformDragOperation(void* self, nint cmd, void* sender)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return 0;

        var contentRect = window->ns.view != null
            ? objc_msgSend_rect(window->ns.view, cocoa_sel("frame"))
            : cocoa_makeRect(0, 0, window->ns.width, window->ns.height);
        var pos = objc_msgSend_point(sender, cocoa_sel("draggingLocation"));
        _glfwInputCursorPos(window, pos.x, contentRect.size.height - pos.y);

        var pasteboard = cocoa_msgSend_id(sender, "draggingPasteboard");
        if (pasteboard == null)
            return 1;

        var urlClass = cocoa_getClass("NSURL");
        var classes = cocoa_msgSend_id_ptr(cocoa_getClass("NSArray"), "arrayWithObject:", urlClass);
        var fileURLsOnlyKey = cocoa_stringFromUTF8("NSPasteboardURLReadingFileURLsOnlyKey");
        var fileURLsOnly = cocoa_msgSend_id_bool(cocoa_getClass("NSNumber"), "numberWithBool:", GLFW_TRUE);
        var options = fileURLsOnlyKey != null && fileURLsOnly != null
            ? objc_msgSend_id_ptr_ptr(cocoa_getClass("NSDictionary"),
                cocoa_sel("dictionaryWithObject:forKey:"),
                fileURLsOnly,
                fileURLsOnlyKey)
            : null;
        var urls = objc_msgSend_id_ptr_ptr(pasteboard,
            cocoa_sel("readObjectsForClasses:options:"),
            classes,
            options);
        cocoa_releaseTemporaryString(fileURLsOnlyKey);

        if (urls == null)
            return 1;

        var urlCount = (nuint)objc_msgSend_ulong(urls, cocoa_sel("count"));
        if (urlCount == 0)
            return 1;

        var paths = (byte**)_glfw_calloc(urlCount, (nuint)sizeof(byte*));
        if (paths == null)
        {
            _glfwInputError(GLFW_OUT_OF_MEMORY);
            return 0;
        }

        var pathCount = 0;
        for (nuint i = 0; i < urlCount; i++)
        {
            var url = objc_msgSend_id_ulong(urls, cocoa_sel("objectAtIndex:"), (ulong)i);
            if (url == null || objc_msgSend_bool(url, cocoa_sel("isFileURL")) == 0)
                continue;

            var representation = objc_msgSend_UTF8String(url, cocoa_sel("fileSystemRepresentation"));
            if (representation == null)
                continue;

            paths[pathCount] = _glfw_strdup(representation);
            if (paths[pathCount] != null)
                pathCount++;
        }

        if (pathCount != 0)
            _glfwInputDrop(window, pathCount, paths);

        for (var i = 0; i < pathCount; i++)
            _glfw_free(paths[i]);
        _glfw_free(paths);

        return 1;
    }

    [UnmanagedCallersOnly]
    static byte cocoa_viewHasMarkedText(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.markedText == null)
            return 0;

        return objc_msgSend_ulong(window->ns.markedText, cocoa_sel("length")) != 0 ? (byte)1 : (byte)0;
    }

    [UnmanagedCallersOnly]
    static NSRange cocoa_viewMarkedRange(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.markedText == null)
            return cocoa_emptyRange();

        var length = (nuint)objc_msgSend_ulong(window->ns.markedText, cocoa_sel("length"));
        if (length == 0)
            return cocoa_emptyRange();

        return new NSRange { location = 0, length = length - 1 };
    }

    [UnmanagedCallersOnly]
    static NSRange cocoa_viewSelectedRange(void* self, nint cmd)
    {
        return cocoa_emptyRange();
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewSetMarkedText(void* self, nint cmd, void* stringObject, NSRange selectedRange, NSRange replacementRange)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        cocoa_msgSend_void(window->ns.markedText, "release");
        window->ns.markedText = null;

        var mutableClass = cocoa_getClass("NSMutableAttributedString");
        var allocated = cocoa_msgSend_id(mutableClass, "alloc");
        if (allocated == null)
            return;

        if (stringObject == null)
            window->ns.markedText = cocoa_msgSend_id(allocated, "init");
        else if (objc_msgSend_bool_ptr(stringObject,
                     cocoa_sel("isKindOfClass:"),
                     cocoa_getClass("NSAttributedString")) != 0)
        {
            window->ns.markedText = cocoa_msgSend_id_ptr(allocated,
                "initWithAttributedString:",
                stringObject);
        }
        else
        {
            window->ns.markedText = cocoa_msgSend_id_ptr(allocated,
                "initWithString:",
                stringObject);
        }
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewUnmarkText(void* self, nint cmd)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.markedText == null)
            return;

        var mutableString = cocoa_msgSend_id(window->ns.markedText, "mutableString");
        if (mutableString == null)
            return;

        var empty = cocoa_stringFromUTF8("");
        cocoa_msgSend_void_ptr(mutableString, "setString:", empty);
        cocoa_releaseTemporaryString(empty);
    }

    [UnmanagedCallersOnly]
    static void* cocoa_viewValidAttributesForMarkedText(void* self, nint cmd)
    {
        return cocoa_msgSend_id(cocoa_getClass("NSArray"), "array");
    }

    [UnmanagedCallersOnly]
    static void* cocoa_viewAttributedSubstringForProposedRange(void* self, nint cmd, NSRange range, void* actualRange)
    {
        return null;
    }

    [UnmanagedCallersOnly]
    static ulong cocoa_viewCharacterIndexForPoint(void* self, nint cmd, NSPoint point)
    {
        return 0;
    }

    [UnmanagedCallersOnly]
    static NSRect cocoa_viewFirstRectForCharacterRange(void* self, nint cmd, NSRange range, void* actualRange)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || window->ns.view == null)
            return cocoa_makeRect(0, 0, 0, 0);

        var frame = objc_msgSend_rect(window->ns.view, cocoa_sel("frame"));
        return cocoa_makeRect(frame.origin.x, frame.origin.y, 0, 0);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewInsertText(void* self, nint cmd, void* stringObject, NSRange replacementRange)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null || stringObject == null)
            return;

        void* characters;
        if (objc_msgSend_bool_ptr(stringObject,
                cocoa_sel("isKindOfClass:"),
                cocoa_getClass("NSAttributedString")) != 0)
        {
            characters = cocoa_msgSend_id(stringObject, "string");
        }
        else
            characters = stringObject;

        if (characters == null)
            return;

        var utf8 = objc_msgSend_UTF8String(characters, cocoa_sel("UTF8String"));
        if (utf8 == null)
            return;

        var text = Marshal.PtrToStringUTF8((nint)utf8);
        if (string.IsNullOrEmpty(text))
            return;

        var eventObject = cocoa_msgSend_id(cocoa_getNSApp(), "currentEvent");
        var mods = eventObject != null ? cocoa_eventMods(eventObject) : 0;
        var plain = (mods & GLFW_MOD_SUPER) == 0 ? GLFW_TRUE : GLFW_FALSE;
        foreach (var rune in text.EnumerateRunes())
        {
            var codepoint = rune.Value;
            if (codepoint >= 0xf700 && codepoint <= 0xf7ff)
                continue;

            _glfwInputChar(window, (uint)codepoint, mods, plain);
        }
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewDoCommandBySelector(void* self, nint cmd, nint selector)
    {
    }

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_getClass")]
    static extern void* objc_getClass(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_lookUpClass")]
    static extern void* objc_lookUpClass(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_getProtocol")]
    static extern void* objc_getProtocol(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "sel_registerName")]
    static extern nint sel_registerName(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_allocateClassPair")]
    static extern void* objc_allocateClassPair(void* superclass, byte* name, nuint extraBytes);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_registerClassPair")]
    static extern void objc_registerClassPair(void* cls);

    [DllImport("libobjc.A.dylib", EntryPoint = "class_addMethod")]
    static extern byte class_addMethod(void* cls, nint name, void* imp, byte* types);

    [DllImport("libobjc.A.dylib", EntryPoint = "class_addProtocol")]
    static extern byte class_addProtocol(void* cls, void* protocol);

    [DllImport("libobjc.A.dylib", EntryPoint = "class_addIvar")]
    static extern byte class_addIvar(void* cls, byte* name, nuint size, byte alignment, byte* types);

    [DllImport("libobjc.A.dylib", EntryPoint = "object_getIvar")]
    static extern void* object_getIvar(void* obj, void* ivar);

    [DllImport("libobjc.A.dylib", EntryPoint = "object_setIvar")]
    static extern void object_setIvar(void* obj, void* ivar, void* value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_bool(void* receiver, nint selector, byte value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr(void* receiver, nint selector, void* value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_ptr(void* receiver, nint selector, void* value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_bool(void* receiver, nint selector, void* value, byte value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_long(void* receiver, nint selector, void* value, long value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_nint_ptr_ptr(void* receiver, nint selector, nint value1, void* value2, void* value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_nint_ptr(void* receiver, nint selector, nint value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_nint_ptr_ptr(void* receiver, nint selector, void* value1, nint value2, void* value3, void* value4);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_ptr_ptr(void* receiver, nint selector, void* value1, void* value2, void* value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_bool(void* receiver, nint selector, byte value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_int(void* receiver, nint selector, int value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_uint(void* receiver, nint selector, uint value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ulong(void* receiver, nint selector, ulong value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ulong_ptr(void* receiver, nint selector, ulong value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_nint(void* receiver, nint selector, nint value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_double(void* receiver, nint selector, double value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr(void* receiver, nint selector, void* value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr_ptr(void* receiver, nint selector, void* value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr_nint_ptr(void* receiver, nint selector, void* value1, nint value2, void* value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr_long_long_long_long_bool_bool_ptr_ulong_long_long(void* receiver, nint selector, void* value1, long value2, long value3, long value4, long value5, byte value6, byte value7, void* value8, ulong value9, long value10, long value11);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_size(void* receiver, nint selector, NSSize size);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr_point(void* receiver, nint selector, void* value1, NSPoint value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_rect(void* receiver, nint selector, NSRect rect);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_rect_ulong_ptr_ptr(void* receiver, nint selector, NSRect rect, ulong value1, void* value2, void* value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_rect_ulong_long_bool(void* receiver, nint selector, NSRect rect, ulong value1, long value2, byte value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool_ptr(void* receiver, nint selector, void* value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool_nint(void* receiver, nint selector, nint value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool_ptr_ptr(void* receiver, nint selector, void* value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool_ptr_ptr_ptr(void* receiver, nint selector, void* value1, void* value2, void* value3);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte objc_msgSend_bool_point_rect(void* receiver, nint selector, NSPoint point, NSRect rect);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern int objc_msgSend_int(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern uint objc_msgSend_uint(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern long objc_msgSend_long(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern ulong objc_msgSend_ulong(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern long objc_msgSend_long_ptr_ptr(void* receiver, nint selector, void* value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern long objc_msgSend_long_point_long(void* receiver, nint selector, NSPoint point, long value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern double objc_msgSend_double(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte* objc_msgSend_UTF8String(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte* objc_msgSend_bytes(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern NSPoint objc_msgSend_point(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern NSRect objc_msgSend_rect(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern NSRect objc_msgSend_rect_rect(void* receiver, nint selector, NSRect rect);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern NSRect objc_msgSend_rect_rect_ulong(void* receiver, nint selector, NSRect rect, ulong value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_long(void* receiver, nint selector, long value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ulong(void* receiver, nint selector, ulong value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_double(void* receiver, nint selector, double value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_point(void* receiver, nint selector, NSPoint point);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_size(void* receiver, nint selector, NSSize size);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_rect_bool(void* receiver, nint selector, NSRect rect, byte value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ulong_ptr_ptr_bool(void* receiver, nint selector, ulong value1, void* value2, void* value3, byte value4);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_long_point_ulong_double_long_ptr_long_long_long(void* receiver, nint selector, long value1, NSPoint value2, ulong value3, double value4, long value5, void* value6, long value7, long value8, long value9);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGMainDisplayID();

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern NSRect CGDisplayBounds(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGGetOnlineDisplayList(uint maxDisplays, uint* onlineDisplays, uint* displayCount);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern byte CGDisplayIsAsleep(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGDisplayUnitNumber(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGDisplayVendorNumber(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGDisplayModelNumber(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern NSSize CGDisplayScreenSize(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void* CGDisplayCopyDisplayMode(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void* CGDisplayCopyAllDisplayModes(uint display, void* options);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGDisplaySetDisplayMode(uint display, void* mode, void* options);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGOpenGLDisplayMaskToDisplayID(uint mask);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGAcquireDisplayFadeReservation(double seconds, uint* token);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGDisplayFade(uint token, double duration, double startBlend, double endBlend, double redBlend, double greenBlend, double blueBlend, byte synchronous);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void CGReleaseDisplayFadeReservation(uint token);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGDisplayMoveCursorToPoint(uint display, NSPoint point);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGWarpMouseCursorPosition(NSPoint point);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGAssociateMouseAndMouseCursorPosition(byte connected);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void CGDisplayModeRelease(void* mode);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern nuint CGDisplayModeGetWidth(void* mode);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern nuint CGDisplayModeGetHeight(void* mode);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern double CGDisplayModeGetRefreshRate(void* mode);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGDisplayModeGetIOFlags(void* mode);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern uint CGDisplayGammaTableCapacity(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGGetDisplayTransferByTable(uint display, uint capacity, float* red, float* green, float* blue, uint* sampleCount);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGSetDisplayTransferByTable(uint display, uint tableSize, float* red, float* green, float* blue);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void* CGEventSourceCreate(int stateID);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void CGEventSourceSetLocalEventsSuppressionInterval(void* source, double seconds);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFStringCreateWithCString(void* allocator, byte* cStr, uint encoding);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFStringCreateWithCharacters(void* allocator, ushort* chars, nint numChars);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFRelease(void* value);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern byte CFNumberGetValue(void* number, int theType, void* valuePtr);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern byte CFStringGetCString(void* theString, byte* buffer, nint bufferSize, uint encoding);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern nint CFStringGetLength(void* theString);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern nint CFStringGetMaximumSizeForEncoding(nint length, uint encoding);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFDictionaryGetValue(void* theDict, void* key);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern byte CFDictionaryGetValueIfPresent(void* theDict, void* key, void** value);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFArrayCreateMutable(void* allocator, nint capacity, void* callBacks);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern nint CFArrayGetCount(void* array);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFArrayGetValueAtIndex(void* array, nint index);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFArrayAppendValue(void* theArray, void* value);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFArraySortValues(void* theArray,
                                         CFRange range,
                                         delegate* unmanaged<void*, void*, void*, nint> comparator,
                                         void* context);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern nuint CFGetTypeID(void* cf);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetBundleWithIdentifier(void* bundleID);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetMainBundle();

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleCopyPrivateFrameworksURL(void* bundle);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetFunctionPointerForName(void* bundle, void* functionName);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetDataPointerForName(void* bundle, void* symbolName);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFURLCreateCopyAppendingPathComponent(void* allocator, void* url, void* pathComponent, byte isDirectory);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern byte CFURLGetFileSystemRepresentation(void* url, byte resolveAgainstBase, byte* buffer, nint maxBufLen);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFRunLoopGetMain();

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern int CFRunLoopRunInMode(void* mode, double seconds, byte returnAfterSourceHandled);

    [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "_NSGetProgname")]
    static extern byte** _NSGetProgname();

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IOServiceMatching(byte* name);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern int IOServiceGetMatchingServices(uint mainPort, void* matching, uint* existing);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern uint IOIteratorNext(uint iterator);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern int IOObjectRelease(uint @object);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IORegistryEntryCreateCFProperty(uint entry, void* key, void* allocator, uint options);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IODisplayCreateInfoDictionary(uint framebuffer, uint options);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IOHIDManagerCreate(void* allocator, int options);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void IOHIDManagerSetDeviceMatchingMultiple(void* manager, void* multiple);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void IOHIDManagerRegisterDeviceMatchingCallback(void* manager,
                                                                  delegate* unmanaged<void*, int, void*, void*, void> callback,
                                                                  void* context);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void IOHIDManagerRegisterDeviceRemovalCallback(void* manager,
                                                                 delegate* unmanaged<void*, int, void*, void*, void> callback,
                                                                 void* context);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void IOHIDManagerScheduleWithRunLoop(void* manager, void* runLoop, void* runLoopMode);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern int IOHIDManagerOpen(void* manager, int options);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IOHIDDeviceGetProperty(void* device, void* key);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern void* IOHIDDeviceCopyMatchingElements(void* device, void* matching, int options);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern int IOHIDDeviceGetValue(void* device, void* element, void** value);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern long IOHIDValueGetIntegerValue(void* value);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern nuint IOHIDElementGetTypeID();

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern int IOHIDElementGetType(void* element);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern uint IOHIDElementGetUsage(void* element);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern uint IOHIDElementGetUsagePage(void* element);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern long IOHIDElementGetLogicalMin(void* element);

    [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    static extern long IOHIDElementGetLogicalMax(void* element);
}
