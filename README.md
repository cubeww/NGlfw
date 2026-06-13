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

- full parity checks against the upstream GLFW test suite

### EGL / OSMesa Context Backends

Implemented or mostly implemented:

- EGL dynamic library loading and entry point resolution
- EGL platform display selection through backend callbacks
- EGL config selection, context creation, window surface creation, swapping,
  swap interval, extension lookup, and function lookup
- EGL native accessors for display, context, and surface
- X11 EGL visual selection through `EGL_NATIVE_VISUAL_ID`
- OSMesa dynamic library loading and entry point resolution
- OSMesa context creation, offscreen framebuffer allocation, buffer accessors,
  and native context accessor

Known remaining work:

- runtime validation across real EGL providers, ANGLE, Mesa EGL, and OSMesa
- Wayland-specific EGL surface behavior once the Wayland backend is added

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
- EGL platform selection, X11 visual selection, context creation, and window
  surface creation

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
- more complete XKB parity for physical key names and layout edge cases
- XIM instantiate/destroy callbacks and input-method restart edge cases
- full Xdnd edge-case coverage
- Linux runtime testing on real X11 environments

### Cocoa / macOS Backend

Under active implementation and compile-validated on non-macOS hosts, but still
unverified on real macOS:

- Cocoa platform selection and support detection
- Objective-C runtime helper layer for classes, selectors, messages, and
  autorelease pools
- basic `NSApplicationDelegate` registration for terminate requests, screen
  parameter changes, finish-launch wakeup, will-finish menu setup, and app
  hide video-mode restoration
- Cocoa menu bar setup, including bundled `MainMenu.nib` loading and the
  fallback application/window menus
- Cocoa finish-launch path through `NSRunningApplication`, `NSApp run`, and
  regular activation policy for menubar-enabled apps
- Cocoa resource-directory `chdir` behavior for bundled apps
- Cocoa `NSUserDefaults` registration for `ApplePressAndHoldEnabled = NO`
- Cocoa `CGEventSource` setup with local event suppression disabled
- Cocoa backend file skeletons for init, windows, monitors, cursors, clipboard,
  joysticks, Vulkan surface hooks, and native accessors
- Cocoa native accessors for `NSWindow`, `NSView`, `CGDirectDisplayID`, and
  `NSOpenGLContext`
- basic runtime `GLFWWindow` subclass registration through the Objective-C
  runtime
- runtime `GLFWWindowDelegate` and `GLFWContentView` subclass registration with
  object-to-GLFW-window routing
- basic `NSWindow` / `NSView` creation for Cocoa windows
- common Cocoa window operations, including title, position, size, visibility,
  focus, opacity, style, limits, aspect ratio, mouse passthrough, and event
  polling
- basic Cocoa window delegate callbacks for close, resize, move, iconify,
  restore, focus, occlusion state, and framebuffer/content-scale changes
- Cocoa view callbacks for damage, mouse buttons, cursor movement,
  cursor enter/leave, precise scrolling, key presses/releases, modifier key
  changes, `interpretKeyEvents` text input, and basic `NSTextInputClient`
  marked-text handling
- Cocoa HIToolbox/TIS keyboard layout loading, input-source change
  notifications, and Unicode scancode names for `glfwGetKeyName`
- Cocoa `NSTrackingArea` setup for cursor enter/leave and cursor update events
- Cocoa file drag-and-drop through URL pasteboard objects and `_glfwInputDrop`
- basic CoreGraphics monitor enumeration, monitor position, full display mode
  enumeration with duplicate filtering, current video mode, fullscreen video
  mode switching/restoration with display fade transitions, work area fallback,
  gamma ramp access, and the Cocoa monitor native accessor
- Cocoa monitor hotplug polling with disconnected-monitor detection
- NSScreen mapping for localized monitor names, content scale, and visible
  work area queries
- IOKit fallback monitor names for systems without `NSScreen.localizedName`
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
- Cocoa local Vulkan loader lookup in the application bundle `Frameworks`
  directory
- Cocoa IOHID manager creation, joystick/gamepad matching setup, run-loop
  scheduling, manager teardown, device callbacks, element enumeration, SDL-style
  element sorting, axis/button/hat polling, GUID generation, and disconnect
  cleanup
- POSIX runtime plumbing adjusted for macOS library names and pthread key sizes

Known remaining work:

- remaining NSApplication launch lifecycle validation
- complete `NSView` / `NSTextInputClient` parity, including IME and
  drag-and-drop edge-case validation
- NSGL runtime validation and edge-case parity for context hints unsupported by
  macOS
- macOS runtime validation for IOKit / HID joystick behavior and edge cases
- runtime validation on macOS

### Other Backends

Wayland work has started.

Implemented or partially implemented:

- Wayland platform selection and dynamic `libwayland-client` loading
- Wayland platform callback table
- initial Wayland library, window, monitor, and cursor state structures
- libwayland-client core entry point and protocol interface resolution
- registry listener setup and global discovery for core Wayland objects
- binding for `wl_compositor`, `wl_subcompositor`, `wl_shm`, `wl_output`,
  `wl_seat`, `wl_data_device_manager`, and `xdg_wm_base`
- initial `wl_output` monitor listener path for geometry, modes, scale, and
  monitor connection/removal
- native `wl_surface` creation/destruction for windows
- hand-written minimal xdg-shell protocol tables for `xdg_wm_base`,
  `xdg_surface`, and `xdg_toplevel`
- initial `xdg_surface` / `xdg_toplevel` creation, configure ack, close
  callback, title/app-id, fullscreen, maximize, minimize, and size-limit
  requests
- xdg toplevel configure state parsing for activation, maximize, fullscreen,
  and aspect-ratio constrained configure sizes
- initial `wl_surface` listener handling for output enter/leave and integer
  buffer scale updates
- initial `wl_seat` capability handling and `wl_pointer` enter/leave, motion,
  button, and scroll callbacks
- Wayland EGL native window creation and EGL/OSMesa context creation path
- basic Wayland monitor/window/native accessor entry points
- Wayland EGL and Vulkan extension plumbing entry points

Known remaining work:

- generated/translated Wayland protocol interface tables and marshal wrappers
  beyond the currently hand-written core wrappers
- keyboard, data-device, cursor theme, and XKB event handling
- advanced pointer events, disabled-cursor behavior, cursor surfaces, and
  interactive decoration move/resize paths
- preferred buffer scale/transform and fractional-scale protocol handling
- protocol requests for regions, opaque/input regions, buffer scale, and
  fractional scaling
- libdecor and fallback decoration support
- clipboard and drag-and-drop data transfer
- runtime validation on a Wayland compositor

Not implemented yet:

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
