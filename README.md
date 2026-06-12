# NGlfw

NGlfw is an experimental source-level C# port of GLFW.  The goal is not to
bind to the native GLFW library, but to translate the GLFW C implementation
into unsafe C# as directly as possible.

The port intentionally keeps the original GLFW structure recognizable:

- public constants, handles, callbacks, and entry points follow the GLFW API
- native backend files are split similarly to the upstream GLFW source tree
- translated implementation files live as `partial class Glfw`
- C-style names are preserved where practical
- pointer-heavy platform code uses `unsafe` C# instead of wrapper bindings

This makes the project useful for studying GLFW internals from C#, and for
eventually producing a managed assembly that contains the platform backends
directly.

## Repository Layout

- `NGlfw/` contains the library source.
- `examples/` contains example projects using the library.
- `NGlfw.slnx` is the solution file.

## Implementation Status

This project is still in active development.  The current focus is bringing up
the original GLFW backends one platform layer at a time.

### Shared Core

Implemented or mostly implemented:

- initialization and termination flow
- allocator plumbing
- monitor, window, input, context, and Vulkan-facing public API surface
- error reporting
- gamepad mapping data
- UTF-8 and URI-list helpers used by platform code

Still incomplete:

- EGL context creation
- OSMesa context creation
- full parity checks against the upstream GLFW test suite

### Win32 Backend

The Win32 backend is the most complete backend in this port.

Implemented or mostly implemented:

- Win32 platform initialization
- window creation and event handling
- monitor enumeration, video modes, work area, gamma ramp, DPI/content scale
- keyboard, mouse, cursor, clipboard, icon, opacity, and window attributes
- WGL context creation and extension handling
- joystick support through the translated Win32 joystick path
- Vulkan surface support for Win32

Remaining work is mainly validation against real GLFW behavior and edge cases.

### Linux / POSIX / X11 Backend

The Linux backend is under active implementation.

Implemented or partially implemented:

- POSIX platform base for timing, TLS, mutexes, and modules
- Linux joystick skeleton
- X11 initialization and dynamic Xlib/X extension loading
- X11 window creation and event loop
- X11 keyboard/scancode tables and text input fallback
- X11 mouse, cursor, icons, window hints, opacity, frame extents, and work area
- Xcursor custom and hidden cursors
- XShape mouse passthrough
- XRender transparent visual support
- XI2 raw mouse motion
- clipboard and primary selection basics, including several selection targets
- Xdnd file drop main path for `text/uri-list`
- RandR monitor enumeration, video modes, gamma ramp, video mode switching,
  fullscreen monitor handling, and hotplug polling
- GLX context creation, swapping, swap interval, extension lookup, and Vulkan
  Xlib surface support

Known remaining work:

- deeper XKB support, including layout/group tracking and physical key names
- XIM input method parity
- INCR clipboard transfer support
- full Xdnd edge-case coverage
- more complete Linux joystick hotplug/input behavior
- Linux runtime testing on real X11 environments

### Other Backends

Not implemented yet:

- Wayland
- Cocoa / macOS
- NSGL
- platform-specific native accessors beyond the currently translated paths

## Building

Requirements:

- .NET SDK with support for the target framework used by the project
- platform libraries available at runtime for the backend being tested

Build the solution from the repository root:

```powershell
dotnet build .\NGlfw.slnx
```

The examples are included in the solution and are built together with the
library.

## Notes

This is a low-level unsafe port.  It deliberately favors structural similarity
with upstream GLFW over idiomatic managed abstractions.  Many files therefore
look closer to translated C than typical C# library code.

The project should be treated as experimental until each backend has been
tested against real platform behavior and compared with upstream GLFW.
