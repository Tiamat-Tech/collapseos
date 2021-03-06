#include <stdlib.h>
#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>
#include <stdbool.h>

#include "common.h"

/* Push specified file to specified device blk device, starting from blkno
 * and upwards.
 */

int main(int argc, char **argv)
{
    if (argc != 4) {
        fprintf(stderr, "Usage: ./blkup device blkno fname\n");
        return 1;
    }
    unsigned int blkno = strtol(argv[2], NULL, 10);
    FILE *fp = fopen(argv[3], "r");
    if (!fp) {
        fprintf(stderr, "Can't open %s.\n", argv[3]);
        return 1;
    }
    int fd = ttyopen(argv[1]);
    if (fd < 0) {
        fprintf(stderr, "Could not open %s\n", argv[1]);
        return 1;
    }
    char s[0x40];
    char buf[1024] = {0};
    sendcmdp(fd, ": _ 1024 0 DO KEY DUP .x I BLK( + C! LOOP ;");
    sendcmdp(fd, ": Z BLK( 1024 0 FILL ;");

    int returncode = 0;
    while (fread(buf, 1, 1024, fp)) {
        bool allzero = true;
        for (int i=0; i<1024; i++) {
            if (buf[i] != 0) {
                allzero = false;
                break;
            }
        }
        if (allzero) {
            sendcmdp(fd, "Z");
            putchar('Z');
            fflush(stdout);
        } else {
            sendcmd(fd, "_");
            for (int i=0; i<1024; i++) {
                putchar('.');
                fflush(stdout);
                write(fd, &buf[i], 1);
                usleep(1000); // let it breathe
                mread(fd, s, 2); // read hex pair
                s[2] = 0; // null terminate
                unsigned char c = strtol(s, NULL, 16);
                if (c != buf[i]) {
                    // mismatch!
                    fprintf(stderr, "Mismatch at bno %d (%d) %d != %d.\n", blkno, i, buf[i], c);
                    // we don't exit now because we need to "consume" our whole program.
                    returncode = 1;
                }
                usleep(1000); // let it breathe
            }
            readprompt(fd);
            if (returncode) break;
            memset(buf, 0, 1024);
        }
        sprintf(s, "%d BLK> ! BLK!", blkno);
        sendcmdp(fd, s);
        blkno++;
    }
    sendcmdp(fd, "FORGET _");
    printf("Done!\n");
    fclose(fp);
    return returncode;
}

