using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NGlfw;

public static unsafe partial class Glfw
{
    const uint XUSER_MAX_COUNT = 4;
    const uint ERROR_SUCCESS = 0;
    const uint ERROR_DEVICE_NOT_CONNECTED = 1167;

    const uint DIRECTINPUT_VERSION = 0x0800;

    const int _GLFW_TYPE_AXIS = 0;
    const int _GLFW_TYPE_SLIDER = 1;
    const int _GLFW_TYPE_BUTTON = 2;
    const int _GLFW_TYPE_POV = 3;

    const uint DI8DEVCLASS_GAMECTRL = 4;
    const uint DIEDFL_ALLDEVICES = 0x00000000;
    const int DIENUM_STOP = 0;
    const int DIENUM_CONTINUE = 1;

    const uint DIDFT_ABSAXIS = 0x00000002;
    const uint DIDFT_AXIS = 0x00000003;
    const uint DIDFT_BUTTON = 0x0000000c;
    const uint DIDFT_POV = 0x00000010;
    const uint DIDFT_ANYINSTANCE = 0x00ffff00;
    const uint DIDFT_OPTIONAL = 0x80000000;
    const uint DIDOI_ASPECTPOSITION = 0x00000100;

    const uint DIPH_DEVICE = 0;
    const uint DIPH_BYID = 2;
    const uint DIPROPAXISMODE_ABS = 0;
    const int DI_DEGREES = 100;

    const int DIERR_INPUTLOST = unchecked((int)0x8007001e);
    const int DIERR_NOTACQUIRED = unchecked((int)0x8007000c);

    const ushort XINPUT_CAPS_WIRELESS = 0x0002;

    const byte XINPUT_DEVSUBTYPE_WHEEL = 0x02;
    const byte XINPUT_DEVSUBTYPE_ARCADE_STICK = 0x03;
    const byte XINPUT_DEVSUBTYPE_FLIGHT_STICK = 0x04;
    const byte XINPUT_DEVSUBTYPE_DANCE_PAD = 0x05;
    const byte XINPUT_DEVSUBTYPE_GUITAR = 0x06;
    const byte XINPUT_DEVSUBTYPE_DRUM_KIT = 0x08;
    const byte XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;

    const ushort XINPUT_GAMEPAD_DPAD_UP = 0x0001;
    const ushort XINPUT_GAMEPAD_DPAD_DOWN = 0x0002;
    const ushort XINPUT_GAMEPAD_DPAD_LEFT = 0x0004;
    const ushort XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008;
    const ushort XINPUT_GAMEPAD_START = 0x0010;
    const ushort XINPUT_GAMEPAD_BACK = 0x0020;
    const ushort XINPUT_GAMEPAD_LEFT_THUMB = 0x0040;
    const ushort XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080;
    const ushort XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;
    const ushort XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;
    const ushort XINPUT_GAMEPAD_A = 0x1000;
    const ushort XINPUT_GAMEPAD_B = 0x2000;
    const ushort XINPUT_GAMEPAD_X = 0x4000;
    const ushort XINPUT_GAMEPAD_Y = 0x8000;

