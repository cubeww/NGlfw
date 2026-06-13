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
- XKB detectable auto-repeat, keyboard state, and layout group tracking
- XIM input contexts, UTF-8 lookup, focus handling, and filtered key events
- X11 mouse, cursor, icons, window hints, opacity, frame extents, and work area
- Xcursor custom and hidden cursors
- XShape mouse passthrough
- XRender transparent visual support
- XI2 raw mouse motion
- clipboard and primary selection basics, including several selection targets and
  INCR transfers
- Xdnd file drop main path for `text/uri-list`
- RandR monitor enumeration, video modes, gamma ramp, video mode switching,
  fullscreen monitor handling, and hotplug polling
- GLX context creation, swapping, swap interval, extension lookup, and Vulkan
  Xlib/XCB surface support

Known remaining work:

- full Linux joystick behavior, including `/dev/input/event*` enumeration,
  device opening, event polling, mapping, and disconnect handling
- real X11 cursor capture/disabled behavior through pointer grabs, ungrabs, and
  confinement instead of only hiding, warping, and raw motion
- X11 asynchronous error capture through `XSetErrorHandler` / `XSync`, plus
  useful diagnostics from `_glfwInputErrorX11`
- additional ICCCM/EWMH polish, including `_NET_WM_PID`,
  `_NET_WM_WINDOW_TYPE_NORMAL`, `_NET_SUPPORTED` detection, screen saver
  save/restore, `XSetWMHints`, and fullscreen fallback details
- `ConfigureNotify` position correction for reparented windows by translating
  coordinates back to the root window
- X11 EGL platform selection in `_glfwGetEGLPlatformX11`
- more complete XKB parity for physical key names and layout edge cases
- XIM instantiate/destroy callbacks and input-method restart edge cases
- full Xdnd edge-case coverage
- Linux runtime testing on real X11 environments

### Cocoa / macOS Backend

Started, but not yet functional:

- Cocoa platform selection and support detection
- Objective-C runtime helper layer for classes, selectors, messages, and
  autorelease pools
- basic `NSApplicationDelegate` registration for terminate requests, screen
  parameter changes, finish-launch wakeup, and app hide video-mode restoration
- Cocoa `NSUserDefaults` registration for `ApplePressAndHoldEnabled = NO`
- Cocoa backend file skeletons for init, windows, monitors, cursors, clipboard,
  joysticks, Vulkan surface hooks, and native accessors
- basic runtime `GLFWWindow` subclass registration through the Objective-C
  runtime
- runtime `GLFWWindowDelegate` and `GLFWContentView` subclass registration with
  object-to-GLFW-window routing
- basic `NSWindow` / `NSView` creation for Cocoa windows
- common Cocoa window operations, including title, position, size, visibility,
  focus, opacity, style, limits, aspect ratio, and event polling
- basic Cocoa window delegate callbacks for close, resize, move, iconify,
  restore, focus, occlusion state, and framebuffer/content-scale changes
- Cocoa view callbacks for damage, mouse buttons, cursor movement,
  cursor enter/leave, precise scrolling, key presses/releases, modifier key
  changes, `interpretKeyEvents` text input, and basic `NSTextInputClient`
  marked-text handling
- Cocoa `NSTrackingArea` setup for cursor enter/leave and cursor update events
- Cocoa file drag-and-drop through URL pasteboard objects and `_glfwInputDrop`
- basic CoreGraphics monitor enumeration, monitor position, full display mode
  enumeration with duplicate filtering, current video mode, fullscreen video
  mode switching/restoration with display fade transitions, work area fallback,
  gamma ramp access, and the Cocoa monitor native accessor
- Cocoa monitor hotplug polling with disconnected-monitor detection
- NSScreen mapping for localized monitor names, content scale, and visible
  work area queries
- IOKit fallback refresh-rate lookup for CoreGraphics display modes that report
  zero refresh
- standard Cocoa cursor creation for the public cursor shapes, including
  private resize-cursor fallback selectors
- custom bitmap cursor creation through `NSBitmapImageRep`, `NSImage`, and
  `NSCursor`
- hidden/disabled cursor mode hide/unhide plumbing
- CoreGraphics cursor warping and disabled-cursor mouse/cursor disassociation
- `NSPasteboard` clipboard string path
- `NSEvent` application-defined wakeups for `postEmptyEvent`
- basic NSGL initialization through the OpenGL framework bundle
- NSGL pixel format attribute selection, native OpenGL context creation,
  make-current, swap buffers, swap interval, function lookup, destroy path, and
  native context accessor
- NSGL occluded-window swap interval sleep simulation
- Cocoa EGL platform selection for ANGLE OpenGL and Metal hints
- Cocoa Vulkan surface creation through `CAMetalLayer`,
  `VK_EXT_metal_surface`, and `VK_MVK_macos_surface`
- POSIX runtime plumbing adjusted for macOS library names and pthread key sizes

Known remaining work:

- full NSApplication helper behavior, menu bar setup, resource directory
  handling, key-up monitor, and launch lifecycle parity
- complete `NSView` / `NSTextInputClient` parity, including IME and
  drag-and-drop edge-case validation
- full keyboard layout Unicode translation for scancode names
- IOKit fallback monitor names
- NSGL runtime validation and edge-case parity for context hints unsupported by
  macOS
- IOKit / HID joystick support
- runtime validation on macOS

### Other Backends

Not implemented yet:

- Wayland
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
