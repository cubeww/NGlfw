namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint kCFStringEncodingASCII = 0x0600;

    const int NSOpenGLPFAAllRenderers = 1;
    const int NSOpenGLPFADoubleBuffer = 5;
    const int NSOpenGLPFAStereo = 6;
    const int NSOpenGLPFAAuxBuffers = 7;
    const int NSOpenGLPFAColorSize = 8;
    const int NSOpenGLPFAAlphaSize = 11;
    const int NSOpenGLPFADepthSize = 12;
    const int NSOpenGLPFAStencilSize = 13;
    const int NSOpenGLPFAAccumSize = 14;
    const int NSOpenGLPFASampleBuffers = 55;
    const int NSOpenGLPFASamples = 56;
    const int NSOpenGLPFAAccelerated = 73;
    const int NSOpenGLPFAClosestPolicy = 74;
    const int NSOpenGLPFAAllowOfflineRenderers = 96;
    const int NSOpenGLPFAOpenGLProfile = 99;
    const int kCGLPFASupportsAutomaticGraphicsSwitching = 101;

    const int NSOpenGLProfileVersion3_2Core = 0x3200;
    const int NSOpenGLProfileVersion4_1Core = 0x4100;

    const int NSOpenGLContextParameterSwapInterval = 222;
    const int NSOpenGLContextParameterSurfaceOpacity = 236;

    static void makeContextCurrentNSGL(_GLFWwindow* window)
    {
        if (window != null)
            cocoa_msgSend_void(window->context.nsgl.@object, "makeCurrentContext");
        else
            cocoa_msgSend_void(cocoa_getClass("NSOpenGLContext"), "clearCurrentContext");

        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfwPlatformSetTls(&glfw->contextSlot, window);
    }

    static void swapBuffersNSGL(_GLFWwindow* window)
    {
        cocoa_msgSend_void(window->context.nsgl.@object, "flushBuffer");
    }

    static void swapIntervalNSGL(int interval)
    {
        _GLFWwindow* window;
        fixed (_GLFWlibrary* glfw = &_glfw)
            window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);

        if (window == null || window->context.nsgl.@object == null)
            return;

        objc_msgSend_void_ptr_long(window->context.nsgl.@object,
            cocoa_sel("setValues:forParameter:"),
            &interval,
            NSOpenGLContextParameterSwapInterval);
    }

    static int extensionSupportedNSGL(byte* extension)
    {
        return GLFW_FALSE;
    }

    static void* getProcAddressNSGL(byte* procname)
    {
        if (_glfw.nsgl.framework == null || procname == null)
            return null;

        var symbolName = CFStringCreateWithCString(null, procname, kCFStringEncodingASCII);
        if (symbolName == null)
            return null;

        var symbol = CFBundleGetFunctionPointerForName(_glfw.nsgl.framework, symbolName);
        CFRelease(symbolName);
        return symbol;
    }

    static void destroyContextNSGL(_GLFWwindow* window)
    {
        cocoa_msgSend_void(window->context.nsgl.pixelFormat, "release");
        window->context.nsgl.pixelFormat = null;

        cocoa_msgSend_void(window->context.nsgl.@object, "release");
        window->context.nsgl.@object = null;
    }

    static int _glfwInitNSGL()
    {
        if (!OperatingSystem.IsMacOS())
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "NSGL: Platform not available on this system");
            return GLFW_FALSE;
        }

        if (_glfw.nsgl.framework != null)
            return GLFW_TRUE;

        var bundleID = CFStringCreateWithCString(null, _glfwCocoaOpenGLBundleID, kCFStringEncodingASCII);
        if (bundleID == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "NSGL: Failed to create OpenGL framework bundle identifier");
            return GLFW_FALSE;
        }

        _glfw.nsgl.framework = CFBundleGetBundleWithIdentifier(bundleID);
        CFRelease(bundleID);

        if (_glfw.nsgl.framework == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "NSGL: Failed to locate OpenGL framework");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateNSGL()
    {
        _glfw.nsgl = default;
    }

    static int _glfwCreateContextNSGL(_GLFWwindow* window,
                                      _GLFWctxconfig* ctxconfig,
                                      _GLFWfbconfig* fbconfig)
    {
        if (ctxconfig->client == GLFW_OPENGL_ES_API)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "NSGL: OpenGL ES is not available via NSGL");
            return GLFW_FALSE;
        }

        if (ctxconfig->major > 2 && ctxconfig->major == 3 && ctxconfig->minor < 2)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                "NSGL: The targeted version of macOS does not support OpenGL 3.0 or 3.1 but may support 3.2 and above");
            return GLFW_FALSE;
        }

        if (ctxconfig->major >= 3 && ctxconfig->profile == GLFW_OPENGL_COMPAT_PROFILE)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                "NSGL: The compatibility profile is not available on macOS");
            return GLFW_FALSE;
        }

        var attribs = stackalloc int[40];
        var index = 0;

        void AddAttrib(int attrib)
        {
            if (index < 40)
                attribs[index++] = attrib;
        }

        void SetAttrib(int attrib, int value)
        {
            AddAttrib(attrib);
            AddAttrib(value);
        }

        AddAttrib(NSOpenGLPFAAccelerated);
        AddAttrib(NSOpenGLPFAClosestPolicy);

        if (ctxconfig->nsgl.offline != 0)
        {
            AddAttrib(NSOpenGLPFAAllowOfflineRenderers);
            AddAttrib(kCGLPFASupportsAutomaticGraphicsSwitching);
        }

        if (ctxconfig->major >= 4)
            SetAttrib(NSOpenGLPFAOpenGLProfile, NSOpenGLProfileVersion4_1Core);
        else if (ctxconfig->major >= 3)
            SetAttrib(NSOpenGLPFAOpenGLProfile, NSOpenGLProfileVersion3_2Core);

        if (ctxconfig->major <= 2)
        {
            if (fbconfig->auxBuffers != GLFW_DONT_CARE)
                SetAttrib(NSOpenGLPFAAuxBuffers, fbconfig->auxBuffers);

            if (fbconfig->accumRedBits != GLFW_DONT_CARE &&
                fbconfig->accumGreenBits != GLFW_DONT_CARE &&
                fbconfig->accumBlueBits != GLFW_DONT_CARE &&
                fbconfig->accumAlphaBits != GLFW_DONT_CARE)
            {
                SetAttrib(NSOpenGLPFAAccumSize,
                    fbconfig->accumRedBits +
                    fbconfig->accumGreenBits +
                    fbconfig->accumBlueBits +
                    fbconfig->accumAlphaBits);
            }
        }

        if (fbconfig->redBits != GLFW_DONT_CARE &&
            fbconfig->greenBits != GLFW_DONT_CARE &&
            fbconfig->blueBits != GLFW_DONT_CARE)
        {
            var colorBits = fbconfig->redBits + fbconfig->greenBits + fbconfig->blueBits;
            if (colorBits == 0)
                colorBits = 24;
            else if (colorBits < 15)
                colorBits = 15;

            SetAttrib(NSOpenGLPFAColorSize, colorBits);
        }

        if (fbconfig->alphaBits != GLFW_DONT_CARE)
            SetAttrib(NSOpenGLPFAAlphaSize, fbconfig->alphaBits);
        if (fbconfig->depthBits != GLFW_DONT_CARE)
            SetAttrib(NSOpenGLPFADepthSize, fbconfig->depthBits);
        if (fbconfig->stencilBits != GLFW_DONT_CARE)
            SetAttrib(NSOpenGLPFAStencilSize, fbconfig->stencilBits);

        if (fbconfig->stereo != 0)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "NSGL: Stereo rendering is deprecated");
            return GLFW_FALSE;
        }

        if (fbconfig->doublebuffer != 0)
            AddAttrib(NSOpenGLPFADoubleBuffer);

        if (fbconfig->samples != GLFW_DONT_CARE)
        {
            if (fbconfig->samples == 0)
                SetAttrib(NSOpenGLPFASampleBuffers, 0);
            else
            {
                SetAttrib(NSOpenGLPFASampleBuffers, 1);
                SetAttrib(NSOpenGLPFASamples, fbconfig->samples);
            }
        }

        AddAttrib(0);

        var pixelFormatClass = cocoa_getClass("NSOpenGLPixelFormat");
        var allocatedPixelFormat = cocoa_msgSend_id(pixelFormatClass, "alloc");
        window->context.nsgl.pixelFormat =
            cocoa_msgSend_id_ptr(allocatedPixelFormat, "initWithAttributes:", attribs);

        if (window->context.nsgl.pixelFormat == null)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "NSGL: Failed to find a suitable pixel format");
            return GLFW_FALSE;
        }

        var share = ctxconfig->share != null ? ctxconfig->share->context.nsgl.@object : null;
        var contextClass = cocoa_getClass("NSOpenGLContext");
        var allocatedContext = cocoa_msgSend_id(contextClass, "alloc");
        window->context.nsgl.@object = objc_msgSend_id_ptr_ptr(allocatedContext,
            cocoa_sel("initWithFormat:shareContext:"),
            window->context.nsgl.pixelFormat,
            share);

        if (window->context.nsgl.@object == null)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE, "NSGL: Failed to create OpenGL context");
            return GLFW_FALSE;
        }

        if (fbconfig->transparent != 0)
        {
            var opaque = 0;
            objc_msgSend_void_ptr_long(window->context.nsgl.@object,
                cocoa_sel("setValues:forParameter:"),
                &opaque,
                NSOpenGLContextParameterSurfaceOpacity);
        }

        cocoa_msgSend_void_bool(window->ns.view,
            "setWantsBestResolutionOpenGLSurface:",
            window->ns.retina);
        cocoa_msgSend_void_ptr(window->context.nsgl.@object, "setView:", window->ns.view);

        window->context.makeCurrent = &makeContextCurrentNSGL;
        window->context.swapBuffers = &swapBuffersNSGL;
        window->context.swapInterval = &swapIntervalNSGL;
        window->context.extensionSupported = &extensionSupportedNSGL;
        window->context.getProcAddress = &getProcAddressNSGL;
        window->context.destroy = &destroyContextNSGL;

        return GLFW_TRUE;
    }

    public static void* glfwGetNSGLContext(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_COCOA)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "NSGL: Platform not initialized");
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_NATIVE_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return internalWindow->context.nsgl.@object;
    }
}
