using System;
namespace NGlfw;

public static unsafe partial class Glfw
{
    static void _glfwPollMonitorsNull()
    {
        const float dpi = 141f;
        var mode = null_getVideoMode();
        var monitor = _glfwAllocMonitor("Null SuperNoop 0",
            (int)(mode.width * 25.4f / dpi),
            (int)(mode.height * 25.4f / dpi));

        monitor->currentMode = mode;
        _glfwInputMonitor(monitor, GLFW_CONNECTED, _GLFW_INSERT_FIRST);
    }

    static void _glfwSetGammaRampNull(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (monitor->@null.ramp.size != ramp->size)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Null: Gamma ramp size must match current ramp size");
            return;
        }

        _glfw_memcpy(monitor->@null.ramp.red, ramp->red, sizeof(ushort) * ramp->size);
        _glfw_memcpy(monitor->@null.ramp.green, ramp->green, sizeof(ushort) * ramp->size);
        _glfw_memcpy(monitor->@null.ramp.blue, ramp->blue, sizeof(ushort) * ramp->size);
    }

    static int _glfwInitJoysticksNull()
    {
        return GLFW_TRUE;
    }

    static void _glfwFreeMonitor(_GLFWmonitor* monitor)
    {
        if (_glfw.platform.freeMonitor != null)
            _glfw.platform.freeMonitor(monitor);
        _glfw_free(monitor->modes);
        _glfwFreeGammaArrays(&monitor->originalRamp);
        _glfwFreeGammaArrays(&monitor->currentRamp);
        _glfw_free(monitor);
    }

    static void _glfwFreeMonitorNull(_GLFWmonitor* monitor)
    {
        _glfwFreeGammaArrays(&monitor->@null.ramp);
    }

    static void _glfwGetMonitorPosNull(_GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;
    }

    static void _glfwGetMonitorContentScaleNull(_GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 1f;
        if (yscale != null)
            *yscale = 1f;
    }

    static void _glfwGetMonitorWorkareaNull(_GLFWmonitor* monitor, int* xpos, int* ypos, int* width, int* height)
    {
        var mode = null_getVideoMode();

        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 10;
        if (width != null)
            *width = mode.width;
        if (height != null)
            *height = mode.height - 10;
    }

    static GLFWvidmode* _glfwGetVideoModesNull(_GLFWmonitor* monitor, int* found)
    {
        var modes = (GLFWvidmode*)_glfw_calloc(1, (nuint)sizeof(GLFWvidmode));
        *modes = null_getVideoMode();
        if (found != null)
            *found = 1;
        return modes;
    }

    static int _glfwGetVideoModeNull(_GLFWmonitor* monitor, GLFWvidmode* mode)
    {
        if (mode != null)
            *mode = null_getVideoMode();
        return GLFW_TRUE;
    }

    static int _glfwGetGammaRampNull(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (monitor->@null.ramp.size == 0)
        {
            _glfwAllocGammaArrays(&monitor->@null.ramp, 256);

            for (var i = 0u; i < monitor->@null.ramp.size; i++)
            {
                const float gamma = 2.2f;
                var value = i / (float)(monitor->@null.ramp.size - 1);
                value = MathF.Pow(value, 1f / gamma) * 65535f + 0.5f;
                value = MathF.Min(value, 65535f);

                monitor->@null.ramp.red[i] = (ushort)value;
                monitor->@null.ramp.green[i] = (ushort)value;
                monitor->@null.ramp.blue[i] = (ushort)value;
            }
        }

        if (ramp != null)
        {
            _glfwAllocGammaArrays(ramp, monitor->@null.ramp.size);
            _glfw_memcpy(ramp->red, monitor->@null.ramp.red, sizeof(ushort) * ramp->size);
            _glfw_memcpy(ramp->green, monitor->@null.ramp.green, sizeof(ushort) * ramp->size);
            _glfw_memcpy(ramp->blue, monitor->@null.ramp.blue, sizeof(ushort) * ramp->size);
        }

        return GLFW_TRUE;
    }

    static GLFWvidmode null_getVideoMode()
    {
        GLFWvidmode mode;
        mode.width = 1920;
        mode.height = 1080;
        mode.redBits = 8;
        mode.greenBits = 8;
        mode.blueBits = 8;
        mode.refreshRate = 60;
        return mode;
    }

    static void _glfwAllocGammaArrays(GLFWgammaramp* ramp, uint size)
    {
        ramp->red = (ushort*)_glfw_calloc(size, sizeof(ushort));
        ramp->green = (ushort*)_glfw_calloc(size, sizeof(ushort));
        ramp->blue = (ushort*)_glfw_calloc(size, sizeof(ushort));
        ramp->size = size;
    }

    static void _glfwFreeGammaArrays(GLFWgammaramp* ramp)
    {
        if (ramp == null)
            return;

        _glfw_free(ramp->red);
        _glfw_free(ramp->green);
        _glfw_free(ramp->blue);
        *ramp = default;
    }

    static void _glfwInputMonitorWindow(_GLFWmonitor* monitor, _GLFWwindow* window)
    {
        if (monitor != null)
            monitor->window = window;
    }

    static int _glfwPollJoystickNull(_GLFWjoystick* js, int mode)
    {
        return GLFW_FALSE;
    }

    static byte* _glfwGetMappingNameNull()
    {
        return null;
    }

    static void _glfwUpdateGamepadGUIDNull(byte* guid)
    {
    }

    static void _glfwTerminateJoysticksNull()
    {
    }

    static void _glfwCenterCursorInContentArea(_GLFWwindow* window)
    {
        var width = 0;
        var height = 0;
        if (_glfw.platform.getWindowSize != null)
            _glfw.platform.getWindowSize(window, &width, &height);
        if (_glfw.platform.setCursorPos != null)
            _glfw.platform.setCursorPos(window, width / 2.0, height / 2.0);
    }

    static void _glfwInputKey(_GLFWwindow* window, int key, int scancode, int action, int mods)
    {
        if (key >= 0 && key <= GLFW_KEY_LAST)
        {
            var repeated = GLFW_FALSE;

            if (action == GLFW_RELEASE && window->keys[key] == GLFW_RELEASE)
                return;

            if (action == GLFW_PRESS && window->keys[key] == GLFW_PRESS)
                repeated = GLFW_TRUE;

            if (action == GLFW_RELEASE && window->stickyKeys != 0)
                window->keys[key] = _GLFW_STICK;
            else
                window->keys[key] = (byte)action;

            if (repeated != 0)
                action = GLFW_REPEAT;
        }

        if (window->lockKeyMods == 0)
            mods &= ~(GLFW_MOD_CAPS_LOCK | GLFW_MOD_NUM_LOCK);

        if (window->callbacks.key != null)
            window->callbacks.key((GLFWwindow*)window, key, scancode, action, mods);
    }

    static void _glfwInputMouseClick(_GLFWwindow* window, int button, int action, int mods)
    {
        if (button < 0 || button > GLFW_MOUSE_BUTTON_LAST)
            return;

        if (window->lockKeyMods == 0)
            mods &= ~(GLFW_MOD_CAPS_LOCK | GLFW_MOD_NUM_LOCK);

        if (button >= 0 && button <= GLFW_MOUSE_BUTTON_LAST)
        {
            if (action == GLFW_RELEASE && window->stickyMouseButtons != 0)
                window->mouseButtons[button] = _GLFW_STICK;
            else
                window->mouseButtons[button] = (byte)action;
        }

        if (window->callbacks.mouseButton != null)
            window->callbacks.mouseButton((GLFWwindow*)window, button, action, mods);
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
}
