using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int CLOCK_REALTIME = 0;
    const int CLOCK_MONOTONIC = 1;
    const int RTLD_LAZY = 0x00001;
    const int RTLD_LOCAL = 0x00000;
    const int EINTR = 4;
    const int EAGAIN = 11;

    [StructLayout(LayoutKind.Sequential)]
    public struct PTHREAD_MUTEX_T
    {
        public fixed byte storage[40];
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TIMESPEC
    {
        public nint tv_sec;
        public nint tv_nsec;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct POLLFD
    {
        public int fd;
        public short events;
        public short revents;
    }

    static void _glfwPlatformInitTimerPOSIX()
    {
        _glfw.timer.posixClock = CLOCK_REALTIME;
        _glfw.timer.frequency = 1000000000;

        TIMESPEC ts;
        if (clock_gettime(CLOCK_MONOTONIC, &ts) == 0)
            _glfw.timer.posixClock = CLOCK_MONOTONIC;
    }

    static ulong _glfwPlatformGetTimerValuePOSIX()
    {
        TIMESPEC ts;
        clock_gettime(_glfw.timer.posixClock, &ts);
        return (ulong)ts.tv_sec * _glfw.timer.frequency + (ulong)ts.tv_nsec;
    }

    static ulong _glfwPlatformGetTimerFrequencyPOSIX()
    {
        return _glfw.timer.frequency;
    }

    static int _glfwPlatformCreateTlsPOSIX(_GLFWtls* tls)
    {
        uint key;
        if (pthread_key_create(&key, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "POSIX: Failed to create context TLS");
            return GLFW_FALSE;
        }

        tls->key = key;
        tls->allocated = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void _glfwPlatformDestroyTlsPOSIX(_GLFWtls* tls)
    {
        if (tls->allocated != 0)
            pthread_key_delete(tls->key);
        *tls = default;
    }

    static void* _glfwPlatformGetTlsPOSIX(_GLFWtls* tls)
    {
        return pthread_getspecific(tls->key);
    }

    static void _glfwPlatformSetTlsPOSIX(_GLFWtls* tls, void* value)
    {
        pthread_setspecific(tls->key, value);
    }

    static int _glfwPlatformCreateMutexPOSIX(_GLFWmutex* mutex)
    {
        if (pthread_mutex_init(&mutex->posix, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "POSIX: Failed to create mutex");
            return GLFW_FALSE;
        }

        mutex->allocated = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void _glfwPlatformDestroyMutexPOSIX(_GLFWmutex* mutex)
    {
        if (mutex->allocated != 0)
            pthread_mutex_destroy(&mutex->posix);
        *mutex = default;
    }

    static void _glfwPlatformLockMutexPOSIX(_GLFWmutex* mutex)
    {
        pthread_mutex_lock(&mutex->posix);
    }

    static void _glfwPlatformUnlockMutexPOSIX(_GLFWmutex* mutex)
    {
        pthread_mutex_unlock(&mutex->posix);
    }

    static void* _glfwPlatformLoadModulePOSIX(byte* path)
    {
        return dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    }

    static void _glfwPlatformFreeModulePOSIX(void* module)
    {
        if (module != null)
            dlclose(module);
    }

    static void* _glfwPlatformGetModuleSymbolPOSIX(void* module, byte* name)
    {
        return dlsym(module, name);
    }

    static int _glfwPollPOSIX(POLLFD* fds, nuint count, double* timeout)
    {
        for (;;)
        {
            if (timeout != null)
            {
                var before = _glfwPlatformGetTimerValue();
                var wholeSeconds = (long)*timeout;
                var seconds = (nint)wholeSeconds;
                var nanoseconds = (nint)((*timeout - wholeSeconds) * 1e9);
                var ts = new TIMESPEC { tv_sec = seconds, tv_nsec = nanoseconds };
                var result = ppoll(fds, count, &ts, null);
                var error = Marshal.GetLastPInvokeError();

                *timeout -= (_glfwPlatformGetTimerValue() - before) /
                            (double)_glfwPlatformGetTimerFrequency();

                if (result > 0)
                    return GLFW_TRUE;
                if (result == -1 && error != EINTR && error != EAGAIN)
                    return GLFW_FALSE;
                if (*timeout <= 0.0)
                    return GLFW_FALSE;
            }
            else
            {
                var result = poll(fds, count, -1);
                var error = Marshal.GetLastPInvokeError();
                if (result > 0)
                    return GLFW_TRUE;
                if (result == -1 && error != EINTR && error != EAGAIN)
                    return GLFW_FALSE;
            }
        }
    }

    [DllImport("libc", SetLastError = true)]
    static extern int clock_gettime(int clk_id, TIMESPEC* tp);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_key_create(uint* key, void* destructor);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_key_delete(uint key);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern void* pthread_getspecific(uint key);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_setspecific(uint key, void* value);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_mutex_init(PTHREAD_MUTEX_T* mutex, void* attr);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_mutex_destroy(PTHREAD_MUTEX_T* mutex);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_mutex_lock(PTHREAD_MUTEX_T* mutex);

    [DllImport("libpthread.so.0", SetLastError = true)]
    static extern int pthread_mutex_unlock(PTHREAD_MUTEX_T* mutex);

    [DllImport("libdl.so.2", SetLastError = true)]
    static extern void* dlopen(byte* filename, int flags);

    [DllImport("libdl.so.2", SetLastError = true)]
    static extern int dlclose(void* handle);

    [DllImport("libdl.so.2", SetLastError = true)]
    static extern void* dlsym(void* handle, byte* symbol);

    [DllImport("libc", SetLastError = true)]
    static extern int poll(POLLFD* fds, nuint nfds, int timeout);

    [DllImport("libc", SetLastError = true)]
    static extern int ppoll(POLLFD* fds, nuint nfds, TIMESPEC* timeout, void* sigmask);
}
