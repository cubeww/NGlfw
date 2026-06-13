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
        public fixed byte storage[64];
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
        if (posix_clock_gettime(CLOCK_MONOTONIC, &ts) == 0)
            _glfw.timer.posixClock = CLOCK_MONOTONIC;
    }

    static ulong _glfwPlatformGetTimerValuePOSIX()
    {
        TIMESPEC ts;
        posix_clock_gettime(_glfw.timer.posixClock, &ts);
        return (ulong)ts.tv_sec * _glfw.timer.frequency + (ulong)ts.tv_nsec;
    }

    static ulong _glfwPlatformGetTimerFrequencyPOSIX()
    {
        return _glfw.timer.frequency;
    }

    static int _glfwPlatformCreateTlsPOSIX(_GLFWtls* tls)
    {
        nuint key;
        if (posix_pthread_key_create(&key) != 0)
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
            posix_pthread_key_delete(tls->key);
        *tls = default;
    }

    static void* _glfwPlatformGetTlsPOSIX(_GLFWtls* tls)
    {
        return posix_pthread_getspecific(tls->key);
    }

    static void _glfwPlatformSetTlsPOSIX(_GLFWtls* tls, void* value)
    {
        posix_pthread_setspecific(tls->key, value);
    }

    static int _glfwPlatformCreateMutexPOSIX(_GLFWmutex* mutex)
    {
        if (posix_pthread_mutex_init(&mutex->posix) != 0)
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
            posix_pthread_mutex_destroy(&mutex->posix);
        *mutex = default;
    }

    static void _glfwPlatformLockMutexPOSIX(_GLFWmutex* mutex)
    {
        posix_pthread_mutex_lock(&mutex->posix);
    }

    static void _glfwPlatformUnlockMutexPOSIX(_GLFWmutex* mutex)
    {
        posix_pthread_mutex_unlock(&mutex->posix);
    }

    static void* _glfwPlatformLoadModulePOSIX(byte* path)
    {
        return posix_dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    }

    static void _glfwPlatformFreeModulePOSIX(void* module)
    {
        if (module != null)
            posix_dlclose(module);
    }

    static void* _glfwPlatformGetModuleSymbolPOSIX(void* module, byte* name)
    {
        return posix_dlsym(module, name);
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
                var result = OperatingSystem.IsMacOS()
                    ? poll_macos(fds, count, (int)(wholeSeconds * 1000 + nanoseconds / 1000000))
                    : ppoll_linux(fds, count, &ts, null);
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
                var result = OperatingSystem.IsMacOS()
                    ? poll_macos(fds, count, -1)
                    : poll_linux(fds, count, -1);
                var error = Marshal.GetLastPInvokeError();
                if (result > 0)
                    return GLFW_TRUE;
                if (result == -1 && error != EINTR && error != EAGAIN)
                    return GLFW_FALSE;
            }
        }
    }

    static int posix_clock_gettime(int clk_id, TIMESPEC* tp)
    {
        return OperatingSystem.IsMacOS()
            ? clock_gettime_macos(clk_id, tp)
            : clock_gettime_linux(clk_id, tp);
    }

    static int posix_pthread_key_create(nuint* key)
    {
        if (OperatingSystem.IsMacOS())
            return pthread_key_create_macos(key, null);

        uint linuxKey;
        var result = pthread_key_create_linux(&linuxKey, null);
        *key = linuxKey;
        return result;
    }

    static int posix_pthread_key_delete(nuint key)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_key_delete_macos(key)
            : pthread_key_delete_linux((uint)key);
    }

    static void* posix_pthread_getspecific(nuint key)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_getspecific_macos(key)
            : pthread_getspecific_linux((uint)key);
    }

    static int posix_pthread_setspecific(nuint key, void* value)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_setspecific_macos(key, value)
            : pthread_setspecific_linux((uint)key, value);
    }

    static int posix_pthread_mutex_init(PTHREAD_MUTEX_T* mutex)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_mutex_init_macos(mutex, null)
            : pthread_mutex_init_linux(mutex, null);
    }

    static int posix_pthread_mutex_destroy(PTHREAD_MUTEX_T* mutex)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_mutex_destroy_macos(mutex)
            : pthread_mutex_destroy_linux(mutex);
    }

    static int posix_pthread_mutex_lock(PTHREAD_MUTEX_T* mutex)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_mutex_lock_macos(mutex)
            : pthread_mutex_lock_linux(mutex);
    }

    static int posix_pthread_mutex_unlock(PTHREAD_MUTEX_T* mutex)
    {
        return OperatingSystem.IsMacOS()
            ? pthread_mutex_unlock_macos(mutex)
            : pthread_mutex_unlock_linux(mutex);
    }

    static void* posix_dlopen(byte* filename, int flags)
    {
        return OperatingSystem.IsMacOS()
            ? dlopen_macos(filename, flags)
            : dlopen_linux(filename, flags);
    }

    static int posix_dlclose(void* handle)
    {
        return OperatingSystem.IsMacOS()
            ? dlclose_macos(handle)
            : dlclose_linux(handle);
    }

    static void* posix_dlsym(void* handle, byte* symbol)
    {
        return OperatingSystem.IsMacOS()
            ? dlsym_macos(handle, symbol)
            : dlsym_linux(handle, symbol);
    }

    [DllImport("libc", EntryPoint = "clock_gettime", SetLastError = true)]
    static extern int clock_gettime_linux(int clk_id, TIMESPEC* tp);

    [DllImport("libSystem.B.dylib", EntryPoint = "clock_gettime", SetLastError = true)]
    static extern int clock_gettime_macos(int clk_id, TIMESPEC* tp);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_key_create", SetLastError = true)]
    static extern int pthread_key_create_linux(uint* key, void* destructor);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_key_create", SetLastError = true)]
    static extern int pthread_key_create_macos(nuint* key, void* destructor);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_key_delete", SetLastError = true)]
    static extern int pthread_key_delete_linux(uint key);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_key_delete", SetLastError = true)]
    static extern int pthread_key_delete_macos(nuint key);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_getspecific", SetLastError = true)]
    static extern void* pthread_getspecific_linux(uint key);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_getspecific", SetLastError = true)]
    static extern void* pthread_getspecific_macos(nuint key);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_setspecific", SetLastError = true)]
    static extern int pthread_setspecific_linux(uint key, void* value);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_setspecific", SetLastError = true)]
    static extern int pthread_setspecific_macos(nuint key, void* value);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_mutex_init", SetLastError = true)]
    static extern int pthread_mutex_init_linux(PTHREAD_MUTEX_T* mutex, void* attr);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_mutex_init", SetLastError = true)]
    static extern int pthread_mutex_init_macos(PTHREAD_MUTEX_T* mutex, void* attr);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_mutex_destroy", SetLastError = true)]
    static extern int pthread_mutex_destroy_linux(PTHREAD_MUTEX_T* mutex);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_mutex_destroy", SetLastError = true)]
    static extern int pthread_mutex_destroy_macos(PTHREAD_MUTEX_T* mutex);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_mutex_lock", SetLastError = true)]
    static extern int pthread_mutex_lock_linux(PTHREAD_MUTEX_T* mutex);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_mutex_lock", SetLastError = true)]
    static extern int pthread_mutex_lock_macos(PTHREAD_MUTEX_T* mutex);

    [DllImport("libpthread.so.0", EntryPoint = "pthread_mutex_unlock", SetLastError = true)]
    static extern int pthread_mutex_unlock_linux(PTHREAD_MUTEX_T* mutex);

    [DllImport("libSystem.B.dylib", EntryPoint = "pthread_mutex_unlock", SetLastError = true)]
    static extern int pthread_mutex_unlock_macos(PTHREAD_MUTEX_T* mutex);

    [DllImport("libdl.so.2", EntryPoint = "dlopen", SetLastError = true)]
    static extern void* dlopen_linux(byte* filename, int flags);

    [DllImport("libSystem.B.dylib", EntryPoint = "dlopen", SetLastError = true)]
    static extern void* dlopen_macos(byte* filename, int flags);

    [DllImport("libdl.so.2", EntryPoint = "dlclose", SetLastError = true)]
    static extern int dlclose_linux(void* handle);

    [DllImport("libSystem.B.dylib", EntryPoint = "dlclose", SetLastError = true)]
    static extern int dlclose_macos(void* handle);

    [DllImport("libdl.so.2", EntryPoint = "dlsym", SetLastError = true)]
    static extern void* dlsym_linux(void* handle, byte* symbol);

    [DllImport("libSystem.B.dylib", EntryPoint = "dlsym", SetLastError = true)]
    static extern void* dlsym_macos(void* handle, byte* symbol);

    [DllImport("libc", EntryPoint = "poll", SetLastError = true)]
    static extern int poll_linux(POLLFD* fds, nuint nfds, int timeout);

    [DllImport("libSystem.B.dylib", EntryPoint = "poll", SetLastError = true)]
    static extern int poll_macos(POLLFD* fds, nuint nfds, int timeout);

    [DllImport("libc", EntryPoint = "ppoll", SetLastError = true)]
    static extern int ppoll_linux(POLLFD* fds, nuint nfds, TIMESPEC* timeout, void* sigmask);
}
