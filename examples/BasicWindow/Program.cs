using System.Text;
using static NGlfw.Glfw;

unsafe
{
    if (glfwInit() == GLFW_FALSE)
    {
        Console.Error.WriteLine("glfwInit failed.");
        return 1;
    }

    try
    {
        var title = Encoding.UTF8.GetBytes("NGlfw Basic Window\0");

        fixed (byte* titlePtr = title)
        {
            var window = glfwCreateWindow(800, 600, titlePtr, null, null);
            if (window == null)
            {
                Console.Error.WriteLine("glfwCreateWindow failed.");
                return 1;
            }

            glfwMakeContextCurrent(window);
            glfwSwapInterval(1);

            while (glfwWindowShouldClose(window) == GLFW_FALSE)
            {
                glfwSwapBuffers(window);
                glfwPollEvents();
            }

            glfwDestroyWindow(window);
        }
    }
    finally
    {
        glfwTerminate();
    }

    return 0;
}
