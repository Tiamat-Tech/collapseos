# Z80 emulation

This folder contains a couple of tools running under the [libz80][libz80]
emulator.

## Requirements

You need `curses` to build the `forth` executable.

For `sms` and `ti84` emulators, you need XCB and pkg-config.

## Build

Running `make` builds all targets described below

## Vanilla Forth

The `./forth` executable here works like the one in `/cvm`, except that it runs
under an emulated z80 machine instead of running natively. Refer to
`/cvm/README.md` for details.

`./forth` doesn't try to emulate real hardware
because the goal here is to facilitate "high level" development.

These apps run on imaginary hardware and use many cheats to simplify I/Os.

## RC2014 emulation

This emulates a RC2014 classic with 8K of ROM, 32K of RAM and an ACIA hooked to
stdin/stdout.

Run `./rc2014 /path/to/rom` (for example, `os.bin` from RC2014's recipe).
Serial I/O is hooked to stdin/stdout. `CTRL+D` to quit.

You can press `CTRL+E` to dump the whole 64K of memory into `memdump`.

Options:

* `-s` replaces the ACIA with a Zilog SIO.
* `-e` puts a 8K AT28 EEPROM at address `0x2000`.
* `-c/path/to/image` hooks up a SD card with specified contents.

## Sega Master System emulator

This emulates a Sega Master system with a monochrome screen and a Genesis pad
hooked to port A.

Launch the emulator with `./sms /path/to/rom` (you can use the binary from the
`sms` recipe.

This will show a window with the screen's content on it. The mappings to the
pad are:

* W --> Up
* A --> Left
* S --> Down
* D --> Right
* H --> A
* J --> B
* K --> C
* L --> Start

If your ROM is configured with PS/2 keyboard input, run this emulator with the
`-k` flag to replace SMS pad emulation with keyboard emulation.

The `-c` option connects a SD card in the same way as the RC2014 emulator.

In both cases (pad or keyboard), only port A emulation is supported.

Press ESC to quit.

## TI-84

This emulates a TI-84+ with its screen and keyboard. This is suitable for
running the `ti84` recipe.

Launch the emulator with `./ti84 /path/to/rom` (you can use the binary from the
`ti84` recipe. Use the small one, not the one having been filled to 1MB).

This will show a window with the LCD screen's content on it. Most applications,
upon boot, halt after initialization and stay halted until the ON key is
pressed. The ON key is mapped to the tilde (~) key.

Press ESC to quit.

As for the rest of the mappings, they map at the key level. For example, the 'Y'
key maps to '1' (which yields 'y' when in alpha mode). Therefore, '1' and 'Y'
map to the same calculator key. Backspace maps to DEL.

Left Shift maps to 2nd. Left Ctrl maps to Alpha.
[libz80]: https://github.com/ggambetta/libz80
