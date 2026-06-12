using System.Text;
using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    [StructLayout(LayoutKind.Sequential)]
    struct VkXlibSurfaceCreateInfoKHR
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* dpy;
        public nuint window;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct VkXcbSurfaceCreateInfoKHR
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* connection;
        public uint window;
    }

    static void* x11_loadModule(string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* path = bytes)
            return _glfwPlatformLoadModule(path);
    }

    static void* x11_getModuleSymbol(void* module, string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* symbol = bytes)
            return _glfwPlatformGetModuleSymbol(module, symbol);
    }
}
