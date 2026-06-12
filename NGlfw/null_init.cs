using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwConnectNull(int platformID, _GLFWplatform* platform)
    {
        *platform = default;
        platform->platformID = GLFW_PLATFORM_NULL;
        platform->init = &_glfwInitNull;
        platform->terminate = &_glfwTerminateNull;
        platform->getCursorPos = &_glfwGetCursorPosNull;
        platform->setCursorPos = &_glfwSetCursorPosNull;
        platform->setCursorMode = &_glfwSetCursorModeNull;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionNull;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedNull;
        platform->createCursor = &_glfwCreateCursorNull;
        platform->createStandardCursor = &_glfwCreateStandardCursorNull;
        platform->destroyCursor = &_glfwDestroyCursorNull;
        platform->setCursor = &_glfwSetCursorNull;
        platform->getScancodeName = &_glfwGetScancodeNameNull;
        platform->getKeyScancode = &_glfwGetKeyScancodeNull;
        platform->setClipboardString = &_glfwSetClipboardStringNull;
        platform->getClipboardString = &_glfwGetClipboardStringNull;
        platform->initJoysticks = &_glfwInitJoysticksNull;
        platform->terminateJoysticks = &_glfwTerminateJoysticksNull;
        platform->pollJoystick = &_glfwPollJoystickNull;
        platform->getMappingName = &_glfwGetMappingNameNull;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDNull;
        platform->freeMonitor = &_glfwFreeMonitorNull;
        platform->getMonitorPos = &_glfwGetMonitorPosNull;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleNull;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaNull;
        platform->getVideoModes = &_glfwGetVideoModesNull;
        platform->getVideoMode = &_glfwGetVideoModeNull;
        platform->getGammaRamp = &_glfwGetGammaRampNull;
        platform->setGammaRamp = &_glfwSetGammaRampNull;
        platform->createWindow = &_glfwCreateWindowNull;
        platform->destroyWindow = &_glfwDestroyWindowNull;
        platform->setWindowTitle = &_glfwSetWindowTitleNull;
        platform->setWindowIcon = &_glfwSetWindowIconNull;
        platform->getWindowPos = &_glfwGetWindowPosNull;
        platform->setWindowPos = &_glfwSetWindowPosNull;
        platform->getWindowSize = &_glfwGetWindowSizeNull;
        platform->setWindowSize = &_glfwSetWindowSizeNull;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsNull;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioNull;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeNull;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeNull;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleNull;
        platform->iconifyWindow = &_glfwIconifyWindowNull;
        platform->restoreWindow = &_glfwRestoreWindowNull;
        platform->maximizeWindow = &_glfwMaximizeWindowNull;
        platform->showWindow = &_glfwShowWindowNull;
        platform->hideWindow = &_glfwHideWindowNull;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionNull;
        platform->focusWindow = &_glfwFocusWindowNull;
        platform->setWindowMonitor = &_glfwSetWindowMonitorNull;
        platform->windowFocused = &_glfwWindowFocusedNull;
        platform->windowIconified = &_glfwWindowIconifiedNull;
        platform->windowVisible = &_glfwWindowVisibleNull;
        platform->windowMaximized = &_glfwWindowMaximizedNull;
        platform->windowHovered = &_glfwWindowHoveredNull;
        platform->framebufferTransparent = &_glfwFramebufferTransparentNull;
        platform->getWindowOpacity = &_glfwGetWindowOpacityNull;
        platform->setWindowResizable = &_glfwSetWindowResizableNull;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedNull;
        platform->setWindowFloating = &_glfwSetWindowFloatingNull;
        platform->setWindowOpacity = &_glfwSetWindowOpacityNull;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughNull;
        platform->pollEvents = &_glfwPollEventsNull;
        platform->waitEvents = &_glfwWaitEventsNull;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutNull;
        platform->postEmptyEvent = &_glfwPostEmptyEventNull;
        platform->getEGLPlatform = &_glfwGetEGLPlatformNull;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayNull;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowNull;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsNull;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportNull;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceNull;
        return GLFW_TRUE;
    }

    static int _glfwInitNull()
    {
        fixed (ushort* keycodes = _glfw.@null.keycodes)
            _glfw_memset(keycodes, 0xff, (nuint)((GLFW_NULL_SC_LAST + 1) * sizeof(ushort)));
        fixed (byte* scancodes = _glfw.@null.scancodes)
            _glfw_memset(scancodes, 0xff, GLFW_KEY_LAST + 1);

        _glfwPollMonitorsNull();
        return GLFW_TRUE;
    }

    static void _glfwTerminateNull()
    {
        _glfw_free(_glfw.@null.clipboardString);
        _glfwTerminateOSMesa();
        _glfwTerminateEGL();
    }
}
