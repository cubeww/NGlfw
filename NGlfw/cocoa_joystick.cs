namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwInitJoysticksCocoa()
    {
        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksCocoa()
    {
    }

    static int _glfwPollJoystickCocoa(_GLFWjoystick* js, int mode)
    {
        return js->connected;
    }

    static byte* _glfwGetMappingNameCocoa()
    {
        return _glfwCocoaMappingName;
    }

    static void _glfwUpdateGamepadGUIDCocoa(byte* guid)
    {
    }
}
