using System;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static void _glfwInputMonitor(_GLFWmonitor* monitor, int action, int placement)
    {
        if (action == GLFW_CONNECTED)
        {
            _glfw.monitorCount++;
            _glfw.monitors = (_GLFWmonitor**)_glfw_realloc(_glfw.monitors,
                (nuint)(sizeof(_GLFWmonitor*) * _glfw.monitorCount));

            if (placement == _GLFW_INSERT_FIRST)
            {
                for (var i = _glfw.monitorCount - 1; i > 0; i--)
                    _glfw.monitors[i] = _glfw.monitors[i - 1];
                _glfw.monitors[0] = monitor;
            }
            else
                _glfw.monitors[_glfw.monitorCount - 1] = monitor;
        }
        else if (action == GLFW_DISCONNECTED)
        {
            for (var window = _glfw.windowListHead; window != null; window = window->next)
            {
                if (window->monitor == monitor)
                {
                    int width;
                    int height;
                    int xoff;
                    int yoff;

                    _glfw.platform.getWindowSize(window, &width, &height);
                    _glfw.platform.setWindowMonitor(window, null, 0, 0, width, height, 0);
                    _glfw.platform.getWindowFrameSize(window, &xoff, &yoff, null, null);
                    _glfw.platform.setWindowPos(window, xoff, yoff);
                }
            }

            for (var i = 0; i < _glfw.monitorCount; i++)
            {
                if (_glfw.monitors[i] != monitor)
                    continue;

                _glfw.monitorCount--;
                for (var j = i; j < _glfw.monitorCount; j++)
                    _glfw.monitors[j] = _glfw.monitors[j + 1];
                break;
            }
        }

        if (_glfw.callbacks.monitor != null)
            _glfw.callbacks.monitor((GLFWmonitor*)monitor, action);

        if (action == GLFW_DISCONNECTED)
            _glfwFreeMonitor(monitor);
    }

    static _GLFWmonitor* _glfwAllocMonitor(string name, int widthMM, int heightMM)
    {
        var monitor = (_GLFWmonitor*)_glfw_calloc(1, (nuint)sizeof(_GLFWmonitor));
        if (monitor == null)
            return null;

        monitor->widthMM = widthMM;
        monitor->heightMM = heightMM;
        _glfw_strcpy(monitor->name, name);

        return monitor;
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

    static int monitor_refreshVideoModes(_GLFWmonitor* monitor)
    {
        if (monitor->modes != null)
            return GLFW_TRUE;

        int modeCount;
        var modes = _glfw.platform.getVideoModes(monitor, &modeCount);
        if (modes == null)
            return GLFW_FALSE;

        _glfw_free(monitor->modes);
        monitor->modes = modes;
        monitor->modeCount = modeCount;
        return GLFW_TRUE;
    }

    static GLFWvidmode* _glfwChooseVideoMode(_GLFWmonitor* monitor, GLFWvidmode* desired)
    {
        uint leastSizeDiff = uint.MaxValue;
        uint leastRateDiff = uint.MaxValue;
        uint leastColorDiff = uint.MaxValue;
        GLFWvidmode* closest = null;

        if (monitor_refreshVideoModes(monitor) == 0)
            return null;

        for (var i = 0; i < monitor->modeCount; i++)
        {
            var current = monitor->modes + i;
            uint colorDiff = 0;

            if (desired->redBits != GLFW_DONT_CARE)
                colorDiff += (uint)Math.Abs(current->redBits - desired->redBits);
            if (desired->greenBits != GLFW_DONT_CARE)
                colorDiff += (uint)Math.Abs(current->greenBits - desired->greenBits);
            if (desired->blueBits != GLFW_DONT_CARE)
                colorDiff += (uint)Math.Abs(current->blueBits - desired->blueBits);

            var widthDiff = current->width - desired->width;
            var heightDiff = current->height - desired->height;
            var sizeDiff = (uint)Math.Abs(widthDiff * widthDiff + heightDiff * heightDiff);

            uint rateDiff = desired->refreshRate != GLFW_DONT_CARE
                ? (uint)Math.Abs(current->refreshRate - desired->refreshRate)
                : uint.MaxValue - (uint)current->refreshRate;

            if (colorDiff < leastColorDiff ||
                (colorDiff == leastColorDiff && sizeDiff < leastSizeDiff) ||
                (colorDiff == leastColorDiff && sizeDiff == leastSizeDiff && rateDiff < leastRateDiff))
            {
                closest = current;
                leastSizeDiff = sizeDiff;
                leastRateDiff = rateDiff;
                leastColorDiff = colorDiff;
            }
        }

        return closest;
    }

    static int _glfwCompareVideoModes(GLFWvidmode* fm, GLFWvidmode* sm)
    {
        var fbpp = fm->redBits + fm->greenBits + fm->blueBits;
        var sbpp = sm->redBits + sm->greenBits + sm->blueBits;
        var farea = fm->width * fm->height;
        var sarea = sm->width * sm->height;

        if (fbpp != sbpp)
            return fbpp - sbpp;
        if (farea != sarea)
            return farea - sarea;
        if (fm->width != sm->width)
            return fm->width - sm->width;

        return fm->refreshRate - sm->refreshRate;
    }

    static void _glfwSplitBPP(int bpp, int* red, int* green, int* blue)
    {
        if (bpp == 32)
            bpp = 24;

        *red = *green = *blue = bpp / 3;
        var delta = bpp - *red * 3;

        if (delta >= 1)
            *green = *green + 1;
        if (delta == 2)
            *red = *red + 1;
    }

    public static GLFWmonitor** glfwGetMonitors(int* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (count != null)
            *count = _glfw.monitorCount;

        return (GLFWmonitor**)_glfw.monitors;
    }

    public static GLFWmonitor* glfwGetPrimaryMonitor()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.monitorCount == 0)
            return null;

        return (GLFWmonitor*)_glfw.monitors[0];
    }

    public static void glfwGetMonitorPos(GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getMonitorPos((_GLFWmonitor*)monitor, xpos, ypos);
    }

    public static void glfwGetMonitorWorkarea(GLFWmonitor* monitor, int* xpos, int* ypos, int* width, int* height)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;
        if (width != null)
            *width = 0;
        if (height != null)
            *height = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getMonitorWorkarea((_GLFWmonitor*)monitor, xpos, ypos, width, height);
    }

    public static void glfwGetMonitorPhysicalSize(GLFWmonitor* monitor, int* widthMM, int* heightMM)
    {
        if (widthMM != null)
            *widthMM = 0;
        if (heightMM != null)
            *heightMM = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalMonitor = (_GLFWmonitor*)monitor;
        if (widthMM != null)
            *widthMM = internalMonitor->widthMM;
        if (heightMM != null)
            *heightMM = internalMonitor->heightMM;
    }

    public static void glfwGetMonitorContentScale(GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        if (xscale != null)
            *xscale = 0f;
        if (yscale != null)
            *yscale = 0f;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _glfw.platform.getMonitorContentScale((_GLFWmonitor*)monitor, xscale, yscale);
    }

    public static byte* glfwGetMonitorName(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return ((_GLFWmonitor*)monitor)->name;
    }

    public static void glfwSetMonitorUserPointer(GLFWmonitor* monitor, void* pointer)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        ((_GLFWmonitor*)monitor)->userPointer = pointer;
    }

    public static void* glfwGetMonitorUserPointer(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return ((_GLFWmonitor*)monitor)->userPointer;
    }

    public static delegate*<GLFWmonitor*, int, void> glfwSetMonitorCallback(delegate*<GLFWmonitor*, int, void> cbfun)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var previous = _glfw.callbacks.monitor;
        _glfw.callbacks.monitor = cbfun;
        return previous;
    }

    public static GLFWvidmode* glfwGetVideoModes(GLFWmonitor* monitor, int* count)
    {
        if (count != null)
            *count = 0;

        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalMonitor = (_GLFWmonitor*)monitor;
        if (monitor_refreshVideoModes(internalMonitor) == 0)
            return null;

        if (count != null)
            *count = internalMonitor->modeCount;

        return internalMonitor->modes;
    }

    public static GLFWvidmode* glfwGetVideoMode(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalMonitor = (_GLFWmonitor*)monitor;
        if (_glfw.platform.getVideoMode(internalMonitor, &internalMonitor->currentMode) == 0)
            return null;

        return &internalMonitor->currentMode;
    }

    public static void glfwSetGamma(GLFWmonitor* monitor, float gamma)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (float.IsNaN(gamma) || gamma <= 0f || float.IsInfinity(gamma))
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid gamma value {0}", gamma);
            return;
        }

        var original = glfwGetGammaRamp(monitor);
        if (original == null)
            return;

        var values = (ushort*)_glfw_calloc(original->size, sizeof(ushort));

        for (var i = 0u; i < original->size; i++)
        {
            var value = i / (float)(original->size - 1);
            value = MathF.Pow(value, 1f / gamma) * 65535f + 0.5f;
            value = MathF.Min(value, 65535f);
            values[i] = (ushort)value;
        }

        GLFWgammaramp ramp;
        ramp.red = values;
        ramp.green = values;
        ramp.blue = values;
        ramp.size = original->size;

        glfwSetGammaRamp(monitor, &ramp);
        _glfw_free(values);
    }

    public static GLFWgammaramp* glfwGetGammaRamp(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalMonitor = (_GLFWmonitor*)monitor;
        _glfwFreeGammaArrays(&internalMonitor->currentRamp);

        if (_glfw.platform.getGammaRamp(internalMonitor, &internalMonitor->currentRamp) == 0)
            return null;

        return &internalMonitor->currentRamp;
    }

    public static void glfwSetGammaRamp(GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        if (ramp == null || ramp->size == 0 || ramp->red == null || ramp->green == null || ramp->blue == null)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Invalid gamma ramp");
            return;
        }

        var internalMonitor = (_GLFWmonitor*)monitor;

        if (internalMonitor->originalRamp.size == 0)
        {
            if (_glfw.platform.getGammaRamp(internalMonitor, &internalMonitor->originalRamp) == 0)
                return;
        }

        _glfw.platform.setGammaRamp(internalMonitor, ramp);
    }
}
