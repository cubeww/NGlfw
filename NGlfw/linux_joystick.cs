using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int IN_ATTRIB = 0x00000004;
    const int IN_CREATE = 0x00000100;
    const int IN_DELETE = 0x00000200;
    const int IN_NONBLOCK = 0x00000800;
    const int IN_CLOEXEC = 0x00080000;

    static readonly byte* _glfwLinuxMappingName = _glfw_allocate_static_string("Linux");
    static readonly byte* _glfwLinuxInputPath = _glfw_allocate_static_string("/dev/input");

    static void _glfwDetectJoystickConnectionLinux()
    {
        if (_glfw.linux_js.inotify < 0)
            return;

        var buffer = stackalloc byte[16384];
        while (read(_glfw.linux_js.inotify, buffer, 16384) > 0)
        {
        }
    }

    static int _glfwInitJoysticksLinux()
    {
        _glfw.linux_js.inotify = inotify_init1(IN_NONBLOCK | IN_CLOEXEC);
        if (_glfw.linux_js.inotify >= 0)
        {
            _glfw.linux_js.watch = inotify_add_watch(_glfw.linux_js.inotify,
                _glfwLinuxInputPath,
                IN_CREATE | IN_ATTRIB | IN_DELETE);
        }
        else
        {
            _glfw.linux_js.inotify = -1;
            _glfw.linux_js.watch = -1;
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksLinux()
    {
        if (_glfw.linux_js.inotify >= 0)
        {
            if (_glfw.linux_js.watch >= 0)
                inotify_rm_watch(_glfw.linux_js.inotify, _glfw.linux_js.watch);

            close(_glfw.linux_js.inotify);
        }

        _glfw.linux_js.inotify = -1;
        _glfw.linux_js.watch = -1;
    }

    static int _glfwPollJoystickLinux(_GLFWjoystick* js, int mode)
    {
        _glfwDetectJoystickConnectionLinux();
        return js->connected;
    }

    static byte* _glfwGetMappingNameLinux()
    {
        return _glfwLinuxMappingName;
    }

    static void _glfwUpdateGamepadGUIDLinux(byte* guid)
    {
    }

    [DllImport("libc", SetLastError = true)]
    static extern int inotify_init1(int flags);

    [DllImport("libc", SetLastError = true)]
    static extern int inotify_add_watch(int fd, byte* pathname, uint mask);

    [DllImport("libc", SetLastError = true)]
    static extern int inotify_rm_watch(int fd, int wd);

    [DllImport("libc", SetLastError = true)]
    static extern nint read(int fd, void* buf, nuint count);

    [DllImport("libc", SetLastError = true)]
    static extern int close(int fd);
}
