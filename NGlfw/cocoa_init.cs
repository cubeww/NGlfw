using System.Runtime.InteropServices;

namespace NGlfw;

public static unsafe partial class Glfw
{
    static int _glfwConnectCocoa(int platformID, _GLFWplatform* platform)
    {
        *platform = default;
        platform->platformID = GLFW_PLATFORM_COCOA;
        platform->init = &_glfwInitCocoa;
        platform->terminate = &_glfwTerminateCocoa;
        platform->getCursorPos = &_glfwGetCursorPosCocoa;
        platform->setCursorPos = &_glfwSetCursorPosCocoa;
        platform->setCursorMode = &_glfwSetCursorModeCocoa;
        platform->setRawMouseMotion = &_glfwSetRawMouseMotionCocoa;
        platform->rawMouseMotionSupported = &_glfwRawMouseMotionSupportedCocoa;
        platform->createCursor = &_glfwCreateCursorCocoa;
        platform->createStandardCursor = &_glfwCreateStandardCursorCocoa;
        platform->destroyCursor = &_glfwDestroyCursorCocoa;
        platform->setCursor = &_glfwSetCursorCocoa;
        platform->getScancodeName = &_glfwGetScancodeNameCocoa;
        platform->getKeyScancode = &_glfwGetKeyScancodeCocoa;
        platform->setClipboardString = &_glfwSetClipboardStringCocoa;
        platform->getClipboardString = &_glfwGetClipboardStringCocoa;
        platform->initJoysticks = &_glfwInitJoysticksCocoa;
        platform->terminateJoysticks = &_glfwTerminateJoysticksCocoa;
        platform->pollJoystick = &_glfwPollJoystickCocoa;
        platform->getMappingName = &_glfwGetMappingNameCocoa;
        platform->updateGamepadGUID = &_glfwUpdateGamepadGUIDCocoa;
        platform->freeMonitor = &_glfwFreeMonitorCocoa;
        platform->getMonitorPos = &_glfwGetMonitorPosCocoa;
        platform->getMonitorContentScale = &_glfwGetMonitorContentScaleCocoa;
        platform->getMonitorWorkarea = &_glfwGetMonitorWorkareaCocoa;
        platform->getVideoModes = &_glfwGetVideoModesCocoa;
        platform->getVideoMode = &_glfwGetVideoModeCocoa;
        platform->getGammaRamp = &_glfwGetGammaRampCocoa;
        platform->setGammaRamp = &_glfwSetGammaRampCocoa;
        platform->createWindow = &_glfwCreateWindowCocoa;
        platform->destroyWindow = &_glfwDestroyWindowCocoa;
        platform->setWindowTitle = &_glfwSetWindowTitleCocoa;
        platform->setWindowIcon = &_glfwSetWindowIconCocoa;
        platform->getWindowPos = &_glfwGetWindowPosCocoa;
        platform->setWindowPos = &_glfwSetWindowPosCocoa;
        platform->getWindowSize = &_glfwGetWindowSizeCocoa;
        platform->setWindowSize = &_glfwSetWindowSizeCocoa;
        platform->setWindowSizeLimits = &_glfwSetWindowSizeLimitsCocoa;
        platform->setWindowAspectRatio = &_glfwSetWindowAspectRatioCocoa;
        platform->getFramebufferSize = &_glfwGetFramebufferSizeCocoa;
        platform->getWindowFrameSize = &_glfwGetWindowFrameSizeCocoa;
        platform->getWindowContentScale = &_glfwGetWindowContentScaleCocoa;
        platform->iconifyWindow = &_glfwIconifyWindowCocoa;
        platform->restoreWindow = &_glfwRestoreWindowCocoa;
        platform->maximizeWindow = &_glfwMaximizeWindowCocoa;
        platform->showWindow = &_glfwShowWindowCocoa;
        platform->hideWindow = &_glfwHideWindowCocoa;
        platform->requestWindowAttention = &_glfwRequestWindowAttentionCocoa;
        platform->focusWindow = &_glfwFocusWindowCocoa;
        platform->setWindowMonitor = &_glfwSetWindowMonitorCocoa;
        platform->windowFocused = &_glfwWindowFocusedCocoa;
        platform->windowIconified = &_glfwWindowIconifiedCocoa;
        platform->windowVisible = &_glfwWindowVisibleCocoa;
        platform->windowMaximized = &_glfwWindowMaximizedCocoa;
        platform->windowHovered = &_glfwWindowHoveredCocoa;
        platform->framebufferTransparent = &_glfwFramebufferTransparentCocoa;
        platform->getWindowOpacity = &_glfwGetWindowOpacityCocoa;
        platform->setWindowResizable = &_glfwSetWindowResizableCocoa;
        platform->setWindowDecorated = &_glfwSetWindowDecoratedCocoa;
        platform->setWindowFloating = &_glfwSetWindowFloatingCocoa;
        platform->setWindowOpacity = &_glfwSetWindowOpacityCocoa;
        platform->setWindowMousePassthrough = &_glfwSetWindowMousePassthroughCocoa;
        platform->pollEvents = &_glfwPollEventsCocoa;
        platform->waitEvents = &_glfwWaitEventsCocoa;
        platform->waitEventsTimeout = &_glfwWaitEventsTimeoutCocoa;
        platform->postEmptyEvent = &_glfwPostEmptyEventCocoa;
        platform->getEGLPlatform = &_glfwGetEGLPlatformCocoa;
        platform->getEGLNativeDisplay = &_glfwGetEGLNativeDisplayCocoa;
        platform->getEGLNativeWindow = &_glfwGetEGLNativeWindowCocoa;
        platform->getRequiredInstanceExtensions = &_glfwGetRequiredInstanceExtensionsCocoa;
        platform->getPhysicalDevicePresentationSupport = &_glfwGetPhysicalDevicePresentationSupportCocoa;
        platform->createWindowSurface = &_glfwCreateWindowSurfaceCocoa;
        return GLFW_TRUE;
    }

