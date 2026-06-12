namespace NGlfw;

public static unsafe partial class Glfw
{
    const int RR_Connected = 0;
    const int RRNotify = 1;
    const int RROutputChangeNotifyMask = 1 << 2;
    const ulong RR_Interlace = 0x10;
    const ushort RR_Rotate_90 = 2;
    const ushort RR_Rotate_270 = 8;

    static int x11_randrAvailable()
    {
        return _glfw.x11.randrAvailable != 0 &&
               _glfw.x11.randrMonitorBroken == 0 &&
               _glfw.x11.XRRGetScreenResourcesCurrent != null &&
               _glfw.x11.XRRFreeScreenResources != null &&
               _glfw.x11.XRRGetOutputInfo != null &&
               _glfw.x11.XRRFreeOutputInfo != null &&
               _glfw.x11.XRRGetCrtcInfo != null &&
               _glfw.x11.XRRFreeCrtcInfo != null
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static XRRModeInfo* x11_getModeInfo(XRRScreenResources* resources, nuint id)
    {
        if (resources == null || resources->modes == null)
            return null;

        for (var i = 0; i < resources->nmode; i++)
        {
            if (resources->modes[i].id == id)
                return resources->modes + i;
        }

        return null;
    }

    static int x11_modeIsGood(XRRModeInfo* mode)
    {
        return mode != null && (mode->modeFlags & RR_Interlace) == 0
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static int x11_calculateRefreshRate(XRRModeInfo* mode)
    {
        if (mode == null || mode->hTotal == 0 || mode->vTotal == 0)
            return 0;

        return (int)System.Math.Round((double)mode->dotClock / (mode->hTotal * (double)mode->vTotal));
    }

    static GLFWvidmode x11_vidmodeFromModeInfo(XRRModeInfo* mode, XRRCrtcInfo* crtc)
    {
        GLFWvidmode result = default;

        if (crtc != null && (crtc->rotation == RR_Rotate_90 || crtc->rotation == RR_Rotate_270))
        {
            result.width = (int)mode->height;
            result.height = (int)mode->width;
        }
        else
        {
            result.width = (int)mode->width;
            result.height = (int)mode->height;
        }

        result.refreshRate = x11_calculateRefreshRate(mode);

        var depth = _glfw.x11.XDefaultDepth != null
            ? _glfw.x11.XDefaultDepth(_glfw.x11.display, _glfw.x11.screen)
            : 24;
        _glfwSplitBPP(depth, &result.redBits, &result.greenBits, &result.blueBits);

        return result;
    }

    static GLFWvidmode x11_getVideoMode()
    {
        var width = _glfw.x11.XDisplayWidth != null
            ? _glfw.x11.XDisplayWidth(_glfw.x11.display, _glfw.x11.screen)
            : 0;
        var height = _glfw.x11.XDisplayHeight != null
            ? _glfw.x11.XDisplayHeight(_glfw.x11.display, _glfw.x11.screen)
            : 0;

        if (width <= 0)
            width = 640;
        if (height <= 0)
            height = 480;

        return new GLFWvidmode
        {
            width = width,
            height = height,
            redBits = 8,
            greenBits = 8,
            blueBits = 8,
            refreshRate = 60
        };
    }

    static nuint x11_getFirstRandRCrtc()
    {
        if (_glfw.x11.XRRGetScreenResourcesCurrent == null ||
            _glfw.x11.XRRFreeScreenResources == null)
        {
            return 0;
        }

        var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
        if (resources == null)
            return 0;

        var crtc = resources->ncrtc > 0 && resources->crtcs != null
            ? resources->crtcs[0]
            : 0;

        _glfw.x11.XRRFreeScreenResources(resources);
        return crtc;
    }

    static int x11_getMonitorIndexFromXinerama(XineramaScreenInfo* screens, int screenCount, XRRCrtcInfo* crtcInfo)
    {
        if (screens == null || crtcInfo == null)
            return -1;

        for (var i = 0; i < screenCount; i++)
        {
            if (screens[i].x_org == crtcInfo->x &&
                screens[i].y_org == crtcInfo->y &&
                screens[i].width == (int)crtcInfo->width &&
                screens[i].height == (int)crtcInfo->height)
            {
                return screens[i].screen_number;
            }
        }

        return -1;
    }

    static void _glfwPollMonitorsX11()
    {
        if (x11_randrAvailable() != 0)
        {
            var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
            if (resources != null)
            {
                var disconnectedCount = _glfw.monitorCount;
                _GLFWmonitor** disconnected = null;
                if (disconnectedCount > 0)
                {
                    disconnected = (_GLFWmonitor**)_glfw_calloc((nuint)disconnectedCount, (nuint)sizeof(_GLFWmonitor*));
                    if (disconnected != null)
                    {
                        _glfw_memcpy(disconnected,
                            _glfw.monitors,
                            (nuint)(disconnectedCount * sizeof(_GLFWmonitor*)));
                    }
                    else
                    {
                        disconnectedCount = 0;
                    }
                }

                var primary = _glfw.x11.XRRGetOutputPrimary != null
                    ? _glfw.x11.XRRGetOutputPrimary(_glfw.x11.display, _glfw.x11.root)
                    : 0;

                var xineramaScreenCount = 0;
                XineramaScreenInfo* xineramaScreens = null;
                if (_glfw.x11.xineramaAvailable != 0 &&
                    _glfw.x11.XineramaQueryScreens != null)
                {
                    xineramaScreens = _glfw.x11.XineramaQueryScreens(_glfw.x11.display, &xineramaScreenCount);
                }

                for (var i = 0; i < resources->noutput; i++)
                {
                    var output = resources->outputs[i];
                    var outputInfo = _glfw.x11.XRRGetOutputInfo(_glfw.x11.display, resources, output);
                    if (outputInfo == null)
                        continue;

                    if (outputInfo->connection != RR_Connected || outputInfo->crtc == 0)
                    {
                        _glfw.x11.XRRFreeOutputInfo(outputInfo);
                        continue;
                    }

                    var existingIndex = -1;
                    for (var j = 0; j < disconnectedCount; j++)
                    {
                        if (disconnected[j] != null &&
                            disconnected[j]->x11.output == output)
                        {
                            existingIndex = j;
                            break;
                        }
                    }

                    var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, outputInfo->crtc);
                    if (crtcInfo == null)
                    {
                        _glfw.x11.XRRFreeOutputInfo(outputInfo);
                        continue;
                    }

                    var randrWidthMM = (int)outputInfo->mm_width;
                    var randrHeightMM = (int)outputInfo->mm_height;
                    if (crtcInfo->rotation == RR_Rotate_90 || crtcInfo->rotation == RR_Rotate_270)
                        (randrWidthMM, randrHeightMM) = (randrHeightMM, randrWidthMM);

                    if (randrWidthMM <= 0 || randrHeightMM <= 0)
                    {
                        randrWidthMM = (int)(crtcInfo->width * 25.4f / 96f);
                        randrHeightMM = (int)(crtcInfo->height * 25.4f / 96f);
                    }

                    var monitorIndex = x11_getMonitorIndexFromXinerama(xineramaScreens, xineramaScreenCount, crtcInfo);
                    if (monitorIndex < 0)
                        monitorIndex = i;

                    var name = outputInfo->name != null && outputInfo->nameLen > 0
                        ? System.Text.Encoding.UTF8.GetString(new System.ReadOnlySpan<byte>(outputInfo->name, outputInfo->nameLen))
                        : $"X11 Output {i}";

                    if (existingIndex >= 0)
                    {
                        var randrMonitor = disconnected[existingIndex];
                        disconnected[existingIndex] = null;

                        randrMonitor->widthMM = randrWidthMM;
                        randrMonitor->heightMM = randrHeightMM;
                        randrMonitor->x11.crtc = outputInfo->crtc;
                        randrMonitor->x11.index = monitorIndex;
                        _glfw_free(randrMonitor->modes);
                        randrMonitor->modes = null;
                        randrMonitor->modeCount = 0;

                        var modeInfo = x11_getModeInfo(resources, crtcInfo->mode);
                        randrMonitor->currentMode = modeInfo != null
                            ? x11_vidmodeFromModeInfo(modeInfo, crtcInfo)
                            : x11_getVideoMode();
                    }
                    else
                    {
                        var randrMonitor = _glfwAllocMonitor(name, randrWidthMM, randrHeightMM);
                        if (randrMonitor == null)
                        {
                            _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                            _glfw.x11.XRRFreeOutputInfo(outputInfo);
                            continue;
                        }

                        randrMonitor->x11.output = output;
                        randrMonitor->x11.crtc = outputInfo->crtc;
                        randrMonitor->x11.index = monitorIndex;

                        var modeInfo = x11_getModeInfo(resources, crtcInfo->mode);
                        randrMonitor->currentMode = modeInfo != null
                            ? x11_vidmodeFromModeInfo(modeInfo, crtcInfo)
                            : x11_getVideoMode();

                        _glfwInputMonitor(randrMonitor,
                            GLFW_CONNECTED,
                            output == primary ? _GLFW_INSERT_FIRST : _GLFW_INSERT_LAST);
                    }

                    _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
                    _glfw.x11.XRRFreeOutputInfo(outputInfo);
                }

                if (xineramaScreens != null && _glfw.x11.XFree != null)
                    _glfw.x11.XFree(xineramaScreens);

                _glfw.x11.XRRFreeScreenResources(resources);

                for (var i = 0; i < disconnectedCount; i++)
                {
                    if (disconnected[i] != null)
                        _glfwInputMonitor(disconnected[i], GLFW_DISCONNECTED, 0);
                }

                _glfw_free(disconnected);

                if (_glfw.monitorCount > 0)
                    return;
            }
        }

        var mode = x11_getVideoMode();
        var widthMM = _glfw.x11.XDisplayWidthMM != null
            ? _glfw.x11.XDisplayWidthMM(_glfw.x11.display, _glfw.x11.screen)
            : 0;
        var heightMM = _glfw.x11.XDisplayHeightMM != null
            ? _glfw.x11.XDisplayHeightMM(_glfw.x11.display, _glfw.x11.screen)
            : 0;

        if (widthMM <= 0)
            widthMM = (int)(mode.width * 25.4f / 96f);
        if (heightMM <= 0)
            heightMM = (int)(mode.height * 25.4f / 96f);

        var monitor = _glfwAllocMonitor("X11 Screen 0", widthMM, heightMM);
        if (monitor == null)
            return;

        monitor->currentMode = mode;
        monitor->x11.crtc = x11_getFirstRandRCrtc();
        _glfwInputMonitor(monitor, GLFW_CONNECTED, _GLFW_INSERT_FIRST);
    }

    static void _glfwSetVideoModeX11(_GLFWmonitor* monitor, GLFWvidmode* desired)
    {
        if (monitor == null ||
            desired == null ||
            monitor->x11.output == 0 ||
            monitor->x11.crtc == 0 ||
            x11_randrAvailable() == 0 ||
            _glfw.x11.XRRSetCrtcConfig == null)
        {
            return;
        }

        var best = _glfwChooseVideoMode(monitor, desired);
        if (best == null)
            return;

        GLFWvidmode current;
        if (_glfwGetVideoModeX11(monitor, &current) != 0 &&
            _glfwCompareVideoModes(&current, best) == 0)
        {
            return;
        }

        var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
        if (resources == null)
            return;

        var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);
        var outputInfo = _glfw.x11.XRRGetOutputInfo(_glfw.x11.display, resources, monitor->x11.output);

        if (crtcInfo != null && outputInfo != null)
        {
            nuint native = 0;
            for (var i = 0; i < outputInfo->nmode; i++)
            {
                var modeInfo = x11_getModeInfo(resources, outputInfo->modes[i]);
                if (x11_modeIsGood(modeInfo) == 0)
                    continue;

                var mode = x11_vidmodeFromModeInfo(modeInfo, crtcInfo);
                if (_glfwCompareVideoModes(best, &mode) == 0)
                {
                    native = modeInfo->id;
                    break;
                }
            }

            if (native != 0)
            {
                if (monitor->x11.oldMode == 0)
                    monitor->x11.oldMode = crtcInfo->mode;

                _glfw.x11.XRRSetCrtcConfig(_glfw.x11.display,
                    resources,
                    monitor->x11.crtc,
                    CurrentTime,
                    crtcInfo->x,
                    crtcInfo->y,
                    native,
                    crtcInfo->rotation,
                    crtcInfo->outputs,
                    crtcInfo->noutput);
            }
        }

        if (outputInfo != null)
            _glfw.x11.XRRFreeOutputInfo(outputInfo);
        if (crtcInfo != null)
            _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
        _glfw.x11.XRRFreeScreenResources(resources);
    }

