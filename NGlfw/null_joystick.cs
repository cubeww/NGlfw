namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwInitJoysticksNull()
    {
        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksNull()
    {
    }

    static int _glfwPollJoystickNull(_GLFWjoystick* js, int mode)
    {
        return GLFW_FALSE;
    }

    static byte* _glfwGetMappingNameNull()
    {
        return null;
    }

    static void _glfwUpdateGamepadGUIDNull(byte* guid)
    {
    }
}