    static void cocoa_registerUserDefaults()
    {
        var defaults = cocoa_msgSend_id(cocoa_getClass("NSUserDefaults"), "standardUserDefaults");
        if (defaults == null)
            return;

        var key = cocoa_stringFromUTF8("ApplePressAndHoldEnabled");
        if (key == null)
            return;

        var value = cocoa_msgSend_id_bool(cocoa_getClass("NSNumber"), "numberWithBool:", GLFW_FALSE);
        var dictionary = objc_msgSend_id_ptr_ptr(cocoa_getClass("NSDictionary"),
            cocoa_sel("dictionaryWithObject:forKey:"),
            value,
            key);
        if (dictionary != null)
            cocoa_msgSend_void_ptr(defaults, "registerDefaults:", dictionary);

        cocoa_releaseTemporaryString(key);
    }

    static void cocoa_changeToResourcesDirectory()
    {
        var bundle = cocoa_msgSend_id(cocoa_getClass("NSBundle"), "mainBundle");
        if (bundle == null)
            return;

        var resourcesPath = cocoa_msgSend_id(bundle, "resourcePath");
        if (resourcesPath == null)
            return;

        var lastComponent = cocoa_msgSend_id(resourcesPath, "lastPathComponent");
        if (lastComponent == null)
            return;

        var expected = cocoa_stringFromUTF8("Resources");
        if (expected == null)
            return;

        var isResources = objc_msgSend_bool_ptr(lastComponent,
            cocoa_sel("isEqualToString:"),
            expected);
        cocoa_releaseTemporaryString(expected);

        if (isResources == 0)
            return;

        var fileManager = cocoa_msgSend_id(cocoa_getClass("NSFileManager"), "defaultManager");
        if (fileManager != null)
            objc_msgSend_bool_ptr(fileManager, cocoa_sel("changeCurrentDirectoryPath:"), resourcesPath);
    }

    static void* cocoa_copyApplicationName()
    {
        var bundle = cocoa_msgSend_id(cocoa_getClass("NSBundle"), "mainBundle");
        var bundleInfo = bundle != null ? cocoa_msgSend_id(bundle, "infoDictionary") : null;
        if (bundleInfo != null)
        {
            string[] nameKeys =
            {
                "CFBundleDisplayName",
                "CFBundleName",
                "CFBundleExecutable"
            };

            foreach (var key in nameKeys)
            {
                var keyObject = cocoa_stringFromUTF8(key);
                if (keyObject == null)
                    continue;

                var name = objc_msgSend_id_ptr(bundleInfo, cocoa_sel("objectForKey:"), keyObject);
                cocoa_releaseTemporaryString(keyObject);

                if (name != null &&
                    objc_msgSend_bool_ptr(name, cocoa_sel("isKindOfClass:"), cocoa_getClass("NSString")) != 0 &&
                    objc_msgSend_ulong(name, cocoa_sel("length")) != 0)
                {
                    return cocoa_msgSend_id(name, "retain");
                }
            }
        }

        var progname = _NSGetProgname();
        if (progname != null && *progname != null)
            return cocoa_stringFromUTF8(*progname);

        return cocoa_stringFromUTF8("GLFW Application");
    }

