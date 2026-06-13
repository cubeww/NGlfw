using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int WL_OUTPUT_MODE_CURRENT = 0x1;
    const uint WL_OUTPUT_NAME_SINCE_VERSION = 4;
    const uint WL_OUTPUT_RELEASE_SINCE_VERSION = 3;

    struct wl_output_listener
    {
        public delegate* unmanaged<void*, void*, int, int, int, int, int, byte*, byte*, int, void> geometry;
        public delegate* unmanaged<void*, void*, uint, int, int, int, void> mode;
        public delegate* unmanaged<void*, void*, void> done;
        public delegate* unmanaged<void*, void*, int, void> scale;
        public delegate* unmanaged<void*, void*, byte*, void> name;
        public delegate* unmanaged<void*, void*, byte*, void> description;
    }

    static wl_output_listener* _glfwWaylandOutputListener;

    static wl_output_listener* wayland_getOutputListener()
    {
        if (_glfwWaylandOutputListener == null)
        {
            _glfwWaylandOutputListener = (wl_output_listener*)_glfw_calloc(1, (nuint)sizeof(wl_output_listener));
            if (_glfwWaylandOutputListener != null)
            {
                _glfwWaylandOutputListener->geometry = &wayland_outputHandleGeometry;
                _glfwWaylandOutputListener->mode = &wayland_outputHandleMode;
                _glfwWaylandOutputListener->done = &wayland_outputHandleDone;
                _glfwWaylandOutputListener->scale = &wayland_outputHandleScale;
                _glfwWaylandOutputListener->name = &wayland_outputHandleName;
                _glfwWaylandOutputListener->description = &wayland_outputHandleDescription;
            }
        }

        return _glfwWaylandOutputListener;
    }

    static void wayland_outputDestroy(void* output)
    {
        if (output == null)
            return;

        if (_glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(output) >= WL_OUTPUT_RELEASE_SINCE_VERSION)
        {
            wayland_proxyDestroyWithOpcode(output, WL_OUTPUT_RELEASE);
        }
        else
            wayland_proxyDestroy(output);
    }

    static void wayland_copyOutputMakeModel(byte* destination, byte* make, byte* model)
    {
        var index = 0;

        if (make != null)
        {
            for (; index < 127 && make[index] != 0; index++)
                destination[index] = make[index];
        }

        if (index < 127)
            destination[index++] = (byte)' ';

        if (model != null)
        {
            for (var i = 0; index < 127 && model[i] != 0; i++, index++)
                destination[index] = model[i];
        }

        destination[index] = 0;
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleGeometry(void* userData,
                                             void* output,
                                             int x,
                                             int y,
                                             int physicalWidth,
                                             int physicalHeight,
                                             int subpixel,
                                             byte* make,
                                             byte* model,
                                             int transform)
    {
        var monitor = (_GLFWmonitor*)userData;

        monitor->wl.x = x;
        monitor->wl.y = y;
        monitor->widthMM = physicalWidth;
        monitor->heightMM = physicalHeight;

        if (monitor->name[0] == 0)
            wayland_copyOutputMakeModel(monitor->name, make, model);
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleMode(void* userData,
                                         void* output,
                                         uint flags,
                                         int width,
                                         int height,
                                         int refresh)
    {
        var monitor = (_GLFWmonitor*)userData;
        var mode = new GLFWvidmode
        {
            width = width,
            height = height,
            redBits = 8,
            greenBits = 8,
            blueBits = 8,
            refreshRate = (int)System.Math.Round(refresh / 1000.0)
        };

        var newCount = monitor->modeCount + 1;
        var modes = (GLFWvidmode*)_glfw_realloc(monitor->modes, (nuint)(newCount * sizeof(GLFWvidmode)));
        if (modes == null)
            return;

        monitor->modes = modes;
        monitor->modeCount = newCount;
        monitor->modes[newCount - 1] = mode;

        if ((flags & WL_OUTPUT_MODE_CURRENT) != 0)
        {
            monitor->wl.currentMode = newCount - 1;
            monitor->currentMode = mode;
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleDone(void* userData, void* output)
    {
        var monitor = (_GLFWmonitor*)userData;

        if (monitor->modeCount == 0)
            return;

        if (monitor->widthMM <= 0 || monitor->heightMM <= 0)
        {
            var mode = monitor->modes + monitor->wl.currentMode;
            monitor->widthMM = (int)(mode->width * 25.4f / 96f);
            monitor->heightMM = (int)(mode->height * 25.4f / 96f);
        }

        for (var i = 0; i < _glfw.monitorCount; i++)
        {
            if (_glfw.monitors[i] == monitor)
                return;
        }

        _glfwInputMonitor(monitor, GLFW_CONNECTED, _GLFW_INSERT_LAST);
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleScale(void* userData, void* output, int factor)
    {
        var monitor = (_GLFWmonitor*)userData;
        monitor->wl.scale = factor;

        for (var window = _glfw.windowListHead; window != null; window = window->next)
        {
            for (nuint i = 0; i < window->wl.outputScaleCount; i++)
            {
                if (window->wl.outputScales[i].output == monitor->wl.output)
                {
                    window->wl.outputScales[i].factor = factor;
                    _glfwUpdateBufferScaleFromOutputsWayland(window);
                    break;
                }
            }
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleName(void* userData, void* output, byte* name)
    {
        var monitor = (_GLFWmonitor*)userData;
        _glfw_strncpy(monitor->name, name, 128);
    }

    [UnmanagedCallersOnly]
    static void wayland_outputHandleDescription(void* userData, void* output, byte* description)
    {
    }

    static void _glfwAddOutputWayland(uint name, uint version)
    {
        if (version < 2)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Unsupported output interface version");
            return;
        }

        version = wayland_min(version, WL_OUTPUT_NAME_SINCE_VERSION);

        var output = wayland_registryBind(_glfw.wl.registry,
            name,
            _glfw.wl.client.outputInterface,
            _glfwWaylandWlOutput,
            version);

        if (output == null)
            return;

        var monitor = _glfwAllocMonitor("", 0, 0);
        if (monitor == null)
        {
            wayland_outputDestroy(output);
            return;
        }

        monitor->wl.scale = 1;
        monitor->wl.output = output;
        monitor->wl.name = name;

        var outputListener = wayland_getOutputListener();
        if (outputListener == null ||
            _glfw.wl.client.proxy_add_listener(output, outputListener, monitor) != 0)
        {
            _glfwFreeMonitor(monitor);
        }
    }

    static void _glfwFreeMonitorWayland(_GLFWmonitor* monitor)
    {
        wayland_outputDestroy(monitor->wl.output);
    }

    static void _glfwGetMonitorPosWayland(_GLFWmonitor* monitor, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = monitor->wl.x;
        if (ypos != null)
            *ypos = monitor->wl.y;
    }

    static void _glfwGetMonitorContentScaleWayland(_GLFWmonitor* monitor, float* xscale, float* yscale)
    {
        var scale = monitor->wl.scale != 0 ? monitor->wl.scale : 1;

        if (xscale != null)
            *xscale = scale;
        if (yscale != null)
            *yscale = scale;
    }

    static void _glfwGetMonitorWorkareaWayland(_GLFWmonitor* monitor,
                                               int* xpos,
                                               int* ypos,
                                               int* width,
                                               int* height)
    {
        if (xpos != null)
            *xpos = monitor->wl.x;
        if (ypos != null)
            *ypos = monitor->wl.y;

        if (monitor->modes != null && monitor->modeCount > 0)
        {
            var index = monitor->wl.currentMode;
            if (index < 0 || index >= monitor->modeCount)
                index = 0;

            if (width != null)
                *width = monitor->modes[index].width;
            if (height != null)
                *height = monitor->modes[index].height;
        }
        else
        {
            if (width != null)
                *width = 0;
            if (height != null)
                *height = 0;
        }
    }

    static GLFWvidmode* _glfwGetVideoModesWayland(_GLFWmonitor* monitor, int* found)
    {
        if (found != null)
            *found = monitor->modeCount;
        return monitor->modes;
    }

    static int _glfwGetVideoModeWayland(_GLFWmonitor* monitor, GLFWvidmode* mode)
    {
        if (monitor->modes == null || monitor->modeCount == 0)
            return GLFW_FALSE;

        var index = monitor->wl.currentMode;
        if (index < 0 || index >= monitor->modeCount)
            index = 0;

        if (mode != null)
            *mode = monitor->modes[index];

        return GLFW_TRUE;
    }

    static int _glfwGetGammaRampWayland(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: Gamma ramp access is not available");
        return GLFW_FALSE;
    }

    static void _glfwSetGammaRampWayland(_GLFWmonitor* monitor, GLFWgammaramp* ramp)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: Gamma ramp access is not available");
    }

    public static void* glfwGetWaylandMonitor(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WAYLAND)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not initialized");
            return null;
        }

        return ((_GLFWmonitor*)monitor)->wl.output;
    }
}
