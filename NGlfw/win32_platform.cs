using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const string _GLFW_WIN32_WINDOW_CLASS = "NGlfw30";
    const string _GLFW_WIN32_HELPER_WINDOW_CLASS = "NGlfw3 Helper";

    const uint CS_VREDRAW = 0x0001;
    const uint CS_HREDRAW = 0x0002;
    const uint CS_OWNDC = 0x0020;

    const uint WS_POPUP = 0x80000000;
    const uint WS_CAPTION = 0x00C00000;
    const uint WS_SYSMENU = 0x00080000;
    const uint WS_THICKFRAME = 0x00040000;
    const uint WS_MINIMIZEBOX = 0x00020000;
    const uint WS_MAXIMIZEBOX = 0x00010000;
    const uint WS_CLIPSIBLINGS = 0x04000000;
    const uint WS_CLIPCHILDREN = 0x02000000;
    const uint WS_MAXIMIZE = 0x01000000;
    const uint WS_VISIBLE = 0x10000000;

    const uint WS_EX_TOPMOST = 0x00000008;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    const uint WS_EX_APPWINDOW = 0x00040000;
    const uint WS_EX_LAYERED = 0x00080000;

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_NOACTIVATE = 0x0010;
    const uint SWP_FRAMECHANGED = 0x0020;
    const uint SWP_SHOWWINDOW = 0x0040;
    const uint SWP_NOCOPYBITS = 0x0100;
    const uint SWP_NOOWNERZORDER = 0x0200;

    const int SW_HIDE = 0;
    const int SW_SHOWNORMAL = 1;
    const int SW_SHOWMINIMIZED = 2;
    const int SW_SHOWMAXIMIZED = 3;
    const int SW_SHOWNOACTIVATE = 4;
    const int SW_RESTORE = 9;
    const int SW_SHOWDEFAULT = 10;
    const int SW_FORCEMINIMIZE = 11;

    const int CW_USEDEFAULT = unchecked((int)0x80000000);

    const uint PM_NOREMOVE = 0x0000;
    const uint PM_REMOVE = 0x0001;
    const uint QS_ALLINPUT = 0x04ff;

    const uint WM_NULL = 0x0000;
    const uint WM_CREATE = 0x0001;
    const uint WM_NCCREATE = 0x0081;
    const uint WM_NCACTIVATE = 0x0086;
    const uint WM_NCPAINT = 0x0085;
    const uint WM_DESTROY = 0x0002;
    const uint WM_MOVE = 0x0003;
    const uint WM_SIZE = 0x0005;
    const uint WM_SETFOCUS = 0x0007;
    const uint WM_KILLFOCUS = 0x0008;
    const uint WM_ERASEBKGND = 0x0014;
    const uint WM_CLOSE = 0x0010;
    const uint WM_GETMINMAXINFO = 0x0024;
    const uint WM_PAINT = 0x000f;
    const uint WM_SETCURSOR = 0x0020;
    const uint WM_COPYDATA = 0x004a;
    const uint WM_MOUSEMOVE = 0x0200;
    const uint WM_MOUSEACTIVATE = 0x0021;
    const uint WM_LBUTTONDOWN = 0x0201;
    const uint WM_LBUTTONUP = 0x0202;
    const uint WM_RBUTTONDOWN = 0x0204;
    const uint WM_RBUTTONUP = 0x0205;
    const uint WM_MBUTTONDOWN = 0x0207;
    const uint WM_MBUTTONUP = 0x0208;
    const uint WM_MOUSEWHEEL = 0x020a;
    const uint WM_XBUTTONDOWN = 0x020b;
    const uint WM_XBUTTONUP = 0x020c;
    const uint WM_MOUSEHWHEEL = 0x020e;
    const uint WM_SIZING = 0x0214;
    const uint WM_KEYDOWN = 0x0100;
    const uint WM_KEYUP = 0x0101;
    const uint WM_CHAR = 0x0102;
    const uint WM_SYSKEYDOWN = 0x0104;
    const uint WM_SYSKEYUP = 0x0105;
    const uint WM_SYSCHAR = 0x0106;
    const uint WM_UNICHAR = 0x0109;
    const uint WM_INPUT = 0x00ff;
    const uint WM_SYSCOMMAND = 0x0112;
    const uint WM_CAPTURECHANGED = 0x0215;
    const uint WM_INPUTLANGCHANGE = 0x0051;
    const uint WM_ENTERMENULOOP = 0x0211;
    const uint WM_EXITMENULOOP = 0x0212;
    const uint WM_ENTERSIZEMOVE = 0x0231;
    const uint WM_EXITSIZEMOVE = 0x0232;
    const uint WM_MOUSELEAVE = 0x02a3;
    const uint WM_DROPFILES = 0x0233;
    const uint WM_DPICHANGED = 0x02e0;
    const uint WM_GETDPISCALEDSIZE = 0x02e4;
    const uint WM_COPYGLOBALDATA = 0x0049;
    const uint WM_QUIT = 0x0012;
    const uint WM_USER = 0x0400;
    const uint WM_DISPLAYCHANGE = 0x007e;
    const uint WM_DEVICECHANGE = 0x0219;
    const uint WM_DWMCOMPOSITIONCHANGED = 0x031e;
    const uint WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

    const int SIZE_RESTORED = 0;
    const int SIZE_MINIMIZED = 1;
    const int SIZE_MAXIMIZED = 2;

    const int HTCLIENT = 1;
    const int UNICODE_NOCHAR = 0xffff;
    const uint MSGFLT_ALLOW = 1;
    const nuint SC_SCREENSAVE = 0xf140;
    const nuint SC_MONITORPOWER = 0xf170;
    const nuint SC_KEYMENU = 0xf100;

    const int WMSZ_LEFT = 1;
    const int WMSZ_RIGHT = 2;
    const int WMSZ_TOP = 3;
    const int WMSZ_TOPLEFT = 4;
    const int WMSZ_TOPRIGHT = 5;
    const int WMSZ_BOTTOM = 6;
    const int WMSZ_BOTTOMLEFT = 7;
    const int WMSZ_BOTTOMRIGHT = 8;

    const int VK_SHIFT = 0x10;
    const int VK_CONTROL = 0x11;
    const int VK_MENU = 0x12;
    const int VK_SNAPSHOT = 0x2c;
    const int VK_LSHIFT = 0xa0;
    const int VK_RSHIFT = 0xa1;
    const int VK_PROCESSKEY = 0xe5;
    const int VK_CAPITAL = 0x14;
    const int VK_LWIN = 0x5b;
    const int VK_RWIN = 0x5c;
    const int VK_NUMLOCK = 0x90;

    const int IDC_ARROW = 32512;
    const int IDC_IBEAM = 32513;
    const int IDC_CROSS = 32515;
    const int IDC_HAND = 32649;
    const int IDC_SIZEWE = 32644;
    const int IDC_SIZENS = 32645;
    const int IDC_SIZENWSE = 32642;
    const int IDC_SIZENESW = 32643;
    const int IDC_SIZEALL = 32646;
    const int IDC_NO = 32648;
    const int IDI_APPLICATION = 32512;

    const int GWL_STYLE = -16;
    const int GWL_EXSTYLE = -20;

    const byte AC_SRC_ALPHA = 1;
    const uint LWA_ALPHA = 0x00000002;
    const uint DWM_BB_ENABLE = 0x00000001;
    const uint DWM_BB_BLURREGION = 0x00000002;
    const uint GMEM_MOVEABLE = 0x0002;
    const uint CF_UNICODETEXT = 13;
    const uint BI_BITFIELDS = 3;
    const uint DIB_RGB_COLORS = 0;
    const uint PFD_DOUBLEBUFFER = 0x00000001;
    const uint PFD_STEREO = 0x00000002;
    const uint PFD_DRAW_TO_WINDOW = 0x00000004;
    const uint PFD_SUPPORT_OPENGL = 0x00000020;
    const uint PFD_GENERIC_FORMAT = 0x00000040;
    const uint PFD_GENERIC_ACCELERATED = 0x00001000;
    const byte PFD_TYPE_RGBA = 0;
    const byte PFD_MAIN_PLANE = 0;
    const int IMAGE_ICON = 1;
    const int IMAGE_CURSOR = 2;
    const uint ICON_BIG = 1;
    const uint ICON_SMALL = 0;
    const uint WM_SETICON = 0x0080;
    const uint TME_LEAVE = 0x00000002;
    const int SM_CXICON = 11;
    const int SM_CYICON = 12;
    const int SM_CXCURSOR = 13;
    const int SM_CYCURSOR = 14;
    const int SM_CXSMICON = 49;
    const int SM_CYSMICON = 50;
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;
    const int SM_XVIRTUALSCREEN = 76;
    const int SM_YVIRTUALSCREEN = 77;
    const int SM_CXVIRTUALSCREEN = 78;
    const int SM_CYVIRTUALSCREEN = 79;
    const uint LR_DEFAULTSIZE = 0x00000040;
    const uint LR_SHARED = 0x00008000;
    const uint RID_INPUT = 0x10000003;
    const uint RIDEV_REMOVE = 0x00000001;
    const uint RIM_TYPEMOUSE = 0;
    const uint RIM_TYPEHID = 2;
    const uint RIDI_DEVICENAME = 0x20000007;
    const uint RIDI_DEVICEINFO = 0x2000000b;
    const int MAPVK_VK_TO_VSC = 0;
    const ushort MOUSE_MOVE_ABSOLUTE = 0x0001;
    const ushort MOUSE_VIRTUAL_DESKTOP = 0x0002;
    const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
    const uint USER_DEFAULT_SCREEN_DPI = 96;
    const int MDT_EFFECTIVE_DPI = 0;
    const int LOGPIXELSX = 88;
    const int LOGPIXELSY = 90;
    const int S_OK = 0;
    const uint TLS_OUT_OF_INDEXES = 0xffffffff;
    const uint DBT_DEVICEARRIVAL = 0x8000;
    const uint DBT_DEVICEREMOVECOMPLETE = 0x8004;
    const uint DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
    const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
    const uint KF_EXTENDED = 0x0100;
    const uint KF_UP = 0x8000;

    static readonly nint HWND_TOP = 0;
    static readonly nint HWND_TOPMOST = -1;
    static readonly nint HWND_NOTOPMOST = -2;

    static readonly WNDPROC win32_windowProcDelegate = win32_windowProc;
    static readonly WNDPROC win32_helperWindowProcDelegate = win32_helperWindowProc;
    static readonly Dictionary<nint, nint> win32_windows = new();
    static readonly object win32_windowLock = new();

    public struct _GLFWwindowWin32
    {
        public nint handle;
        public nint bigIcon;
        public nint smallIcon;
        public int cursorTracked;
        public int frameAction;
        public int iconified;
        public int maximized;
        public int transparent;
        public int scaleToMonitor;
        public int keymenu;
        public int showDefault;
        public int width;
        public int height;
        public int lastCursorPosX;
        public int lastCursorPosY;
        public char highSurrogate;
    }

    public struct _GLFWlibraryWin32
    {
        public nint instance;
        public nint helperWindowHandle;
        public ushort helperWindowClass;
        public ushort mainWindowClass;
        public nint deviceNotificationHandle;
        public uint mainThreadId;
        public byte* clipboardString;
        public _GLFWwindow* disabledCursorWindow;
        public _GLFWwindow* capturedCursorWindow;
        public double restoreCursorPosX;
        public double restoreCursorPosY;
        public nint blankCursor;
        public _GLFWlibraryWin32_dinput8 dinput8;
        public _GLFWlibraryWin32_xinput xinput;
        public fixed short keycodes[512];
        public fixed short scancodes[GLFW_KEY_LAST + 1];
        public fixed byte keyname[64];
    }

    public struct _GLFWmonitorWin32
    {
        public nint handle;
        public fixed char adapterName[32];
        public fixed char displayName[32];
        public fixed byte publicAdapterName[32];
        public fixed byte publicDisplayName[32];
        public int modesPruned;
        public int modeChanged;
    }

    public struct _GLFWcursorWin32
    {
        public nint handle;
        public int standard;
    }

    public struct _GLFWjoystickWin32
    {
        public _GLFWjoyobjectWin32* objects;
        public int objectCount;
        public void* device;
        public uint index;
        public Guid guid;
    }

    public struct _GLFWjoyobjectWin32
    {
        public int offset;
        public int type;
    }

    public struct _GLFWlibraryWin32_dinput8
    {
        public void* instance;
        public delegate* unmanaged[Stdcall]<nint, uint, Guid*, void**, void*, int> Create;
        public void* api;
    }

    public struct _GLFWlibraryWin32_xinput
    {
        public void* instance;
        public delegate* unmanaged<uint, uint, XINPUT_CAPABILITIES*, uint> GetCapabilities;
        public delegate* unmanaged<uint, XINPUT_STATE*, uint> GetState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_GAMEPAD
    {
        public ushort wButtons;
        public byte bLeftTrigger;
        public byte bRightTrigger;
        public short sThumbLX;
        public short sThumbLY;
        public short sThumbRX;
        public short sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_STATE
    {
        public uint dwPacketNumber;
        public XINPUT_GAMEPAD Gamepad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_VIBRATION
    {
        public ushort wLeftMotorSpeed;
        public ushort wRightMotorSpeed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_CAPABILITIES
    {
        public byte Type;
        public byte SubType;
        public ushort Flags;
        public XINPUT_GAMEPAD Gamepad;
        public XINPUT_VIBRATION Vibration;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CRITICAL_SECTION
    {
        public nint DebugInfo;
        public int LockCount;
        public int RecursionCount;
        public nint OwningThread;
        public nint LockSemaphore;
        public nuint SpinCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTDEVICELIST
    {
        public nint hDevice;
        public uint dwType;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RID_DEVICE_INFO
    {
        public uint cbSize;
        public uint dwType;
        public RID_DEVICE_INFO_HID hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RID_DEVICE_INFO_HID
    {
        public uint dwVendorId;
        public uint dwProductId;
        public uint dwVersionNumber;
        public ushort usUsagePage;
        public ushort usUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIOBJECTDATAFORMAT
    {
        public Guid* pguid;
        public uint dwOfs;
        public uint dwType;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIDATAFORMAT
    {
        public uint dwSize;
        public uint dwObjSize;
        public uint dwFlags;
        public uint dwDataSize;
        public uint dwNumObjs;
        public DIOBJECTDATAFORMAT* rgodf;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIPROPHEADER
    {
        public uint dwSize;
        public uint dwHeaderSize;
        public uint dwObj;
        public uint dwHow;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIPROPDWORD
    {
        public DIPROPHEADER diph;
        public uint dwData;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIPROPRANGE
    {
        public DIPROPHEADER diph;
        public int lMin;
        public int lMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIDEVCAPS
    {
        public uint dwSize;
        public uint dwFlags;
        public uint dwDevType;
        public uint dwAxes;
        public uint dwButtons;
        public uint dwPOVs;
        public uint dwFFSamplePeriod;
        public uint dwFFMinTimeResolution;
        public uint dwFirmwareRevision;
        public uint dwHardwareRevision;
        public uint dwFFDriverVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DIJOYSTATE
    {
        public int lX;
        public int lY;
        public int lZ;
        public int lRx;
        public int lRy;
        public int lRz;
        public fixed int rglSlider[2];
        public fixed uint rgdwPOV[4];
        public fixed byte rgbButtons[32];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct DIDEVICEINSTANCEW
    {
        public uint dwSize;
        public Guid guidInstance;
        public Guid guidProduct;
        public uint dwDevType;
        public fixed char tszInstanceName[260];
        public fixed char tszProductName[260];
        public Guid guidFFDriver;
        public ushort wUsagePage;
        public ushort wUsage;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct DIDEVICEOBJECTINSTANCEW
    {
        public uint dwSize;
        public Guid guidType;
        public uint dwOfs;
        public uint dwType;
        public uint dwFlags;
        public fixed char tszName[260];
        public uint dwFFMaxForce;
        public uint dwFFForceResolution;
        public ushort wCollectionNumber;
        public ushort wDesignatorIndex;
        public ushort wUsagePage;
        public ushort wUsage;
        public uint dwDimension;
        public ushort wExponent;
        public ushort wReportId;
    }

    delegate nint WNDPROC(nint hWnd, uint uMsg, nuint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MSG
    {
        public nint hwnd;
        public uint message;
        public nuint wParam;
        public nint lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CREATESTRUCTW
    {
        public void* lpCreateParams;
        public nint hInstance;
        public nint hMenu;
        public nint hwndParent;
        public int cy;
        public int cx;
        public int y;
        public int x;
        public int style;
        public nint lpszName;
        public nint lpszClass;
        public uint dwExStyle;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DEV_BROADCAST_HDR
    {
        public uint dbch_size;
        public uint dbch_devicetype;
        public uint dbch_reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DEV_BROADCAST_DEVICEINTERFACE_W
    {
        public uint dbcc_size;
        public uint dbcc_devicetype;
        public uint dbcc_reserved;
        public Guid dbcc_classguid;
        public ushort dbcc_name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WNDCLASSEXW
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string? lpszClassName;
        public nint hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct BITMAPV5HEADER
    {
        public uint bV5Size;
        public int bV5Width;
        public int bV5Height;
        public ushort bV5Planes;
        public ushort bV5BitCount;
        public uint bV5Compression;
        public uint bV5SizeImage;
        public int bV5XPelsPerMeter;
        public int bV5YPelsPerMeter;
        public uint bV5ClrUsed;
        public uint bV5ClrImportant;
        public uint bV5RedMask;
        public uint bV5GreenMask;
        public uint bV5BlueMask;
        public uint bV5AlphaMask;
        public uint bV5CSType;
        public fixed int bV5Endpoints[9];
        public uint bV5GammaRed;
        public uint bV5GammaGreen;
        public uint bV5GammaBlue;
        public uint bV5Intent;
        public uint bV5ProfileData;
        public uint bV5ProfileSize;
        public uint bV5Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ICONINFO
    {
        public int fIcon;
        public uint xHotspot;
        public uint yHotspot;
        public nint hbmMask;
        public nint hbmColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TRACKMOUSEEVENT
    {
        public uint cbSize;
        public uint dwFlags;
        public nint hwndTrack;
        public uint dwHoverTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTDEVICE
    {
        public ushort usUsagePage;
        public ushort usUsage;
        public uint dwFlags;
        public nint hwndTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTHEADER
    {
        public uint dwType;
        public uint dwSize;
        public nint hDevice;
        public nint wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWMOUSE
    {
        public ushort usFlags;
        public ushort usButtonFlags;
        public ushort usButtonData;
        public ushort padding;
        public uint ulRawButtons;
        public int lLastX;
        public int lLastY;
        public uint ulExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUT
    {
        public RAWINPUTHEADER header;
        public RAWMOUSE mouse;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct DWM_BLURBEHIND
    {
        public uint dwFlags;
        public int fEnable;
        public nint hRgnBlur;
        public int fTransitionOnMaximized;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PIXELFORMATDESCRIPTOR
    {
        public ushort nSize;
        public ushort nVersion;
        public uint dwFlags;
        public byte iPixelType;
        public byte cColorBits;
        public byte cRedBits;
        public byte cRedShift;
        public byte cGreenBits;
        public byte cGreenShift;
        public byte cBlueBits;
        public byte cBlueShift;
        public byte cAlphaBits;
        public byte cAlphaShift;
        public byte cAccumBits;
        public byte cAccumRedBits;
        public byte cAccumGreenBits;
        public byte cAccumBlueBits;
        public byte cAccumAlphaBits;
        public byte cDepthBits;
        public byte cStencilBits;
        public byte cAuxBuffers;
        public byte iLayerType;
        public byte bReserved;
        public uint dwLayerMask;
        public uint dwVisibleMask;
        public uint dwDamageMask;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VkWin32SurfaceCreateInfoKHR
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public nint hinstance;
        public nint hwnd;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GetModuleHandleW(char* lpModuleName);

    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    static extern uint GetLastError();

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int QueryPerformanceFrequency(ulong* lpFrequency);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int QueryPerformanceCounter(ulong* lpPerformanceCount);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint TlsAlloc();

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int TlsFree(uint dwTlsIndex);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern void* TlsGetValue(uint dwTlsIndex);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int TlsSetValue(uint dwTlsIndex, void* lpTlsValue);

    [DllImport("kernel32.dll")]
    static extern void InitializeCriticalSection(CRITICAL_SECTION* lpCriticalSection);

    [DllImport("kernel32.dll")]
    static extern void DeleteCriticalSection(CRITICAL_SECTION* lpCriticalSection);

    [DllImport("kernel32.dll")]
    static extern void EnterCriticalSection(CRITICAL_SECTION* lpCriticalSection);

    [DllImport("kernel32.dll")]
    static extern void LeaveCriticalSection(CRITICAL_SECTION* lpCriticalSection);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint LoadLibraryA(byte* lpLibFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int FreeLibrary(nint hLibModule);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern void* GetProcAddress(nint hModule, byte* lpProcName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GlobalAlloc(uint uFlags, nuint dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GlobalLock(nint hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int GlobalUnlock(nint hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern nint GlobalFree(nint hMem);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern ushort RegisterClassExW(ref WNDCLASSEXW unnamedParam1);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int UnregisterClassW(string lpClassName, nint hInstance);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern nint CreateWindowExW(uint dwExStyle, string lpClassName, string lpWindowName,
        uint dwStyle, int x, int y, int nWidth, int nHeight, nint hWndParent,
        nint hMenu, nint hInstance, nint lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int DestroyWindow(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int UpdateWindow(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint SetFocus(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int BringWindowToTop(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint SetCapture(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int ReleaseCapture();

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int SetWindowTextW(nint hWnd, string lpString);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, int bMenu, uint dwExStyle);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int AdjustWindowRectExForDpi(ref RECT lpRect, uint dwStyle, int bMenu, uint dwExStyle, uint dpi);

    [DllImport("user32.dll")]
    static extern uint GetDpiForWindow(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int IsIconic(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int IsZoomed(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int IsWindowVisible(nint hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint GetFocus();

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetCursorPos(int x, int y);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int ClipCursor(RECT* lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int ScreenToClient(nint hWnd, ref POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int ClientToScreen(nint hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint MonitorFromWindow(nint hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    static extern nint SetCursor(nint hCursor);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint LoadCursorW(nint hInstance, nint lpCursorName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint LoadIconW(nint hInstance, nint lpIconName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint LoadImageW(nint hinst, nint name, uint type, int cx, int cy, uint fuLoad);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int DestroyIcon(nint hIcon);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int DestroyCursor(nint hCursor);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int PeekMessageW(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll")]
    static extern uint GetMessageTime();

    [DllImport("user32.dll")]
    static extern uint MapVirtualKeyW(uint uCode, uint uMapType);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetMessageW(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    static extern int TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    static extern nint DispatchMessageW(ref MSG lpMsg);

    [DllImport("user32.dll")]
    static extern int WaitMessage();

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint MsgWaitForMultipleObjects(uint nCount, nint pHandles, int bWaitAll, uint dwMilliseconds, uint dwWakeMask);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int PostThreadMessageW(uint idThread, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int PostMessageW(nint hWnd, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint RegisterDeviceNotificationW(nint hRecipient, DEV_BROADCAST_DEVICEINTERFACE_W* notificationFilter, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int UnregisterDeviceNotification(nint handle);

    [DllImport("user32.dll")]
    static extern nint DefWindowProcW(nint hWnd, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongPtrW")]
    static extern nint GetWindowLongPtrW(nint hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtrW")]
    static extern nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int FlashWindow(nint hWnd, int bInvert);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetLayeredWindowAttributes(nint hwnd, uint* pcrKey, byte* pbAlpha, uint* pdwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int OpenClipboard(nint hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    static extern int EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint SetClipboardData(uint uFormat, nint hMem);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint GetClipboardData(uint uFormat);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int GetKeyNameTextW(int lParam, char* lpString, int cchSize);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int RegisterRawInputDevices(RAWINPUTDEVICE* pRawInputDevices, uint uiNumDevices, uint cbSize);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetRawInputDeviceList(RAWINPUTDEVICELIST* pRawInputDeviceList, uint* puiNumDevices, uint cbSize);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetRawInputDeviceInfoA")]
    static extern uint GetRawInputDeviceInfoA(nint hDevice, uint uiCommand, void* pData, uint* pcbSize);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetRawInputData(nint hRawInput, uint uiCommand, RAWINPUT* pData, uint* pcbSize, uint cbSizeHeader);

    [DllImport("user32.dll")]
    static extern nint SendMessageW(nint hWnd, uint msg, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("shell32.dll")]
    static extern void DragAcceptFiles(nint hWnd, int fAccept);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "DragQueryFileW")]
    static extern uint DragQueryFileW(nint hDrop, uint iFile, char* lpszFile, uint cch);

    [DllImport("shell32.dll")]
    static extern int DragQueryPoint(nint hDrop, POINT* lppt);

    [DllImport("shell32.dll")]
    static extern void DragFinish(nint hDrop);

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint CreateIconIndirect(ref ICONINFO piconinfo);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint CreateDIBSection(nint hdc, BITMAPV5HEADER* pbmi, uint usage, void** ppvBits, nint hSection, uint offset);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint CreateBitmap(int nWidth, int nHeight, uint nPlanes, uint nBitCount, void* lpBits);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint CreateRectRgn(int x1, int y1, int x2, int y2);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int DeleteObject(nint ho);

    [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern nint CreateDCW(string pwszDriver, string pwszDevice, string? pszPort, nint pdm);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int DeleteDC(nint hdc);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int GetDeviceGammaRamp(nint hdc, ushort* lpRamp);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int SetDeviceGammaRamp(nint hdc, ushort* lpRamp);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int GetDeviceCaps(nint hdc, int index);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int ChoosePixelFormat(nint hdc, PIXELFORMATDESCRIPTOR* ppfd);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int DescribePixelFormat(nint hdc, int iPixelFormat, uint nBytes, PIXELFORMATDESCRIPTOR* ppfd);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int SetPixelFormat(nint hdc, int format, PIXELFORMATDESCRIPTOR* ppfd);

    [DllImport("gdi32.dll", SetLastError = true)]
    static extern int SwapBuffers(nint hdc);

    [DllImport("shcore.dll")]
    static extern int GetDpiForMonitor(nint hmonitor, int dpiType, uint* dpiX, uint* dpiY);

    [DllImport("dwmapi.dll")]
    static extern int DwmEnableBlurBehindWindow(nint hWnd, DWM_BLURBEHIND* pBlurBehind);

    [DllImport("dwmapi.dll")]
    static extern int DwmGetColorizationColor(uint* pcrColorization, int* pfOpaqueBlend);
}