    static void* cocoa_copyStringByAppendingString(string prefix, void* suffix)
    {
        var prefixObject = cocoa_stringFromUTF8(prefix);
        if (prefixObject == null || suffix == null)
        {
            cocoa_releaseTemporaryString(prefixObject);
            return null;
        }

        var result = objc_msgSend_id_ptr(prefixObject,
            cocoa_sel("stringByAppendingString:"),
            suffix);
        if (result != null)
            cocoa_msgSend_void(result, "retain");

        cocoa_releaseTemporaryString(prefixObject);
        return result;
    }

    static void* cocoa_addMenuItem(void* menu, void* title, nint action, string keyEquivalent)
    {
        if (menu == null || title == null)
            return null;

        var key = cocoa_stringFromUTF8(keyEquivalent);
        if (key == null)
            return null;

        var item = objc_msgSend_id_ptr_nint_ptr(menu,
            cocoa_sel("addItemWithTitle:action:keyEquivalent:"),
            title,
            action,
            key);
        cocoa_releaseTemporaryString(key);
        return item;
    }

    static void* cocoa_addMenuItem(void* menu, string title, nint action, string keyEquivalent)
    {
        var titleObject = cocoa_stringFromUTF8(title);
        var item = cocoa_addMenuItem(menu, titleObject, action, keyEquivalent);
        cocoa_releaseTemporaryString(titleObject);
        return item;
    }

    static void cocoa_addMenuSeparator(void* menu)
    {
        var item = cocoa_msgSend_id(cocoa_getClass("NSMenuItem"), "separatorItem");
        cocoa_msgSend_void_ptr(menu, "addItem:", item);
    }

    static void cocoa_createMenuBar()
    {
        var app = cocoa_getNSApp();
        if (app == null)
            return;

        var appName = cocoa_copyApplicationName();
        if (appName == null)
            return;

        var menuClass = cocoa_getClass("NSMenu");
        var bar = cocoa_msgSend_id(cocoa_msgSend_id(menuClass, "alloc"), "init");
        if (bar == null)
        {
            cocoa_releaseTemporaryString(appName);
            return;
        }

        cocoa_msgSend_void_ptr(app, "setMainMenu:", bar);

        var appMenuItem = cocoa_addMenuItem(bar, "", 0, "");
        var appMenu = cocoa_msgSend_id(cocoa_msgSend_id(menuClass, "alloc"), "init");
        cocoa_msgSend_void_ptr(appMenuItem, "setSubmenu:", appMenu);

        var title = cocoa_copyStringByAppendingString("About ", appName);
        cocoa_addMenuItem(appMenu, title, cocoa_sel("orderFrontStandardAboutPanel:"), "");
        cocoa_releaseTemporaryString(title);

        cocoa_addMenuSeparator(appMenu);

        var servicesMenu = cocoa_msgSend_id(cocoa_msgSend_id(menuClass, "alloc"), "init");
        cocoa_msgSend_void_ptr(app, "setServicesMenu:", servicesMenu);
        var servicesItem = cocoa_addMenuItem(appMenu, "Services", 0, "");
        cocoa_msgSend_void_ptr(servicesItem, "setSubmenu:", servicesMenu);
        cocoa_msgSend_void(servicesMenu, "release");

        cocoa_addMenuSeparator(appMenu);

        title = cocoa_copyStringByAppendingString("Hide ", appName);
        cocoa_addMenuItem(appMenu, title, cocoa_sel("hide:"), "h");
        cocoa_releaseTemporaryString(title);

        var hideOthersItem = cocoa_addMenuItem(appMenu, "Hide Others",
            cocoa_sel("hideOtherApplications:"),
            "h");
        objc_msgSend_void_ulong(hideOthersItem,
            cocoa_sel("setKeyEquivalentModifierMask:"),
            NSEventModifierFlagOption | NSEventModifierFlagCommand);

        cocoa_addMenuItem(appMenu, "Show All", cocoa_sel("unhideAllApplications:"), "");
        cocoa_addMenuSeparator(appMenu);

        title = cocoa_copyStringByAppendingString("Quit ", appName);
        cocoa_addMenuItem(appMenu, title, cocoa_sel("terminate:"), "q");
        cocoa_releaseTemporaryString(title);

        var windowMenuItem = cocoa_addMenuItem(bar, "", 0, "");
        cocoa_msgSend_void(bar, "release");

        title = cocoa_stringFromUTF8("Window");
        var windowMenu = objc_msgSend_id_ptr(cocoa_msgSend_id(menuClass, "alloc"),
            cocoa_sel("initWithTitle:"),
            title);
        cocoa_releaseTemporaryString(title);

        cocoa_msgSend_void_ptr(app, "setWindowsMenu:", windowMenu);
        cocoa_msgSend_void_ptr(windowMenuItem, "setSubmenu:", windowMenu);

        cocoa_addMenuItem(windowMenu, "Minimize", cocoa_sel("performMiniaturize:"), "m");
        cocoa_addMenuItem(windowMenu, "Zoom", cocoa_sel("performZoom:"), "");
        cocoa_addMenuSeparator(windowMenu);
        cocoa_addMenuItem(windowMenu, "Bring All to Front", cocoa_sel("arrangeInFront:"), "");
        cocoa_addMenuSeparator(windowMenu);

        var fullscreenItem = cocoa_addMenuItem(windowMenu, "Enter Full Screen",
            cocoa_sel("toggleFullScreen:"),
            "f");
        objc_msgSend_void_ulong(fullscreenItem,
            cocoa_sel("setKeyEquivalentModifierMask:"),
            NSEventModifierFlagControl | NSEventModifierFlagCommand);

        var setAppleMenu = cocoa_sel("setAppleMenu:");
        if (objc_msgSend_bool_nint(app, cocoa_sel("respondsToSelector:"), setAppleMenu) != 0)
        {
            objc_msgSend_void_nint_ptr(app,
                cocoa_sel("performSelector:withObject:"),
                setAppleMenu,
                appMenu);
        }

        cocoa_releaseTemporaryString(appName);
    }

