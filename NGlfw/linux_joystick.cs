using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int IN_ATTRIB = 0x00000004;
    const int IN_CREATE = 0x00000100;
    const int IN_DELETE = 0x00000200;
    const int IN_NONBLOCK = 0x00000800;
    const int IN_CLOEXEC = 0x00080000;

    const int O_RDONLY = 0;
    const int O_NONBLOCK = 0x00000800;
    const int O_CLOEXEC = 0x00080000;
    const int ENODEV = 19;

    const int PATH_MAX = 4096;
    const int EV_SYN = 0x00;
    const int EV_KEY = 0x01;
    const int EV_ABS = 0x03;
    const int EV_MAX = 0x1f;
    const int EV_CNT = EV_MAX + 1;
    const int SYN_REPORT = 0;
    const int SYN_DROPPED = 3;
    const int KEY_MAX = 0x2ff;
    const int KEY_CNT = KEY_MAX + 1;
    const int BTN_MISC = 0x100;
    const int ABS_MAX = 0x3f;
    const int ABS_CNT = ABS_MAX + 1;
    const int ABS_HAT0X = 0x10;
    const int ABS_HAT3Y = 0x17;

    const int IOC_NRBITS = 8;
    const int IOC_TYPEBITS = 8;
    const int IOC_SIZEBITS = 14;
    const int IOC_NRSHIFT = 0;
    const int IOC_TYPESHIFT = IOC_NRSHIFT + IOC_NRBITS;
    const int IOC_SIZESHIFT = IOC_TYPESHIFT + IOC_TYPEBITS;
    const int IOC_DIRSHIFT = IOC_SIZESHIFT + IOC_SIZEBITS;
    const int IOC_READ = 2;

    static readonly byte* _glfwLinuxMappingName = _glfw_allocate_static_string("Linux");
    static readonly byte* _glfwLinuxInputPath = _glfw_allocate_static_string("/dev/input");

    public struct LINUX_TIMEVAL
    {
        public nint tv_sec;
        public nint tv_usec;
    }

    public struct LINUX_INPUT_EVENT
    {
        public LINUX_TIMEVAL time;
        public ushort type;
        public ushort code;
        public int value;
    }

    public struct LINUX_INPUT_ID
    {
        public ushort bustype;
        public ushort vendor;
        public ushort product;
        public ushort version;
    }

    public struct LINUX_INPUT_ABSINFO
    {
        public int value;
        public int minimum;
        public int maximum;
        public int fuzz;
        public int flat;
        public int resolution;
    }

    public struct LINUX_INOTIFY_EVENT
    {
        public int wd;
        public uint mask;
        public uint cookie;
        public uint len;
    }

    [InlineArray(KEY_CNT - BTN_MISC)]
    public struct LINUX_KEYMAP
    {
        private int _element0;
    }

    [InlineArray(ABS_CNT)]
    public struct LINUX_ABSMAP
    {
        private int _element0;
    }

    [InlineArray(ABS_CNT)]
    public struct LINUX_ABSINFO_ARRAY
    {
        private LINUX_INPUT_ABSINFO _element0;
    }

    [InlineArray(8)]
    public struct LINUX_HATSTATE
    {
        private int _element0;
    }

    public struct _GLFWjoystickLinux
    {
        public int fd;
        public fixed byte path[PATH_MAX];
        public LINUX_KEYMAP keyMap;
        public LINUX_ABSMAP absMap;
        public LINUX_HATSTATE hats;
        public LINUX_ABSINFO_ARRAY absInfo;
    }

    static ulong linux_IOC(int dir, int type, int nr, int size)
    {
        return ((ulong)dir << IOC_DIRSHIFT) |
               ((ulong)type << IOC_TYPESHIFT) |
               ((ulong)nr << IOC_NRSHIFT) |
               ((ulong)size << IOC_SIZESHIFT);
    }

    static ulong linux_EVIOCGBIT(int ev, int len)
    {
        return linux_IOC(IOC_READ, (byte)'E', 0x20 + ev, len);
    }

    static ulong linux_EVIOCGABS(int abs)
    {
        return linux_IOC(IOC_READ, (byte)'E', 0x40 + abs, sizeof(LINUX_INPUT_ABSINFO));
    }

    static ulong linux_EVIOCGNAME(int len)
    {
        return linux_IOC(IOC_READ, (byte)'E', 0x06, len);
    }

    static ulong linux_EVIOCGID()
    {
        return linux_IOC(IOC_READ, (byte)'E', 0x02, sizeof(LINUX_INPUT_ID));
    }

    static int linux_isBitSet(int bit, byte* bits)
    {
        return (bits[bit / 8] & (1 << (bit % 8))) != 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void linux_clearBytes(byte* buffer, int count)
    {
        for (var i = 0; i < count; i++)
            buffer[i] = 0;
    }

    static int linux_isEventDeviceName(string name)
    {
        if (!name.StartsWith("event", StringComparison.Ordinal) || name.Length == 5)
            return GLFW_FALSE;

        for (var i = 5; i < name.Length; i++)
        {
            if (name[i] < '0' || name[i] > '9')
                return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static int linux_stringEquals(byte* a, byte* b)
    {
        while (*a == *b)
        {
            if (*a == 0)
                return GLFW_TRUE;

            a++;
            b++;
        }

        return GLFW_FALSE;
    }

    static string linux_stringFromBuffer(byte* value)
    {
        return Marshal.PtrToStringUTF8((nint)value) ?? string.Empty;
    }

    static void linux_copyPath(_GLFWjoystickLinux* linux, byte* path)
    {
        var destination = linux->path;
        var i = 0;
        for (; i < PATH_MAX - 1 && path[i] != 0; i++)
            destination[i] = path[i];
        destination[i] = 0;
    }

    static string linux_createGUID(LINUX_INPUT_ID id, string name)
    {
        if (id.vendor != 0 && id.product != 0 && id.version != 0)
        {
            return $"{id.bustype & 0xff:x2}{id.bustype >> 8:x2}0000" +
                   $"{id.vendor & 0xff:x2}{id.vendor >> 8:x2}0000" +
                   $"{id.product & 0xff:x2}{id.product >> 8:x2}0000" +
                   $"{id.version & 0xff:x2}{id.version >> 8:x2}0000";
        }

        var bytes = Encoding.UTF8.GetBytes(name);
        int b(int index) => index < bytes.Length ? bytes[index] : 0;

        return $"{id.bustype & 0xff:x2}{id.bustype >> 8:x2}0000" +
               $"{b(0):x2}{b(1):x2}{b(2):x2}{b(3):x2}" +
               $"{b(4):x2}{b(5):x2}{b(6):x2}{b(7):x2}" +
               $"{b(8):x2}{b(9):x2}{b(10):x2}00";
    }

    static int linux_hatState(int x, int y)
    {
        var state = GLFW_HAT_CENTERED;

        if (x == 1)
            state |= GLFW_HAT_LEFT;
        else if (x == 2)
            state |= GLFW_HAT_RIGHT;

        if (y == 1)
            state |= GLFW_HAT_UP;
        else if (y == 2)
            state |= GLFW_HAT_DOWN;

        return state;
    }

    static void linux_handleKeyEvent(_GLFWjoystick* js, int code, int value)
    {
        if (code < BTN_MISC || code >= KEY_CNT)
            return;

        var button = js->linux.keyMap[code - BTN_MISC];
        if (button >= 0 && button < js->buttonCount)
            _glfwInputJoystickButton(js, button, value != 0 ? GLFW_PRESS : GLFW_RELEASE);
    }

    static void linux_handleAbsEvent(_GLFWjoystick* js, int code, int value)
    {
        if (code < 0 || code >= ABS_CNT)
            return;

        var index = js->linux.absMap[code];
        if (index < 0)
            return;

        if (code >= ABS_HAT0X && code <= ABS_HAT3Y)
        {
            var hat = (code - ABS_HAT0X) / 2;
            var axis = (code - ABS_HAT0X) % 2;
            var state = 0;

            if (value < 0)
                state = 1;
            else if (value > 0)
                state = 2;

            js->linux.hats[hat * 2 + axis] = state;
            _glfwInputJoystickHat(js,
                index,
                linux_hatState(js->linux.hats[hat * 2], js->linux.hats[hat * 2 + 1]));
        }
        else
        {
            var info = js->linux.absInfo[code];
            var normalized = (float)value;
            var range = info.maximum - info.minimum;

            if (range != 0)
            {
                normalized = (normalized - info.minimum) / range;
                normalized = normalized * 2f - 1f;
            }

            _glfwInputJoystickAxis(js, index, normalized);
        }
    }

    static void linux_pollAbsState(_GLFWjoystick* js)
    {
        for (var code = 0; code < ABS_CNT; code++)
        {
            if (js->linux.absMap[code] < 0)
                continue;

            var info = js->linux.absInfo[code];
            if (ioctl(js->linux.fd, linux_EVIOCGABS(code), &info) < 0)
                continue;

            js->linux.absInfo[code] = info;
            linux_handleAbsEvent(js, code, info.value);
        }
    }

    static int linux_openJoystickDevice(string path)
    {
        var pathBytes = Encoding.UTF8.GetBytes(path + '\0');

        fixed (byte* pathPtr = pathBytes)
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var joystick = &glfw->joysticks[jid];
                if (joystick->connected == 0)
                    continue;

                if (linux_stringEquals(joystick->linux.path, pathPtr) != 0)
                    return GLFW_FALSE;
            }

            _GLFWjoystickLinux linux = default;
            linux.fd = open(pathPtr, O_RDONLY | O_NONBLOCK | O_CLOEXEC);
            if (linux.fd < 0)
                return GLFW_FALSE;

            var evBitsCount = (EV_CNT + 7) / 8;
            var keyBitsCount = (KEY_CNT + 7) / 8;
            var absBitsCount = (ABS_CNT + 7) / 8;
            var evBits = stackalloc byte[evBitsCount];
            var keyBits = stackalloc byte[keyBitsCount];
            var absBits = stackalloc byte[absBitsCount];
            linux_clearBytes(evBits, evBitsCount);
            linux_clearBytes(keyBits, keyBitsCount);
            linux_clearBytes(absBits, absBitsCount);

            LINUX_INPUT_ID id;
            if (ioctl(linux.fd, linux_EVIOCGBIT(0, evBitsCount), evBits) < 0 ||
                ioctl(linux.fd, linux_EVIOCGBIT(EV_KEY, keyBitsCount), keyBits) < 0 ||
                ioctl(linux.fd, linux_EVIOCGBIT(EV_ABS, absBitsCount), absBits) < 0 ||
                ioctl(linux.fd, linux_EVIOCGID(), &id) < 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Linux: Failed to query input device");
                close(linux.fd);
                return GLFW_FALSE;
            }

            if (linux_isBitSet(EV_ABS, evBits) == 0)
            {
                close(linux.fd);
                return GLFW_FALSE;
            }

            var nameBuffer = stackalloc byte[256];
            linux_clearBytes(nameBuffer, 256);
            if (ioctl(linux.fd, linux_EVIOCGNAME(256), nameBuffer) < 0)
            {
                var unknown = Encoding.UTF8.GetBytes("Unknown\0");
                for (var i = 0; i < unknown.Length; i++)
                    nameBuffer[i] = unknown[i];
            }

            var name = linux_stringFromBuffer(nameBuffer);
            var guid = linux_createGUID(id, name);
            var axisCount = 0;
            var buttonCount = 0;
            var hatCount = 0;

            for (var code = BTN_MISC; code < KEY_CNT; code++)
            {
                linux.keyMap[code - BTN_MISC] = -1;

                if (linux_isBitSet(code, keyBits) == 0)
                    continue;

                linux.keyMap[code - BTN_MISC] = buttonCount;
                buttonCount++;
            }

            for (var code = 0; code < ABS_CNT; code++)
            {
                linux.absMap[code] = -1;
                if (linux_isBitSet(code, absBits) == 0)
                    continue;

                if (code >= ABS_HAT0X && code <= ABS_HAT3Y)
                {
                    linux.absMap[code] = hatCount;
                    if (code + 1 <= ABS_HAT3Y)
                        linux.absMap[code + 1] = hatCount;

                    hatCount++;
                    code++;
                }
                else
                {
                    var info = linux.absInfo[code];
                    if (ioctl(linux.fd, linux_EVIOCGABS(code), &info) < 0)
                        continue;

                    linux.absInfo[code] = info;
                    linux.absMap[code] = axisCount;
                    axisCount++;
                }
            }

            var js = _glfwAllocJoystick(name, guid, axisCount, buttonCount, hatCount);
            if (js == null)
            {
                close(linux.fd);
                return GLFW_FALSE;
            }

            linux_copyPath(&linux, pathPtr);
            js->linux = linux;
            linux_pollAbsState(js);

            _glfwInputJoystick(js, GLFW_CONNECTED);
            return GLFW_TRUE;
        }
    }

    static void linux_closeJoystick(_GLFWjoystick* js)
    {
        if (js->connected != 0)
            _glfwInputJoystick(js, GLFW_DISCONNECTED);

        if (js->linux.fd >= 0)
            close(js->linux.fd);

        _glfwFreeJoystick(js);
    }

    static void _glfwDetectJoystickConnectionLinux()
    {
        if (_glfw.linux_js.inotify < 0)
            return;

        var buffer = stackalloc byte[16384];
        var size = read(_glfw.linux_js.inotify, buffer, 16384);
        if (size <= 0)
            return;

        nint offset = 0;
        while (offset + sizeof(LINUX_INOTIFY_EVENT) <= size)
        {
            var @event = (LINUX_INOTIFY_EVENT*)(buffer + offset);
            var name = @event->len != 0
                ? Marshal.PtrToStringUTF8((nint)(buffer + offset + sizeof(LINUX_INOTIFY_EVENT))) ?? string.Empty
                : string.Empty;

            offset += sizeof(LINUX_INOTIFY_EVENT) + (nint)@event->len;

            if (linux_isEventDeviceName(name) == 0)
                continue;

            var path = "/dev/input/" + name;
            if ((@event->mask & (IN_CREATE | IN_ATTRIB)) != 0)
            {
                linux_openJoystickDevice(path);
            }
            else if ((@event->mask & IN_DELETE) != 0)
            {
                var pathBytes = Encoding.UTF8.GetBytes(path + '\0');
                fixed (byte* pathPtr = pathBytes)
                fixed (_GLFWlibrary* glfw = &_glfw)
                {
                    for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
                    {
                        var js = &glfw->joysticks[jid];
                        if (linux_stringEquals(js->linux.path, pathPtr) != 0)
                        {
                            linux_closeJoystick(js);
                            break;
                        }
                    }
                }
            }
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

        if (Directory.Exists("/dev/input"))
        {
            foreach (var path in Directory.EnumerateFiles("/dev/input", "event*"))
            {
                if (linux_isEventDeviceName(Path.GetFileName(path)) != 0)
                    linux_openJoystickDevice(path);
            }
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksLinux()
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0)
                    linux_closeJoystick(js);
            }
        }

        if (_glfw.linux_js.inotify >= 0)
        {
            if (_glfw.linux_js.watch >= 0)
                inotify_rm_watch(_glfw.linux_js.inotify, _glfw.linux_js.watch);

            close(_glfw.linux_js.inotify);
        }

        _glfw.linux_js.inotify = -1;
        _glfw.linux_js.watch = -1;
        _glfw.linux_js.dropped = GLFW_FALSE;
    }

    static int _glfwPollJoystickLinux(_GLFWjoystick* js, int mode)
    {
        _glfwDetectJoystickConnectionLinux();

        if (js->connected == 0)
            return GLFW_FALSE;

        for (;;)
        {
            LINUX_INPUT_EVENT @event;
            var result = read(js->linux.fd, &@event, (nuint)sizeof(LINUX_INPUT_EVENT));
            if (result < 0)
            {
                var error = Marshal.GetLastPInvokeError();
                if (error == EINTR)
                    continue;
                if (error == ENODEV)
                    linux_closeJoystick(js);

                break;
            }

            if (result != sizeof(LINUX_INPUT_EVENT))
                break;

            if (@event.type == EV_SYN)
            {
                if (@event.code == SYN_DROPPED)
                    _glfw.linux_js.dropped = GLFW_TRUE;
                else if (@event.code == SYN_REPORT)
                {
                    _glfw.linux_js.dropped = GLFW_FALSE;
                    linux_pollAbsState(js);
                }
            }

            if (_glfw.linux_js.dropped != 0)
                continue;

            if (@event.type == EV_KEY)
                linux_handleKeyEvent(js, @event.code, @event.value);
            else if (@event.type == EV_ABS)
                linux_handleAbsEvent(js, @event.code, @event.value);
        }

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
    static extern int open(byte* pathname, int flags);

    [DllImport("libc", SetLastError = true)]
    static extern int ioctl(int fd, ulong request, void* argp);

    [DllImport("libc", SetLastError = true)]
    static extern nint read(int fd, void* buf, nuint count);

    [DllImport("libc", SetLastError = true)]
    static extern int close(int fd);
}
