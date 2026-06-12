namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint GLFW_INVALID_CODEPOINT = 0xffffffffu;

    static uint _glfwKeySym2Unicode(uint keysym)
    {
        if ((keysym >= 0x0020 && keysym <= 0x007e) ||
            (keysym >= 0x00a0 && keysym <= 0x00ff))
            return keysym;

        if ((keysym & 0xff000000) == 0x01000000)
            return keysym & 0x00ffffff;

        return GLFW_INVALID_CODEPOINT;
    }
}