    static void* cocoa_createCFStringASCII(string value)
    {
        var bytes = cocoa_ascii(value);
        fixed (byte* p = bytes)
            return CFStringCreateWithCString(null, p, kCFStringEncodingASCII);
    }

    static void* cocoa_getBundleFunctionPointer(void* bundle, string symbol)
    {
        var symbolName = cocoa_createCFStringASCII(symbol);
        if (symbolName == null)
            return null;

        var pointer = CFBundleGetFunctionPointerForName(bundle, symbolName);
        CFRelease(symbolName);
        return pointer;
    }

    static void* cocoa_getBundleDataPointer(void* bundle, string symbol)
    {
        var symbolName = cocoa_createCFStringASCII(symbol);
        if (symbolName == null)
            return null;

        var pointer = CFBundleGetDataPointerForName(bundle, symbolName);
        CFRelease(symbolName);
        return pointer;
    }

    static int cocoa_updateUnicodeData()
    {
        if (_glfw.ns.inputSource != null)
        {
            CFRelease(_glfw.ns.inputSource);
            _glfw.ns.inputSource = null;
            _glfw.ns.unicodeData = null;
        }

        if (_glfw.ns.tis.CopyCurrentKeyboardLayoutInputSource == null ||
            _glfw.ns.tis.GetInputSourceProperty == null ||
            _glfw.ns.tis.kPropertyUnicodeKeyLayoutData == null)
        {
            return GLFW_FALSE;
        }

        _glfw.ns.inputSource = _glfw.ns.tis.CopyCurrentKeyboardLayoutInputSource();
        if (_glfw.ns.inputSource == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to retrieve keyboard layout input source");
            return GLFW_FALSE;
        }

        _glfw.ns.unicodeData = _glfw.ns.tis.GetInputSourceProperty(_glfw.ns.inputSource,
            _glfw.ns.tis.kPropertyUnicodeKeyLayoutData);
        if (_glfw.ns.unicodeData == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to retrieve keyboard layout Unicode data");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static int cocoa_initializeTIS()
    {
        if (_glfw.ns.tis.bundle == null)
        {
            var bundleID = CFStringCreateWithCString(null, _glfwCocoaHIToolboxBundleID, kCFStringEncodingASCII);
            if (bundleID == null)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create HIToolbox framework bundle identifier");
                return GLFW_FALSE;
            }

            _glfw.ns.tis.bundle = CFBundleGetBundleWithIdentifier(bundleID);
            CFRelease(bundleID);
        }

        if (_glfw.ns.tis.bundle == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to load HIToolbox.framework");
            return GLFW_FALSE;
        }

        var keyLayoutData = (void**)cocoa_getBundleDataPointer(_glfw.ns.tis.bundle,
            "kTISPropertyUnicodeKeyLayoutData");
        _glfw.ns.tis.CopyCurrentKeyboardLayoutInputSource =
            (delegate* unmanaged<void*>)cocoa_getBundleFunctionPointer(_glfw.ns.tis.bundle,
                "TISCopyCurrentKeyboardLayoutInputSource");
        _glfw.ns.tis.GetInputSourceProperty =
            (delegate* unmanaged<void*, void*, void*>)cocoa_getBundleFunctionPointer(_glfw.ns.tis.bundle,
                "TISGetInputSourceProperty");
        _glfw.ns.tis.GetKbdType =
            (delegate* unmanaged<byte>)cocoa_getBundleFunctionPointer(_glfw.ns.tis.bundle,
                "LMGetKbdType");
        _glfw.ns.tis.UCKeyTranslate =
            (delegate* unmanaged<byte*, ushort, ushort, uint, uint, uint, uint*, uint, uint*, ushort*, int>)
            cocoa_getBundleFunctionPointer(_glfw.ns.tis.bundle,
                "UCKeyTranslate");

        if (keyLayoutData == null ||
            _glfw.ns.tis.CopyCurrentKeyboardLayoutInputSource == null ||
            _glfw.ns.tis.GetInputSourceProperty == null ||
            _glfw.ns.tis.GetKbdType == null ||
            _glfw.ns.tis.UCKeyTranslate == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to load TIS API symbols");
            return GLFW_FALSE;
        }

        _glfw.ns.tis.kPropertyUnicodeKeyLayoutData = *keyLayoutData;
        return cocoa_updateUnicodeData();
    }

    static void cocoa_observeKeyboardInputSourceChanges()
    {
        if (_glfw.ns.helper == null)
            return;

        var center = cocoa_msgSend_id(cocoa_getClass("NSNotificationCenter"), "defaultCenter");
        var name = cocoa_stringFromUTF8("NSTextInputContextKeyboardSelectionDidChangeNotification");
        if (center != null && name != null)
        {
            objc_msgSend_void_ptr_nint_ptr_ptr(center,
                cocoa_sel("addObserver:selector:name:object:"),
                _glfw.ns.helper,
                cocoa_sel("selectedKeyboardInputSourceChanged:"),
                name,
                null);
        }

        cocoa_releaseTemporaryString(name);
    }

    [UnmanagedCallersOnly]
    static void* cocoa_keyUpMonitorBlockInvoke(void* block, void* eventObject)
    {
        if (eventObject != null &&
            ((ulong)objc_msgSend_ulong(eventObject, cocoa_sel("modifierFlags")) & NSEventModifierFlagCommand) != 0)
        {
            var app = cocoa_getNSApp();
            var keyWindow = cocoa_msgSend_id(app, "keyWindow");
            cocoa_msgSend_void_ptr(keyWindow, "sendEvent:", eventObject);
        }

        return eventObject;
    }

    static void* cocoa_getNSConcreteGlobalBlock()
    {
        if (!NativeLibrary.TryLoad("/usr/lib/libSystem.B.dylib", out var handle))
            return null;

        return NativeLibrary.TryGetExport(handle, "_NSConcreteGlobalBlock", out var symbol)
            ? (void*)symbol
            : null;
    }

    static void cocoa_createKeyUpMonitor()
    {
        var blockClass = cocoa_getNSConcreteGlobalBlock();
        if (blockClass == null)
            return;

        var descriptor = (ObjCBlockDescriptor*)NativeMemory.AllocZeroed((nuint)sizeof(ObjCBlockDescriptor));
        var block = (ObjCBlockLiteral*)NativeMemory.AllocZeroed((nuint)sizeof(ObjCBlockLiteral));
        if (descriptor == null || block == null)
        {
            NativeMemory.Free(descriptor);
            NativeMemory.Free(block);
            return;
        }

        descriptor->reserved = 0;
        descriptor->size = (nuint)sizeof(ObjCBlockLiteral);
        descriptor->signature = _glfwCocoaEventBlockSignature;

        block->isa = blockClass;
        block->flags = BLOCK_IS_GLOBAL | BLOCK_HAS_SIGNATURE;
        block->reserved = 0;
        block->invoke = &cocoa_keyUpMonitorBlockInvoke;
        block->descriptor = descriptor;

        _glfw.ns.keyUpMonitorBlock = block;
        _glfw.ns.keyUpMonitorBlockDescriptor = descriptor;

        _glfw.ns.keyUpMonitor = objc_msgSend_id_ulong_ptr(cocoa_getClass("NSEvent"),
            cocoa_sel("addLocalMonitorForEventsMatchingMask:handler:"),
            NSEventMaskKeyUp,
            block);
    }

    static void cocoa_destroyKeyUpMonitor()
    {
        if (_glfw.ns.keyUpMonitor != null)
        {
            cocoa_msgSend_void_ptr(cocoa_getClass("NSEvent"), "removeMonitor:", _glfw.ns.keyUpMonitor);
            _glfw.ns.keyUpMonitor = null;
        }

        NativeMemory.Free(_glfw.ns.keyUpMonitorBlock);
        NativeMemory.Free(_glfw.ns.keyUpMonitorBlockDescriptor);
        _glfw.ns.keyUpMonitorBlock = null;
        _glfw.ns.keyUpMonitorBlockDescriptor = null;
    }

    static void cocoa_removeKeyboardInputSourceObserver()
    {
        if (_glfw.ns.helper == null)
            return;

        var center = cocoa_msgSend_id(cocoa_getClass("NSNotificationCenter"), "defaultCenter");
        var name = cocoa_stringFromUTF8("NSTextInputContextKeyboardSelectionDidChangeNotification");
        if (center != null && name != null)
        {
            objc_msgSend_void_ptr_ptr_ptr(center,
                cocoa_sel("removeObserver:name:object:"),
                _glfw.ns.helper,
                name,
                null);
            cocoa_msgSend_void_ptr(center, "removeObserver:", _glfw.ns.helper);
        }

        cocoa_releaseTemporaryString(name);
    }

    static void cocoa_createKeyTables()
    {
        fixed (short* keycodes = _glfw.ns.keycodes)
        fixed (short* scancodes = _glfw.ns.scancodes)
        {
            _glfw_memset(keycodes, 0xff, 256 * (nuint)sizeof(short));
            _glfw_memset(scancodes, 0xff, (GLFW_KEY_LAST + 1) * (nuint)sizeof(short));

            keycodes[0x1D] = GLFW_KEY_0;
            keycodes[0x12] = GLFW_KEY_1;
            keycodes[0x13] = GLFW_KEY_2;
            keycodes[0x14] = GLFW_KEY_3;
            keycodes[0x15] = GLFW_KEY_4;
            keycodes[0x17] = GLFW_KEY_5;
            keycodes[0x16] = GLFW_KEY_6;
            keycodes[0x1A] = GLFW_KEY_7;
            keycodes[0x1C] = GLFW_KEY_8;
            keycodes[0x19] = GLFW_KEY_9;
            keycodes[0x00] = GLFW_KEY_A;
            keycodes[0x0B] = GLFW_KEY_B;
            keycodes[0x08] = GLFW_KEY_C;
            keycodes[0x02] = GLFW_KEY_D;
            keycodes[0x0E] = GLFW_KEY_E;
            keycodes[0x03] = GLFW_KEY_F;
            keycodes[0x05] = GLFW_KEY_G;
            keycodes[0x04] = GLFW_KEY_H;
            keycodes[0x22] = GLFW_KEY_I;
            keycodes[0x26] = GLFW_KEY_J;
            keycodes[0x28] = GLFW_KEY_K;
            keycodes[0x25] = GLFW_KEY_L;
            keycodes[0x2E] = GLFW_KEY_M;
            keycodes[0x2D] = GLFW_KEY_N;
            keycodes[0x1F] = GLFW_KEY_O;
            keycodes[0x23] = GLFW_KEY_P;
            keycodes[0x0C] = GLFW_KEY_Q;
            keycodes[0x0F] = GLFW_KEY_R;
            keycodes[0x01] = GLFW_KEY_S;
            keycodes[0x11] = GLFW_KEY_T;
            keycodes[0x20] = GLFW_KEY_U;
            keycodes[0x09] = GLFW_KEY_V;
            keycodes[0x0D] = GLFW_KEY_W;
            keycodes[0x07] = GLFW_KEY_X;
            keycodes[0x10] = GLFW_KEY_Y;
            keycodes[0x06] = GLFW_KEY_Z;

            keycodes[0x27] = GLFW_KEY_APOSTROPHE;
            keycodes[0x2A] = GLFW_KEY_BACKSLASH;
            keycodes[0x2B] = GLFW_KEY_COMMA;
            keycodes[0x18] = GLFW_KEY_EQUAL;
            keycodes[0x32] = GLFW_KEY_GRAVE_ACCENT;
            keycodes[0x21] = GLFW_KEY_LEFT_BRACKET;
            keycodes[0x1B] = GLFW_KEY_MINUS;
            keycodes[0x2F] = GLFW_KEY_PERIOD;
            keycodes[0x1E] = GLFW_KEY_RIGHT_BRACKET;
            keycodes[0x29] = GLFW_KEY_SEMICOLON;
            keycodes[0x2C] = GLFW_KEY_SLASH;
            keycodes[0x0A] = GLFW_KEY_WORLD_1;

            keycodes[0x33] = GLFW_KEY_BACKSPACE;
            keycodes[0x39] = GLFW_KEY_CAPS_LOCK;
            keycodes[0x75] = GLFW_KEY_DELETE;
            keycodes[0x7D] = GLFW_KEY_DOWN;
            keycodes[0x77] = GLFW_KEY_END;
            keycodes[0x24] = GLFW_KEY_ENTER;
            keycodes[0x35] = GLFW_KEY_ESCAPE;
            keycodes[0x7A] = GLFW_KEY_F1;
            keycodes[0x78] = GLFW_KEY_F2;
            keycodes[0x63] = GLFW_KEY_F3;
            keycodes[0x76] = GLFW_KEY_F4;
            keycodes[0x60] = GLFW_KEY_F5;
            keycodes[0x61] = GLFW_KEY_F6;
            keycodes[0x62] = GLFW_KEY_F7;
            keycodes[0x64] = GLFW_KEY_F8;
            keycodes[0x65] = GLFW_KEY_F9;
            keycodes[0x6D] = GLFW_KEY_F10;
            keycodes[0x67] = GLFW_KEY_F11;
            keycodes[0x6F] = GLFW_KEY_F12;
            keycodes[0x69] = GLFW_KEY_PRINT_SCREEN;
            keycodes[0x6B] = GLFW_KEY_F14;
            keycodes[0x71] = GLFW_KEY_F15;
            keycodes[0x6A] = GLFW_KEY_F16;
            keycodes[0x40] = GLFW_KEY_F17;
            keycodes[0x4F] = GLFW_KEY_F18;
            keycodes[0x50] = GLFW_KEY_F19;
            keycodes[0x5A] = GLFW_KEY_F20;
            keycodes[0x73] = GLFW_KEY_HOME;
            keycodes[0x72] = GLFW_KEY_INSERT;
            keycodes[0x7B] = GLFW_KEY_LEFT;
            keycodes[0x3A] = GLFW_KEY_LEFT_ALT;
            keycodes[0x3B] = GLFW_KEY_LEFT_CONTROL;
            keycodes[0x38] = GLFW_KEY_LEFT_SHIFT;
            keycodes[0x37] = GLFW_KEY_LEFT_SUPER;
            keycodes[0x6E] = GLFW_KEY_MENU;
            keycodes[0x47] = GLFW_KEY_NUM_LOCK;
            keycodes[0x79] = GLFW_KEY_PAGE_DOWN;
            keycodes[0x74] = GLFW_KEY_PAGE_UP;
            keycodes[0x7C] = GLFW_KEY_RIGHT;
            keycodes[0x3D] = GLFW_KEY_RIGHT_ALT;
            keycodes[0x3E] = GLFW_KEY_RIGHT_CONTROL;
            keycodes[0x3C] = GLFW_KEY_RIGHT_SHIFT;
            keycodes[0x36] = GLFW_KEY_RIGHT_SUPER;
            keycodes[0x31] = GLFW_KEY_SPACE;
            keycodes[0x30] = GLFW_KEY_TAB;
            keycodes[0x7E] = GLFW_KEY_UP;

            keycodes[0x52] = GLFW_KEY_KP_0;
            keycodes[0x53] = GLFW_KEY_KP_1;
            keycodes[0x54] = GLFW_KEY_KP_2;
            keycodes[0x55] = GLFW_KEY_KP_3;
            keycodes[0x56] = GLFW_KEY_KP_4;
            keycodes[0x57] = GLFW_KEY_KP_5;
            keycodes[0x58] = GLFW_KEY_KP_6;
            keycodes[0x59] = GLFW_KEY_KP_7;
            keycodes[0x5B] = GLFW_KEY_KP_8;
            keycodes[0x5C] = GLFW_KEY_KP_9;
            keycodes[0x45] = GLFW_KEY_KP_ADD;
            keycodes[0x41] = GLFW_KEY_KP_DECIMAL;
            keycodes[0x4B] = GLFW_KEY_KP_DIVIDE;
            keycodes[0x4C] = GLFW_KEY_KP_ENTER;
            keycodes[0x51] = GLFW_KEY_KP_EQUAL;
            keycodes[0x43] = GLFW_KEY_KP_MULTIPLY;
            keycodes[0x4E] = GLFW_KEY_KP_SUBTRACT;

            for (var scancode = 0; scancode < 256; scancode++)
            {
                if (keycodes[scancode] >= 0)
                    scancodes[keycodes[scancode]] = (short)scancode;
            }
        }
    }

    static int _glfwInitCocoa()
    {
        if (!OperatingSystem.IsMacOS())
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Cocoa: Platform not available on this system");
            return GLFW_FALSE;
        }

        _glfw.ns.autoreleasePool = cocoa_createAutoreleasePool();
        if (_glfw.ns.autoreleasePool == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create autorelease pool");
            return GLFW_FALSE;
        }

        if (cocoa_registerWindowClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register window class");
            return GLFW_FALSE;
        }

        if (cocoa_registerWindowDelegateClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register window delegate class");
            return GLFW_FALSE;
        }

        if (cocoa_registerContentViewClass() == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register content view class");
            return GLFW_FALSE;
        }

        var helperClass = cocoa_registerHelperClass();
        if (helperClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to register helper class");
            return GLFW_FALSE;
        }

        _glfw.ns.helper = cocoa_msgSend_id(cocoa_msgSend_id(helperClass, "alloc"), "init");
        if (_glfw.ns.helper == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create helper object");
            return GLFW_FALSE;
        }

        objc_msgSend_void_nint_ptr_ptr(cocoa_getClass("NSThread"),
            cocoa_sel("detachNewThreadSelector:toTarget:withObject:"),
            cocoa_sel("doNothing:"),
            _glfw.ns.helper,
            null);

        var app = cocoa_getNSApp();
        var appDelegateClass = cocoa_registerApplicationDelegateClass();
        if (app == null || appDelegateClass == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to initialize application delegate class");
            return GLFW_FALSE;
        }

        _glfw.ns.delegateObject = cocoa_msgSend_id(cocoa_msgSend_id(appDelegateClass, "alloc"), "init");
        if (_glfw.ns.delegateObject == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Cocoa: Failed to create application delegate");
            return GLFW_FALSE;
        }

        cocoa_msgSend_void_ptr(app, "setDelegate:", _glfw.ns.delegateObject);
        cocoa_createKeyUpMonitor();

        if (_glfw.hints.init.ns.chdir != 0)
            cocoa_changeToResourcesDirectory();

        cocoa_registerUserDefaults();
        cocoa_observeKeyboardInputSourceChanges();
        cocoa_createKeyTables();

        _glfw.ns.eventSource = CGEventSourceCreate(kCGEventSourceStateHIDSystemState);
        if (_glfw.ns.eventSource == null)
            return GLFW_FALSE;

        CGEventSourceSetLocalEventsSuppressionInterval(_glfw.ns.eventSource, 0.0);

        if (cocoa_initializeTIS() == 0)
            return GLFW_FALSE;

        _glfwPollMonitorsCocoa();

        var currentApplication = cocoa_msgSend_id(cocoa_getClass("NSRunningApplication"), "currentApplication");
        if (currentApplication != null &&
            objc_msgSend_bool(currentApplication, cocoa_sel("isFinishedLaunching")) == 0)
        {
            cocoa_msgSend_void(app, "run");
        }

        if (_glfw.hints.init.ns.menubar != 0)
            objc_msgSend_void_long(app, cocoa_sel("setActivationPolicy:"), NSApplicationActivationPolicyRegular);

        return GLFW_TRUE;
    }

    static void _glfwTerminateCocoa()
    {
        cocoa_showCursor();
        _glfwTerminateNSGL();

        if (_glfw.ns.inputSource != null)
        {
            CFRelease(_glfw.ns.inputSource);
            _glfw.ns.inputSource = null;
            _glfw.ns.unicodeData = null;
        }

        if (_glfw.ns.eventSource != null)
        {
            CFRelease(_glfw.ns.eventSource);
            _glfw.ns.eventSource = null;
        }

        _glfw_free(_glfw.ns.clipboardString);
        _glfw.ns.clipboardString = null;

        var app = cocoa_getNSApp();
        cocoa_destroyKeyUpMonitor();

        cocoa_msgSend_void_ptr(app, "setDelegate:", null);
        cocoa_msgSend_void(_glfw.ns.delegateObject, "release");
        _glfw.ns.delegateObject = null;

        if (_glfw.ns.helper != null)
        {
            cocoa_removeKeyboardInputSourceObserver();
            cocoa_msgSend_void(_glfw.ns.helper, "release");
            _glfw.ns.helper = null;
        }

        _glfw.ns.tis = default;
        cocoa_drainAutoreleasePool(_glfw.ns.autoreleasePool);
        _glfw.ns.autoreleasePool = null;
    }
}
