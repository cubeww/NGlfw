using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int _GLFW_INSERT_FIRST = 0;
    const int _GLFW_INSERT_LAST = 1;

    const int _GLFW_POLL_PRESENCE = 0;
    const int _GLFW_POLL_AXES = 1;
    const int _GLFW_POLL_BUTTONS = 2;
    const int _GLFW_POLL_ALL = _GLFW_POLL_AXES | _GLFW_POLL_BUTTONS;

    const int _GLFW_MESSAGE_SIZE = 1024;
    const int _GLFW_X11_KEYCODE_LAST = 255;

    const int EGL_NONE = 0x3038;
    const int EGL_PLATFORM_ANGLE_ANGLE = 0x3202;
    const int EGL_PLATFORM_ANGLE_TYPE_ANGLE = 0x3203;
    const int EGL_PLATFORM_ANGLE_TYPE_OPENGL_ANGLE = 0x320d;
    const int EGL_PLATFORM_ANGLE_TYPE_OPENGLES_ANGLE = 0x320e;
    const int EGL_PLATFORM_ANGLE_TYPE_D3D9_ANGLE = 0x3207;
    const int EGL_PLATFORM_ANGLE_TYPE_D3D11_ANGLE = 0x3208;
    const int EGL_PLATFORM_ANGLE_TYPE_VULKAN_ANGLE = 0x3450;
    const int EGL_PLATFORM_ANGLE_TYPE_METAL_ANGLE = 0x3489;
    const int EGL_PLATFORM_X11_EXT = 0x31d5;
    const int EGL_PLATFORM_ANGLE_NATIVE_PLATFORM_TYPE_ANGLE = 0x348f;

    static _GLFWlibrary _glfw;
    static _GLFWerror _glfwMainThreadError;
    static delegate*<int, byte*, void> _glfwErrorCallback;
    static GLFWallocator _glfwInitAllocator;
    static _GLFWinitconfig _glfwInitHints;
    static readonly byte* _glfwVersionString;
    static readonly byte* _glfwVkKHRSurfaceExtensionName;
    static readonly byte* _glfwVkKHRWin32SurfaceExtensionName;
    static readonly byte* _glfwVkKHRXlibSurfaceExtensionName;
    static readonly byte* _glfwVkKHRXcbSurfaceExtensionName;
    static readonly byte* _glfwVkKHRWaylandSurfaceExtensionName;
    static readonly byte* _glfwVkMVKMacOSSurfaceExtensionName;
    static readonly byte* _glfwVkEXTMetalSurfaceExtensionName;
    static readonly byte* _glfwX11InputStyleName;
    static readonly byte* _glfwX11ClientWindowName;
    static readonly byte* _glfwX11FocusWindowName;
    static readonly byte* _glfwX11FilterEventsName;
    static readonly byte* _glfwX11QueryInputStyleName;
    static readonly byte* _glfwX11DestroyCallbackName;
    static readonly byte* _glfwX11EmptyString;
    static readonly byte* _glfwWin32MappingName;

    static Glfw()
    {
        _glfwVersionString = _glfw_allocate_static_string("3.4.0 Win32 WGL X11 GLX Wayland Cocoa NSGL Null EGL OSMesa");
        _glfwVkKHRSurfaceExtensionName = _glfw_allocate_static_string("VK_KHR_surface");
        _glfwVkKHRWin32SurfaceExtensionName = _glfw_allocate_static_string("VK_KHR_win32_surface");
        _glfwVkKHRXlibSurfaceExtensionName = _glfw_allocate_static_string("VK_KHR_xlib_surface");
        _glfwVkKHRXcbSurfaceExtensionName = _glfw_allocate_static_string("VK_KHR_xcb_surface");
        _glfwVkKHRWaylandSurfaceExtensionName = _glfw_allocate_static_string("VK_KHR_wayland_surface");
        _glfwVkMVKMacOSSurfaceExtensionName = _glfw_allocate_static_string("VK_MVK_macos_surface");
        _glfwVkEXTMetalSurfaceExtensionName = _glfw_allocate_static_string("VK_EXT_metal_surface");
        _glfwX11InputStyleName = _glfw_allocate_static_string("inputStyle");
        _glfwX11ClientWindowName = _glfw_allocate_static_string("clientWindow");
        _glfwX11FocusWindowName = _glfw_allocate_static_string("focusWindow");
        _glfwX11FilterEventsName = _glfw_allocate_static_string("filterEvents");
        _glfwX11QueryInputStyleName = _glfw_allocate_static_string("queryInputStyle");
        _glfwX11DestroyCallbackName = _glfw_allocate_static_string("destroyCallback");
        _glfwX11EmptyString = _glfw_allocate_static_string("");
        _glfwWin32MappingName = _glfw_allocate_static_string("Windows");
        _glfwInitHints = default;
        _glfwInitHints.hatButtons = GLFW_TRUE;
        _glfwInitHints.angleType = GLFW_ANGLE_PLATFORM_TYPE_NONE;
        _glfwInitHints.platformID = GLFW_ANY_PLATFORM;
        _glfwInitHints.ns.menubar = GLFW_TRUE;
        _glfwInitHints.ns.chdir = GLFW_TRUE;
        _glfwInitHints.x11.xcbVulkanSurface = GLFW_TRUE;
        _glfwInitHints.wl.libdecorMode = GLFW_WAYLAND_PREFER_LIBDECOR;
    }

    public struct _GLFWerror
    {
        public _GLFWerror* next;
        public int code;
        public fixed byte description[_GLFW_MESSAGE_SIZE];
    }

    public struct _GLFWinitconfig
    {
        public int hatButtons;
        public int angleType;
        public int platformID;
        public delegate*<void*, byte*, void*> vulkanLoader;
        public delegate* unmanaged<void*, byte*, void*> vulkanLoaderNative;
        public _GLFWinitconfig_ns ns;
        public _GLFWinitconfig_x11 x11;
        public _GLFWinitconfig_wl wl;
    }

    public struct _GLFWinitconfig_ns
    {
        public int menubar;
        public int chdir;
    }

    public struct _GLFWinitconfig_x11
    {
        public int xcbVulkanSurface;
    }

    public struct _GLFWinitconfig_wl
    {
        public int libdecorMode;
    }

    public struct _GLFWwndconfig
    {
        public int xpos;
        public int ypos;
        public int width;
        public int height;
        public byte* title;
        public int resizable;
        public int visible;
        public int decorated;
        public int focused;
        public int autoIconify;
        public int floating;
        public int maximized;
        public int centerCursor;
        public int focusOnShow;
        public int mousePassthrough;
        public int scaleToMonitor;
        public int scaleFramebuffer;
        public _GLFWwndconfig_ns ns;
        public _GLFWwndconfig_x11 x11;
        public _GLFWwndconfig_win32 win32;
        public _GLFWwndconfig_wl wl;
    }

    public struct _GLFWwndconfig_ns
    {
        public fixed byte frameName[256];
    }

    public struct _GLFWwndconfig_x11
    {
        public fixed byte className[256];
        public fixed byte instanceName[256];
    }

    public struct _GLFWwndconfig_win32
    {
        public int keymenu;
        public int showDefault;
    }

    public struct _GLFWwndconfig_wl
    {
        public fixed byte appId[256];
    }

    public struct _GLFWctxconfig
    {
        public int client;
        public int source;
        public int major;
        public int minor;
        public int forward;
        public int debug;
        public int noerror;
        public int profile;
        public int robustness;
        public int release;
        public _GLFWwindow* share;
        public _GLFWctxconfig_nsgl nsgl;
    }

    public struct _GLFWctxconfig_nsgl
    {
        public int offline;
    }

    public struct _GLFWfbconfig
    {
        public int redBits;
        public int greenBits;
        public int blueBits;
        public int alphaBits;
        public int depthBits;
        public int stencilBits;
        public int accumRedBits;
        public int accumGreenBits;
        public int accumBlueBits;
        public int accumAlphaBits;
        public int auxBuffers;
        public int stereo;
        public int samples;
        public int sRGB;
        public int doublebuffer;
        public int transparent;
        public nuint handle;
    }

    public struct _GLFWcontext
    {
        public int client;
        public int source;
        public int major;
        public int minor;
        public int revision;
        public int forward;
        public int debug;
        public int noerror;
        public int profile;
        public int robustness;
        public int release;

        public delegate* unmanaged<uint, uint, byte*> GetStringi;
        public delegate* unmanaged<uint, int*, void> GetIntegerv;
        public delegate* unmanaged<uint, byte*> GetString;

        public delegate*<_GLFWwindow*, void> makeCurrent;
        public delegate*<_GLFWwindow*, void> swapBuffers;
        public delegate*<int, void> swapInterval;
        public delegate*<byte*, int> extensionSupported;
        public delegate*<byte*, void*> getProcAddress;
        public delegate*<_GLFWwindow*, void> destroy;

        public _GLFWcontext_egl egl;
        public _GLFWcontext_osmesa osmesa;
        public _GLFWcontext_wgl wgl;
        public _GLFWcontext_glx glx;
        public _GLFWcontext_nsgl nsgl;
    }

    public struct _GLFWcontext_egl
    {
        public void* config;
        public void* handle;
        public void* surface;
        public void* client;
    }

    public struct _GLFWcontext_osmesa
    {
        public void* handle;
        public int width;
        public int height;
        public void* buffer;
    }

    public struct _GLFWcontext_wgl
    {
        public nint dc;
        public nint handle;
        public int interval;
    }

    public struct _GLFWcontext_glx
    {
        public void* handle;
        public nuint window;
    }

    public struct _GLFWcontext_nsgl
    {
        public void* pixelFormat;
        public void* @object;
    }

    public struct _GLFWwindow
    {
        public _GLFWwindow* next;

        public int resizable;
        public int decorated;
        public int autoIconify;
        public int floating;
        public int focusOnShow;
        public int mousePassthrough;
        public int shouldClose;
        public void* userPointer;
        public int doublebuffer;
        public GLFWvidmode videoMode;
        public _GLFWmonitor* monitor;
        public _GLFWcursor* cursor;
        public byte* title;

        public int minwidth;
        public int minheight;
        public int maxwidth;
        public int maxheight;
        public int numer;
        public int denom;

        public int stickyKeys;
        public int stickyMouseButtons;
        public int lockKeyMods;
        public int cursorMode;
        public fixed byte mouseButtons[GLFW_MOUSE_BUTTON_LAST + 1];
        public fixed byte keys[GLFW_KEY_LAST + 1];
        public double virtualCursorPosX;
        public double virtualCursorPosY;
        public int rawMouseMotion;

        public _GLFWcontext context;
        public _GLFWwindow_callbacks callbacks;
        public _GLFWwindowNull @null;
        public _GLFWwindowWin32 win32;
        public _GLFWwindowX11 x11;
        public _GLFWwindowWayland wl;
        public _GLFWwindowNS ns;
    }

    public struct _GLFWwindow_callbacks
    {
        public delegate*<GLFWwindow*, int, int, void> pos;
        public delegate*<GLFWwindow*, int, int, void> size;
        public delegate*<GLFWwindow*, void> close;
        public delegate*<GLFWwindow*, void> refresh;
        public delegate*<GLFWwindow*, int, void> focus;
        public delegate*<GLFWwindow*, int, void> iconify;
        public delegate*<GLFWwindow*, int, void> maximize;
        public delegate*<GLFWwindow*, int, int, void> fbsize;
        public delegate*<GLFWwindow*, float, float, void> scale;
        public delegate*<GLFWwindow*, int, int, int, void> mouseButton;
        public delegate*<GLFWwindow*, double, double, void> cursorPos;
        public delegate*<GLFWwindow*, int, void> cursorEnter;
        public delegate*<GLFWwindow*, double, double, void> scroll;
        public delegate*<GLFWwindow*, int, int, int, int, void> key;
        public delegate*<GLFWwindow*, uint, void> character;
        public delegate*<GLFWwindow*, uint, int, void> charmods;
        public delegate*<GLFWwindow*, int, byte**, void> drop;
    }

    public struct _GLFWmonitor
    {
        public fixed byte name[128];
        public void* userPointer;
        public int widthMM;
        public int heightMM;
        public _GLFWwindow* window;
        public GLFWvidmode* modes;
        public int modeCount;
        public GLFWvidmode currentMode;
        public GLFWgammaramp originalRamp;
        public GLFWgammaramp currentRamp;
        public _GLFWmonitorNull @null;
        public _GLFWmonitorWin32 win32;
        public _GLFWmonitorX11 x11;
        public _GLFWmonitorWayland wl;
        public _GLFWmonitorNS ns;
    }

    public struct _GLFWcursor
    {
        public _GLFWcursor* next;
        public _GLFWcursorWin32 win32;
        public _GLFWcursorX11 x11;
        public _GLFWcursorWayland wl;
        public _GLFWcursorNS ns;
    }

    public struct _GLFWmapelement
    {
        public byte type;
        public byte index;
        public sbyte axisScale;
        public sbyte axisOffset;
    }

    public struct _GLFWmapping
    {
        public fixed byte name[128];
        public fixed byte guid[33];
        public _GLFWmapelementArray15 buttons;
        public _GLFWmapelementArray6 axes;
    }

    [InlineArray(15)]
    public struct _GLFWmapelementArray15
    {
        private _GLFWmapelement _element0;
    }

    [InlineArray(6)]
    public struct _GLFWmapelementArray6
    {
        private _GLFWmapelement _element0;
    }

    public struct _GLFWjoystick
    {
        public int allocated;
        public int connected;
        public float* axes;
        public int axisCount;
        public byte* buttons;
        public int buttonCount;
        public byte* hats;
        public int hatCount;
        public fixed byte name[128];
        public void* userPointer;
        public fixed byte guid[33];
        public _GLFWmapping* mapping;
        public _GLFWjoystickWin32 win32;
        public _GLFWjoystickNS ns;
        public _GLFWjoystickLinux linux;
    }

    [InlineArray(GLFW_JOYSTICK_LAST + 1)]
    public struct _GLFWjoystickArray
    {
        private _GLFWjoystick _element0;
    }

    public struct _GLFWtls
    {
        public int allocated;
        public uint index;
        public nuint key;
    }

    public struct _GLFWmutex
    {
        public int allocated;
        public CRITICAL_SECTION section;
        public PTHREAD_MUTEX_T posix;
    }

    public struct _GLFWplatform
    {
        public int platformID;

        public delegate*<int> init;
        public delegate*<void> terminate;

        public delegate*<_GLFWwindow*, double*, double*, void> getCursorPos;
        public delegate*<_GLFWwindow*, double, double, void> setCursorPos;
        public delegate*<_GLFWwindow*, int, void> setCursorMode;
        public delegate*<_GLFWwindow*, int, void> setRawMouseMotion;
        public delegate*<int> rawMouseMotionSupported;
        public delegate*<_GLFWcursor*, GLFWimage*, int, int, int> createCursor;
        public delegate*<_GLFWcursor*, int, int> createStandardCursor;
        public delegate*<_GLFWcursor*, void> destroyCursor;
        public delegate*<_GLFWwindow*, _GLFWcursor*, void> setCursor;
        public delegate*<int, byte*> getScancodeName;
        public delegate*<int, int> getKeyScancode;
        public delegate*<byte*, void> setClipboardString;
        public delegate*<byte*> getClipboardString;
        public delegate*<int> initJoysticks;
        public delegate*<void> terminateJoysticks;
        public delegate*<_GLFWjoystick*, int, int> pollJoystick;
        public delegate*<byte*> getMappingName;
        public delegate*<byte*, void> updateGamepadGUID;

        public delegate*<_GLFWmonitor*, void> freeMonitor;
        public delegate*<_GLFWmonitor*, int*, int*, void> getMonitorPos;
        public delegate*<_GLFWmonitor*, float*, float*, void> getMonitorContentScale;
        public delegate*<_GLFWmonitor*, int*, int*, int*, int*, void> getMonitorWorkarea;
        public delegate*<_GLFWmonitor*, int*, GLFWvidmode*> getVideoModes;
        public delegate*<_GLFWmonitor*, GLFWvidmode*, int> getVideoMode;
        public delegate*<_GLFWmonitor*, GLFWgammaramp*, int> getGammaRamp;
        public delegate*<_GLFWmonitor*, GLFWgammaramp*, void> setGammaRamp;

        public delegate*<_GLFWwindow*, _GLFWwndconfig*, _GLFWctxconfig*, _GLFWfbconfig*, int> createWindow;
        public delegate*<_GLFWwindow*, void> destroyWindow;
        public delegate*<_GLFWwindow*, byte*, void> setWindowTitle;
        public delegate*<_GLFWwindow*, int, GLFWimage*, void> setWindowIcon;
        public delegate*<_GLFWwindow*, int*, int*, void> getWindowPos;
        public delegate*<_GLFWwindow*, int, int, void> setWindowPos;
        public delegate*<_GLFWwindow*, int*, int*, void> getWindowSize;
        public delegate*<_GLFWwindow*, int, int, void> setWindowSize;
        public delegate*<_GLFWwindow*, int, int, int, int, void> setWindowSizeLimits;
        public delegate*<_GLFWwindow*, int, int, void> setWindowAspectRatio;
        public delegate*<_GLFWwindow*, int*, int*, void> getFramebufferSize;
        public delegate*<_GLFWwindow*, int*, int*, int*, int*, void> getWindowFrameSize;
        public delegate*<_GLFWwindow*, float*, float*, void> getWindowContentScale;
        public delegate*<_GLFWwindow*, void> iconifyWindow;
        public delegate*<_GLFWwindow*, void> restoreWindow;
        public delegate*<_GLFWwindow*, void> maximizeWindow;
        public delegate*<_GLFWwindow*, void> showWindow;
        public delegate*<_GLFWwindow*, void> hideWindow;
        public delegate*<_GLFWwindow*, void> requestWindowAttention;
        public delegate*<_GLFWwindow*, void> focusWindow;
        public delegate*<_GLFWwindow*, _GLFWmonitor*, int, int, int, int, int, void> setWindowMonitor;
        public delegate*<_GLFWwindow*, int> windowFocused;
        public delegate*<_GLFWwindow*, int> windowIconified;
        public delegate*<_GLFWwindow*, int> windowVisible;
        public delegate*<_GLFWwindow*, int> windowMaximized;
        public delegate*<_GLFWwindow*, int> windowHovered;
        public delegate*<_GLFWwindow*, int> framebufferTransparent;
        public delegate*<_GLFWwindow*, float> getWindowOpacity;
        public delegate*<_GLFWwindow*, int, void> setWindowResizable;
        public delegate*<_GLFWwindow*, int, void> setWindowDecorated;
        public delegate*<_GLFWwindow*, int, void> setWindowFloating;
        public delegate*<_GLFWwindow*, float, void> setWindowOpacity;
        public delegate*<_GLFWwindow*, int, void> setWindowMousePassthrough;
        public delegate*<void> pollEvents;
        public delegate*<void> waitEvents;
        public delegate*<double, void> waitEventsTimeout;
        public delegate*<void> postEmptyEvent;

        public delegate*<int**, int> getEGLPlatform;
        public delegate*<void*> getEGLNativeDisplay;
        public delegate*<_GLFWwindow*, void*> getEGLNativeWindow;

        public delegate*<void*> loadLocalVulkanLoader;
        public delegate*<byte**, void> getRequiredInstanceExtensions;
        public delegate*<void*, void*, uint, int> getPhysicalDevicePresentationSupport;
        public delegate*<void*, _GLFWwindow*, void*, ulong*, int> createWindowSurface;
    }

    public struct _GLFWlibrary
    {
        public int initialized;
        public GLFWallocator allocator;
        public _GLFWplatform platform;
        public _GLFWlibrary_hints hints;
        public _GLFWerror* errorListHead;
        public _GLFWcursor* cursorListHead;
        public _GLFWwindow* windowListHead;
        public _GLFWmonitor** monitors;
        public int monitorCount;
        public int joysticksInitialized;
        public _GLFWjoystickArray joysticks;
        public _GLFWmapping* mappings;
        public int mappingCount;
        public _GLFWtls errorSlot;
        public _GLFWtls contextSlot;
        public _GLFWmutex errorLock;
        public _GLFWlibrary_timer timer;
        public _GLFWlibrary_egl egl;
        public _GLFWlibrary_osmesa osmesa;
        public _GLFWlibrary_wgl wgl;
        public _GLFWlibrary_vk vk;
        public _GLFWlibrary_callbacks callbacks;
        public _GLFWlibraryNull @null;
        public _GLFWlibraryWin32 win32;
        public _GLFWlibraryX11 x11;
        public _GLFWlibraryWayland wl;
        public _GLFWlibraryGLX glx;
        public _GLFWlibraryNS ns;
        public _GLFWlibraryNSGL nsgl;
        public _GLFWlibraryLinuxJoystick linux_js;
    }

    public struct _GLFWwindowWayland
    {
        public int width;
        public int height;
        public int fbWidth;
        public int fbHeight;
        public int visible;
        public int maximized;
        public int activated;
        public int fullscreen;
        public int hovered;
        public int transparent;
        public int scaleFramebuffer;
        public void* surface;
        public void* callback;
        public _GLFWwindowWayland_egl egl;
        public _GLFWwindowWayland_pending pending;
        public _GLFWwindowWayland_xdg xdg;
        public _GLFWwindowWayland_libdecor libdecor;
        public _GLFWcursor* currentCursor;
        public double cursorPosX;
        public double cursorPosY;
        public byte* appId;
        public int bufferScale;
        public _GLFWscaleWayland* outputScales;
        public nuint outputScaleCount;
        public nuint outputScaleSize;
        public void* scalingViewport;
        public uint scalingNumerator;
        public void* fractionalScale;
        public void* relativePointer;
        public void* lockedPointer;
        public void* confinedPointer;
        public void* idleInhibitor;
        public void* activationToken;
        public _GLFWwindowWayland_fallback fallback;
    }

    public struct _GLFWwindowWayland_egl
    {
        public void* window;
    }

    public struct _GLFWwindowWayland_pending
    {
        public int width;
        public int height;
        public int maximized;
        public int iconified;
        public int activated;
        public int fullscreen;
    }

    public struct _GLFWwindowWayland_xdg
    {
        public void* surface;
        public void* toplevel;
        public void* decoration;
        public uint decorationMode;
    }

    public struct _GLFWwindowWayland_libdecor
    {
        public void* frame;
    }

    public struct _GLFWfallbackEdgeWayland
    {
        public void* surface;
        public void* subsurface;
        public void* viewport;
    }

    public struct _GLFWwindowWayland_fallback
    {
        public int decorations;
        public void* buffer;
        public _GLFWfallbackEdgeWayland top;
        public _GLFWfallbackEdgeWayland left;
        public _GLFWfallbackEdgeWayland right;
        public _GLFWfallbackEdgeWayland bottom;
        public void* focus;
    }

    public struct _GLFWofferWayland
    {
        public void* offer;
        public int text_plain_utf8;
        public int text_uri_list;
    }

    public struct _GLFWscaleWayland
    {
        public void* output;
        public int factor;
    }

    public struct _GLFWmonitorWayland
    {
        public void* output;
        public uint name;
        public int currentMode;
        public int x;
        public int y;
        public int scale;
    }

    public struct _GLFWcursorWayland
    {
        public void* cursor;
        public void* cursorHiDPI;
        public void* buffer;
        public int width;
        public int height;
        public int xhot;
        public int yhot;
        public int currentImage;
    }

    public struct _GLFWlibraryWayland
    {
        public void* display;
        public void* registry;
        public void* compositor;
        public void* subcompositor;
        public void* shm;
        public void* seat;
        public void* pointer;
        public void* keyboard;
        public void* dataDeviceManager;
        public void* dataDevice;
        public void* wmBase;
        public void* decorationManager;
        public void* viewporter;
        public void* relativePointerManager;
        public void* pointerConstraints;
        public void* idleInhibitManager;
        public void* activationManager;
        public void* fractionalScaleManager;
        public _GLFWofferWayland* offers;
        public uint offerCount;
        public void* selectionOffer;
        public void* selectionSource;
        public void* dragOffer;
        public _GLFWwindow* dragFocus;
        public uint dragSerial;
        public byte* tag;
        public void* cursorTheme;
        public void* cursorThemeHiDPI;
        public void* cursorSurface;
        public byte* cursorPreviousName;
        public int cursorTimerfd;
        public uint serial;
        public uint pointerEnterSerial;
        public int keyRepeatTimerfd;
        public int keyRepeatRate;
        public int keyRepeatDelay;
        public int keyRepeatScancode;
        public byte* clipboardString;
        public fixed short keycodes[256];
        public fixed short scancodes[GLFW_KEY_LAST + 1];
        public _GLFWlibraryWayland_xkb xkb;
        public _GLFWwindow* pointerFocus;
        public _GLFWwindow* keyboardFocus;
        public _GLFWlibraryWayland_client client;
        public _GLFWlibraryWayland_cursor cursor;
        public _GLFWlibraryWayland_egl egl;
        public _GLFWlibraryWayland_libdecor libdecor;
    }

    public struct _GLFWlibraryWayland_client
    {
        public void* handle;
        public delegate* unmanaged<byte*, void*> display_connect;
        public delegate* unmanaged<void*, void> display_disconnect;
        public delegate* unmanaged<void*, int> display_flush;
        public delegate* unmanaged<void*, int> display_dispatch_pending;
        public delegate* unmanaged<void*, int> display_roundtrip;
        public delegate* unmanaged<void*, int> display_get_fd;
        public delegate* unmanaged<void*, int> display_prepare_read;
        public delegate* unmanaged<void*, void> display_cancel_read;
        public delegate* unmanaged<void*, int> display_read_events;
        public delegate* unmanaged<void*, uint, void> proxy_marshal;
        public delegate* unmanaged<void*, uint, byte*, void> proxy_marshal_string;
        public delegate* unmanaged<void*, uint, uint, void> proxy_marshal_uint;
        public delegate* unmanaged<void*, uint, uint, void*, void> proxy_marshal_uint_object;
        public delegate* unmanaged<void*, uint, uint, byte*, void> proxy_marshal_uint_string;
        public delegate* unmanaged<void*, uint, int, void> proxy_marshal_int;
        public delegate* unmanaged<void*, uint, byte*, int, void> proxy_marshal_string_int;
        public delegate* unmanaged<void*, uint, byte*, void*, void> proxy_marshal_string_object;
        public delegate* unmanaged<void*, uint, void*, void> proxy_marshal_object;
        public delegate* unmanaged<void*, uint, void*, uint, void> proxy_marshal_object_uint;
        public delegate* unmanaged<void*, uint, void*, int, int, void> proxy_marshal_object_int_int;
        public delegate* unmanaged<void*, uint, uint, void*, int, int, void> proxy_marshal_uint_object_int_int;
        public delegate* unmanaged<void*, uint, int, int, void> proxy_marshal_int_int;
        public delegate* unmanaged<void*, uint, int, int, int, int, void> proxy_marshal_int_int_int_int;
        public delegate* unmanaged<void*, void*, void*, int> proxy_add_listener;
        public delegate* unmanaged<void*, void> proxy_destroy;
        public delegate* unmanaged<void*, uint, void*, void*, void*> proxy_marshal_constructor;
        public delegate* unmanaged<void*, uint, void*, void*, void*, void*> proxy_marshal_constructor_object;
        public delegate* unmanaged<void*, uint, void*, void*, void*, void*, void*, uint, void*> proxy_marshal_constructor_object_object_object_uint;
        public delegate* unmanaged<void*, uint, void*, void*, int, int, void*> proxy_marshal_constructor_int_int;
        public delegate* unmanaged<void*, uint, void*, void*, int, int, int, int, uint, void*> proxy_marshal_constructor_int_int_int_int_uint;
        public delegate* unmanaged<void*, uint, void*, uint, void*> proxy_marshal_constructor_versioned;
        public delegate* unmanaged<void*, uint, void*, uint, uint, byte*, uint, void*, void*> proxy_marshal_constructor_versioned_registry_bind;
        public delegate* unmanaged<void*, void*> proxy_get_user_data;
        public delegate* unmanaged<void*, void*, void> proxy_set_user_data;
        public delegate* unmanaged<void*, byte**> proxy_get_tag;
        public delegate* unmanaged<void*, byte**, void> proxy_set_tag;
        public delegate* unmanaged<void*, uint> proxy_get_version;
        public delegate* unmanaged<void*, uint, void*, uint, uint, void*> proxy_marshal_flags;
        public void* callbackInterface;
        public void* registryInterface;
        public void* compositorInterface;
        public void* subcompositorInterface;
        public void* shmInterface;
        public void* shmPoolInterface;
        public void* bufferInterface;
        public void* regionInterface;
        public void* seatInterface;
        public void* pointerInterface;
        public void* keyboardInterface;
        public void* outputInterface;
        public void* dataDeviceManagerInterface;
        public void* dataDeviceInterface;
        public void* dataOfferInterface;
        public void* dataSourceInterface;
        public void* surfaceInterface;
    }

    public struct _GLFWlibraryWayland_cursor
    {
        public void* handle;
        public delegate* unmanaged<byte*, int, void*, void*> theme_load;
        public delegate* unmanaged<void*, void> theme_destroy;
        public delegate* unmanaged<void*, byte*, void*> theme_get_cursor;
        public delegate* unmanaged<void*, void*> image_get_buffer;
    }

    public struct _GLFWlibraryWayland_egl
    {
        public void* handle;
        public delegate* unmanaged<void*, int, int, void*> window_create;
        public delegate* unmanaged<void*, void> window_destroy;
        public delegate* unmanaged<void*, int, int, int, int, void> window_resize;
    }

    public struct _GLFWlibraryWayland_xkb
    {
        public void* handle;
        public delegate* unmanaged<int, void*> context_new;
        public delegate* unmanaged<void*, void> context_unref;
        public delegate* unmanaged<void*, byte*, int, int, void*> keymap_new_from_string;
        public delegate* unmanaged<void*, void> keymap_unref;
        public delegate* unmanaged<void*, byte*, uint> keymap_mod_get_index;
        public delegate* unmanaged<void*, uint, int> keymap_key_repeats;
        public delegate* unmanaged<void*, uint, uint, uint, uint**, int> keymap_key_get_syms_by_level;
        public delegate* unmanaged<void*, void*> state_new;
        public delegate* unmanaged<void*, void> state_unref;
        public delegate* unmanaged<void*, uint, uint**, int> state_key_get_syms;
        public delegate* unmanaged<void*, uint, uint, uint, uint, uint, uint, int> state_update_mask;
        public delegate* unmanaged<void*, uint, uint> state_key_get_layout;
        public delegate* unmanaged<void*, uint, int, int> state_mod_index_is_active;
        public delegate* unmanaged<void*, byte*, int, void*> compose_table_new_from_locale;
        public delegate* unmanaged<void*, void> compose_table_unref;
        public delegate* unmanaged<void*, int, void*> compose_state_new;
        public delegate* unmanaged<void*, void> compose_state_unref;
        public delegate* unmanaged<void*, uint, int> compose_state_feed;
        public delegate* unmanaged<void*, int> compose_state_get_status;
        public delegate* unmanaged<void*, uint> compose_state_get_one_sym;
        public void* context;
        public void* keymap;
        public void* state;
        public void* composeState;
        public uint controlIndex;
        public uint altIndex;
        public uint shiftIndex;
        public uint superIndex;
        public uint capsLockIndex;
        public uint numLockIndex;
        public uint modifiers;
        public fixed byte keyname[64];
    }

    public struct _GLFWlibraryWayland_libdecor
    {
        public void* handle;
        public void* context;
        public void* callback;
        public int ready;
    }

    public struct _GLFWlibraryX11
    {
        public void* handle;
        public void* display;
        public int screen;
        public nuint root;
        public float contentScaleX;
        public float contentScaleY;
        public nuint helperWindowHandle;
        public nuint hiddenCursorHandle;
        public nuint context;
        public void* im;
        public delegate* unmanaged<void*, XErrorEvent*, int> errorHandler;
        public int errorCode;
        public byte* primarySelectionString;
        public byte* clipboardString;
        public _GLFWwindow* disabledCursorWindow;
        public double restoreCursorPosX;
        public double restoreCursorPosY;
        public void* xcursorHandle;
        public void* randrHandle;
        public int randrAvailable;
        public int randrMonitorBroken;
        public int randrGammaBroken;
        public int randrEventBase;
        public int randrErrorBase;
        public int randrMajor;
        public int randrMinor;
        public void* xineramaHandle;
        public int xineramaAvailable;
        public void* x11xcbHandle;
        public void* x11xcbConnection;
        public void* xshapeHandle;
        public void* xrenderHandle;
        public void* xiHandle;
        public int xiAvailable;
        public int xiMajorOpcode;
        public int xkbAvailable;
        public int xkbDetectable;
        public int xkbMajorOpcode;
        public int xkbEventBase;
        public int xkbErrorBase;
        public int xkbMajor;
        public int xkbMinor;
        public uint xkbGroup;
        public nuint WM_PROTOCOLS;
        public nuint WM_DELETE_WINDOW;
        public nuint NET_WM_PING;
        public nuint WM_STATE;
        public nuint NET_WM_NAME;
        public nuint NET_WM_ICON_NAME;
        public nuint NET_WM_ICON;
        public nuint NET_WM_STATE;
        public nuint NET_ACTIVE_WINDOW;
        public nuint NET_WM_STATE_ABOVE;
        public nuint NET_WM_STATE_DEMANDS_ATTENTION;
        public nuint NET_WM_STATE_MAXIMIZED_VERT;
        public nuint NET_WM_STATE_MAXIMIZED_HORZ;
        public nuint NET_WM_STATE_FULLSCREEN;
        public nuint NET_WM_FULLSCREEN_MONITORS;
        public nuint NET_WM_WINDOW_OPACITY;
        public nuint NET_WM_BYPASS_COMPOSITOR;
        public nuint NET_FRAME_EXTENTS;
        public nuint NET_REQUEST_FRAME_EXTENTS;
        public nuint NET_WORKAREA;
        public nuint NET_CURRENT_DESKTOP;
        public nuint MOTIF_WM_HINTS;
        public nuint PRIMARY;
        public nuint CLIPBOARD;
        public nuint CLIPBOARD_MANAGER;
        public nuint TARGETS;
        public nuint MULTIPLE;
        public nuint ATOM_PAIR;
        public nuint SAVE_TARGETS;
        public nuint TEXT;
        public nuint NULL_;
        public nuint INCR;
        public nuint GLFW_SELECTION;
        public nuint GLFW_EMPTY_EVENT;
        public nuint UTF8_STRING;
        public nuint XdndAware;
        public nuint XdndEnter;
        public nuint XdndPosition;
        public nuint XdndStatus;
        public nuint XdndActionCopy;
        public nuint XdndDrop;
        public nuint XdndFinished;
        public nuint XdndSelection;
        public nuint XdndTypeList;
        public nuint text_uri_list;
        public nuint xdndSource;
        public nuint xdndFormat;
        public int xdndVersion;
        public int saverTimeout;
        public int saverInterval;
        public int saverBlanking;
        public int saverExposure;
        public int saverCount;
        public fixed short keycodes[_GLFW_X11_KEYCODE_LAST + 1];
        public fixed short scancodes[GLFW_KEY_LAST + 1];
        public fixed byte keyname[64];
        public nuint NET_SUPPORTED;
        public nuint NET_SUPPORTING_WM_CHECK;
        public nuint NET_WM_PID;
        public nuint NET_WM_WINDOW_TYPE;
        public nuint NET_WM_WINDOW_TYPE_NORMAL;
        public delegate* unmanaged<void*, int> XCloseDisplay;
        public delegate* unmanaged<void*, int> XConnectionNumber;
        public delegate* unmanaged<void*, int> XFree;
        public delegate* unmanaged<void*, int> XDefaultScreen;
        public delegate* unmanaged<void*, int, nuint> XRootWindow;
        public delegate* unmanaged<void*, int, void*> XDefaultVisual;
        public delegate* unmanaged<void*, int, int> XDefaultDepth;
        public delegate* unmanaged<void*, int, int> XDisplayWidth;
        public delegate* unmanaged<void*, int, int> XDisplayHeight;
        public delegate* unmanaged<void*, int, int> XDisplayWidthMM;
        public delegate* unmanaged<void*, int, int> XDisplayHeightMM;
        public delegate* unmanaged<void*, int*, int*, int> XDisplayKeycodes;
        public delegate* unmanaged<void*, uint, int, int, nuint> XkbKeycodeToKeysym;
        public delegate* unmanaged<void*, uint, uint, XkbDescRec*> XkbGetMap;
        public delegate* unmanaged<void*, uint, XkbDescRec*, int> XkbGetNames;
        public delegate* unmanaged<XkbDescRec*, uint, int, void> XkbFreeNames;
        public delegate* unmanaged<XkbDescRec*, uint, int, void> XkbFreeKeyboard;
        public delegate* unmanaged<void*, int*, int*, int*, int*, int*, int> XkbQueryExtension;
        public delegate* unmanaged<void*, int, int*, int> XkbSetDetectableAutoRepeat;
        public delegate* unmanaged<void*, uint, XkbStateRec*, int> XkbGetState;
        public delegate* unmanaged<void*, uint, uint, ulong, ulong, int> XkbSelectEventDetails;
        public delegate* unmanaged<int, int, XcursorImage*> XcursorImageCreate;
        public delegate* unmanaged<XcursorImage*, void> XcursorImageDestroy;
        public delegate* unmanaged<void*, XcursorImage*, nuint> XcursorImageLoadCursor;
        public delegate* unmanaged<void*, nuint, XRRScreenResources*> XRRGetScreenResourcesCurrent;
        public delegate* unmanaged<XRRScreenResources*, void> XRRFreeScreenResources;
        public delegate* unmanaged<void*, XRRScreenResources*, nuint, XRROutputInfo*> XRRGetOutputInfo;
        public delegate* unmanaged<XRROutputInfo*, void> XRRFreeOutputInfo;
        public delegate* unmanaged<void*, XRRScreenResources*, nuint, XRRCrtcInfo*> XRRGetCrtcInfo;
        public delegate* unmanaged<XRRCrtcInfo*, void> XRRFreeCrtcInfo;
        public delegate* unmanaged<void*, nuint, nuint> XRRGetOutputPrimary;
        public delegate* unmanaged<void*, int*, int*, int> XRRQueryExtension;
        public delegate* unmanaged<void*, int*, int*, int> XRRQueryVersion;
        public delegate* unmanaged<void*, nuint, int, void> XRRSelectInput;
        public delegate* unmanaged<void*, XRRScreenResources*, nuint, ulong, int, int, nuint, ushort, nuint*, int, int> XRRSetCrtcConfig;
        public delegate* unmanaged<void*, nuint, int> XRRGetCrtcGammaSize;
        public delegate* unmanaged<void*, nuint, XRRCrtcGamma*> XRRGetCrtcGamma;
        public delegate* unmanaged<int, XRRCrtcGamma*> XRRAllocGamma;
        public delegate* unmanaged<XRRCrtcGamma*, void> XRRFreeGamma;
        public delegate* unmanaged<void*, nuint, XRRCrtcGamma*, void> XRRSetCrtcGamma;
        public delegate* unmanaged<XEvent*, int> XRRUpdateConfiguration;
        public delegate* unmanaged<void*, int*, int*, int> XineramaQueryExtension;
        public delegate* unmanaged<void*, int> XineramaIsActive;
        public delegate* unmanaged<void*, int*, XineramaScreenInfo*> XineramaQueryScreens;
        public delegate* unmanaged<void*, void*> XGetXCBConnection;
        public delegate* unmanaged<void*> XCreateRegion;
        public delegate* unmanaged<void*, int> XDestroyRegion;
        public delegate* unmanaged<void*, nuint, int, int, int, void*, int, void> XShapeCombineRegion;
        public delegate* unmanaged<void*, nuint, int, int, int, nuint, int, void> XShapeCombineMask;
        public delegate* unmanaged<void*, void*, XRenderPictFormat*> XRenderFindVisualFormat;
        public delegate* unmanaged<void*, byte*, int*, int*, int*, int> XQueryExtension;
        public delegate* unmanaged<void*, XEvent*, int> XGetEventData;
        public delegate* unmanaged<void*, XEvent*, void> XFreeEventData;
        public delegate* unmanaged<void*, int*, int*, int> XIQueryVersion;
        public delegate* unmanaged<void*, nuint, XIEventMask*, int, int> XISelectEvents;
        public delegate* unmanaged<void*, nuint, void*, int, nuint> XCreateColormap;
        public delegate* unmanaged<void*, nuint, int, int, uint, uint, uint, int, uint, void*, nuint, XSetWindowAttributes*, nuint> XCreateWindow;
        public delegate* unmanaged<void*, nuint, int, int, uint, uint, uint, ulong, ulong, nuint> XCreateSimpleWindow;
        public delegate* unmanaged<void*, nuint, int> XDestroyWindow;
        public delegate* unmanaged<void*, nuint, int> XFreeColormap;
        public delegate* unmanaged<void*, nuint, byte*, int> XStoreName;
        public delegate* unmanaged<void*, nuint, byte*, byte*, byte**, int, void*, void*, void*, void> Xutf8SetWMProperties;
        public delegate* unmanaged<void*, void*, byte*, byte*, void*> XOpenIM;
        public delegate* unmanaged<void*, int> XCloseIM;
        public delegate* unmanaged<void*, byte*, nuint, byte*, nuint, byte*, nuint, byte*, XIMCallback*, void*, void*> XCreateIC;
        public delegate* unmanaged<void*, void> XDestroyIC;
        public delegate* unmanaged<void*, byte*, XIMStyles**, void*, byte*> XGetIMValues;
        public delegate* unmanaged<void*, byte*, XIMCallback*, void*, byte*> XSetIMValues;
        public delegate* unmanaged<void*, byte*, ulong*, void*, byte*> XGetICValues;
        public delegate* unmanaged<void*, void> XSetICFocus;
        public delegate* unmanaged<void*, void> XUnsetICFocus;
        public delegate* unmanaged<void*, void*, byte*, byte*, delegate* unmanaged<void*, void*, void*, void>, void*, int> XRegisterIMInstantiateCallback;
        public delegate* unmanaged<void*, void*, byte*, byte*, delegate* unmanaged<void*, void*, void*, void>, void*, int> XUnregisterIMInstantiateCallback;
        public delegate* unmanaged<int> XSupportsLocale;
        public delegate* unmanaged<byte*, byte*> XSetLocaleModifiers;
        public delegate* unmanaged<void*, XEvent*, byte*, int, nuint*, int*, int> Xutf8LookupString;
        public delegate* unmanaged<XEvent*, nuint, int> XFilterEvent;
        public delegate* unmanaged<void*, nuint, nuint, nuint, int, int, byte*, int, int> XChangeProperty;
        public delegate* unmanaged<void*, nuint, nuint, nint, nint, int, nuint, nuint*, int*, nuint*, nuint*, byte**, int> XGetWindowProperty;
        public delegate* unmanaged<void*, nuint, nuint, int> XDeleteProperty;
        public delegate* unmanaged<void*, byte*, int, nuint> XInternAtom;
        public delegate* unmanaged<void*, nuint, nuint, nuint, nuint, ulong, int> XConvertSelection;
        public delegate* unmanaged<void*, nuint, nuint, ulong, int> XSetSelectionOwner;
        public delegate* unmanaged<void*, nuint, nuint> XGetSelectionOwner;
        public delegate* unmanaged<void*, nuint, int, nint, XEvent*, int> XSendEvent;
        public delegate* unmanaged<void*, nuint, XSizeHints*, void> XSetWMNormalHints;
        public delegate* unmanaged<void*, nuint, XWMHints*, void> XSetWMHints;
        public delegate* unmanaged<void*, nuint, nuint*, int, int> XSetWMProtocols;
        public delegate* unmanaged<void*, nuint, XClassHint*, int> XSetClassHint;
        public delegate* unmanaged<void*, nuint, nint, int> XSelectInput;
        public delegate* unmanaged<void*, nuint, int> XMapWindow;
        public delegate* unmanaged<void*, nuint, int> XUnmapWindow;
        public delegate* unmanaged<void*, nuint, int, int, int> XMoveWindow;
        public delegate* unmanaged<void*, nuint, uint, uint, int> XResizeWindow;
        public delegate* unmanaged<void*, nuint, int, int, uint, uint, int> XMoveResizeWindow;
        public delegate* unmanaged<void*, nuint, int> XRaiseWindow;
        public delegate* unmanaged<void*, nuint, int, ulong, int> XSetInputFocus;
        public delegate* unmanaged<void*, nuint, int, int> XIconifyWindow;
        public delegate* unmanaged<void*, nuint, nuint*, nuint*, int*, int*, int*, int*, uint*, int> XQueryPointer;
        public delegate* unmanaged<void*, nuint, nuint, int, int, int*, int*, nuint*, int> XTranslateCoordinates;
        public delegate* unmanaged<void*, nuint, nuint, int, int, uint, uint, int, int, int> XWarpPointer;
        public delegate* unmanaged<void*, nuint, int, uint, int, int, nuint, nuint, ulong, int> XGrabPointer;
        public delegate* unmanaged<void*, ulong, int> XUngrabPointer;
        public delegate* unmanaged<void*, uint, nuint> XCreateFontCursor;
        public delegate* unmanaged<void*, nuint> XVisualIDFromVisual;
        public delegate* unmanaged<void*, nint, XVisualInfo*, int*, XVisualInfo*> XGetVisualInfo;
        public delegate* unmanaged<void*, nuint, int> XFreeCursor;
        public delegate* unmanaged<void*, nuint, nuint, int> XDefineCursor;
        public delegate* unmanaged<void*, nuint, int> XUndefineCursor;
        public delegate* unmanaged<void*, int> XPending;
        public delegate* unmanaged<void*, int, int> XEventsQueued;
        public delegate* unmanaged<void*, XEvent*, int> XNextEvent;
        public delegate* unmanaged<void*, XEvent*, int> XPeekEvent;
        public delegate* unmanaged<XEvent*, byte*, int, nuint*, void*, int> XLookupString;
        public delegate* unmanaged<void*, int> XFlush;
        public delegate* unmanaged<void*, int, int> XSync;
        public delegate* unmanaged<void*, int, byte*, int, int> XGetErrorText;
        public delegate* unmanaged<delegate* unmanaged<void*, XErrorEvent*, int>, delegate* unmanaged<void*, XErrorEvent*, int>> XSetErrorHandler;
        public delegate* unmanaged<void*, int*, int*, int*, int*, int> XGetScreenSaver;
        public delegate* unmanaged<void*, int, int, int, int, int> XSetScreenSaver;
    }

    public struct _GLFWlibraryGLX
    {
        public int major;
        public int minor;
        public int eventBase;
        public int errorBase;
        public void* handle;
        public delegate* unmanaged<void*, int, int*, void**> GetFBConfigs;
        public delegate* unmanaged<void*, void*, int, int*, int> GetFBConfigAttrib;
        public delegate* unmanaged<void*, int, byte*> GetClientString;
        public delegate* unmanaged<void*, int*, int*, int> QueryExtension;
        public delegate* unmanaged<void*, int*, int*, int> QueryVersion;
        public delegate* unmanaged<void*, void*, void> DestroyContext;
        public delegate* unmanaged<void*, nuint, void*, int> MakeCurrent;
        public delegate* unmanaged<void*, nuint, void> SwapBuffers;
        public delegate* unmanaged<void*, int, byte*> QueryExtensionsString;
        public delegate* unmanaged<void*, void*, int, void*, int, void*> CreateNewContext;
        public delegate* unmanaged<void*, void*, XVisualInfo*> GetVisualFromFBConfig;
        public delegate* unmanaged<void*, void*, nuint, int*, nuint> CreateWindow;
        public delegate* unmanaged<void*, nuint, void> DestroyWindow;
        public delegate* unmanaged<byte*, void*> GetProcAddress;
        public delegate* unmanaged<byte*, void*> GetProcAddressARB;
        public delegate* unmanaged<int, int> SwapIntervalSGI;
        public delegate* unmanaged<void*, nuint, int, void> SwapIntervalEXT;
        public delegate* unmanaged<int, int> SwapIntervalMESA;
        public delegate* unmanaged<void*, void*, void*, int, int*, void*> CreateContextAttribsARB;
        public int SGI_swap_control;
        public int EXT_swap_control;
        public int MESA_swap_control;
        public int ARB_multisample;
        public int ARB_framebuffer_sRGB;
        public int EXT_framebuffer_sRGB;
        public int ARB_create_context;
        public int ARB_create_context_profile;
        public int ARB_create_context_robustness;
        public int EXT_create_context_es2_profile;
        public int ARB_create_context_no_error;
        public int ARB_context_flush_control;
    }

    public struct _GLFWwindowX11
    {
        public nuint colormap;
        public nuint handle;
        public nuint parent;
        public void* ic;
        public int overrideRedirect;
        public int visible;
        public int focused;
        public int iconified;
        public int maximized;
        public int transparent;
        public int width;
        public int height;
        public int xpos;
        public int ypos;
        public int lastCursorPosX;
        public int lastCursorPosY;
        public int warpCursorPosX;
        public int warpCursorPosY;
        public float opacity;
        public fixed ulong keyPressTimes[256];
    }

    public struct _GLFWmonitorX11
    {
        public nuint output;
        public nuint crtc;
        public nuint oldMode;
        public int index;
    }

    public struct _GLFWcursorX11
    {
        public nuint handle;
    }

    public struct XkbStateRec
    {
        public byte group;
        public byte locked_group;
        public ushort base_group;
        public ushort latched_group;
        public byte mods;
        public byte base_mods;
        public byte latched_mods;
        public byte locked_mods;
        public byte compat_state;
        public byte grab_mods;
        public byte compat_grab_mods;
        public byte lookup_mods;
        public byte compat_lookup_mods;
        public ushort ptr_buttons;
    }

    public struct XkbDescRec
    {
        public void* display;
        public ushort flags;
        public ushort device_spec;
        public byte min_key_code;
        public byte max_key_code;
        public void* ctrls;
        public void* server;
        public void* map;
        public void* indicators;
        public XkbNamesRec* names;
        public void* compat;
        public void* geom;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct XkbNamesRec
    {
        [FieldOffset(456)] public XkbKeyNameRec* keys;
        [FieldOffset(464)] public XkbKeyAliasRec* key_aliases;
        [FieldOffset(489)] public byte num_key_aliases;
    }

    public struct XkbKeyNameRec
    {
        public fixed byte name[4];
    }

    public struct XkbKeyAliasRec
    {
        public fixed byte real[4];
        public fixed byte alias[4];
    }

    public struct XcursorImage
    {
        public uint version;
        public uint size;
        public uint width;
        public uint height;
        public uint xhot;
        public uint yhot;
        public uint delay;
        public uint* pixels;
    }

    public struct XRRScreenResources
    {
        public ulong timestamp;
        public ulong configTimestamp;
        public int ncrtc;
        public nuint* crtcs;
        public int noutput;
        public nuint* outputs;
        public int nmode;
        public XRRModeInfo* modes;
    }

    public struct XRRModeInfo
    {
        public nuint id;
        public uint width;
        public uint height;
        public ulong dotClock;
        public uint hSyncStart;
        public uint hSyncEnd;
        public uint hTotal;
        public uint hSkew;
        public uint vSyncStart;
        public uint vSyncEnd;
        public uint vTotal;
        public byte* name;
        public uint nameLength;
        public ulong modeFlags;
    }

    public struct XRROutputInfo
    {
        public ulong timestamp;
        public nuint crtc;
        public byte* name;
        public int nameLen;
        public ulong mm_width;
        public ulong mm_height;
        public int connection;
        public int subpixel_order;
        public int ncrtc;
        public nuint* crtcs;
        public int nclone;
        public nuint* clones;
        public int nmode;
        public int npreferred;
        public nuint* modes;
    }

    public struct XRRCrtcInfo
    {
        public ulong timestamp;
        public int x;
        public int y;
        public uint width;
        public uint height;
        public nuint mode;
        public ushort rotation;
        public int noutput;
        public nuint* outputs;
        public ushort rotations;
        public int npossible;
        public nuint* possible;
    }

    public struct XRRCrtcGamma
    {
        public int size;
        public ushort* red;
        public ushort* green;
        public ushort* blue;
    }

    public struct XRenderDirectFormat
    {
        public short red;
        public short redMask;
        public short green;
        public short greenMask;
        public short blue;
        public short blueMask;
        public short alpha;
        public short alphaMask;
    }

    public struct XRenderPictFormat
    {
        public nuint id;
        public int type;
        public int depth;
        public XRenderDirectFormat direct;
        public nuint colormap;
    }

    public struct XIEventMask
    {
        public int deviceid;
        public int mask_len;
        public byte* mask;
    }

    public struct XIValuatorState
    {
        public int mask_len;
        public byte* mask;
        public double* values;
    }

    public struct XIRawEvent
    {
        public int type;
        public ulong serial;
        public int send_event;
        public void* display;
        public int extension;
        public int evtype;
        public ulong time;
        public int deviceid;
        public int sourceid;
        public int detail;
        public int flags;
        public XIValuatorState valuators;
        public double* raw_values;
    }

    public struct XVisualInfo
    {
        public void* visual;
        public nuint visualid;
        public int screen;
        public int depth;
        public int @class;
        public nuint red_mask;
        public nuint green_mask;
        public nuint blue_mask;
        public int colormap_size;
        public int bits_per_rgb;
    }

    public struct XSetWindowAttributes
    {
        public nuint background_pixmap;
        public nuint background_pixel;
        public nuint border_pixmap;
        public nuint border_pixel;
        public int bit_gravity;
        public int win_gravity;
        public int backing_store;
        public nuint backing_planes;
        public nuint backing_pixel;
        public int save_under;
        public nint event_mask;
        public nint do_not_propagate_mask;
        public int override_redirect;
        public nuint colormap;
        public nuint cursor;
    }

    public struct XSizeHints
    {
        public nint flags;
        public int x;
        public int y;
        public int width;
        public int height;
        public int min_width;
        public int min_height;
        public int max_width;
        public int max_height;
        public int width_inc;
        public int height_inc;
        public int min_aspect_x;
        public int min_aspect_y;
        public int max_aspect_x;
        public int max_aspect_y;
        public int base_width;
        public int base_height;
        public int win_gravity;
    }

    public struct XWMHints
    {
        public nint flags;
        public int input;
        public int initial_state;
        public nuint icon_pixmap;
        public nuint icon_window;
        public int icon_x;
        public int icon_y;
        public nuint icon_mask;
        public nuint window_group;
    }

    public struct XErrorEvent
    {
        public int type;
        public void* display;
        public nuint resourceid;
        public nuint serial;
        public byte error_code;
        public byte request_code;
        public byte minor_code;
    }

    public struct XIMStyles
    {
        public ushort count_styles;
        public nuint* supported_styles;
    }

    public struct XIMCallback
    {
        public void* client_data;
        public void* callback;
    }

    [StructLayout(LayoutKind.Explicit, Size = 192)]
    public struct XEvent
    {
        [FieldOffset(0)] public int type;
        [FieldOffset(16)] public int send_event;
        [FieldOffset(24)] public void* display;
        [FieldOffset(32)] public nuint anyWindow;

        [FieldOffset(40)] public nuint configureWindow;
        [FieldOffset(48)] public int configureX;
        [FieldOffset(52)] public int configureY;
        [FieldOffset(56)] public int configureWidth;
        [FieldOffset(60)] public int configureHeight;

        [FieldOffset(40)] public nuint reparentWindow;
        [FieldOffset(48)] public nuint reparentParent;

        [FieldOffset(40)] public int exposeX;
        [FieldOffset(44)] public int exposeY;
        [FieldOffset(48)] public int exposeWidth;
        [FieldOffset(52)] public int exposeHeight;

        [FieldOffset(40)] public nuint clientMessageType;
        [FieldOffset(48)] public int clientFormat;
        [FieldOffset(56)] public nint clientData0;
        [FieldOffset(64)] public nint clientData1;
        [FieldOffset(72)] public nint clientData2;
        [FieldOffset(80)] public nint clientData3;
        [FieldOffset(88)] public nint clientData4;

        [FieldOffset(56)] public ulong time;
        [FieldOffset(64)] public int x;
        [FieldOffset(68)] public int y;
        [FieldOffset(72)] public int x_root;
        [FieldOffset(76)] public int y_root;
        [FieldOffset(80)] public uint state;
        [FieldOffset(84)] public uint keycode;
        [FieldOffset(84)] public uint button;

        [FieldOffset(40)] public int focusMode;
        [FieldOffset(44)] public int focusDetail;

        [FieldOffset(40)] public nuint propertyAtom;
        [FieldOffset(56)] public int propertyState;

        [FieldOffset(32)] public int genericExtension;
        [FieldOffset(36)] public int genericEvType;
        [FieldOffset(40)] public uint genericCookie;
        [FieldOffset(48)] public void* genericData;

        [FieldOffset(40)] public int xkbType;
        [FieldOffset(48)] public uint xkbStateChanged;
        [FieldOffset(52)] public int xkbStateGroup;

        [FieldOffset(32)] public nuint selectionRequestOwner;
        [FieldOffset(40)] public nuint selectionRequestor;
        [FieldOffset(48)] public nuint selectionRequestSelection;
        [FieldOffset(56)] public nuint selectionRequestTarget;
        [FieldOffset(64)] public nuint selectionRequestProperty;
        [FieldOffset(72)] public ulong selectionRequestTime;

        [FieldOffset(32)] public nuint selectionNotifyRequestor;
        [FieldOffset(40)] public nuint selectionNotifySelection;
        [FieldOffset(48)] public nuint selectionNotifyTarget;
        [FieldOffset(56)] public nuint selectionNotifyProperty;
        [FieldOffset(64)] public ulong selectionNotifyTime;
    }

    public struct XineramaScreenInfo
    {
        public int screen_number;
        public short x_org;
        public short y_org;
        public short width;
        public short height;
    }

    public struct XClassHint
    {
        public byte* res_name;
        public byte* res_class;
    }

    public struct _GLFWlibraryLinuxJoystick
    {
        public int inotify;
        public int watch;
        public int dropped;
    }

    public struct _GLFWlibrary_hints
    {
        public _GLFWinitconfig init;
        public _GLFWfbconfig framebuffer;
        public _GLFWwndconfig window;
        public _GLFWctxconfig context;
        public int refreshRate;
    }

    public struct _GLFWlibrary_timer
    {
        public ulong offset;
        public ulong frequency;
        public int posixClock;
    }

    public struct _GLFWlibrary_egl
    {
        public int platform;
        public void* display;
        public int major;
        public int minor;
        public int prefix;
        public int ANGLE_platform_angle;
        public int ANGLE_platform_angle_opengl;
        public int ANGLE_platform_angle_d3d;
        public int ANGLE_platform_angle_vulkan;
        public int ANGLE_platform_angle_metal;
        public int EXT_platform_base;
        public int EXT_platform_x11;
        public int EXT_platform_wayland;
        public int EXT_client_extensions;
        public int KHR_create_context;
        public int KHR_create_context_no_error;
        public int KHR_gl_colorspace;
        public int KHR_get_all_proc_addresses;
        public int KHR_context_flush_control;
        public int EXT_present_opaque;
        public void* handle;
        public delegate* unmanaged<void*, void*, int, int*, int> GetConfigAttrib;
        public delegate* unmanaged<void*, void**, int, int*, int> GetConfigs;
        public delegate* unmanaged<void*, void*> GetDisplay;
        public delegate* unmanaged<int> GetError;
        public delegate* unmanaged<void*, int*, int*, int> Initialize;
        public delegate* unmanaged<void*, int> Terminate;
        public delegate* unmanaged<int, int> BindAPI;
        public delegate* unmanaged<void*, void*, void*, int*, void*> CreateContext;
        public delegate* unmanaged<void*, void*, int> DestroySurface;
        public delegate* unmanaged<void*, void*, int> DestroyContext;
        public delegate* unmanaged<void*, void*, void*, int*, void*> CreateWindowSurface;
        public delegate* unmanaged<void*, void*, void*, void*, int> MakeCurrent;
        public delegate* unmanaged<void*, void*, int> SwapBuffers;
        public delegate* unmanaged<void*, int, int> SwapInterval;
        public delegate* unmanaged<void*, int, byte*> QueryString;
        public delegate* unmanaged<byte*, void*> GetProcAddress;
        public delegate* unmanaged<int, void*, int*, void*> GetPlatformDisplayEXT;
        public delegate* unmanaged<void*, void*, void*, int*, void*> CreatePlatformWindowSurfaceEXT;
    }

    public struct _GLFWlibrary_osmesa
    {
        public void* handle;
        public delegate* unmanaged<int, int, int, int, void*, void*> CreateContextExt;
        public delegate* unmanaged<int*, void*, void*> CreateContextAttribs;
        public delegate* unmanaged<void*, void> DestroyContext;
        public delegate* unmanaged<void*, void*, uint, int, int, int> MakeCurrent;
        public delegate* unmanaged<void*, int*, int*, int*, void**, int> GetColorBuffer;
        public delegate* unmanaged<void*, int*, int*, int*, void**, int> GetDepthBuffer;
        public delegate* unmanaged<byte*, void*> GetProcAddress;
    }

    public struct _GLFWlibrary_wgl
    {
        public void* instance;
        public delegate* unmanaged<nint, nint> CreateContext;
        public delegate* unmanaged<nint, int> DeleteContext;
        public delegate* unmanaged<byte*, void*> GetProcAddress;
        public delegate* unmanaged<nint> GetCurrentDC;
        public delegate* unmanaged<nint> GetCurrentContext;
        public delegate* unmanaged<nint, nint, int> MakeCurrent;
        public delegate* unmanaged<nint, nint, int> ShareLists;

        public delegate* unmanaged<int, int> SwapIntervalEXT;
        public delegate* unmanaged<nint, int, int, uint, int*, int*, int> GetPixelFormatAttribivARB;
        public delegate* unmanaged<byte*> GetExtensionsStringEXT;
        public delegate* unmanaged<nint, byte*> GetExtensionsStringARB;
        public delegate* unmanaged<nint, nint, int*, nint> CreateContextAttribsARB;

        public int EXT_swap_control;
        public int EXT_colorspace;
        public int ARB_multisample;
        public int ARB_framebuffer_sRGB;
        public int EXT_framebuffer_sRGB;
        public int ARB_pixel_format;
        public int ARB_create_context;
        public int ARB_create_context_profile;
        public int EXT_create_context_es2_profile;
        public int ARB_create_context_robustness;
        public int ARB_create_context_no_error;
        public int ARB_context_flush_control;
    }

    public struct _GLFWlibrary_vk
    {
        public int available;
        public void* handle;
        public byte** extensions;
        public delegate*<void*, byte*, void*> GetInstanceProcAddr;
        public delegate* unmanaged<void*, byte*, void*> GetInstanceProcAddrNative;
        public int KHR_surface;
        public int KHR_win32_surface;
        public int MVK_macos_surface;
        public int EXT_metal_surface;
        public int KHR_xlib_surface;
        public int KHR_xcb_surface;
        public int KHR_wayland_surface;
    }

    public struct _GLFWlibrary_callbacks
    {
        public delegate*<GLFWmonitor*, int, void> monitor;
        public delegate*<int, int, void> joystick;
    }

    static byte* _glfw_allocate_static_string(string value)
    {
        var byteCount = Encoding.UTF8.GetByteCount(value);
        var result = (byte*)NativeMemory.Alloc((nuint)byteCount + 1);
        fixed (char* chars = value)
            Encoding.UTF8.GetBytes(chars, value.Length, result, byteCount);
        result[byteCount] = 0;
        return result;
    }

    static int _glfw_strlen(byte* source)
    {
        var length = 0;
        while (source[length] != 0)
            length++;
        return length;
    }

    static void _glfw_strcpy(byte* destination, byte* source)
    {
        while ((*destination++ = *source++) != 0)
        {
        }
    }

    static void _glfw_strcpy(byte* destination, string source)
    {
        var byteCount = Encoding.UTF8.GetByteCount(source);
        fixed (char* chars = source)
            Encoding.UTF8.GetBytes(chars, source.Length, destination, Math.Min(byteCount, _GLFW_MESSAGE_SIZE - 1));
        destination[Math.Min(byteCount, _GLFW_MESSAGE_SIZE - 1)] = 0;
    }

    static int _glfw_strncmp(byte* first, byte* second, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var diff = first[i] - second[i];
            if (diff != 0 || first[i] == 0 || second[i] == 0)
                return diff;
        }

        return 0;
    }

    static void _glfw_strncpy(byte* destination, byte* source, nuint size)
    {
        if (size == 0)
            return;

        nuint i = 0;
        for (; i + 1 < size && source[i] != 0; i++)
            destination[i] = source[i];

        destination[i] = 0;
    }

    static void _glfw_memset(void* destination, int value, nuint count)
    {
        NativeMemory.Fill(destination, count, (byte)value);
    }

    static void* _glfw_memcpy(void* destination, void* source, nuint count)
    {
        Buffer.MemoryCopy(source, destination, count, count);
        return destination;
    }
}
