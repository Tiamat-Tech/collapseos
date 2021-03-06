( ----- 600 )
601 MC6850 driver              606 Zilog SIO driver
615 SPI relay                  619 Xcomp unit
( ----- 601 )
( MC6850 Driver. Load range B601-B603. Requires:
  6850_CTL for control register
  6850_IO for data register.
  CTL numbers used: 0x16 = no interrupt, 8bit words, 1 stop bit
  64x divide. 0x56 = RTS high )
: _rts 0x16 ( RTS low ) [ 6850_CTL LITN ] PC! ;
: _rts^ 0x56 ( RTS high ) [ 6850_CTL LITN ] PC! ;
: 6850<? ( -- c? f )
    [ 6850_CTL LITN ] PC@ 1 AND ( is rcv buff full ? )
    NOT IF ( RTS low, then wait 1ms and try again )
        _rts 10 TICKS ( 1ms ) _rts^
        [ 6850_CTL LITN ] PC@ 1 AND ( is rcv buff full ? )
        NOT IF 0 EXIT THEN
    THEN [ 6850_IO LITN ] PC@ ( c ) 1 ( f ) ;
( ----- 602 )
CODE 6850>
    HL POP, chkPS,
    BEGIN,
        6850_CTL INAi, 0x02 ANDi, ( are we transmitting? )
    JRZ, ( yes, loop ) AGAIN,
    A L LDrr, 6850_IO OUTiA,
;CODE
( ----- 603 )
: (key?) 6850<? ;
: (emit) 6850> ;
: 6850$ 0x56 ( RTS high ) [ 6850_CTL LITN ] PC! ;
( ----- 605 )
( Zilog SIO driver. Load range B605-607. Requires:
  SIOA_CTL for ch A control register
  SIOA_DATA for ch A data register
  SIOB_CTL for ch B control register
  SIOB_DATA for ch B data register )
: _<? ( io ctl -- c? f )
    DUP ( io ctl ctl ) PC@ 1 AND ( is rcv buff full ? )
    NOT IF ( io ctl )
        0x05 ( PTR5 ) OVER PC! 0b01101000 OVER PC! ( RTS low )
        10 TICKS ( 1ms )
        0x05 ( PTR5 ) OVER PC! 0b01101010 OVER PC! ( RTS high )
        PC@ 1 AND ( is rcv buff full ? )
        NOT IF DROP 0 ( f ) EXIT THEN
    ELSE DROP THEN ( io ) PC@ ( c ) 1 ( f ) ;
: SIOA<? [ SIOA_DATA LITN SIOA_CTL LITN ] _<? ;
( ----- 606 )
CODE SIOA>
    HL POP, chkPS,
    BEGIN,
        SIOA_CTL INAi, 0x04 ANDi, ( are we transmitting? )
    JRZ, ( yes, loop ) AGAIN,
    A L LDrr, SIOA_DATA OUTiA,
;CODE
CREATE _ ( init data ) 0x18 C, ( CMD3 )
    0x24 C, ( CMD2/PTR4 ) 0b11000100 C, ( WR4/64x/1stop/nopar )
    0x03 C, ( PTR3 ) 0b11000001 C, ( WR3/RXen/8char )
    0x05 C, ( PTR5 ) 0b01101010 C, ( WR5/TXen/8char/RTS )
    0x21 C, ( CMD2/PTR1 ) 0 C, ( WR1/Rx no INT )
: SIOA$ 9 0 DO _ I + C@ [ SIOA_CTL LITN ] PC! LOOP ;
( ----- 607 )
: SIOB<? [ SIOB_DATA LITN SIOB_CTL LITN ] _<? ;
CODE SIOB>
    HL POP, chkPS,
    BEGIN,
        SIOB_CTL INAi, 0x04 ANDi, ( are we transmitting? )
    JRZ, ( yes, loop ) AGAIN,
    A L LDrr, SIOB_DATA OUTiA,
;CODE
: SIOB$ 9 0 DO _ I + C@ [ SIOB_CTL LITN ] PC! LOOP ;
( ----- 619 )
( RC2014 classic with MC6850 )
0xff00 CONSTANT RS_ADDR        0xfffa CONSTANT PS_ADDR
RS_ADDR 0xa0 - CONSTANT SYSVARS
0x8000 CONSTANT HERESTART
0x80 CONSTANT 6850_CTL 0x81 CONSTANT 6850_IO
4 CONSTANT SPI_DATA 5 CONSTANT SPI_CTL 1 CONSTANT SDC_DEVID
5 LOAD    ( z80 assembler )
262 LOAD  ( xcomp )            282 LOAD  ( boot.z80.decl )
270 LOAD  ( xcomp overrides )  283 335 LOADR ( boot.z80 )
353 LOAD  ( xcomp core low )   601 603 LOADR ( MC6850 )
419 LOAD  ( SPI relay )        423 436 LOADR ( SD Card )
400 LOAD  ( AT28 )
390 LOAD  ( xcomp core high )
(entry) _
PC ORG @ 8 + ! ( Update LATEST )
," 6850$ BLK$ " EOT,
( ----- 620 )
( RC2014 classic with SIO )
0xff00 CONSTANT RS_ADDR        0xfffa CONSTANT PS_ADDR
RS_ADDR 0xa0 - CONSTANT SYSVARS
0x8000 CONSTANT HERESTART
0x80 CONSTANT SIOA_CTL   0x81 CONSTANT SIOA_DATA
0x82 CONSTANT SIOB_CTL   0x83 CONSTANT SIOB_DATA
4 CONSTANT SPI_DATA 5 CONSTANT SPI_CTL 1 CONSTANT SDC_DEVID
5 LOAD    ( z80 assembler )
262 LOAD  ( xcomp )            282 LOAD  ( boot.z80.decl )
270 LOAD  ( xcomp overrides )  283 335 LOADR ( boot.z80 )
353 LOAD  ( xcomp core low )   605 607 LOADR ( SIO )
419 LOAD  ( SPI relay )        423 436 LOADR ( SD Card )
400 LOAD  ( AT28 ) : (key?) SIOA<? ; : (emit) SIOA> ;
390 LOAD  ( xcomp core high )
(entry) _ PC ORG @ 8 + ! ( Update LATEST )
," SIOA$ BLK$ " EOT,
