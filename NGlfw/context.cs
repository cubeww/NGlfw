using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint GLFW_UINT_MAX = uint.MaxValue;
    const uint GL_VERSION = 0x1f02;
    const uint GL_NONE = 0;
    const uint GL_COLOR_BUFFER_BIT = 0x00004000;
    const uint GL_EXTENSIONS = 0x1f03;
    const uint GL_NUM_EXTENSIONS = 0x821d;
    const uint GL_CONTEXT_FLAGS = 0x821e;
    const uint GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 0x00000001;
    const uint GL_CONTEXT_FLAG_DEBUG_BIT = 0x00000002;
    const uint GL_CONTEXT_PROFILE_MASK = 0x9126;
    const uint GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x00000002;
    const uint GL_CONTEXT_CORE_PROFILE_BIT = 0x00000001;
    const uint GL_RESET_NOTIFICATION_STRATEGY_ARB = 0x8256;
    const int GL_LOSE_CONTEXT_ON_RESET_ARB = 0x8252;
    const int GL_NO_RESET_NOTIFICATION_ARB = 0x8261;
    const uint GL_CONTEXT_RELEASE_BEHAVIOR = 0x82fb;
    const int GL_CONTEXT_RELEASE_BEHAVIOR_FLUSH = 0x82fc;
    const uint GL_CONTEXT_FLAG_NO_ERROR_BIT_KHR = 0x00000008;

    static int _glfwIsValidContextConfig(_GLFWctxconfig* ctxconfig)
    {
        if (ctxconfig->source != GLFW_NATIVE_CONTEXT_API &&
            ctxconfig->source != GLFW_EGL_CONTEXT_API &&
            ctxconfig->source != GLFW_OSMESA_CONTEXT_API)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid context creation API 0x%08X", ctxconfig->source);
            return GLFW_FALSE;
        }

        if (ctxconfig->client != GLFW_NO_API &&
            ctxconfig->client != GLFW_OPENGL_API &&
            ctxconfig->client != GLFW_OPENGL_ES_API)
        {
            _glfwInputError(GLFW_INVALID_ENUM, "Invalid client API 0x%08X", ctxconfig->client);
            return GLFW_FALSE;
        }

        if (ctxconfig->share != null)
        {
            if (ctxconfig->client == GLFW_NO_API ||
                ctxconfig->share->context.client == GLFW_NO_API)
            {
                _glfwInputError(GLFW_NO_WINDOW_CONTEXT);
                return GLFW_FALSE;
            }

            if (ctxconfig->source != ctxconfig->share->context.source)
            {
                _glfwInputError(GLFW_INVALID_ENUM, "Context creation APIs do not match between contexts");
                return GLFW_FALSE;
            }
        }

        if (ctxconfig->client == GLFW_OPENGL_API)
        {
            if ((ctxconfig->major < 1 || ctxconfig->minor < 0) ||
                (ctxconfig->major == 1 && ctxconfig->minor > 5) ||
                (ctxconfig->major == 2 && ctxconfig->minor > 1) ||
                (ctxconfig->major == 3 && ctxconfig->minor > 3))
            {
                _glfwInputError(GLFW_INVALID_VALUE,
                    "Invalid OpenGL version {0}.{1}",
                    ctxconfig->major,
                    ctxconfig->minor);
                return GLFW_FALSE;
            }

            if (ctxconfig->profile != 0)
            {
                if (ctxconfig->profile != GLFW_OPENGL_CORE_PROFILE &&
                    ctxconfig->profile != GLFW_OPENGL_COMPAT_PROFILE)
                {
                    _glfwInputError(GLFW_INVALID_ENUM, "Invalid OpenGL profile 0x%08X", ctxconfig->profile);
                    return GLFW_FALSE;
                }

                if (ctxconfig->major <= 2 ||
                    (ctxconfig->major == 3 && ctxconfig->minor < 2))
                {
                    _glfwInputError(GLFW_INVALID_VALUE,
                        "Context profiles are only defined for OpenGL version 3.2 and above");
                    return GLFW_FALSE;
                }
            }

            if (ctxconfig->forward != 0 && ctxconfig->major <= 2)
            {
                _glfwInputError(GLFW_INVALID_VALUE,
                    "Forward-compatibility is only defined for OpenGL version 3.0 and above");
                return GLFW_FALSE;
            }
        }
        else if (ctxconfig->client == GLFW_OPENGL_ES_API)
        {
            if (ctxconfig->major < 1 || ctxconfig->minor < 0 ||
                (ctxconfig->major == 1 && ctxconfig->minor > 1) ||
                (ctxconfig->major == 2 && ctxconfig->minor > 0))
            {
                _glfwInputError(GLFW_INVALID_VALUE,
                    "Invalid OpenGL ES version {0}.{1}",
                    ctxconfig->major,
                    ctxconfig->minor);
                return GLFW_FALSE;
            }
        }

        if (ctxconfig->robustness != 0)
        {
            if (ctxconfig->robustness != GLFW_NO_RESET_NOTIFICATION &&
                ctxconfig->robustness != GLFW_LOSE_CONTEXT_ON_RESET)
            {
                _glfwInputError(GLFW_INVALID_ENUM, "Invalid context robustness mode 0x%08X", ctxconfig->robustness);
                return GLFW_FALSE;
            }
        }

        if (ctxconfig->release != 0)
        {
            if (ctxconfig->release != GLFW_RELEASE_BEHAVIOR_NONE &&
                ctxconfig->release != GLFW_RELEASE_BEHAVIOR_FLUSH)
            {
                _glfwInputError(GLFW_INVALID_ENUM, "Invalid context release behavior 0x%08X", ctxconfig->release);
                return GLFW_FALSE;
            }
        }

        return GLFW_TRUE;
    }

    static _GLFWfbconfig* _glfwChooseFBConfig(_GLFWfbconfig* desired,
                                              _GLFWfbconfig* alternatives,
                                              uint count)
    {
        var leastMissing = GLFW_UINT_MAX;
        var leastColorDiff = GLFW_UINT_MAX;
        var leastExtraDiff = GLFW_UINT_MAX;
        _GLFWfbconfig* closest = null;

        for (uint i = 0; i < count; i++)
        {
            var current = alternatives + i;

            if (desired->stereo > 0 && current->stereo == 0)
                continue;

            uint missing = 0;

            if (desired->alphaBits > 0 && current->alphaBits == 0)
                missing++;

            if (desired->depthBits > 0 && current->depthBits == 0)
                missing++;

            if (desired->stencilBits > 0 && current->stencilBits == 0)
                missing++;

            if (desired->auxBuffers > 0 && current->auxBuffers < desired->auxBuffers)
                missing += (uint)(desired->auxBuffers - current->auxBuffers);

            if (desired->samples > 0 && current->samples == 0)
                missing++;

            if (desired->transparent != current->transparent)
                missing++;

            uint colorDiff = 0;

            if (desired->redBits != GLFW_DONT_CARE)
                colorDiff += (uint)((desired->redBits - current->redBits) * (desired->redBits - current->redBits));

            if (desired->greenBits != GLFW_DONT_CARE)
                colorDiff += (uint)((desired->greenBits - current->greenBits) * (desired->greenBits - current->greenBits));

            if (desired->blueBits != GLFW_DONT_CARE)
                colorDiff += (uint)((desired->blueBits - current->blueBits) * (desired->blueBits - current->blueBits));

            uint extraDiff = 0;

            if (desired->alphaBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->alphaBits - current->alphaBits) * (desired->alphaBits - current->alphaBits));

            if (desired->depthBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->depthBits - current->depthBits) * (desired->depthBits - current->depthBits));

            if (desired->stencilBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->stencilBits - current->stencilBits) * (desired->stencilBits - current->stencilBits));

            if (desired->accumRedBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->accumRedBits - current->accumRedBits) * (desired->accumRedBits - current->accumRedBits));

            if (desired->accumGreenBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->accumGreenBits - current->accumGreenBits) * (desired->accumGreenBits - current->accumGreenBits));

            if (desired->accumBlueBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->accumBlueBits - current->accumBlueBits) * (desired->accumBlueBits - current->accumBlueBits));

            if (desired->accumAlphaBits != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->accumAlphaBits - current->accumAlphaBits) * (desired->accumAlphaBits - current->accumAlphaBits));

            if (desired->samples != GLFW_DONT_CARE)
                extraDiff += (uint)((desired->samples - current->samples) * (desired->samples - current->samples));

            if (desired->sRGB != 0 && current->sRGB == 0)
                extraDiff++;

            if (missing < leastMissing)
                closest = current;
            else if (missing == leastMissing)
            {
                if (colorDiff < leastColorDiff ||
                    (colorDiff == leastColorDiff && extraDiff < leastExtraDiff))
                {
                    closest = current;
                }
            }

            if (current == closest)
            {
                leastMissing = missing;
                leastColorDiff = colorDiff;
                leastExtraDiff = extraDiff;
            }
        }

        return closest;
    }

    static int _glfwRefreshContextAttribs(_GLFWwindow* window, _GLFWctxconfig* ctxconfig)
    {
        var prefixes = new[]
        {
            "OpenGL ES-CM ",
            "OpenGL ES-CL ",
            "OpenGL ES "
        };

        window->context.source = ctxconfig->source;
        window->context.client = GLFW_OPENGL_API;

        _GLFWwindow* previous;
        fixed (_GLFWlibrary* glfw = &_glfw)
            previous = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);

        glfwMakeContextCurrent((GLFWwindow*)window);
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (_glfwPlatformGetTls(&glfw->contextSlot) != window)
                return GLFW_FALSE;
        }

        window->context.GetIntegerv =
            (delegate* unmanaged<uint, int*, void>)context_getProcAddress(window, "glGetIntegerv");
        window->context.GetString =
            (delegate* unmanaged<uint, byte*>)context_getProcAddress(window, "glGetString");
        window->context.Flush =
            (delegate* unmanaged<void>)context_getProcAddress(window, "glFlush");

        if (window->context.GetIntegerv == null ||
            window->context.GetString == null ||
            window->context.Flush == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Entry point retrieval is broken");
            glfwMakeContextCurrent((GLFWwindow*)previous);
            return GLFW_FALSE;
        }

        var versionPtr = window->context.GetString(GL_VERSION);
        if (versionPtr == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                ctxconfig->client == GLFW_OPENGL_API
                    ? "OpenGL version string retrieval is broken"
                    : "OpenGL ES version string retrieval is broken");
            glfwMakeContextCurrent((GLFWwindow*)previous);
            return GLFW_FALSE;
        }

        var version = Marshal.PtrToStringUTF8((nint)versionPtr) ?? string.Empty;
        foreach (var prefix in prefixes)
        {
            if (version.StartsWith(prefix, StringComparison.Ordinal))
            {
                version = version[prefix.Length..];
                window->context.client = GLFW_OPENGL_ES_API;
                break;
            }
        }

        if (context_parseVersion(version,
                &window->context.major,
                &window->context.minor,
                &window->context.revision) == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                window->context.client == GLFW_OPENGL_API
                    ? "No version found in OpenGL version string"
                    : "No version found in OpenGL ES version string");
            glfwMakeContextCurrent((GLFWwindow*)previous);
            return GLFW_FALSE;
        }

        if (window->context.major < ctxconfig->major ||
            (window->context.major == ctxconfig->major &&
             window->context.minor < ctxconfig->minor))
        {
            _glfwInputError(GLFW_VERSION_UNAVAILABLE,
                window->context.client == GLFW_OPENGL_API
                    ? "Requested OpenGL version {0}.{1}, got version {2}.{3}"
                    : "Requested OpenGL ES version {0}.{1}, got version {2}.{3}",
                ctxconfig->major,
                ctxconfig->minor,
                window->context.major,
                window->context.minor);
            glfwMakeContextCurrent((GLFWwindow*)previous);
            return GLFW_FALSE;
        }

        if (window->context.major >= 3)
        {
            window->context.GetStringi =
                (delegate* unmanaged<uint, uint, byte*>)context_getProcAddress(window, "glGetStringi");
            if (window->context.GetStringi == null)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Entry point retrieval is broken");
                glfwMakeContextCurrent((GLFWwindow*)previous);
                return GLFW_FALSE;
            }
        }

        if (window->context.client == GLFW_OPENGL_API)
        {
            if (window->context.major >= 3)
            {
                int flags;
                window->context.GetIntegerv(GL_CONTEXT_FLAGS, &flags);

                if ((flags & GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT) != 0)
                    window->context.forward = GLFW_TRUE;

                if ((flags & GL_CONTEXT_FLAG_DEBUG_BIT) != 0)
                    window->context.debug = GLFW_TRUE;
                else if (context_extensionSupported("GL_ARB_debug_output") != 0 && ctxconfig->debug != 0)
                    window->context.debug = GLFW_TRUE;

                if ((flags & GL_CONTEXT_FLAG_NO_ERROR_BIT_KHR) != 0)
                    window->context.noerror = GLFW_TRUE;
            }

            if (window->context.major >= 4 ||
                (window->context.major == 3 && window->context.minor >= 2))
            {
                int mask;
                window->context.GetIntegerv(GL_CONTEXT_PROFILE_MASK, &mask);

                if ((mask & GL_CONTEXT_COMPATIBILITY_PROFILE_BIT) != 0)
                    window->context.profile = GLFW_OPENGL_COMPAT_PROFILE;
                else if ((mask & GL_CONTEXT_CORE_PROFILE_BIT) != 0)
                    window->context.profile = GLFW_OPENGL_CORE_PROFILE;
                else if (context_extensionSupported("GL_ARB_compatibility") != 0)
                    window->context.profile = GLFW_OPENGL_COMPAT_PROFILE;
            }

            if (context_extensionSupported("GL_ARB_robustness") != 0)
            {
                int strategy;
                window->context.GetIntegerv(GL_RESET_NOTIFICATION_STRATEGY_ARB, &strategy);

                if (strategy == GL_LOSE_CONTEXT_ON_RESET_ARB)
                    window->context.robustness = GLFW_LOSE_CONTEXT_ON_RESET;
                else if (strategy == GL_NO_RESET_NOTIFICATION_ARB)
                    window->context.robustness = GLFW_NO_RESET_NOTIFICATION;
            }
        }
        else
        {
            if (context_extensionSupported("GL_EXT_robustness") != 0)
            {
                int strategy;
                window->context.GetIntegerv(GL_RESET_NOTIFICATION_STRATEGY_ARB, &strategy);

                if (strategy == GL_LOSE_CONTEXT_ON_RESET_ARB)
                    window->context.robustness = GLFW_LOSE_CONTEXT_ON_RESET;
                else if (strategy == GL_NO_RESET_NOTIFICATION_ARB)
                    window->context.robustness = GLFW_NO_RESET_NOTIFICATION;
            }
        }

        if (context_extensionSupported("GL_KHR_context_flush_control") != 0)
        {
            int behavior;
            window->context.GetIntegerv(GL_CONTEXT_RELEASE_BEHAVIOR, &behavior);

            if (behavior == GL_NONE)
                window->context.release = GLFW_RELEASE_BEHAVIOR_NONE;
            else if (behavior == GL_CONTEXT_RELEASE_BEHAVIOR_FLUSH)
                window->context.release = GLFW_RELEASE_BEHAVIOR_FLUSH;
        }

        var glClear = (delegate* unmanaged<uint, void>)context_getProcAddress(window, "glClear");
        if (glClear != null)
        {
            glClear(GL_COLOR_BUFFER_BIT);

            if (window->doublebuffer != 0 && window->context.swapBuffers != null)
                window->context.swapBuffers(window);
        }

        glfwMakeContextCurrent((GLFWwindow*)previous);
        return GLFW_TRUE;
    }

    static void* context_getProcAddress(_GLFWwindow* window, string procname)
    {
        var name = stackalloc byte[procname.Length + 1];
        for (var i = 0; i < procname.Length; i++)
            name[i] = (byte)procname[i];
        name[procname.Length] = 0;

        return window->context.getProcAddress != null ? window->context.getProcAddress(name) : null;
    }

    static int context_extensionSupported(string extension)
    {
        var name = stackalloc byte[extension.Length + 1];
        for (var i = 0; i < extension.Length; i++)
            name[i] = (byte)extension[i];
        name[extension.Length] = 0;

        return glfwExtensionSupported(name);
    }

    static int context_parseVersion(string version, int* major, int* minor, int* revision)
    {
        var index = 0;
        if (context_parseInt(version, ref index, out var parsedMajor) == 0)
            return GLFW_FALSE;

        var parsedMinor = 0;
        var parsedRevision = 0;

        if (index < version.Length && version[index] == '.')
        {
            index++;
            context_parseInt(version, ref index, out parsedMinor);
        }

        if (index < version.Length && version[index] == '.')
        {
            index++;
            context_parseInt(version, ref index, out parsedRevision);
        }

        *major = parsedMajor;
        *minor = parsedMinor;
        *revision = parsedRevision;
        return GLFW_TRUE;
    }

    static int context_parseInt(string value, ref int index, out int result)
    {
        result = 0;
        var start = index;

        while (index < value.Length && value[index] >= '0' && value[index] <= '9')
        {
            result = result * 10 + value[index] - '0';
            index++;
        }

        return index != start ? GLFW_TRUE : GLFW_FALSE;
    }

    static int context_stringEquals(byte* value, byte* expected)
    {
        while (*value != 0 && *expected != 0)
        {
            if (*value != *expected)
                return GLFW_FALSE;

            value++;
            expected++;
        }

        return *value == *expected ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwStringInExtensionString(byte* @string, byte* extensions)
    {
        var name = Marshal.PtrToStringUTF8((nint)@string);
        var list = Marshal.PtrToStringUTF8((nint)extensions);
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(list))
            return GLFW_FALSE;

        foreach (var extension in list.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (extension == name)
                return GLFW_TRUE;
        }

        return GLFW_FALSE;
    }

    public static void glfwMakeContextCurrent(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow != null && internalWindow->context.client == GLFW_NO_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT,
                "Cannot make current with a window that has no OpenGL or OpenGL ES context");
            return;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            var previous = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
            if (previous != null && (internalWindow == null || internalWindow->context.source != previous->context.source))
            {
                if (previous->context.makeCurrent != null)
                    previous->context.makeCurrent(null);
                else
                    _glfwPlatformSetTls(&glfw->contextSlot, null);
            }

            if (internalWindow != null)
            {
                if (internalWindow->context.makeCurrent != null)
                    internalWindow->context.makeCurrent(internalWindow);
                else
                    _glfwPlatformSetTls(&glfw->contextSlot, internalWindow);
            }
            else
                _glfwPlatformSetTls(&glfw->contextSlot, null);
        }
    }

    public static GLFWwindow* glfwGetCurrentContext()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
            return (GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);
    }

    public static void glfwSwapBuffers(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        var internalWindow = (_GLFWwindow*)window;
        if (internalWindow->context.client == GLFW_NO_API)
        {
            _glfwInputError(GLFW_NO_WINDOW_CONTEXT,
                "Cannot swap buffers of a window that has no OpenGL or OpenGL ES context");
            return;
        }

        if (internalWindow->context.swapBuffers != null)
            internalWindow->context.swapBuffers(internalWindow);
        else
            _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED, "Context buffer swapping has not yet been ported");
    }

    public static void glfwSwapInterval(int interval)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return;
        }

        _GLFWwindow* window;
        fixed (_GLFWlibrary* glfw = &_glfw)
            window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);

        if (window == null)
        {
            _glfwInputError(GLFW_NO_CURRENT_CONTEXT,
                "Cannot set swap interval without a current OpenGL or OpenGL ES context");
            return;
        }

        if (window->context.swapInterval != null)
            window->context.swapInterval(interval);
        else
            _glfwInputError(GLFW_FEATURE_UNIMPLEMENTED, "Context swap interval has not yet been ported");
    }

    public static int glfwExtensionSupported(byte* extension)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return GLFW_FALSE;
        }

        _GLFWwindow* window;
        fixed (_GLFWlibrary* glfw = &_glfw)
            window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);

        if (window == null)
        {
            _glfwInputError(GLFW_NO_CURRENT_CONTEXT,
                "Cannot query extension without a current OpenGL or OpenGL ES context");
            return GLFW_FALSE;
        }

        if (extension == null || *extension == 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Extension name cannot be an empty string");
            return GLFW_FALSE;
        }

        if (window->context.major >= 3)
        {
            int count;
            window->context.GetIntegerv(GL_NUM_EXTENSIONS, &count);

            for (var i = 0; i < count; i++)
            {
                var name = window->context.GetStringi(GL_EXTENSIONS, (uint)i);
                if (name == null)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Extension string retrieval is broken");
                    return GLFW_FALSE;
                }

                if (context_stringEquals(name, extension) != 0)
                    return GLFW_TRUE;
            }
        }
        else
        {
            var extensions = window->context.GetString(GL_EXTENSIONS);
            if (extensions == null)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Extension string retrieval is broken");
                return GLFW_FALSE;
            }

            if (_glfwStringInExtensionString(extension, extensions) != 0)
                return GLFW_TRUE;
        }

        return window->context.extensionSupported != null && window->context.extensionSupported(extension) != 0
            ? GLFW_TRUE
            : GLFW_FALSE;
    }

    public static void* glfwGetProcAddress(byte* procname)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        _GLFWwindow* window;
        fixed (_GLFWlibrary* glfw = &_glfw)
            window = (_GLFWwindow*)_glfwPlatformGetTls(&glfw->contextSlot);

        if (window == null)
        {
            _glfwInputError(GLFW_NO_CURRENT_CONTEXT,
                "Cannot query entry point without a current OpenGL or OpenGL ES context");
            return null;
        }

        if (procname == null || *procname == 0)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Entry point name cannot be an empty string");
            return null;
        }

        return window->context.getProcAddress != null
            ? window->context.getProcAddress(procname)
            : null;
    }
}
