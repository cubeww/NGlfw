using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const int VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR = 1000006000;
    const uint WL_SEAT_CAPABILITY_POINTER = 1;
    const uint WL_SEAT_CAPABILITY_KEYBOARD = 2;
    const uint WL_POINTER_BUTTON_STATE_RELEASED = 0;
    const uint WL_POINTER_BUTTON_STATE_PRESSED = 1;
    const uint WL_POINTER_AXIS_VERTICAL_SCROLL = 0;
    const uint WL_POINTER_AXIS_HORIZONTAL_SCROLL = 1;
    const uint WL_POINTER_SET_CURSOR = 0;
    const uint WL_KEYBOARD_KEYMAP_FORMAT_XKB_V1 = 1;
    const uint WL_KEYBOARD_KEY_STATE_RELEASED = 0;
    const uint WL_KEYBOARD_KEY_STATE_PRESSED = 1;
    const uint WL_KEYBOARD_REPEAT_INFO_SINCE_VERSION = 4;
    const uint XKB_KEY_NoSymbol = 0;
    const int XKB_KEYMAP_FORMAT_TEXT_V1 = 1;
    const int XKB_COMPOSE_COMPILE_NO_FLAGS = 0;
    const int XKB_COMPOSE_STATE_NO_FLAGS = 0;
    const int XKB_COMPOSE_FEED_ACCEPTED = 1;
    const int XKB_COMPOSE_COMPOSING = 1;
    const int XKB_COMPOSE_COMPOSED = 2;
    const int XKB_COMPOSE_CANCELLED = 3;
    const int XKB_STATE_MODS_EFFECTIVE = 1;
    const uint XKB_LAYOUT_INVALID = 0xffffffffu;
    const int PROT_READ = 1;
    const int PROT_WRITE = 2;
    const int MAP_SHARED = 1;
    const int ENOENT = 2;
    const short POLLOUT = 0x0004;
    const int TFD_CLOEXEC = 0x80000;
    const int TFD_NONBLOCK = 0x800;
    const uint BTN_LEFT = 0x110;
    const uint BTN_RIGHT = 0x111;
    const uint BTN_MIDDLE = 0x112;
    const uint BTN_SIDE = 0x113;
    const uint BTN_EXTRA = 0x114;
    const uint BTN_FORWARD = 0x115;
    const uint BTN_BACK = 0x116;
    const uint BTN_TASK = 0x117;
    const uint XDG_TOPLEVEL_STATE_MAXIMIZED = 1;
    const uint XDG_TOPLEVEL_STATE_FULLSCREEN = 2;
    const uint XDG_TOPLEVEL_STATE_RESIZING = 3;
    const uint XDG_TOPLEVEL_STATE_ACTIVATED = 4;
    const uint ZXDG_TOPLEVEL_DECORATION_V1_MODE_CLIENT_SIDE = 1;
    const uint ZXDG_TOPLEVEL_DECORATION_V1_MODE_SERVER_SIDE = 2;

    struct VkWaylandSurfaceCreateInfoKHR
    {
        public int sType;
        public void* pNext;
        public uint flags;
        public void* display;
        public void* surface;
    }

#pragma warning disable CS0649
    struct wl_array
    {
        public nuint size;
        public nuint alloc;
        public void* data;
    }
#pragma warning restore CS0649

#pragma warning disable CS0649
    struct wl_cursor_image
    {
        public uint width;
        public uint height;
        public uint hotspot_x;
        public uint hotspot_y;
        public uint delay;
    }

    struct wl_cursor
    {
        public uint image_count;
        public wl_cursor_image** images;
        public byte* name;
    }