    static readonly Guid win32_IID_IDirectInput8W = new("BF798031-483A-4DA2-AA99-5D64ED369700");
    static readonly Guid win32_GUID_XAxis = new("A36D02E0-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_YAxis = new("A36D02E1-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_ZAxis = new("A36D02E2-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_RxAxis = new("A36D02F4-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_RyAxis = new("A36D02F5-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_RzAxis = new("A36D02E3-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_Slider = new("A36D02E4-C9F3-11CF-BFC7-444553540000");
    static readonly Guid win32_GUID_POV = new("A36D02F2-C9F3-11CF-BFC7-444553540000");

    struct _GLFWobjenumWin32
    {
        public void* device;
        public _GLFWjoyobjectWin32* objects;
        public int objectCount;
        public int axisCount;
        public int sliderCount;
        public int buttonCount;
        public int povCount;
    }

    static void win32_loadXInput()
    {
        string[] names =
        [
            "xinput1_4.dll",
            "xinput1_3.dll",
            "xinput9_1_0.dll",
            "xinput1_2.dll",
            "xinput1_1.dll"
        ];

        foreach (var name in names)
        {
            if (!NativeLibrary.TryLoad(name, out var instance))
                continue;

            if (NativeLibrary.TryGetExport(instance, "XInputGetCapabilities", out var getCapabilities) &&
                NativeLibrary.TryGetExport(instance, "XInputGetState", out var getState))
            {
                _glfw.win32.xinput.instance = (void*)instance;
                _glfw.win32.xinput.GetCapabilities =
                    (delegate* unmanaged<uint, uint, XINPUT_CAPABILITIES*, uint>)getCapabilities;
                _glfw.win32.xinput.GetState =
                    (delegate* unmanaged<uint, XINPUT_STATE*, uint>)getState;
                return;
            }

            NativeLibrary.Free(instance);
        }
    }

    static void win32_freeXInput()
    {
        if (_glfw.win32.xinput.instance != null)
            NativeLibrary.Free((nint)_glfw.win32.xinput.instance);

        _glfw.win32.xinput = default;
    }

    static void win32_loadDirectInput()
    {
        if (!NativeLibrary.TryLoad("dinput8.dll", out var instance))
            return;

        if (NativeLibrary.TryGetExport(instance, "DirectInput8Create", out var create))
        {
            _glfw.win32.dinput8.instance = (void*)instance;
            _glfw.win32.dinput8.Create =
                (delegate* unmanaged[Stdcall]<nint, uint, Guid*, void**, void*, int>)create;
            return;
        }

        NativeLibrary.Free(instance);
    }

    static void win32_freeDirectInput()
    {
        if (_glfw.win32.dinput8.instance != null)
            NativeLibrary.Free((nint)_glfw.win32.dinput8.instance);

        _glfw.win32.dinput8 = default;
    }

    static int win32_failed(int hr)
    {
        return hr < 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void** win32_vtbl(void* instance)
    {
        return *(void***)instance;
    }

    static uint win32_diRelease(void* instance)
    {
        if (instance == null)
            return 0;

        var vtbl = win32_vtbl(instance);
        return ((delegate* unmanaged[Stdcall]<void*, uint>)vtbl[2])(instance);
    }

    static int win32_diCreateDevice(void* api, Guid* guid, void** device)
    {
        var vtbl = win32_vtbl(api);
        return ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, void*, int>)vtbl[3])(api, guid, device, null);
    }

    static int win32_diEnumDevices(void* api,
                                   uint devType,
                                   delegate* unmanaged[Stdcall]<DIDEVICEINSTANCEW*, void*, int> callback,
                                   void* user,
                                   uint flags)
    {
        var vtbl = win32_vtbl(api);
        return ((delegate* unmanaged[Stdcall]<void*, uint, delegate* unmanaged[Stdcall]<DIDEVICEINSTANCEW*, void*, int>, void*, uint, int>)vtbl[4])(
            api, devType, callback, user, flags);
    }

    static int win32_didGetCapabilities(void* device, DIDEVCAPS* caps)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, DIDEVCAPS*, int>)vtbl[3])(device, caps);
    }

    static int win32_didEnumObjects(void* device,
                                    delegate* unmanaged[Stdcall]<DIDEVICEOBJECTINSTANCEW*, void*, int> callback,
                                    void* user,
                                    uint flags)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, delegate* unmanaged[Stdcall]<DIDEVICEOBJECTINSTANCEW*, void*, int>, void*, uint, int>)vtbl[4])(
            device, callback, user, flags);
    }

    static int win32_didSetProperty(void* device, void* property, DIPROPHEADER* header)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, void*, DIPROPHEADER*, int>)vtbl[6])(device, property, header);
    }

    static int win32_didAcquire(void* device)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, int>)vtbl[7])(device);
    }

    static int win32_didUnacquire(void* device)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, int>)vtbl[8])(device);
    }

    static int win32_didGetDeviceState(void* device, uint size, void* data)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)vtbl[9])(device, size, data);
    }

    static int win32_didSetDataFormat(void* device, DIDATAFORMAT* format)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, DIDATAFORMAT*, int>)vtbl[11])(device, format);
    }

    static int win32_didPoll(void* device)
    {
        var vtbl = win32_vtbl(device);
        return ((delegate* unmanaged[Stdcall]<void*, int>)vtbl[25])(device);
    }

    static string win32_getDeviceDescription(XINPUT_CAPABILITIES* xic)
    {
        return xic->SubType switch
        {
            XINPUT_DEVSUBTYPE_WHEEL => "XInput Wheel",
            XINPUT_DEVSUBTYPE_ARCADE_STICK => "XInput Arcade Stick",
            XINPUT_DEVSUBTYPE_FLIGHT_STICK => "XInput Flight Stick",
            XINPUT_DEVSUBTYPE_DANCE_PAD => "XInput Dance Pad",
            XINPUT_DEVSUBTYPE_GUITAR => "XInput Guitar",
            XINPUT_DEVSUBTYPE_DRUM_KIT => "XInput Drum Kit",
            XINPUT_DEVSUBTYPE_GAMEPAD => (xic->Flags & XINPUT_CAPS_WIRELESS) != 0
                ? "Wireless Xbox Controller"
                : "Xbox Controller",
            _ => "Unknown XInput Device"
        };
    }

    static uint win32_didftGetType(uint type)
    {
        return type & 0xff;
    }

    static uint win32_dijofsSlider(int index)
    {
        return (uint)(24 + index * sizeof(int));
    }

    static uint win32_dijofsPov(int index)
    {
        return (uint)(32 + index * sizeof(uint));
    }

    static uint win32_dijofsButton(int index)
    {
        return (uint)(48 + index);
    }

    static int win32_asciiContains(byte* value, string needle)
    {
        for (var i = 0; value[i] != 0; i++)
        {
            var j = 0;
            for (; j < needle.Length; j++)
            {
                if (value[i + j] != (byte)needle[j])
                    break;
            }

            if (j == needle.Length)
                return GLFW_TRUE;
        }

        return GLFW_FALSE;
    }

    static int win32_supportsXInput(Guid* guid)
    {
        var count = 0u;
        if (GetRawInputDeviceList(null, &count, (uint)sizeof(RAWINPUTDEVICELIST)) != 0)
            return GLFW_FALSE;

        var ridl = (RAWINPUTDEVICELIST*)_glfw_calloc(count, (nuint)sizeof(RAWINPUTDEVICELIST));
        if (ridl == null)
            return GLFW_FALSE;

        var result = GLFW_FALSE;
        if (GetRawInputDeviceList(ridl, &count, (uint)sizeof(RAWINPUTDEVICELIST)) == uint.MaxValue)
        {
            _glfw_free(ridl);
            return GLFW_FALSE;
        }

        var guidBytes = stackalloc byte[16];
        guid->TryWriteBytes(new Span<byte>(guidBytes, 16));
        var product = (uint)(guidBytes[0] |
                            (guidBytes[1] << 8) |
                            (guidBytes[2] << 16) |
                            (guidBytes[3] << 24));

        var name = stackalloc byte[256];

        for (var i = 0; i < count; i++)
        {
            if (ridl[i].dwType != RIM_TYPEHID)
                continue;

            var rdi = new RID_DEVICE_INFO { cbSize = (uint)sizeof(RID_DEVICE_INFO) };
            var size = (uint)sizeof(RID_DEVICE_INFO);

            if (GetRawInputDeviceInfoA(ridl[i].hDevice, RIDI_DEVICEINFO, &rdi, &size) == uint.MaxValue)
                continue;

            var deviceProduct = (rdi.hid.dwVendorId & 0xffff) |
                                ((rdi.hid.dwProductId & 0xffff) << 16);
            if (deviceProduct != product)
                continue;

            _glfw_memset(name, 0, 256);
            size = 256;

            if (GetRawInputDeviceInfoA(ridl[i].hDevice, RIDI_DEVICENAME, name, &size) == uint.MaxValue)
                break;

            name[255] = 0;
            if (win32_asciiContains(name, "IG_") != 0)
            {
                result = GLFW_TRUE;
                break;
            }
        }

        _glfw_free(ridl);
        return result;
    }

    static void win32_setObjectDataFormat(DIOBJECTDATAFORMAT* formats,
                                          int index,
                                          Guid* guid,
                                          uint offset,
                                          uint type,
                                          uint flags)
    {
        formats[index].pguid = guid;
        formats[index].dwOfs = offset;
        formats[index].dwType = type;
        formats[index].dwFlags = flags;
    }

    static int win32_setJoystickDataFormat(void* device)
    {
        var guids = stackalloc Guid[8];
        guids[0] = win32_GUID_XAxis;
        guids[1] = win32_GUID_YAxis;
        guids[2] = win32_GUID_ZAxis;
        guids[3] = win32_GUID_RxAxis;
        guids[4] = win32_GUID_RyAxis;
        guids[5] = win32_GUID_RzAxis;
        guids[6] = win32_GUID_Slider;
        guids[7] = win32_GUID_POV;

        var formats = stackalloc DIOBJECTDATAFORMAT[44];
        var type = DIDFT_AXIS | DIDFT_OPTIONAL | DIDFT_ANYINSTANCE;

        win32_setObjectDataFormat(formats, 0, guids + 0, 0, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 1, guids + 1, 4, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 2, guids + 2, 8, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 3, guids + 3, 12, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 4, guids + 4, 16, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 5, guids + 5, 20, type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 6, guids + 6, win32_dijofsSlider(0), type, DIDOI_ASPECTPOSITION);
        win32_setObjectDataFormat(formats, 7, guids + 6, win32_dijofsSlider(1), type, DIDOI_ASPECTPOSITION);

        for (var i = 0; i < 4; i++)
        {
            win32_setObjectDataFormat(formats, 8 + i, guids + 7, win32_dijofsPov(i),
                DIDFT_POV | DIDFT_OPTIONAL | DIDFT_ANYINSTANCE, 0);
        }

        for (var i = 0; i < 32; i++)
        {
            win32_setObjectDataFormat(formats, 12 + i, null, win32_dijofsButton(i),
                DIDFT_BUTTON | DIDFT_OPTIONAL | DIDFT_ANYINSTANCE, 0);
        }

        var format = new DIDATAFORMAT
        {
            dwSize = (uint)sizeof(DIDATAFORMAT),
            dwObjSize = (uint)sizeof(DIOBJECTDATAFORMAT),
            dwFlags = DIDFT_ABSAXIS,
            dwDataSize = (uint)sizeof(DIJOYSTATE),
            dwNumObjs = 44,
            rgodf = formats
        };

        return win32_didSetDataFormat(device, &format);
    }

    static int win32_compareJoystickObjects(_GLFWjoyobjectWin32* first, _GLFWjoyobjectWin32* second)
    {
        if (first->type != second->type)
            return first->type - second->type;

        return first->offset - second->offset;
    }

    static void win32_sortJoystickObjects(_GLFWjoyobjectWin32* objects, int count)
    {
        for (var i = 1; i < count; i++)
        {
            var value = objects[i];
            var j = i - 1;

            while (j >= 0 && win32_compareJoystickObjects(objects + j, &value) > 0)
            {
                objects[j + 1] = objects[j];
                j--;
            }

            objects[j + 1] = value;
        }
    }

    static byte win32_nameByte(byte[] bytes, int index)
    {
        return index < bytes.Length ? bytes[index] : (byte)0;
    }

    static string win32_createDirectInputGUID(Guid* product, string name)
    {
        var bytes = stackalloc byte[16];
        product->TryWriteBytes(new Span<byte>(bytes, 16));

        if (bytes[10] == (byte)'P' &&
            bytes[11] == (byte)'I' &&
            bytes[12] == (byte)'D' &&
            bytes[13] == (byte)'V' &&
            bytes[14] == (byte)'I' &&
            bytes[15] == (byte)'D')
        {
            return $"03000000{bytes[0]:x2}{bytes[1]:x2}0000{bytes[2]:x2}{bytes[3]:x2}000000000000";
        }

        var nameBytes = Encoding.UTF8.GetBytes(name);
        return "05000000" +
               $"{win32_nameByte(nameBytes, 0):x2}{win32_nameByte(nameBytes, 1):x2}" +
               $"{win32_nameByte(nameBytes, 2):x2}{win32_nameByte(nameBytes, 3):x2}" +
               $"{win32_nameByte(nameBytes, 4):x2}{win32_nameByte(nameBytes, 5):x2}" +
               $"{win32_nameByte(nameBytes, 6):x2}{win32_nameByte(nameBytes, 7):x2}" +
               $"{win32_nameByte(nameBytes, 8):x2}{win32_nameByte(nameBytes, 9):x2}" +
               $"{win32_nameByte(nameBytes, 10):x2}00";
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    static int win32_deviceObjectCallback(DIDEVICEOBJECTINSTANCEW* doi, void* user)
    {
        var data = (_GLFWobjenumWin32*)user;
        var object_ = data->objects + data->objectCount;
        var type = win32_didftGetType(doi->dwType);

        if ((type & DIDFT_AXIS) != 0)
        {
            if (doi->guidType.Equals(win32_GUID_Slider))
                object_->offset = (int)win32_dijofsSlider(data->sliderCount);
            else if (doi->guidType.Equals(win32_GUID_XAxis))
                object_->offset = 0;
            else if (doi->guidType.Equals(win32_GUID_YAxis))
                object_->offset = 4;
            else if (doi->guidType.Equals(win32_GUID_ZAxis))
                object_->offset = 8;
            else if (doi->guidType.Equals(win32_GUID_RxAxis))
                object_->offset = 12;
            else if (doi->guidType.Equals(win32_GUID_RyAxis))
                object_->offset = 16;
            else if (doi->guidType.Equals(win32_GUID_RzAxis))
                object_->offset = 20;
            else
                return DIENUM_CONTINUE;

            var dipr = new DIPROPRANGE
            {
                diph =
                {
                    dwSize = (uint)sizeof(DIPROPRANGE),
                    dwHeaderSize = (uint)sizeof(DIPROPHEADER),
                    dwObj = doi->dwType,
                    dwHow = DIPH_BYID
                },
                lMin = -32768,
                lMax = 32767
            };

            if (win32_failed(win32_didSetProperty(data->device, (void*)4, &dipr.diph)) != 0)
                return DIENUM_CONTINUE;

            if (doi->guidType.Equals(win32_GUID_Slider))
            {
                object_->type = _GLFW_TYPE_SLIDER;
                data->sliderCount++;
            }
            else
            {
                object_->type = _GLFW_TYPE_AXIS;
                data->axisCount++;
            }
        }
        else if ((type & DIDFT_BUTTON) != 0)
        {
            object_->offset = (int)win32_dijofsButton(data->buttonCount);
            object_->type = _GLFW_TYPE_BUTTON;
            data->buttonCount++;
        }
        else if ((type & DIDFT_POV) != 0)
        {
            object_->offset = (int)win32_dijofsPov(data->povCount);
            object_->type = _GLFW_TYPE_POV;
            data->povCount++;
        }

        data->objectCount++;
        return DIENUM_CONTINUE;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    static int win32_deviceCallback(DIDEVICEINSTANCEW* di, void* user)
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0 && js->win32.guid.Equals(di->guidInstance))
                    return DIENUM_CONTINUE;
            }
        }

        if (win32_supportsXInput(&di->guidProduct) != 0)
            return DIENUM_CONTINUE;

        void* device = null;
        if (win32_failed(win32_diCreateDevice(_glfw.win32.dinput8.api, &di->guidInstance, &device)) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to create device");
            return DIENUM_CONTINUE;
        }

        if (win32_failed(win32_setJoystickDataFormat(device)) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to set device data format");
            win32_diRelease(device);
            return DIENUM_CONTINUE;
        }

        var dc = new DIDEVCAPS { dwSize = (uint)sizeof(DIDEVCAPS) };
        if (win32_failed(win32_didGetCapabilities(device, &dc)) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to query device capabilities");
            win32_diRelease(device);
            return DIENUM_CONTINUE;
        }

        var dipd = new DIPROPDWORD
        {
            diph =
            {
                dwSize = (uint)sizeof(DIPROPDWORD),
                dwHeaderSize = (uint)sizeof(DIPROPHEADER),
                dwHow = DIPH_DEVICE
            },
            dwData = DIPROPAXISMODE_ABS
        };

        if (win32_failed(win32_didSetProperty(device, (void*)2, &dipd.diph)) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to set device axis mode");
            win32_diRelease(device);
            return DIENUM_CONTINUE;
        }

        var objectCapacity = dc.dwAxes + dc.dwButtons + dc.dwPOVs;
        var data = new _GLFWobjenumWin32
        {
            device = device,
            objects = (_GLFWjoyobjectWin32*)_glfw_calloc(objectCapacity, (nuint)sizeof(_GLFWjoyobjectWin32))
        };

        if (objectCapacity != 0 && data.objects == null)
        {
            win32_diRelease(device);
            return DIENUM_STOP;
        }

        if (win32_failed(win32_didEnumObjects(device,
                &win32_deviceObjectCallback,
                &data,
                DIDFT_AXIS | DIDFT_BUTTON | DIDFT_POV)) != 0)
        {
            _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to enumerate device objects");
            win32_diRelease(device);
            _glfw_free(data.objects);
            return DIENUM_CONTINUE;
        }

        win32_sortJoystickObjects(data.objects, data.objectCount);

        var name = win32_fixedWideString(di->tszInstanceName, 260);
        var guid = win32_createDirectInputGUID(&di->guidProduct, name);
        var js2 = _glfwAllocJoystick(name, guid,
            data.axisCount + data.sliderCount,
            data.buttonCount,
            data.povCount);

        if (js2 == null)
        {
            win32_diRelease(device);
            _glfw_free(data.objects);
            return DIENUM_STOP;
        }

        js2->win32.device = device;
        js2->win32.guid = di->guidInstance;
        js2->win32.objects = data.objects;
        js2->win32.objectCount = data.objectCount;

        _glfwInputJoystick(js2, GLFW_CONNECTED);
        return DIENUM_CONTINUE;
    }

    static void win32_closeJoystick(_GLFWjoystick* js)
    {
        if (js->connected != 0)
            _glfwInputJoystick(js, GLFW_DISCONNECTED);

        if (js->win32.device != null)
        {
            win32_didUnacquire(js->win32.device);
            win32_diRelease(js->win32.device);
        }

        _glfw_free(js->win32.objects);
        _glfwFreeJoystick(js);
    }

    static void _glfwDetectJoystickConnectionWin32()
    {
        if (_glfw.win32.xinput.GetCapabilities != null)
        {
            fixed (_GLFWlibrary* glfw = &_glfw)
            {
                for (uint index = 0; index < XUSER_MAX_COUNT; index++)
                {
                    var present = GLFW_FALSE;
                    for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
                    {
                        if (glfw->joysticks[jid].connected != 0 &&
                            glfw->joysticks[jid].win32.device == null &&
                            glfw->joysticks[jid].win32.index == index)
                        {
                            present = GLFW_TRUE;
                            break;
                        }
                    }

                    if (present != 0)
                        continue;

                    XINPUT_CAPABILITIES xic;
                    if (_glfw.win32.xinput.GetCapabilities(index, 0, &xic) != ERROR_SUCCESS)
                        continue;

                    var guid = $"78696e707574{xic.SubType & 0xff:x2}000000000000000000";
                    var js = _glfwAllocJoystick(win32_getDeviceDescription(&xic), guid, 6, 10, 1);
                    if (js == null)
                        continue;

                    js->win32.index = index;
                    _glfwInputJoystick(js, GLFW_CONNECTED);
                }
            }
        }

        if (_glfw.win32.dinput8.api != null)
        {
            if (win32_failed(win32_diEnumDevices(_glfw.win32.dinput8.api,
                    DI8DEVCLASS_GAMECTRL,
                    &win32_deviceCallback,
                    null,
                    DIEDFL_ALLDEVICES)) != 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to enumerate DirectInput8 devices");
            }
        }
    }

    static void _glfwDetectJoystickDisconnectionWin32()
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = 0; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->connected != 0)
                    _glfwPollJoystickWin32(js, _GLFW_POLL_PRESENCE);
            }
        }
    }

    static int _glfwInitJoysticksWin32()
    {
        if (_glfw.win32.xinput.instance == null)
            win32_loadXInput();

        if (_glfw.win32.dinput8.instance == null)
            win32_loadDirectInput();

        if (_glfw.win32.dinput8.Create != null)
        {
            var iid = win32_IID_IDirectInput8W;
            void* api = null;

            if (win32_failed(_glfw.win32.dinput8.Create(_glfw.win32.instance,
                    DIRECTINPUT_VERSION,
                    &iid,
                    &api,
                    null)) != 0)
            {
                _glfwInputError(GLFW_PLATFORM_ERROR, "Win32: Failed to create DirectInput8 interface");
                return GLFW_FALSE;
            }

            _glfw.win32.dinput8.api = api;
        }

        _glfwDetectJoystickConnectionWin32();
        return GLFW_TRUE;
    }

    static void _glfwTerminateJoysticksWin32()
    {
        fixed (_GLFWlibrary* glfw = &_glfw)
        {
            for (var jid = GLFW_JOYSTICK_1; jid <= GLFW_JOYSTICK_LAST; jid++)
            {
                var js = &glfw->joysticks[jid];
                if (js->allocated != 0)
                    win32_closeJoystick(js);
            }
        }

        if (_glfw.win32.dinput8.api != null)
            win32_diRelease(_glfw.win32.dinput8.api);

        win32_freeDirectInput();
    }

    static int _glfwPollJoystickWin32(_GLFWjoystick* js, int mode)
    {
        if (js->win32.device != null)
        {
            var ai = 0;
            var bi = 0;
            var pi = 0;
            var state = new DIJOYSTATE();

            win32_didPoll(js->win32.device);
            var diResult = win32_didGetDeviceState(js->win32.device, (uint)sizeof(DIJOYSTATE), &state);
            if (diResult == DIERR_NOTACQUIRED || diResult == DIERR_INPUTLOST)
            {
                win32_didAcquire(js->win32.device);
                win32_didPoll(js->win32.device);
                diResult = win32_didGetDeviceState(js->win32.device, (uint)sizeof(DIJOYSTATE), &state);
            }

            if (win32_failed(diResult) != 0)
            {
                win32_closeJoystick(js);
                return GLFW_FALSE;
            }

            if (mode == _GLFW_POLL_PRESENCE)
                return GLFW_TRUE;

            for (var i = 0; i < js->win32.objectCount; i++)
            {
                var data = (byte*)&state + js->win32.objects[i].offset;

                switch (js->win32.objects[i].type)
                {
                    case _GLFW_TYPE_AXIS:
                    case _GLFW_TYPE_SLIDER:
                    {
                        var value = (*(int*)data + 0.5f) / 32767.5f;
                        _glfwInputJoystickAxis(js, ai, value);
                        ai++;
                        break;
                    }

                    case _GLFW_TYPE_BUTTON:
                    {
                        var value = ((*data & 0x80) != 0) ? GLFW_PRESS : GLFW_RELEASE;
                        _glfwInputJoystickButton(js, bi, value);
                        bi++;
                        break;
                    }

                    case _GLFW_TYPE_POV:
                    {
                        var states = stackalloc int[9];
                        states[0] = GLFW_HAT_UP;
                        states[1] = GLFW_HAT_RIGHT_UP;
                        states[2] = GLFW_HAT_RIGHT;
                        states[3] = GLFW_HAT_RIGHT_DOWN;
                        states[4] = GLFW_HAT_DOWN;
                        states[5] = GLFW_HAT_LEFT_DOWN;
                        states[6] = GLFW_HAT_LEFT;
                        states[7] = GLFW_HAT_LEFT_UP;
                        states[8] = GLFW_HAT_CENTERED;

                        var stateIndex = (int)(*(uint*)data & 0xffff) / (45 * DI_DEGREES);
                        if (stateIndex < 0 || stateIndex > 8)
                            stateIndex = 8;

                        _glfwInputJoystickHat(js, pi, states[stateIndex]);
                        pi++;
                        break;
                    }
                }
            }

            return GLFW_TRUE;
        }

        if (_glfw.win32.xinput.GetState == null)
            return GLFW_FALSE;

        XINPUT_STATE xis;
        var result = _glfw.win32.xinput.GetState(js->win32.index, &xis);
        if (result != ERROR_SUCCESS)
        {
            if (result == ERROR_DEVICE_NOT_CONNECTED)
                win32_closeJoystick(js);

            return GLFW_FALSE;
        }

        if (mode == _GLFW_POLL_PRESENCE)
            return GLFW_TRUE;

        _glfwInputJoystickAxis(js, 0, (xis.Gamepad.sThumbLX + 0.5f) / 32767.5f);
        _glfwInputJoystickAxis(js, 1, -(xis.Gamepad.sThumbLY + 0.5f) / 32767.5f);
        _glfwInputJoystickAxis(js, 2, (xis.Gamepad.sThumbRX + 0.5f) / 32767.5f);
        _glfwInputJoystickAxis(js, 3, -(xis.Gamepad.sThumbRY + 0.5f) / 32767.5f);
        _glfwInputJoystickAxis(js, 4, xis.Gamepad.bLeftTrigger / 127.5f - 1f);
        _glfwInputJoystickAxis(js, 5, xis.Gamepad.bRightTrigger / 127.5f - 1f);

        var buttons = stackalloc ushort[10];
        buttons[0] = XINPUT_GAMEPAD_A;
        buttons[1] = XINPUT_GAMEPAD_B;
        buttons[2] = XINPUT_GAMEPAD_X;
        buttons[3] = XINPUT_GAMEPAD_Y;
        buttons[4] = XINPUT_GAMEPAD_LEFT_SHOULDER;
        buttons[5] = XINPUT_GAMEPAD_RIGHT_SHOULDER;
        buttons[6] = XINPUT_GAMEPAD_BACK;
        buttons[7] = XINPUT_GAMEPAD_START;
        buttons[8] = XINPUT_GAMEPAD_LEFT_THUMB;
        buttons[9] = XINPUT_GAMEPAD_RIGHT_THUMB;

        for (var i = 0; i < 10; i++)
        {
            var value = (xis.Gamepad.wButtons & buttons[i]) != 0 ? GLFW_PRESS : GLFW_RELEASE;
            _glfwInputJoystickButton(js, i, value);
        }

        var dpad = GLFW_HAT_CENTERED;
        if ((xis.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_UP) != 0)
            dpad |= GLFW_HAT_UP;
        if ((xis.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_RIGHT) != 0)
            dpad |= GLFW_HAT_RIGHT;
        if ((xis.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_DOWN) != 0)
            dpad |= GLFW_HAT_DOWN;
        if ((xis.Gamepad.wButtons & XINPUT_GAMEPAD_DPAD_LEFT) != 0)
            dpad |= GLFW_HAT_LEFT;

        if ((dpad & GLFW_HAT_RIGHT) != 0 && (dpad & GLFW_HAT_LEFT) != 0)
            dpad &= ~(GLFW_HAT_RIGHT | GLFW_HAT_LEFT);
        if ((dpad & GLFW_HAT_UP) != 0 && (dpad & GLFW_HAT_DOWN) != 0)
            dpad &= ~(GLFW_HAT_UP | GLFW_HAT_DOWN);

        _glfwInputJoystickHat(js, 0, dpad);
        return GLFW_TRUE;
    }

    static byte* _glfwGetMappingNameWin32()
    {
        return _glfwWin32MappingName;
    }

    static int win32_asciiEquals(byte* value, string expected)
    {
        for (var i = 0; i < expected.Length; i++)
        {
            if (value[i] != (byte)expected[i])
                return GLFW_FALSE;
        }

        return value[expected.Length] == 0 ? GLFW_TRUE : GLFW_FALSE;
    }

    static void win32_writeAscii(byte* destination, string value)
    {
        for (var i = 0; i < value.Length; i++)
            destination[i] = (byte)value[i];
    }

    static void _glfwUpdateGamepadGUIDWin32(byte* guid)
    {
        if (win32_asciiEquals(guid + 20, "504944564944") == 0)
            return;

        var original = stackalloc byte[33];
        for (var i = 0; i < 33; i++)
            original[i] = guid[i];

        win32_writeAscii(guid, "03000000");
        for (var i = 0; i < 4; i++)
            guid[8 + i] = original[i];
        win32_writeAscii(guid + 12, "0000");
        for (var i = 0; i < 4; i++)
            guid[16 + i] = original[4 + i];
        win32_writeAscii(guid + 20, "000000000000");
        guid[32] = 0;
    }
}
