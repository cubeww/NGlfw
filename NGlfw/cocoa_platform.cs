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
    const ulong NSEventMaskAny = ulong.MaxValue;
    const long NSEventTypeApplicationDefined = 15;

    static readonly byte* _glfwCocoaMappingName = _glfw_allocate_static_string("Mac OS X");
    static readonly byte* _glfwCocoaPasteboardTypeString = _glfw_allocate_static_string("public.utf8-plain-text");
    static readonly byte* _glfwCocoaRunLoopDefaultMode = _glfw_allocate_static_string("kCFRunLoopDefaultMode");
    static readonly byte* _glfwCocoaOpenGLBundleID = _glfw_allocate_static_string("com.apple.opengl");
    static readonly object cocoaObjectMapLock = new();
    static readonly Dictionary<nint, nint> cocoaObjectWindows = new();

    public struct _GLFWlibraryNS
    {
        public void* autoreleasePool;
        public void* app;
        public void* delegateObject;
        public void* helper;
        public void* cursor;
        public void* keyUpMonitor;
        public void* nibObjects;
        public void* windowClass;
        public void* contentViewClass;
        public void* windowDelegateClass;
        public byte* clipboardString;
        public _GLFWwindow* disabledCursorWindow;
        public int cursorHidden;
        public double restoreCursorPosX;
        public double restoreCursorPosY;
        public NSPoint cascadePoint;
        public fixed short keycodes[256];
        public fixed short scancodes[GLFW_KEY_LAST + 1];
        public fixed byte keynames[512];
    }

    public struct _GLFWwindowNS
    {
        public void* @object;
        public void* delegateObject;
        public void* view;
        public void* layer;
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
        public void* elements;
    }

    public struct _GLFWlibraryNSGL
    {
        public void* framework;
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
            }

            objc_registerClassPair(cls);
            _glfw.ns.windowDelegateClass = cls;
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

            var boolTypes = cocoa_ascii("c@:");
            var voidTypes = cocoa_ascii("v@:");
            var eventVoidTypes = cocoa_ascii("v@:@");
            var drawRectTypes = cocoa_ascii("v@:{CGRect={CGPoint=dd}{CGSize=dd}}");
            fixed (byte* boolTypesPtr = boolTypes)
            fixed (byte* voidTypesPtr = voidTypes)
            fixed (byte* eventVoidTypesPtr = eventVoidTypes)
            fixed (byte* drawRectTypesPtr = drawRectTypes)
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
                class_addMethod(cls, cocoa_sel("wantsUpdateLayer"),
                    (void*)(delegate* unmanaged<void*, nint, byte>)&cocoa_viewYes,
                    boolTypesPtr);
                class_addMethod(cls, cocoa_sel("updateLayer"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewUpdateLayer,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("viewDidChangeBackingProperties"),
                    (void*)(delegate* unmanaged<void*, nint, void>)&cocoa_viewDidChangeBackingProperties,
                    voidTypesPtr);
                class_addMethod(cls, cocoa_sel("drawRect:"),
                    (void*)(delegate* unmanaged<void*, nint, NSRect, void>)&cocoa_viewDrawRect,
                    drawRectTypesPtr);
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

        window->ns.hovered = GLFW_TRUE;
        _glfwInputCursorEnter(window, GLFW_TRUE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewMouseExited(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        window->ns.hovered = GLFW_FALSE;
        _glfwInputCursorEnter(window, GLFW_FALSE);
    }

    [UnmanagedCallersOnly]
    static void cocoa_viewScrollWheel(void* self, nint cmd, void* eventObject)
    {
        var window = cocoa_getObjectWindow(self);
        if (window == null)
            return;

        _glfwInputScroll(window,
            objc_msgSend_double(eventObject, cocoa_sel("scrollingDeltaX")),
            objc_msgSend_double(eventObject, cocoa_sel("scrollingDeltaY")));
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

        var characters = cocoa_msgSend_id(eventObject, "characters");
        if (characters == null)
            return;

        var utf8 = objc_msgSend_UTF8String(characters, cocoa_sel("UTF8String"));
        if (utf8 == null)
            return;

        var text = Marshal.PtrToStringUTF8((nint)utf8);
        if (string.IsNullOrEmpty(text))
            return;

        var mods = cocoa_eventMods(eventObject);
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

        var action = key >= 0 && key <= GLFW_KEY_LAST && window->keys[key] == GLFW_PRESS
            ? GLFW_RELEASE
            : GLFW_PRESS;

        _glfwInputKey(window, key, scancode, action, cocoa_eventMods(eventObject));
    }

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_getClass")]
    static extern void* objc_getClass(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_lookUpClass")]
    static extern void* objc_lookUpClass(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "sel_registerName")]
    static extern nint sel_registerName(byte* name);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_allocateClassPair")]
    static extern void* objc_allocateClassPair(void* superclass, byte* name, nuint extraBytes);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_registerClassPair")]
    static extern void objc_registerClassPair(void* cls);

    [DllImport("libobjc.A.dylib", EntryPoint = "class_addMethod")]
    static extern byte class_addMethod(void* cls, nint name, void* imp, byte* types);

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
    static extern void objc_msgSend_void_ptr_bool(void* receiver, nint selector, void* value, byte value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void objc_msgSend_void_ptr_long(void* receiver, nint selector, void* value, long value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_bool(void* receiver, nint selector, byte value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_int(void* receiver, nint selector, int value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_uint(void* receiver, nint selector, uint value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ulong(void* receiver, nint selector, ulong value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_nint(void* receiver, nint selector, nint value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_double(void* receiver, nint selector, double value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr(void* receiver, nint selector, void* value);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_ptr_ptr(void* receiver, nint selector, void* value1, void* value2);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern void* objc_msgSend_id_rect(void* receiver, nint selector, NSRect rect);

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
    static extern double objc_msgSend_double(void* receiver, nint selector);

    [DllImport("libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    static extern byte* objc_msgSend_UTF8String(void* receiver, nint selector);

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
    static extern NSSize CGDisplayScreenSize(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void* CGDisplayCopyDisplayMode(uint display);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern void* CGDisplayCopyAllDisplayModes(uint display, void* options);

    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    static extern int CGDisplaySetDisplayMode(uint display, void* mode, void* options);

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

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFStringCreateWithCString(void* allocator, byte* cStr, uint encoding);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void CFRelease(void* value);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern nint CFArrayGetCount(void* array);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFArrayGetValueAtIndex(void* array, nint index);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetBundleWithIdentifier(void* bundleID);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    static extern void* CFBundleGetFunctionPointerForName(void* bundle, void* functionName);
}