    static void _glfwRestoreVideoModeX11(_GLFWmonitor* monitor)
    {
        if (monitor == null ||
            monitor->x11.oldMode == 0 ||
            monitor->x11.crtc == 0 ||
            x11_randrAvailable() == 0 ||
            _glfw.x11.XRRSetCrtcConfig == null)
        {
            return;
        }

        var resources = _glfw.x11.XRRGetScreenResourcesCurrent(_glfw.x11.display, _glfw.x11.root);
        if (resources == null)
            return;

        var crtcInfo = _glfw.x11.XRRGetCrtcInfo(_glfw.x11.display, resources, monitor->x11.crtc);
        if (crtcInfo != null)
        {
            _glfw.x11.XRRSetCrtcConfig(_glfw.x11.display,
                resources,
                monitor->x11.crtc,
                CurrentTime,
                crtcInfo->x,
                crtcInfo->y,
                monitor->x11.oldMode,
                crtcInfo->rotation,
                crtcInfo->outputs,
                crtcInfo->noutput);

            _glfw.x11.XRRFreeCrtcInfo(crtcInfo);
            monitor->x11.oldMode = 0;
        }

        _glfw.x11.XRRFreeScreenResources(resources);
    }

    public static nuint glfwGetX11Adapter(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return 0;
        }

        return ((_GLFWmonitor*)monitor)->x11.crtc;
    }

    public static nuint glfwGetX11Monitor(GLFWmonitor* monitor)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "X11: Platform not initialized");
            return 0;
        }

        return ((_GLFWmonitor*)monitor)->x11.output;
    }
}
