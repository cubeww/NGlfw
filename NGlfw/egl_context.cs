using System;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int EGL_SUCCESS = 0x3000;
    const int EGL_NOT_INITIALIZED = 0x3001;
    const int EGL_BAD_ACCESS = 0x3002;
    const int EGL_BAD_ALLOC = 0x3003;
    const int EGL_BAD_ATTRIBUTE = 0x3004;
    const int EGL_BAD_CONFIG = 0x3005;
    const int EGL_BAD_CONTEXT = 0x3006;
    const int EGL_BAD_CURRENT_SURFACE = 0x3007;
    const int EGL_BAD_DISPLAY = 0x3008;
    const int EGL_BAD_MATCH = 0x3009;
    const int EGL_BAD_NATIVE_PIXMAP = 0x300a;
    const int EGL_BAD_NATIVE_WINDOW = 0x300b;
    const int EGL_BAD_PARAMETER = 0x300c;
    const int EGL_BAD_SURFACE = 0x300d;
    const int EGL_CONTEXT_LOST = 0x300e;
    const int EGL_COLOR_BUFFER_TYPE = 0x303f;
    const int EGL_RGB_BUFFER = 0x308e;
    const int EGL_SURFACE_TYPE = 0x3033;
    const int EGL_WINDOW_BIT = 0x0004;
    const int EGL_RENDERABLE_TYPE = 0x3040;
    const int EGL_OPENGL_ES_BIT = 0x0001;
    const int EGL_OPENGL_ES2_BIT = 0x0004;
    const int EGL_OPENGL_BIT = 0x0008;
    const int EGL_ALPHA_SIZE = 0x3021;
    const int EGL_BLUE_SIZE = 0x3022;
    const int EGL_GREEN_SIZE = 0x3023;
    const int EGL_RED_SIZE = 0x3024;
    const int EGL_DEPTH_SIZE = 0x3025;
    const int EGL_STENCIL_SIZE = 0x3026;
    const int EGL_SAMPLES = 0x3031;
    const int EGL_OPENGL_ES_API = 0x30a0;
    const int EGL_OPENGL_API = 0x30a2;
    const int EGL_RENDER_BUFFER = 0x3086;
    const int EGL_SINGLE_BUFFER = 0x3085;
    const int EGL_EXTENSIONS = 0x3055;
    const int EGL_CONTEXT_CLIENT_VERSION = 0x3098;
    const int EGL_NATIVE_VISUAL_ID = 0x302e;
    const int EGL_CONTEXT_OPENGL_FORWARD_COMPATIBLE_BIT_KHR = 0x00000002;
    const int EGL_CONTEXT_OPENGL_CORE_PROFILE_BIT_KHR = 0x00000001;
    const int EGL_CONTEXT_OPENGL_COMPATIBILITY_PROFILE_BIT_KHR = 0x00000002;
    const int EGL_CONTEXT_OPENGL_DEBUG_BIT_KHR = 0x00000001;
    const int EGL_CONTEXT_OPENGL_RESET_NOTIFICATION_STRATEGY_KHR = 0x31bd;
    const int EGL_NO_RESET_NOTIFICATION_KHR = 0x31be;
    const int EGL_LOSE_CONTEXT_ON_RESET_KHR = 0x31bf;
    const int EGL_CONTEXT_OPENGL_ROBUST_ACCESS_BIT_KHR = 0x00000004;
    const int EGL_CONTEXT_MAJOR_VERSION_KHR = 0x3098;
    const int EGL_CONTEXT_MINOR_VERSION_KHR = 0x30fb;
    const int EGL_CONTEXT_OPENGL_PROFILE_MASK_KHR = 0x30fd;
    const int EGL_CONTEXT_FLAGS_KHR = 0x30fc;
    const int EGL_CONTEXT_OPENGL_NO_ERROR_KHR = 0x31b3;
    const int EGL_GL_COLORSPACE_KHR = 0x309d;
    const int EGL_GL_COLORSPACE_SRGB_KHR = 0x3089;
    const int EGL_CONTEXT_RELEASE_BEHAVIOR_KHR = 0x2097;
    const int EGL_CONTEXT_RELEASE_BEHAVIOR_NONE_KHR = 0;
    const int EGL_CONTEXT_RELEASE_BEHAVIOR_FLUSH_KHR = 0x2098;
    const int EGL_PLATFORM_WAYLAND_EXT = 0x31d8;
    const int EGL_PRESENT_OPAQUE_EXT = 0x31df;
    const nint VisualIDMask = 0x1;
    const nint VisualScreenMask = 0x2;

    static void* egl_loadModule(string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* path = bytes)
            return _glfwPlatformLoadModule(path);
    }

    static void* egl_getModuleSymbol(void* module, string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* symbol = bytes)
            return _glfwPlatformGetModuleSymbol(module, symbol);
    }

    static string egl_getErrorString(int error)
    {
        return error switch
        {
            EGL_SUCCESS => "Success",
            EGL_NOT_INITIALIZED => "EGL is not or could not be initialized",
            EGL_BAD_ACCESS => "EGL cannot access a requested resource",
            EGL_BAD_ALLOC => "EGL failed to allocate resources for the requested operation",
            EGL_BAD_ATTRIBUTE => "An unrecognized attribute or attribute value was passed in the attribute list",
            EGL_BAD_CONTEXT => "An EGLContext argument does not name a valid EGL rendering context",
            EGL_BAD_CONFIG => "An EGLConfig argument does not name a valid EGL frame buffer configuration",
            EGL_BAD_CURRENT_SURFACE => "The current surface of the calling thread is a window, pixel buffer or pixmap that is no longer valid",
            EGL_BAD_DISPLAY => "An EGLDisplay argument does not name a valid EGL display connection",
            EGL_BAD_SURFACE => "An EGLSurface argument does not name a valid surface configured for GL rendering",
            EGL_BAD_MATCH => "Arguments are inconsistent",
            EGL_BAD_PARAMETER => "One or more argument values are invalid",
            EGL_BAD_NATIVE_PIXMAP => "A NativePixmapType argument does not refer to a valid native pixmap",
            EGL_BAD_NATIVE_WINDOW => "A NativeWindowType argument does not refer to a valid native window",
            EGL_CONTEXT_LOST => "The application must destroy all contexts and reinitialise",
            _ => "ERROR: UNKNOWN EGL ERROR"
        };
    }

    static int egl_getConfigAttrib(void* config, int attrib)
    {
        int value = 0;
        _glfw.egl.GetConfigAttrib(_glfw.egl.display, config, attrib, &value);
        return value;
    }

    static int egl_stringInExtensionString(string extension, byte* extensions)
    {
        var bytes = Encoding.ASCII.GetBytes(extension + '\0');
        fixed (byte* name = bytes)
            return _glfwStringInExtensionString(name, extensions);
    }

    static void* egl_getProcAddress(string procname)
    {
        var bytes = Encoding.ASCII.GetBytes(procname + '\0');
        fixed (byte* name = bytes)
            return _glfw.egl.GetProcAddress(name);
    }

    static int egl_extensionSupported(string extension)
    {
        var extensions = _glfw.egl.QueryString(_glfw.egl.display, EGL_EXTENSIONS);
        if (extensions != null && egl_stringInExtensionString(extension, extensions) != 0)
            return GLFW_TRUE;

        return GLFW_FALSE;
    }

    static int chooseEGLConfig(_GLFWctxconfig* ctxconfig,
                               _GLFWfbconfig* fbconfig,
                               void** result)
    {
        int apiBit;
        var wrongApiAvailable = GLFW_FALSE;

        if (ctxconfig->client == GLFW_OPENGL_ES_API)
            apiBit = ctxconfig->major == 1 ? EGL_OPENGL_ES_BIT : EGL_OPENGL_ES2_BIT;
        else
            apiBit = EGL_OPENGL_BIT;

        if (fbconfig->stereo != 0)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "EGL: Stereo rendering not supported");
            return GLFW_FALSE;
        }

        int nativeCount;
        _glfw.egl.GetConfigs(_glfw.egl.display, null, 0, &nativeCount);
        if (nativeCount == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: No EGLConfigs returned");
            return GLFW_FALSE;
        }

        var nativeConfigs = (void**)_glfw_calloc((nuint)nativeCount, (nuint)sizeof(void*));
        if (nativeConfigs == null)
            return GLFW_FALSE;

        if (_glfw.egl.GetConfigs(_glfw.egl.display, nativeConfigs, nativeCount, &nativeCount) == 0)
        {
            _glfw_free(nativeConfigs);
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "EGL: Failed to retrieve EGLConfigs: {0}",
                egl_getErrorString(_glfw.egl.GetError()));
            return GLFW_FALSE;
        }

        var usableConfigs = (_GLFWfbconfig*)_glfw_calloc((nuint)nativeCount, (nuint)sizeof(_GLFWfbconfig));
        if (usableConfigs == null)
        {
            _glfw_free(nativeConfigs);
            return GLFW_FALSE;
        }

        var usableCount = 0;

        for (var i = 0; i < nativeCount; i++)
        {
            var native = nativeConfigs[i];
            var usable = usableConfigs + usableCount;

            if (egl_getConfigAttrib(native, EGL_COLOR_BUFFER_TYPE) != EGL_RGB_BUFFER)
                continue;

            if ((egl_getConfigAttrib(native, EGL_SURFACE_TYPE) & EGL_WINDOW_BIT) == 0)
                continue;

            if (_glfw.platform.platformID == GLFW_PLATFORM_X11)
            {
                XVisualInfo vi = default;

                vi.visualid = (nuint)egl_getConfigAttrib(native, EGL_NATIVE_VISUAL_ID);
                if (vi.visualid == 0)
                    continue;

                if (fbconfig->transparent != 0 && _glfw.x11.XGetVisualInfo != null)
                {
                    int count;
                    var vis = _glfw.x11.XGetVisualInfo(_glfw.x11.display, VisualIDMask, &vi, &count);
                    if (vis != null)
                    {
                        usable->transparent = _glfwIsVisualTransparentX11(vis->visual);
                        x11_free(vis);
                    }
                }
            }

            if ((egl_getConfigAttrib(native, EGL_RENDERABLE_TYPE) & apiBit) == 0)
            {
                wrongApiAvailable = GLFW_TRUE;
                continue;
            }

            usable->redBits = egl_getConfigAttrib(native, EGL_RED_SIZE);
            usable->greenBits = egl_getConfigAttrib(native, EGL_GREEN_SIZE);
            usable->blueBits = egl_getConfigAttrib(native, EGL_BLUE_SIZE);
            usable->alphaBits = egl_getConfigAttrib(native, EGL_ALPHA_SIZE);
            usable->depthBits = egl_getConfigAttrib(native, EGL_DEPTH_SIZE);
            usable->stencilBits = egl_getConfigAttrib(native, EGL_STENCIL_SIZE);
            usable->samples = egl_getConfigAttrib(native, EGL_SAMPLES);
            usable->doublebuffer = fbconfig->doublebuffer;
            usable->handle = (nuint)native;
            usableCount++;
        }

        var closest = _glfwChooseFBConfig(fbconfig, usableConfigs, (uint)usableCount);
        if (closest != null)
            *result = (void*)closest->handle;
        else
        {
            if (wrongApiAvailable != 0)
            {
                if (ctxconfig->client == GLFW_OPENGL_ES_API)
                {
                    if (ctxconfig->major == 1)
                        _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: Failed to find support for OpenGL ES 1.x");
                    else
                        _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: Failed to find support for OpenGL ES 2 or later");
                }
                else
                    _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: Failed to find support for OpenGL");
            }
            else
                _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "EGL: Failed to find a suitable EGLConfig");
        }

        _glfw_free(nativeConfigs);
        _glfw_free(usableConfigs);
        return closest != null ? GLFW_TRUE : GLFW_FALSE;
    }

    static void makeContextCurrentEGL(_GLFWwindow* window)
    {
        if (window != null)
        {
            if (_glfw.egl.MakeCurrent(_glfw.egl.display,
                    window->context.egl.surface,
                    window->context.egl.surface,
                    window->context.egl.handle) == 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR,
                    "EGL: Failed to make context current: {0}",
                    egl_getErrorString(_glfw.egl.GetError()));
                return;
            }
        }
        else
        {
            if (_glfw.egl.MakeCurrent(_glfw.egl.display, null, null, null) == 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR,
                    "EGL: Failed to clear current context: {0}",
                    egl_getErrorString(_glfw.egl.GetError()));
                return;
            }
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfwPlatformSetTls(&glfw->contextSlot, window);
    }

    static void swapBuffersEGL(_GLFWwindow* window)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (window != (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot))
            {
                _glfwInputError(GLFW_PLATFORM_ERROR,
                    "EGL: The context must be current on the calling thread when swapping buffers");
                return;
            }
        }

        if (_glfw.platform.platformID == GLFW_PLATFORM_WAYLAND)
        {
            if (window->wl.visible == 0)
                return;

            if (window->wl.egl.interval > 0 && _glfwWaitForEGLFrameWayland(window) == 0)
                return;
        }

        _glfw.egl.SwapBuffers(_glfw.egl.display, window->context.egl.surface);
    }

    static void swapIntervalEGL(int interval)
    {
        if (_glfw.platform.platformID == GLFW_PLATFORM_WAYLAND)
        {
            fixed (_GLFWlibrary* glfw = &_glfw)
            {
                var window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
                if (window != null)
                    window->wl.egl.interval = interval;
            }

            return;
        }

        _glfw.egl.SwapInterval(_glfw.egl.display, interval);
    }

    static int extensionSupportedEGL(byte* extension)
    {
        var extensions = _glfw.egl.QueryString(_glfw.egl.display, EGL_EXTENSIONS);
        if (extensions != null && _glfwStringInExtensionString(extension, extensions) != 0)
            return GLFW_TRUE;

        return GLFW_FALSE;
    }

    static void* getProcAddressEGL(byte* procname)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
            if (window != null && window->context.egl.client != null)
            {
                var proc = _glfwPlatformGetModuleSymbol(window->context.egl.client, procname);
                if (proc != null)
                    return proc;
            }
        }

        return _glfw.egl.GetProcAddress(procname);
    }

    static void destroyContextEGL(_GLFWwindow* window)
    {
        if ((_glfw.platform.platformID != GLFW_PLATFORM_X11 ||
             window->context.client != GLFW_OPENGL_API) &&
            window->context.egl.client != null)
        {
            _glfwPlatformFreeModule(window->context.egl.client);
            window->context.egl.client = null;
        }

        if (window->context.egl.surface != null)
        {
            _glfw.egl.DestroySurface(_glfw.egl.display, window->context.egl.surface);
            window->context.egl.surface = null;
        }

        if (window->context.egl.handle != null)
        {
            _glfw.egl.DestroyContext(_glfw.egl.display, window->context.egl.handle);
            window->context.egl.handle = null;
        }
    }

    static int _glfwInitEGL()
    {
        if (_glfw.egl.handle != null)
            return GLFW_TRUE;

        string[] sonames = OperatingSystem.IsWindows()
            ? ["libEGL.dll", "EGL.dll"]
            : OperatingSystem.IsMacOS()
                ? ["libEGL.dylib"]
                : ["libEGL.so.1", "libEGL.so"];

        string? selectedSoname = null;
        foreach (var soname in sonames)
        {
            _glfw.egl.handle = egl_loadModule(soname);
            if (_glfw.egl.handle != null)
            {
                selectedSoname = soname;
                break;
            }
        }

        if (_glfw.egl.handle == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: Library not found");
            return GLFW_FALSE;
        }

        _glfw.egl.prefix = selectedSoname != null &&
                           selectedSoname.StartsWith("lib", StringComparison.Ordinal)
            ? GLFW_TRUE
            : GLFW_FALSE;

        _glfw.egl.GetConfigAttrib =
            (delegate* unmanaged<void*, void*, int, int*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglGetConfigAttrib");
        _glfw.egl.GetConfigs =
            (delegate* unmanaged<void*, void**, int, int*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglGetConfigs");
        _glfw.egl.GetDisplay =
            (delegate* unmanaged<void*, void*>)egl_getModuleSymbol(_glfw.egl.handle, "eglGetDisplay");
        _glfw.egl.GetError =
            (delegate* unmanaged<int>)egl_getModuleSymbol(_glfw.egl.handle, "eglGetError");
        _glfw.egl.Initialize =
            (delegate* unmanaged<void*, int*, int*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglInitialize");
        _glfw.egl.Terminate =
            (delegate* unmanaged<void*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglTerminate");
        _glfw.egl.BindAPI =
            (delegate* unmanaged<int, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglBindAPI");
        _glfw.egl.CreateContext =
            (delegate* unmanaged<void*, void*, void*, int*, void*>)egl_getModuleSymbol(_glfw.egl.handle, "eglCreateContext");
        _glfw.egl.DestroySurface =
            (delegate* unmanaged<void*, void*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglDestroySurface");
        _glfw.egl.DestroyContext =
            (delegate* unmanaged<void*, void*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglDestroyContext");
        _glfw.egl.CreateWindowSurface =
            (delegate* unmanaged<void*, void*, void*, int*, void*>)egl_getModuleSymbol(_glfw.egl.handle, "eglCreateWindowSurface");
        _glfw.egl.MakeCurrent =
            (delegate* unmanaged<void*, void*, void*, void*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglMakeCurrent");
        _glfw.egl.SwapBuffers =
            (delegate* unmanaged<void*, void*, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglSwapBuffers");
        _glfw.egl.SwapInterval =
            (delegate* unmanaged<void*, int, int>)egl_getModuleSymbol(_glfw.egl.handle, "eglSwapInterval");
        _glfw.egl.QueryString =
            (delegate* unmanaged<void*, int, byte*>)egl_getModuleSymbol(_glfw.egl.handle, "eglQueryString");
        _glfw.egl.GetProcAddress =
            (delegate* unmanaged<byte*, void*>)egl_getModuleSymbol(_glfw.egl.handle, "eglGetProcAddress");

        if (_glfw.egl.GetConfigAttrib == null ||
            _glfw.egl.GetConfigs == null ||
            _glfw.egl.GetDisplay == null ||
            _glfw.egl.GetError == null ||
            _glfw.egl.Initialize == null ||
            _glfw.egl.Terminate == null ||
            _glfw.egl.BindAPI == null ||
            _glfw.egl.CreateContext == null ||
            _glfw.egl.DestroySurface == null ||
            _glfw.egl.DestroyContext == null ||
            _glfw.egl.CreateWindowSurface == null ||
            _glfw.egl.MakeCurrent == null ||
            _glfw.egl.SwapBuffers == null ||
            _glfw.egl.SwapInterval == null ||
            _glfw.egl.QueryString == null ||
            _glfw.egl.GetProcAddress == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "EGL: Failed to load required entry points");
            _glfwTerminateEGL();
            return GLFW_FALSE;
        }

        var extensions = _glfw.egl.QueryString(null, EGL_EXTENSIONS);
        if (extensions != null && _glfw.egl.GetError() == EGL_SUCCESS)
            _glfw.egl.EXT_client_extensions = GLFW_TRUE;

        if (_glfw.egl.EXT_client_extensions != 0)
        {
            _glfw.egl.EXT_platform_base =
                egl_stringInExtensionString("EGL_EXT_platform_base", extensions);
            _glfw.egl.EXT_platform_x11 =
                egl_stringInExtensionString("EGL_EXT_platform_x11", extensions);
            _glfw.egl.EXT_platform_wayland =
                egl_stringInExtensionString("EGL_EXT_platform_wayland", extensions);
            _glfw.egl.ANGLE_platform_angle =
                egl_stringInExtensionString("EGL_ANGLE_platform_angle", extensions);
            _glfw.egl.ANGLE_platform_angle_opengl =
                egl_stringInExtensionString("EGL_ANGLE_platform_angle_opengl", extensions);
            _glfw.egl.ANGLE_platform_angle_d3d =
                egl_stringInExtensionString("EGL_ANGLE_platform_angle_d3d", extensions);
            _glfw.egl.ANGLE_platform_angle_vulkan =
                egl_stringInExtensionString("EGL_ANGLE_platform_angle_vulkan", extensions);
            _glfw.egl.ANGLE_platform_angle_metal =
                egl_stringInExtensionString("EGL_ANGLE_platform_angle_metal", extensions);
        }

        if (_glfw.egl.EXT_platform_base != 0)
        {
            _glfw.egl.GetPlatformDisplayEXT =
                (delegate* unmanaged<int, void*, int*, void*>)egl_getProcAddress("eglGetPlatformDisplayEXT");
            _glfw.egl.CreatePlatformWindowSurfaceEXT =
                (delegate* unmanaged<void*, void*, void*, int*, void*>)egl_getProcAddress("eglCreatePlatformWindowSurfaceEXT");
        }

        int* attribs = null;
        var nativeDisplay = _glfw.platform.getEGLNativeDisplay != null
            ? _glfw.platform.getEGLNativeDisplay()
            : null;

        if (_glfw.platform.getEGLPlatform != null)
            _glfw.egl.platform = _glfw.platform.getEGLPlatform(&attribs);

        if (_glfw.egl.platform != 0 && _glfw.egl.GetPlatformDisplayEXT != null)
            _glfw.egl.display = _glfw.egl.GetPlatformDisplayEXT(_glfw.egl.platform, nativeDisplay, attribs);
        else
            _glfw.egl.display = _glfw.egl.GetDisplay(nativeDisplay);

        _glfw_free(attribs);

        if (_glfw.egl.display == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "EGL: Failed to get EGL display: {0}",
                egl_getErrorString(_glfw.egl.GetError()));
            _glfwTerminateEGL();
            return GLFW_FALSE;
        }

        int major;
        int minor;
        if (_glfw.egl.Initialize(_glfw.egl.display, &major, &minor) == 0)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "EGL: Failed to initialize EGL: {0}",
                egl_getErrorString(_glfw.egl.GetError()));
            _glfwTerminateEGL();
            return GLFW_FALSE;
        }

        _glfw.egl.major = major;
        _glfw.egl.minor = minor;

        _glfw.egl.KHR_create_context = egl_extensionSupported("EGL_KHR_create_context");
        _glfw.egl.KHR_create_context_no_error = egl_extensionSupported("EGL_KHR_create_context_no_error");
        _glfw.egl.KHR_gl_colorspace = egl_extensionSupported("EGL_KHR_gl_colorspace");
        _glfw.egl.KHR_get_all_proc_addresses = egl_extensionSupported("EGL_KHR_get_all_proc_addresses");
        _glfw.egl.KHR_context_flush_control = egl_extensionSupported("EGL_KHR_context_flush_control");
        _glfw.egl.EXT_present_opaque = egl_extensionSupported("EGL_EXT_present_opaque");

        return GLFW_TRUE;
    }

    static void _glfwTerminateEGL()
    {
        if (_glfw.egl.display != null && _glfw.egl.Terminate != null)
            _glfw.egl.Terminate(_glfw.egl.display);

        if (_glfw.egl.handle != null)
            _glfwPlatformFreeModule(_glfw.egl.handle);

        _glfw.egl = default;
    }

    static int _glfwCreateContextEGL(_GLFWwindow* window,
                                     _GLFWctxconfig* ctxconfig,
                                     _GLFWfbconfig* fbconfig)
    {
        void* config = null;
        var share = ctxconfig->share != null ? ctxconfig->share->context.egl.handle : null;
        var attribs = stackalloc int[40];
        var index = 0;

        void SetAttrib(int attrib, int value)
        {
            attribs[index++] = attrib;
            attribs[index++] = value;
        }

        if (_glfw.egl.display == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: API not available");
            return GLFW_FALSE;
        }

        if (chooseEGLConfig(ctxconfig, fbconfig, &config) == 0)
            return GLFW_FALSE;

        if (ctxconfig->client == GLFW_OPENGL_ES_API)
        {
            if (_glfw.egl.BindAPI(EGL_OPENGL_ES_API) == 0)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "EGL: Failed to bind OpenGL ES: {0}",
                    egl_getErrorString(_glfw.egl.GetError()));
                return GLFW_FALSE;
            }
        }
        else
        {
            if (_glfw.egl.BindAPI(EGL_OPENGL_API) == 0)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE,
                    "EGL: Failed to bind OpenGL: {0}",
                    egl_getErrorString(_glfw.egl.GetError()));
                return GLFW_FALSE;
            }
        }

        if (_glfw.egl.KHR_create_context != 0)
        {
            var mask = 0;
            var flags = 0;

            if (ctxconfig->client == GLFW_OPENGL_API)
            {
                if (ctxconfig->forward != 0)
                    flags |= EGL_CONTEXT_OPENGL_FORWARD_COMPATIBLE_BIT_KHR;

                if (ctxconfig->profile == GLFW_OPENGL_CORE_PROFILE)
                    mask |= EGL_CONTEXT_OPENGL_CORE_PROFILE_BIT_KHR;
                else if (ctxconfig->profile == GLFW_OPENGL_COMPAT_PROFILE)
                    mask |= EGL_CONTEXT_OPENGL_COMPATIBILITY_PROFILE_BIT_KHR;
            }

            if (ctxconfig->debug != 0)
                flags |= EGL_CONTEXT_OPENGL_DEBUG_BIT_KHR;

            if (ctxconfig->robustness != 0)
            {
                if (ctxconfig->robustness == GLFW_NO_RESET_NOTIFICATION)
                    SetAttrib(EGL_CONTEXT_OPENGL_RESET_NOTIFICATION_STRATEGY_KHR, EGL_NO_RESET_NOTIFICATION_KHR);
                else if (ctxconfig->robustness == GLFW_LOSE_CONTEXT_ON_RESET)
                    SetAttrib(EGL_CONTEXT_OPENGL_RESET_NOTIFICATION_STRATEGY_KHR, EGL_LOSE_CONTEXT_ON_RESET_KHR);

                flags |= EGL_CONTEXT_OPENGL_ROBUST_ACCESS_BIT_KHR;
            }

            if (ctxconfig->major != 1 || ctxconfig->minor != 0)
            {
                SetAttrib(EGL_CONTEXT_MAJOR_VERSION_KHR, ctxconfig->major);
                SetAttrib(EGL_CONTEXT_MINOR_VERSION_KHR, ctxconfig->minor);
            }

            if (ctxconfig->noerror != 0 && _glfw.egl.KHR_create_context_no_error != 0)
                SetAttrib(EGL_CONTEXT_OPENGL_NO_ERROR_KHR, GLFW_TRUE);

            if (mask != 0)
                SetAttrib(EGL_CONTEXT_OPENGL_PROFILE_MASK_KHR, mask);

            if (flags != 0)
                SetAttrib(EGL_CONTEXT_FLAGS_KHR, flags);
        }
        else if (ctxconfig->client == GLFW_OPENGL_ES_API)
            SetAttrib(EGL_CONTEXT_CLIENT_VERSION, ctxconfig->major);

        if (_glfw.egl.KHR_context_flush_control != 0)
        {
            if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_NONE)
                SetAttrib(EGL_CONTEXT_RELEASE_BEHAVIOR_KHR, EGL_CONTEXT_RELEASE_BEHAVIOR_NONE_KHR);
            else if (ctxconfig->release == GLFW_RELEASE_BEHAVIOR_FLUSH)
                SetAttrib(EGL_CONTEXT_RELEASE_BEHAVIOR_KHR, EGL_CONTEXT_RELEASE_BEHAVIOR_FLUSH_KHR);
        }

        SetAttrib(EGL_NONE, EGL_NONE);

        window->context.egl.handle =
            _glfw.egl.CreateContext(_glfw.egl.display, config, share, attribs);

        if (window->context.egl.handle == null)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                "EGL: Failed to create context: {0}",
                egl_getErrorString(_glfw.egl.GetError()));
            return GLFW_FALSE;
        }

        index = 0;

        if (fbconfig->sRGB != 0 && _glfw.egl.KHR_gl_colorspace != 0)
            SetAttrib(EGL_GL_COLORSPACE_KHR, EGL_GL_COLORSPACE_SRGB_KHR);

        if (fbconfig->doublebuffer == 0)
            SetAttrib(EGL_RENDER_BUFFER, EGL_SINGLE_BUFFER);

        if (_glfw.platform.platformID == GLFW_PLATFORM_WAYLAND && _glfw.egl.EXT_present_opaque != 0)
            SetAttrib(EGL_PRESENT_OPAQUE_EXT, fbconfig->transparent == 0 ? GLFW_TRUE : GLFW_FALSE);

        SetAttrib(EGL_NONE, EGL_NONE);

        var native = _glfw.platform.getEGLNativeWindow != null
            ? _glfw.platform.getEGLNativeWindow(window)
            : null;

        if (_glfw.egl.platform != 0 &&
            _glfw.egl.platform != EGL_PLATFORM_ANGLE_ANGLE &&
            _glfw.egl.CreatePlatformWindowSurfaceEXT != null)
        {
            window->context.egl.surface =
                _glfw.egl.CreatePlatformWindowSurfaceEXT(_glfw.egl.display, config, native, attribs);
        }
        else
        {
            window->context.egl.surface =
                _glfw.egl.CreateWindowSurface(_glfw.egl.display, config, native, attribs);
        }

        if (window->context.egl.surface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "EGL: Failed to create window surface: {0}",
                egl_getErrorString(_glfw.egl.GetError()));
            return GLFW_FALSE;
        }

        window->context.egl.config = config;

        if (_glfw.egl.KHR_get_all_proc_addresses == 0)
        {
            string[] sonames;

            if (ctxconfig->client == GLFW_OPENGL_ES_API)
            {
                if (ctxconfig->major == 1)
                {
                    sonames = OperatingSystem.IsWindows()
                        ? ["GLESv1_CM.dll", "libGLES_CM.dll"]
                        : OperatingSystem.IsMacOS()
                            ? ["libGLESv1_CM.dylib"]
                            : ["libGLESv1_CM.so.1", "libGLES_CM.so.1", "libGLESv1_CM.so"];
                }
                else
                {
                    sonames = OperatingSystem.IsWindows()
                        ? ["GLESv2.dll", "libGLESv2.dll"]
                        : OperatingSystem.IsMacOS()
                            ? ["libGLESv2.dylib"]
                            : ["libGLESv2.so.2", "libGLESv2.so"];
                }
            }
            else
            {
                sonames = OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
                    ? []
                    : ["libOpenGL.so.0", "libGL.so.1", "libGL.so"];
            }

            foreach (var soname in sonames)
            {
                if (_glfw.egl.prefix != (soname.StartsWith("lib", StringComparison.Ordinal) ? GLFW_TRUE : GLFW_FALSE))
                    continue;

                window->context.egl.client = egl_loadModule(soname);
                if (window->context.egl.client != null)
                    break;
            }

            if (window->context.egl.client == null)
            {
                _glfwInputError(GLFW_API_UNAVAILABLE, "EGL: Failed to load client library");
                return GLFW_FALSE;
            }
        }

        window->context.makeCurrent = &makeContextCurrentEGL;
        window->context.swapBuffers = &swapBuffersEGL;
        window->context.swapInterval = &swapIntervalEGL;
        window->context.extensionSupported = &extensionSupportedEGL;
        window->context.getProcAddress = &getProcAddressEGL;
        window->context.destroy = &destroyContextEGL;

        return GLFW_TRUE;
    }

    static int _glfwChooseVisualEGL(_GLFWwndconfig* wndconfig,
                                    _GLFWctxconfig* ctxconfig,
                                    _GLFWfbconfig* fbconfig,
                                    void** visual,
                                    int* depth)
    {
        void* native = null;
        var visualID = 0;
        var count = 0;
        const nint vimask = VisualScreenMask | VisualIDMask;

        if (chooseEGLConfig(ctxconfig, fbconfig, &native) == 0)
            return GLFW_FALSE;

        _glfw.egl.GetConfigAttrib(_glfw.egl.display, native, EGL_NATIVE_VISUAL_ID, &visualID);

        XVisualInfo desired = default;
        desired.screen = _glfw.x11.screen;
        desired.visualid = (nuint)visualID;

        if (_glfw.x11.XGetVisualInfo == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "EGL: XGetVisualInfo entry point is unavailable");
            return GLFW_FALSE;
        }

        var result = _glfw.x11.XGetVisualInfo(_glfw.x11.display, vimask, &desired, &count);
        if (result == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "EGL: Failed to retrieve Visual for EGLConfig");
            return GLFW_FALSE;
        }

        if (visual != null)
            *visual = result->visual;
        if (depth != null)
            *depth = result->depth;

        x11_free(result);
        return GLFW_TRUE;
    }

    public static void* glfwGetEGLDisplay()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        return _glfw.egl.display;
    }

    public static void* glfwGetEGLContext(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_EGL_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return internalWindow->context.egl.handle;
    }

    public static void* glfwGetEGLSurface(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_EGL_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return internalWindow->context.egl.surface;
    }
}
