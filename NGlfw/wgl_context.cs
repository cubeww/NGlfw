using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int WGL_NUMBER_PIXEL_FORMATS_ARB = 0x2000;
    const int WGL_SUPPORT_OPENGL_ARB = 0x2010;
    const int WGL_DRAW_TO_WINDOW_ARB = 0x2001;
    const int WGL_PIXEL_TYPE_ARB = 0x2013;
    const int WGL_TYPE_RGBA_ARB = 0x202b;
    const int WGL_ACCELERATION_ARB = 0x2003;
    const int WGL_NO_ACCELERATION_ARB = 0x2025;
    const int WGL_RED_BITS_ARB = 0x2015;
    const int WGL_RED_SHIFT_ARB = 0x2016;
    const int WGL_GREEN_BITS_ARB = 0x2017;
    const int WGL_GREEN_SHIFT_ARB = 0x2018;
    const int WGL_BLUE_BITS_ARB = 0x2019;
    const int WGL_BLUE_SHIFT_ARB = 0x201a;
    const int WGL_ALPHA_BITS_ARB = 0x201b;
    const int WGL_ALPHA_SHIFT_ARB = 0x201c;
    const int WGL_ACCUM_BITS_ARB = 0x201d;
    const int WGL_ACCUM_RED_BITS_ARB = 0x201e;
    const int WGL_ACCUM_GREEN_BITS_ARB = 0x201f;
    const int WGL_ACCUM_BLUE_BITS_ARB = 0x2020;
    const int WGL_ACCUM_ALPHA_BITS_ARB = 0x2021;
    const int WGL_DEPTH_BITS_ARB = 0x2022;
    const int WGL_STENCIL_BITS_ARB = 0x2023;
    const int WGL_AUX_BUFFERS_ARB = 0x2024;
    const int WGL_STEREO_ARB = 0x2012;
    const int WGL_DOUBLE_BUFFER_ARB = 0x2011;
    const int WGL_SAMPLES_ARB = 0x2042;
    const int WGL_FRAMEBUFFER_SRGB_CAPABLE_ARB = 0x20a9;
    const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;
    const int WGL_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001;
    const int WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 0x00000002;
    const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
    const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
    const int WGL_CONTEXT_FLAGS_ARB = 0x2094;
    const int WGL_CONTEXT_ES2_PROFILE_BIT_EXT = 0x00000004;
    const int WGL_CONTEXT_ROBUST_ACCESS_BIT_ARB = 0x00000004;
    const int WGL_LOSE_CONTEXT_ON_RESET_ARB = 0x8252;
    const int WGL_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB = 0x8256;
    const int WGL_NO_RESET_NOTIFICATION_ARB = 0x8261;
    const int WGL_CONTEXT_RELEASE_BEHAVIOR_ARB = 0x2097;
    const int WGL_CONTEXT_RELEASE_BEHAVIOR_NONE_ARB = 0;
    const int WGL_CONTEXT_RELEASE_BEHAVIOR_FLUSH_ARB = 0x2098;
    const int WGL_CONTEXT_OPENGL_NO_ERROR_ARB = 0x31b3;
    const int WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x00000002;
    const int WGL_CONTEXT_DEBUG_BIT_ARB = 0x00000001;
    const int WGL_COLORSPACE_EXT = 0x309d;
    const int WGL_COLORSPACE_SRGB_EXT = 0x3089;

    const uint WGL_ERROR_INVALID_VERSION_ARB = 0xc0070000 | 0x2095;
    const uint WGL_ERROR_INVALID_PROFILE_ARB = 0xc0070000 | 0x2096;
    const uint WGL_ERROR_INCOMPATIBLE_DEVICE_CONTEXTS_ARB = 0xc0070000 | 0x2054;

    [DllImport("dwmapi.dll")]
    static extern int DwmIsCompositionEnabled(out int pfEnabled);

    [DllImport("dwmapi.dll")]
    static extern int DwmFlush();

    static int findPixelFormatAttribValueWGL(int* attribs,
                                             int attribCount,
                                             int* values,
                                             int attrib)
    {
        for (var i = 0; i < attribCount; i++)
        {
            if (attribs[i] == attrib)
                return values[i];
        }

        _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Unknown pixel format attribute requested");
        return 0;
    }

    static int choosePixelFormatWGL(_GLFWwindow* window,
                                    _GLFWctxconfig* ctxconfig,
                                    _GLFWfbconfig* fbconfig)
    {
        var nativeCount = DescribePixelFormat(window->context.wgl.dc,
            1,
            (uint)sizeof(PIXELFORMATDESCRIPTOR),
            null);

        if (nativeCount == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to enumerate pixel formats");
            return 0;
        }

        var attribs = stackalloc int[40];
        var values = stackalloc int[40];
        var attribCount = 0;

        void AddAttrib(int attrib)
        {
            attribs[attribCount++] = attrib;
        }

        int FindAttribValue(int attrib)
        {
            return findPixelFormatAttribValueWGL(attribs, attribCount, values, attrib);
        }

        if (_glfw.wgl.ARB_pixel_format != 0 && _glfw.wgl.GetPixelFormatAttribivARB != null)
        {
            AddAttrib(WGL_SUPPORT_OPENGL_ARB);
            AddAttrib(WGL_DRAW_TO_WINDOW_ARB);
            AddAttrib(WGL_PIXEL_TYPE_ARB);
            AddAttrib(WGL_ACCELERATION_ARB);
            AddAttrib(WGL_RED_BITS_ARB);
            AddAttrib(WGL_RED_SHIFT_ARB);
            AddAttrib(WGL_GREEN_BITS_ARB);
            AddAttrib(WGL_GREEN_SHIFT_ARB);
            AddAttrib(WGL_BLUE_BITS_ARB);
            AddAttrib(WGL_BLUE_SHIFT_ARB);
            AddAttrib(WGL_ALPHA_BITS_ARB);
            AddAttrib(WGL_ALPHA_SHIFT_ARB);
            AddAttrib(WGL_DEPTH_BITS_ARB);
            AddAttrib(WGL_STENCIL_BITS_ARB);
            AddAttrib(WGL_ACCUM_BITS_ARB);
            AddAttrib(WGL_ACCUM_RED_BITS_ARB);
            AddAttrib(WGL_ACCUM_GREEN_BITS_ARB);
            AddAttrib(WGL_ACCUM_BLUE_BITS_ARB);
            AddAttrib(WGL_ACCUM_ALPHA_BITS_ARB);
            AddAttrib(WGL_AUX_BUFFERS_ARB);
            AddAttrib(WGL_STEREO_ARB);
            AddAttrib(WGL_DOUBLE_BUFFER_ARB);

            if (_glfw.wgl.ARB_multisample != 0)
                AddAttrib(WGL_SAMPLES_ARB);

            if (ctxconfig->client == GLFW_OPENGL_API)
            {
                if (_glfw.wgl.ARB_framebuffer_sRGB != 0 ||
                    _glfw.wgl.EXT_framebuffer_sRGB != 0)
                    AddAttrib(WGL_FRAMEBUFFER_SRGB_CAPABLE_ARB);
            }
            else
            {
                if (_glfw.wgl.EXT_colorspace != 0)
                    AddAttrib(WGL_COLORSPACE_EXT);
            }

            var attrib = WGL_NUMBER_PIXEL_FORMATS_ARB;
            var extensionCount = 0;

            if (_glfw.wgl.GetPixelFormatAttribivARB(window->context.wgl.dc,
                    1,
                    0,
                    1,
                    &attrib,
                    &extensionCount) == 0)
            {
                _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                    "WGL: Failed to retrieve pixel format attribute");
                return 0;
            }

            nativeCount = _glfw_min(nativeCount, extensionCount);
        }

        var usableConfigs = (_GLFWfbconfig*)_glfw_calloc((nuint)nativeCount, (nuint)sizeof(_GLFWfbconfig));
        if (usableConfigs == null)
            return 0;

        var usableCount = 0;

        for (var i = 0; i < nativeCount; i++)
        {
            var pixelFormat = i + 1;
            var u = usableConfigs + usableCount;

            if (_glfw.wgl.ARB_pixel_format != 0 && _glfw.wgl.GetPixelFormatAttribivARB != null)
            {
                if (_glfw.wgl.GetPixelFormatAttribivARB(window->context.wgl.dc,
                        pixelFormat,
                        0,
                        (uint)attribCount,
                        attribs,
                        values) == 0)
                {
                    _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                        "WGL: Failed to retrieve pixel format attributes");

                    _glfw_free(usableConfigs);
                    return 0;
                }

                if (FindAttribValue(WGL_SUPPORT_OPENGL_ARB) == 0 ||
                    FindAttribValue(WGL_DRAW_TO_WINDOW_ARB) == 0)
                    continue;

                if (FindAttribValue(WGL_PIXEL_TYPE_ARB) != WGL_TYPE_RGBA_ARB)
                    continue;

                if (FindAttribValue(WGL_ACCELERATION_ARB) == WGL_NO_ACCELERATION_ARB)
                    continue;

                if (FindAttribValue(WGL_DOUBLE_BUFFER_ARB) != fbconfig->doublebuffer)
                    continue;

                u->redBits = FindAttribValue(WGL_RED_BITS_ARB);
                u->greenBits = FindAttribValue(WGL_GREEN_BITS_ARB);
                u->blueBits = FindAttribValue(WGL_BLUE_BITS_ARB);
                u->alphaBits = FindAttribValue(WGL_ALPHA_BITS_ARB);
                u->depthBits = FindAttribValue(WGL_DEPTH_BITS_ARB);
                u->stencilBits = FindAttribValue(WGL_STENCIL_BITS_ARB);
                u->accumRedBits = FindAttribValue(WGL_ACCUM_RED_BITS_ARB);
                u->accumGreenBits = FindAttribValue(WGL_ACCUM_GREEN_BITS_ARB);
                u->accumBlueBits = FindAttribValue(WGL_ACCUM_BLUE_BITS_ARB);
                u->accumAlphaBits = FindAttribValue(WGL_ACCUM_ALPHA_BITS_ARB);
                u->auxBuffers = FindAttribValue(WGL_AUX_BUFFERS_ARB);

                if (FindAttribValue(WGL_STEREO_ARB) != 0)
                    u->stereo = GLFW_TRUE;

                if (_glfw.wgl.ARB_multisample != 0)
                    u->samples = FindAttribValue(WGL_SAMPLES_ARB);

                if (ctxconfig->client == GLFW_OPENGL_API)
                {
                    if (_glfw.wgl.ARB_framebuffer_sRGB != 0 ||
                        _glfw.wgl.EXT_framebuffer_sRGB != 0)
                    {
                        if (FindAttribValue(WGL_FRAMEBUFFER_SRGB_CAPABLE_ARB) != 0)
                            u->sRGB = GLFW_TRUE;
                    }
                }
                else
                {
                    if (_glfw.wgl.EXT_colorspace != 0)
                    {
                        if (FindAttribValue(WGL_COLORSPACE_EXT) == WGL_COLORSPACE_SRGB_EXT)
                            u->sRGB = GLFW_TRUE;
                    }
                }
            }
            else
            {
                PIXELFORMATDESCRIPTOR pfd;

                if (DescribePixelFormat(window->context.wgl.dc,
                        pixelFormat,
                        (uint)sizeof(PIXELFORMATDESCRIPTOR),
                        &pfd) == 0)
                {
                    _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to describe pixel format");

                    _glfw_free(usableConfigs);
                    return 0;
                }

                if ((pfd.dwFlags & PFD_DRAW_TO_WINDOW) == 0 ||
                    (pfd.dwFlags & PFD_SUPPORT_OPENGL) == 0)
                    continue;

                if ((pfd.dwFlags & PFD_GENERIC_ACCELERATED) == 0 &&
                    (pfd.dwFlags & PFD_GENERIC_FORMAT) != 0)
                    continue;

                if (pfd.iPixelType != PFD_TYPE_RGBA)
                    continue;

                if (((pfd.dwFlags & PFD_DOUBLEBUFFER) != 0 ? GLFW_TRUE : GLFW_FALSE) != fbconfig->doublebuffer)
                    continue;

                u->redBits = pfd.cRedBits;
                u->greenBits = pfd.cGreenBits;
                u->blueBits = pfd.cBlueBits;
                u->alphaBits = pfd.cAlphaBits;
                u->depthBits = pfd.cDepthBits;
                u->stencilBits = pfd.cStencilBits;
                u->accumRedBits = pfd.cAccumRedBits;
                u->accumGreenBits = pfd.cAccumGreenBits;
                u->accumBlueBits = pfd.cAccumBlueBits;
                u->accumAlphaBits = pfd.cAccumAlphaBits;
                u->auxBuffers = pfd.cAuxBuffers;

                if ((pfd.dwFlags & PFD_STEREO) != 0)
                    u->stereo = GLFW_TRUE;
            }

            u->handle = (nuint)pixelFormat;

            usableCount++;
        }

        if (usableCount == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "WGL: The driver does not appear to support OpenGL");

            _glfw_free(usableConfigs);
            return 0;
        }

        var closest = _glfwChooseFBConfig(fbconfig, usableConfigs, (uint)usableCount);
        if (closest == null)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE,
                "WGL: Failed to find a suitable pixel format");

            _glfw_free(usableConfigs);
            return 0;
        }

        var result = (int)closest->handle;
        _glfw_free(usableConfigs);
        return result;
    }

    static void makeContextCurrentWGL(_GLFWwindow* window)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (window != null)
            {
                if (_glfw.wgl.MakeCurrent(window->context.wgl.dc, window->context.wgl.handle) != 0)
                    _glfwPlatformSetTls(&glfw->contextSlot, window);
                else
                {
                    _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to make context current");
                    _glfwPlatformSetTls(&glfw->contextSlot, null);
                }
            }
            else
            {
                if (_glfw.wgl.MakeCurrent(0, 0) == 0)
                    _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to clear current context");

                _glfwPlatformSetTls(&glfw->contextSlot, null);
            }
        }
    }

    static void swapBuffersWGL(_GLFWwindow* window)
    {
        if (window->monitor == null && wgl_dwmCompositionEnabled() != 0)
        {
            var count = Math.Abs(window->context.wgl.interval);
            while (count-- > 0)
                DwmFlush();
        }

        SwapBuffers(window->context.wgl.dc);
    }

    static void swapIntervalWGL(int interval)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
            if (window != null)
                window->context.wgl.interval = interval;

            if (window != null && window->monitor == null && wgl_dwmCompositionEnabled() != 0)
                interval = 0;
        }

        if (_glfw.wgl.EXT_swap_control != 0 && _glfw.wgl.SwapIntervalEXT != null)
            _glfw.wgl.SwapIntervalEXT(interval);
    }

    static int wgl_dwmCompositionEnabled()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(6, 0) ||
            OperatingSystem.IsWindowsVersionAtLeast(6, 2))
            return GLFW_FALSE;

        return DwmIsCompositionEnabled(out var enabled) >= 0 && enabled != 0
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    static int wgl_stringInExtensionString(byte* extension, byte* extensions)
    {
        return extension != null && extensions != null
            ? _glfwStringInExtensionString(extension, extensions)
            : GLFW_FALSE;
    }

    static int extensionSupportedWGL(byte* extension)
    {
        byte* extensions = null;

        if (_glfw.wgl.GetExtensionsStringARB != null)
            extensions = _glfw.wgl.GetExtensionsStringARB(_glfw.wgl.GetCurrentDC());
        else if (_glfw.wgl.GetExtensionsStringEXT != null)
            extensions = _glfw.wgl.GetExtensionsStringEXT();

        if (extensions == null)
            return GLFW_FALSE;

        return wgl_stringInExtensionString(extension, extensions);
    }

    static void* getProcAddressWGL(byte* procname)
    {
        var proc = _glfw.wgl.GetProcAddress != null ? _glfw.wgl.GetProcAddress(procname) : null;
        if (proc != null)
            return proc;

        if (_glfw.wgl.instance == null)
            return null;

        var name = Marshal.PtrToStringUTF8((nint)procname);
        if (name == null)
            return null;

        return NativeLibrary.TryGetExport((nint)_glfw.wgl.instance, name, out var symbol)
            ? (void*)symbol
            : null;
    }

    static void* wgl_getProcAddress(string procname)
    {
        var name = stackalloc byte[procname.Length + 1];
        for (var i = 0; i < procname.Length; i++)
            name[i] = (byte)procname[i];
        name[procname.Length] = 0;

        return getProcAddressWGL(name);
    }

    static int wgl_extensionSupported(string extension)
    {
        var name = stackalloc byte[extension.Length + 1];
        for (var i = 0; i < extension.Length; i++)
            name[i] = (byte)extension[i];
        name[extension.Length] = 0;

        return extensionSupportedWGL(name);
    }

    static void destroyContextWGL(_GLFWwindow* window)
    {
        if (window->context.wgl.handle != 0)
        {
            _glfw.wgl.DeleteContext(window->context.wgl.handle);
            window->context.wgl.handle = 0;
        }
    }

    static int _glfwInitWGL()
    {
        if (_glfw.wgl.instance != null)
            return GLFW_TRUE;

        if (!NativeLibrary.TryLoad("opengl32.dll", out var instance))
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to load opengl32.dll");
            return GLFW_FALSE;
        }

        _glfw.wgl.instance = (void*)instance;

        if (!NativeLibrary.TryGetExport(instance, "wglCreateContext", out var createContext) ||
            !NativeLibrary.TryGetExport(instance, "wglDeleteContext", out var deleteContext) ||
            !NativeLibrary.TryGetExport(instance, "wglGetProcAddress", out var getProcAddress) ||
            !NativeLibrary.TryGetExport(instance, "wglGetCurrentDC", out var getCurrentDC) ||
            !NativeLibrary.TryGetExport(instance, "wglGetCurrentContext", out var getCurrentContext) ||
            !NativeLibrary.TryGetExport(instance, "wglMakeCurrent", out var makeCurrent) ||
            !NativeLibrary.TryGetExport(instance, "wglShareLists", out var shareLists))
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to load opengl32 functions");
            _glfwTerminateWGL();
            return GLFW_FALSE;
        }

        _glfw.wgl.CreateContext = (delegate* unmanaged<nint, nint>)createContext;
        _glfw.wgl.DeleteContext = (delegate* unmanaged<nint, int>)deleteContext;
        _glfw.wgl.GetProcAddress = (delegate* unmanaged<byte*, void*>)getProcAddress;
        _glfw.wgl.GetCurrentDC = (delegate* unmanaged<nint>)getCurrentDC;
        _glfw.wgl.GetCurrentContext = (delegate* unmanaged<nint>)getCurrentContext;
        _glfw.wgl.MakeCurrent = (delegate* unmanaged<nint, nint, int>)makeCurrent;
        _glfw.wgl.ShareLists = (delegate* unmanaged<nint, nint, int>)shareLists;

        var dc = GetDC(_glfw.win32.helperWindowHandle);

        var pfd = new PIXELFORMATDESCRIPTOR
        {
            nSize = (ushort)sizeof(PIXELFORMATDESCRIPTOR),
            nVersion = 1,
            dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
            iPixelType = PFD_TYPE_RGBA,
            cColorBits = 24,
            iLayerType = PFD_MAIN_PLANE
        };

        var pixelFormat = ChoosePixelFormat(dc, &pfd);
        if (pixelFormat == 0 || SetPixelFormat(dc, pixelFormat, &pfd) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                "WGL: Failed to set pixel format for dummy context");
            _glfwTerminateWGL();
            return GLFW_FALSE;
        }

        var rc = _glfw.wgl.CreateContext(dc);
        if (rc == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to create dummy context");
            _glfwTerminateWGL();
            return GLFW_FALSE;
        }

        var pdc = _glfw.wgl.GetCurrentDC();
        var prc = _glfw.wgl.GetCurrentContext();

        if (_glfw.wgl.MakeCurrent(dc, rc) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to make dummy context current");
            _glfw.wgl.MakeCurrent(pdc, prc);
            _glfw.wgl.DeleteContext(rc);
            _glfwTerminateWGL();
            return GLFW_FALSE;
        }

        _glfw.wgl.GetExtensionsStringEXT =
            (delegate* unmanaged<byte*>)wgl_getProcAddress("wglGetExtensionsStringEXT");
        _glfw.wgl.GetExtensionsStringARB =
            (delegate* unmanaged<nint, byte*>)wgl_getProcAddress("wglGetExtensionsStringARB");
        _glfw.wgl.CreateContextAttribsARB =
            (delegate* unmanaged<nint, nint, int*, nint>)wgl_getProcAddress("wglCreateContextAttribsARB");
        _glfw.wgl.SwapIntervalEXT =
            (delegate* unmanaged<int, int>)wgl_getProcAddress("wglSwapIntervalEXT");
        _glfw.wgl.GetPixelFormatAttribivARB =
            (delegate* unmanaged<nint, int, int, uint, int*, int*, int>)wgl_getProcAddress("wglGetPixelFormatAttribivARB");

        _glfw.wgl.ARB_multisample = wgl_extensionSupported("WGL_ARB_multisample");
        _glfw.wgl.ARB_framebuffer_sRGB = wgl_extensionSupported("WGL_ARB_framebuffer_sRGB");
        _glfw.wgl.EXT_framebuffer_sRGB = wgl_extensionSupported("WGL_EXT_framebuffer_sRGB");
        _glfw.wgl.ARB_create_context = wgl_extensionSupported("WGL_ARB_create_context");
        _glfw.wgl.ARB_create_context_profile = wgl_extensionSupported("WGL_ARB_create_context_profile");
        _glfw.wgl.EXT_create_context_es2_profile = wgl_extensionSupported("WGL_EXT_create_context_es2_profile");
        _glfw.wgl.ARB_create_context_robustness = wgl_extensionSupported("WGL_ARB_create_context_robustness");
        _glfw.wgl.ARB_create_context_no_error = wgl_extensionSupported("WGL_ARB_create_context_no_error");
        _glfw.wgl.EXT_swap_control = wgl_extensionSupported("WGL_EXT_swap_control");
        _glfw.wgl.EXT_colorspace = wgl_extensionSupported("WGL_EXT_colorspace");
        _glfw.wgl.ARB_pixel_format = wgl_extensionSupported("WGL_ARB_pixel_format");
        _glfw.wgl.ARB_context_flush_control = wgl_extensionSupported("WGL_ARB_context_flush_control");

        _glfw.wgl.MakeCurrent(pdc, prc);
        _glfw.wgl.DeleteContext(rc);
        return GLFW_TRUE;
    }

    static void _glfwTerminateWGL()
    {
        if (_glfw.wgl.instance != null)
            NativeLibrary.Free((nint)_glfw.wgl.instance);

        _glfw.wgl = default;
    }

    static int _glfwCreateContextWGL(_GLFWwindow* window,
                                     _GLFWctxconfig* ctxconfig,
                                     _GLFWfbconfig* fbconfig)
    {
        var share = ctxconfig->share != null ? ctxconfig->share->context.wgl.handle : 0;

        window->context.wgl.dc = GetDC(window->win32.handle);
        if (window->context.wgl.dc == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "WGL: Failed to retrieve DC for window");
            return GLFW_FALSE;
        }

        var pixelFormat = choosePixelFormatWGL(window, ctxconfig, fbconfig);
        if (pixelFormat == 0)
            return GLFW_FALSE;

        PIXELFORMATDESCRIPTOR pfd;
        if (DescribePixelFormat(window->context.wgl.dc,
                pixelFormat,
                (uint)sizeof(PIXELFORMATDESCRIPTOR),
                &pfd) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                "WGL: Failed to retrieve PFD for selected pixel format");
            return GLFW_FALSE;
        }

        if (SetPixelFormat(window->context.wgl.dc, pixelFormat, &pfd) == 0)
        {
            _glfwInputErrorWin32(GLFW_PLATFORM_ERROR, "WGL: Failed to set selected pixel format");
            return GLFW_FALSE;
        }

        if (ctxconfig->client == GLFW_OPENGL_API)
        {
            if (ctxconfig->forward != 0 && _glfw.wgl.ARB_create_context == 0)
            {
                _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                    "WGL: A forward compatible OpenGL context requested but WGL_ARB_create_context is unavailable");
                return GLFW_FALSE;
            }

            if (ctxconfig->profile != 0 && _glfw.wgl.ARB_create_context_profile == 0)
            {
                _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                    "WGL: OpenGL profile requested but WGL_ARB_create_context_profile is unavailable");
                return GLFW_FALSE;
            }
        }
        else
        {
            if (_glfw.wgl.ARB_create_context == 0 ||
                _glfw.wgl.ARB_create_context_profile == 0 ||
                _glfw.wgl.EXT_create_context_es2_profile == 0)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "WGL: OpenGL ES requested but WGL_ARB_create_context_es2_profile is unavailable");
                return GLFW_FALSE;
            }
        }

        if (_glfw.wgl.ARB_create_context != 0 && _glfw.wgl.CreateContextAttribsARB != null)
        {
            var attribs = stackalloc int[40];
            var index = 0;
            var mask = 0;
            var flags = 0;

            void SetAttrib(int attrib, int value)
            {
                attribs[index++] = attrib;
                attribs[index++] = value;
            }

            if (ctxconfig->client == GLFW_OPENGL_API)
            {
                if (ctxconfig->forward != 0)
                    flags |= WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB;

                if (ctxconfig->profile == GLFW_OPENGL_CORE_PROFILE)
                    mask |= WGL_CONTEXT_CORE_PROFILE_BIT_ARB;
                else if (ctxconfig->profile == GLFW_OPENGL_COMPAT_PROFILE)
                    mask |= WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB;
            }
            else
                mask |= WGL_CONTEXT_ES2_PROFILE_BIT_EXT;

            if (ctxconfig->debug != 0)
                flags |= WGL_CONTEXT_DEBUG_BIT_ARB;

            if (ctxconfig->robustness != 0 && _glfw.wgl.ARB_create_context_robustness != 0)
            {
                if (ctxconfig->robustness == GLFW_NO_RESET_NOTIFICATION)
                    SetAttrib(WGL_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB, WGL_NO_RESET_NOTIFICATION_ARB);
                else if (ctxconfig->robustness == GLFW_LOSE_CONTEXT_ON_RESET)
                    SetAttrib(WGL_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB, WGL_LOSE_CONTEXT_ON_RESET_ARB);

                flags |= WGL_CONTEXT_ROBUST_ACCESS_BIT_ARB;
            }

            if (ctxconfig->release != 0 && _glfw.wgl.ARB_context_flush_control != 0)
            {
                if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_NONE)
                    SetAttrib(WGL_CONTEXT_RELEASE_BEHAVIOR_ARB, WGL_CONTEXT_RELEASE_BEHAVIOR_NONE_ARB);
                else if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_FLUSH)
                    SetAttrib(WGL_CONTEXT_RELEASE_BEHAVIOR_ARB, WGL_CONTEXT_RELEASE_BEHAVIOR_FLUSH_ARB);
            }

            if (ctxconfig->noerror != 0 && _glfw.wgl.ARB_create_context_no_error != 0)
                SetAttrib(WGL_CONTEXT_OPENGL_NO_ERROR_ARB, GLFW_TRUE);

            if (ctxconfig->major != 1 || ctxconfig->minor != 0)
            {
                SetAttrib(WGL_CONTEXT_MAJOR_VERSION_ARB, ctxconfig->major);
                SetAttrib(WGL_CONTEXT_MINOR_VERSION_ARB, ctxconfig->minor);
            }

            if (flags != 0)
                SetAttrib(WGL_CONTEXT_FLAGS_ARB, flags);

            if (mask != 0)
                SetAttrib(WGL_CONTEXT_PROFILE_MASK_ARB, mask);

            SetAttrib(0, 0);

            window->context.wgl.handle =
                _glfw.wgl.CreateContextAttribsARB(window->context.wgl.dc, share, attribs);
            if (window->context.wgl.handle == 0)
            {
                var error = GetLastError();

                if (error == WGL_ERROR_INVALID_VERSION_ARB)
                {
                    _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                        ctxconfig->client == GLFW_OPENGL_API
                            ? "WGL: Driver does not support OpenGL version {0}.{1}"
                            : "WGL: Driver does not support OpenGL ES version {0}.{1}",
                        ctxconfig->major,
                        ctxconfig->minor);
                }
                else if (error == WGL_ERROR_INVALID_PROFILE_ARB)
                    _glfwInputError(GLFW_VERSION_UNAVAILABLE, "WGL: Driver does not support the requested OpenGL profile");
                else if (error == WGL_ERROR_INCOMPATIBLE_DEVICE_CONTEXTS_ARB)
                {
                    _glfwInputError(GLFW_INVALID_VALUE,
                        "WGL: The share context is not compatible with the requested context");
                }
                else
                {
                    _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                        ctxconfig->client == GLFW_OPENGL_API
                            ? "WGL: Failed to create OpenGL context"
                            : "WGL: Failed to create OpenGL ES context");
                }

                return GLFW_FALSE;
            }
        }
        else
        {
            window->context.wgl.handle = _glfw.wgl.CreateContext(window->context.wgl.dc);
            if (window->context.wgl.handle == 0)
            {
                _glfwInputErrorWin32(GLFW_VERSION_UNAVAILABLE, "WGL: Failed to create OpenGL context");
                return GLFW_FALSE;
            }

            if (share != 0 && _glfw.wgl.ShareLists(share, window->context.wgl.handle) == 0)
            {
                _glfwInputErrorWin32(GLFW_PLATFORM_ERROR,
                    "WGL: Failed to enable sharing with specified OpenGL context");
                return GLFW_FALSE;
            }
        }

        window->context.makeCurrent = &makeContextCurrentWGL;
        window->context.swapBuffers = &swapBuffersWGL;
        window->context.swapInterval = &swapIntervalWGL;
        window->context.extensionSupported = &extensionSupportedWGL;
        window->context.getProcAddress = &getProcAddressWGL;
        window->context.destroy = &destroyContextWGL;

        return GLFW_TRUE;
    }

    public static void* glfwGetWGLContext(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WIN32)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "WGL: Platform not initialized");
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_NATIVE_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return (void*)internalWindow->context.wgl.handle;
    }
}
