using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int GLX_VENDOR = 1;
    const int GLX_RGBA_BIT = 0x00000001;
    const int GLX_WINDOW_BIT = 0x00000001;
    const int GLX_DRAWABLE_TYPE = 0x8010;
    const int GLX_RENDER_TYPE = 0x8011;
    const int GLX_RGBA_TYPE = 0x8014;
    const int GLX_DOUBLEBUFFER = 5;
    const int GLX_STEREO = 6;
    const int GLX_AUX_BUFFERS = 7;
    const int GLX_RED_SIZE = 8;
    const int GLX_GREEN_SIZE = 9;
    const int GLX_BLUE_SIZE = 10;
    const int GLX_ALPHA_SIZE = 11;
    const int GLX_DEPTH_SIZE = 12;
    const int GLX_STENCIL_SIZE = 13;
    const int GLX_ACCUM_RED_SIZE = 14;
    const int GLX_ACCUM_GREEN_SIZE = 15;
    const int GLX_ACCUM_BLUE_SIZE = 16;
    const int GLX_ACCUM_ALPHA_SIZE = 17;
    const int GLX_SAMPLES = 0x186a1;
    const int GLX_FRAMEBUFFER_SRGB_CAPABLE_ARB = 0x20b2;
    const int GLX_CONTEXT_DEBUG_BIT_ARB = 0x00000001;
    const int GLX_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 0x00000002;
    const int GLX_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001;
    const int GLX_CONTEXT_PROFILE_MASK_ARB = 0x9126;
    const int GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x00000002;
    const int GLX_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
    const int GLX_CONTEXT_MINOR_VERSION_ARB = 0x2092;
    const int GLX_CONTEXT_FLAGS_ARB = 0x2094;
    const int GLX_CONTEXT_ES2_PROFILE_BIT_EXT = 0x00000004;
    const int GLX_CONTEXT_ROBUST_ACCESS_BIT_ARB = 0x00000004;
    const int GLX_LOSE_CONTEXT_ON_RESET_ARB = 0x8252;
    const int GLX_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB = 0x8256;
    const int GLX_NO_RESET_NOTIFICATION_ARB = 0x8261;
    const int GLX_CONTEXT_RELEASE_BEHAVIOR_ARB = 0x2097;
    const int GLX_CONTEXT_RELEASE_BEHAVIOR_NONE_ARB = 0;
    const int GLX_CONTEXT_RELEASE_BEHAVIOR_FLUSH_ARB = 0x2098;
    const int GLX_CONTEXT_OPENGL_NO_ERROR_ARB = 0x31b3;
    const int GLXBadProfileARB = 13;

    static int getGLXFBConfigAttrib(void* fbconfig, int attrib)
    {
        int value;
        _glfw.glx.GetFBConfigAttrib(_glfw.x11.display, fbconfig, attrib, &value);
        return value;
    }

    static void x11_free(void* value)
    {
        if (value != null && _glfw.x11.XFree != null)
            _glfw.x11.XFree(value);
    }

    static int chooseGLXFBConfig(_GLFWfbconfig* desired, void** result)
    {
        int nativeCount;
        var nativeConfigs = _glfw.glx.GetFBConfigs(_glfw.x11.display, _glfw.x11.screen, &nativeCount);
        if (nativeConfigs == null || nativeCount == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "GLX: No GLXFBConfigs returned");
            return GLFW_FALSE;
        }

        var usableConfigs = (_GLFWfbconfig*)_glfw_calloc((nuint)nativeCount, (nuint)sizeof(_GLFWfbconfig));
        if (usableConfigs == null)
        {
            x11_free(nativeConfigs);
            return GLFW_FALSE;
        }

        var trustWindowBit = GLFW_TRUE;
        var vendor = _glfw.glx.GetClientString(_glfw.x11.display, GLX_VENDOR);
        if (vendor != null && Marshal.PtrToStringUTF8((nint)vendor) == "Chromium")
            trustWindowBit = GLFW_FALSE;

        var usableCount = 0;

        for (var i = 0; i < nativeCount; i++)
        {
            var native = nativeConfigs[i];
            var usable = usableConfigs + usableCount;

            if ((getGLXFBConfigAttrib(native, GLX_RENDER_TYPE) & GLX_RGBA_BIT) == 0)
                continue;

            if ((getGLXFBConfigAttrib(native, GLX_DRAWABLE_TYPE) & GLX_WINDOW_BIT) == 0)
            {
                if (trustWindowBit != 0)
                    continue;
            }

            if (getGLXFBConfigAttrib(native, GLX_DOUBLEBUFFER) != desired->doublebuffer)
                continue;

            if (desired->transparent != 0 && _glfw.glx.GetVisualFromFBConfig != null)
            {
                var visualInfo = _glfw.glx.GetVisualFromFBConfig(_glfw.x11.display, native);
                if (visualInfo != null)
                {
                    usable->transparent = _glfwIsVisualTransparentX11(visualInfo->visual);
                    x11_free(visualInfo);
                }
            }

            usable->redBits = getGLXFBConfigAttrib(native, GLX_RED_SIZE);
            usable->greenBits = getGLXFBConfigAttrib(native, GLX_GREEN_SIZE);
            usable->blueBits = getGLXFBConfigAttrib(native, GLX_BLUE_SIZE);
            usable->alphaBits = getGLXFBConfigAttrib(native, GLX_ALPHA_SIZE);
            usable->depthBits = getGLXFBConfigAttrib(native, GLX_DEPTH_SIZE);
            usable->stencilBits = getGLXFBConfigAttrib(native, GLX_STENCIL_SIZE);
            usable->accumRedBits = getGLXFBConfigAttrib(native, GLX_ACCUM_RED_SIZE);
            usable->accumGreenBits = getGLXFBConfigAttrib(native, GLX_ACCUM_GREEN_SIZE);
            usable->accumBlueBits = getGLXFBConfigAttrib(native, GLX_ACCUM_BLUE_SIZE);
            usable->accumAlphaBits = getGLXFBConfigAttrib(native, GLX_ACCUM_ALPHA_SIZE);
            usable->auxBuffers = getGLXFBConfigAttrib(native, GLX_AUX_BUFFERS);

            if (getGLXFBConfigAttrib(native, GLX_STEREO) != 0)
                usable->stereo = GLFW_TRUE;

            if (_glfw.glx.ARB_multisample != 0)
                usable->samples = getGLXFBConfigAttrib(native, GLX_SAMPLES);

            if (_glfw.glx.ARB_framebuffer_sRGB != 0 || _glfw.glx.EXT_framebuffer_sRGB != 0)
                usable->sRGB = getGLXFBConfigAttrib(native, GLX_FRAMEBUFFER_SRGB_CAPABLE_ARB);

            usable->handle = (nuint)native;
            usableCount++;
        }

        var closest = _glfwChooseFBConfig(desired, usableConfigs, (uint)usableCount);
        if (closest != null && result != null)
            *result = (void*)closest->handle;

        x11_free(nativeConfigs);
        _glfw_free(usableConfigs);
        return closest != null ? GLFW_TRUE : GLFW_FALSE;
    }

    static void* createLegacyContextGLX(_GLFWwindow* window, void* fbconfig, void* share)
    {
        return _glfw.glx.CreateNewContext(_glfw.x11.display, fbconfig, GLX_RGBA_TYPE, share, GLFW_TRUE);
    }

    static void makeContextCurrentGLX(_GLFWwindow* window)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (window != null)
            {
                if (_glfw.glx.MakeCurrent(_glfw.x11.display, window->context.glx.window, window->context.glx.handle) == 0)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "GLX: Failed to make context current");
                    return;
                }
            }
            else
            {
                if (_glfw.glx.MakeCurrent(_glfw.x11.display, 0, null) == 0)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "GLX: Failed to clear current context");
                    return;
                }
            }

            _glfwPlatformSetTls(&glfw->contextSlot, window);
        }
    }

    static void swapBuffersGLX(_GLFWwindow* window)
    {
        _glfw.glx.SwapBuffers(_glfw.x11.display, window->context.glx.window);
    }

    static void swapIntervalGLX(int interval)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
            if (window == null)
                return;

            if (_glfw.glx.EXT_swap_control != 0 && _glfw.glx.SwapIntervalEXT != null)
                _glfw.glx.SwapIntervalEXT(_glfw.x11.display, window->context.glx.window, interval);
            else if (_glfw.glx.MESA_swap_control != 0 && _glfw.glx.SwapIntervalMESA != null)
                _glfw.glx.SwapIntervalMESA(interval);
            else if (_glfw.glx.SGI_swap_control != 0 && _glfw.glx.SwapIntervalSGI != null)
            {
                if (interval > 0)
                    _glfw.glx.SwapIntervalSGI(interval);
            }
        }
    }

    static int extensionSupportedGLX(byte* extension)
    {
        var extensions = _glfw.glx.QueryExtensionsString(_glfw.x11.display, _glfw.x11.screen);
        if (extensions != null && _glfwStringInExtensionString(extension, extensions) != 0)
            return GLFW_TRUE;

        return GLFW_FALSE;
    }

    static void* getProcAddressGLX(byte* procname)
    {
        if (_glfw.glx.GetProcAddress != null)
            return _glfw.glx.GetProcAddress(procname);

        if (_glfw.glx.GetProcAddressARB != null)
            return _glfw.glx.GetProcAddressARB(procname);

        return _glfwPlatformGetModuleSymbol(_glfw.glx.handle, procname);
    }

    static void* glx_getProcAddress(string procname)
    {
        var name = stackalloc byte[procname.Length + 1];
        for (var i = 0; i < procname.Length; i++)
            name[i] = (byte)procname[i];
        name[procname.Length] = 0;

        return getProcAddressGLX(name);
    }

    static int glx_extensionSupported(string extension)
    {
        var name = stackalloc byte[extension.Length + 1];
        for (var i = 0; i < extension.Length; i++)
            name[i] = (byte)extension[i];
        name[extension.Length] = 0;

        return extensionSupportedGLX(name);
    }

    static void destroyContextGLX(_GLFWwindow* window)
    {
        if (window->context.glx.window != 0)
        {
            _glfw.glx.DestroyWindow(_glfw.x11.display, window->context.glx.window);
            window->context.glx.window = 0;
        }

        if (window->context.glx.handle != null)
        {
            _glfw.glx.DestroyContext(_glfw.x11.display, window->context.glx.handle);
            window->context.glx.handle = null;
        }
    }

    static int _glfwInitGLX()
    {
        if (_glfw.glx.handle != null)
            return GLFW_TRUE;

        string[] sonames =
        [
            "libGLX.so.0",
            "libGL.so.1",
            "libGL.so"
        ];

        foreach (var soname in sonames)
        {
            _glfw.glx.handle = x11_loadModule(soname);
            if (_glfw.glx.handle != null)
                break;
        }

        if (_glfw.glx.handle == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "GLX: Failed to load GLX");
            return GLFW_FALSE;
        }

        _glfw.glx.GetFBConfigs =
            (delegate* unmanaged<void*, int, int*, void**>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetFBConfigs");
        _glfw.glx.GetFBConfigAttrib =
            (delegate* unmanaged<void*, void*, int, int*, int>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetFBConfigAttrib");
        _glfw.glx.GetClientString =
            (delegate* unmanaged<void*, int, byte*>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetClientString");
        _glfw.glx.QueryExtension =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.glx.handle, "glXQueryExtension");
        _glfw.glx.QueryVersion =
            (delegate* unmanaged<void*, int*, int*, int>)x11_getModuleSymbol(_glfw.glx.handle, "glXQueryVersion");
        _glfw.glx.DestroyContext =
            (delegate* unmanaged<void*, void*, void>)x11_getModuleSymbol(_glfw.glx.handle, "glXDestroyContext");
        _glfw.glx.MakeCurrent =
            (delegate* unmanaged<void*, nuint, void*, int>)x11_getModuleSymbol(_glfw.glx.handle, "glXMakeCurrent");
        _glfw.glx.SwapBuffers =
            (delegate* unmanaged<void*, nuint, void>)x11_getModuleSymbol(_glfw.glx.handle, "glXSwapBuffers");
        _glfw.glx.QueryExtensionsString =
            (delegate* unmanaged<void*, int, byte*>)x11_getModuleSymbol(_glfw.glx.handle, "glXQueryExtensionsString");
        _glfw.glx.CreateNewContext =
            (delegate* unmanaged<void*, void*, int, void*, int, void*>)x11_getModuleSymbol(_glfw.glx.handle, "glXCreateNewContext");
        _glfw.glx.CreateWindow =
            (delegate* unmanaged<void*, void*, nuint, int*, nuint>)x11_getModuleSymbol(_glfw.glx.handle, "glXCreateWindow");
        _glfw.glx.DestroyWindow =
            (delegate* unmanaged<void*, nuint, void>)x11_getModuleSymbol(_glfw.glx.handle, "glXDestroyWindow");
        _glfw.glx.GetVisualFromFBConfig =
            (delegate* unmanaged<void*, void*, XVisualInfo*>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetVisualFromFBConfig");

        if (_glfw.glx.GetFBConfigs == null ||
            _glfw.glx.GetFBConfigAttrib == null ||
            _glfw.glx.GetClientString == null ||
            _glfw.glx.QueryExtension == null ||
            _glfw.glx.QueryVersion == null ||
            _glfw.glx.DestroyContext == null ||
            _glfw.glx.MakeCurrent == null ||
            _glfw.glx.SwapBuffers == null ||
            _glfw.glx.QueryExtensionsString == null ||
            _glfw.glx.CreateNewContext == null ||
            _glfw.glx.CreateWindow == null ||
            _glfw.glx.DestroyWindow == null ||
            _glfw.glx.GetVisualFromFBConfig == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "GLX: Failed to load required entry points");
            _glfwTerminateGLX();
            return GLFW_FALSE;
        }

        _glfw.glx.GetProcAddress =
            (delegate* unmanaged<byte*, void*>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetProcAddress");
        _glfw.glx.GetProcAddressARB =
            (delegate* unmanaged<byte*, void*>)x11_getModuleSymbol(_glfw.glx.handle, "glXGetProcAddressARB");

        int errorBase;
        int eventBase;
        if (_glfw.glx.QueryExtension(_glfw.x11.display, &errorBase, &eventBase) == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "GLX: GLX extension not found");
            _glfwTerminateGLX();
            return GLFW_FALSE;
        }

        _glfw.glx.errorBase = errorBase;
        _glfw.glx.eventBase = eventBase;

        int major;
        int minor;
        if (_glfw.glx.QueryVersion(_glfw.x11.display, &major, &minor) == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "GLX: Failed to query GLX version");
            _glfwTerminateGLX();
            return GLFW_FALSE;
        }

        _glfw.glx.major = major;
        _glfw.glx.minor = minor;

        if (_glfw.glx.major == 1 && _glfw.glx.minor < 3)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "GLX: GLX version 1.3 is required");
            _glfwTerminateGLX();
            return GLFW_FALSE;
        }

        if (glx_extensionSupported("GLX_EXT_swap_control") != 0)
        {
            _glfw.glx.SwapIntervalEXT =
                (delegate* unmanaged<void*, nuint, int, void>)glx_getProcAddress("glXSwapIntervalEXT");

            if (_glfw.glx.SwapIntervalEXT != null)
                _glfw.glx.EXT_swap_control = GLFW_TRUE;
        }

        if (glx_extensionSupported("GLX_SGI_swap_control") != 0)
        {
            _glfw.glx.SwapIntervalSGI =
                (delegate* unmanaged<int, int>)glx_getProcAddress("glXSwapIntervalSGI");

            if (_glfw.glx.SwapIntervalSGI != null)
                _glfw.glx.SGI_swap_control = GLFW_TRUE;
        }

        if (glx_extensionSupported("GLX_MESA_swap_control") != 0)
        {
            _glfw.glx.SwapIntervalMESA =
                (delegate* unmanaged<int, int>)glx_getProcAddress("glXSwapIntervalMESA");

            if (_glfw.glx.SwapIntervalMESA != null)
                _glfw.glx.MESA_swap_control = GLFW_TRUE;
        }

        if (glx_extensionSupported("GLX_ARB_multisample") != 0)
            _glfw.glx.ARB_multisample = GLFW_TRUE;
        if (glx_extensionSupported("GLX_ARB_framebuffer_sRGB") != 0)
            _glfw.glx.ARB_framebuffer_sRGB = GLFW_TRUE;
        if (glx_extensionSupported("GLX_EXT_framebuffer_sRGB") != 0)
            _glfw.glx.EXT_framebuffer_sRGB = GLFW_TRUE;

        if (glx_extensionSupported("GLX_ARB_create_context") != 0)
        {
            _glfw.glx.CreateContextAttribsARB =
                (delegate* unmanaged<void*, void*, void*, int, int*, void*>)glx_getProcAddress("glXCreateContextAttribsARB");

            if (_glfw.glx.CreateContextAttribsARB != null)
                _glfw.glx.ARB_create_context = GLFW_TRUE;
        }

        if (glx_extensionSupported("GLX_ARB_create_context_robustness") != 0)
            _glfw.glx.ARB_create_context_robustness = GLFW_TRUE;
        if (glx_extensionSupported("GLX_ARB_create_context_profile") != 0)
            _glfw.glx.ARB_create_context_profile = GLFW_TRUE;
        if (glx_extensionSupported("GLX_EXT_create_context_es2_profile") != 0)
            _glfw.glx.EXT_create_context_es2_profile = GLFW_TRUE;
        if (glx_extensionSupported("GLX_ARB_create_context_no_error") != 0)
            _glfw.glx.ARB_create_context_no_error = GLFW_TRUE;
        if (glx_extensionSupported("GLX_ARB_context_flush_control") != 0)
            _glfw.glx.ARB_context_flush_control = GLFW_TRUE;

        return GLFW_TRUE;
    }

    static void _glfwTerminateGLX()
    {
        if (_glfw.glx.handle != null)
            _glfwPlatformFreeModule(_glfw.glx.handle);

        _glfw.glx = default;
    }

    static int _glfwCreateContextGLX(_GLFWwindow* window,
                                     _GLFWctxconfig* ctxconfig,
                                     _GLFWfbconfig* fbconfig)
    {
        var attribs = stackalloc int[40];
        void* native = null;
        var share = ctxconfig->share != null ? ctxconfig->share->context.glx.handle : null;

        if (chooseGLXFBConfig(fbconfig, &native) == 0)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "GLX: Failed to find a suitable GLXFBConfig");
            return GLFW_FALSE;
        }

        if (ctxconfig->client == GLFW_OPENGL_ES_API)
        {
            if (_glfw.glx.ARB_create_context == 0 ||
                _glfw.glx.ARB_create_context_profile == 0 ||
                _glfw.glx.EXT_create_context_es2_profile == 0)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "GLX: OpenGL ES requested but GLX_EXT_create_context_es2_profile is unavailable");
                return GLFW_FALSE;
            }
        }

        if (ctxconfig->forward != 0 && _glfw.glx.ARB_create_context == 0)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                "GLX: Forward compatibility requested but GLX_ARB_create_context_profile is unavailable");
            return GLFW_FALSE;
        }

        if (ctxconfig->profile != 0 &&
            (_glfw.glx.ARB_create_context == 0 || _glfw.glx.ARB_create_context_profile == 0))
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                "GLX: An OpenGL profile requested but GLX_ARB_create_context_profile is unavailable");
            return GLFW_FALSE;
        }

        _glfwGrabErrorHandlerX11();

        if (_glfw.glx.ARB_create_context != 0 && _glfw.glx.CreateContextAttribsARB != null)
        {
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
                    flags |= GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB;

                if (ctxconfig->profile == GLFW_OPENGL_CORE_PROFILE)
                    mask |= GLX_CONTEXT_CORE_PROFILE_BIT_ARB;
                else if (ctxconfig->profile == GLFW_OPENGL_COMPAT_PROFILE)
                    mask |= GLX_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB;
            }
            else
                mask |= GLX_CONTEXT_ES2_PROFILE_BIT_EXT;

            if (ctxconfig->debug != 0)
                flags |= GLX_CONTEXT_DEBUG_BIT_ARB;

            if (ctxconfig->robustness != 0 && _glfw.glx.ARB_create_context_robustness != 0)
            {
                if (ctxconfig->robustness == GLFW_NO_RESET_NOTIFICATION)
                    SetAttrib(GLX_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB, GLX_NO_RESET_NOTIFICATION_ARB);
                else if (ctxconfig->robustness == GLFW_LOSE_CONTEXT_ON_RESET)
                    SetAttrib(GLX_CONTEXT_RESET_NOTIFICATION_STRATEGY_ARB, GLX_LOSE_CONTEXT_ON_RESET_ARB);

                flags |= GLX_CONTEXT_ROBUST_ACCESS_BIT_ARB;
            }

            if (ctxconfig->release != 0 && _glfw.glx.ARB_context_flush_control != 0)
            {
                if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_NONE)
                    SetAttrib(GLX_CONTEXT_RELEASE_BEHAVIOR_ARB, GLX_CONTEXT_RELEASE_BEHAVIOR_NONE_ARB);
                else if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_FLUSH)
                    SetAttrib(GLX_CONTEXT_RELEASE_BEHAVIOR_ARB, GLX_CONTEXT_RELEASE_BEHAVIOR_FLUSH_ARB);
            }

            if (ctxconfig->noerror != 0 && _glfw.glx.ARB_create_context_no_error != 0)
                SetAttrib(GLX_CONTEXT_OPENGL_NO_ERROR_ARB, GLFW_TRUE);

            if (ctxconfig->major != 1 || ctxconfig->minor != 0)
            {
                SetAttrib(GLX_CONTEXT_MAJOR_VERSION_ARB, ctxconfig->major);
                SetAttrib(GLX_CONTEXT_MINOR_VERSION_ARB, ctxconfig->minor);
            }

            if (mask != 0)
                SetAttrib(GLX_CONTEXT_PROFILE_MASK_ARB, mask);
            if (flags != 0)
                SetAttrib(GLX_CONTEXT_FLAGS_ARB, flags);

            SetAttrib(0, 0);

            window->context.glx.handle =
                _glfw.glx.CreateContextAttribsARB(_glfw.x11.display, native, share, GLFW_TRUE, attribs);

            if (window->context.glx.handle == null)
            {
                if (_glfw.x11.errorCode == _glfw.glx.errorBase + GLXBadProfileARB &&
                    ctxconfig->client == GLFW_OPENGL_API &&
                    ctxconfig->profile == GLFW_OPENGL_ANY_PROFILE &&
                    ctxconfig->forward == GLFW_FALSE)
                {
                    window->context.glx.handle = createLegacyContextGLX(window, native, share);
                }
            }
        }
        else
            window->context.glx.handle = createLegacyContextGLX(window, native, share);

        _glfwReleaseErrorHandlerX11();

        if (window->context.glx.handle == null)
        {
            _glfwInputErrorX11(GLFW_VERSION_UNAVAILABLE, "GLX: Failed to create context");
            return GLFW_FALSE;
        }

        window->context.glx.window =
            _glfw.glx.CreateWindow(_glfw.x11.display, native, window->x11.handle, null);
        if (window->context.glx.window == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "GLX: Failed to create window");
            return GLFW_FALSE;
        }

        window->context.makeCurrent = &makeContextCurrentGLX;
        window->context.swapBuffers = &swapBuffersGLX;
        window->context.swapInterval = &swapIntervalGLX;
        window->context.extensionSupported = &extensionSupportedGLX;
        window->context.getProcAddress = &getProcAddressGLX;
        window->context.destroy = &destroyContextGLX;

        return GLFW_TRUE;
    }

    static int _glfwChooseVisualGLX(_GLFWwndconfig* wndconfig,
                                    _GLFWctxconfig* ctxconfig,
                                    _GLFWfbconfig* fbconfig,
                                    void** visual,
                                    int* depth)
    {
        void* native = null;

        if (chooseGLXFBConfig(fbconfig, &native) == 0)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "GLX: Failed to find a suitable GLXFBConfig");
            return GLFW_FALSE;
        }

        var result = _glfw.glx.GetVisualFromFBConfig(_glfw.x11.display, native);
        if (result == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "GLX: Failed to retrieve Visual for GLXFBConfig");
            return GLFW_FALSE;
        }

        if (visual != null)
            *visual = result->visual;
        if (depth != null)
            *depth = result->depth;

        x11_free(result);
        return GLFW_TRUE;
    }

    public static void* glfwGetGLXContext(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "GLX: Platform not initialized");
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_NATIVE_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return internalWindow->context.glx.handle;
    }

    public static nuint glfwGetGLXWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return 0;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_X11)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "GLX: Platform not initialized");
            return 0;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_NATIVE_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return 0;
        }

        return internalWindow->context.glx.window;
    }
}
