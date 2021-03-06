( xcomp using the Text Mode if the VDP. Only works on actual
  SMS. The Megadrive's VDP doesn't have TMS9918 modes in it. )
( 8K of onboard RAM )
0xdd00 CONSTANT RS_ADDR
( Memory register at the end of RAM. Must not overwrite )
0xddca CONSTANT PS_ADDR
RS_ADDR 0xb0 - CONSTANT SYSVARS
0xc000 CONSTANT HERESTART
0xbf   CONSTANT TMS_CTLPORT
0xbe   CONSTANT TMS_DATAPORT
SYSVARS 0xa0 + CONSTANT GRID_MEM
SYSVARS 0xa3 + CONSTANT CPORT_MEM
0x3f   CONSTANT CPORT_CTL
0xdc   CONSTANT CPORT_D1
0xdd   CONSTANT CPORT_D2
SYSVARS 0xa4 + CONSTANT PS2_MEM
5 LOAD  ( z80 assembler )
262 LOAD  ( xcomp )
523 LOAD  ( font compiler )
165 LOAD  ( Sega ROM signer )
282 LOAD  ( boot.z80.decl )
270 LOAD  ( xcomp overrides )

DI, 0x100 JP, 0x62 ALLOT0 ( 0x66 )
RETN, 0x98 ALLOT0 ( 0x100 )
( All set, carry on! )
CURRENT @ XCURRENT !
0x100 BIN( !
283 335 LOADR ( boot.z80 )
353 LOAD  ( xcomp core low )
CREATE ~FNT CPFNT5x7
470 472 LOADR ( TMS9918 )
402 404 LOADR ( Grid )
625 626 LOADR ( SMS ports )
620 LOAD ( PAD ) : (ps2kc) (ps2kcA) ; 411 414 LOADR
622 LOAD ( SPI )
1 CONSTANT SDC_DEVID 423 436 LOADR ( SDC )
390 LOAD  ( xcomp core high )
(entry) _
( Update LATEST )
PC ORG @ 8 + !
," TMS$ GRID$ PS2$ BLK$ ' SDC@ ' BLK@* **! (im1) " EOT,
ORG @ 0x100 - DUP |M 2 PC! 2 PC!
DUP 1 ( 16K ) segasig
0x4000 + |M 2 PC! 2 PC!
