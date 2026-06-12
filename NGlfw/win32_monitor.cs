using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int ENUM_CURRENT_SETTINGS = -1;
    const uint DISPLAY_DEVICE_ACTIVE = 0x00000001;
    const uint DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;
    const uint DISPLAY_DEVICE_MODESPRUNED = 0x08000000;
    const uint MONITORINFOF_PRIMARY = 0x00000001;
    const uint DM_BITSPERPEL = 0x00040000;
    const uint DM_PELSWIDTH = 0x00080000;
    const uint DM_PELSHEIGHT = 0x00100000;
    const uint DM_DISPLAYFREQUENCY = 0x00400000;
    const uint CDS_FULLSCREEN = 0x00000004;
    const int DISP_CHANGE_SUCCESSFUL = 0;

    static readonly MONITORENUMPROC win32_monitorCallbackDelegate = win32_monitorCallback;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct DISPLAY_DEVICEW
    {
        public uint cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        public uint StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct DEVMODEW
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public uint dmDisplayOrientation;
        public uint dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public ushort dmLogPixels;
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;
        public uint dmICMMethod;
        public uint dmICMIntent;
        public uint dmMediaType;
        public uint dmDitherType;
        public uint dmReserved1;
        public uint dmReserved2;
        public uint dmPanningWidth;
        public uint dmPanningHeight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct MONITORINFOEXW
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }

    delegate int MONITORENUMPROC(nint hMonitor, nint hdcMonitor, nint lprcMonitor, nint dwData);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int EnumDisplayDevicesW(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICEW lpDisplayDevice, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int EnumDisplaySettingsW(string lpszDeviceName, int iModeNum, ref DEVMODEW lpDevMode);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int EnumDisplayMonitors(nint hdc, nint lprcClip, MONITORENUMPROC lpfnEnum, nint dwData);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int GetMonitorInfoW(nint hMonitor, ref MONITORINFOEXW lpmi);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int ChangeDisplaySettingsExW(string lpszDeviceName, ref DEVMODEW lpDevMode, nint hwnd, uint dwflags, nint lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern int ChangeDisplaySettingsExW(string lpszDeviceName, nint lpDevMode, nint hwnd, uint dwflags, nint lParam);

    static DISPLAY_DEVICEW win32_createDisplayDevice()
    {
        return new DISPLAY_DEVICEW
        {
            cb = (uint)Marshal.SizeOf<DISPLAY_DEVICEW>(),
            DeviceName = string.Empty,
            DeviceString = string.Empty,
            DeviceID = string.Empty,
            DeviceKey = string.Empty
        };
    }

    static DEVMODEW win32_createDevMode()
    {
        return new DEVMODEW
        {
            dmDeviceName = string.Empty,
            dmFormName = string.Empty,
            dmSize = (ushort)Marshal.SizeOf<DEVMODEW>()
        };
    }

    static MONITORINFOEXW win32_createMonitorInfo()
    {
        return new MONITORINFOEXW
        {
            cbSize = (uint)Marshal.SizeOf<MONITORINFOEXW>(),
            szDevice = string.Empty
        };
    }

    static void win32_copyWideString(char* destination, int capacity, string source)
    {
        var length = Math.Min(source.Length, capacity - 1);

        for (var i = 0; i < length; i++)
            destination[i] = source[i];

        destination[length] = '\0';
    }

    static string win32_fixedWideString(char* source, int capacity)
    {
        var length = 0;
        while (length < capacity && source[length] != '\0')
            length++;

        return new string(source, 0, length);
    }

    static void win32_copyUtf8String(byte* destination, int capacity, string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);
        var length = Math.Min(bytes.Length, capacity - 1);

        for (var i = 0; i < length; i++)
            destination[i] = bytes[i];

        destination[length] = 0;
    }

    static int win32_monitorCallback(nint handle, nint dc, nint rect, nint data)
    {
        var monitor = (_GLFWmonitor*)data;
        var mi = win32_createMonitorInfo();

        if (GetMonitorInfoW(handle, ref mi) != 0)
        {
            if (win32_fixedWideString(monitor->win32.adapterName, 32) == mi.szDevice)
                monitor->win32.handle = handle;
        }

        return GLFW_TRUE;
    }

    static void win32_vidmodeFromDevMode(ref DEVMODEW dm, GLFWvidmode* mode)
    {
        mode->width = (int)dm.dmPelsWidth;
        mode->height = (int)dm.dmPelsHeight;
        mode->refreshRate = (int)dm.dmDisplayFrequency;

        if (mode->refreshRate == 0 || mode->refreshRate == 1)
            mode->refreshRate = 60;

        _glfwSplitBPP((int)dm.dmBitsPerPel, &mode->redBits, &mode->greenBits, &mode->blueBits);
    }

    static _GLFWmonitor* win32_createMonitor(ref DISPLAY_DEVICEW adapter, int placement)
    {
        var dm = win32_createDevMode();
        if (EnumDisplaySettingsW(adapter.DeviceName, ENUM_CURRENT_SETTINGS, ref dm) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to query display settings");
            return null;
        }

        var dpi = 96f;
        var widthMM = (int)(dm.dmPelsWidth * 25.4f / dpi);
        var heightMM = (int)(dm.dmPelsHeight * 25.4f / dpi);
        var name = string.IsNullOrEmpty(adapter.DeviceString) ? adapter.DeviceName : adapter.DeviceString;
        var monitor = _glfwAllocMonitor(name, widthMM, heightMM);
        if (monitor == null)
            return null;

        if ((adapter.StateFlags & DISPLAY_DEVICE_MODESPRUNED) != 0)
            monitor->win32.modesPruned = GLFW_TRUE;

        win32_copyWideString(monitor->win32.adapterName, 32, adapter.DeviceName);
        win32_copyUtf8String(monitor->win32.publicAdapterName, 32, adapter.DeviceName);

        EnumDisplayMonitors(0, 0, win32_monitorCallbackDelegate, (nint)monitor);

        if (monitor->win32.handle == 0)
        {
            _glfwFreeMonitor(monitor);
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to find monitor handle");
            return null;
        }

        win32_vidmodeFromDevMode(ref dm, &monitor->currentMode);
        return monitor;
    }

    static void _glfwPollMonitorsWin32()
    {
        for (uint adapterIndex = 0; ; adapterIndex++)
        {
            var adapter = win32_createDisplayDevice();
            if (EnumDisplayDevicesW(null, adapterIndex, ref adapter, 0) == 0)
                break;

            if ((adapter.StateFlags & DISPLAY_DEVICE_ACTIVE) == 0)
                continue;

            var placement = (adapter.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE) != 0
                ? _GLFW_INSERT_FIRST
                : _GLFW_INSERT_LAST;

            var monitor = win32_createMonitor(ref adapter, placement);
            if (monitor == null)
                return;

            _glfwInputMonitor(monitor, GLFW_CONNECTED, placement);
        }
    }

    static void _glfwFreeMonitorWin32(_GLFWmonitor* monitor)
    {
        _glfwFreeGammaArrays(&monitor->@null.ramp);
    }

    static void _glfwSetVideoModeWin32(_GLFWmonitor* monitor, GLFWvidmode* desired)
    {
        var best = _glfwChooseVideoMode(monitor, desired);
        if (best == null)
            return;

        GLFWvidmode current;
        if (_glfwGetVideoModeWin32(monitor, &current) == 0)
            return;

        if (_glfwCompareVideoModes(&current, best) == 0)
            return;

        var dm = win32_createDevMode();
        dm.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL | DM_DISPLAYFREQUENCY;
        dm.dmPelsWidth = (uint)best->width;
        dm.dmPelsHeight = (uint)best->height;
        dm.dmBitsPerPel = (uint)(best->redBits + best->greenBits + best->blueBits);
        dm.dmDisplayFrequency = (uint)best->refreshRate;

        if (dm.dmBitsPerPel < 15 || dm.dmBitsPerPel >= 24)
            dm.dmBitsPerPel = 32;

        var result = ChangeDisplaySettingsExW(
            win32_fixedWideString(monitor->win32.adapterName, 32),
            ref dm,
            0,
            CDS_FULLSCREEN,
            0);

        if (result == DISP_CHANGE_SUCCESSFUL)
            monitor->win32.modeChanged = GLFW_TRUE;
        else
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to set video mode");
    }

    static void _glfwRestoreVideoModeWin32(_GLFWmonitor* monitor)
    {
        if (monitor->win32.modeChanged == 0)
            return;

        ChangeDisplaySettingsExW(
            win32_fixedWideString(monitor->win32.adapterName, 32),
            0,
            0,
            CDS_FULLSCREEN,
            0);
        monitor->win32.modeChanged = GLFW_FALSE;
    }

    static void _glfwGetHMONITORContentScaleWin32(nint handle, float* xscale, float* yscale)
    {
        uint xdpi;
        uint ydpi;

        if (xscale != null)
            *xscale = 0f;
        if (yscale != null)
            *yscale = 0f;

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 3))
        {
            if (GetDpiForMonitor(handle, MDT_EFFECTIVE_DPI, &xdpi, &ydpi) != S_OK)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to query monitor DPI");
                return;
            }
        }
        else
        {
            var dc = GetDC(0);
            if (dc == 0)
            {
                _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to retrieve screen DC");
                return;
            }

            xdpi = (uint)GetDeviceCaps(dc, LOGPIXELSX);
            ydpi = (uint)GetDeviceCaps(dc, LOGPIXELSY);
            ReleaseDC(0, dc);
        }

        if (xscale != null)
            *xscale = xdpi / (float)USER_DEFAULT_SCREEN_DPI;
        if (yscale != null)
            *yscale = ydpi / (float)USER_DEFAULT_SCREEN_DPI;
    }

    static void _glfwGetMonitorPosWin32(_GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        var mi = win32_createMonitorInfo();
        if (GetMonitorInfoW(monitor->win32.handle, ref mi) == 0)
            return;

        if (xpos != null)
            *xpos = mi.rcMonitor.left;
        if (ypos != null)
            *ypos = mi.rcMonitor.top;
    }

    static void _glfwGetMonitorContentScaleWin32(_GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        _glfwGetHMONITORContentScaleWin32(monitor->win32.handle, xscale, yscale);
    }

    static void _glfwGetMonitorWorkareaWin32(_GLFWmonitor* monitor, int* xpos, int* ypos, int* width, int* height)
    {
        var mi = win32_createMonitorInfo();
        if (GetMonitorInfoW(monitor->win32.handle, ref mi) == 0)
            return;

        if (xpos != null)
            *xpos = mi.rcWork.left;
        if (ypos != null)
            *ypos = mi.rcWork.top;
        if (width != null)
            *width = mi.rcWork.right - mi.rcWork.left;
        if (height != null)
            *height = mi.rcWork.bottom - mi.rcWork.top;
    }

    static GLFWvidmode* _glfwGetVideoModesWin32(_GLFWmonitor* monitor, int* found)
    {
        GLFWvidmode* result = null;
        var count = 0;

        string adapterName;
        adapterName = win32_fixedWideString(monitor->win32.adapterName, 32);

        for (var modeIndex = 0; ; modeIndex++)
        {
            var dm = win32_createDevMode();
            if (EnumDisplaySettingsW(adapterName, modeIndex, ref dm) == 0)
                break;

            if (dm.dmBitsPerPel < 15 || dm.dmPelsWidth == 0 || dm.dmPelsHeight == 0)
                continue;

            result = (GLFWvidmode*)_glfw_realloc(result, (nuint)((count + 1) * sizeof(GLFWvidmode)));
            if (result == null)
                return null;

            win32_vidmodeFromDevMode(ref dm, &result[count]);
            count++;
        }

        if (count == 0)
        {
            result = (GLFWvidmode*)_glfw_calloc(1, (nuint)sizeof(GLFWvidmode));
            if (result == null)
                return null;

            if (_glfwGetVideoModeWin32(monitor, result) == 0)
            {
                _glfw_free(result);
                return null;
            }

            count = 1;
        }

        if (found != null)
            *found = count;

        return result;
    }

    static int _glfwGetVideoModeWin32(_GLFWmonitor* monitor, GLFWvidmode* mode)
    {
        string adapterName;
        adapterName = win32_fixedWideString(monitor->win32.adapterName, 32);

        var dm = win32_createDevMode();
        if (EnumDisplaySettingsW(adapterName, ENUM_CURRENT_SETTINGS, ref dm) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to query current display mode");
            return GLFW_FALSE;
        }

        if (mode != null)
            win32_vidmodeFromDevMode(ref dm, mode);

        return GLFW_TRUE;
    }

    static int _glfwGetGammaRampWin32(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        var values = stackalloc ushort[3 * 256];
        var dc = CreateDCW("DISPLAY", win32_fixedWideString(monitor->win32.adapterName, 32), null, 0);
        if (dc == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create display device context");
            return GLFW_FALSE;
        }

        if (GetDeviceGammaRamp(dc, values) == 0)
        {
            DeleteDC(dc);
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to query gamma ramp");
            return GLFW_FALSE;
        }

        DeleteDC(dc);

        _glfwAllocGammaArrays(ramp, 256);
        for (var i = 0; i < 256; i++)
        {
            ramp->red[i] = values[i];
            ramp->green[i] = values[256 + i];
            ramp->blue[i] = values[512 + i];
        }

        return GLFW_TRUE;
    }

    static void _glfwSetGammaRampWin32(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (ramp->size != 256)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Gamma ramp size must be 256");
            return;
        }

        var values = stackalloc ushort[3 * 256];
        for (var i = 0; i < 256; i++)
        {
            values[i] = ramp->red[i];
            values[256 + i] = ramp->green[i];
            values[512 + i] = ramp->blue[i];
        }

        var dc = CreateDCW("DISPLAY", win32_fixedWideString(monitor->win32.adapterName, 32), null, 0);
        if (dc == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to create display device context");
            return;
        }

        if (SetDeviceGammaRamp(dc, values) == 0)
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "Win32: Failed to set gamma ramp");

        DeleteDC(dc);
    }

    public static byte* glfwGetWin32Adapter(GLFWmonitor* monitor)
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

        return ((_GLFWmonitor*)monitor)->win32.publicAdapterName;
    }

    public static byte* glfwGetWin32Monitor(GLFWmonitor* monitor)
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

        return ((_GLFWmonitor*)monitor)->win32.publicDisplayName;
    }
}
