namespace NGlfw;

public static unsafe partial class Glfw
{
    const int GLFW_NULL_SC_FIRST = GLFW_NULL_SC_SPACE;
    const int GLFW_NULL_SC_SPACE = 1;
    const int GLFW_NULL_SC_LAST = GLFW_NULL_SC_MENU;
    const int GLFW_NULL_SC_MENU = 120;

    public struct _GLFWwindowNull
    {
        public int xpos;
        public int ypos;
        public int width;
        public int height;
        public int visible;
        public int iconified;
        public int maximized;
        public int resizable;
        public int decorated;
        public int floating;
        public int transparent;
        public float opacity;
    }

    public struct _GLFWmonitorNull
    {
        public GLFWgammaramp ramp;
    }

    public struct _GLFWlibraryNull
    {
        public int xcursor;
        public int ycursor;
        public byte* clipboardString;
        public _GLFWwindow* focusedWindow;
        public fixed ushort keycodes[GLFW_NULL_SC_LAST + 1];
        public fixed byte scancodes[GLFW_KEY_LAST + 1];
    }
}
