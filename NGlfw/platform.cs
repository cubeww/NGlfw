namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwSelectPlatform(int desiredID, _GLFWplatform* platform)
    {
        if (desiredID != GLFW_ANY_PLATFORM &&
            desiredID != GLFW_PLATFORM_WIN32 &&
            desiredID != GLFW_PLATFORM_COCOA &&
            desiredID != GLFW_PLATFORM_WAYLAND &&
            desiredID != GLFW_PLATFORM_X11 &&
            desiredID != GLFW_PLATFORM_NULL)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid platform ID 0x%08X", desiredID);
            return GLFW_FALSE;
        }

        if (desiredID == GLFW_PLATFORM_NULL)
            return _glfwConnectNull(desiredID, platform);

        if (desiredID == GLFW_ANY_PLATFORM)
        {
            if (OperatingSystem.IsWindows())
                return _glfwConnectWin32(desiredID, platform);

            if (OperatingSystem.IsMacOS())
                return _glfwConnectCocoa(desiredID, platform);

            if (OperatingSystem.IsLinux() && _glfwConnectX11(desiredID, platform) != 0)
                return GLFW_TRUE;

            return _glfwConnectNull(desiredID, platform);
        }

        if (desiredID == GLFW_PLATFORM_WIN32)
        {
            if (OperatingSystem.IsWindows())
                return _glfwConnectWin32(desiredID, platform);

            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Win32: Platform not available on this system");
            return GLFW_FALSE;
        }

        if (desiredID == GLFW_PLATFORM_COCOA)
        {
            if (OperatingSystem.IsMacOS())
                return _glfwConnectCocoa(desiredID, platform);

            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not available on this system");
            return GLFW_FALSE;
        }

        if (desiredID == GLFW_PLATFORM_X11)
            return _glfwConnectX11(desiredID, platform);

        _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "The requested platform is not available in this binary");
        return GLFW_FALSE;
    }

    static void _glfwPlatformInitTimer()
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformInitTimerPOSIX();
            return;
        }

        ulong frequency;
        QueryPerformanceFrequency(&frequency);
        _glfw.timer.frequency = frequency;
    }

    static ulong _glfwPlatformGetTimerValue()
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformGetTimerValuePOSIX();

        ulong value;
        QueryPerformanceCounter(&value);
        return value;
    }

    static ulong _glfwPlatformGetTimerFrequency()
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformGetTimerFrequencyPOSIX();

        return _glfw.timer.frequency;
    }

    static int _glfwPlatformCreateTls(_GLFWtls* tls)
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformCreateTlsPOSIX(tls);

        tls->index = TlsAlloc();
        if (tls->index == TLS_OUT_OF_INDEXES)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to allocate TLS index");
            return GLFW_FALSE;
        }

        tls->allocated = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void _glfwPlatformDestroyTls(_GLFWtls* tls)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformDestroyTlsPOSIX(tls);
            return;
        }

        if (tls->allocated != 0)
            TlsFree(tls->index);
        *tls = default;
    }

    static void* _glfwPlatformGetTls(_GLFWtls* tls)
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformGetTlsPOSIX(tls);

        return TlsGetValue(tls->index);
    }

    static void _glfwPlatformSetTls(_GLFWtls* tls, void* value)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformSetTlsPOSIX(tls, value);
            return;
        }

        TlsSetValue(tls->index, value);
    }

    static int _glfwPlatformCreateMutex(_GLFWmutex* mutex)
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformCreateMutexPOSIX(mutex);

        InitializeCriticalSection(&mutex->section);
        mutex->allocated = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void _glfwPlatformDestroyMutex(_GLFWmutex* mutex)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformDestroyMutexPOSIX(mutex);
            return;
        }

        if (mutex->allocated != 0)
            DeleteCriticalSection(&mutex->section);
        *mutex = default;
    }

    static void _glfwPlatformLockMutex(_GLFWmutex* mutex)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformLockMutexPOSIX(mutex);
            return;
        }

        EnterCriticalSection(&mutex->section);
    }

    static void _glfwPlatformUnlockMutex(_GLFWmutex* mutex)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformUnlockMutexPOSIX(mutex);
            return;
        }

        LeaveCriticalSection(&mutex->section);
    }

    static void* _glfwPlatformLoadModule(byte* path)
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformLoadModulePOSIX(path);

        return (void*)LoadLibraryA(path);
    }

    static void _glfwPlatformFreeModule(void* module)
    {
        if (!OperatingSystem.IsWindows())
        {
            _glfwPlatformFreeModulePOSIX(module);
            return;
        }

        if (module != null)
            FreeLibrary((nint)module);
    }

    static void* _glfwPlatformGetModuleSymbol(void* module, byte* name)
    {
        if (!OperatingSystem.IsWindows())
            return _glfwPlatformGetModuleSymbolPOSIX(module, name);

        return GetProcAddress((nint)module, name);
    }

    public static int glfwGetPlatform()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        return _glfw.platform.platformID;
    }

    public static int glfwPlatformSupported(int platformID)
    {
        if (platformID != GLFW_PLATFORM_WIN32 &&
            platformID != GLFW_PLATFORM_COCOA &&
            platformID != GLFW_PLATFORM_WAYLAND &&
            platformID != GLFW_PLATFORM_X11 &&
            platformID != GLFW_PLATFORM_NULL)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid platform ID 0x%08X", platformID);
            return GLFW_FALSE;
        }

        if (platformID == GLFW_PLATFORM_NULL)
            return GLFW_TRUE;

        if (platformID == GLFW_PLATFORM_WIN32 && OperatingSystem.IsWindows())
            return GLFW_TRUE;

        if (platformID == GLFW_PLATFORM_COCOA && OperatingSystem.IsMacOS())
            return GLFW_TRUE;

        if (platformID == GLFW_PLATFORM_X11 && OperatingSystem.IsLinux())
            return GLFW_TRUE;

        return GLFW_FALSE;
    }

    public static byte* glfwGetVersionString()
    {
        return _glfwVersionString;
    }
}