#pragma warning restore CS0649

    struct wl_surface_listener
    {
        public delegate* unmanaged<void*, void*, void*, void> enter;
        public delegate* unmanaged<void*, void*, void*, void> leave;
        public delegate* unmanaged<void*, void*, int, void> preferred_buffer_scale;
        public delegate* unmanaged<void*, void*, uint, void> preferred_buffer_transform;
    }

    struct wl_seat_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> capabilities;
        public delegate* unmanaged<void*, void*, byte*, void> name;
    }

    struct wl_pointer_listener
    {
        public delegate* unmanaged<void*, void*, uint, void*, int, int, void> enter;
        public delegate* unmanaged<void*, void*, uint, void*, void> leave;
        public delegate* unmanaged<void*, void*, uint, int, int, void> motion;
        public delegate* unmanaged<void*, void*, uint, uint, uint, uint, void> button;
        public delegate* unmanaged<void*, void*, uint, uint, int, void> axis;
        public delegate* unmanaged<void*, void*, void> frame;
        public delegate* unmanaged<void*, void*, uint, void> axis_source;
        public delegate* unmanaged<void*, void*, uint, uint, void> axis_stop;
        public delegate* unmanaged<void*, void*, uint, int, void> axis_discrete;
        public delegate* unmanaged<void*, void*, uint, int, void> axis_value120;
        public delegate* unmanaged<void*, void*, uint, uint, void> axis_relative_direction;
    }

    struct wl_keyboard_listener
    {
        public delegate* unmanaged<void*, void*, uint, int, uint, void> keymap;
        public delegate* unmanaged<void*, void*, uint, void*, void*, void> enter;
        public delegate* unmanaged<void*, void*, uint, void*, void> leave;
        public delegate* unmanaged<void*, void*, uint, uint, uint, uint, void> key;
        public delegate* unmanaged<void*, void*, uint, uint, uint, uint, uint, void> modifiers;
        public delegate* unmanaged<void*, void*, int, int, void> repeat_info;
    }

    struct wl_data_offer_listener
    {
        public delegate* unmanaged<void*, void*, byte*, void> offer;
    }

    struct wl_data_device_listener
    {
        public delegate* unmanaged<void*, void*, void*, void> data_offer;
        public delegate* unmanaged<void*, void*, uint, void*, int, int, void*, void> enter;
        public delegate* unmanaged<void*, void*, void> leave;
        public delegate* unmanaged<void*, void*, uint, int, int, void> motion;
        public delegate* unmanaged<void*, void*, void> drop;
        public delegate* unmanaged<void*, void*, void*, void> selection;
    }

    struct wl_data_source_listener
    {
        public delegate* unmanaged<void*, void*, byte*, void> target;
        public delegate* unmanaged<void*, void*, byte*, int, void> send;
        public delegate* unmanaged<void*, void*, void> cancelled;
        public delegate* unmanaged<void*, void*, void> dnd_drop_performed;
        public delegate* unmanaged<void*, void*, void> dnd_finished;
        public delegate* unmanaged<void*, void*, uint, void> action;
    }

    struct wp_fractional_scale_v1_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> preferred_scale;
    }

    struct zxdg_toplevel_decoration_v1_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> configure;
    }

    struct ITIMERSPEC
    {
        public TIMESPEC it_interval;
        public TIMESPEC it_value;
    }

    struct xdg_surface_listener
    {
        public delegate* unmanaged<void*, void*, uint, void> configure;
    }

    struct xdg_toplevel_listener
    {
        public delegate* unmanaged<void*, void*, int, int, void*, void> configure;
        public delegate* unmanaged<void*, void*, void> close;
    }

    static wl_surface_listener* _glfwWaylandSurfaceListener;
    static wl_seat_listener* _glfwWaylandSeatListener;
    static wl_pointer_listener* _glfwWaylandPointerListener;
    static wl_keyboard_listener* _glfwWaylandKeyboardListener;
    static wl_data_offer_listener* _glfwWaylandDataOfferListener;
    static wl_data_device_listener* _glfwWaylandDataDeviceListener;
    static wl_data_source_listener* _glfwWaylandDataSourceListener;
    static wp_fractional_scale_v1_listener* _glfwWaylandFractionalScaleListener;
    static zxdg_toplevel_decoration_v1_listener* _glfwWaylandXdgDecorationListener;
    static xdg_surface_listener* _glfwWaylandXdgSurfaceListener;
    static xdg_toplevel_listener* _glfwWaylandXdgToplevelListener;

    static wl_surface_listener* wayland_getSurfaceListener()
    {
        if (_glfwWaylandSurfaceListener == null)
        {
            _glfwWaylandSurfaceListener = (wl_surface_listener*)_glfw_calloc(1, (nuint)sizeof(wl_surface_listener));
            if (_glfwWaylandSurfaceListener != null)
            {
                _glfwWaylandSurfaceListener->enter = &wayland_surfaceHandleEnter;
                _glfwWaylandSurfaceListener->leave = &wayland_surfaceHandleLeave;
                _glfwWaylandSurfaceListener->preferred_buffer_scale = &wayland_surfaceHandlePreferredBufferScale;
                _glfwWaylandSurfaceListener->preferred_buffer_transform = &wayland_surfaceHandlePreferredBufferTransform;
            }
        }

        return _glfwWaylandSurfaceListener;
    }

    static wl_seat_listener* wayland_getSeatListener()
    {
        if (_glfwWaylandSeatListener == null)
        {
            _glfwWaylandSeatListener = (wl_seat_listener*)_glfw_calloc(1, (nuint)sizeof(wl_seat_listener));
            if (_glfwWaylandSeatListener != null)
            {
                _glfwWaylandSeatListener->capabilities = &wayland_seatHandleCapabilities;
                _glfwWaylandSeatListener->name = &wayland_seatHandleName;
            }
        }

        return _glfwWaylandSeatListener;
    }

    static wl_pointer_listener* wayland_getPointerListener()
    {
        if (_glfwWaylandPointerListener == null)
        {
            _glfwWaylandPointerListener = (wl_pointer_listener*)_glfw_calloc(1, (nuint)sizeof(wl_pointer_listener));
            if (_glfwWaylandPointerListener != null)
            {
                _glfwWaylandPointerListener->enter = &wayland_pointerHandleEnter;
                _glfwWaylandPointerListener->leave = &wayland_pointerHandleLeave;
                _glfwWaylandPointerListener->motion = &wayland_pointerHandleMotion;
                _glfwWaylandPointerListener->button = &wayland_pointerHandleButton;
                _glfwWaylandPointerListener->axis = &wayland_pointerHandleAxis;
                _glfwWaylandPointerListener->frame = &wayland_pointerHandleFrame;
                _glfwWaylandPointerListener->axis_source = &wayland_pointerHandleAxisSource;
                _glfwWaylandPointerListener->axis_stop = &wayland_pointerHandleAxisStop;
                _glfwWaylandPointerListener->axis_discrete = &wayland_pointerHandleAxisDiscrete;
                _glfwWaylandPointerListener->axis_value120 = &wayland_pointerHandleAxisValue120;
                _glfwWaylandPointerListener->axis_relative_direction = &wayland_pointerHandleAxisRelativeDirection;
            }
        }

        return _glfwWaylandPointerListener;
    }

    static wl_keyboard_listener* wayland_getKeyboardListener()
    {
        if (_glfwWaylandKeyboardListener == null)
        {
            _glfwWaylandKeyboardListener = (wl_keyboard_listener*)_glfw_calloc(1, (nuint)sizeof(wl_keyboard_listener));
            if (_glfwWaylandKeyboardListener != null)
            {
                _glfwWaylandKeyboardListener->keymap = &wayland_keyboardHandleKeymap;
                _glfwWaylandKeyboardListener->enter = &wayland_keyboardHandleEnter;
                _glfwWaylandKeyboardListener->leave = &wayland_keyboardHandleLeave;
                _glfwWaylandKeyboardListener->key = &wayland_keyboardHandleKey;
                _glfwWaylandKeyboardListener->modifiers = &wayland_keyboardHandleModifiers;
                _glfwWaylandKeyboardListener->repeat_info = &wayland_keyboardHandleRepeatInfo;
            }
        }

        return _glfwWaylandKeyboardListener;
    }

    static wl_data_offer_listener* wayland_getDataOfferListener()
    {
        if (_glfwWaylandDataOfferListener == null)
        {
            _glfwWaylandDataOfferListener = (wl_data_offer_listener*)_glfw_calloc(1, (nuint)sizeof(wl_data_offer_listener));
            if (_glfwWaylandDataOfferListener != null)
                _glfwWaylandDataOfferListener->offer = &wayland_dataOfferHandleOffer;
        }

        return _glfwWaylandDataOfferListener;
    }

    static wl_data_device_listener* wayland_getDataDeviceListener()
    {
        if (_glfwWaylandDataDeviceListener == null)
        {
            _glfwWaylandDataDeviceListener = (wl_data_device_listener*)_glfw_calloc(1, (nuint)sizeof(wl_data_device_listener));
            if (_glfwWaylandDataDeviceListener != null)
            {
                _glfwWaylandDataDeviceListener->data_offer = &wayland_dataDeviceHandleDataOffer;
                _glfwWaylandDataDeviceListener->enter = &wayland_dataDeviceHandleEnter;
                _glfwWaylandDataDeviceListener->leave = &wayland_dataDeviceHandleLeave;
                _glfwWaylandDataDeviceListener->motion = &wayland_dataDeviceHandleMotion;
                _glfwWaylandDataDeviceListener->drop = &wayland_dataDeviceHandleDrop;
                _glfwWaylandDataDeviceListener->selection = &wayland_dataDeviceHandleSelection;
            }
        }

        return _glfwWaylandDataDeviceListener;
    }

    static wl_data_source_listener* wayland_getDataSourceListener()
    {
        if (_glfwWaylandDataSourceListener == null)
        {
            _glfwWaylandDataSourceListener = (wl_data_source_listener*)_glfw_calloc(1, (nuint)sizeof(wl_data_source_listener));
            if (_glfwWaylandDataSourceListener != null)
            {
                _glfwWaylandDataSourceListener->target = &wayland_dataSourceHandleTarget;
                _glfwWaylandDataSourceListener->send = &wayland_dataSourceHandleSend;
                _glfwWaylandDataSourceListener->cancelled = &wayland_dataSourceHandleCancelled;
                _glfwWaylandDataSourceListener->dnd_drop_performed = &wayland_dataSourceHandleDndDropPerformed;
                _glfwWaylandDataSourceListener->dnd_finished = &wayland_dataSourceHandleDndFinished;
                _glfwWaylandDataSourceListener->action = &wayland_dataSourceHandleAction;
            }
        }

        return _glfwWaylandDataSourceListener;
    }

    static wp_fractional_scale_v1_listener* wayland_getFractionalScaleListener()
    {
        if (_glfwWaylandFractionalScaleListener == null)
        {
            _glfwWaylandFractionalScaleListener =
                (wp_fractional_scale_v1_listener*)_glfw_calloc(1, (nuint)sizeof(wp_fractional_scale_v1_listener));
            if (_glfwWaylandFractionalScaleListener != null)
                _glfwWaylandFractionalScaleListener->preferred_scale = &wayland_fractionalScaleHandlePreferredScale;
        }

        return _glfwWaylandFractionalScaleListener;
    }

    static zxdg_toplevel_decoration_v1_listener* wayland_getXdgDecorationListener()
    {
        if (_glfwWaylandXdgDecorationListener == null)
        {
            _glfwWaylandXdgDecorationListener =
                (zxdg_toplevel_decoration_v1_listener*)_glfw_calloc(1, (nuint)sizeof(zxdg_toplevel_decoration_v1_listener));
            if (_glfwWaylandXdgDecorationListener != null)
                _glfwWaylandXdgDecorationListener->configure = &wayland_xdgDecorationHandleConfigure;
        }

        return _glfwWaylandXdgDecorationListener;
    }

    static xdg_surface_listener* wayland_getXdgSurfaceListener()
    {
        if (_glfwWaylandXdgSurfaceListener == null)
        {
            _glfwWaylandXdgSurfaceListener = (xdg_surface_listener*)_glfw_calloc(1, (nuint)sizeof(xdg_surface_listener));
            if (_glfwWaylandXdgSurfaceListener != null)
                _glfwWaylandXdgSurfaceListener->configure = &wayland_xdgSurfaceHandleConfigure;
        }

        return _glfwWaylandXdgSurfaceListener;
    }

    static xdg_toplevel_listener* wayland_getXdgToplevelListener()
    {
        if (_glfwWaylandXdgToplevelListener == null)
        {
            _glfwWaylandXdgToplevelListener = (xdg_toplevel_listener*)_glfw_calloc(1, (nuint)sizeof(xdg_toplevel_listener));
            if (_glfwWaylandXdgToplevelListener != null)
            {
                _glfwWaylandXdgToplevelListener->configure = &wayland_xdgToplevelHandleConfigure;
                _glfwWaylandXdgToplevelListener->close = &wayland_xdgToplevelHandleClose;
            }
        }

        return _glfwWaylandXdgToplevelListener;
    }

    static void _glfwUpdateBufferScaleFromOutputsWayland(_GLFWwindow* window)
    {
        if (window->wl.surface == null ||
            _glfw.wl.client.proxy_marshal_int == null ||
            _glfw.wl.client.proxy_get_version == null ||
            _glfw.wl.client.proxy_get_version(window->wl.surface) < WL_SURFACE_SET_BUFFER_SCALE_SINCE_VERSION)
        {
            return;
        }

        var scale = 1;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
            scale = _glfw_max(scale, window->wl.outputScales[i].factor);

        if (window->wl.scaleFramebuffer == 0 || window->wl.fractionalScale != null)
            return;

        if (window->wl.bufferScale == scale)
            return;

        window->wl.bufferScale = scale;
        wayland_surfaceSetBufferScale(window->wl.surface, scale);

        window->wl.fbWidth = window->wl.width * scale;
        window->wl.fbHeight = window->wl.height * scale;

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        _glfwInputWindowContentScale(window, scale, scale);
        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    static void wayland_surfaceCommit(void* surface)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(surface, WL_SURFACE_COMMIT);
    }

    static void wayland_surfaceAttach(void* surface, void* buffer, int x, int y)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal_object_int_int != null)
            _glfw.wl.client.proxy_marshal_object_int_int(surface, WL_SURFACE_ATTACH, buffer, x, y);
    }

    static void wayland_surfaceDamage(void* surface, int x, int y, int width, int height)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal_int_int_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int_int_int(surface, WL_SURFACE_DAMAGE, x, y, width, height);
    }

    static void wayland_surfaceSetBufferScale(void* surface, int scale)
    {
        if (surface == null ||
            _glfw.wl.client.proxy_marshal_int == null ||
            _glfw.wl.client.proxy_get_version == null ||
            _glfw.wl.client.proxy_get_version(surface) < WL_SURFACE_SET_BUFFER_SCALE_SINCE_VERSION)
        {
            return;
        }

        _glfw.wl.client.proxy_marshal_int(surface, WL_SURFACE_SET_BUFFER_SCALE, scale);
    }

    static void wayland_surfaceSetOpaqueRegion(void* surface, void* region)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal_object != null)
            _glfw.wl.client.proxy_marshal_object(surface, WL_SURFACE_SET_OPAQUE_REGION, region);
    }

    static void wayland_surfaceSetInputRegion(void* surface, void* region)
    {
        if (surface != null && _glfw.wl.client.proxy_marshal_object != null)
            _glfw.wl.client.proxy_marshal_object(surface, WL_SURFACE_SET_INPUT_REGION, region);
    }

    static void* wayland_compositorCreateRegion()
    {
        if (_glfw.wl.compositor == null ||
            _glfw.wl.client.regionInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor == null)
        {
            return null;
        }

        var region = _glfw.wl.client.proxy_marshal_constructor(_glfw.wl.compositor,
            WL_COMPOSITOR_CREATE_REGION,
            _glfw.wl.client.regionInterface,
            null);

        wayland_tagProxy(region);
        return region;
    }

    static void wayland_regionAdd(void* region, int x, int y, int width, int height)
    {
        if (region != null && _glfw.wl.client.proxy_marshal_int_int_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int_int_int(region, WL_REGION_ADD, x, y, width, height);
    }

    static void wayland_regionDestroy(void* region)
    {
        wayland_proxyDestroyWithOpcode(region, WL_REGION_DESTROY);
    }

    static void wayland_setContentAreaOpaque(_GLFWwindow* window)
    {
        if (window == null || window->wl.surface == null)
            return;

        var region = wayland_compositorCreateRegion();
        if (region == null)
            return;

        wayland_regionAdd(region, 0, 0, window->wl.width, window->wl.height);
        wayland_surfaceSetOpaqueRegion(window->wl.surface, region);
        wayland_regionDestroy(region);
    }

    static void* wayland_viewporterGetViewport(void* viewporter, void* surface)
    {
        if (viewporter == null ||
            surface == null ||
            _glfwWaylandWpViewportInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null)
        {
            return null;
        }

        var viewport = _glfw.wl.client.proxy_marshal_constructor_object(viewporter,
            WP_VIEWPORTER_GET_VIEWPORT,
            _glfwWaylandWpViewportInterface,
            null,
            surface);

        wayland_tagProxy(viewport);
        return viewport;
    }

    static void wayland_viewportSetDestination(void* viewport, int width, int height)
    {
        if (viewport != null && _glfw.wl.client.proxy_marshal_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int(viewport, WP_VIEWPORT_SET_DESTINATION, width, height);
    }

    static void wayland_viewportDestroy(void* viewport)
    {
        wayland_proxyDestroyWithOpcode(viewport, WP_VIEWPORT_DESTROY);
    }

    static void* wayland_fractionalScaleManagerGetFractionalScale(void* manager, void* surface)
    {
        if (manager == null ||
            surface == null ||
            _glfwWaylandWpFractionalScaleInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null)
        {
            return null;
        }

        var fractionalScale = _glfw.wl.client.proxy_marshal_constructor_object(manager,
            WP_FRACTIONAL_SCALE_MANAGER_GET_FRACTIONAL_SCALE,
            _glfwWaylandWpFractionalScaleInterface,
            null,
            surface);

        wayland_tagProxy(fractionalScale);
        return fractionalScale;
    }

    static void wayland_fractionalScaleDestroy(void* fractionalScale)
    {
        wayland_proxyDestroyWithOpcode(fractionalScale, WP_FRACTIONAL_SCALE_DESTROY);
    }

    static void wayland_updateFramebufferSize(_GLFWwindow* window)
    {
        if (window->wl.fractionalScale != null)
        {
            window->wl.fbWidth = (int)(window->wl.width * window->wl.scalingNumerator / 120);
            window->wl.fbHeight = (int)(window->wl.height * window->wl.scalingNumerator / 120);
        }
        else
        {
            var scale = window->wl.bufferScale != 0 ? window->wl.bufferScale : 1;
            window->wl.fbWidth = window->wl.width * scale;
            window->wl.fbHeight = window->wl.height * scale;
        }

        if (window->wl.egl.window != null && _glfw.wl.egl.window_resize != null)
            _glfw.wl.egl.window_resize(window->wl.egl.window, window->wl.fbWidth, window->wl.fbHeight, 0, 0);

        if (window->wl.transparent == 0)
            wayland_setContentAreaOpaque(window);

        _glfwInputFramebufferSize(window, window->wl.fbWidth, window->wl.fbHeight);
    }

    static void* wayland_shmCreatePool(int fd, int size)
    {
        if (_glfw.wl.shm == null ||
            _glfw.wl.client.shmPoolInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor_int_int == null)
        {
            return null;
        }

        return _glfw.wl.client.proxy_marshal_constructor_int_int(_glfw.wl.shm,
            WL_SHM_CREATE_POOL,
            _glfw.wl.client.shmPoolInterface,
            null,
            fd,
            size);
    }

    static void* wayland_shmPoolCreateBuffer(void* pool,
                                             int offset,
                                             int width,
                                             int height,
                                             int stride,
                                             uint format)
    {
        if (pool == null ||
            _glfw.wl.client.bufferInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor_int_int_int_int_uint == null)
        {
            return null;
        }

        return _glfw.wl.client.proxy_marshal_constructor_int_int_int_int_uint(pool,
            WL_SHM_POOL_CREATE_BUFFER,
            _glfw.wl.client.bufferInterface,
            null,
            offset,
            width,
            height,
            stride,
            format);
    }

    static void wayland_shmPoolDestroy(void* pool)
    {
        wayland_proxyDestroyWithOpcode(pool, WL_SHM_POOL_DESTROY);
    }

    static int wayland_createAnonymousFile(nint size, out int errorCode)
    {
        errorCode = 0;

        var path = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        if (string.IsNullOrEmpty(path))
        {
            errorCode = ENOENT;
            return -1;
        }

        var nameBytes = Encoding.UTF8.GetBytes(path + "/glfw-shared-XXXXXX\0");
        fixed (byte* name = nameBytes)
        {
            var fd = wayland_mkstemp(name);
            if (fd < 0)
            {
                errorCode = Marshal.GetLastPInvokeError();
                return -1;
            }

            wayland_unlink(name);

            var result = wayland_posix_fallocate(fd, 0, size);
            if (result != 0)
            {
                wayland_close(fd);
                errorCode = result;
                return -1;
            }

            return fd;
        }
    }

    static void* wayland_createShmBuffer(GLFWimage* image)
    {
        var stride = image->width * 4;
        var length = image->width * image->height * 4;

        var fd = wayland_createAnonymousFile(length, out var errorCode);
        if (fd < 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR,
                "Wayland: Failed to create buffer file of size {0}: errno {1}",
                length,
                errorCode);
            return null;
        }

        var data = wayland_mmap(null, (nuint)length, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0);
        if ((nint)data == -1)
        {
            errorCode = Marshal.GetLastPInvokeError();
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to map file: errno {0}", errorCode);
            wayland_close(fd);
            return null;
        }

        var pool = wayland_shmCreatePool(fd, length);
        wayland_close(fd);
        if (pool == null)
        {
            wayland_munmap(data, (nuint)length);
            return null;
        }

        byte* source = image->pixels;
        byte* target = (byte*)data;
        for (var i = 0; i < image->width * image->height; i++, source += 4)
        {
            var alpha = source[3];

            *target++ = (byte)((source[2] * alpha) / 255);
            *target++ = (byte)((source[1] * alpha) / 255);
            *target++ = (byte)((source[0] * alpha) / 255);
            *target++ = alpha;
        }

        var buffer = wayland_shmPoolCreateBuffer(pool,
            0,
            image->width,
            image->height,
            stride,
            WL_SHM_FORMAT_ARGB8888);
        wayland_munmap(data, (nuint)length);
        wayland_shmPoolDestroy(pool);

        return buffer;
    }

    static int wayland_proxyHasTag(void* proxy)
    {
        if (proxy == null || _glfw.wl.client.proxy_get_tag == null)
            return GLFW_FALSE;

        fixed (_GLFWlibrary* glfw = &_glfw)
            return _glfw.wl.client.proxy_get_tag(proxy) == &glfw->wl.tag ? GLFW_TRUE : GLFW_FALSE;
    }

    static void wayland_surfaceAddOutputScale(_GLFWwindow* window, void* output, int factor)
    {
        if (window == null || output == null)
            return;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
        {
            if (window->wl.outputScales[i].output == output)
                return;
        }

        if (window->wl.outputScaleCount + 1 > window->wl.outputScaleSize)
        {
            var size = window->wl.outputScaleSize + 1;
            var scales = (_GLFWscaleWayland*)_glfw_realloc(window->wl.outputScales,
                (nuint)(size * (nuint)sizeof(_GLFWscaleWayland)));
            if (scales == null)
                return;

            window->wl.outputScales = scales;
            window->wl.outputScaleSize = size;
        }

        window->wl.outputScales[window->wl.outputScaleCount++] = new _GLFWscaleWayland
        {
            output = output,
            factor = factor
        };

        _glfwUpdateBufferScaleFromOutputsWayland(window);
    }

    static void wayland_surfaceRemoveOutputScale(_GLFWwindow* window, void* output)
    {
        if (window == null || output == null)
            return;

        for (nuint i = 0; i < window->wl.outputScaleCount; i++)
        {
            if (window->wl.outputScales[i].output == output)
            {
                window->wl.outputScales[i] = window->wl.outputScales[window->wl.outputScaleCount - 1];
                window->wl.outputScaleCount--;
                break;
            }
        }

        _glfwUpdateBufferScaleFromOutputsWayland(window);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandleEnter(void* userData, void* surface, void* output)
    {
        if (wayland_proxyHasTag(output) == 0 || _glfw.wl.client.proxy_get_user_data == null)
            return;

        var window = (_GLFWwindow*)userData;
        var monitor = (_GLFWmonitor*)_glfw.wl.client.proxy_get_user_data(output);
        if (window == null || monitor == null)
            return;

        wayland_surfaceAddOutputScale(window, output, monitor->wl.scale != 0 ? monitor->wl.scale : 1);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandleLeave(void* userData, void* surface, void* output)
    {
        if (wayland_proxyHasTag(output) == 0)
            return;

        wayland_surfaceRemoveOutputScale((_GLFWwindow*)userData, output);
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandlePreferredBufferScale(void* userData, void* surface, int factor)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_surfaceHandlePreferredBufferTransform(void* userData, void* surface, uint transform)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_fractionalScaleHandlePreferredScale(void* userData, void* fractionalScale, uint numerator)
    {
        var window = (_GLFWwindow*)userData;
        if (window == null)
            return;

        window->wl.scalingNumerator = numerator;
        _glfwInputWindowContentScale(window, numerator / 120f, numerator / 120f);
        wayland_updateFramebufferSize(window);

        if (window->wl.visible != 0)
            _glfwInputWindowDamage(window);
    }

    static double wayland_fixedToDouble(int value)
    {
        return value / 256.0;
    }

    static int wayland_pointerButtonToGLFW(uint button)
    {
        if (button >= BTN_LEFT && button <= BTN_TASK)
            return (int)(button - BTN_LEFT);

        return -1;
    }

    static void* wayland_themeGetCursor(void* theme, string name)
    {
        if (theme == null || _glfw.wl.cursor.theme_get_cursor == null)
            return null;

        var bytes = Encoding.UTF8.GetBytes(name + '\0');
        fixed (byte* cursorName = bytes)
            return _glfw.wl.cursor.theme_get_cursor(theme, cursorName);
    }

    static void wayland_pointerSetCursor(void* pointer, uint serial, void* surface, int xhot, int yhot)
    {
        if (pointer != null && _glfw.wl.client.proxy_marshal_uint_object_int_int != null)
            _glfw.wl.client.proxy_marshal_uint_object_int_int(pointer, WL_POINTER_SET_CURSOR, serial, surface, xhot, yhot);
    }

    static void* wayland_seatGetPointer(void* seat)
    {
        if (seat == null || _glfw.wl.client.pointerInterface == null)
            return null;

        var pointer = _glfw.wl.client.proxy_marshal_constructor(seat,
            WL_SEAT_GET_POINTER,
            _glfw.wl.client.pointerInterface,
            null);

        wayland_tagProxy(pointer);
        return pointer;
    }

    static void wayland_pointerDestroy(void* pointer)
    {
        if (pointer == null)
            return;

        if (_glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(pointer) >= WL_POINTER_RELEASE_SINCE_VERSION)
        {
            wayland_proxyDestroyWithOpcode(pointer, WL_POINTER_RELEASE);
        }
        else
            wayland_proxyDestroy(pointer);
    }

    static void wayland_seatDestroy(void* seat)
    {
        if (seat == null)
            return;

        if (_glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(seat) >= WL_SEAT_RELEASE_SINCE_VERSION)
        {
            wayland_proxyDestroyWithOpcode(seat, WL_SEAT_RELEASE);
        }
        else
            wayland_proxyDestroy(seat);
    }

    static void wayland_createPointer(void* seat)
    {
        if (_glfw.wl.pointer != null)
            return;

        var pointer = wayland_seatGetPointer(seat);
        if (pointer == null)
            return;

        var listener = wayland_getPointerListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(pointer, listener, null) != 0)
        {
            wayland_pointerDestroy(pointer);
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add pointer listener");
            return;
        }

        _glfw.wl.pointer = pointer;
    }

    static void* wayland_seatGetKeyboard(void* seat)
    {
        if (seat == null || _glfw.wl.client.keyboardInterface == null)
            return null;

        var keyboard = _glfw.wl.client.proxy_marshal_constructor(seat,
            WL_SEAT_GET_KEYBOARD,
            _glfw.wl.client.keyboardInterface,
            null);

        wayland_tagProxy(keyboard);
        return keyboard;
    }

    static void wayland_keyboardDestroy(void* keyboard)
    {
        if (keyboard == null)
            return;

        if (_glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(keyboard) >= WL_KEYBOARD_RELEASE_SINCE_VERSION)
        {
            wayland_proxyDestroyWithOpcode(keyboard, WL_KEYBOARD_RELEASE);
        }
        else
            wayland_proxyDestroy(keyboard);
    }

    static void wayland_stopKeyRepeatTimer()
    {
        if (_glfw.wl.keyRepeatTimerfd < 0)
            return;

        ITIMERSPEC timer = default;
        wayland_timerfd_settime(_glfw.wl.keyRepeatTimerfd, 0, &timer, null);
    }

    static void wayland_stopCursorTimer()
    {
        if (_glfw.wl.cursorTimerfd < 0)
            return;

        ITIMERSPEC timer = default;
        wayland_timerfd_settime(_glfw.wl.cursorTimerfd, 0, &timer, null);
    }

    static void wayland_createKeyboard(void* seat)
    {
        if (_glfw.wl.keyboard != null)
            return;

        var keyboard = wayland_seatGetKeyboard(seat);
        if (keyboard == null)
            return;

        var listener = wayland_getKeyboardListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(keyboard, listener, null) != 0)
        {
            wayland_keyboardDestroy(keyboard);
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add keyboard listener");
            return;
        }

        _glfw.wl.keyboard = keyboard;
    }

    static int wayland_translateKey(uint scancode)
    {
        fixed (short* keycodes = _glfw.wl.keycodes)
        {
            if (scancode < 256)
                return keycodes[scancode];
        }

        return GLFW_KEY_UNKNOWN;
    }

    static uint wayland_composeSymbol(uint sym)
    {
        if (sym == XKB_KEY_NoSymbol || _glfw.wl.xkb.composeState == null)
            return sym;

        if (_glfw.wl.xkb.compose_state_feed == null ||
            _glfw.wl.xkb.compose_state_get_status == null ||
            _glfw.wl.xkb.compose_state_get_one_sym == null ||
            _glfw.wl.xkb.compose_state_feed(_glfw.wl.xkb.composeState, sym) != XKB_COMPOSE_FEED_ACCEPTED)
        {
            return sym;
        }

        return _glfw.wl.xkb.compose_state_get_status(_glfw.wl.xkb.composeState) switch
        {
            XKB_COMPOSE_COMPOSED => _glfw.wl.xkb.compose_state_get_one_sym(_glfw.wl.xkb.composeState),
            XKB_COMPOSE_COMPOSING => XKB_KEY_NoSymbol,
            XKB_COMPOSE_CANCELLED => XKB_KEY_NoSymbol,
            _ => sym
        };
    }

    static void wayland_inputText(_GLFWwindow* window, uint scancode)
    {
        if (window == null ||
            _glfw.wl.xkb.state == null ||
            _glfw.wl.xkb.state_key_get_syms == null)
        {
            return;
        }

        uint* keysyms = null;
        var keycode = scancode + 8;
        if (_glfw.wl.xkb.state_key_get_syms(_glfw.wl.xkb.state, keycode, &keysyms) == 1)
        {
            var keysym = wayland_composeSymbol(keysyms[0]);
            var codepoint = _glfwKeySym2Unicode(keysym);
            if (codepoint != GLFW_INVALID_CODEPOINT)
            {
                var mods = (int)_glfw.wl.xkb.modifiers;
                var plain = (mods & (GLFW_MOD_CONTROL | GLFW_MOD_ALT)) == 0 ? GLFW_TRUE : GLFW_FALSE;
                _glfwInputChar(window, codepoint, mods, plain);
            }
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleKeymap(void* userData, void* keyboard, uint format, int fd, uint size)
    {
        if (format != WL_KEYBOARD_KEYMAP_FORMAT_XKB_V1)
        {
            wayland_close(fd);
            return;
        }

        var mapStr = wayland_mmap(null, (nuint)size, PROT_READ, MAP_SHARED, fd, 0);
        if ((nint)mapStr == -1)
        {
            wayland_close(fd);
            return;
        }

        var keymap = _glfw.wl.xkb.keymap_new_from_string != null
            ? _glfw.wl.xkb.keymap_new_from_string(_glfw.wl.xkb.context,
                (byte*)mapStr,
                XKB_KEYMAP_FORMAT_TEXT_V1,
                0)
            : null;

        wayland_munmap(mapStr, (nuint)size);
        wayland_close(fd);

        if (keymap == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to compile keymap");
            return;
        }

        var state = _glfw.wl.xkb.state_new != null ? _glfw.wl.xkb.state_new(keymap) : null;
        if (state == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create XKB state");
            if (_glfw.wl.xkb.keymap_unref != null)
                _glfw.wl.xkb.keymap_unref(keymap);
            return;
        }

        var locale = Environment.GetEnvironmentVariable("LC_ALL");
        if (string.IsNullOrEmpty(locale))
            locale = Environment.GetEnvironmentVariable("LC_CTYPE");
        if (string.IsNullOrEmpty(locale))
            locale = Environment.GetEnvironmentVariable("LANG");
        if (string.IsNullOrEmpty(locale))
            locale = "C";

        var localeBytes = Encoding.UTF8.GetBytes(locale + '\0');
        fixed (byte* localeName = localeBytes)
        {
            var composeTable = _glfw.wl.xkb.compose_table_new_from_locale != null
                ? _glfw.wl.xkb.compose_table_new_from_locale(_glfw.wl.xkb.context,
                    localeName,
                    XKB_COMPOSE_COMPILE_NO_FLAGS)
                : null;

            if (composeTable != null)
            {
                var composeState = _glfw.wl.xkb.compose_state_new != null
                    ? _glfw.wl.xkb.compose_state_new(composeTable, XKB_COMPOSE_STATE_NO_FLAGS)
                    : null;
                if (_glfw.wl.xkb.compose_table_unref != null)
                    _glfw.wl.xkb.compose_table_unref(composeTable);

                if (composeState != null)
                {
                    if (_glfw.wl.xkb.composeState != null && _glfw.wl.xkb.compose_state_unref != null)
                        _glfw.wl.xkb.compose_state_unref(_glfw.wl.xkb.composeState);
                    _glfw.wl.xkb.composeState = composeState;
                }
                else
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create XKB compose state");
            }
            else
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create XKB compose table");
        }

        if (_glfw.wl.xkb.keymap != null && _glfw.wl.xkb.keymap_unref != null)
            _glfw.wl.xkb.keymap_unref(_glfw.wl.xkb.keymap);
        if (_glfw.wl.xkb.state != null && _glfw.wl.xkb.state_unref != null)
            _glfw.wl.xkb.state_unref(_glfw.wl.xkb.state);

        _glfw.wl.xkb.keymap = keymap;
        _glfw.wl.xkb.state = state;

        if (_glfw.wl.xkb.keymap_mod_get_index != null)
        {
            _glfw.wl.xkb.controlIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbControl);
            _glfw.wl.xkb.altIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbMod1);
            _glfw.wl.xkb.shiftIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbShift);
            _glfw.wl.xkb.superIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbMod4);
            _glfw.wl.xkb.capsLockIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbLock);
            _glfw.wl.xkb.numLockIndex = _glfw.wl.xkb.keymap_mod_get_index(keymap, _glfwWaylandXkbMod2);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleEnter(void* userData, void* keyboard, uint serial, void* surface, void* keys)
    {
        if (surface == null || wayland_proxyHasTag(surface) == 0 || _glfw.wl.client.proxy_get_user_data == null)
            return;

        var window = (_GLFWwindow*)_glfw.wl.client.proxy_get_user_data(surface);
        if (window == null || surface != window->wl.surface)
            return;

        _glfw.wl.serial = serial;
        _glfw.wl.keyboardFocus = window;
        _glfwInputWindowFocus(window, GLFW_TRUE);
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleLeave(void* userData, void* keyboard, uint serial, void* surface)
    {
        var window = _glfw.wl.keyboardFocus;
        if (window == null)
            return;

        wayland_stopKeyRepeatTimer();

        _glfw.wl.serial = serial;
        _glfw.wl.keyboardFocus = null;
        _glfwInputWindowFocus(window, GLFW_FALSE);
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleKey(void* userData,
                                          void* keyboard,
                                          uint serial,
                                          uint time,
                                          uint scancode,
                                          uint state)
    {
        var window = _glfw.wl.keyboardFocus;
        if (window == null)
            return;

        var key = wayland_translateKey(scancode);
        var action = state == WL_KEYBOARD_KEY_STATE_PRESSED ? GLFW_PRESS : GLFW_RELEASE;

        _glfw.wl.serial = serial;

        ITIMERSPEC timer = default;
        if (action == GLFW_PRESS &&
            _glfw.wl.keyRepeatTimerfd >= 0 &&
            _glfw.wl.xkb.keymap != null &&
            _glfw.wl.xkb.keymap_key_repeats != null &&
            _glfw.wl.xkb.keymap_key_repeats(_glfw.wl.xkb.keymap, scancode + 8) != 0 &&
            _glfw.wl.keyRepeatRate > 0)
        {
            _glfw.wl.keyRepeatScancode = (int)scancode;
            if (_glfw.wl.keyRepeatRate > 1)
                timer.it_interval.tv_nsec = 1000000000 / _glfw.wl.keyRepeatRate;
            else
                timer.it_interval.tv_sec = 1;

            timer.it_value.tv_sec = _glfw.wl.keyRepeatDelay / 1000;
            timer.it_value.tv_nsec = (_glfw.wl.keyRepeatDelay % 1000) * 1000000;
        }

        if (_glfw.wl.keyRepeatTimerfd >= 0)
            wayland_timerfd_settime(_glfw.wl.keyRepeatTimerfd, 0, &timer, null);

        _glfwInputKey(window, key, (int)scancode, action, (int)_glfw.wl.xkb.modifiers);

        if (action == GLFW_PRESS)
            wayland_inputText(window, scancode);
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleModifiers(void* userData,
                                                void* keyboard,
                                                uint serial,
                                                uint modsDepressed,
                                                uint modsLatched,
                                                uint modsLocked,
                                                uint group)
    {
        _glfw.wl.serial = serial;

        if (_glfw.wl.xkb.keymap == null ||
            _glfw.wl.xkb.state == null ||
            _glfw.wl.xkb.state_update_mask == null ||
            _glfw.wl.xkb.state_mod_index_is_active == null)
        {
            return;
        }

        _glfw.wl.xkb.state_update_mask(_glfw.wl.xkb.state,
            modsDepressed,
            modsLatched,
            modsLocked,
            0,
            0,
            group);

        _glfw.wl.xkb.modifiers = 0;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.controlIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_CONTROL;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.altIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_ALT;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.shiftIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_SHIFT;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.superIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_SUPER;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.capsLockIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_CAPS_LOCK;
        if (_glfw.wl.xkb.state_mod_index_is_active(_glfw.wl.xkb.state, _glfw.wl.xkb.numLockIndex, XKB_STATE_MODS_EFFECTIVE) == 1)
            _glfw.wl.xkb.modifiers |= GLFW_MOD_NUM_LOCK;
    }

    [UnmanagedCallersOnly]
    static void wayland_keyboardHandleRepeatInfo(void* userData, void* keyboard, int rate, int delay)
    {
        if (keyboard != _glfw.wl.keyboard)
            return;

        _glfw.wl.keyRepeatRate = rate;
        _glfw.wl.keyRepeatDelay = delay;
    }

    static void wayland_dispatchKeyRepeats()
    {
        var window = _glfw.wl.keyboardFocus;
        if (window == null || _glfw.wl.keyRepeatTimerfd < 0)
            return;

        ulong repeats;
        if (wayland_read(_glfw.wl.keyRepeatTimerfd, &repeats, (nuint)sizeof(ulong)) != sizeof(ulong))
            return;

        for (ulong i = 0; i < repeats; i++)
        {
            var scancode = _glfw.wl.keyRepeatScancode;
            _glfwInputKey(window,
                wayland_translateKey((uint)scancode),
                scancode,
                GLFW_PRESS,
                (int)_glfw.wl.xkb.modifiers);
            wayland_inputText(window, (uint)scancode);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleEnter(void* userData,
                                           void* pointer,
                                           uint serial,
                                           void* surface,
                                           int sx,
                                           int sy)
    {
        if (surface == null || wayland_proxyHasTag(surface) == 0 || _glfw.wl.client.proxy_get_user_data == null)
            return;

        var window = (_GLFWwindow*)_glfw.wl.client.proxy_get_user_data(surface);
        if (window == null)
            return;

        _glfw.wl.serial = serial;
        _glfw.wl.pointerEnterSerial = serial;
        _glfw.wl.pointerFocus = window;

        if (surface == window->wl.surface)
        {
            window->wl.hovered = GLFW_TRUE;
            _glfwSetCursorWayland(window, window->wl.currentCursor);
            _glfwInputCursorEnter(window, GLFW_TRUE);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleLeave(void* userData, void* pointer, uint serial, void* surface)
    {
        if (surface == null || wayland_proxyHasTag(surface) == 0)
            return;

        var window = _glfw.wl.pointerFocus;
        if (window == null)
            return;

        _glfw.wl.serial = serial;
        _glfw.wl.pointerFocus = null;
        _glfw.wl.cursorPreviousName = null;
        wayland_stopCursorTimer();

        if (window->wl.hovered != 0)
        {
            window->wl.hovered = GLFW_FALSE;
            _glfwInputCursorEnter(window, GLFW_FALSE);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleMotion(void* userData, void* pointer, uint time, int sx, int sy)
    {
        var window = _glfw.wl.pointerFocus;
        if (window == null || window->cursorMode == GLFW_CURSOR_DISABLED)
            return;

        var xpos = wayland_fixedToDouble(sx);
        var ypos = wayland_fixedToDouble(sy);
        window->wl.cursorPosX = xpos;
        window->wl.cursorPosY = ypos;

        if (window->wl.hovered != 0)
        {
            _glfw.wl.cursorPreviousName = null;
            _glfwInputCursorPos(window, xpos, ypos);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleButton(void* userData,
                                            void* pointer,
                                            uint serial,
                                            uint time,
                                            uint button,
                                            uint state)
    {
        var window = _glfw.wl.pointerFocus;
        if (window == null || window->wl.hovered == 0)
            return;

        _glfw.wl.serial = serial;

        _glfwInputMouseClick(window,
            wayland_pointerButtonToGLFW(button),
            state == WL_POINTER_BUTTON_STATE_PRESSED ? GLFW_PRESS : GLFW_RELEASE,
            (int)_glfw.wl.xkb.modifiers);
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxis(void* userData, void* pointer, uint time, uint axis, int value)
    {
        var window = _glfw.wl.pointerFocus;
        if (window == null)
            return;

        if (axis == WL_POINTER_AXIS_HORIZONTAL_SCROLL)
            _glfwInputScroll(window, -wayland_fixedToDouble(value) / 10.0, 0.0);
        else if (axis == WL_POINTER_AXIS_VERTICAL_SCROLL)
            _glfwInputScroll(window, 0.0, -wayland_fixedToDouble(value) / 10.0);
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleFrame(void* userData, void* pointer)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxisSource(void* userData, void* pointer, uint source)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxisStop(void* userData, void* pointer, uint time, uint axis)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxisDiscrete(void* userData, void* pointer, uint axis, int discrete)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxisValue120(void* userData, void* pointer, uint axis, int value120)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_pointerHandleAxisRelativeDirection(void* userData, void* pointer, uint axis, uint direction)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_seatHandleCapabilities(void* userData, void* seat, uint caps)
    {
        if ((caps & WL_SEAT_CAPABILITY_POINTER) != 0)
        {
            if (_glfw.wl.pointer == null)
                wayland_createPointer(seat);
        }
        else if (_glfw.wl.pointer != null)
        {
            if (_glfw.wl.pointerFocus != null && _glfw.wl.pointerFocus->wl.hovered != 0)
            {
                _glfw.wl.pointerFocus->wl.hovered = GLFW_FALSE;
                _glfwInputCursorEnter(_glfw.wl.pointerFocus, GLFW_FALSE);
            }

            wayland_stopCursorTimer();
            wayland_pointerDestroy(_glfw.wl.pointer);
            _glfw.wl.pointer = null;
            _glfw.wl.pointerFocus = null;
        }

        if ((caps & WL_SEAT_CAPABILITY_KEYBOARD) != 0)
        {
            if (_glfw.wl.keyboard == null)
                wayland_createKeyboard(seat);
        }
        else if (_glfw.wl.keyboard != null)
        {
            if (_glfw.wl.keyboardFocus != null)
                _glfwInputWindowFocus(_glfw.wl.keyboardFocus, GLFW_FALSE);

            wayland_keyboardDestroy(_glfw.wl.keyboard);
            _glfw.wl.keyboard = null;
            _glfw.wl.keyboardFocus = null;
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_seatHandleName(void* userData, void* seat, byte* name)
    {
    }

    static void _glfwAddSeatListenerWayland(void* seat)
    {
        if (seat == null)
            return;

        var listener = wayland_getSeatListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(seat, listener, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add seat listener");
        }
    }

    static void* wayland_dataDeviceManagerGetDataDevice(void* manager, void* seat)
    {
        if (manager == null ||
            seat == null ||
            _glfw.wl.client.dataDeviceInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null)
        {
            return null;
        }

        var device = _glfw.wl.client.proxy_marshal_constructor_object(manager,
            WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE,
            _glfw.wl.client.dataDeviceInterface,
            null,
            seat);

        wayland_tagProxy(device);
        return device;
    }

    static void wayland_dataDeviceDestroy(void* device)
    {
        if (device == null)
            return;

        if (_glfw.wl.client.proxy_get_version != null &&
            _glfw.wl.client.proxy_get_version(device) >= WL_DATA_DEVICE_RELEASE_SINCE_VERSION)
        {
            wayland_proxyDestroyWithOpcode(device, WL_DATA_DEVICE_RELEASE);
        }
        else
            wayland_proxyDestroy(device);
    }

    static void wayland_dataOfferAccept(void* offer, uint serial, byte* mimeType)
    {
        if (offer != null && _glfw.wl.client.proxy_marshal_uint_string != null)
            _glfw.wl.client.proxy_marshal_uint_string(offer, WL_DATA_OFFER_ACCEPT, serial, mimeType);
    }

    static void wayland_dataOfferReceive(void* offer, byte* mimeType, int fd)
    {
        if (offer != null && _glfw.wl.client.proxy_marshal_string_int != null)
            _glfw.wl.client.proxy_marshal_string_int(offer, WL_DATA_OFFER_RECEIVE, mimeType, fd);
    }

    static void wayland_dataOfferDestroy(void* offer)
    {
        wayland_proxyDestroyWithOpcode(offer, WL_DATA_OFFER_DESTROY);
    }

    static void _glfwAddDataDeviceListenerWayland(void* device)
    {
        if (device == null)
            return;

        var listener = wayland_getDataDeviceListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(device, listener, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add data device listener");
        }
    }

    static int wayland_offerIndex(void* offer)
    {
        for (uint i = 0; i < _glfw.wl.offerCount; i++)
        {
            if (_glfw.wl.offers[i].offer == offer)
                return (int)i;
        }

        return -1;
    }

    static void wayland_removeOfferAt(uint index)
    {
        if (index >= _glfw.wl.offerCount)
            return;

        _glfw.wl.offers[index] = _glfw.wl.offers[_glfw.wl.offerCount - 1];
        _glfw.wl.offerCount--;
    }

    static byte* wayland_readDataOfferAsString(void* offer, byte* mimeType)
    {
        int* fds = stackalloc int[2];
        if (wayland_pipe2(fds, O_CLOEXEC) == -1)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create pipe for data offer");
            return null;
        }

        wayland_dataOfferReceive(offer, mimeType, fds[1]);
        wayland_flushDisplay();
        wayland_close(fds[1]);

        byte* @string = null;
        nuint size = 0;
        nuint length = 0;

        for (;;)
        {
            const nuint readSize = 4096;
            var requiredSize = length + readSize + 1;
            if (requiredSize > size)
            {
                var longer = (byte*)_glfw_realloc(@string, requiredSize);
                if (longer == null)
                {
                    _glfw_free(@string);
                    wayland_close(fds[0]);
                    _glfwInputError(GLFW_OUT_OF_MEMORY);
                    return null;
                }

                @string = longer;
                size = requiredSize;
            }

            var result = wayland_read(fds[0], @string + length, readSize);
            if (result == 0)
                break;
            if (result == -1)
            {
                if (Marshal.GetLastPInvokeError() == EINTR)
                    continue;

                _glfw_free(@string);
                wayland_close(fds[0]);
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to read from data offer pipe");
                return null;
            }

            length += (nuint)result;
        }

        wayland_close(fds[0]);
        @string[length] = 0;
        return @string;
    }

    [UnmanagedCallersOnly]
    static void wayland_dataOfferHandleOffer(void* userData, void* offer, byte* mimeType)
    {
        var index = wayland_offerIndex(offer);
        if (index < 0)
            return;

        if (wayland_stringEquals(mimeType, "text/plain;charset=utf-8") != 0)
            _glfw.wl.offers[index].text_plain_utf8 = GLFW_TRUE;
        else if (wayland_stringEquals(mimeType, "text/uri-list") != 0)
            _glfw.wl.offers[index].text_uri_list = GLFW_TRUE;
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleDataOffer(void* userData, void* device, void* offer)
    {
        var offers = (_GLFWofferWayland*)_glfw_realloc(_glfw.wl.offers,
            (nuint)((_glfw.wl.offerCount + 1) * (uint)sizeof(_GLFWofferWayland)));
        if (offers == null)
        {
            _glfwInputError(GLFW_OUT_OF_MEMORY);
            return;
        }

        _glfw.wl.offers = offers;
        _glfw.wl.offers[_glfw.wl.offerCount++] = new _GLFWofferWayland { offer = offer };

        var listener = wayland_getDataOfferListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(offer, listener, null) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add data offer listener");
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleEnter(void* userData,
                                             void* device,
                                             uint serial,
                                             void* surface,
                                             int x,
                                             int y,
                                             void* offer)
    {
        if (_glfw.wl.dragOffer != null)
        {
            wayland_dataOfferDestroy(_glfw.wl.dragOffer);
            _glfw.wl.dragOffer = null;
            _glfw.wl.dragFocus = null;
        }

        var index = wayland_offerIndex(offer);
        if (index >= 0)
        {
            _GLFWwindow* window = null;
            if (surface != null && wayland_proxyHasTag(surface) != 0 && _glfw.wl.client.proxy_get_user_data != null)
                window = (_GLFWwindow*)_glfw.wl.client.proxy_get_user_data(surface);

            if (window != null &&
                surface == window->wl.surface &&
                _glfw.wl.offers[index].text_uri_list != 0)
            {
                _glfw.wl.dragOffer = offer;
                _glfw.wl.dragFocus = window;
                _glfw.wl.dragSerial = serial;
            }

            wayland_removeOfferAt((uint)index);
        }

        if (surface == null || wayland_proxyHasTag(surface) == 0)
            return;

        if (_glfw.wl.dragOffer != null)
            wayland_dataOfferAccept(offer, serial, _glfwWaylandTextUriList);
        else
        {
            wayland_dataOfferAccept(offer, serial, null);
            wayland_dataOfferDestroy(offer);
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleLeave(void* userData, void* device)
    {
        if (_glfw.wl.dragOffer != null)
        {
            wayland_dataOfferDestroy(_glfw.wl.dragOffer);
            _glfw.wl.dragOffer = null;
            _glfw.wl.dragFocus = null;
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleMotion(void* userData, void* device, uint time, int x, int y)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleDrop(void* userData, void* device)
    {
        if (_glfw.wl.dragOffer == null || _glfw.wl.dragFocus == null)
            return;

        var @string = wayland_readDataOfferAsString(_glfw.wl.dragOffer, _glfwWaylandTextUriList);
        if (@string != null)
        {
            var count = 0;
            var paths = _glfwParseUriList(@string, &count);
            if (paths != null)
                _glfwInputDrop(_glfw.wl.dragFocus, count, paths);

            for (var i = 0; i < count; i++)
                _glfw_free(paths[i]);
            _glfw_free(paths);
        }

        _glfw_free(@string);
    }

    [UnmanagedCallersOnly]
    static void wayland_dataDeviceHandleSelection(void* userData, void* device, void* offer)
    {
        if (_glfw.wl.selectionOffer != null)
        {
            wayland_dataOfferDestroy(_glfw.wl.selectionOffer);
            _glfw.wl.selectionOffer = null;
        }

        var index = wayland_offerIndex(offer);
        if (index < 0)
            return;

        if (_glfw.wl.offers[index].text_plain_utf8 != 0)
            _glfw.wl.selectionOffer = offer;
        else
            wayland_dataOfferDestroy(offer);

        wayland_removeOfferAt((uint)index);
    }

    static void* wayland_xdgWmBaseGetXdgSurface(void* wmBase, void* surface)
    {
        if (wmBase == null || surface == null || _glfwWaylandXdgSurfaceInterface == null)
            return null;

        var xdgSurface = _glfw.wl.client.proxy_marshal_constructor_object(wmBase,
            XDG_WM_BASE_GET_XDG_SURFACE,
            _glfwWaylandXdgSurfaceInterface,
            null,
            surface);

        wayland_tagProxy(xdgSurface);
        return xdgSurface;
    }

    static void* wayland_xdgSurfaceGetToplevel(void* xdgSurface)
    {
        if (xdgSurface == null || _glfwWaylandXdgToplevelInterface == null)
            return null;

        var toplevel = _glfw.wl.client.proxy_marshal_constructor(xdgSurface,
            XDG_SURFACE_GET_TOPLEVEL,
            _glfwWaylandXdgToplevelInterface,
            null);

        wayland_tagProxy(toplevel);
        return toplevel;
    }

    static void wayland_xdgSurfaceAckConfigure(void* xdgSurface, uint serial)
    {
        if (xdgSurface != null && _glfw.wl.client.proxy_marshal_uint != null)
            _glfw.wl.client.proxy_marshal_uint(xdgSurface, XDG_SURFACE_ACK_CONFIGURE, serial);
    }

    static void wayland_xdgSurfaceSetWindowGeometry(void* xdgSurface, int x, int y, int width, int height)
    {
        if (xdgSurface != null && _glfw.wl.client.proxy_marshal_int_int_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int_int_int(xdgSurface, XDG_SURFACE_SET_WINDOW_GEOMETRY, x, y, width, height);
    }

    static void wayland_xdgToplevelSetTitle(void* toplevel, byte* title)
    {
        if (toplevel != null && title != null && _glfw.wl.client.proxy_marshal_string != null)
            _glfw.wl.client.proxy_marshal_string(toplevel, XDG_TOPLEVEL_SET_TITLE, title);
    }

    static void wayland_xdgToplevelSetAppId(void* toplevel, byte* appId)
    {
        if (toplevel != null && appId != null && _glfw.wl.client.proxy_marshal_string != null)
            _glfw.wl.client.proxy_marshal_string(toplevel, XDG_TOPLEVEL_SET_APP_ID, appId);
    }

    static void wayland_xdgToplevelSetMinSize(void* toplevel, int width, int height)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int(toplevel, XDG_TOPLEVEL_SET_MIN_SIZE, width, height);
    }

    static void wayland_xdgToplevelSetMaxSize(void* toplevel, int width, int height)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_int_int != null)
            _glfw.wl.client.proxy_marshal_int_int(toplevel, XDG_TOPLEVEL_SET_MAX_SIZE, width, height);
    }

    static void wayland_xdgToplevelSetMaximized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_SET_MAXIMIZED);
    }

    static void wayland_xdgToplevelUnsetMaximized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_UNSET_MAXIMIZED);
    }

    static void wayland_xdgToplevelSetFullscreen(void* toplevel, void* output)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal_object != null)
            _glfw.wl.client.proxy_marshal_object(toplevel, XDG_TOPLEVEL_SET_FULLSCREEN, output);
    }

    static void wayland_xdgToplevelUnsetFullscreen(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_UNSET_FULLSCREEN);
    }

    static void wayland_xdgToplevelSetMinimized(void* toplevel)
    {
        if (toplevel != null && _glfw.wl.client.proxy_marshal != null)
            _glfw.wl.client.proxy_marshal(toplevel, XDG_TOPLEVEL_SET_MINIMIZED);
    }

    static void* wayland_zxdgDecorationManagerGetToplevelDecoration(void* manager, void* toplevel)
    {
        if (manager == null ||
            toplevel == null ||
            _glfwWaylandZxdgToplevelDecorationV1Interface == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null)
        {
            return null;
        }

        var decoration = _glfw.wl.client.proxy_marshal_constructor_object(manager,
            ZXDG_DECORATION_MANAGER_GET_TOPLEVEL_DECORATION,
            _glfwWaylandZxdgToplevelDecorationV1Interface,
            null,
            toplevel);

        wayland_tagProxy(decoration);
        return decoration;
    }

    static void wayland_zxdgToplevelDecorationSetMode(void* decoration, uint mode)
    {
        if (decoration != null && _glfw.wl.client.proxy_marshal_uint != null)
            _glfw.wl.client.proxy_marshal_uint(decoration, ZXDG_TOPLEVEL_DECORATION_SET_MODE, mode);
    }

    static uint wayland_getDecorationMode(_GLFWwindow* window)
    {
        return window->decorated != 0
            ? ZXDG_TOPLEVEL_DECORATION_V1_MODE_SERVER_SIDE
            : ZXDG_TOPLEVEL_DECORATION_V1_MODE_CLIENT_SIDE;
    }

    static void wayland_updateXdgDecorationMode(_GLFWwindow* window)
    {
        if (window->wl.xdg.decoration != null)
            wayland_zxdgToplevelDecorationSetMode(window->wl.xdg.decoration, wayland_getDecorationMode(window));
    }

    static void* wayland_idleInhibitManagerCreateInhibitor(void* manager, void* surface)
    {
        if (manager == null ||
            surface == null ||
            _glfwWaylandZwpIdleInhibitorV1Interface == null ||
            _glfw.wl.client.proxy_marshal_constructor_object == null)
        {
            return null;
        }

        var inhibitor = _glfw.wl.client.proxy_marshal_constructor_object(manager,
            ZWP_IDLE_INHIBIT_MANAGER_CREATE_INHIBITOR,
            _glfwWaylandZwpIdleInhibitorV1Interface,
            null,
            surface);

        wayland_tagProxy(inhibitor);
        return inhibitor;
    }

    static void wayland_idleInhibitorDestroy(void* inhibitor)
    {
        wayland_proxyDestroyWithOpcode(inhibitor, ZWP_IDLE_INHIBITOR_DESTROY);
    }

    static void wayland_setIdleInhibitor(_GLFWwindow* window, int enabled)
    {
        if (enabled != 0 && window->wl.idleInhibitor == null && _glfw.wl.idleInhibitManager != null)
        {
            window->wl.idleInhibitor =
                wayland_idleInhibitManagerCreateInhibitor(_glfw.wl.idleInhibitManager, window->wl.surface);
            if (window->wl.idleInhibitor == null)
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create idle inhibitor");
        }
        else if (enabled == 0 && window->wl.idleInhibitor != null)
        {
            wayland_idleInhibitorDestroy(window->wl.idleInhibitor);
            window->wl.idleInhibitor = null;
        }
    }

    static void wayland_updateXdgSizeLimits(_GLFWwindow* window)
    {
        if (window->wl.xdg.toplevel == null)
            return;

        int minwidth;
        int minheight;
        int maxwidth;
        int maxheight;

        if (window->resizable != 0)
        {
            if (window->minwidth == GLFW_DONT_CARE || window->minheight == GLFW_DONT_CARE)
                minwidth = minheight = 0;
            else
            {
                minwidth = window->minwidth;
                minheight = window->minheight;
            }

            if (window->maxwidth == GLFW_DONT_CARE || window->maxheight == GLFW_DONT_CARE)
                maxwidth = maxheight = 0;
            else
            {
                maxwidth = window->maxwidth;
                maxheight = window->maxheight;
            }
        }
        else
        {
            minwidth = maxwidth = window->wl.width;
            minheight = maxheight = window->wl.height;
        }

        wayland_xdgToplevelSetMinSize(window->wl.xdg.toplevel, minwidth, minheight);
        wayland_xdgToplevelSetMaxSize(window->wl.xdg.toplevel, maxwidth, maxheight);
    }

    static int wayland_resizeWindow(_GLFWwindow* window, int width, int height)
    {
        width = _glfw_max(width, 1);
        height = _glfw_max(height, 1);

        if (width == window->wl.width && height == window->wl.height)
            return GLFW_FALSE;

        window->wl.width = width;
        window->wl.height = height;
        wayland_updateFramebufferSize(window);

        if (window->wl.scalingViewport != null)
            wayland_viewportSetDestination(window->wl.scalingViewport, window->wl.width, window->wl.height);

        if (window->wl.xdg.surface != null)
            wayland_xdgSurfaceSetWindowGeometry(window->wl.xdg.surface, 0, 0, window->wl.width, window->wl.height);

        return GLFW_TRUE;
    }

    static void wayland_applyPendingSize(_GLFWwindow* window)
    {
        var width = window->wl.pending.width;
        var height = window->wl.pending.height;

        window->wl.pending.width = 0;
        window->wl.pending.height = 0;

        if (width <= 0 || height <= 0)
            return;

        if (window->wl.maximized == 0 &&
            window->wl.fullscreen == 0 &&
            window->numer != GLFW_DONT_CARE &&
            window->denom != GLFW_DONT_CARE)
        {
            var aspectRatio = (float)width / height;
            var targetRatio = (float)window->numer / window->denom;

            if (aspectRatio < targetRatio)
                height = (int)(width / targetRatio);
            else if (aspectRatio > targetRatio)
                width = (int)(height * targetRatio);
        }

        if (wayland_resizeWindow(window, width, height) == 0)
            return;

        wayland_surfaceCommit(window->wl.surface);

        _glfwInputWindowSize(window, window->wl.width, window->wl.height);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgSurfaceHandleConfigure(void* userData, void* xdgSurface, uint serial)
    {
        var window = (_GLFWwindow*)userData;

        wayland_xdgSurfaceAckConfigure(xdgSurface, serial);

        if (window->wl.activated != window->wl.pending.activated)
        {
            window->wl.activated = window->wl.pending.activated;
            if (window->wl.activated == 0 && window->monitor != null && window->autoIconify != 0)
                wayland_xdgToplevelSetMinimized(window->wl.xdg.toplevel);
        }

        if (window->wl.maximized != window->wl.pending.maximized)
        {
            window->wl.maximized = window->wl.pending.maximized;
            _glfwInputWindowMaximize(window, window->wl.maximized);
        }

        window->wl.fullscreen = window->wl.pending.fullscreen;
        wayland_applyPendingSize(window);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgToplevelHandleConfigure(void* userData,
                                                  void* toplevel,
                                                  int width,
                                                  int height,
                                                  void* states)
    {
        var window = (_GLFWwindow*)userData;

        window->wl.pending.activated = GLFW_FALSE;
        window->wl.pending.maximized = GLFW_FALSE;
        window->wl.pending.fullscreen = GLFW_FALSE;

        if (states != null)
        {
            var array = (wl_array*)states;
            var state = (uint*)array->data;
            var count = array->size / (nuint)sizeof(uint);

            for (nuint i = 0; i < count; i++)
            {
                switch (state[i])
                {
                    case XDG_TOPLEVEL_STATE_MAXIMIZED:
                        window->wl.pending.maximized = GLFW_TRUE;
                        break;
                    case XDG_TOPLEVEL_STATE_FULLSCREEN:
                        window->wl.pending.fullscreen = GLFW_TRUE;
                        break;
                    case XDG_TOPLEVEL_STATE_RESIZING:
                        break;
                    case XDG_TOPLEVEL_STATE_ACTIVATED:
                        window->wl.pending.activated = GLFW_TRUE;
                        break;
                }
            }
        }

        if (width > 0 && height > 0)
        {
            window->wl.pending.width = width;
            window->wl.pending.height = height;
        }
        else
        {
            window->wl.pending.width = window->wl.width;
            window->wl.pending.height = window->wl.height;
        }
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgToplevelHandleClose(void* userData, void* toplevel)
    {
        _glfwInputWindowCloseRequest((_GLFWwindow*)userData);
    }

    [UnmanagedCallersOnly]
    static void wayland_xdgDecorationHandleConfigure(void* userData, void* decoration, uint mode)
    {
        var window = (_GLFWwindow*)userData;
        if (window == null || decoration != window->wl.xdg.decoration)
            return;

        window->wl.xdg.decorationMode = mode;
    }

    static int wayland_createXdgShellObjects(_GLFWwindow* window)
    {
        window->wl.xdg.surface = wayland_xdgWmBaseGetXdgSurface(_glfw.wl.wmBase, window->wl.surface);
        if (window->wl.xdg.surface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create xdg-surface for window");
            return GLFW_FALSE;
        }

        var surfaceListener = wayland_getXdgSurfaceListener();
        if (surfaceListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.xdg.surface, surfaceListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add xdg-surface listener");
            return GLFW_FALSE;
        }

        window->wl.xdg.toplevel = wayland_xdgSurfaceGetToplevel(window->wl.xdg.surface);
        if (window->wl.xdg.toplevel == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create xdg-toplevel for window");
            return GLFW_FALSE;
        }

        var toplevelListener = wayland_getXdgToplevelListener();
        if (toplevelListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.xdg.toplevel, toplevelListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add xdg-toplevel listener");
            return GLFW_FALSE;
        }

        if (window->wl.appId != null)
            wayland_xdgToplevelSetAppId(window->wl.xdg.toplevel, window->wl.appId);

        wayland_xdgToplevelSetTitle(window->wl.xdg.toplevel, window->title);
        wayland_xdgSurfaceSetWindowGeometry(window->wl.xdg.surface, 0, 0, window->wl.width, window->wl.height);

        if (window->monitor != null)
        {
            window->wl.fullscreen = GLFW_TRUE;
            wayland_xdgToplevelSetFullscreen(window->wl.xdg.toplevel, window->monitor->wl.output);
            wayland_setIdleInhibitor(window, GLFW_TRUE);
        }
        else
        {
            if (window->wl.maximized != 0)
                wayland_xdgToplevelSetMaximized(window->wl.xdg.toplevel);

            wayland_setIdleInhibitor(window, GLFW_FALSE);
        }

        if (_glfw.wl.decorationManager != null)
        {
            window->wl.xdg.decoration =
                wayland_zxdgDecorationManagerGetToplevelDecoration(_glfw.wl.decorationManager, window->wl.xdg.toplevel);
            var decorationListener = wayland_getXdgDecorationListener();
            if (window->wl.xdg.decoration == null ||
                decorationListener == null ||
                _glfw.wl.client.proxy_add_listener(window->wl.xdg.decoration, decorationListener, window) != 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create xdg-decoration object");
                return GLFW_FALSE;
            }

            wayland_updateXdgDecorationMode(window);
        }

        wayland_updateXdgSizeLimits(window);
        wayland_surfaceCommit(window->wl.surface);

        if (_glfw.wl.client.display_roundtrip(_glfw.wl.display) < 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to process initial xdg-shell events");
            return GLFW_FALSE;
        }

        window->wl.visible = GLFW_TRUE;
        return GLFW_TRUE;
    }

    static void wayland_destroyShellObjects(_GLFWwindow* window)
    {
        wayland_proxyDestroyWithOpcode(window->wl.xdg.decoration, ZXDG_TOPLEVEL_DECORATION_DESTROY);
        wayland_proxyDestroyWithOpcode(window->wl.xdg.toplevel, XDG_TOPLEVEL_DESTROY);
        wayland_proxyDestroyWithOpcode(window->wl.xdg.surface, XDG_SURFACE_DESTROY);

        window->wl.xdg.toplevel = null;
        window->wl.xdg.surface = null;
        window->wl.xdg.decoration = null;
        window->wl.xdg.decorationMode = 0;
    }

    static int _glfwCreateWindowWayland(_GLFWwindow* window,
                                        _GLFWwndconfig* wndconfig,
                                        _GLFWctxconfig* ctxconfig,
                                        _GLFWfbconfig* fbconfig)
    {
        window->wl.width = wndconfig->width;
        window->wl.height = wndconfig->height;
        window->wl.fbWidth = wndconfig->width;
        window->wl.fbHeight = wndconfig->height;
        window->wl.transparent = fbconfig->transparent;
        window->wl.scaleFramebuffer = wndconfig->scaleFramebuffer;
        window->wl.bufferScale = 1;
        window->wl.scalingNumerator = 120;

        if (wndconfig->wl.appId[0] != 0)
            window->wl.appId = _glfw_strdup(wndconfig->wl.appId);

        window->wl.maximized = wndconfig->maximized;

        if (_glfw.wl.compositor == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: No wl_compositor available");
            return GLFW_FALSE;
        }

        window->wl.surface = wayland_compositorCreateSurface();
        if (window->wl.surface == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create window surface");
            return GLFW_FALSE;
        }

        var surfaceListener = wayland_getSurfaceListener();
        if (surfaceListener == null ||
            _glfw.wl.client.proxy_add_listener(window->wl.surface, surfaceListener, window) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add surface listener");
            return GLFW_FALSE;
        }

        if (ctxconfig->client != GLFW_NO_API)
        {
            if (ctxconfig->source == GLFW_NATIVE_CONTEXT_API ||
                ctxconfig->source == GLFW_EGL_CONTEXT_API)
            {
                if (_glfw.wl.egl.window_create == null)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to load libwayland-egl entry point");
                    return GLFW_FALSE;
                }

                window->wl.egl.window = _glfw.wl.egl.window_create(window->wl.surface,
                    window->wl.fbWidth,
                    window->wl.fbHeight);
                if (window->wl.egl.window == null)
                {
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create EGL window");
                    return GLFW_FALSE;
                }

                if (_glfwInitEGL() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextEGL(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }
            else if (ctxconfig->source == GLFW_OSMESA_CONTEXT_API)
            {
                if (_glfwInitOSMesa() == 0)
                    return GLFW_FALSE;
                if (_glfwCreateContextOSMesa(window, ctxconfig, fbconfig) == 0)
                    return GLFW_FALSE;
            }

            if (_glfwRefreshContextAttribs(window, ctxconfig) == 0)
                return GLFW_FALSE;
        }

        if (window->wl.transparent == 0)
            wayland_setContentAreaOpaque(window);

        if (_glfw.wl.fractionalScaleManager != null &&
            _glfw.wl.viewporter != null &&
            window->wl.scaleFramebuffer != 0)
        {
            window->wl.scalingViewport = wayland_viewporterGetViewport(_glfw.wl.viewporter, window->wl.surface);
            if (window->wl.scalingViewport != null)
                wayland_viewportSetDestination(window->wl.scalingViewport, window->wl.width, window->wl.height);

            window->wl.fractionalScale =
                wayland_fractionalScaleManagerGetFractionalScale(_glfw.wl.fractionalScaleManager, window->wl.surface);
            var fractionalScaleListener = wayland_getFractionalScaleListener();
            if (window->wl.fractionalScale != null &&
                fractionalScaleListener != null &&
                _glfw.wl.client.proxy_add_listener(window->wl.fractionalScale, fractionalScaleListener, window) != 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add fractional-scale listener");
                return GLFW_FALSE;
            }
        }

        if (wndconfig->mousePassthrough != 0)
            _glfwSetWindowMousePassthroughWayland(window, GLFW_TRUE);

        if (window->monitor != null || wndconfig->visible != 0)
        {
            if (wayland_createXdgShellObjects(window) == 0)
                return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyWindowWayland(_GLFWwindow* window)
    {
        if (window == _glfw.wl.pointerFocus)
            _glfw.wl.pointerFocus = null;
        if (window == _glfw.wl.keyboardFocus)
            _glfw.wl.keyboardFocus = null;

        if (window->context.destroy != null)
            window->context.destroy(window);

        wayland_destroyShellObjects(window);

        if (window->wl.egl.window != null && _glfw.wl.egl.window_destroy != null)
            _glfw.wl.egl.window_destroy(window->wl.egl.window);

        wayland_setIdleInhibitor(window, GLFW_FALSE);
        wayland_fractionalScaleDestroy(window->wl.fractionalScale);
        wayland_viewportDestroy(window->wl.scalingViewport);
        wayland_proxyDestroyWithOpcode(window->wl.surface, WL_SURFACE_DESTROY);

        _glfw_free(window->wl.appId);
        _glfw_free(window->wl.outputScales);
        window->wl = default;
    }

    static void _glfwSetWindowTitleWayland(_GLFWwindow* window, byte* title)
    {
        wayland_xdgToplevelSetTitle(window->wl.xdg.toplevel, title);
    }

    static void _glfwSetWindowIconWayland(_GLFWwindow* window, int count, GLFWimage* images)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window icon");
    }

    static void _glfwGetWindowPosWayland(_GLFWwindow* window, int* xpos, int* ypos)
    {
        if (xpos != null)
            *xpos = 0;
        if (ypos != null)
            *ypos = 0;

        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not provide the window position");
    }

    static void _glfwSetWindowPosWayland(_GLFWwindow* window, int xpos, int ypos)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window position");
    }

    static void _glfwGetWindowSizeWayland(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->wl.width;
        if (height != null)
            *height = window->wl.height;
    }

    static void _glfwSetWindowSizeWayland(_GLFWwindow* window, int width, int height)
    {
        if (window->monitor != null)
            return;

        if (wayland_resizeWindow(window, width, height) == 0)
            return;

        _glfwInputWindowSize(window, window->wl.width, window->wl.height);

        if (window->wl.visible != 0)
        {
            wayland_surfaceCommit(window->wl.surface);
            _glfwInputWindowDamage(window);
        }
    }

    static void _glfwSetWindowSizeLimitsWayland(_GLFWwindow* window,
                                                int minwidth,
                                                int minheight,
                                                int maxwidth,
                                                int maxheight)
    {
        wayland_updateXdgSizeLimits(window);
    }

    static void _glfwSetWindowAspectRatioWayland(_GLFWwindow* window, int numer, int denom)
    {
        if (window->wl.maximized != 0 || window->wl.fullscreen != 0)
            return;

        var width = window->wl.width;
        var height = window->wl.height;

        if (numer != GLFW_DONT_CARE && denom != GLFW_DONT_CARE)
        {
            var aspectRatio = (float)width / height;
            var targetRatio = (float)numer / denom;

            if (aspectRatio < targetRatio)
                height = (int)(width / targetRatio);
            else if (aspectRatio > targetRatio)
                width = (int)(height * targetRatio);
        }

        if (wayland_resizeWindow(window, width, height) == 0)
            return;

        _glfwInputWindowSize(window, window->wl.width, window->wl.height);

        if (window->wl.visible != 0)
        {
            wayland_surfaceCommit(window->wl.surface);
            _glfwInputWindowDamage(window);
        }
    }

    static void _glfwGetFramebufferSizeWayland(_GLFWwindow* window, int* width, int* height)
    {
        if (width != null)
            *width = window->wl.fbWidth;
        if (height != null)
            *height = window->wl.fbHeight;
    }

    static void _glfwGetWindowFrameSizeWayland(_GLFWwindow* window,
                                               int* left,
                                               int* top,
                                               int* right,
                                               int* bottom)
    {
        if (left != null)
            *left = 0;
        if (top != null)
            *top = 0;
        if (right != null)
            *right = 0;
        if (bottom != null)
            *bottom = 0;
    }

    static void _glfwGetWindowContentScaleWayland(_GLFWwindow* window, float* xscale, float* yscale)
    {
        var scale = window->wl.fractionalScale != null
            ? window->wl.scalingNumerator / 120f
            : window->wl.bufferScale != 0 ? window->wl.bufferScale : 1f;

        if (xscale != null)
            *xscale = scale;
        if (yscale != null)
            *yscale = scale;
    }

    static void _glfwIconifyWindowWayland(_GLFWwindow* window)
    {
        wayland_xdgToplevelSetMinimized(window->wl.xdg.toplevel);
    }

    static void _glfwRestoreWindowWayland(_GLFWwindow* window)
    {
        if (window->wl.maximized != 0)
            wayland_xdgToplevelUnsetMaximized(window->wl.xdg.toplevel);

        window->wl.maximized = GLFW_FALSE;
    }

    static void _glfwMaximizeWindowWayland(_GLFWwindow* window)
    {
        window->wl.maximized = GLFW_TRUE;
        wayland_xdgToplevelSetMaximized(window->wl.xdg.toplevel);
    }

    static void _glfwShowWindowWayland(_GLFWwindow* window)
    {
        if (window->wl.xdg.surface == null &&
            wayland_createXdgShellObjects(window) == 0)
        {
            return;
        }

        window->wl.visible = GLFW_TRUE;
    }

    static void _glfwHideWindowWayland(_GLFWwindow* window)
    {
        wayland_destroyShellObjects(window);
        wayland_surfaceCommit(window->wl.surface);
        window->wl.visible = GLFW_FALSE;
    }

    static void _glfwRequestWindowAttentionWayland(_GLFWwindow* window)
    {
    }

    static void _glfwFocusWindowWayland(_GLFWwindow* window)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the input focus");
    }

    static void _glfwSetWindowMonitorWayland(_GLFWwindow* window,
                                             _GLFWmonitor* monitor,
                                             int xpos,
                                             int ypos,
                                             int width,
                                             int height,
                                             int refreshRate)
    {
        _glfwInputWindowMonitor(window, monitor);

        if (monitor == null)
        {
            if (window->wl.fullscreen != 0)
                wayland_xdgToplevelUnsetFullscreen(window->wl.xdg.toplevel);
            wayland_setIdleInhibitor(window, GLFW_FALSE);
            window->wl.fullscreen = GLFW_FALSE;
            _glfwSetWindowSizeWayland(window, width, height);
        }
        else
        {
            window->wl.fullscreen = GLFW_TRUE;
            wayland_xdgToplevelSetFullscreen(window->wl.xdg.toplevel, monitor->wl.output);
            wayland_setIdleInhibitor(window, GLFW_TRUE);
        }

        wayland_updateXdgDecorationMode(window);
    }

    static int _glfwWindowFocusedWayland(_GLFWwindow* window)
    {
        return _glfw.wl.keyboardFocus == window ? GLFW_TRUE : GLFW_FALSE;
    }

    static int _glfwWindowIconifiedWayland(_GLFWwindow* window)
    {
        return GLFW_FALSE;
    }

    static int _glfwWindowVisibleWayland(_GLFWwindow* window)
    {
        return window->wl.visible;
    }

    static int _glfwWindowMaximizedWayland(_GLFWwindow* window)
    {
        return window->wl.maximized;
    }

    static int _glfwWindowHoveredWayland(_GLFWwindow* window)
    {
        return window->wl.hovered;
    }

    static int _glfwFramebufferTransparentWayland(_GLFWwindow* window)
    {
        return window->wl.transparent;
    }

    static void _glfwSetWindowResizableWayland(_GLFWwindow* window, int enabled)
    {
        window->resizable = enabled;
        wayland_updateXdgSizeLimits(window);
    }

    static void _glfwSetWindowDecoratedWayland(_GLFWwindow* window, int enabled)
    {
        window->decorated = enabled;
        wayland_updateXdgDecorationMode(window);
    }

    static void _glfwSetWindowFloatingWayland(_GLFWwindow* window, int enabled)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: Platform does not support making a window floating");
    }

    static void _glfwSetWindowMousePassthroughWayland(_GLFWwindow* window, int enabled)
    {
        window->mousePassthrough = enabled;

        if (window->wl.surface == null)
            return;

        if (enabled != 0)
        {
            var region = wayland_compositorCreateRegion();
            wayland_surfaceSetInputRegion(window->wl.surface, region);
            wayland_regionDestroy(region);
        }
        else
            wayland_surfaceSetInputRegion(window->wl.surface, null);
    }

    static float _glfwGetWindowOpacityWayland(_GLFWwindow* window)
    {
        return 1f;
    }

    static void _glfwSetWindowOpacityWayland(_GLFWwindow* window, float opacity)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the window opacity");
    }

    static void _glfwSetRawMouseMotionWayland(_GLFWwindow* window, int enabled)
    {
    }

    static int _glfwRawMouseMotionSupportedWayland()
    {
        return _glfw.wl.relativePointerManager != null ? GLFW_TRUE : GLFW_FALSE;
    }

    static int wayland_flushDisplay()
    {
        if (_glfw.wl.display == null ||
            _glfw.wl.client.display_flush == null ||
            _glfw.wl.client.display_get_fd == null)
        {
            return GLFW_FALSE;
        }

        while (_glfw.wl.client.display_flush(_glfw.wl.display) == -1)
        {
            POLLFD fd = new()
            {
                fd = _glfw.wl.client.display_get_fd(_glfw.wl.display),
                events = POLLOUT
            };

            if (_glfwPollPOSIX(&fd, 1, null) == 0)
                return GLFW_FALSE;
        }

        return GLFW_TRUE;
    }

    static void wayland_dispatchCloseRequests()
    {
        var window = _glfw.windowListHead;
        while (window != null)
        {
            _glfwInputWindowCloseRequest(window);
            window = window->next;
        }
    }

    static void wayland_handleEvents(double* timeout)
    {
        if (_glfw.wl.display == null ||
            _glfw.wl.client.display_prepare_read == null ||
            _glfw.wl.client.display_cancel_read == null ||
            _glfw.wl.client.display_read_events == null ||
            _glfw.wl.client.display_dispatch_pending == null ||
            _glfw.wl.client.display_get_fd == null)
        {
            return;
        }

        var @event = GLFW_FALSE;
        const int DISPLAY_FD = 0;
        const int KEYREPEAT_FD = 1;
        const int CURSOR_FD = 2;
        POLLFD* fds = stackalloc POLLFD[3];
        fds[DISPLAY_FD] = new POLLFD
        {
            fd = _glfw.wl.client.display_get_fd(_glfw.wl.display),
            events = POLLIN
        };
        fds[KEYREPEAT_FD] = new POLLFD
        {
            fd = _glfw.wl.keyRepeatTimerfd,
            events = POLLIN
        };
        fds[CURSOR_FD] = new POLLFD
        {
            fd = _glfw.wl.cursorTimerfd,
            events = POLLIN
        };

        while (@event == 0)
        {
            while (_glfw.wl.client.display_prepare_read(_glfw.wl.display) != 0)
            {
                if (_glfw.wl.client.display_dispatch_pending(_glfw.wl.display) > 0)
                    return;
            }

            if (wayland_flushDisplay() == 0)
            {
                _glfw.wl.client.display_cancel_read(_glfw.wl.display);
                wayland_dispatchCloseRequests();
                return;
            }

            fds[DISPLAY_FD].revents = 0;
            fds[KEYREPEAT_FD].fd = _glfw.wl.keyRepeatTimerfd;
            fds[KEYREPEAT_FD].revents = 0;
            fds[CURSOR_FD].fd = _glfw.wl.cursorTimerfd;
            fds[CURSOR_FD].revents = 0;
            if (_glfwPollPOSIX(fds, 3, timeout) == 0)
            {
                _glfw.wl.client.display_cancel_read(_glfw.wl.display);
                return;
            }

            if ((fds[DISPLAY_FD].revents & POLLIN) != 0)
            {
                _glfw.wl.client.display_read_events(_glfw.wl.display);
                if (_glfw.wl.client.display_dispatch_pending(_glfw.wl.display) > 0)
                    @event = GLFW_TRUE;
            }
            else
                _glfw.wl.client.display_cancel_read(_glfw.wl.display);

            if ((fds[KEYREPEAT_FD].revents & POLLIN) != 0)
            {
                wayland_dispatchKeyRepeats();
                @event = GLFW_TRUE;
            }

            if ((fds[CURSOR_FD].revents & POLLIN) != 0)
            {
                wayland_dispatchCursorTimer();
                @event = GLFW_TRUE;
            }
        }
    }

    static void wayland_displaySync()
    {
        if (_glfw.wl.display == null ||
            _glfw.wl.client.callbackInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor == null)
        {
            return;
        }

        _glfw.wl.client.proxy_marshal_constructor(_glfw.wl.display,
            WL_DISPLAY_SYNC,
            _glfw.wl.client.callbackInterface,
            null);
    }

    static void _glfwPollEventsWayland()
    {
        var timeout = 0.0;
        wayland_handleEvents(&timeout);
    }

    static void _glfwWaitEventsWayland()
    {
        wayland_handleEvents(null);
    }

    static void _glfwWaitEventsTimeoutWayland(double timeout)
    {
        wayland_handleEvents(&timeout);
    }

    static void _glfwPostEmptyEventWayland()
    {
        wayland_displaySync();
        wayland_flushDisplay();
    }

    static void _glfwGetCursorPosWayland(_GLFWwindow* window, double* xpos, double* ypos)
    {
        if (xpos != null)
            *xpos = window->wl.cursorPosX;
        if (ypos != null)
            *ypos = window->wl.cursorPosY;
    }

    static void _glfwSetCursorPosWayland(_GLFWwindow* window, double x, double y)
    {
        _glfwInputError(GLFW_FEATURE_UNAVAILABLE, "Wayland: The platform does not support setting the cursor position");
    }

    static void _glfwSetCursorModeWayland(_GLFWwindow* window, int mode)
    {
        _glfwSetCursorWayland(window, window->wl.currentCursor);
    }

    static byte* _glfwGetScancodeNameWayland(int scancode)
    {
        if (scancode < 0 || scancode > 255)
        {
            _glfwInputError(GLFW_INVALID_VALUE, "Wayland: Invalid scancode {0}", scancode);
            return null;
        }

        if (_glfw.wl.xkb.keymap == null ||
            _glfw.wl.xkb.state == null ||
            _glfw.wl.xkb.state_key_get_layout == null ||
            _glfw.wl.xkb.keymap_key_get_syms_by_level == null)
        {
            return null;
        }

        var key = GLFW_KEY_UNKNOWN;
        fixed (short* keycodes = _glfw.wl.keycodes)
            key = keycodes[scancode];

        if (key == GLFW_KEY_UNKNOWN)
            return null;

        var keycode = (uint)scancode + 8;
        var layout = _glfw.wl.xkb.state_key_get_layout(_glfw.wl.xkb.state, keycode);
        if (layout == XKB_LAYOUT_INVALID)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to retrieve layout for key name");
            return null;
        }

        uint* keysyms = null;
        _glfw.wl.xkb.keymap_key_get_syms_by_level(_glfw.wl.xkb.keymap,
            keycode,
            layout,
            0,
            &keysyms);
        if (keysyms == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to retrieve keysym for key name");
            return null;
        }

        var codepoint = _glfwKeySym2Unicode(keysyms[0]);
        if (codepoint == GLFW_INVALID_CODEPOINT)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to retrieve codepoint for key name");
            return null;
        }

        fixed (byte* keyname = _glfw.wl.xkb.keyname)
        {
            var count = _glfwEncodeUTF8(keyname, codepoint);
            if (count == 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to encode codepoint for key name");
                return null;
            }

            keyname[count] = 0;
            return keyname;
        }
    }

    static int _glfwGetKeyScancodeWayland(int key)
    {
        return key >= 0 && key <= GLFW_KEY_LAST ? _glfw.wl.scancodes[key] : -1;
    }

    static int _glfwCreateCursorWayland(_GLFWcursor* cursor, GLFWimage* image, int xhot, int yhot)
    {
        cursor->wl.buffer = wayland_createShmBuffer(image);
        if (cursor->wl.buffer == null)
            return GLFW_FALSE;

        cursor->wl.width = image->width;
        cursor->wl.height = image->height;
        cursor->wl.xhot = xhot;
        cursor->wl.yhot = yhot;
        return GLFW_TRUE;
    }

    static string? wayland_getXdgCursorName(int shape)
    {
        return shape switch
        {
            GLFW_ARROW_CURSOR => "default",
            GLFW_IBEAM_CURSOR => "text",
            GLFW_CROSSHAIR_CURSOR => "crosshair",
            GLFW_POINTING_HAND_CURSOR => "pointer",
            GLFW_RESIZE_EW_CURSOR => "ew-resize",
            GLFW_RESIZE_NS_CURSOR => "ns-resize",
            GLFW_RESIZE_NWSE_CURSOR => "nwse-resize",
            GLFW_RESIZE_NESW_CURSOR => "nesw-resize",
            GLFW_RESIZE_ALL_CURSOR => "all-scroll",
            GLFW_NOT_ALLOWED_CURSOR => "not-allowed",
            _ => null
        };
    }

    static string? wayland_getCoreCursorName(int shape)
    {
        return shape switch
        {
            GLFW_ARROW_CURSOR => "left_ptr",
            GLFW_IBEAM_CURSOR => "xterm",
            GLFW_CROSSHAIR_CURSOR => "crosshair",
            GLFW_POINTING_HAND_CURSOR => "hand2",
            GLFW_RESIZE_EW_CURSOR => "sb_h_double_arrow",
            GLFW_RESIZE_NS_CURSOR => "sb_v_double_arrow",
            GLFW_RESIZE_ALL_CURSOR => "fleur",
            _ => null
        };
    }

    static int _glfwCreateStandardCursorWayland(_GLFWcursor* cursor, int shape)
    {
        var name = wayland_getXdgCursorName(shape);
        if (name == null)
        {
            _glfwInputError(GLFW_CURSOR_UNAVAILABLE, "Wayland: Standard cursor shape unavailable");
            return GLFW_FALSE;
        }

        cursor->wl.cursor = wayland_themeGetCursor(_glfw.wl.cursorTheme, name);
        if (_glfw.wl.cursorThemeHiDPI != null)
            cursor->wl.cursorHiDPI = wayland_themeGetCursor(_glfw.wl.cursorThemeHiDPI, name);

        if (cursor->wl.cursor == null)
        {
            name = wayland_getCoreCursorName(shape);
            if (name == null)
            {
                _glfwInputError(GLFW_CURSOR_UNAVAILABLE, "Wayland: Standard cursor shape unavailable");
                return GLFW_FALSE;
            }

            cursor->wl.cursor = wayland_themeGetCursor(_glfw.wl.cursorTheme, name);
            if (cursor->wl.cursor == null)
            {
                _glfwInputError(GLFW_CURSOR_UNAVAILABLE, "Wayland: Failed to create standard cursor \"{0}\"", name);
                return GLFW_FALSE;
            }

            if (_glfw.wl.cursorThemeHiDPI != null && cursor->wl.cursorHiDPI == null)
                cursor->wl.cursorHiDPI = wayland_themeGetCursor(_glfw.wl.cursorThemeHiDPI, name);
        }

        return GLFW_TRUE;
    }

    static void _glfwDestroyCursorWayland(_GLFWcursor* cursor)
    {
        if (cursor->wl.cursor != null)
            return;

        if (cursor->wl.buffer != null)
            wayland_proxyDestroyWithOpcode(cursor->wl.buffer, WL_BUFFER_DESTROY);
        cursor->wl = default;
    }

    static void wayland_setCursorImage(_GLFWwindow* window, _GLFWcursorWayland* cursorWayland)
    {
        if (_glfw.wl.pointer == null || _glfw.wl.cursorSurface == null)
            return;

        if (cursorWayland->cursor != null)
        {
            var cursor = (wl_cursor*)cursorWayland->cursor;
            var scale = 1;

            if (window->wl.bufferScale > 1 && cursorWayland->cursorHiDPI != null)
            {
                cursor = (wl_cursor*)cursorWayland->cursorHiDPI;
                scale = 2;
            }

            if (cursor == null || cursor->image_count == 0 || cursor->images == null)
                return;

            if (cursorWayland->currentImage < 0 ||
                (uint)cursorWayland->currentImage >= cursor->image_count)
            {
                cursorWayland->currentImage = 0;
            }

            var image = cursor->images[cursorWayland->currentImage];
            if (image == null)
                return;

            var buffer = _glfw.wl.cursor.image_get_buffer != null
                ? _glfw.wl.cursor.image_get_buffer(image)
                : null;
            if (buffer == null)
                return;

            cursorWayland->width = (int)image->width;
            cursorWayland->height = (int)image->height;
            cursorWayland->xhot = (int)image->hotspot_x;
            cursorWayland->yhot = (int)image->hotspot_y;

            if (_glfw.wl.cursorTimerfd >= 0)
            {
                ITIMERSPEC timer = default;
                timer.it_value.tv_sec = (nint)(image->delay / 1000);
                timer.it_value.tv_nsec = (nint)((image->delay % 1000) * 1000000);
                wayland_timerfd_settime(_glfw.wl.cursorTimerfd, 0, &timer, null);
            }

            wayland_pointerSetCursor(_glfw.wl.pointer,
                _glfw.wl.pointerEnterSerial,
                _glfw.wl.cursorSurface,
                cursorWayland->xhot / scale,
                cursorWayland->yhot / scale);
            wayland_surfaceSetBufferScale(_glfw.wl.cursorSurface, scale);
            wayland_surfaceAttach(_glfw.wl.cursorSurface, buffer, 0, 0);
            wayland_surfaceDamage(_glfw.wl.cursorSurface, 0, 0, cursorWayland->width, cursorWayland->height);
            wayland_surfaceCommit(_glfw.wl.cursorSurface);
            return;
        }

        if (cursorWayland->buffer != null)
        {
            wayland_stopCursorTimer();
            wayland_pointerSetCursor(_glfw.wl.pointer,
                _glfw.wl.pointerEnterSerial,
                _glfw.wl.cursorSurface,
                cursorWayland->xhot,
                cursorWayland->yhot);
            wayland_surfaceSetBufferScale(_glfw.wl.cursorSurface, 1);
            wayland_surfaceAttach(_glfw.wl.cursorSurface, cursorWayland->buffer, 0, 0);
            wayland_surfaceDamage(_glfw.wl.cursorSurface, 0, 0, cursorWayland->width, cursorWayland->height);
            wayland_surfaceCommit(_glfw.wl.cursorSurface);
        }
    }

    static void wayland_dispatchCursorTimer()
    {
        if (_glfw.wl.cursorTimerfd < 0)
            return;

        ulong repeats;
        if (wayland_read(_glfw.wl.cursorTimerfd, &repeats, (nuint)sizeof(ulong)) != sizeof(ulong))
            return;

        var window = _glfw.wl.pointerFocus;
        if (window == null || window->wl.hovered == 0)
            return;

        var cursor = window->wl.currentCursor;
        if (cursor == null || cursor->wl.cursor == null)
            return;

        var wlCursor = (wl_cursor*)cursor->wl.cursor;
        if (wlCursor->image_count == 0)
            return;

        cursor->wl.currentImage++;
        cursor->wl.currentImage %= (int)wlCursor->image_count;
        wayland_setCursorImage(window, &cursor->wl);
    }

    static void _glfwSetCursorWayland(_GLFWwindow* window, _GLFWcursor* cursor)
    {
        window->wl.currentCursor = cursor;

        if (_glfw.wl.pointer == null || _glfw.wl.pointerFocus != window)
            return;

        if (window->cursorMode == GLFW_CURSOR_HIDDEN ||
            window->cursorMode == GLFW_CURSOR_DISABLED)
        {
            wayland_stopCursorTimer();
            wayland_pointerSetCursor(_glfw.wl.pointer, _glfw.wl.pointerEnterSerial, null, 0, 0);
        }
        else
        {
            if (cursor != null)
                wayland_setCursorImage(window, &cursor->wl);
            else
            {
                _GLFWcursorWayland defaultCursor = default;
                defaultCursor.cursor = wayland_themeGetCursor(_glfw.wl.cursorTheme, "left_ptr");
                if (_glfw.wl.cursorThemeHiDPI != null)
                    defaultCursor.cursorHiDPI = wayland_themeGetCursor(_glfw.wl.cursorThemeHiDPI, "left_ptr");

                if (defaultCursor.cursor == null)
                    _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Standard cursor not found");
                else
                    wayland_setCursorImage(window, &defaultCursor);
            }
        }
    }

    static void* wayland_dataDeviceManagerCreateDataSource(void* manager)
    {
        if (manager == null ||
            _glfw.wl.client.dataSourceInterface == null ||
            _glfw.wl.client.proxy_marshal_constructor == null)
        {
            return null;
        }

        var source = _glfw.wl.client.proxy_marshal_constructor(manager,
            WL_DATA_DEVICE_MANAGER_CREATE_DATA_SOURCE,
            _glfw.wl.client.dataSourceInterface,
            null);

        wayland_tagProxy(source);
        return source;
    }

    static void wayland_dataSourceOffer(void* source, byte* mimeType)
    {
        if (source != null && mimeType != null && _glfw.wl.client.proxy_marshal_string != null)
            _glfw.wl.client.proxy_marshal_string(source, WL_DATA_SOURCE_OFFER, mimeType);
    }

    static void wayland_dataSourceDestroy(void* source)
    {
        wayland_proxyDestroyWithOpcode(source, WL_DATA_SOURCE_DESTROY);
    }

    static void wayland_dataDeviceSetSelection(void* device, void* source, uint serial)
    {
        if (device != null && _glfw.wl.client.proxy_marshal_object_uint != null)
            _glfw.wl.client.proxy_marshal_object_uint(device, WL_DATA_DEVICE_SET_SELECTION, source, serial);
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleTarget(void* userData, void* source, byte* mimeType)
    {
        if (_glfw.wl.selectionSource != source)
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Unknown clipboard data source");
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleSend(void* userData, void* source, byte* mimeType, int fd)
    {
        if (_glfw.wl.selectionSource != source ||
            wayland_stringEquals(mimeType, "text/plain;charset=utf-8") == 0)
        {
            wayland_close(fd);
            return;
        }

        var @string = _glfw.wl.clipboardString;
        var length = (nuint)_glfw_strlen(@string);

        while (length > 0)
        {
            var result = wayland_write(fd, @string, length);
            if (result == -1)
            {
                if (Marshal.GetLastPInvokeError() == EINTR)
                    continue;

                _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Error while writing the clipboard");
                break;
            }

            length -= (nuint)result;
            @string += result;
        }

        wayland_close(fd);
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleCancelled(void* userData, void* source)
    {
        wayland_dataSourceDestroy(source);

        if (_glfw.wl.selectionSource == source)
            _glfw.wl.selectionSource = null;
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleDndDropPerformed(void* userData, void* source)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleDndFinished(void* userData, void* source)
    {
    }

    [UnmanagedCallersOnly]
    static void wayland_dataSourceHandleAction(void* userData, void* source, uint action)
    {
    }

    static void _glfwSetClipboardStringWayland(byte* @string)
    {
        if (_glfw.wl.selectionSource != null)
        {
            wayland_dataSourceDestroy(_glfw.wl.selectionSource);
            _glfw.wl.selectionSource = null;
        }

        var copy = _glfw_strdup(@string);
        if (copy == null)
        {
            _glfwInputError(GLFW_OUT_OF_MEMORY);
            return;
        }

        _glfw_free(_glfw.wl.clipboardString);
        _glfw.wl.clipboardString = copy;

        if (_glfw.wl.dataDeviceManager == null || _glfw.wl.dataDevice == null)
            return;

        _glfw.wl.selectionSource = wayland_dataDeviceManagerCreateDataSource(_glfw.wl.dataDeviceManager);
        if (_glfw.wl.selectionSource == null)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create clipboard data source");
            return;
        }

        var listener = wayland_getDataSourceListener();
        if (listener == null ||
            _glfw.wl.client.proxy_add_listener(_glfw.wl.selectionSource, listener, null) != 0)
        {
            wayland_dataSourceDestroy(_glfw.wl.selectionSource);
            _glfw.wl.selectionSource = null;
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to add data source listener");
            return;
        }

        wayland_dataSourceOffer(_glfw.wl.selectionSource, _glfwWaylandTextPlainUtf8);
        wayland_dataDeviceSetSelection(_glfw.wl.dataDevice, _glfw.wl.selectionSource, _glfw.wl.serial);
    }

    static byte* _glfwGetClipboardStringWayland()
    {
        if (_glfw.wl.selectionOffer != null)
        {
            if (_glfw.wl.selectionSource != null)
                return _glfw.wl.clipboardString;

            _glfw_free(_glfw.wl.clipboardString);
            _glfw.wl.clipboardString = wayland_readDataOfferAsString(_glfw.wl.selectionOffer, _glfwWaylandTextPlainUtf8);
            return _glfw.wl.clipboardString;
        }

        if (_glfw.wl.clipboardString == null)
        {
            _glfwInputError(GLFW_FORMAT_UNAVAILABLE, "Wayland: No clipboard data available");
            return null;
        }

        return _glfw.wl.clipboardString;
    }

    static int _glfwGetEGLPlatformWayland(int** attribs)
    {
        if (attribs != null)
            *attribs = null;

        if (_glfw.egl.EXT_platform_base != 0 && _glfw.egl.EXT_platform_wayland != 0)
            return EGL_PLATFORM_WAYLAND_EXT;

        return 0;
    }

    static void* _glfwGetEGLNativeDisplayWayland()
    {
        return _glfw.wl.display;
    }

    static void* _glfwGetEGLNativeWindowWayland(_GLFWwindow* window)
    {
        return window->wl.egl.window;
    }

    static void _glfwGetRequiredInstanceExtensionsWayland(byte** extensions)
    {
        if (_glfw.vk.KHR_surface == 0 || _glfw.vk.KHR_wayland_surface == 0)
            return;

        extensions[0] = _glfwVkKHRSurfaceExtensionName;
        extensions[1] = _glfwVkKHRWaylandSurfaceExtensionName;
    }

    static int _glfwGetPhysicalDevicePresentationSupportWayland(void* instance, void* device, uint queuefamily)
    {
        var proc = (delegate* unmanaged<void*, uint, void*, int>)
            vulkan_getInstanceProcAddress(instance, "vkGetPhysicalDeviceWaylandPresentationSupportKHR");
        if (proc == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Wayland: Vulkan instance missing VK_KHR_wayland_surface extension");
            return GLFW_FALSE;
        }

        return proc(device, queuefamily, _glfw.wl.display);
    }

    static int _glfwCreateWindowSurfaceWayland(void* instance, _GLFWwindow* window, void* allocator, ulong* surface)
    {
        var vkCreateWaylandSurfaceKHR = (delegate* unmanaged<void*, VkWaylandSurfaceCreateInfoKHR*, void*, ulong*, int>)
            vulkan_getInstanceProcAddress(instance, "vkCreateWaylandSurfaceKHR");
        if (vkCreateWaylandSurfaceKHR == null)
        {
            _glfwInputError(GLFW_API_UNAVAILABLE,
                "Wayland: Vulkan instance missing VK_KHR_wayland_surface extension");
            return VK_ERROR_EXTENSION_NOT_PRESENT;
        }

        VkWaylandSurfaceCreateInfoKHR sci = default;
        sci.sType = VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR;
        sci.display = _glfw.wl.display;
        sci.surface = window->wl.surface;

        var err = vkCreateWaylandSurfaceKHR(instance, &sci, allocator, surface);
        if (err != VK_SUCCESS)
            _glfwInputError(GLFW_PLATFORM_ERROR, "Wayland: Failed to create Vulkan surface: {0}", vulkan_resultString(err));

        return err;
    }

    public static void* glfwGetWaylandDisplay()
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WAYLAND)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not initialized");
            return null;
        }

        return _glfw.wl.display;
    }

    public static void* glfwGetWaylandWindow(GLFWwindow* window)
    {
        if (_glfw.initialized == 0)
        {
            _glfwInputError(GLFW_NOT_INITIALIZED);
            return null;
        }

        if (_glfw.platform.platformID != GLFW_PLATFORM_WAYLAND)
        {
            _glfwInputError(GLFW_PLATFORM_UNAVAILABLE, "Wayland: Platform not initialized");
            return null;
        }

        return ((_GLFWwindow*)window)->wl.surface;
    }

    [DllImport("libc", EntryPoint = "mkstemp", SetLastError = true)]
    static extern int wayland_mkstemp(byte* template);

    [DllImport("libc", EntryPoint = "unlink", SetLastError = true)]
    static extern int wayland_unlink(byte* pathname);

    [DllImport("libc", EntryPoint = "posix_fallocate")]
    static extern int wayland_posix_fallocate(int fd, nint offset, nint len);

    [DllImport("libc", EntryPoint = "mmap", SetLastError = true)]
    static extern void* wayland_mmap(void* addr, nuint length, int prot, int flags, int fd, nint offset);

    [DllImport("libc", EntryPoint = "munmap", SetLastError = true)]
    static extern int wayland_munmap(void* addr, nuint length);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    static extern int wayland_close(int fd);

    [DllImport("libc", EntryPoint = "read", SetLastError = true)]
    static extern nint wayland_read(int fd, void* buf, nuint count);

    [DllImport("libc", EntryPoint = "write", SetLastError = true)]
    static extern nint wayland_write(int fd, void* buf, nuint count);

    [DllImport("libc", EntryPoint = "timerfd_create", SetLastError = true)]
    static extern int wayland_timerfd_create(int clockid, int flags);

    [DllImport("libc", EntryPoint = "timerfd_settime", SetLastError = true)]
    static extern int wayland_timerfd_settime(int fd, int flags, ITIMERSPEC* newValue, ITIMERSPEC* oldValue);

    [DllImport("libc", EntryPoint = "pipe2", SetLastError = true)]
    static extern int wayland_pipe2(int* pipefd, int flags);
}
