using System;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static void* defaultAllocate(nuint size, void* user)
    {
        return NativeMemory.Alloc(size);
    }

    static void defaultDeallocate(void* block, void* user)
    {
        NativeMemory.Free(block);
    }

    static void* defaultReallocate(void* block, nuint size, void* user)
    {
        return NativeMemory.Realloc(block, size);
    }

    static void terminate()
    {
        _glfw.callbacks = default;

        while (_glfw.windowListHead != null)
            glfwDestroyWindow((GLFWwindow*)_glfw.windowListHead);

        while (_glfw.cursorListHead != null)
            glfwDestroyCursor((GLFWcursor*)_glfw.cursorListHead);

        for (var i = 0; i < _glfw.monitorCount; i++)
        {
            var monitor = _glfw.monitors[i];
            if (monitor->originalRamp.size != 0 && _glfw.platform.setGammaRamp != null)
                _glfw.platform.setGammaRamp(monitor, &monitor->originalRamp);
            _glfwFreeMonitor(monitor);
        }

        _glfw_free(_glfw.monitors);
        _glfw.monitors = null;
        _glfw.monitorCount = 0;

        _glfw_free(_glfw.mappings);
        _glfw.mappings = null;
        _glfw.mappingCount = 0;

        _glfwTerminateVulkan();
        if (_glfw.platform.terminateJoysticks != null)
            _glfw.platform.terminateJoysticks();
        if (_glfw.platform.terminate != null)
            _glfw.platform.terminate();

        _glfw.initialized = GLFW_FALSE;

        while (_glfw.errorListHead != null)
        {
            var error = _glfw.errorListHead;
            _glfw.errorListHead = error->next;
            _glfw_free(error);
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            _glfwPlatformDestroyTls(&glfw->contextSlot);
            _glfwPlatformDestroyTls(&glfw->errorSlot);
            _glfwPlatformDestroyMutex(&glfw->errorLock);
        }

        _glfw = default;
    }

    static nuint _glfwEncodeUTF8(byte* s, uint codepoint)
    {
        nuint count = 0;

        if (codepoint < 0x80)
            s[count++] = (byte)codepoint;
        else if (codepoint < 0x800)
        {
            s[count++] = (byte)((codepoint >> 6) | 0xc0);
            s[count++] = (byte)((codepoint & 0x3f) | 0x80);
        }
        else if (codepoint < 0x10000)
        {
            s[count++] = (byte)((codepoint >> 12) | 0xe0);
            s[count++] = (byte)(((codepoint >> 6) & 0x3f) | 0x80);
            s[count++] = (byte)((codepoint & 0x3f) | 0x80);
        }
        else if (codepoint < 0x110000)
        {
            s[count++] = (byte)((codepoint >> 18) | 0xf0);
            s[count++] = (byte)(((codepoint >> 12) & 0x3f) | 0x80);
            s[count++] = (byte)(((codepoint >> 6) & 0x3f) | 0x80);
            s[count++] = (byte)((codepoint & 0x3f) | 0x80);
        }

        return count;
    }

    static int _glfwHexDigit(byte c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        if (c >= 'a' && c <= 'f')
            return c - 'a' + 10;
        if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;
        return 0;
    }

    static byte** _glfwParseUriList(byte* text, int* count)
    {
        var prefix = stackalloc byte[8] { (byte)'f', (byte)'i', (byte)'l', (byte)'e', (byte)':', (byte)'/', (byte)'/', 0 };
        byte** paths = null;
        *count = 0;

        while (text != null && *text != 0)
        {
            var line = text;

            while (*text != 0 && *text != '\r' && *text != '\n')
                text++;

            if (*text != 0)
            {
                *text++ = 0;
                if (*(text - 1) == '\r' && *text == '\n')
                    *text++ = 0;
            }

            while (*text == '\r' || *text == '\n')
                *text++ = 0;

            if (*line == 0 || *line == '#')
                continue;

            if (_glfw_strncmp(line, prefix, 7) == 0)
            {
                line += 7;
                while (*line != 0 && *line != '/')
                    line++;

                if (*line == 0)
                    continue;
            }

            var newPaths = (byte**)_glfw_realloc(paths, (nuint)((*count + 1) * sizeof(byte*)));
            if (newPaths == null)
                return paths;

            paths = newPaths;
            var path = (byte*)_glfw_calloc((nuint)_glfw_strlen(line) + 1, 1);
            if (path == null)
                return paths;

            paths[*count] = path;
            (*count)++;

            while (*line != 0)
            {
                if (line[0] == '%' && line[1] != 0 && line[2] != 0)
                {
                    *path = (byte)((_glfwHexDigit(line[1]) << 4) | _glfwHexDigit(line[2]));
                    line += 2;
                }
                else
                {
                    *path = *line;
                }

                path++;
                line++;
            }
        }

        return paths;
    }

    static byte* _glfw_strdup(byte* source)
    {
        var length = _glfw_strlen(source);
        var result = (byte*)_glfw_calloc((nuint)length + 1, 1);
        _glfw_strcpy(result, source);
        return result;
    }

    static int _glfw_min(int a, int b)
    {
        return a < b ? a : b;
    }

    static int _glfw_max(int a, int b)
    {
        return a > b ? a : b;
    }

    static void* _glfw_calloc(nuint count, nuint size)
    {
        if (count != 0 && size != 0)
        {
            if (count > nuint.MaxValue / size)
            {
                _glfwInputError(GLFW_INVALID_VALUE, "Allocation size overflow");
                return null;
            }

            var block = _glfw.allocator.allocate(count * size, _glfw.allocator.user);
            if (block != null)
            {
                _glfw_memset(block, 0, count * size);
                return block;
            }

            _glfwInputError(GLFW_OUT_OF_MEMORY);
            return null;
        }

        return null;
    }

    static void* _glfw_realloc(void* block, nuint size)
    {
        if (block != null && size != 0)
        {
            var resized = _glfw.allocator.reallocate(block, size, _glfw.allocator.user);
            if (resized != null)
                return resized;

            _glfwInputError(GLFW_OUT_OF_MEMORY);
            return null;
        }

        if (block != null)
        {
            _glfw_free(block);
            return null;
        }

        return _glfw_calloc(1, size);
    }

    static void _glfw_free(void* block)
    {
        if (block != null)
            _glfw.allocator.deallocate(block, _glfw.allocator.user);
    }

    static void _glfwInputError(int code, string? format = null, params object[] args)
    {
        var description = stackalloc byte[_GLFW_MESSAGE_SIZE];

        if (format != null)
            _glfw_strcpy(description, args.Length == 0 ? format : string.Format(format.Replace("%08X", "{0:X8}"), args));
        else
            _glfw_strcpy(description, _glfwDefaultErrorString(code));

        if (_glfw.initialized != 0)
        {
            _GLFWerror* error;
            fixed (_GLFWlibrary* glfw = &_glfw)
                error = (_GLFWerror*)_glfwPlatformGetTls(&glfw->errorSlot);
            if (error == null)
            {
                error = (_GLFWerror*)_glfw_calloc(1, (nuint)sizeof(_GLFWerror));
                fixed (_GLFWlibrary* glfw = &_glfw)
                {
                    _glfwPlatformSetTls(&glfw->errorSlot, error);
                    _glfwPlatformLockMutex(&glfw->errorLock);
                }
                error->next = _glfw.errorListHead;
                _glfw.errorListHead = error;
                fixed (_GLFWlibrary* glfw = &_glfw)
                    _glfwPlatformUnlockMutex(&glfw->errorLock);
            }

            error->code = code;
            _glfw_strcpy(error->description, description);
        }
        else
        {
            fixed (_GLFWerror* error = &_glfwMainThreadError)
            {
                error->code = code;
                _glfw_strcpy(error->description, description);
            }
        }

        if (_glfwErrorCallback != null)
            _glfwErrorCallback(code, description);
    }

    static string _glfwDefaultErrorString(int code)
    {
        return code switch
        {
            GLFW_NOT_INITIALIZED => "The GLFW library is not initialized",
            GLFW_NO_CURRENT_CONTEXT => "There is no current context",
            GLFW_INVALID_ENUM => "Invalid argument for enum parameter",
            GLFW_INVALID_VALUE => "Invalid value for parameter",
            GLFW_OUT_OF_MEMORY => "Out of memory",
            GLFW_API_UNAVAILABLE => "The requested API is unavailable",
            GLFW_VERSION_UNAVAILABLE => "The requested API version is unavailable",
            GLFW_PLATFORM_ERROR => "A platform-specific error occurred",
            GLFW_FORMAT_UNAVAILABLE => "The requested format is unavailable",
            GLFW_NO_WINDOW_CONTEXT => "The specified window has no context",
            GLFW_CURSOR_UNAVAILABLE => "The specified cursor shape is unavailable",
            GLFW_FEATURE_UNAVAILABLE => "The requested feature cannot be implemented for this platform",
            GLFW_FEATURE_UNIMPLEMENTED => "The requested feature has not yet been implemented for this platform",
            GLFW_PLATFORM_UNAVAILABLE => "The requested platform is unavailable",
            _ => "ERROR: UNKNOWN GLFW ERROR"
        };
    }

    public static int glfwInit()
    {
        if (_glfw.initialized != 0)
            return GLFW_TRUE;

        _glfw = default;
        _glfw.hints.init = _glfwInitHints;

        _glfw.allocator = _glfwInitAllocator;
        if (_glfw.allocator.allocate == null)
        {
            _glfw.allocator.allocate = &defaultAllocate;
            _glfw.allocator.reallocate = &defaultReallocate;
            _glfw.allocator.deallocate = &defaultDeallocate;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (_glfwSelectPlatform(glfw->hints.init.platformID, &glfw->platform) == 0)
                return GLFW_FALSE;
        }

        if (_glfw.platform.init() == 0)
        {
            terminate();
            return GLFW_FALSE;
        }

        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            if (_glfwPlatformCreateMutex(&glfw->errorLock) == 0 ||
                _glfwPlatformCreateTls(&glfw->errorSlot) == 0 ||
                _glfwPlatformCreateTls(&glfw->contextSlot) == 0)
            {
                terminate();
                return GLFW_FALSE;
            }
        }

        fixed (_GLFWerror* error = &_glfwMainThreadError)
        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfwPlatformSetTls(&glfw->errorSlot, error);

        _glfwInitGamepadMappings();

        _glfwPlatformInitTimer();
        _glfw.timer.offset = _glfwPlatformGetTimerValue();

        _glfw.initialized = GLFW_TRUE;

        glfwDefaultWindowHints();
        return GLFW_TRUE;
    }

    public static void glfwTerminate()
    {
        if (_glfw.initialized == 0)
            return;

        terminate();
    }

    public static void glfwInitHint(int hint, int value)
    {
        switch (hint)
        {
            case GLFW_JOYSTICK_HAT_BUTTONS:
                _glfwInitHints.hatButtons = value;
                return;
            case GLFW_ANGLE_PLATFORM_TYPE:
                _glfwInitHints.angleType = value;
                return;
            case GLFW_PLATFORM:
                _glfwInitHints.platformID = value;
                return;
            case GLFW_COCOA_CHDIR_RESOURCES:
                _glfwInitHints.ns.chdir = value;
                return;
            case GLFW_COCOA_MENUBAR:
                _glfwInitHints.ns.menubar = value;
                return;
            case GLFW_X11_XCB_VULKAN_SURFACE:
                _glfwInitHints.x11.xcbVulkanSurface = value;
                return;
            case GLFW_WAYLAND_LIBDECOR:
                _glfwInitHints.wl.libdecorMode = value;
                return;
        }

        _glfwInputError(GLFW_INVALID_ENUM, "Invalid init hint 0x%08X", hint);
    }

    public static void glfwInitAllocator(GLFWallocator* allocator)
    {
        if (allocator != null)
        {
            if (allocator->allocate != null &&
                allocator->reallocate != null &&
                allocator->deallocate != null)
                _glfwInitAllocator = *allocator;
            else
                _glfwInputError(GLFW_INVALID_VALUE, "Missing function in allocator");
        }
        else
            _glfwInitAllocator = default;
    }

    public static void glfwInitVulkanLoader(delegate*<void*, byte*, void*> loader)
    {
        _glfwInitHints.vulkanLoader = loader;
        _glfwInitHints.vulkanLoaderNative = null;
    }

    public static void glfwInitVulkanLoader(delegate* unmanaged<void*, byte*, void*> loader)
    {
        _glfwInitHints.vulkanLoader = null;
        _glfwInitHints.vulkanLoaderNative = loader;
    }

    public static void glfwGetVersion(int* major, int* minor, int* rev)
    {
        if (major != null)
            *major = GLFW_VERSION_MAJOR;
        if (minor != null)
            *minor = GLFW_VERSION_MINOR;
        if (rev != null)
            *rev = GLFW_VERSION_REVISION;
    }

    public static int glfwGetError(byte** description)
    {
        _GLFWerror* error;
        var code = GLFW_NO_ERROR;

        if (description != null)
            *description = null;

        if (_glfw.initialized != 0)
        {
            fixed (_GLFWlibrary* glfw = &_glfw)
                error = (_GLFWerror*)_glfwPlatformGetTls(&glfw->errorSlot);
        }
        else
        {
            fixed (_GLFWerror* mainThreadError = &_glfwMainThreadError)
                error = mainThreadError;
        }

        if (error != null)
        {
            code = error->code;
            error->code = GLFW_NO_ERROR;
            if (description != null && code != 0)
            {
                *description = error->description;
            }
        }

        return code;
    }

    public static delegate*<int, byte*, void> glfwSetErrorCallback(delegate*<int, byte*, void> cbfun)
    {
        var previous = _glfwErrorCallback;
        _glfwErrorCallback = cbfun;
        return previous;
    }
}
