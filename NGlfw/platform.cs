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
