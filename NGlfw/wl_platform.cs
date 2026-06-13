using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static void* wayland_loadModule(string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* path = bytes)
            return _glfwPlatformLoadModule(path);
    }

    static void* wayland_getModuleSymbol(void* module, string name)
    {
        var bytes = Encoding.ASCII.GetBytes(name + '\0');
        fixed (byte* symbol = bytes)
            return _glfwPlatformGetModuleSymbol(module, symbol);
    }

    static void wayland_setPlatformCallbacks(_GLFWplatform* platform)
    {
        *platform = default;
        platform->platformID = GLFW_PLATFORM_WAYLAND;
        platform->init = &_glfwInitWayland;
        platform->terminate = &_glfwTerminateWayland;
        platform->getCursorPos = &_glfwGetCursorPosWayland;
        platform->setCursorPos = &_glfwSetCursorPosWayland;
        platform->setCursorMode = &_glfwSetCursorModeWayland;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionWayland;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedWayland;
        platform->createCursor = &_glfwCreateCursorWayland;
        platform->createStandardCursor = &_glfwCreateStandardCursorWayland;
        platform->destroyCursor = &_glfwDestroyCursorWayland;
        platform->setCursor = &_glfwSetCursorWayland;
        platform->getScancodeName = &_glfwGetScancodeNameWayland;
        platform->getKeyScancode = &_glfwGetKeyScancodeWayland;
        platform->setClipboardString = &_glfwSetClipboardStringWayland;
        platform->getClipboardString = &_glfwGetClipboardStringWayland;
        platform->initJoysticks = &_glfwInitJoysticksLinux;
        platform->terminateJoysticks = &_glfwTerminateJoysticksLinux;
        platform->pollJoystick = &_glfwPollJoystickLinux;
        platform->getMappingName = &_glfwGetMappingNameLinux;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDLinux;
        platform->freeMonitor = &_glfwFreeMonitorWayland;
        platform->getMonitorPos = &_glfwGetMonitorPosWayland;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleWayland;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaWayland;
        platform->getVideoModes = &_glfwGetVideoModesWayland;
        platform->getVideoMode = &_glfwGetVideoModeWayland;
        platform->getGammaRamp = &_glfwGetGammaRampWayland;
        platform->setGammaRamp = &_glfwSetGammaRampWayland;
        platform->createWindow = &_glfwCreateWindowWayland;
        platform->destroyWindow = &_glfwDestroyWindowWayland;
        platform->setWindowTitle = &_glfwSetWindowTitleWayland;
        platform->setWindowIcon = &_glfwSetWindowIconWayland;
        platform->getWindowPos = &_glfwGetWindowPosWayland;
        platform->setWindowPos = &_glfwSetWindowPosWayland;
        platform->getWindowSize = &_glfwGetWindowSizeWayland;
        platform->setWindowSize = &_glfwSetWindowSizeWayland;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsWayland;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioWayland;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeWayland;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeWayland;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleWayland;
        platform->iconifyWindow = &_glfwIconifyWindowWayland;
        platform->restoreWindow = &_glfwRestoreWindowWayland;
        platform->maximizeWindow = &_glfwMaximizeWindowWayland;
        platform->showWindow = &_glfwShowWindowWayland;
        platform->hideWindow = &_glfwHideWindowWayland;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionWayland;
        platform->focusWindow = &_glfwFocusWindowWayland;
        platform->setWindowMonitor = &_glfwSetWindowMonitorWayland;
        platform->windowFocused = &_glfwWindowFocusedWayland;
        platform->windowIconified = &_glfwWindowIconifiedWayland;
        platform->windowVisible = &_glfwWindowVisibleWayland;
        platform->windowMaximized = &_glfwWindowMaximizedWayland;
        platform->windowHovered = &_glfwWindowHoveredWayland;
        platform->framebufferTransparent = &_glfwFramebufferTransparentWayland;
        platform->getWindowOpacity = &_glfwGetWindowOpacityWayland;
        platform->setWindowResizable = &_glfwSetWindowResizableWayland;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedWayland;
        platform->setWindowFloating = &_glfwSetWindowFloatingWayland;
        platform->setWindowOpacity = &_glfwSetWindowOpacityWayland;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughWayland;
        platform->pollEvents = &_glfwPollEventsWayland;
        platform->waitEvents = &_glfwWaitEventsWayland;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutWayland;
        platform->postEmptyEvent = &_glfwPostEmptyEventWayland;
        platform->getEGLPlatform = &_glfwGetEGLPlatformWayland;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayWayland;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowWayland;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsWayland;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportWayland;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceWayland;
    }
}
