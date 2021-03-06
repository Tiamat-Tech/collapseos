#include <stdint.h>
#include <stdio.h>
#include <unistd.h>
#include <curses.h>
#include <termios.h>
#include "vm.h"

#ifndef BLKFS_PATH
#error BLKFS_PATH needed
#endif
#ifndef FBIN_PATH
#error FBIN_PATH needed
#endif
#define WCOLS 80
#define WLINES 32
#define STDIO_PORT 0x00
// This binary is also used for automated tests and those tests, when
// failing, send a non-zero value to RET_PORT to indicate failure
#define RET_PORT 0x01
#define SETX_PORT 0x05
#define SETY_PORT 0x06

static FILE *fp;
static int retcode = 0;
WINDOW *bw, *dw, *w;

void debug_panel()
{
    char buf[30];
    VM_debugstr(buf);
    mvwaddnstr(dw, 0, 0, buf, 30);
    wrefresh(dw);
}

static uint8_t iord_stdio()
{
    int c;
    if (fp != NULL) {
        c = getc(fp);
    } else {
        debug_panel();
        c = wgetch(w);
    }
    if (c == EOF) {
        c = 4; // ASCII EOT
    }
    return (uint8_t)c;
}

static void iowr_stdio(uint8_t val)
{
    if (fp != NULL) {
        putchar(val);
    } else {
        if (val >= 0x20) {
            wechochar(w, val);
        }
    }
}

static void iowr_ret(uint8_t val)
{
    retcode = val;
}

static void iowr_setx(uint8_t val)
{
    int y, x; getyx(w, y, x);
    wmove(w, y, val);
}

static void iowr_sety(uint8_t val)
{
    int y, x; getyx(w, y, x);
    wmove(w, val, x);
}

int main(int argc, char *argv[])
{
    VM *vm = VM_init(FBIN_PATH, BLKFS_PATH);
    if (!vm) {
        return 1;
    }
    vm->iord[STDIO_PORT] = iord_stdio;
    vm->iowr[STDIO_PORT] = iowr_stdio;
    vm->iowr[RET_PORT] = iowr_ret;
    vm->iowr[SETX_PORT] = iowr_setx;
    vm->iowr[SETY_PORT] = iowr_sety;
    w = NULL;
    if (argc == 2) {
        fp = fopen(argv[1], "r");
        if (fp == NULL) {
            fprintf(stderr, "Can't open %s\n", argv[1]);
            return 1;
        }
        while (VM_steps(1000));
        fclose(fp);
    } else if (argc == 1) {
        fp = NULL;
        initscr(); cbreak(); noecho(); nl(); clear();
        // border window
        bw = newwin(WLINES+2, WCOLS+2, 0, 0);
        wborder(bw, 0, 0, 0, 0, 0, 0, 0, 0);
        wrefresh(bw);
        // debug panel
        dw = newwin(1, 30, LINES-1, COLS-30);
        w = newwin(WLINES, WCOLS, 1, 1);
        scrollok(w, 0);
        while (VM_steps(1000)) {
            debug_panel();
        }
        nocbreak(); echo(); delwin(w); delwin(bw); delwin(dw); endwin();
        printf("\nDone!\n");
        fprintf(stderr, "Done!\n");
        VM_printdbg();
    } else {
        fprintf(stderr, "Usage: ./forth [filename]\n");
        retcode = 1;
    }
    VM_deinit();
    return retcode;
}
