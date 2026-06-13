using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint WL_DISPLAY_SYNC = 0;
    const uint WL_DISPLAY_GET_REGISTRY = 1;
    const uint WL_REGISTRY_BIND = 0;
    const uint WL_COMPOSITOR_CREATE_SURFACE = 0;
    const uint WL_COMPOSITOR_CREATE_REGION = 1;
    const uint WL_SURFACE_DESTROY = 0;
    const uint WL_SURFACE_ATTACH = 1;
    const uint WL_SURFACE_DAMAGE = 2;
    const uint WL_SURFACE_SET_OPAQUE_REGION = 4;
    const uint WL_SURFACE_SET_INPUT_REGION = 5;
    const uint WL_SURFACE_COMMIT = 6;
    const uint WL_SURFACE_SET_BUFFER_SCALE = 8;
    const uint WL_SURFACE_SET_BUFFER_SCALE_SINCE_VERSION = 3;
    const uint WL_REGION_DESTROY = 0;
    const uint WL_REGION_ADD = 1;
    const uint WL_OUTPUT_RELEASE = 0;
    const uint WL_SEAT_GET_POINTER = 0;
    const uint WL_SEAT_GET_KEYBOARD = 1;
    const uint WL_SEAT_RELEASE = 3;
    const uint WL_SEAT_RELEASE_SINCE_VERSION = 5;
    const uint WL_POINTER_RELEASE = 1;
    const uint WL_POINTER_RELEASE_SINCE_VERSION = 3;
    const uint WL_KEYBOARD_RELEASE = 0;
    const uint WL_KEYBOARD_RELEASE_SINCE_VERSION = 3;
    const uint WL_DATA_OFFER_ACCEPT = 0;
    const uint WL_DATA_OFFER_RECEIVE = 1;
    const uint WL_DATA_OFFER_DESTROY = 2;
    const uint WL_DATA_SOURCE_OFFER = 0;
    const uint WL_DATA_SOURCE_DESTROY = 1;
    const uint WL_DATA_DEVICE_SET_SELECTION = 1;
    const uint WL_DATA_DEVICE_MANAGER_CREATE_DATA_SOURCE = 0;
    const uint WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE = 1;
    const uint WL_DATA_DEVICE_RELEASE = 2;
    const uint WL_DATA_DEVICE_RELEASE_SINCE_VERSION = 2;
    const uint WL_BUFFER_DESTROY = 0;
    const uint WL_SHM_CREATE_POOL = 0;
    const uint WL_SHM_POOL_CREATE_BUFFER = 0;
    const uint WL_SHM_POOL_DESTROY = 1;
    const uint WL_SHM_FORMAT_ARGB8888 = 0;
    const uint WL_MARSHAL_FLAG_DESTROY = 1;
    const uint XDG_WM_BASE_DESTROY = 0;
    const uint XDG_WM_BASE_GET_XDG_SURFACE = 2;
    const uint XDG_WM_BASE_PONG = 3;
    const uint XDG_SURFACE_DESTROY = 0;
    const uint XDG_SURFACE_GET_TOPLEVEL = 1;
    const uint XDG_SURFACE_SET_WINDOW_GEOMETRY = 3;
    const uint XDG_SURFACE_ACK_CONFIGURE = 4;
    const uint XDG_TOPLEVEL_DESTROY = 0;
    const uint XDG_TOPLEVEL_SET_PARENT = 1;
    const uint XDG_TOPLEVEL_SET_TITLE = 2;
    const uint XDG_TOPLEVEL_SET_APP_ID = 3;
    const uint XDG_TOPLEVEL_SET_MAX_SIZE = 7;
    const uint XDG_TOPLEVEL_SET_MIN_SIZE = 8;
    const uint XDG_TOPLEVEL_SET_MAXIMIZED = 9;
    const uint XDG_TOPLEVEL_UNSET_MAXIMIZED = 10;
    const uint XDG_TOPLEVEL_SET_FULLSCREEN = 11;
    const uint XDG_TOPLEVEL_UNSET_FULLSCREEN = 12;
    const uint XDG_TOPLEVEL_SET_MINIMIZED = 13;
    const uint ZXDG_DECORATION_MANAGER_DESTROY = 0;
    const uint ZXDG_DECORATION_MANAGER_GET_TOPLEVEL_DECORATION = 1;
    const uint ZXDG_TOPLEVEL_DECORATION_DESTROY = 0;
    const uint ZXDG_TOPLEVEL_DECORATION_SET_MODE = 1;
    const uint ZXDG_TOPLEVEL_DECORATION_UNSET_MODE = 2;
    const uint ZWP_IDLE_INHIBIT_MANAGER_DESTROY = 0;
    const uint ZWP_IDLE_INHIBIT_MANAGER_CREATE_INHIBITOR = 1;
    const uint ZWP_IDLE_INHIBITOR_DESTROY = 0;
    const uint XDG_ACTIVATION_DESTROY = 0;
    const uint XDG_ACTIVATION_GET_ACTIVATION_TOKEN = 1;
    const uint XDG_ACTIVATION_ACTIVATE = 2;
    const uint XDG_ACTIVATION_TOKEN_SET_SERIAL = 0;
    const uint XDG_ACTIVATION_TOKEN_SET_APP_ID = 1;
    const uint XDG_ACTIVATION_TOKEN_SET_SURFACE = 2;
    const uint XDG_ACTIVATION_TOKEN_COMMIT = 3;
    const uint XDG_ACTIVATION_TOKEN_DESTROY = 4;
    const uint ZWP_RELATIVE_POINTER_MANAGER_DESTROY = 0;
    const uint ZWP_RELATIVE_POINTER_MANAGER_GET_RELATIVE_POINTER = 1;
    const uint ZWP_RELATIVE_POINTER_DESTROY = 0;
    const uint ZWP_POINTER_CONSTRAINTS_DESTROY = 0;
    const uint ZWP_POINTER_CONSTRAINTS_LOCK_POINTER = 1;
    const uint ZWP_POINTER_CONSTRAINTS_CONFINE_POINTER = 2;
    const uint ZWP_POINTER_CONSTRAINTS_LIFETIME_PERSISTENT = 2;
    const uint ZWP_LOCKED_POINTER_DESTROY = 0;
    const uint ZWP_CONFINED_POINTER_DESTROY = 0;
    const uint WP_VIEWPORTER_DESTROY = 0;
    const uint WP_VIEWPORTER_GET_VIEWPORT = 1;
    const uint WP_VIEWPORT_DESTROY = 0;
    const uint WP_VIEWPORT_SET_DESTINATION = 2;
    const uint WP_FRACTIONAL_SCALE_MANAGER_DESTROY = 0;
    const uint WP_FRACTIONAL_SCALE_MANAGER_GET_FRACTIONAL_SCALE = 1;
    const uint WP_FRACTIONAL_SCALE_DESTROY = 0;

    static readonly byte* _glfwWaylandWlCompositor = _glfw_allocate_static_string("wl_compositor");
    static readonly byte* _glfwWaylandWlSubcompositor = _glfw_allocate_static_string("wl_subcompositor");
    static readonly byte* _glfwWaylandWlShm = _glfw_allocate_static_string("wl_shm");
    static readonly byte* _glfwWaylandWlOutput = _glfw_allocate_static_string("wl_output");
    static readonly byte* _glfwWaylandWlSeat = _glfw_allocate_static_string("wl_seat");
    static readonly byte* _glfwWaylandWlDataDeviceManager = _glfw_allocate_static_string("wl_data_device_manager");
    static readonly byte* _glfwWaylandXdgWmBase = _glfw_allocate_static_string("xdg_wm_base");
    static readonly byte* _glfwWaylandZxdgDecorationManagerV1 = _glfw_allocate_static_string("zxdg_decoration_manager_v1");
    static readonly byte* _glfwWaylandZwpIdleInhibitManagerV1 = _glfw_allocate_static_string("zwp_idle_inhibit_manager_v1");
    static readonly byte* _glfwWaylandXdgActivationV1 = _glfw_allocate_static_string("xdg_activation_v1");
    static readonly byte* _glfwWaylandZwpRelativePointerManagerV1 = _glfw_allocate_static_string("zwp_relative_pointer_manager_v1");
    static readonly byte* _glfwWaylandZwpPointerConstraintsV1 = _glfw_allocate_static_string("zwp_pointer_constraints_v1");
    static readonly byte* _glfwWaylandWpViewporter = _glfw_allocate_static_string("wp_viewporter");
    static readonly byte* _glfwWaylandWpFractionalScaleManagerV1 = _glfw_allocate_static_string("wp_fractional_scale_manager_v1");
    static readonly byte* _glfwWaylandXkbControl = _glfw_allocate_static_string("Control");
    static readonly byte* _glfwWaylandXkbMod1 = _glfw_allocate_static_string("Mod1");
    static readonly byte* _glfwWaylandXkbShift = _glfw_allocate_static_string("Shift");
    static readonly byte* _glfwWaylandXkbMod4 = _glfw_allocate_static_string("Mod4");
    static readonly byte* _glfwWaylandXkbLock = _glfw_allocate_static_string("Lock");
    static readonly byte* _glfwWaylandXkbMod2 = _glfw_allocate_static_string("Mod2");
    static readonly byte* _glfwWaylandTextPlainUtf8 = _glfw_allocate_static_string("text/plain;charset=utf-8");
    static readonly byte* _glfwWaylandTextUriList = _glfw_allocate_static_string("text/uri-list");

    struct wl_message
    {
        public byte* name;
        public byte* signature;
        public wl_interface** types;
    }

    struct wl_interface
    {
        public byte* name;
        public int version;
        public int method_count;
        public wl_message* methods;
        public int event_count;
        public wl_message* events;
    }

    struct wl_registry_listener
    {
        public delegate* unmanaged<void*, void*, uint, byte*, uint, void> global;
        public delegate* unmanaged<void*, void*, uint, void> global_remove;
    }

    struct xdg_wm_base_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> ping;
    }

    static wl_registry_listener* _glfwWaylandRegistryListener;
    static xdg_wm_base_listener* _glfwWaylandXdgWmBaseListener;
    static wl_interface* _glfwWaylandXdgWmBaseInterface;
    static wl_interface* _glfwWaylandXdgPositionerInterface;
    static wl_interface* _glfwWaylandXdgSurfaceInterface;
    static wl_interface* _glfwWaylandXdgToplevelInterface;
    static wl_interface* _glfwWaylandXdgPopupInterface;
    static wl_interface* _glfwWaylandZxdgDecorationManagerV1Interface;
    static wl_interface* _glfwWaylandZxdgToplevelDecorationV1Interface;
    static wl_interface* _glfwWaylandZwpIdleInhibitManagerV1Interface;
    static wl_interface* _glfwWaylandZwpIdleInhibitorV1Interface;
    static wl_interface* _glfwWaylandXdgActivationV1Interface;
    static wl_interface* _glfwWaylandXdgActivationTokenV1Interface;
    static wl_interface* _glfwWaylandZwpRelativePointerManagerV1Interface;
    static wl_interface* _glfwWaylandZwpRelativePointerV1Interface;
    static wl_interface* _glfwWaylandZwpPointerConstraintsV1Interface;
    static wl_interface* _glfwWaylandZwpLockedPointerV1Interface;
    static wl_interface* _glfwWaylandZwpConfinedPointerV1Interface;
    static wl_interface* _glfwWaylandWpViewporterInterface;
    static wl_interface* _glfwWaylandWpViewportInterface;
    static wl_interface* _glfwWaylandWpFractionalScaleManagerInterface;
    static wl_interface* _glfwWaylandWpFractionalScaleInterface;

    static uint wayland_min(uint a, uint b)
    {
        return a < b ? a : b;
    }

    static int wayland_stringEquals(byte* value, string expected)
    {
        if (value == null)
            return GLFW_FALSE;

        for (var i = 0; i < expected.Length; i++)
        {
            if (value[i] != (byte)expected[i])
                return GLFW_FALSE;
        }

        return value[expected.Length] == 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static wl_message* wayland_allocMessages(int count)
    {
        return (wl_message*)_glfw_calloc((nuint)count, (nuint)sizeof(wl_message));
    }

    static wl_interface** wayland_allocTypes(int count)
    {
        return (wl_interface**)_glfw_calloc((nuint)count, (nuint)sizeof(wl_interface*));
    }

    static void wayland_setMessage(wl_message* messages, int index, string name, string signature, wl_interface** types)
    {
        messages[index].name = _glfw_allocate_static_string(name);
        messages[index].signature = _glfw_allocate_static_string(signature);
        messages[index].types = types;
    }

    static int wayland_initXdgShellInterfaces()
    {
        if (_glfwWaylandXdgWmBaseInterface != null)
            return GLFW_TRUE;

        var wmBase = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var positioner = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var surface = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var toplevel = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var popup = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (wmBase == null || positioner == null || surface == null || toplevel == null || popup == null)
            return GLFW_FALSE;

        var wmBaseMethods = wayland_allocMessages(4);
        var wmBaseEvents = wayland_allocMessages(1);
        var surfaceMethods = wayland_allocMessages(5);
        var surfaceEvents = wayland_allocMessages(1);
        var toplevelMethods = wayland_allocMessages(14);
        var toplevelEvents = wayland_allocMessages(2);
        if (wmBaseMethods == null || wmBaseEvents == null ||
            surfaceMethods == null || surfaceEvents == null ||
            toplevelMethods == null || toplevelEvents == null)
        {
            return GLFW_FALSE;
        }

        var wmBaseCreatePositionerTypes = wayland_allocTypes(1);
        var wmBaseGetSurfaceTypes = wayland_allocTypes(2);
        var surfaceGetToplevelTypes = wayland_allocTypes(1);
        var surfaceGetPopupTypes = wayland_allocTypes(3);
        var toplevelSetParentTypes = wayland_allocTypes(1);
        var toplevelShowMenuTypes = wayland_allocTypes(4);
        var toplevelMoveTypes = wayland_allocTypes(2);
        var toplevelResizeTypes = wayland_allocTypes(3);
        var toplevelSetFullscreenTypes = wayland_allocTypes(1);
        if (wmBaseCreatePositionerTypes == null ||
            wmBaseGetSurfaceTypes == null ||
            surfaceGetToplevelTypes == null ||
            surfaceGetPopupTypes == null ||
            toplevelSetParentTypes == null ||
            toplevelShowMenuTypes == null ||
            toplevelMoveTypes == null ||
            toplevelResizeTypes == null ||
            toplevelSetFullscreenTypes == null)
        {
            return GLFW_FALSE;
        }

        wmBaseCreatePositionerTypes[0] = positioner;
        wmBaseGetSurfaceTypes[0] = surface;
        wmBaseGetSurfaceTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;
        surfaceGetToplevelTypes[0] = toplevel;
        surfaceGetPopupTypes[0] = popup;
        surfaceGetPopupTypes[1] = surface;
        surfaceGetPopupTypes[2] = positioner;
        toplevelSetParentTypes[0] = toplevel;
        toplevelShowMenuTypes[0] = (wl_interface*)_glfw.wl.client.seatInterface;
        toplevelMoveTypes[0] = (wl_interface*)_glfw.wl.client.seatInterface;
        toplevelResizeTypes[0] = (wl_interface*)_glfw.wl.client.seatInterface;
        toplevelSetFullscreenTypes[0] = (wl_interface*)_glfw.wl.client.outputInterface;

        wayland_setMessage(wmBaseMethods, 0, "destroy", "", null);
        wayland_setMessage(wmBaseMethods, 1, "create_positioner", "n", wmBaseCreatePositionerTypes);
        wayland_setMessage(wmBaseMethods, 2, "get_xdg_surface", "no", wmBaseGetSurfaceTypes);
        wayland_setMessage(wmBaseMethods, 3, "pong", "u", null);
        wayland_setMessage(wmBaseEvents, 0, "ping", "u", null);

        wayland_setMessage(surfaceMethods, 0, "destroy", "", null);
        wayland_setMessage(surfaceMethods, 1, "get_toplevel", "n", surfaceGetToplevelTypes);
        wayland_setMessage(surfaceMethods, 2, "get_popup", "noo", surfaceGetPopupTypes);
        wayland_setMessage(surfaceMethods, 3, "set_window_geometry", "iiii", null);
        wayland_setMessage(surfaceMethods, 4, "ack_configure", "u", null);
        wayland_setMessage(surfaceEvents, 0, "configure", "u", null);

        wayland_setMessage(toplevelMethods, 0, "destroy", "", null);
        wayland_setMessage(toplevelMethods, 1, "set_parent", "?o", toplevelSetParentTypes);
        wayland_setMessage(toplevelMethods, 2, "set_title", "s", null);
        wayland_setMessage(toplevelMethods, 3, "set_app_id", "s", null);
        wayland_setMessage(toplevelMethods, 4, "show_window_menu", "ouii", toplevelShowMenuTypes);
        wayland_setMessage(toplevelMethods, 5, "move", "ou", toplevelMoveTypes);
        wayland_setMessage(toplevelMethods, 6, "resize", "ouu", toplevelResizeTypes);
        wayland_setMessage(toplevelMethods, 7, "set_max_size", "ii", null);
        wayland_setMessage(toplevelMethods, 8, "set_min_size", "ii", null);
        wayland_setMessage(toplevelMethods, 9, "set_maximized", "", null);
        wayland_setMessage(toplevelMethods, 10, "unset_maximized", "", null);
        wayland_setMessage(toplevelMethods, 11, "set_fullscreen", "?o", toplevelSetFullscreenTypes);
        wayland_setMessage(toplevelMethods, 12, "unset_fullscreen", "", null);
        wayland_setMessage(toplevelMethods, 13, "set_minimized", "", null);
        wayland_setMessage(toplevelEvents, 0, "configure", "iia", null);
        wayland_setMessage(toplevelEvents, 1, "close", "", null);

        *wmBase = new wl_interface
        {
            name = _glfwWaylandXdgWmBase,
            version = 6,
            method_count = 4,
            methods = wmBaseMethods,
            event_count = 1,
            events = wmBaseEvents
        };
        *positioner = new wl_interface
        {
            name = _glfw_allocate_static_string("xdg_positioner"),
            version = 6
        };
        *surface = new wl_interface
        {
            name = _glfw_allocate_static_string("xdg_surface"),
            version = 6,
            method_count = 5,
            methods = surfaceMethods,
            event_count = 1,
            events = surfaceEvents
        };
        *toplevel = new wl_interface
        {
            name = _glfw_allocate_static_string("xdg_toplevel"),
            version = 6,
            method_count = 14,
            methods = toplevelMethods,
            event_count = 2,
            events = toplevelEvents
        };
        *popup = new wl_interface
        {
            name = _glfw_allocate_static_string("xdg_popup"),
            version = 6
        };

        _glfwWaylandXdgWmBaseInterface = wmBase;
        _glfwWaylandXdgPositionerInterface = positioner;
        _glfwWaylandXdgSurfaceInterface = surface;
        _glfwWaylandXdgToplevelInterface = toplevel;
        _glfwWaylandXdgPopupInterface = popup;

        return GLFW_TRUE;
    }

    static int wayland_initXdgDecorationInterfaces()
    {
        if (_glfwWaylandZxdgDecorationManagerV1Interface != null)
            return GLFW_TRUE;

        if (_glfwWaylandXdgToplevelInterface == null)
            return GLFW_FALSE;

        var manager = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var decoration = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (manager == null || decoration == null)
            return GLFW_FALSE;

        var managerMethods = wayland_allocMessages(2);
        var decorationMethods = wayland_allocMessages(3);
        var decorationEvents = wayland_allocMessages(1);
        if (managerMethods == null || decorationMethods == null || decorationEvents == null)
            return GLFW_FALSE;

        var getDecorationTypes = wayland_allocTypes(2);
        if (getDecorationTypes == null)
            return GLFW_FALSE;

        getDecorationTypes[0] = decoration;
        getDecorationTypes[1] = _glfwWaylandXdgToplevelInterface;

        wayland_setMessage(managerMethods, 0, "destroy", "", null);
        wayland_setMessage(managerMethods, 1, "get_toplevel_decoration", "no", getDecorationTypes);

        wayland_setMessage(decorationMethods, 0, "destroy", "", null);
        wayland_setMessage(decorationMethods, 1, "set_mode", "u", null);
        wayland_setMessage(decorationMethods, 2, "unset_mode", "", null);
        wayland_setMessage(decorationEvents, 0, "configure", "u", null);

        *manager = new wl_interface
        {
            name = _glfwWaylandZxdgDecorationManagerV1,
            version = 1,
            method_count = 2,
            methods = managerMethods
        };
        *decoration = new wl_interface
        {
            name = _glfw_allocate_static_string("zxdg_toplevel_decoration_v1"),
            version = 1,
            method_count = 3,
            methods = decorationMethods,
            event_count = 1,
            events = decorationEvents
        };

        _glfwWaylandZxdgDecorationManagerV1Interface = manager;
        _glfwWaylandZxdgToplevelDecorationV1Interface = decoration;
        return GLFW_TRUE;
    }

    static int wayland_initIdleInhibitInterfaces()
    {
        if (_glfwWaylandZwpIdleInhibitManagerV1Interface != null)
            return GLFW_TRUE;

        var manager = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var inhibitor = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (manager == null || inhibitor == null)
            return GLFW_FALSE;

        var managerMethods = wayland_allocMessages(2);
        var inhibitorMethods = wayland_allocMessages(1);
        if (managerMethods == null || inhibitorMethods == null)
            return GLFW_FALSE;

        var createInhibitorTypes = wayland_allocTypes(2);
        if (createInhibitorTypes == null)
            return GLFW_FALSE;

        createInhibitorTypes[0] = inhibitor;
        createInhibitorTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;

        wayland_setMessage(managerMethods, 0, "destroy", "", null);
        wayland_setMessage(managerMethods, 1, "create_inhibitor", "no", createInhibitorTypes);
        wayland_setMessage(inhibitorMethods, 0, "destroy", "", null);

        *manager = new wl_interface
        {
            name = _glfwWaylandZwpIdleInhibitManagerV1,
            version = 1,
            method_count = 2,
            methods = managerMethods
        };
        *inhibitor = new wl_interface
        {
            name = _glfw_allocate_static_string("zwp_idle_inhibitor_v1"),
            version = 1,
            method_count = 1,
            methods = inhibitorMethods
        };

        _glfwWaylandZwpIdleInhibitManagerV1Interface = manager;
        _glfwWaylandZwpIdleInhibitorV1Interface = inhibitor;
        return GLFW_TRUE;
    }

    static int wayland_initXdgActivationInterfaces()
    {
        if (_glfwWaylandXdgActivationV1Interface != null)
            return GLFW_TRUE;

        var activation = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var token = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (activation == null || token == null)
            return GLFW_FALSE;

        var activationMethods = wayland_allocMessages(3);
        var tokenMethods = wayland_allocMessages(5);
        var tokenEvents = wayland_allocMessages(1);
        if (activationMethods == null || tokenMethods == null || tokenEvents == null)
            return GLFW_FALSE;

        var getTokenTypes = wayland_allocTypes(1);
        var activateTypes = wayland_allocTypes(2);
        var setSerialTypes = wayland_allocTypes(2);
        var setSurfaceTypes = wayland_allocTypes(1);
        if (getTokenTypes == null || activateTypes == null || setSerialTypes == null || setSurfaceTypes == null)
            return GLFW_FALSE;

        getTokenTypes[0] = token;
        activateTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;
        setSerialTypes[1] = (wl_interface*)_glfw.wl.client.seatInterface;
        setSurfaceTypes[0] = (wl_interface*)_glfw.wl.client.surfaceInterface;

        wayland_setMessage(activationMethods, 0, "destroy", "", null);
        wayland_setMessage(activationMethods, 1, "get_activation_token", "n", getTokenTypes);
        wayland_setMessage(activationMethods, 2, "activate", "so", activateTypes);

        wayland_setMessage(tokenMethods, 0, "set_serial", "uo", setSerialTypes);
        wayland_setMessage(tokenMethods, 1, "set_app_id", "s", null);
        wayland_setMessage(tokenMethods, 2, "set_surface", "o", setSurfaceTypes);
        wayland_setMessage(tokenMethods, 3, "commit", "", null);
        wayland_setMessage(tokenMethods, 4, "destroy", "", null);
        wayland_setMessage(tokenEvents, 0, "done", "s", null);

        *activation = new wl_interface
        {
            name = _glfwWaylandXdgActivationV1,
            version = 1,
            method_count = 3,
            methods = activationMethods
        };
        *token = new wl_interface
        {
            name = _glfw_allocate_static_string("xdg_activation_token_v1"),
            version = 1,
            method_count = 5,
            methods = tokenMethods,
            event_count = 1,
            events = tokenEvents
        };

        _glfwWaylandXdgActivationV1Interface = activation;
        _glfwWaylandXdgActivationTokenV1Interface = token;
        return GLFW_TRUE;
    }

    static int wayland_initPointerConstraintInterfaces()
    {
        if (_glfwWaylandZwpRelativePointerManagerV1Interface != null)
            return GLFW_TRUE;

        var relativeManager = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var relativePointer = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var constraints = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var lockedPointer = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var confinedPointer = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (relativeManager == null ||
            relativePointer == null ||
            constraints == null ||
            lockedPointer == null ||
            confinedPointer == null)
        {
            return GLFW_FALSE;
        }

        var relativeManagerMethods = wayland_allocMessages(2);
        var relativePointerMethods = wayland_allocMessages(1);
        var relativePointerEvents = wayland_allocMessages(1);
        var constraintsMethods = wayland_allocMessages(3);
        var lockedPointerMethods = wayland_allocMessages(3);
        var lockedPointerEvents = wayland_allocMessages(2);
        var confinedPointerMethods = wayland_allocMessages(2);
        var confinedPointerEvents = wayland_allocMessages(2);
        if (relativeManagerMethods == null ||
            relativePointerMethods == null ||
            relativePointerEvents == null ||
            constraintsMethods == null ||
            lockedPointerMethods == null ||
            lockedPointerEvents == null ||
            confinedPointerMethods == null ||
            confinedPointerEvents == null)
        {
            return GLFW_FALSE;
        }

        var getRelativePointerTypes = wayland_allocTypes(2);
        var lockPointerTypes = wayland_allocTypes(5);
        var confinePointerTypes = wayland_allocTypes(5);
        var setLockedRegionTypes = wayland_allocTypes(1);
        var setConfinedRegionTypes = wayland_allocTypes(1);
        if (getRelativePointerTypes == null ||
            lockPointerTypes == null ||
            confinePointerTypes == null ||
            setLockedRegionTypes == null ||
            setConfinedRegionTypes == null)
        {
            return GLFW_FALSE;
        }

        getRelativePointerTypes[0] = relativePointer;
        getRelativePointerTypes[1] = (wl_interface*)_glfw.wl.client.pointerInterface;
        lockPointerTypes[0] = lockedPointer;
        lockPointerTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;
        lockPointerTypes[2] = (wl_interface*)_glfw.wl.client.pointerInterface;
        lockPointerTypes[3] = (wl_interface*)_glfw.wl.client.regionInterface;
        confinePointerTypes[0] = confinedPointer;
        confinePointerTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;
        confinePointerTypes[2] = (wl_interface*)_glfw.wl.client.pointerInterface;
        confinePointerTypes[3] = (wl_interface*)_glfw.wl.client.regionInterface;
        setLockedRegionTypes[0] = (wl_interface*)_glfw.wl.client.regionInterface;
        setConfinedRegionTypes[0] = (wl_interface*)_glfw.wl.client.regionInterface;

        wayland_setMessage(relativeManagerMethods, 0, "destroy", "", null);
        wayland_setMessage(relativeManagerMethods, 1, "get_relative_pointer", "no", getRelativePointerTypes);
        wayland_setMessage(relativePointerMethods, 0, "destroy", "", null);
        wayland_setMessage(relativePointerEvents, 0, "relative_motion", "uuffff", null);

        wayland_setMessage(constraintsMethods, 0, "destroy", "", null);
        wayland_setMessage(constraintsMethods, 1, "lock_pointer", "noo?ou", lockPointerTypes);
        wayland_setMessage(constraintsMethods, 2, "confine_pointer", "noo?ou", confinePointerTypes);

        wayland_setMessage(lockedPointerMethods, 0, "destroy", "", null);
        wayland_setMessage(lockedPointerMethods, 1, "set_cursor_position_hint", "ff", null);
        wayland_setMessage(lockedPointerMethods, 2, "set_region", "?o", setLockedRegionTypes);
        wayland_setMessage(lockedPointerEvents, 0, "locked", "", null);
        wayland_setMessage(lockedPointerEvents, 1, "unlocked", "", null);

        wayland_setMessage(confinedPointerMethods, 0, "destroy", "", null);
        wayland_setMessage(confinedPointerMethods, 1, "set_region", "?o", setConfinedRegionTypes);
        wayland_setMessage(confinedPointerEvents, 0, "confined", "", null);
        wayland_setMessage(confinedPointerEvents, 1, "unconfined", "", null);

        *relativeManager = new wl_interface
        {
            name = _glfwWaylandZwpRelativePointerManagerV1,
            version = 1,
            method_count = 2,
            methods = relativeManagerMethods
        };
        *relativePointer = new wl_interface
        {
            name = _glfw_allocate_static_string("zwp_relative_pointer_v1"),
            version = 1,
            method_count = 1,
            methods = relativePointerMethods,
            event_count = 1,
            events = relativePointerEvents
        };
        *constraints = new wl_interface
        {
            name = _glfwWaylandZwpPointerConstraintsV1,
            version = 1,
            method_count = 3,
            methods = constraintsMethods
        };
        *lockedPointer = new wl_interface
        {
            name = _glfw_allocate_static_string("zwp_locked_pointer_v1"),
            version = 1,
            method_count = 3,
            methods = lockedPointerMethods,
            event_count = 2,
            events = lockedPointerEvents
        };
        *confinedPointer = new wl_interface
        {
            name = _glfw_allocate_static_string("zwp_confined_pointer_v1"),
            version = 1,
            method_count = 2,
            methods = confinedPointerMethods,
            event_count = 2,
            events = confinedPointerEvents
        };

        _glfwWaylandZwpRelativePointerManagerV1Interface = relativeManager;
        _glfwWaylandZwpRelativePointerV1Interface = relativePointer;
        _glfwWaylandZwpPointerConstraintsV1Interface = constraints;
        _glfwWaylandZwpLockedPointerV1Interface = lockedPointer;
        _glfwWaylandZwpConfinedPointerV1Interface = confinedPointer;
        return GLFW_TRUE;
    }

    static int wayland_initViewporterInterfaces()
    {
        if (_glfwWaylandWpViewporterInterface != null)
            return GLFW_TRUE;

        var viewporter = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var viewport = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var scaleManager = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        var fractionalScale = (wl_interface*)_glfw_calloc(1, (nuint)sizeof(wl_interface));
        if (viewporter == null || viewport == null || scaleManager == null || fractionalScale == null)
            return GLFW_FALSE;

        var viewporterMethods = wayland_allocMessages(2);
        var viewportMethods = wayland_allocMessages(3);
        var scaleManagerMethods = wayland_allocMessages(2);
        var fractionalScaleMethods = wayland_allocMessages(1);
        var fractionalScaleEvents = wayland_allocMessages(1);
        if (viewporterMethods == null || viewportMethods == null ||
            scaleManagerMethods == null || fractionalScaleMethods == null ||
            fractionalScaleEvents == null)
        {
            return GLFW_FALSE;
        }

        var getViewportTypes = wayland_allocTypes(2);
        var getFractionalScaleTypes = wayland_allocTypes(2);
        if (getViewportTypes == null || getFractionalScaleTypes == null)
            return GLFW_FALSE;

        getViewportTypes[0] = viewport;
        getViewportTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;
        getFractionalScaleTypes[0] = fractionalScale;
        getFractionalScaleTypes[1] = (wl_interface*)_glfw.wl.client.surfaceInterface;

        wayland_setMessage(viewporterMethods, 0, "destroy", "", null);
        wayland_setMessage(viewporterMethods, 1, "get_viewport", "no", getViewportTypes);

        wayland_setMessage(viewportMethods, 0, "destroy", "", null);
        wayland_setMessage(viewportMethods, 1, "set_source", "ffff", null);
        wayland_setMessage(viewportMethods, 2, "set_destination", "ii", null);

        wayland_setMessage(scaleManagerMethods, 0, "destroy", "", null);
        wayland_setMessage(scaleManagerMethods, 1, "get_fractional_scale", "no", getFractionalScaleTypes);

        wayland_setMessage(fractionalScaleMethods, 0, "destroy", "", null);
        wayland_setMessage(fractionalScaleEvents, 0, "preferred_scale", "u", null);

        *viewporter = new wl_interface
        {
            name = _glfwWaylandWpViewporter,
            version = 1,
            method_count = 2,
            methods = viewporterMethods
        };
        *viewport = new wl_interface
        {
            name = _glfw_allocate_static_string("wp_viewport"),
            version = 1,
            method_count = 3,
            methods = viewportMethods
        };
        *scaleManager = new wl_interface
        {
            name = _glfwWaylandWpFractionalScaleManagerV1,
            version = 1,
            method_count = 2,
            methods = scaleManagerMethods
        };
        *fractionalScale = new wl_interface
        {
            name = _glfw_allocate_static_string("wp_fractional_scale_v1"),
            version = 1,
            method_count = 1,
            methods = fractionalScaleMethods,
            event_count = 1,
            events = fractionalScaleEvents
        };

        _glfwWaylandWpViewporterInterface = viewporter;
        _glfwWaylandWpViewportInterface = viewport;
        _glfwWaylandWpFractionalScaleManagerInterface = scaleManager;
        _glfwWaylandWpFractionalScaleInterface = fractionalScale;
        return GLFW_TRUE;
    }

    static int wayland_loadCursorTheme()
    {
        if (_glfw.wl.cursor.handle == null ||
            _glfw.wl.cursor.theme_load == null ||
            _glfw.wl.cursor.theme_destroy == null ||
            _glfw.wl.cursor.theme_get_cursor == null ||
            _glfw.wl.cursor.image_get_buffer == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-cursor entry point");
            return GLFW_FALSE;
        }

        var cursorSize = 16;
        var sizeString = Environment.GetEnvironmentVariable("XCURSOR_SIZE");
        if (int.TryParse(sizeString, out var parsedSize) && parsedSize > 0)
            cursorSize = parsedSize;

        var themeString = Environment.GetEnvironmentVariable("XCURSOR_THEME");
        var themeBytes = string.IsNullOrEmpty(themeString)
            ? null
            : Encoding.UTF8.GetBytes(themeString + '\0');

        fixed (byte* themeName = themeBytes)
        {
            _glfw.wl.cursorTheme = _glfw.wl.cursor.theme_load(themeName, cursorSize, _glfw.wl.shm);
            if (_glfw.wl.cursorTheme == null)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load default cursor theme");
                return GLFW_FALSE;
            }

            _glfw.wl.cursorThemeHiDPI = _glfw.wl.cursor.theme_load(themeName, cursorSize * 2, _glfw.wl.shm);
        }

        _glfw.wl.cursorSurface = wayland_compositorCreateSurface();
        if (_glfw.wl.cursorSurface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create cursor surface");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void wayland_createKeyTables()
    {
        const int KEY_ESC = 1;
        const int KEY_1 = 2;
        const int KEY_2 = 3;
        const int KEY_3 = 4;
        const int KEY_4 = 5;
        const int KEY_5 = 6;
        const int KEY_6 = 7;
        const int KEY_7 = 8;
        const int KEY_8 = 9;
        const int KEY_9 = 10;
        const int KEY_0 = 11;
        const int KEY_MINUS = 12;
        const int KEY_EQUAL = 13;
        const int KEY_BACKSPACE = 14;
        const int KEY_TAB = 15;
        const int KEY_Q = 16;
        const int KEY_W = 17;
        const int KEY_E = 18;
        const int KEY_R = 19;
        const int KEY_T = 20;
        const int KEY_Y = 21;
        const int KEY_U = 22;
        const int KEY_I = 23;
        const int KEY_O = 24;
        const int KEY_P = 25;
        const int KEY_LEFTBRACE = 26;
        const int KEY_RIGHTBRACE = 27;
        const int KEY_ENTER = 28;
        const int KEY_LEFTCTRL = 29;
        const int KEY_A = 30;
        const int KEY_S = 31;
        const int KEY_D = 32;
        const int KEY_F = 33;
        const int KEY_G = 34;
        const int KEY_H = 35;
        const int KEY_J = 36;
        const int KEY_K = 37;
        const int KEY_L = 38;
        const int KEY_SEMICOLON = 39;
        const int KEY_APOSTROPHE = 40;
        const int KEY_GRAVE = 41;
        const int KEY_LEFTSHIFT = 42;
        const int KEY_BACKSLASH = 43;
        const int KEY_Z = 44;
        const int KEY_X = 45;
        const int KEY_C = 46;
        const int KEY_V = 47;
        const int KEY_B = 48;
        const int KEY_N = 49;
        const int KEY_M = 50;
        const int KEY_COMMA = 51;
        const int KEY_DOT = 52;
        const int KEY_SLASH = 53;
        const int KEY_RIGHTSHIFT = 54;
        const int KEY_KPASTERISK = 55;
        const int KEY_LEFTALT = 56;
        const int KEY_SPACE = 57;
        const int KEY_CAPSLOCK = 58;
        const int KEY_F1 = 59;
        const int KEY_F2 = 60;
        const int KEY_F3 = 61;
        const int KEY_F4 = 62;
        const int KEY_F5 = 63;
        const int KEY_F6 = 64;
        const int KEY_F7 = 65;
        const int KEY_F8 = 66;
        const int KEY_F9 = 67;
        const int KEY_F10 = 68;
        const int KEY_NUMLOCK = 69;
        const int KEY_SCROLLLOCK = 70;
        const int KEY_KP7 = 71;
        const int KEY_KP8 = 72;
        const int KEY_KP9 = 73;
        const int KEY_KPMINUS = 74;
        const int KEY_KP4 = 75;
        const int KEY_KP5 = 76;
        const int KEY_KP6 = 77;
        const int KEY_KPPLUS = 78;
        const int KEY_KP1 = 79;
        const int KEY_KP2 = 80;
        const int KEY_KP3 = 81;
        const int KEY_KP0 = 82;
        const int KEY_KPDOT = 83;
        const int KEY_102ND = 86;
        const int KEY_F11 = 87;
        const int KEY_F12 = 88;
        const int KEY_KPENTER = 96;
        const int KEY_RIGHTCTRL = 97;
        const int KEY_KPSLASH = 98;
        const int KEY_PRINT = 99;
        const int KEY_RIGHTALT = 100;
        const int KEY_HOME = 102;
        const int KEY_UP = 103;
        const int KEY_PAGEUP = 104;
        const int KEY_LEFT = 105;
        const int KEY_RIGHT = 106;
        const int KEY_END = 107;
        const int KEY_DOWN = 108;
        const int KEY_PAGEDOWN = 109;
        const int KEY_INSERT = 110;
        const int KEY_DELETE = 111;
        const int KEY_KPEQUAL = 117;
        const int KEY_PAUSE = 119;
        const int KEY_LEFTMETA = 125;
        const int KEY_RIGHTMETA = 126;
        const int KEY_COMPOSE = 127;
        const int KEY_F13 = 183;
        const int KEY_F14 = 184;
        const int KEY_F15 = 185;
        const int KEY_F16 = 186;
        const int KEY_F17 = 187;
        const int KEY_F18 = 188;
        const int KEY_F19 = 189;
        const int KEY_F20 = 190;
        const int KEY_F21 = 191;
        const int KEY_F22 = 192;
        const int KEY_F23 = 193;
        const int KEY_F24 = 194;

        fixed (short* keycodes = _glfw.wl.keycodes)
        fixed (short* scancodes = _glfw.wl.scancodes)
        {
            _glfw_memset(keycodes, 0xff, 256 * (nuint)sizeof(short));
            _glfw_memset(scancodes, 0xff, (GLFW_KEY_LAST + 1) * (nuint)sizeof(short));

            keycodes[KEY_GRAVE] = GLFW_KEY_GRAVE_ACCENT;
            keycodes[KEY_1] = GLFW_KEY_1;
            keycodes[KEY_2] = GLFW_KEY_2;
            keycodes[KEY_3] = GLFW_KEY_3;
            keycodes[KEY_4] = GLFW_KEY_4;
            keycodes[KEY_5] = GLFW_KEY_5;
            keycodes[KEY_6] = GLFW_KEY_6;
            keycodes[KEY_7] = GLFW_KEY_7;
            keycodes[KEY_8] = GLFW_KEY_8;
            keycodes[KEY_9] = GLFW_KEY_9;
            keycodes[KEY_0] = GLFW_KEY_0;
            keycodes[KEY_SPACE] = GLFW_KEY_SPACE;
            keycodes[KEY_MINUS] = GLFW_KEY_MINUS;
            keycodes[KEY_EQUAL] = GLFW_KEY_EQUAL;
            keycodes[KEY_Q] = GLFW_KEY_Q;
            keycodes[KEY_W] = GLFW_KEY_W;
            keycodes[KEY_E] = GLFW_KEY_E;
            keycodes[KEY_R] = GLFW_KEY_R;
            keycodes[KEY_T] = GLFW_KEY_T;
            keycodes[KEY_Y] = GLFW_KEY_Y;
            keycodes[KEY_U] = GLFW_KEY_U;
            keycodes[KEY_I] = GLFW_KEY_I;
            keycodes[KEY_O] = GLFW_KEY_O;
            keycodes[KEY_P] = GLFW_KEY_P;
            keycodes[KEY_LEFTBRACE] = GLFW_KEY_LEFT_BRACKET;
            keycodes[KEY_RIGHTBRACE] = GLFW_KEY_RIGHT_BRACKET;
            keycodes[KEY_A] = GLFW_KEY_A;
            keycodes[KEY_S] = GLFW_KEY_S;
            keycodes[KEY_D] = GLFW_KEY_D;
            keycodes[KEY_F] = GLFW_KEY_F;
            keycodes[KEY_G] = GLFW_KEY_G;
            keycodes[KEY_H] = GLFW_KEY_H;
            keycodes[KEY_J] = GLFW_KEY_J;
            keycodes[KEY_K] = GLFW_KEY_K;
            keycodes[KEY_L] = GLFW_KEY_L;
            keycodes[KEY_SEMICOLON] = GLFW_KEY_SEMICOLON;
            keycodes[KEY_APOSTROPHE] = GLFW_KEY_APOSTROPHE;
            keycodes[KEY_Z] = GLFW_KEY_Z;
            keycodes[KEY_X] = GLFW_KEY_X;
            keycodes[KEY_C] = GLFW_KEY_C;
            keycodes[KEY_V] = GLFW_KEY_V;
            keycodes[KEY_B] = GLFW_KEY_B;
            keycodes[KEY_N] = GLFW_KEY_N;
            keycodes[KEY_M] = GLFW_KEY_M;
            keycodes[KEY_COMMA] = GLFW_KEY_COMMA;
            keycodes[KEY_DOT] = GLFW_KEY_PERIOD;
            keycodes[KEY_SLASH] = GLFW_KEY_SLASH;
            keycodes[KEY_BACKSLASH] = GLFW_KEY_BACKSLASH;
            keycodes[KEY_ESC] = GLFW_KEY_ESCAPE;
            keycodes[KEY_TAB] = GLFW_KEY_TAB;
            keycodes[KEY_LEFTSHIFT] = GLFW_KEY_LEFT_SHIFT;
            keycodes[KEY_RIGHTSHIFT] = GLFW_KEY_RIGHT_SHIFT;
            keycodes[KEY_LEFTCTRL] = GLFW_KEY_LEFT_CONTROL;
            keycodes[KEY_RIGHTCTRL] = GLFW_KEY_RIGHT_CONTROL;
            keycodes[KEY_LEFTALT] = GLFW_KEY_LEFT_ALT;
            keycodes[KEY_RIGHTALT] = GLFW_KEY_RIGHT_ALT;
            keycodes[KEY_LEFTMETA] = GLFW_KEY_LEFT_SUPER;
            keycodes[KEY_RIGHTMETA] = GLFW_KEY_RIGHT_SUPER;
            keycodes[KEY_COMPOSE] = GLFW_KEY_MENU;
            keycodes[KEY_NUMLOCK] = GLFW_KEY_NUM_LOCK;
            keycodes[KEY_CAPSLOCK] = GLFW_KEY_CAPS_LOCK;
            keycodes[KEY_PRINT] = GLFW_KEY_PRINT_SCREEN;
            keycodes[KEY_SCROLLLOCK] = GLFW_KEY_SCROLL_LOCK;
            keycodes[KEY_PAUSE] = GLFW_KEY_PAUSE;
            keycodes[KEY_DELETE] = GLFW_KEY_DELETE;
            keycodes[KEY_BACKSPACE] = GLFW_KEY_BACKSPACE;
            keycodes[KEY_ENTER] = GLFW_KEY_ENTER;
            keycodes[KEY_HOME] = GLFW_KEY_HOME;
            keycodes[KEY_END] = GLFW_KEY_END;
            keycodes[KEY_PAGEUP] = GLFW_KEY_PAGE_UP;
            keycodes[KEY_PAGEDOWN] = GLFW_KEY_PAGE_DOWN;
            keycodes[KEY_INSERT] = GLFW_KEY_INSERT;
            keycodes[KEY_LEFT] = GLFW_KEY_LEFT;
            keycodes[KEY_RIGHT] = GLFW_KEY_RIGHT;
            keycodes[KEY_DOWN] = GLFW_KEY_DOWN;
            keycodes[KEY_UP] = GLFW_KEY_UP;
            keycodes[KEY_F1] = GLFW_KEY_F1;
            keycodes[KEY_F2] = GLFW_KEY_F2;
            keycodes[KEY_F3] = GLFW_KEY_F3;
            keycodes[KEY_F4] = GLFW_KEY_F4;
            keycodes[KEY_F5] = GLFW_KEY_F5;
            keycodes[KEY_F6] = GLFW_KEY_F6;
            keycodes[KEY_F7] = GLFW_KEY_F7;
            keycodes[KEY_F8] = GLFW_KEY_F8;
            keycodes[KEY_F9] = GLFW_KEY_F9;
            keycodes[KEY_F10] = GLFW_KEY_F10;
            keycodes[KEY_F11] = GLFW_KEY_F11;
            keycodes[KEY_F12] = GLFW_KEY_F12;
            keycodes[KEY_F13] = GLFW_KEY_F13;
            keycodes[KEY_F14] = GLFW_KEY_F14;
            keycodes[KEY_F15] = GLFW_KEY_F15;
            keycodes[KEY_F16] = GLFW_KEY_F16;
            keycodes[KEY_F17] = GLFW_KEY_F17;
            keycodes[KEY_F18] = GLFW_KEY_F18;
            keycodes[KEY_F19] = GLFW_KEY_F19;
            keycodes[KEY_F20] = GLFW_KEY_F20;
            keycodes[KEY_F21] = GLFW_KEY_F21;
            keycodes[KEY_F22] = GLFW_KEY_F22;
            keycodes[KEY_F23] = GLFW_KEY_F23;
            keycodes[KEY_F24] = GLFW_KEY_F24;
            keycodes[KEY_KPSLASH] = GLFW_KEY_KP_DIVIDE;
            keycodes[KEY_KPASTERISK] = GLFW_KEY_KP_MULTIPLY;
            keycodes[KEY_KPMINUS] = GLFW_KEY_KP_SUBTRACT;
            keycodes[KEY_KPPLUS] = GLFW_KEY_KP_ADD;
            keycodes[KEY_KP0] = GLFW_KEY_KP_0;
            keycodes[KEY_KP1] = GLFW_KEY_KP_1;
            keycodes[KEY_KP2] = GLFW_KEY_KP_2;
            keycodes[KEY_KP3] = GLFW_KEY_KP_3;
            keycodes[KEY_KP4] = GLFW_KEY_KP_4;
            keycodes[KEY_KP5] = GLFW_KEY_KP_5;
            keycodes[KEY_KP6] = GLFW_KEY_KP_6;
            keycodes[KEY_KP7] = GLFW_KEY_KP_7;
            keycodes[KEY_KP8] = GLFW_KEY_KP_8;
            keycodes[KEY_KP9] = GLFW_KEY_KP_9;
            keycodes[KEY_KPDOT] = GLFW_KEY_KP_DECIMAL;
            keycodes[KEY_KPEQUAL] = GLFW_KEY_KP_EQUAL;
            keycodes[KEY_KPENTER] = GLFW_KEY_KP_ENTER;
            keycodes[KEY_102ND] = GLFW_KEY_WORLD_2;

            for (var scancode = 0; scancode < 256; scancode++)
            {
                if (keycodes[scancode] > 0)
                    scancodes[keycodes[scancode]] = (short)scancode;
            }
        }
    }

    static int wayland_loadXkb()
    {
        _glfw.wl.xkb.handle = wayland_loadModule("libxkbcommon.so.0");
        if (_glfw.wl.xkb.handle == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libxkbcommon");
            return GLFW_FALSE;
        }

        _glfw.wl.xkb.context_new =
            (delegate* unmanaged<int, void*>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_context_new");
        _glfw.wl.xkb.context_unref =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_context_unref");
        _glfw.wl.xkb.keymap_new_from_string =
            (delegate* unmanaged<void*, byte*, int, int, void*>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_keymap_new_from_string");
        _glfw.wl.xkb.keymap_unref =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_keymap_unref");
        _glfw.wl.xkb.keymap_mod_get_index =
            (delegate* unmanaged<void*, byte*, uint>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_keymap_mod_get_index");
        _glfw.wl.xkb.keymap_key_repeats =
            (delegate* unmanaged<void*, uint, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_keymap_key_repeats");
        _glfw.wl.xkb.keymap_key_get_syms_by_level =
            (delegate* unmanaged<void*, uint, uint, uint, uint**, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_keymap_key_get_syms_by_level");
        _glfw.wl.xkb.state_new =
            (delegate* unmanaged<void*, void*>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_new");
        _glfw.wl.xkb.state_unref =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_unref");
        _glfw.wl.xkb.state_key_get_syms =
            (delegate* unmanaged<void*, uint, uint**, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_key_get_syms");
        _glfw.wl.xkb.state_update_mask =
            (delegate* unmanaged<void*, uint, uint, uint, uint, uint, uint, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_update_mask");
        _glfw.wl.xkb.state_key_get_layout =
            (delegate* unmanaged<void*, uint, uint>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_key_get_layout");
        _glfw.wl.xkb.state_mod_index_is_active =
            (delegate* unmanaged<void*, uint, int, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_state_mod_index_is_active");
        _glfw.wl.xkb.compose_table_new_from_locale =
            (delegate* unmanaged<void*, byte*, int, void*>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_table_new_from_locale");
        _glfw.wl.xkb.compose_table_unref =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_table_unref");
        _glfw.wl.xkb.compose_state_new =
            (delegate* unmanaged<void*, int, void*>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_state_new");
        _glfw.wl.xkb.compose_state_unref =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_state_unref");
        _glfw.wl.xkb.compose_state_feed =
            (delegate* unmanaged<void*, uint, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_state_feed");
        _glfw.wl.xkb.compose_state_get_status =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_state_get_status");
        _glfw.wl.xkb.compose_state_get_one_sym =
            (delegate* unmanaged<void*, uint>)wayland_getModuleSymbol(_glfw.wl.xkb.handle, "xkb_compose_state_get_one_sym");

        if (_glfw.wl.xkb.context_new == null ||
            _glfw.wl.xkb.context_unref == null ||
            _glfw.wl.xkb.keymap_new_from_string == null ||
            _glfw.wl.xkb.keymap_unref == null ||
            _glfw.wl.xkb.keymap_mod_get_index == null ||
            _glfw.wl.xkb.keymap_key_repeats == null ||
            _glfw.wl.xkb.keymap_key_get_syms_by_level == null ||
            _glfw.wl.xkb.state_new == null ||
            _glfw.wl.xkb.state_unref == null ||
            _glfw.wl.xkb.state_key_get_syms == null ||
            _glfw.wl.xkb.state_update_mask == null ||
            _glfw.wl.xkb.state_key_get_layout == null ||
            _glfw.wl.xkb.state_mod_index_is_active == null ||
            _glfw.wl.xkb.compose_table_new_from_locale == null ||
            _glfw.wl.xkb.compose_table_unref == null ||
            _glfw.wl.xkb.compose_state_new == null ||
            _glfw.wl.xkb.compose_state_unref == null ||
            _glfw.wl.xkb.compose_state_feed == null ||
            _glfw.wl.xkb.compose_state_get_status == null ||
            _glfw.wl.xkb.compose_state_get_one_sym == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load all entry points from libxkbcommon");
            return GLFW_FALSE;
        }

        wayland_createKeyTables();

        _glfw.wl.xkb.context = _glfw.wl.xkb.context_new(0);
        if (_glfw.wl.xkb.context == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize xkb context");
            return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static wl_registry_listener* wayland_getRegistryListener()
    {
        if (_glfwWaylandRegistryListener == null)
        {
            _glfwWaylandRegistryListener = (wl_registry_listener*)_glfw_calloc(1, (nuint)sizeof(wl_registry_listener));
            if (_glfwWaylandRegistryListener != null)
            {
                _glfwWaylandRegistryListener->global = &wayland_registryHandleGlobal;
                _glfwWaylandRegistryListener->global_remove = &wayland_registryHandleGlobalRemove;
            }
        }

        return _glfwWaylandRegistryListener;
    }

    static xdg_wm_base_listener* wayland_getXdgWmBaseListener()
    {
        if (_glfwWaylandXdgWmBaseListener == null)
        {
            _glfwWaylandXdgWmBaseListener = (xdg_wm_base_listener*)_glfw_calloc(1, (nuint)sizeof(xdg_wm_base_listener));
            if (_glfwWaylandXdgWmBaseListener != null)
                _glfwWaylandXdgWmBaseListener->ping = &wayland_xdgWmBaseHandlePing;
        }

        return _glfwWaylandXdgWmBaseListener;
    }

    static void wayland_tagProxy(void* proxy)
    {
        if (proxy == null || _glfw.wl.client.proxy_set_tag == null)
            return;

        fixed (_GLFWlibrary* glfw = &_glfw)
            _glfw.wl.client.proxy_set_tag(proxy, &glfw->wl.tag);
    }

    static void* wayland_displayGetRegistry(void* display)
    {
        if (display == null || _glfw.wl.client.proxy_marshal_constructor == null)
            return null;

        return _glfw.wl.client.proxy_marshal_constructor(display,
            WL_DISPLAY_GET_REGISTRY,
            _glfw.wl.client.registryInterface,
            null);
    }

    static void* wayland_registryBind(void* registry,
                                      uint name,
                                      void* interfacePtr,
                                      byte* interfaceName,
                                      uint version)
    {
        if (registry == null ||
            interfacePtr == null ||
            interfaceName == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind == null)
        {
            return null;
        }

        var proxy = _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind(registry,
            WL_REGISTRY_BIND,
            interfacePtr,
            version,
            name,
            interfaceName,
            version,
            null);

        wayland_tagProxy(proxy);
        return proxy;
    }

    static void* wayland_compositorCreateSurface()
    {
        if (_glfw.wl.compositor == null || _glfw.wl.client.surfaceInterface == null)
            return null;

        var surface = _glfw.wl.client.proxy_marshal_constructor(_glfw.wl.compositor,
            WL_COMPOSITOR_CREATE_SURFACE,
            _glfw.wl.client.surfaceInterface,
            null);

        wayland_tagProxy(surface);
        return surface;
    }

    static void wayland_proxyDestroyWithOpcode(void* proxy, uint opcode)
    {
        if (proxy == null)
            return;

        if (_glfw.wl.client.proxy_marshal_flags != null && _glfw.wl.client.proxy_get_version != null)
        {
            _glfw.wl.client.proxy_marshal_flags(proxy,
                opcode,
                null,
                _glfw.wl.client.proxy_get_version(proxy),
                WL_MARSHAL_FLAG_DESTROY);
            return;
        }

        if (_glfw.wl.client.proxy_marshal != null && _glfw.wl.client.proxy_destroy != null)
        {
            _glfw.wl.client.proxy_marshal(proxy, opcode);
            _glfw.wl.client.proxy_destroy(proxy);
            return;
        }

        if (_glfw.wl.client.proxy_destroy != null)
            _glfw.wl.client.proxy_destroy(proxy);
    }

    static void wayland_proxyDestroy(void* proxy)
    {
        if (proxy != null && _glfw.wl.client.proxy_destroy != null)
            _glfw.wl.client.proxy_destroy(proxy);
    }

    static void wayland_xdgWmBasePong(void* wmBase, uint serial)
    {
        if (wmBase != null && _glfw.wl.client.proxy_marshal_uint != null)
            _glfw.wl.client.proxy_marshal_uint(wmBase, XDG_WM_BASE_PONG, serial);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgWmBaseHandlePing(void* userData, void* wmBase, uint serial)
    {
        wayland_xdgWmBasePong(wmBase, serial);
    }

    [UnmanagedCallersOnly]
    static void wayland_registryHandleGlobal(void* userData,
                                             void* registry,
                                             uint name,
                                             byte* interfaceName,
                                             uint version)
    {
        if (wayland_stringEquals(interfaceName, "wl_compositor") != 0)
        {
            if (_glfw.wl.compositor == null)
            {
                _glfw.wl.compositor = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.compositorInterface,
                    _glfwWaylandWlCompositor,
                    wayland_min(version, 3));
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_subcompositor") != 0)
        {
            if (_glfw.wl.subcompositor == null)
            {
                _glfw.wl.subcompositor = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.subcompositorInterface,
                    _glfwWaylandWlSubcompositor,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_shm") != 0)
        {
            if (_glfw.wl.shm == null)
            {
                _glfw.wl.shm = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.shmInterface,
                    _glfwWaylandWlShm,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_output") != 0)
        {
            _glfwAddOutputWayland(name, version);
        }
        else if (wayland_stringEquals(interfaceName, "wl_seat") != 0)
        {
            if (_glfw.wl.seat == null)
            {
                _glfw.wl.seat = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.seatInterface,
                    _glfwWaylandWlSeat,
                    wayland_min(version, 4));
                _glfwAddSeatListenerWayland(_glfw.wl.seat);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wl_data_device_manager") != 0)
        {
            if (_glfw.wl.dataDeviceManager == null)
            {
                _glfw.wl.dataDeviceManager = wayland_registryBind(registry,
                    name,
                    _glfw.wl.client.dataDeviceManagerInterface,
                    _glfwWaylandWlDataDeviceManager,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "xdg_wm_base") != 0)
        {
            if (_glfw.wl.wmBase == null && _glfwWaylandXdgWmBaseInterface != null)
            {
                _glfw.wl.wmBase = wayland_registryBind(registry,
                    name,
                    _glfwWaylandXdgWmBaseInterface,
                    _glfwWaylandXdgWmBase,
                    1);

                var listener = wayland_getXdgWmBaseListener();
                if (_glfw.wl.wmBase != null && listener != null)
                    _glfw.wl.client.proxy_add_listener(_glfw.wl.wmBase, listener, null);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wp_viewporter") != 0)
        {
            if (_glfw.wl.viewporter == null && _glfwWaylandWpViewporterInterface != null)
            {
                _glfw.wl.viewporter = wayland_registryBind(registry,
                    name,
                    _glfwWaylandWpViewporterInterface,
                    _glfwWaylandWpViewporter,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "zxdg_decoration_manager_v1") != 0)
        {
            if (_glfw.wl.decorationManager == null && _glfwWaylandZxdgDecorationManagerV1Interface != null)
            {
                _glfw.wl.decorationManager = wayland_registryBind(registry,
                    name,
                    _glfwWaylandZxdgDecorationManagerV1Interface,
                    _glfwWaylandZxdgDecorationManagerV1,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "zwp_idle_inhibit_manager_v1") != 0)
        {
            if (_glfw.wl.idleInhibitManager == null && _glfwWaylandZwpIdleInhibitManagerV1Interface != null)
            {
                _glfw.wl.idleInhibitManager = wayland_registryBind(registry,
                    name,
                    _glfwWaylandZwpIdleInhibitManagerV1Interface,
                    _glfwWaylandZwpIdleInhibitManagerV1,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "xdg_activation_v1") != 0)
        {
            if (_glfw.wl.activationManager == null && _glfwWaylandXdgActivationV1Interface != null)
            {
                _glfw.wl.activationManager = wayland_registryBind(registry,
                    name,
                    _glfwWaylandXdgActivationV1Interface,
                    _glfwWaylandXdgActivationV1,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "zwp_relative_pointer_manager_v1") != 0)
        {
            if (_glfw.wl.relativePointerManager == null && _glfwWaylandZwpRelativePointerManagerV1Interface != null)
            {
                _glfw.wl.relativePointerManager = wayland_registryBind(registry,
                    name,
                    _glfwWaylandZwpRelativePointerManagerV1Interface,
                    _glfwWaylandZwpRelativePointerManagerV1,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "zwp_pointer_constraints_v1") != 0)
        {
            if (_glfw.wl.pointerConstraints == null && _glfwWaylandZwpPointerConstraintsV1Interface != null)
            {
                _glfw.wl.pointerConstraints = wayland_registryBind(registry,
                    name,
                    _glfwWaylandZwpPointerConstraintsV1Interface,
                    _glfwWaylandZwpPointerConstraintsV1,
                    1);
            }
        }
        else if (wayland_stringEquals(interfaceName, "wp_fractional_scale_manager_v1") != 0)
        {
            if (_glfw.wl.fractionalScaleManager == null && _glfwWaylandWpFractionalScaleManagerInterface != null)
            {
                _glfw.wl.fractionalScaleManager = wayland_registryBind(registry,
                    name,
                    _glfwWaylandWpFractionalScaleManagerInterface,
                    _glfwWaylandWpFractionalScaleManagerV1,
                    1);
            }
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_registryHandleGlobalRemove(void* userData, void* registry, uint name)
    {
        for (var i = 0; i < _glfw.monitorCount; i++)
        {
            var monitor = _glfw.monitors[i];
            if (monitor->wl.name == name)
            {
                _glfwInputMonitor(monitor, GLFW_DISCONNECTED, 0);
                return;
            }
        }
    }

    static int _glfwConnectWayland(int platformID, _GLFWplatform* platform)
    {
        if (!OperatingSystem.IsLinux())
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not available on this system");
            return GLFW_FALSE;
        }

        if (platformID != GLFW_PLATFORM_WAYLAND &&
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return GLFW_FALSE;
        }

        var module = wayland_loadModule("libwayland-client.so.0");
        if (module == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client");
            return GLFW_FALSE;
        }

        var wl_display_connect =
            (delegate* unmanaged<byte*, void*>)wayland_getModuleSymbol(module, "wl_display_connect");
        if (wl_display_connect == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client entry point");
            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        var display = wl_display_connect(null);
        if (display == null)
        {
            if (platformID == GLFW_PLATFORM_WAYLAND)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to connect to display");
            _glfwPlatformFreeModule(module);
            return GLFW_FALSE;
        }

        _glfw.wl.display = display;
        _glfw.wl.client.handle = module;
        _glfw.wl.client.display_connect = wl_display_connect;
        wayland_setPlatformCallbacks(platform);
        return GLFW_TRUE;
    }

    static int _glfwInitWayland()
    {
        _glfw.wl.keyRepeatTimerfd = -1;
        _glfw.wl.cursorTimerfd = -1;
        _glfw.wl.tag = glfwGetVersionString();

        _glfw.wl.client.display_flush =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_flush");
        _glfw.wl.client.display_cancel_read =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_cancel_read");
        _glfw.wl.client.display_dispatch_pending =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_dispatch_pending");
        _glfw.wl.client.display_read_events =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_read_events");
        _glfw.wl.client.display_disconnect =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_disconnect");
        _glfw.wl.client.display_roundtrip =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_roundtrip");
        _glfw.wl.client.display_get_fd =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_get_fd");
        _glfw.wl.client.display_prepare_read =
            (delegate* unmanaged<void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_display_prepare_read");
        _glfw.wl.client.proxy_marshal =
            (delegate* unmanaged<void*, uint, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_string =
            (delegate* unmanaged<void*, uint, byte*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_uint =
            (delegate* unmanaged<void*, uint, uint, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_uint_object =
            (delegate* unmanaged<void*, uint, uint, void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_uint_string =
            (delegate* unmanaged<void*, uint, uint, byte*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_int =
            (delegate* unmanaged<void*, uint, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_string_int =
            (delegate* unmanaged<void*, uint, byte*, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_string_object =
            (delegate* unmanaged<void*, uint, byte*, void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_object =
            (delegate* unmanaged<void*, uint, void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_object_uint =
            (delegate* unmanaged<void*, uint, void*, uint, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_object_int_int =
            (delegate* unmanaged<void*, uint, void*, int, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_uint_object_int_int =
            (delegate* unmanaged<void*, uint, uint, void*, int, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_int_int =
            (delegate* unmanaged<void*, uint, int, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_marshal_int_int_int_int =
            (delegate* unmanaged<void*, uint, int, int, int, int, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal");
        _glfw.wl.client.proxy_add_listener =
            (delegate* unmanaged<void*, void*, void*, int>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_add_listener");
        _glfw.wl.client.proxy_destroy =
            (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_destroy");
        _glfw.wl.client.proxy_marshal_constructor =
            (delegate* unmanaged<void*, uint, void*, void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_object =
            (delegate* unmanaged<void*, uint, void*, void*, void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_object_object_object_uint =
            (delegate* unmanaged<void*, uint, void*, void*, void*, void*, void*, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_int_int =
            (delegate* unmanaged<void*, uint, void*, void*, int, int, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_int_int_int_int_uint =
            (delegate* unmanaged<void*, uint, void*, void*, int, int, int, int, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor");
        _glfw.wl.client.proxy_marshal_constructor_versioned =
            (delegate* unmanaged<void*, uint, void*, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor_versioned");
        _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind =
            (delegate* unmanaged<void*, uint, void*, uint, uint, byte*, uint, void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_constructor_versioned");
        _glfw.wl.client.proxy_get_user_data =
            (delegate* unmanaged<void*, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_user_data");
        _glfw.wl.client.proxy_set_user_data =
            (delegate* unmanaged<void*, void*, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_set_user_data");
        _glfw.wl.client.proxy_get_tag =
            (delegate* unmanaged<void*, byte**>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_tag");
        _glfw.wl.client.proxy_set_tag =
            (delegate* unmanaged<void*, byte**, void>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_set_tag");
        _glfw.wl.client.proxy_get_version =
            (delegate* unmanaged<void*, uint>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_get_version");
        _glfw.wl.client.proxy_marshal_flags =
            (delegate* unmanaged<void*, uint, void*, uint, uint, void*>)wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_proxy_marshal_flags");
        _glfw.wl.client.callbackInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_callback_interface");
        _glfw.wl.client.registryInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_registry_interface");
        _glfw.wl.client.compositorInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_compositor_interface");
        _glfw.wl.client.subcompositorInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_subcompositor_interface");
        _glfw.wl.client.shmInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_shm_interface");
        _glfw.wl.client.shmPoolInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_shm_pool_interface");
        _glfw.wl.client.bufferInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_buffer_interface");
        _glfw.wl.client.regionInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_region_interface");
        _glfw.wl.client.seatInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_seat_interface");
        _glfw.wl.client.pointerInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_pointer_interface");
        _glfw.wl.client.keyboardInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_keyboard_interface");
        _glfw.wl.client.outputInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_output_interface");
        _glfw.wl.client.dataDeviceManagerInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_data_device_manager_interface");
        _glfw.wl.client.dataDeviceInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_data_device_interface");
        _glfw.wl.client.dataOfferInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_data_offer_interface");
        _glfw.wl.client.dataSourceInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_data_source_interface");
        _glfw.wl.client.surfaceInterface = wayland_getModuleSymbol(_glfw.wl.client.handle, "wl_surface_interface");

        if (_glfw.wl.client.display_flush == null ||
            _glfw.wl.client.display_cancel_read == null ||
            _glfw.wl.client.display_dispatch_pending == null ||
            _glfw.wl.client.display_read_events == null ||
            _glfw.wl.client.display_disconnect == null ||
            _glfw.wl.client.display_roundtrip == null ||
            _glfw.wl.client.display_get_fd == null ||
            _glfw.wl.client.display_prepare_read == null ||
            _glfw.wl.client.proxy_marshal == null ||
            _glfw.wl.client.proxy_marshal_string == null ||
            _glfw.wl.client.proxy_marshal_uint == null ||
            _glfw.wl.client.proxy_marshal_uint_object == null ||
            _glfw.wl.client.proxy_marshal_uint_string == null ||
            _glfw.wl.client.proxy_marshal_int == null ||
            _glfw.wl.client.proxy_marshal_string_int == null ||
            _glfw.wl.client.proxy_marshal_string_object == null ||
            _glfw.wl.client.proxy_marshal_object == null ||
            _glfw.wl.client.proxy_marshal_object_uint == null ||
            _glfw.wl.client.proxy_marshal_object_int_int == null ||
            _glfw.wl.client.proxy_marshal_uint_object_int_int == null ||
            _glfw.wl.client.proxy_marshal_int_int == null ||
            _glfw.wl.client.proxy_marshal_int_int_int_int == null ||
            _glfw.wl.client.proxy_add_listener == null ||
            _glfw.wl.client.proxy_destroy == null ||
            _glfw.wl.client.proxy_marshal_constructor == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null ||
            _glfw.wl.client.proxy_marshal_constructor_object_object_object_uint == null ||
            _glfw.wl.client.proxy_marshal_constructor_int_int == null ||
            _glfw.wl.client.proxy_marshal_constructor_int_int_int_int_uint == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned == null ||
            _glfw.wl.client.proxy_marshal_constructor_versioned_registry_bind == null ||
            _glfw.wl.client.proxy_get_user_data == null ||
            _glfw.wl.client.proxy_set_user_data == null ||
            _glfw.wl.client.proxy_get_tag == null ||
            _glfw.wl.client.proxy_set_tag == null ||
            _glfw.wl.client.callbackInterface == null ||
            _glfw.wl.client.registryInterface == null ||
            _glfw.wl.client.compositorInterface == null ||
            _glfw.wl.client.subcompositorInterface == null ||
            _glfw.wl.client.shmInterface == null ||
            _glfw.wl.client.shmPoolInterface == null ||
            _glfw.wl.client.bufferInterface == null ||
            _glfw.wl.client.regionInterface == null ||
            _glfw.wl.client.seatInterface == null ||
            _glfw.wl.client.pointerInterface == null ||
            _glfw.wl.client.keyboardInterface == null ||
            _glfw.wl.client.outputInterface == null ||
            _glfw.wl.client.dataDeviceManagerInterface == null ||
            _glfw.wl.client.dataDeviceInterface == null ||
            _glfw.wl.client.dataOfferInterface == null ||
            _glfw.wl.client.dataSourceInterface == null ||
            _glfw.wl.client.surfaceInterface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-client entry point");
            return GLFW_FALSE;
        }

        _glfw.wl.cursor.handle = wayland_loadModule("libwayland-cursor.so.0");
        if (_glfw.wl.cursor.handle != null)
        {
            _glfw.wl.cursor.theme_load =
                (delegate* unmanaged<byte*, int, void*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_load");
            _glfw.wl.cursor.theme_destroy =
                (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_destroy");
            _glfw.wl.cursor.theme_get_cursor =
                (delegate* unmanaged<void*, byte*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_theme_get_cursor");
            _glfw.wl.cursor.image_get_buffer =
                (delegate* unmanaged<void*, void*>)wayland_getModuleSymbol(_glfw.wl.cursor.handle, "wl_cursor_image_get_buffer");
        }

        _glfw.wl.egl.handle = wayland_loadModule("libwayland-egl.so.1");
        if (_glfw.wl.egl.handle != null)
        {
            _glfw.wl.egl.window_create =
                (delegate* unmanaged<void*, int, int, void*>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_create");
            _glfw.wl.egl.window_destroy =
                (delegate* unmanaged<void*, void>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_destroy");
            _glfw.wl.egl.window_resize =
                (delegate* unmanaged<void*, int, int, int, int, void>)wayland_getModuleSymbol(_glfw.wl.egl.handle, "wl_egl_window_resize");
        }

        if (wayland_loadXkb() == 0)
            return GLFW_FALSE;

        if (wayland_initXdgShellInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize xdg-shell protocol tables");
            return GLFW_FALSE;
        }
        if (wayland_initXdgDecorationInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize xdg-decoration protocol tables");
            return GLFW_FALSE;
        }
        if (wayland_initIdleInhibitInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize idle-inhibit protocol tables");
            return GLFW_FALSE;
        }
        if (wayland_initXdgActivationInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize xdg-activation protocol tables");
            return GLFW_FALSE;
        }
        if (wayland_initPointerConstraintInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize pointer constraint protocol tables");
            return GLFW_FALSE;
        }
        if (wayland_initViewporterInterfaces() == 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to initialize viewporter protocol tables");
            return GLFW_FALSE;
        }

        _glfw.wl.registry = wayland_displayGetRegistry(_glfw.wl.display);
        if (_glfw.wl.registry == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create display registry");
            return GLFW_FALSE;
        }

        var registryListener = wayland_getRegistryListener();
        if (registryListener == null ||
            _glfw.wl.client.proxy_add_listener(_glfw.wl.registry, registryListener, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add registry listener");
            return GLFW_FALSE;
        }

        if (_glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0 ||
            _glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to process initial registry events");
            return GLFW_FALSE;
        }

        if (_glfw.wl.seat != null &&
            _glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(_glfw.wl.seat) >= WL_KEYBOARD_REPEAT_INFO_SINCE_VERSION)
        {
            _glfw.wl.keyRepeatTimerfd = wayland_timerfd_create(CLOCK_MONOTONIC,
                TFD_CLOEXEC | TFD_NONBLOCK);
        }

        if (_glfw.wl.compositor == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to find wl_compositor in your compositor");
            return GLFW_FALSE;
        }

        if (_glfw.wl.wmBase == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to find xdg-shell in your compositor");
            return GLFW_FALSE;
        }

        if (_glfw.wl.shm == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to find wl_shm in your compositor");
            return GLFW_FALSE;
        }

        if (wayland_loadCursorTheme() == 0)
            return GLFW_FALSE;

        _glfw.wl.cursorTimerfd = wayland_timerfd_create(CLOCK_MONOTONIC,
            TFD_CLOEXEC | TFD_NONBLOCK);

        if (_glfw.wl.seat != null && _glfw.wl.dataDeviceManager != null)
        {
            _glfw.wl.dataDevice = wayland_dataDeviceManagerGetDataDevice(_glfw.wl.dataDeviceManager, _glfw.wl.seat);
            _glfwAddDataDeviceListenerWayland(_glfw.wl.dataDevice);
        }

        return GLFW_TRUE;
    }

    static void _glfwTerminateWayland()
    {
        _glfwTerminateEGL();
        _glfwTerminateOSMesa();

        if (_glfw.wl.cursorTheme != null && _glfw.wl.cursor.theme_destroy != null)
            _glfw.wl.cursor.theme_destroy(_glfw.wl.cursorTheme);
        if (_glfw.wl.cursorThemeHiDPI != null && _glfw.wl.cursor.theme_destroy != null)
            _glfw.wl.cursor.theme_destroy(_glfw.wl.cursorThemeHiDPI);
        if (_glfw.wl.cursor.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.cursor.handle);
        if (_glfw.wl.egl.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.egl.handle);
        if (_glfw.wl.xkb.composeState != null && _glfw.wl.xkb.compose_state_unref != null)
            _glfw.wl.xkb.compose_state_unref(_glfw.wl.xkb.composeState);
        if (_glfw.wl.xkb.keymap != null && _glfw.wl.xkb.keymap_unref != null)
            _glfw.wl.xkb.keymap_unref(_glfw.wl.xkb.keymap);
        if (_glfw.wl.xkb.state != null && _glfw.wl.xkb.state_unref != null)
            _glfw.wl.xkb.state_unref(_glfw.wl.xkb.state);
        if (_glfw.wl.xkb.context != null && _glfw.wl.xkb.context_unref != null)
            _glfw.wl.xkb.context_unref(_glfw.wl.xkb.context);
        if (_glfw.wl.xkb.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.xkb.handle);

        for (uint i = 0; i < _glfw.wl.offerCount; i++)
            wayland_dataOfferDestroy(_glfw.wl.offers[i].offer);
        if (_glfw.wl.selectionOffer != null)
            wayland_dataOfferDestroy(_glfw.wl.selectionOffer);
        if (_glfw.wl.dragOffer != null)
            wayland_dataOfferDestroy(_glfw.wl.dragOffer);
        if (_glfw.wl.selectionSource != null)
            wayland_dataSourceDestroy(_glfw.wl.selectionSource);

        wayland_proxyDestroyWithOpcode(_glfw.wl.wmBase, XDG_WM_BASE_DESTROY);
        wayland_pointerDestroy(_glfw.wl.pointer);
        wayland_keyboardDestroy(_glfw.wl.keyboard);
        wayland_dataDeviceDestroy(_glfw.wl.dataDevice);
        wayland_proxyDestroy(_glfw.wl.dataDeviceManager);
        wayland_seatDestroy(_glfw.wl.seat);
        wayland_proxyDestroy(_glfw.wl.shm);
        wayland_proxyDestroyWithOpcode(_glfw.wl.fractionalScaleManager, WP_FRACTIONAL_SCALE_MANAGER_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.viewporter, WP_VIEWPORTER_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.decorationManager, ZXDG_DECORATION_MANAGER_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.idleInhibitManager, ZWP_IDLE_INHIBIT_MANAGER_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.activationManager, XDG_ACTIVATION_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.relativePointerManager, ZWP_RELATIVE_POINTER_MANAGER_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.pointerConstraints, ZWP_POINTER_CONSTRAINTS_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.cursorSurface, WL_SURFACE_DESTROY);
        wayland_proxyDestroyWithOpcode(_glfw.wl.subcompositor, 0);
        wayland_proxyDestroy(_glfw.wl.compositor);
        wayland_proxyDestroy(_glfw.wl.registry);

        if (_glfw.wl.display != null && _glfw.wl.client.display_disconnect != null)
            _glfw.wl.client.display_disconnect(_glfw.wl.display);
        if (_glfw.wl.client.handle != null)
            _glfwPlatformFreeModule(_glfw.wl.client.handle);

        if (_glfw.wl.keyRepeatTimerfd >= 0)
            wayland_close(_glfw.wl.keyRepeatTimerfd);
        if (_glfw.wl.cursorTimerfd >= 0)
            wayland_close(_glfw.wl.cursorTimerfd);

        _glfw_free(_glfw.wl.clipboardString);
        _glfw_free(_glfw.wl.offers);
        _glfw_free(_glfwWaylandRegistryListener);
        _glfw_free(_glfwWaylandOutputListener);
        _glfw_free(_glfwWaylandSurfaceListener);
        _glfw_free(_glfwWaylandSeatListener);
        _glfw_free(_glfwWaylandPointerListener);
        _glfw_free(_glfwWaylandKeyboardListener);
        _glfw_free(_glfwWaylandDataOfferListener);
        _glfw_free(_glfwWaylandDataDeviceListener);
        _glfw_free(_glfwWaylandDataSourceListener);
        _glfw_free(_glfwWaylandFractionalScaleListener);
        _glfw_free(_glfwWaylandXdgDecorationListener);
        _glfw_free(_glfwWaylandXdgActivationListener);
        _glfw_free(_glfwWaylandRelativePointerListener);
        _glfw_free(_glfwWaylandLockedPointerListener);
        _glfw_free(_glfwWaylandConfinedPointerListener);
        _glfw_free(_glfwWaylandXdgWmBaseListener);
        _glfw_free(_glfwWaylandXdgSurfaceListener);
        _glfw_free(_glfwWaylandXdgToplevelListener);
        _glfwWaylandRegistryListener = null;
        _glfwWaylandOutputListener = null;
        _glfwWaylandSurfaceListener = null;
        _glfwWaylandSeatListener = null;
        _glfwWaylandPointerListener = null;
        _glfwWaylandKeyboardListener = null;
        _glfwWaylandDataOfferListener = null;
        _glfwWaylandDataDeviceListener = null;
        _glfwWaylandDataSourceListener = null;
        _glfwWaylandFractionalScaleListener = null;
        _glfwWaylandXdgDecorationListener = null;
        _glfwWaylandXdgActivationListener = null;
        _glfwWaylandRelativePointerListener = null;
        _glfwWaylandLockedPointerListener = null;
        _glfwWaylandConfinedPointerListener = null;
        _glfwWaylandXdgWmBaseListener = null;
        _glfwWaylandXdgSurfaceListener = null;
        _glfwWaylandXdgToplevelListener = null;
        _glfw.wl = default;
    }
}
