using System;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint GL_UNSIGNED_BYTE = 0x1401;
    const int OSMESA_RGBA = 0x1908;
    const int OSMESA_FORMAT = 0x22;
    const int OSMESA_DEPTH_BITS = 0x30;
    const int OSMESA_STENCIL_BITS = 0x31;
    const int OSMESA_ACCUM_BITS = 0x32;
    const int OSMESA_PROFILE = 0x33;
    const int OSMESA_CORE_PROFILE = 0x34;
    const int OSMESA_COMPAT_PROFILE = 0x35;
    const int OSMESA_CONTEXT_MAJOR_VERSION = 0x36;
    const int OSMESA_CONTEXT_MINOR_VERSION = 0x37;

    static void* osmesa_loadModule(string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* path = bytes)
            return _glfwPlatformLoadModule(path);
    }

    static void* osmesa_getModuleSymbol(void* module, string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* symbol = bytes)
            return _glfwPlatformGetModuleSymbol(module, symbol);
    }

    static void makeContextCurrentOSMesa(_GLFWwindow* window)
    {
        if (window != null)
        {
            int width;
            int height;
            _glfw.platform.getFramebufferSize(window, &width, &height);

            if (window->context.osmesa.buffer == null ||
                width != window->context.osmesa.width ||
                height != window->context.osmesa.height)
            {
                _glfw_free(window->context.osmesa.buffer);

                window->context.osmesa.buffer = _glfw_calloc(4, (nuint)(width * height));
                window->context.osmesa.width = width;
                window->context.osmesa.height = height;
            }

            if (_glfw.osmesa.MakeCurrent(window->context.osmesa.handle,
                    window->context.osmesa.buffer,
                    GL_UNSIGNED_BYTE,
                    width,
                    height) == 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "OSMesa: Failed to make context current");
                return;
            }
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfwPlatformSetTls(&glfw->contextSlot, window);
    }

    static void* getProcAddressOSMesa(byte* procname)
    {
        return _glfw.osmesa.GetProcAddress(procname);
    }

    static void destroyContextOSMesa(_GLFWwindow* window)
    {
        if (window->context.osmesa.handle != null)
        {
            _glfw.osmesa.DestroyContext(window->context.osmesa.handle);
            window->context.osmesa.handle = null;
        }

        if (window->context.osmesa.buffer != null)
        {
            _glfw_free(window->context.osmesa.buffer);
            window->context.osmesa.buffer = null;
            window->context.osmesa.width = 0;
            window->context.osmesa.height = 0;
        }
    }

    static void swapBuffersOSMesa(_GLFWwindow* window)
    {
    }

    static void swapIntervalOSMesa(int interval)
    {
    }

    static int extensionSupportedOSMesa(byte* extension)
    {
        return GLFW_FALSE;
    }

    static int _glfwInitOSMesa()
    {
        if (_glfw.osmesa.handle != null)
            return GLFW_TRUE;

        string[] sonames = OperatingSystem.IsWindows()
            ? ["libOSMesa.dll", "OSMesa.dll"]
            : OperatingSystem.IsMacOS()
                ? ["libOSMesa.8.dylib"]
                : ["libOSMesa.so.8", "libOSMesa.so.6", "libOSMesa.so"];

        foreach (var soname in sonames)
        {
            _glfw.osmesa.handle = osmesa_loadModule(soname);
            if (_glfw.osmesa.handle != null)
                break;
        }

        if (_glfw.osmesa.handle == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "OSMesa: Library not found");
            return GLFW_FALSE;
        }

        _glfw.osmesa.CreateContextExt =
            (delegate* unmanaged<int, int, int, int, void*, void*>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaCreateContextExt");
        _glfw.osmesa.CreateContextAttribs =
            (delegate* unmanaged<int*, void*, void*>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaCreateContextAttribs");
        _glfw.osmesa.DestroyContext =
            (delegate* unmanaged<void*, void>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaDestroyContext");
        _glfw.osmesa.MakeCurrent =
            (delegate* unmanaged<void*, void*, uint, int, int, int>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaMakeCurrent");
        _glfw.osmesa.GetColorBuffer =
            (delegate* unmanaged<void*, int*, int*, int*, void**, int>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaGetColorBuffer");
        _glfw.osmesa.GetDepthBuffer =
            (delegate* unmanaged<void*, int*, int*, int*, void**, int>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaGetDepthBuffer");
        _glfw.osmesa.GetProcAddress =
            (delegate* unmanaged<byte*, void*>)osmesa_getModuleSymbol(_glfw.osmesa.handle, "OSMesaGetProcAddress");

        if (_glfw.osmesa.CreateContextExt == null ||
            _glfw.osmesa.DestroyContext == null ||
            _glfw.osmesa.MakeCurrent == null ||
            _glfw.osmesa.GetColorBuffer == null ||
            _glfw.osmesa.GetDepthBuffer == null ||
            _glfw.osmesa.GetProcAddress == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "OSMesa: Failed to load required entry points");
            _glfwTerminateOSMesa();
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateOSMesa()
    {
        if (_glfw.osmesa.handle != null)
            _glfwPlatformFreeModule(_glfw.osmesa.handle);

        _glfw.osmesa = default;
    }

    static int _glfwCreateContextOSMesa(_GLFWwindow* window,
                                        _GLFWctxconfig* ctxconfig,
                                        _GLFWfbconfig* fbconfig)
    {
        var share = ctxconfig->share != null ? ctxconfig->share->context.osmesa.handle : null;
        var accumBits = fbconfig->accumRedBits +
                        fbconfig->accumGreenBits +
                        fbconfig->accumBlueBits +
                        fbconfig->accumAlphaBits;

        if (ctxconfig->client == GLFW_OPENGL_ES_API)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE, "OSMesa: OpenGL ES is not available on OSMesa");
            return GLFW_FALSE;
        }

        if (_glfw.osmesa.CreateContextAttribs != null)
        {
            var index = 0;
            var attribs = stackalloc int[40];

            void SetAttrib(int attrib, int value)
            {
                attribs[index++] = attrib;
                attribs[index++] = value;
            }

            SetAttrib(OSMESA_FORMAT, OSMESA_RGBA);
            SetAttrib(OSMESA_DEPTH_BITS, fbconfig->depthBits);
            SetAttrib(OSMESA_STENCIL_BITS, fbconfig->stencilBits);
            SetAttrib(OSMESA_ACCUM_BITS, accumBits);

            if (ctxconfig->profile == GLFW_OPENGL_CORE_PROFILE)
                SetAttrib(OSMESA_PROFILE, OSMESA_CORE_PROFILE);
            else if (ctxconfig->profile == GLFW_OPENGL_COMPAT_PROFILE)
                SetAttrib(OSMESA_PROFILE, OSMESA_COMPAT_PROFILE);

            if (ctxconfig->major != 1 || ctxconfig->minor != 0)
            {
                SetAttrib(OSMESA_CONTEXT_MAJOR_VERSION, ctxconfig->major);
                SetAttrib(OSMESA_CONTEXT_MINOR_VERSION, ctxconfig->minor);
            }

            if (ctxconfig->forward != 0)
            {
                _glfwInputError(GLFW_VERSION_UNAVAILABLE, "OSMesa: Forward-compatible contexts not supported");
                return GLFW_FALSE;
            }

            SetAttrib(0, 0);

            window->context.osmesa.handle =
                _glfw.osmesa.CreateContextAttribs(attribs, share);
        }
        else
        {
            if (ctxconfig->profile != 0)
            {
                _glfwInputError(GLFW_VERSION_UNAVAILABLE, "OSMesa: OpenGL profiles unavailable");
                return GLFW_FALSE;
            }

            window->context.osmesa.handle =
                _glfw.osmesa.CreateContextExt(OSMESA_RGBA,
                    fbconfig->depthBits,
                    fbconfig->stencilBits,
                    accumBits,
                    share);
        }

        if (window->context.osmesa.handle == null)
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE, "OSMesa: Failed to create context");
            return GLFW_FALSE;
        }

        window->context.makeCurrent = &makeContextCurrentOSMesa;
        window->context.swapBuffers = &swapBuffersOSMesa;
        window->context.swapInterval = &swapIntervalOSMesa;
        window->context.extensionSupported = &extensionSupportedOSMesa;
        window->context.getProcAddress = &getProcAddressOSMesa;
        window->context.destroy = &destroyContextOSMesa;

        return GLFW_TRUE;
    }

    public static int glfwGetOSMesaColorBuffer(GLFWwindow* window,
                                               int* width,
                                               int* height,
                                               int* format,
                                               void** buffer)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_OSMESA_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return GLFW_FALSE;
        }

        int mesaWidth;
        int mesaHeight;
        int mesaFormat;
        void* mesaBuffer;
        if (_glfw.osmesa.GetColorBuffer(internalWindow->context.osmesa.handle,
                &mesaWidth,
                &mesaHeight,
                &mesaFormat,
                &mesaBuffer) == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "OSMesa: Failed to retrieve color buffer");
            return GLFW_FALSE;
        }

        if (width != null)
            *width = mesaWidth;
        if (height != null)
            *height = mesaHeight;
        if (format != null)
            *format = mesaFormat;
        if (buffer != null)
            *buffer = mesaBuffer;

        return GLFW_TRUE;
    }

    public static int glfwGetOSMesaDepthBuffer(GLFWwindow* window,
                                               int* width,
                                               int* height,
                                               int* bytesPerValue,
                                               void** buffer)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_OSMESA_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return GLFW_FALSE;
        }

        int mesaWidth;
        int mesaHeight;
        int mesaBytes;
        void* mesaBuffer;
        if (_glfw.osmesa.GetDepthBuffer(internalWindow->context.osmesa.handle,
                &mesaWidth,
                &mesaHeight,
                &mesaBytes,
                &mesaBuffer) == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "OSMesa: Failed to retrieve depth buffer");
            return GLFW_FALSE;
        }

        if (width != null)
            *width = mesaWidth;
        if (height != null)
            *height = mesaHeight;
        if (bytesPerValue != null)
            *bytesPerValue = mesaBytes;
        if (buffer != null)
            *buffer = mesaBuffer;

        return GLFW_TRUE;
    }

    public static void* glfwGetOSMesaContext(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.source != GLFW_OSMESA_CONTEXT_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
            return null;
        }

        return internalWindow->context.osmesa.handle;
    }
}
