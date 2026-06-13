using System;

namespace NGlfw;

public static unsafe partial class Glfw
{
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
}
