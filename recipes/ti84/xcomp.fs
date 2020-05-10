0x8000 CONSTANT RAMSTART
0xb000 CONSTANT RS_ADDR
RAMSTART 0x70 + CONSTANT LCD_MEM
RAMSTART 0x72 + CONSTANT KBD_MEM
0x01 CONSTANT KBD_PORT
212 LOAD  ( z80 assembler )
262 LOAD  ( xcomp )
522 LOAD  ( font compiler )
: CODE XCODE ;
: IMMEDIATE XIMM ;
: (entry) (xentry) ;
: CREATE XCREATE ; ( for KBD tbls )
: : [ ' X: , ] ;

CURRENT @ XCURRENT !

282 LOAD  ( boot.z80 )
393 LOAD  ( icore low )
555 557 LOADR ( LCD low )
566 570 LOADR ( KBD low )
415 LOAD  ( icore high )
(entry) ~FNT CPFNT3x5
(entry) _
( Update LATEST )
PC ORG @ 8 + !
422 437 XPACKR ( core )
558 560 XPACKR ( LCD high )
438 451 XPACKR ( print fmt readln )
," : _ LCD$ KBD$ (ok) RDLN$ ; _ "
ORG @ 256 /MOD 2 PC! 2 PC!
H@ 256 /MOD 2 PC! 2 PC!
