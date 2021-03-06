( ----- 000 )
MASTER INDEX

005 Z80 assembler             030 8086 assembler
050 AVR assembler             70-99 unused
100 Block editor              120 Visual Editor
150 Remote Shell
160 AVR SPI programmer        165 Sega ROM signer
170-259 unused                260 Cross compilation
280 Z80 boot code             350 Core words
400 AT28 EEPROM driver        401 Grid subsystem
410 PS/2 keyboard subsystem   418 Z80 SPI Relay driver
420 SD Card subsystem         440 8086 boot code
470 Z80 TMS9918 driver
480-519 unused                520 Fonts
( ----- 005 )
( Z80 Assembler

006 Variables & consts
007 Utils                      008 OP1
010 OP1r                       012 OP1d
013 OP1rr                      015 OP2
016 OP2i                       017 OP2ri
018 OP2br                      019 OProt
020 OP2r                       021 OP2d
022 OP3di                      023 OP3i
024 Specials                   025 Flow
028 Macros )
1 23 LOADR+
( ----- 006 )
CREATE ORG 0 ,
CREATE BIN( 0 ,
VARIABLE L1 VARIABLE L2 VARIABLE L3 VARIABLE L4
: A 7 ; : B 0 ; : C 1 ; : D 2 ;
: E 3 ; : H 4 ; : L 5 ; : (HL) 6 ;
: BC 0 ; : DE 1 ; : HL 2 ; : AF 3 ; : SP AF ;
: CNZ 0 ; : CZ 1 ; : CNC 2 ; : CC 3 ;
: CPO 4 ; : CPE 5 ; : CP 6 ; : CM 7 ;
( ----- 007 )
: PC H@ ORG @ - BIN( @ + ;
: <<3 3 LSHIFT ;    : <<4 4 LSHIFT ;
( As a general rule, IX and IY are equivalent to spitting an
  extra 0xdd / 0xfd and then spit the equivalent of HL )
: IX 0xdd C, HL ; : IY 0xfd C, HL ;
: _ix+- 0xff AND 0xdd C, (HL) ;
: _iy+- 0xff AND 0xfd C, (HL) ;
: IX+ _ix+- ; : IX- 0 -^ _ix+- ;
: IY+ _iy+- ; : IY- 0 -^ _iy+- ;
( ----- 008 )
: OP1 CREATE C, DOES> C@ C, ;
0xf3 OP1 DI,                   0xfb OP1 EI,
0xeb OP1 EXDEHL,               0xd9 OP1 EXX,
0x08 OP1 EXAFAF',              0xe3 OP1 EX(SP)HL,
0x76 OP1 HALT,                 0xe9 OP1 JP(HL),
0x12 OP1 LD(DE)A,              0x1a OP1 LDA(DE),
0x02 OP1 LD(BC)A,              0x0a OP1 LDA(BC),
0x00 OP1 NOP,                  0xc9 OP1 RET,
0x17 OP1 RLA,                  0x07 OP1 RLCA,
0x1f OP1 RRA,                  0x0f OP1 RRCA,
0x37 OP1 SCF,
( ----- 009 )
( Relative jumps are a bit special. They're supposed to take
  an argument, but they don't take it so they can work with
  the label system. Therefore, relative jumps are an OP1 but
  when you use them, you're expected to write the offset
  afterwards yourself. )

0x18 OP1 JR,                   0x10 OP1 DJNZ,
0x38 OP1 JRC,                  0x30 OP1 JRNC,
0x28 OP1 JRZ,                  0x20 OP1 JRNZ,
( ----- 010 )
( r -- )
: OP1r
    CREATE C,
    DOES>
    C@              ( r op )
    SWAP            ( op r )
    <<3             ( op r<<3 )
    OR C,
;
0x04 OP1r INCr,                0x05 OP1r DECr,
: INC(IXY+), INCr, C, ;
: DEC(IXY+), DECr, C, ;
( also works for c )
0xc0 OP1r RETc,
( ----- 011 )
: OP1r0 ( r -- )
    CREATE C, DOES>
    C@ ( r op ) OR C, ;
0x80 OP1r0 ADDr,               0x88 OP1r0 ADCr,
0xa0 OP1r0 ANDr,               0xb8 OP1r0 CPr,
0xb0 OP1r0 ORr,                0x90 OP1r0 SUBr,
0x98 OP1r0 SBCr,               0xa8 OP1r0 XORr,
: CP(IXY+), CPr, C, ;
( ----- 012 )
: OP1d
    CREATE C,
    DOES>
    C@              ( d op )
    SWAP            ( op d )
    <<4             ( op d<<4 )
    OR C,
;
0xc5 OP1d PUSH,                0xc1 OP1d POP,
0x03 OP1d INCd,                0x0b OP1d DECd,
0x09 OP1d ADDHLd,

: ADDIXd, 0xdd C, ADDHLd, ;  : ADDIXIX, HL ADDIXd, ;
: ADDIYd, 0xfd C, ADDHLd, ;  : ADDIYIY, HL ADDIYd, ;
( ----- 013 )
: _1rr
    C@              ( rd rr op )
    ROT             ( rr op rd )
    <<3             ( rr op rd<<3 )
    OR OR C,
;

( rd rr )
: OP1rr
    CREATE C,
    DOES>
    _1rr
;
0x40 OP1rr LDrr,
( ----- 014 )
( ixy+- HL rd )
: LDIXYr,
    ( dd/fd has already been spit )
    LDrr,           ( ixy+- )
    C,
;

( rd ixy+- HL )
: LDrIXY,
    ROT             ( ixy+- HL rd )
    SWAP            ( ixy+- rd HL )
    LDIXYr,
;
( ----- 015 )
: OP2 CREATE , DOES> @ |M C, C, ;
0xeda1 OP2 CPI,                0xedb1 OP2 CPIR,
0xeda9 OP2 CPD,                0xedb9 OP2 CPDR,
0xed46 OP2 IM0,                0xed56 OP2 IM1,
0xed5e OP2 IM2,
0xeda0 OP2 LDI,                0xedb0 OP2 LDIR,
0xeda8 OP2 LDD,                0xedb8 OP2 LDDR,
0xed44 OP2 NEG,
0xed4d OP2 RETI,               0xed45 OP2 RETN,
( ----- 016 )
: OP2i ( i -- )
    CREATE C,
    DOES>
    C@ C, C,
;
0xd3 OP2i OUTiA,
0xdb OP2i INAi,
0xc6 OP2i ADDi,
0xe6 OP2i ANDi,
0xf6 OP2i ORi,
0xd6 OP2i SUBi,
0xee OP2i XORi,
0xfe OP2i CPi,
( ----- 017 )
: OP2ri ( r i -- )
    CREATE C,
    DOES>
    C@              ( r i op )
    ROT             ( i op r )
    <<3             ( i op r<<3 )
    OR C, C,
;
0x06 OP2ri LDri,
( ----- 018 )
( b r -- )
: OP2br
    CREATE C,
    DOES>
    0xcb C,
    C@              ( b r op )
    ROT             ( r op b )
    <<3             ( r op b<<3 )
    OR OR C,
;
0xc0 OP2br SET,
0x80 OP2br RES,
0x40 OP2br BIT,
( ----- 019 )
( bitwise rotation ops have a similar sig )
: OProt ( r -- )
    CREATE C,
    DOES>
    0xcb C,
    C@              ( r op )
    OR C,
;
0x10 OProt RL,
0x00 OProt RLC,
0x18 OProt RR,
0x08 OProt RRC,
0x20 OProt SLA,
0x38 OProt SRL,
( ----- 020 )
( cell contains both bytes. MSB is spit as-is, LSB is ORed
  with r )
( r -- )
: OP2r
    CREATE ,
    DOES>
    @ |M    ( r lsb msb )
    C,      ( r lsb )
    SWAP <<3 ( lsb r<<3 )
    OR C,
;
0xed41 OP2r OUT(C)r,
0xed40 OP2r INr(C),
( ----- 021 )
: OP2d ( d -- )
    CREATE C,
    DOES>
    0xed C,
    C@ SWAP         ( op d )
    <<4             ( op d<< 4 )
    OR C,
;
0x4a OP2d ADCHLd,
0x42 OP2d SBCHLd,
( ----- 022 )
( d i -- )
: OP3di
    CREATE C,
    DOES>
    C@              ( d n op )
    ROT             ( n op d )
    <<4             ( n op d<<4 )
    OR C, ,
;
0x01 OP3di LDdi,
( ----- 023 )
( i -- )
: OP3i
    CREATE C,
    DOES>
    C@ C, ,
;
0xcd OP3i CALL,
0xc3 OP3i JP,
0x22 OP3i LD(i)HL,             0x2a OP3i LDHL(i),
0x32 OP3i LD(i)A,              0x3a OP3i LDA(i),
( ----- 024 )
: LDd(i), ( d i -- )
    0xed C,
    SWAP <<4 0x4b OR C, ,
;
: LD(i)d, ( i d -- )
    0xed C,
    <<4 0x43 OR C, ,
;
: RST, 0xc7 OR C, ;

: JP(IX), IX DROP JP(HL), ;
: JP(IY), IY DROP JP(HL), ;
( ----- 025 )
: JPc, SWAP <<3 0xc2 OR C, , ;
: BCALL, BIN( @ + CALL, ;
: BJP, BIN( @ + JP, ;
: BJPc, BIN( @ + JPc, ;

CREATE lblchkPS 0 ,
: chkPS, lblchkPS @ CALL, ; ( chkPS, B305 )
CREATE lblnext 0 , ( stable ABI until set in B300 )
: JPNEXT, lblnext @ ?DUP IF JP, ELSE 0x1a BJP, THEN ;
: CODE ( same as CREATE, but with native word )
    (entry) 0 C, ( 0 == native ) ;
: ;CODE JPNEXT, ;
( ----- 026 )
( Place BEGIN, where you want to jump back and AGAIN after
  a relative jump operator. Just like BSET and BWR. )
: BEGIN,
    PC DUP 0x8000 AND IF ABORT" PC must be < 0x8000" THEN ;
: BSET BEGIN, SWAP ! ;
( same as BSET, but we need to write a placeholder )
: FJR, BEGIN, 0 C, ;
: IFZ, JRNZ, FJR, ;
: IFNZ, JRZ, FJR, ;
: IFC, JRNC, FJR, ;
: IFNC, JRC, FJR, ;
: THEN,
    DUP PC ( l l pc ) -^ 1- ( l off )
    ( warning: l is a PC offset, not a mem addr! )
    SWAP ORG @ + BIN( @ - ( off addr ) C! ;
: ELSE, JR, FJR, SWAP THEN, ;
( ----- 027 )
: FWR BSET 0 C, ;
: FSET @ THEN, ;
: BREAK, FJR, 0x8000 OR ;
: BREAK?, DUP 0x8000 AND IF
        0x7fff AND 1 ALLOT THEN, -1 ALLOT
    THEN ;
: AGAIN, BREAK?, PC - 1- C, ;
: BWR @ AGAIN, ;
( ----- 028 )
( Macros )
( clear carry + SBC )
: SUBHLd, A ORr, SBCHLd, ;
: PUSH0, DE 0 LDdi, DE PUSH, ;
: PUSH1, DE 1 LDdi, DE PUSH, ;
: PUSHZ, DE 0 LDdi, IFZ, DE INCd, THEN, DE PUSH, ;
: PUSHA, D 0 LDri, E A LDrr, DE PUSH, ;
: HLZ, A H LDrr, L ORr, ;
: DEZ, A D LDrr, E ORr, ;
: LDDE(HL), E (HL) LDrr, HL INCd, D (HL) LDrr, ;
: OUTHL, DUP A H LDrr, OUTiA, A L LDrr, OUTiA, ;
: OUTDE, DUP A D LDrr, OUTiA, A E LDrr, OUTiA, ;
( ----- 030 )
( 8086 assembler. See doc/asm.txt )
1 13 LOADR+
( ----- 031 )
VARIABLE ORG
CREATE BIN( 0 , : BIN(+ BIN( @ + ;
VARIABLE L1 VARIABLE L2 VARIABLE L3 VARIABLE L4
: AL 0 ; : CL 1 ; : DL 2 ; : BL 3 ;
: AH 4 ; : CH 5 ; : DH 6 ; : BH 7 ;
: AX 0 ; : CX 1 ; : DX 2 ; : BX 3 ;
: SP 4 ; : BP 5 ; : SI 6 ; : DI 7 ;
: ES 0 ; : CS 1 ; : SS 2 ; : DS 3 ;
: [BX+SI] 0 ; : [BX+DI] 1 ; : [BP+SI] 2 ; : [BP+DI] 3 ;
: [SI] 4 ; : [DI] 5 ; : [BP] 6 ; : [BX] 7 ;
: <<3 3 LSHIFT ;
( ----- 032 )
: PC H@ ORG @ - BIN( @ + ;
( ----- 033 )
: OP1 CREATE C, DOES> C@ C, ;
0xc3 OP1 RET,        0xfa OP1 CLI,       0xfb OP1 STI,
0xf4 OP1 HLT,        0xfc OP1 CLD,       0xfd OP1 STD,
0x90 OP1 NOP,        0x98 OP1 CBW,
0xf3 OP1 REPZ,       0xf2 OP1 REPNZ,     0xac OP1 LODSB,
0xad OP1 LODSW,      0xa6 OP1 CMPSB,     0xa7 OP1 CMPSW,
0xa4 OP1 MOVSB,      0xa5 OP1 MOVSW,     0xae OP1 SCASB,
0xaf OP1 SCASW,      0xaa OP1 STOSB,     0xab OP1 STOSW,
( no argument, jumps with relative addrs are special )
0xeb OP1 JMPs,       0xe9 OP1 JMPn,      0x74 OP1 JZ,
0x75 OP1 JNZ,        0x72 OP1 JC,        0x73 OP1 JNC,
0xe8 OP1 CALL,

: OP1r CREATE C, DOES> C@ + C, ;
0x40 OP1r INCx,      0x48 OP1r DECx,
0x58 OP1r POPx,      0x50 OP1r PUSHx,
( ----- 034 )
: OPr0 ( reg op ) CREATE C, C, DOES>
    C@+ C, C@ <<3 OR 0xc0 OR C, ;
0 0xd0 OPr0 ROLr1,   0 0xd1 OPr0 ROLx1,  4 0xf6 OPr0 MULr,
1 0xd0 OPr0 RORr1,   1 0xd1 OPr0 RORx1,  4 0xf7 OPr0 MULx,
4 0xd0 OPr0 SHLr1,   4 0xd1 OPr0 SHLx1,  6 0xf6 OPr0 DIVr,
5 0xd0 OPr0 SHRr1,   5 0xd1 OPr0 SHRx1,  6 0xf7 OPr0 DIVx,
0 0xd2 OPr0 ROLrCL,  0 0xd3 OPr0 ROLxCL, 1 0xfe OPr0 DECr,
1 0xd2 OPr0 RORrCL,  1 0xd3 OPr0 RORxCL, 0 0xfe OPr0 INCr,
4 0xd2 OPr0 SHLrCL,  4 0xd3 OPr0 SHLxCL,
5 0xd2 OPr0 SHRrCL,  5 0xd3 OPr0 SHRxCL,
( ----- 035 )
: OPrr CREATE C, DOES> C@ C, <<3 OR 0xc0 OR C, ;
0x31 OPrr XORxx,     0x30 OPrr XORrr,
0x88 OPrr MOVrr,     0x89 OPrr MOVxx,    0x28 OPrr SUBrr,
0x29 OPrr SUBxx,     0x08 OPrr ORrr,     0x09 OPrr ORxx,
0x38 OPrr CMPrr,     0x39 OPrr CMPxx,    0x00 OPrr ADDrr,
0x01 OPrr ADDxx,     0x20 OPrr ANDrr,    0x21 OPrr ANDxx,
( ----- 036 )
: OPm ( modrm op ) CREATE C, C, DOES> C@+ C, C@ OR C, ;
0 0xff OPm INC[w], 0 0xfe OPm INC[b],
0x8 0xff OPm DEC[w], 0x8 0xfe OPm DEC[b],
0x30 0xff OPm PUSH[w], 0 0x8f OPm POP[w],

: OPm+ ( modrm op ) CREATE C, C, DOES>
    ( m off ) C@+ C, C@ ROT OR C, C, ;
0x40 0xff OPm+ INC[w]+, 0x40 0xfe OPm+ INC[b]+,
0x48 0xff OPm+ DEC[w]+, 0x48 0xfe OPm+ DEC[b]+,
0x70 0xff OPm+ PUSH[w]+, 0x40 0x8f OPm+ POP[w]+,
( ----- 037 )
: OPrm CREATE C, DOES> C@ C, SWAP 3 LSHIFT OR C, ;
0x8a OPrm MOVr[],    0x8b OPrm MOVx[],
0x3a OPrm CMPr[],    0x3b OPrm CMPx[],

: OPmr CREATE C, DOES> C@ C, 3 LSHIFT OR C, ;
0x88 OPmr MOV[]r,    0x89 OPmr MOV[]x,

: OPrm+ ( r m off ) CREATE C, DOES>
    C@ C, ROT 3 LSHIFT ROT OR 0x40 OR C, C, ;
0x8a OPrm+ MOVr[]+,    0x8b OPrm+ MOVx[]+,
0x3a OPrm+ CMPr[]+,    0x3b OPrm+ CMPx[]+,

: OPm+r ( m off r ) CREATE C, DOES>
    C@ C, 3 LSHIFT ROT OR 0x40 OR C, C, ;
0x88 OPm+r MOV[]+r,    0x89 OPm+r MOV[]+x,
( ----- 038 )
: OPi CREATE C, DOES> C@ C, C, ;
0x04 OPi ADDALi,     0x24 OPi ANDALi,    0x2c OPi SUBALi,
0xcd OPi INT,
: OPI CREATE C, DOES> C@ C, , ;
0x05 OPI ADDAXI,     0x25 OPI ANDAXI,    0x2d OPI SUBAXI,
( ----- 040 )
: MOVri, SWAP 0xb0 OR C, C, ;
: MOVxI, SWAP 0xb8 OR C, , ;
: MOVsx, 0x8e C, SWAP <<3 OR 0xc0 OR C, ;
: MOVrm, 0x8a C, SWAP <<3 0x6 OR C, , ;
: MOVxm, 0x8b C, SWAP <<3 0x6 OR C, , ;
: MOVmr, 0x88 C, <<3 0x6 OR C, , ;
: MOVmx, 0x89 C, <<3 0x6 OR C, , ;
: PUSHs, <<3 0x06 OR C, ; : POPs, <<3 0x07 OR C, ;
: SUBxi, 0x83 C, SWAP 0xe8 OR C, C, ;
: ADDxi, 0x83 C, SWAP 0xc0 OR C, C, ;
: JMPr, 0xff C, 7 AND 0xe0 OR C, ;
: JMPf, ( seg off ) 0xea C, |L C, C, , ;
( ----- 041 )
( Place BEGIN, where you want to jump back and AGAIN after
  a relative jump operator. Just like BSET and BWR. )
: BEGIN, PC ;
: BSET PC SWAP ! ;
( same as BSET, but we need to write a placeholder )
: FJR, PC 0 C, ;
: IFZ, JNZ, FJR, ;
: IFNZ, JZ, FJR, ;
: IFC, JNC, FJR, ;
: IFNC, JC, FJR, ;
: THEN,
    DUP PC          ( l l pc )
    -^ 1-           ( l off )
    ( warning: l is a PC offset, not a mem addr! )
    SWAP ORG @ + BIN( @ - ( off addr )
    C! ;
( ----- 042 )
: FWRs BSET 0 C, ;
: FSET @ THEN, ;
( TODO: add BREAK, )
: RPCs, PC - 1- DUP 128 + 0xff > IF ABORT" PC ovfl" THEN C, ;
: RPCn, PC - 2- , ;
: AGAIN, ( BREAK?, ) RPCs, ;
( Use RPCx with appropriate JMP/CALL op. Example:
  JMPs, 0x42 RPCs, or CALL, 0x1234 RPCn, )
( ----- 043 )
: PUSHZ, CX 0 MOVxI, IFZ, CX INCx, THEN, CX PUSHx, ;
: CODE ( same as CREATE, but with native word )
    (entry) 0 ( native ) C, ;
: ;CODE JMPn, 0x1a ( next ) RPCn, ;
VARIABLE lblchkPS
: chkPS, ( sz -- )
    CX SWAP 2 * MOVxI, CALL, lblchkPS @ RPCn, ;
( ----- 050 )
1 12 LOADR+
( ----- 051 )
VARIABLE ORG
VARIABLE L1 VARIABLE L2 VARIABLE L3 VARIABLE L4
( We divide by 2 because each PC represents a word. )
: PC H@ ORG @ - 1 RSHIFT ;
( ----- 052 )
: _oor ." arg out of range: " .X SPC> ." PC: " PC .X NL> ABORT ;
: _r8c DUP 7 > IF _oor THEN ;
: _r32c DUP 31 > IF _oor THEN ;
: _r16+c _r32c DUP 16 < IF _oor THEN ;
: _r64c DUP 63 > IF _oor THEN ;
: _r256c DUP 255 > IF _oor THEN ;
: _Rdp ( op rd -- op', place Rd ) 4 LSHIFT OR ;
( ----- 053 )
( 0000 000d dddd 0000 )
: OPRd CREATE , DOES> @ SWAP _r32c _Rdp , ;
0b1001010000000101 OPRd ASR,   0b1001010000000000 OPRd COM,
0b1001010000001010 OPRd DEC,   0b1001010000000011 OPRd INC,
0b1001001000000110 OPRd LAC,   0b1001001000000101 OPRd LAS,
0b1001001000000111 OPRd LAT,
0b1001010000000110 OPRd LSR,   0b1001010000000001 OPRd NEG,
0b1001000000001111 OPRd POP,   0b1001001000001111 OPRd PUSH,
0b1001010000000111 OPRd ROR,   0b1001010000000010 OPRd SWAP,
0b1001001000000100 OPRd XCH,
( ----- 054 )
( 0000 00rd dddd rrrr )
: OPRdRr CREATE C, DOES> C@ ( rd rr op )
    OVER _r32c 0x10 AND 3 RSHIFT OR ( rd rr op' )
    8 LSHIFT OR 0xff0f AND ( rd op' )
    SWAP _r32c _Rdp , ;
0x1c OPRdRr ADC,   0x0c OPRdRr ADD,    0x20 OPRdRr AND,
0x14 OPRdRr CP,    0x04 OPRdRr CPC,    0x10 OPRdRr CPSE,
0x24 OPRdRr EOR,   0x2c OPRdRr MOV,    0x9c OPRdRr MUL,
0x28 OPRdRr OR,    0x08 OPRdRr SBC,    0x18 OPRdRr SUB,

( 0000 0AAd dddd AAAA )
: OPRdA CREATE C, DOES> C@ ( rd A op )
    OVER _r64c 0x30 AND 3 RSHIFT OR ( rd A op' )
    8 LSHIFT OR 0xff0f AND ( rd op' ) SWAP _r32c _Rdp , ;
0xb0 OPRdA IN,     0xb8 OPRdA _ : OUT, SWAP _ ;
( ----- 055 )
( 0000 KKKK dddd KKKK )
: OPRdK CREATE C, DOES> C@ ( rd K op )
    OVER _r256c 0xf0 AND 4 RSHIFT OR ( rd K op' )
    ROT _r16+c 4 LSHIFT ROT 0x0f AND OR ( op' rdK ) C, C, ;
0x70 OPRdK ANDI,   0x30 OPRdK CPI,     0xe0 OPRdK LDI,
0x60 OPRdK ORI,    0x40 OPRdK SBCI,    0x60 OPRdK SBR,
0x50 OPRdK SUBI,

( 0000 0000 AAAA Abbb )
: OPAb CREATE C, DOES> C@ ( A b op )
    ROT _r32c 3 LSHIFT ROT _r8c OR C, C, ;
0x98 OPAb CBI,     0x9a OPAb SBI,      0x99 OPAb SBIC,
0x9b OPAb SBIS,
( ----- 056 )
: OPNA CREATE , DOES> @ , ;
0x9598 OPNA BREAK, 0x9488 OPNA CLC,    0x94d8 OPNA CLH,
0x94f8 OPNA CLI,   0x94a8 OPNA CLN,    0x94c8 OPNA CLS,
0x94e8 OPNA CLT,   0x94b8 OPNA CLV,    0x9498 OPNA CLZ,
0x9419 OPNA EIJMP, 0x9509 OPNA ICALL,  0x9519 OPNA EICALL,
0x9409 OPNA IJMP,  0x0000 OPNA NOP,    0x9508 OPNA RET,
0x9518 OPNA RETI,  0x9408 OPNA SEC,    0x9458 OPNA SEH,
0x9478 OPNA SEI,   0x9428 OPNA SEN,    0x9448 OPNA SES,
0x9468 OPNA SET,   0x9438 OPNA SEV,    0x9418 OPNA SEZ,
0x9588 OPNA SLEEP, 0x95a8 OPNA WDR,
( ----- 057 )
( 0000 0000 0sss 0000 )
: OPb CREATE , DOES> @ ( b op )
    SWAP _r8c _Rdp , ;
0b1001010010001000 OPb BCLR,   0b1001010000001000 OPb BSET,

( 0000 000d dddd 0bbb )
: OPRdb CREATE , DOES> @ ( rd b op )
    ROT _r32c _Rdp SWAP _r8c OR , ;
0b1111100000000000 OPRdb BLD,  0b1111101000000000 OPRdb BST,
0b1111110000000000 OPRdb SBRC, 0b1111111000000000 OPRdb SBRS,

( special cases )
: CLR, DUP EOR, ;  : TST, DUP AND, ; : LSL, DUP ADD, ;
( ----- 058 )
( a -- k12, absolute addr a, relative to PC in a k12 addr )
: _r7ffc DUP 0x7ff > IF _oor THEN ;
: _raddr12
    PC - DUP 0< IF 0x800 + _r7ffc 0x800 OR ELSE _r7ffc THEN ;
: RJMP _raddr12 0xc000 OR ;
: RCALL _raddr12 0xd000 OR ;
: RJMP, RJMP , ; : RCALL, RCALL , ;
( ----- 059 )
( a -- k7, absolute addr a, relative to PC in a k7 addr )
: _r3fc DUP 0x3f > IF _oor THEN ;
: _raddr7
    PC - DUP 0< IF 0x40 + _r3fc 0x40 OR ELSE _r3fc THEN ;
: _brbx ( a b op -- a ) OR SWAP _raddr7 3 LSHIFT OR ;
: BRBC 0xf400 _brbx ; : BRBS 0xf000 _brbx ; : BRCC 0 BRBC ;
: BRCS 0 BRBS ; : BREQ 1 BRBS ; : BRNE 1 BRBC ; : BRGE 4 BRBC ;
: BRHC 5 BRBC ; : BRHS 5 BRBS ; : BRID 7 BRBC ; : BRIE 7 BRBS ;
: BRLO BRCS ; : BRLT 4 BRBS ; : BRMI 2 BRBS ; : BRPL 2 BRBC ;
: BRSH BRCC ; : BRTC 6 BRBC ; : BRTS 6 BRBS ; : BRVC 3 BRBC ;
: BRVS 3 BRBS ;
( ----- 060 )
0b11100 CONSTANT X 0b01000 CONSTANT Y 0b00000 CONSTANT Z
0b11101 CONSTANT X+ 0b11001 CONSTANT Y+ 0b10001 CONSTANT Z+
0b11110 CONSTANT -X 0b11010 CONSTANT -Y 0b10010 CONSTANT -Z
: _ldst ( Rd XYZ op ) SWAP DUP 0x10 AND 8 LSHIFT SWAP 0xf AND
    OR OR ( Rd op' ) SWAP _Rdp , ;
: LD, 0x8000 _ldst ; : ST, SWAP 0x8200 _ldst ;
( ----- 061 )
( L1 LBL! .. L1 ' RJMP LBL, )
: LBL! ( l -- ) PC SWAP ! ;
: LBL, ( l op -- ) SWAP @ 1- SWAP EXECUTE , ;
: SKIP, PC 0 , ;
: TO, ( opw pc ) ( TODO: use !* instead of ! )
    ( warning: pc is a PC offset, not a mem addr! )
    2 * ORG @ + PC 1- H@ ( opw addr tgt hbkp )
    ROT HERE ! ( opw tgt hbkp ) SWAP ROT EXECUTE H@ ! ( hbkp )
    HERE ! ;
( L1 FLBL, .. L1 ' RJMP FLBL! )
: FLBL, ( l -- ) LBL! 0 , ;
: FLBL! ( l opw -- ) SWAP @ TO, ;
: BEGIN, PC ; : AGAIN?, ( op ) SWAP 1- SWAP EXECUTE , ;
: AGAIN, ['] RJMP AGAIN?, ;
: IF, ['] BREQ SKIP, ; : THEN, TO, ;
( ----- 062 )
( Constant common to all AVR models )
: R0 0 ; : R1 1 ; : R2 2 ; : R3 3 ; : R4 4 ; : R5 5 ; : R6 6 ;
: R7 7 ; : R8 8 ; : R9 9 ; : R10 10 ; : R11 11 ; : R12 12 ;
: R13 13 ; : R14 14 ; : R15 15 ; : R16 16 ; : R17 17 ;
: R18 18 ; : R19 19 ; : R20 20 ; : R21 21 ; : R22 22 ;
: R24 24 ; : R25 25 ; : R26 26 ; : R27 27 ; : R28 28 ;
: R29 29 ; : R30 30 ; : R31 31 ; : XL R26 ; : XH R27 ;
: YL R28 ; : YH R29 ; : ZL R30 ; : ZH R31 ;
( ----- 065 )
( ATmega328P definitions ) : > CONSTANT ;
0xc6 > UDR0 0xc4 > UBRR0L 0xc5 > UBRR0H 0xc2 > UCSR0C
0xc1 > UCSR0B 0xc0 > UCSR0A 0xbd > TWAMR 0xbc > TWCR
0xbb > TWDR 0xba > TWAR 0xb9 > TWSR 0xb8 > TWBR 0xb6 > ASSR
0xb4 > OCR2B 0xb3 > OCR2A 0xb2 > TCNT2 0xb1 > TCCR2B
0xb0 > TCCR2A 0x8a > OCR1BL 0x8b > OCR1BH 0x88 > OCR1AL
0x89 > OCR1AH 0x86 > ICR1L 0x87 > ICR1H 0x84 > TCNT1L
0x85 > TCNT1H 0x82 > TCCR1C 0x81 > TCCR1B 0x80 > TCCR1A
0x7f > DIDR1 0x7e > DIDR0 0x7c > ADMUX 0x7b > ADCSRB
0x7a > ADCSRA 0x79 > ADCH 0x78 > ADCL 0x70 > TIMSK2
0x6f > TIMSK1 0x6e > TIMSK0 0x6c > PCMSK1 0x6d > PCMSK2
0x6b > PCMSK0 0x69 > EICRA 0x68 > PCICR 0x66 > OSCCAL
0x64 > PRR 0x61 > CLKPR 0x60 > WDTCSR 0x3f > SREG 0x3d > SPL
0x3e > SPH 0x37 > SPMCSR 0x35 > MCUCR 0x34 > MCUSR 0x33 > SMCR
0x30 > ACSR 0x2e > SPDR 0x2d > SPSR 0x2c > SPCR 0x2b > GPIOR2
0x2a > GPIOR1 0x28 > OCR0B 0x27 > OCR0A 0x26 > TCNT0  ( cont. )
( ----- 066 )
( cont. ) 0x25 > TCCR0B 0x24 > TCCR0A 0x23 > GTCCR
0x22 > EEARH 0x21 > EEARL 0x20 > EEDR 0x1f > EECR
0x1e > GPIOR0 0x1d > EIMSK 0x1c > EIFR 0x1b > PCIFR
0x17 > TIFR2 0x16 > TIFR1 0x15 > TIFR0 0x0b > PORTD 0x0a > DDRD
0x09 > PIND 0x08 > PORTC 0x07 > DDRC 0x06 > PINC 0x05 > PORTB
0x04 > DDRB 0x03 > PINB
( ----- 100 )
Block editor

This is an application to conveniently browse the contents of
the disk blocks and edit them. You can load it with "105 LOAD".

See doc/ed.txt
( ----- 105 )
1 7 LOADR+
( ----- 106 )
CREATE ACC 0 ,
: _LIST ." Block " DUP . NL> LIST ;
: L BLK> @ _LIST ;
: B BLK> @ 1- BLK@ L ;
: N BLK> @ 1+ BLK@ L ;
( ----- 107 )
( Cursor position in buffer. EDPOS/64 is line number )
CREATE EDPOS 0 ,
CREATE IBUF 64 ALLOT0
CREATE FBUF 64 ALLOT0
: _cpos BLK( + ;
: _lpos 64 * _cpos ;
: _pln ( lineno -- )
    DUP _lpos DUP 64 + SWAP DO ( lno )
        I EDPOS @ _cpos = IF '^' EMIT THEN
        I C@ DUP SPC < IF DROP SPC THEN
        EMIT
    LOOP ( lno ) 1+ . ;
: _zbuf 64 0 FILL ; ( buf -- )
( ----- 108 )
: _type ( buf -- )
    C< DUP CR = IF 2DROP EXIT THEN SWAP DUP _zbuf ( c a )
    BEGIN ( c a ) C!+ C< TUCK 0x0d = UNTIL ( c a ) C! ;
( user-facing lines are 1-based )
: T 1- DUP 64 * EDPOS ! _pln ;
: P IBUF _type IBUF EDPOS @ _cpos 64 MOVE BLK!! ;
: _mvln+ ( ln -- move ln 1 line down )
    DUP 14 > IF DROP EXIT THEN
    _lpos DUP 64 + 64 MOVE ;
: _mvln- ( ln -- move ln 1 line up )
    DUP 14 > IF DROP 15 _lpos _zbuf
    ELSE 1+ _lpos DUP 64 - 64 MOVE THEN ;
( ----- 109 )
: _U ( U without P, used in VE )
    15 EDPOS @ 64 / - ?DUP IF
    0 DO
        14 I - _mvln+
    LOOP THEN ;
: U _U P ;
( ----- 110 )
: _F ( F without _type and _pln. used in VE )
    FBUF EDPOS @ _cpos 1+ ( a1 a2 )
    BEGIN
        C@+ ROT ( a2+1 c2 a1 ) C@+ ROT ( a2+1 a1+1 c1 c2 )
        = NOT IF DROP FBUF THEN ( a2 a1 )
        TUCK C@ CR = ( a1 a2 f1 )
        OVER BLK) = OR ( a1 a2 f1|f2 )
    UNTIL ( a1 a2 )
    DUP BLK) < IF BLK( - FBUF + -^ EDPOS ! ELSE DROP THEN ;
: F FBUF _type _F EDPOS @ 64 / _pln ;
( ----- 111 )
: _blen ( buf -- length of str in buf )
    DUP BEGIN C@+ SPC < UNTIL -^ 1- ;
: _rbufsz ( size of linebuf to the right of curpos )
    EDPOS @ 64 MOD 63 -^ ;
: _lnfix ( --, ensure no ctl chars in line before EDPOS )
    EDPOS @ DUP 0xffc0 AND 2DUP = IF 2DROP EXIT THEN DO
    I _cpos DUP C@ SPC < IF SPC SWAP C! ELSE DROP THEN LOOP ;
: _i ( i without _pln and _type. used in VE )
    _rbufsz IBUF _blen 2DUP > IF
        _lnfix TUCK - ( ilen chars-to-move )
        SWAP EDPOS @ _cpos 2DUP + ( ctm ilen a a+ilen )
        3 PICK MOVE- ( ctm ilen ) NIP ( ilen )
    ELSE DROP 1+ ( ilen becomes rbuffsize+1 ) THEN
    DUP IBUF EDPOS @ _cpos ROT MOVE ( ilen ) EDPOS +! BLK!! ;
: i IBUF _type _i EDPOS @ 64 / _pln ;
( ----- 112 )
: icpy ( n -- copy n chars from cursor to IBUF )
    IBUF _zbuf EDPOS @ _cpos IBUF ( n a buf ) ROT MOVE ;
: _X ( n -- )
    DUP icpy EDPOS @ _cpos 2DUP + ( n a1 a1+n )
    SWAP _rbufsz MOVE ( n )
    ( get to next line - n )
    DUP EDPOS @ 0xffc0 AND 0x40 + -^ _cpos ( n a )
    SWAP 0 FILL BLK!! ;
: X _X EDPOS @ 64 / _pln ;
: _E FBUF _blen _X ;
: E FBUF _blen X ;
: Y FBUF _blen icpy ;
( ----- 120 )
Visual Editor

This editor, unlike the Block Editor (B100), is grid-based
instead of being command-based. It requires the AT-XY, COLS
and LINES words to be implemented. If you don't have those,
use the Block Editor.

It is loaded with "125 LOAD" and invoked with "VE". Note that
this also fully loads the Block Editor.

This editor uses 19 lines. The top line is the status line and
it's followed by 2 lines showing the contents of IBUF and
FBUF (see B100). There are then 16 contents lines. The contents
shown is that of the currently selected block.

                                                        (cont.)
( ----- 121 )
The status line displays the active block number, then the
"modifier" and then the cursor position. When the block is dir-
ty, an "*" is displayed next. At the right corner, a mode letter
can appear. 'R' for replace, 'I' for insert, 'F' for find.











                                                         (cont.)
( ----- 122 )
All keystrokes are directly interpreted by VE and have the
effect described below.

Pressing a 0-9 digit accumulates that digit into what is named
the "modifier". That modifier affects the behavior of many
keystrokes described below. The modifier starts at zero, but
most commands interpret a zero as a 1 so that they can have an
effect.

'G' selects the block specified by the modifier as the current
block. Any change made to the previously selected block is
saved beforehand.

'[' and ']' advances the selected block by "modifier". 't' opens
the previously opened block.
                                                        (cont.)
( ----- 123 )
'h' and 'l' move the cursor by "modifier" characters. 'j' and
'k', by lines. 'g' moves to "modifier" line.

'H' goes to the beginning of the line, 'L' to the end.

'w' moves forward by "modifier" words. 'b' moves backward.
'W' moves to end-of-word. 'B', backwards.

'I', 'F', 'Y', 'X' and 'E' invoke the corresponding command

'o' inserts a blank line after the cursor. 'O', before.

'D' deletes "modifier" lines at the cursor. The first of those
lines is copied to IBUF.
                                                        (cont.)
( ----- 124 )
'f' puts the contents of your previous cursor movement into
FBUF. If that movement was a forward movement, it brings the
cursor back where it was. This allows for an efficient combi-
nation of movements and 'E'. For example, if you want to delete
the next word, you type 'w', then 'f', then check your FBUF to
be sure, then press 'E'.

'R' goes into replace mode at current cursor position.
Following keystrokes replace current character and advance
cursor. Press return to return to normal mode.

'@' re-reads current block even if it's dirty, thus undoing
recent changes.
( ----- 125 )
-20 LOAD+ ( B105, block editor )
1 7 LOADR+
( ----- 126 )
CREATE CMD 2 C, '$' C, 0 C,
CREATE PREVPOS 0 , CREATE PREVBLK 0 , CREATE xoff 0 ,
: MIN ( n n - n ) 2DUP > IF SWAP THEN DROP ;
: MAX ( n n - n ) 2DUP < IF SWAP THEN DROP ;
: large? COLS 67 > ; : col- 67 COLS MIN -^ ;
: width large? IF 64 ELSE COLS THEN ;
: acc@ ACC @ 1 MAX ; : pos@ ( x y -- ) EDPOS @ 64 /MOD ;
: num ACC @ SWAP _pdacc IF ACC ! ELSE DROP THEN ;
: nspcs ( n -- , spit n space ) 0 DO SPC> LOOP ;
: aty 0 SWAP AT-XY ;
: clrscr COLS LINES * 0 DO SPC I CELL! LOOP ;
: gutter ( ln n ) OVER + SWAP DO 67 I AT-XY '|' EMIT LOOP ;
: status 0 aty ." BLK" SPC> BLK> ? SPC> ACC ?
    SPC> pos@ . ',' EMIT . xoff @ IF '>' EMIT THEN SPC>
    BLKDTY @ IF '*' EMIT THEN 4 nspcs ;
: nums 17 1 DO 2 I + aty I . SPC> SPC> LOOP ;
( ----- 127 )
: mode! ( c -- ) 4 col- CELL! ;
: @emit C@ SPC MAX 0x7f MIN EMIT ;
: contents
    16 0 DO
        large? IF 3 ELSE 0 THEN I 3 + AT-XY
        64 I * BLK( + ( lineaddr ) xoff @ + DUP width + SWAP
        DO I @emit LOOP LOOP
    large? IF 3 16 gutter THEN ;
: selblk BLK> @ PREVBLK ! BLK@ contents ;
: pos! ( newpos -- ) EDPOS @ PREVPOS !
    DUP 0< IF DROP 0 THEN 1023 MIN EDPOS ! ;
: xoff? pos@ DROP ( x )
    xoff @ ?DUP IF < IF 0 xoff ! contents THEN ELSE
        width >= IF 64 COLS - xoff ! contents THEN THEN ;
: setpos ( -- ) pos@ 3 + ( header ) SWAP ( y x ) xoff @ -
    large? IF 3 + ( gutter ) THEN SWAP AT-XY ;
( ----- 128 )
: cmv ( n -- , char movement ) acc@ * EDPOS @ + pos! ;
: buftype ( buf ln -- )
    3 OVER AT-XY KEY DUP EMIT
    DUP SPC < IF 2DROP DROP EXIT THEN
    ( buf ln c ) 4 col- nspcs SWAP 4 SWAP AT-XY ( buf c )
    SWAP C!+ IN( _zbuf RDLN IN( SWAP 63 MOVE ;
: bufp ( buf -- )
    DUP 3 col- + SWAP DO I @emit LOOP ;
: bufs
    1 aty ." I: " IBUF bufp
    2 aty ." F: " FBUF bufp
    large? IF 0 3 gutter THEN ;
( ----- 129 )
: $G ACC @ selblk ;
: $[ BLK> @ acc@ - selblk ;
: $] BLK> @ acc@ + selblk ;
: $t PREVBLK @ selblk ;
: $I 'I' mode! IBUF 1 buftype _i bufs contents SPC mode! ;
: $F 'F' mode! FBUF 2 buftype _F bufs setpos SPC mode! ;
: $Y Y bufs ;
: $E _E bufs contents ;
: $X acc@ _X bufs contents ;
: $h -1 cmv ; : $l 1 cmv ; : $k -64 cmv ; : $j 64 cmv ;
: $H EDPOS @ 0x3c0 AND pos! ;
: $L EDPOS @ 0x3f OR pos! ;
: $g ACC @ 1 MAX 1- 64 * pos! ;
: $@ BLK> @ BLK@* 0 BLKDTY ! contents ;
( ----- 130 )
: $w EDPOS @ BLK( + acc@ 0 DO
    BEGIN C@+ WS? UNTIL BEGIN C@+ WS? NOT UNTIL LOOP
    1- BLK( - pos! ;
: $W EDPOS @ BLK( + acc@ 0 DO
    1+ BEGIN C@+ WS? NOT UNTIL BEGIN C@+ WS? UNTIL LOOP
    2- BLK( - pos! ;
: $b EDPOS @ BLK( + acc@ 0 DO
    1- BEGIN C@- WS? NOT UNTIL BEGIN C@- WS? UNTIL LOOP
    2+ BLK( - pos! ;
: $B EDPOS @ BLK( + acc@ 0 DO
    BEGIN C@- WS? UNTIL BEGIN C@- WS? NOT UNTIL LOOP
    1+ BLK( - pos! ;
( ----- 131 )
: $f EDPOS @ PREVPOS @ 2DUP = IF 2DROP EXIT THEN
    2DUP > IF DUP pos! SWAP THEN
    ( p1 p2, p1 < p2 ) OVER - 64 MIN ( pos len ) FBUF _zbuf
    SWAP _cpos FBUF ( len src dst ) ROT MOVE bufs ;
: $R ( replace mode )
    'R' mode!
    BEGIN setpos KEY DUP BS? IF -1 EDPOS +! DROP 0 THEN
        DUP SPC >= IF
        DUP EMIT EDPOS @ _cpos C! 1 EDPOS +! BLK!! 0
    THEN UNTIL SPC mode! contents ;
: $O _U EDPOS @ 0x3c0 AND DUP pos! _cpos _zbuf BLK!! contents ;
: $o EDPOS @ 0x3c0 < IF EDPOS @ 64 + EDPOS ! $O THEN ;
: $D $H 64 icpy
    acc@ 0 DO 16 EDPOS @ 64 / DO I _mvln- LOOP LOOP
    BLK!! bufs contents ;
( ----- 132 )
: UPPER DUP 'a' 'z' =><= IF 32 - THEN ;
: handle ( c -- f )
    DUP '0' '9' =><= IF num 0 EXIT THEN
    DUP CMD 2+ C! CMD FIND IF EXECUTE ELSE DROP THEN
    0 ACC ! UPPER 'Q' = ;
: VE
    1 XYMODE C! clrscr 0 ACC ! 0 PREVPOS ! nums bufs contents
    BEGIN xoff? status setpos KEY handle UNTIL
    0 XYMODE C! 19 aty IN$ ;
( ----- 150 )
( Remote Shell )
0 :* rsh<? 0 :* rsh>
: rsh BEGIN
    rsh<? IF
        DUP 4 ( EOT ) = IF DROP EXIT THEN EMIT THEN
    KEY? IF DUP 0x80 < IF rsh> ELSE DROP EXIT THEN THEN
    AGAIN ;
( ----- 160 )
( AVR Programmer, load range 160-163. doc/avr.txt )
( page size in words, 64 is default on atmega328P )
CREATE aspfpgsz 64 ,
VARIABLE aspprevx
: _x ( a -- b ) DUP aspprevx ! (spix) ;
: _xc ( a -- b ) DUP (spix) ( a b )
    DUP aspprevx @ = NOT IF ABORT" AVR err" THEN ( a b )
    SWAP aspprevx ! ( b ) ;
: _cmd ( b4 b3 b2 b1 -- r4 ) _xc DROP _xc DROP _xc DROP _x ;
: asprdy ( -- ) BEGIN 0 0 0 0xf0 _cmd 1 AND NOT UNTIL ;
: asp$ ( spidevid -- )
    ( RESET pulse ) DUP (spie) 0 (spie) (spie)
    ( wait >20ms ) 220 TICKS
    ( enable prog ) 0xac (spix) DROP
    0x53 _x DROP 0 _xc DROP 0 _x DROP ;
: asperase 0 0 0x80 0xac _cmd asprdy ;
( ----- 161 )
( fuse access. read/write one byte at a time )
: aspfl@ ( -- lfuse ) 0 0 0 0x50 _cmd ;
: aspfh@ ( -- hfuse ) 0 0 0x08 0x58 _cmd ;
: aspfe@ ( -- efuse ) 0 0 0x00 0x58 _cmd ;
: aspfl! ( lfuse -- ) 0 0xa0 0xac _cmd ;
: aspfh! ( hfuse -- ) 0 0xa8 0xac _cmd ;
: aspfe! ( efuse -- ) 0 0xa4 0xac _cmd ;
( ----- 162 )
: aspfb! ( n a --, write word n to flash buffer addr a )
    SWAP |L ( a hi lo ) ROT ( hi lo a )
    DUP ROT ( hi a a lo ) SWAP ( hi a lo a )
    0 0x40 ( hi a lo a 0 0x40 ) _cmd DROP ( hi a )
    0 0x48 _cmd DROP ;
: aspfp! ( page --, write buffer to page )
    0 SWAP aspfpgsz @ * |M ( 0 lsb msb )
    0x4c _cmd DROP asprdy ;
: aspf@ ( page a -- n, read word from flash )
    SWAP aspfpgsz @ * OR ( addr ) |M ( lsb msb )
    2DUP 0 ROT> ( lsb msb 0 lsb msb )
    0x20 _cmd ( lsb msb low )
    ROT> 0 ROT> ( low 0 lsb msb ) 0x28 _cmd 8 LSHIFT OR ;
( ----- 163 )
: aspe@ ( addr -- byte, read from EEPROM )
    0 SWAP |L ( 0 msb lsb )
    0xa0 ( 0 msb lsb 0xa0 ) _cmd ;
: aspe! ( byte addr --, write to EEPROM )
    |L ( b msb lsb )
    0xc0 ( b msb lsb 0xc0 ) _cmd DROP asprdy ;
( ----- 165 )
( Sega ROM signer. See doc/sega.txt )
: C!+^ ( a c -- a+1 ) OVER C! 1+ ;
: segasig ( addr size -- )
    0x2000 OVER LSHIFT ( a sz bytesz )
    ROT TUCK + 0x10 - ( sz a end )
    TUCK SWAP 0 ROT> ( sz end sum end a ) DO ( sz end sum )
        I C@ + LOOP ( sz end sum ) SWAP ( sz sum end )
    'T' C!+^ 'M' C!+^ 'R' C!+^ SPC C!+^ 'S' C!+^
    'E' C!+^ 'G' C!+^ 'A' C!+^ 0 C!+^ 0 C!+^
    ( sum's LSB ) OVER C!+^ ( MSB ) SWAP 8 RSHIFT OVER C! 1+
    ( sz end ) 0 C!+^ 0 C!+^ 0 C!+^ SWAP 0x4a + SWAP C! ;
( ----- 260 )
Cross compilation program

This programs allows cross compilation of boot binary and
core. Run "262 LOAD" right before your cross compilation and
then "270 LOAD" to apply xcomp overrides.

This unit depends on a properly initialized z80a with ORG and
BIN( set. That is how we determine compilation offsets.

This redefines defining words to achieve cross compilation.
The goal is two-fold:

1. Add an offset to all word references in definitions.
2. Don't shadow important words we need right now.

                                                        (cont.)
( ----- 261 )
Words overrides like ":", "IMMEDIATE" and "CODE" are not
automatically shadowed to allow the harmless inclusion of
this unit. This shadowing has to take place in your xcomp
configuration.

See /doc/cross.txt for details.
( ----- 262 )
1 3 LOADR+
( ----- 263 )
CREATE XCURRENT 0 ,
: XCON XCURRENT CURRENT* ! ; : XCOFF 0x02 RAM+ CURRENT* ! ;
: (xentry) XCON (entry) XCOFF ; : XCREATE (xentry) 2 C, ;
: X:** (xentry) 5 C, , ;
: XCODE XCON CODE XCOFF ; : XIMM XCON IMMEDIATE XCOFF ;
: _xapply ( a -- a-off )
    DUP ORG @ > IF ORG @ - BIN( @ + THEN ;
: XFIND XCURRENT @ SWAP _find DROP _xapply ;
: XLITN LIT" (n)" XFIND , , ;
: X' XCON ' XCOFF ; : X'? XCON '? XCOFF ;
: X['] XCON ' _xapply XLITN XCOFF ;
: XCOMPILE XCON ' _xapply XLITN
    LIT" ," FIND DROP _xapply , XCOFF ;
: X[COMPILE] XCON ' _xapply , XCOFF ;
( ----- 264 )
: XDO LIT" 2>R" XFIND , H@ ;
: XLOOP LIT" (loop)" XFIND , H@ - C, ;
: XIF LIT" (?br)" XFIND , H@ 1 ALLOT ;
: XELSE LIT" (br)" XFIND , 1 ALLOT [COMPILE] THEN H@ 1- ;
: XAGAIN LIT" (br)" XFIND , H@ - C, ;
: XUNTIL LIT" (?br)" XFIND , H@ - C, ;
: XLIT"
    LIT" (s)" XFIND , H@ 0 C, ,"
    DUP H@ -^ 1- SWAP C!
;
( ----- 265 )
: X:
    (xentry) 1 ( compiled ) C,
    BEGIN
    WORD DUP LIT" ;" S= IF
        DROP LIT" EXIT" XFIND , EXIT THEN
    XCURRENT @ SWAP ( xcur w ) _find ( a f )
    IF   ( a )
        DUP IMMED? IF ABORT THEN
        _xapply ,
    ELSE ( w )
        0x02 RAM+ @ SWAP ( cur w ) _find ( a f )
        IF DUP IMMED? NOT IF ABORT THEN EXECUTE
        ELSE (parse) XLITN THEN
    THEN
    AGAIN ;
( ----- 270 )
: CODE XCODE ;
: '? X'? ;
: ['] X['] ; IMMEDIATE
: COMPILE XCOMPILE ; IMMEDIATE
: [COMPILE] X[COMPILE] ; IMMEDIATE
: DO XDO ; IMMEDIATE : LOOP XLOOP ; IMMEDIATE
: IF XIF ; IMMEDIATE : ELSE XELSE ; IMMEDIATE
: AGAIN XAGAIN ; IMMEDIATE : UNTIL XUNTIL ; IMMEDIATE
: LIT" XLIT" ; IMMEDIATE : LITN XLITN ;
: IMMEDIATE XIMM ;
: (entry) (xentry) ; : CREATE XCREATE ; : :** X:** ;
: : [ ' X: , ] ;

CURRENT @ XCURRENT !
( ----- 280 )
Z80 boot code

This assembles the boot binary. It requires the Z80 assembler
(B5) and cross compilation setup (B260). It requires some
constants to be set. See doc/bootstrap.txt for details.

RESERVED REGISTERS: At all times, IX points to RSP TOS and BC
is IP. SP points to PSP TOS, but you can still use the stack
in native code. you just have to make sure you've restored it
before "next".

The boot binary is loaded in 2 parts. The first part, "decla-
rations", are loaded after xcomp, before xcomp overrides, with
"282 LOAD". The rest, after xcomp overrides, with "283 335
LOADR".
( ----- 282 )
VARIABLE lbluflw VARIABLE lblexec
( see comment at TICKS' definition )
( 7.373MHz target: 737t. outer: 37t inner: 16t )
( tickfactor = (737 - 37) / 16 )
CREATE tickfactor 44 ,
( Perform a byte write by taking into account the SYSVARS+3e
  override. )
: LD(HL)E*, SYSVARS 0x3e + LDA(i), A ORr,
    IFZ, (HL) E LDrr, ELSE, SYSVARS 0x3e + CALL, THEN, ;
( ----- 283 )
H@ ORG ! ( STABLE ABI )
0 JP, ( 00, main ) NOP, ( unused ) NOP, NOP, ( 04, BOOT )
NOP, NOP, ( 06, uflw ) NOP, NOP, ( 08, LATEST )
NOP, NOP, NOP, NOP, NOP, NOP, ( unused )
0 JP, ( RST 10 )  NOP, NOP, ( 13, oflw )
NOP, NOP, NOP, NOP, NOP, ( unused )
0 JP, ( 1a, next ) NOP, NOP, NOP, ( unused )
0 JP, ( RST 20 ) 5 ALLOT0
0 JP, ( RST 28 ) 5 ALLOT0
0 JP, ( RST 30 ) 5 ALLOT0
0 JP, ( RST 38 )
( ----- 284 )
PC ORG @ 1 + ! ( main )
    SP PS_ADDR LDdi, IX RS_ADDR LDdi,
( LATEST is a label to the latest entry of the dict. It is
  written at offset 0x08 by the process or person building
  Forth. )
    BIN( @ 0x08 + LDHL(i),
    SYSVARS 0x02 ( CURRENT ) + LD(i)HL,
HERESTART [IF]
    HL HERESTART LDdi,
[THEN]
    SYSVARS 0x04 + LD(i)HL, ( RAM+04 == HERE )
    A XORr, SYSVARS 0x3e + LD(i)A, ( 3e == ~C! )
    SYSVARS 0x41 + LD(i)A, ( 41 == ~C!ERR )
    DE BIN( @ 0x04 ( BOOT ) + LDd(i),
    JR, L1 FWR ( execute, B287 )
( ----- 286 )
lblnext BSET PC ORG @ 0x1b + ! ( next )
( This routine is jumped to at the end of every word. In it,
  we jump to current IP, but we also take care of increasing
  it by 2 before jumping. )
	( Before we continue: are we overflowing? )
    IX PUSH, EX(SP)HL, ( do EX to count the IX push in SP )
    SP SUBHLd, HL POP,
    IFNC, ( SP <= IX? overflow )
        SP PS_ADDR LDdi, IX RS_ADDR LDdi,
        DE BIN( @ 0x13 ( oflw ) + LDd(i),
        JR, L2 FWR ( execute, B287 )
    THEN,
    LDA(BC), E A LDrr, BC INCd,
    LDA(BC), D A LDrr, BC INCd,
    ( continue to execute )
( ----- 287 )
lblexec BSET L1 FSET ( B284 ) L2 FSET ( B286 )
    ( DE -> wordref )
    LDA(DE), DE INCd, EXDEHL, ( HL points to PFA )
    A ORr, IFZ, JP(HL), THEN,
    A DECr, ( compiled? ) IFNZ, ( no )
    3 CPi, IFZ, ( alias ) LDDE(HL), JR, lblexec BWR THEN,
    IFNC, ( ialias )
        LDDE(HL), EXDEHL, LDDE(HL), JR, lblexec BWR THEN,
    ( cell or does. push PFA ) HL PUSH,
    A DECr, JRZ, lblnext BWR ( cell )
    HL INCd, HL INCd, LDDE(HL), EXDEHL, ( does )
    THEN, ( continue to compiledWord )
( ----- 289 )
( compiled word. HL points to its first wordref, which we'll
  execute now.
  1. Push current IP to RS
  2. Set new IP to PFA+2
  3. Execute wordref )
    IX INCd, IX INCd,
    0 IX+ C LDIXYr,
    1 IX+ B LDIXYr,
( While we inc, dereference into DE for execute call later. )
    LDDE(HL), ( DE is new wordref )
    HL INCd, ( HL is new PFA+2 )
    B H LDrr, C L LDrr, ( --> IP )
    JR, lblexec BWR ( execute-B287 )
( ----- 290 )
lblchkPS BSET ( chkPS )
    ( thread carefully in there: sometimes, we're in the
      middle of a EXX to protect BC. BC must never be touched
      here. )
    EXX,
( We have the return address for this very call on the stack
  and protected registers. 2- is to compensate that. )
    HL PS_ADDR 2- LDdi,
    SP SUBHLd,
    EXX,
    CNC RETc, ( PS_ADDR >= SP? good )
    ( continue to uflw )
lbluflw BSET ( abortUnderflow )
    DE BIN( @ 0x06 ( uflw ) + LDd(i),
    JR, lblexec BWR
( ----- 291 )
( Native words )
H@ 5 + XCURRENT ! ( make next CODE have 0 prev field )
CODE _find  ( cur w -- a f )
    HL POP, ( w ) DE POP, ( cur ) chkPS,
    HL PUSH, ( --> lvl 1 )
	( First, figure out string len )
    A (HL) LDrr, A ORr,
	( special case. zero len? we never find anything. )
    IFZ, PUSH0, JPNEXT, THEN,
    BC PUSH, ( --> lvl 2, protect )
( Let's do something weird: We'll hold HL by the *tail*.
  Because of our dict structure and because we know our
  lengths, it's easier to compare starting from the end. )
    C A LDrr, B 0 LDri, ( C holds our length )
    BC ADDHLd, HL INCd, ( HL points to after-last-char )
                                                     ( cont . )
( ----- 292 )
    BEGIN, ( loop )
    ( DE is a wordref, first step, do our len correspond? )
        HL PUSH, ( --> lvl 3 )
        DE PUSH, ( --> lvl 4 )
        DE DECd,
        LDA(DE),
        0x7f ANDi, ( remove IMMEDIATE flag )
        C CPr,                                        ( cont. )
( ----- 293 )
        IFZ,
            ( match, let's compare the string then )
            DE DECd, ( Skip prev field. One less because we )
            DE DECd, ( pre-decrement )
            B C LDrr, ( loop C times )
            BEGIN,
                ( pre-decrement for easier Z matching )
                DE DECd,
                HL DECd,
                LDA(DE),
                (HL) CPr,
                JRNZ, BREAK,
            DJNZ, AGAIN,
        THEN,
                                                      ( cont. )
( ----- 294 )
( At this point, Z is set if we have a match. In all cases,
  we want to pop HL and DE )
        DE POP, ( <-- lvl 4 )
        IFZ, ( match, we're done! )
            HL POP, BC POP, HL POP, ( <-- lvl 1-3 ) DE PUSH,
            PUSH1, JPNEXT,
        THEN,
        ( no match, go to prev and continue )
        DE DECd, DE DECd, DE DECd, ( prev field )
        DE PUSH, ( --> lvl 4 )
        EXDEHL,
        LDDE(HL),


                                                      ( cont. )
( ----- 295 )
        ( DE contains prev offset )
        HL POP, ( <-- lvl 4, prev field )
        DEZ, IFNZ, ( offset not zero )
            ( get absolute addr from offset )
            ( carry cleared from "or e" )
            DE SBCHLd,
            EXDEHL, ( result in DE )
        THEN,
        HL POP, ( <-- lvl 3 )
    JRNZ, AGAIN, ( loop-B292, try to match again )
    BC POP, ( <-- lvl 2 )
    ( Z set? end of dict, not found. "w" already on PSP TOS )
    PUSH0,
;CODE
( ----- 297 )
CODE (br)
L1 BSET ( used in ?br and loop )
    LDA(BC), H 0 LDri, L A LDrr,
    RLA, IFC, H DECr, THEN,
    BC ADDHLd, B H LDrr, C L LDrr,
;CODE
CODE (?br)
    HL POP,
    HLZ,
    JRZ, L1 BWR ( br + 1. False, branch )
    ( True, skip next byte and don't branch )
    BC INCd,
;CODE
( ----- 298 )
CODE (loop)
    0 IX+ INC(IXY+), IFZ, 1 IX+ INC(IXY+), THEN, ( I++ )
    ( Jump if I <> I' )
    A 0 IX+ LDrIXY, 2 IX- CP(IXY+), JRNZ, L1 BWR ( branch )
    A 1 IX+ LDrIXY, 1 IX- CP(IXY+), JRNZ, L1 BWR ( branch )
    ( don't branch )
    IX DECd, IX DECd, IX DECd, IX DECd,
    BC INCd,
;CODE
( ----- 305 )
CODE EXECUTE
    DE POP,
    chkPS,
    lblexec @ JP,

CODE EXIT
    C 0 IX+ LDrIXY,
    B 1 IX+ LDrIXY,
    IX DECd, IX DECd,
    JPNEXT,
( ----- 306 )
CODE (n) ( number literal )
  ( Literal value to push to stack is next to (n) reference
    in the atom list. That is where IP is currently pointing.
    Read, push, then advance IP. )
    LDA(BC), L A LDrr, BC INCd,
    LDA(BC), H A LDrr, BC INCd,
    HL PUSH,
;CODE
( ----- 307 )
CODE (s) ( string literal )
( Like (n) but instead of being followed by a 2 bytes
  number, it's followed by a string. When called, puts the
  string's address on PS )
    BC PUSH,
    LDA(BC), C ADDr,
    IFC, B INCr, THEN,
    C A LDrr,
    BC INCd,
;CODE
( ----- 308 )
CODE ROT ( a b c -- b c a )
    HL POP, ( C ) DE POP, ( B ) IY POP, ( A ) chkPS,
    DE PUSH, ( B ) HL PUSH, ( C ) IY PUSH, ( A )
;CODE
CODE ROT> ( a b c -- c a b )
    HL POP, ( C ) DE POP, ( B ) IY POP, ( A ) chkPS,
    HL PUSH, ( C ) IY PUSH, ( A ) DE PUSH, ( B )
;CODE
CODE DUP ( a -- a a )
    HL POP, chkPS,
    HL PUSH, HL PUSH,
;CODE
CODE ?DUP
    HL POP, chkPS, HL PUSH,
    HLZ, IFNZ, HL PUSH, THEN,
;CODE
( ----- 309 )
CODE DROP ( a -- )
    HL POP, chkPS,
;CODE
CODE SWAP ( a b -- b a )
    HL POP, ( B ) DE POP, ( A )
    chkPS,
    HL PUSH, ( B ) DE PUSH, ( A )
;CODE
CODE OVER ( a b -- a b a )
    HL POP, ( B ) DE POP, ( A )
    chkPS,
    DE PUSH, ( A ) HL PUSH, ( B ) DE PUSH, ( A )
;CODE
( ----- 310 )
CODE PICK EXX, ( protect BC )
    HL POP,
    ( x2 )
    L SLA, H RL,
    SP ADDHLd,
    C (HL) LDrr,
    HL INCd,
    B (HL) LDrr,
    ( check PS range before returning )
    EXDEHL,
    HL PS_ADDR LDdi,
    DE SUBHLd,
    IFC, EXX, lbluflw @ JP, THEN,
    BC PUSH,
EXX, ( unprotect BC ) ;CODE
( ----- 311 )
( Low-level part of ROLL. Example:
  "1 2 3 4 4 (roll)" --> "1 3 4 4". No sanity checks, never
  call with 0. )
CODE (roll)
    HL POP,
    B H LDrr,
    C L LDrr,
    SP ADDHLd,
    HL INCd,
    D H LDrr,
    E L LDrr,
    HL DECd,
    HL DECd,
    LDDR,
;CODE
( ----- 312 )
CODE 2DROP ( a b -- )
    HL POP, HL POP, chkPS,
;CODE

CODE 2DUP ( a b -- a b a b )
    HL POP, ( b ) DE POP, ( a )
    chkPS,
    DE PUSH, HL PUSH,
    DE PUSH, HL PUSH,
;CODE
( ----- 313 )
CODE S0
    HL PS_ADDR LDdi,
    HL PUSH,
;CODE

CODE 'S
    HL 0 LDdi,
    SP ADDHLd,
    HL PUSH,
;CODE
( ----- 314 )
CODE AND
    HL POP,
    DE POP,
    chkPS,
    A E LDrr,
    L ANDr,
    L A LDrr,
    A D LDrr,
    H ANDr,
    H A LDrr,
    HL PUSH,
;CODE
( ----- 315 )
CODE OR
    HL POP,
    DE POP,
    chkPS,
    A E LDrr,
    L ORr,
    L A LDrr,
    A D LDrr,
    H ORr,
    H A LDrr,
    HL PUSH,
;CODE
( ----- 316 )
CODE XOR
    HL POP,
    DE POP,
    chkPS,
    A E LDrr,
    L XORr,
    L A LDrr,
    A D LDrr,
    H XORr,
    H A LDrr,
    HL PUSH,
;CODE
( ----- 317 )
CODE NOT
    HL POP,
    chkPS,
    HLZ,
    PUSHZ,
;CODE
( ----- 318 )
CODE +
    HL POP,
    DE POP,
    chkPS,
    DE ADDHLd,
    HL PUSH,
;CODE

CODE -
    DE POP,
    HL POP,
    chkPS,
    DE SUBHLd,
    HL PUSH,
;CODE
( ----- 319 )
CODE * EXX, ( protect BC )
    ( DE * BC -> DE (high) and HL (low) )
    DE POP, BC POP, chkPS,
    HL 0 LDdi,
    A 0x10 LDri,
    BEGIN,
        HL ADDHLd,
        E RL, D RL,
        IFC,
            BC ADDHLd,
            IFC, DE INCd, THEN,
        THEN,
        A DECr,
    JRNZ, AGAIN,
    HL PUSH,
EXX, ( unprotect BC ) ;CODE
( ----- 320 )
( Borrowed from http://wikiti.brandonw.net/ )
( Divides AC by DE and places the quotient in AC and the
  remainder in HL )
CODE /MOD EXX, ( protect BC )
    DE POP, BC POP, chkPS,
    A B LDrr, B 16 LDri,
    HL 0 LDdi,
    BEGIN,
        SCF, C RL, RLA,
        HL ADCHLd, DE SBCHLd,
        IFC, DE ADDHLd, C DECr, THEN,
    DJNZ, AGAIN,
    B A LDrr,
    HL PUSH, BC PUSH,
EXX, ( unprotect BC ) ;CODE
( ----- 321 )
( The word below is designed to wait the proper 100us per tick
  at 500kHz when tickfactor is 1. If the CPU runs faster,
  tickfactor has to be adjusted accordingly. "t" in comments
  below means "T-cycle", which at 500kHz is worth 2us. )
CODE TICKS
    HL POP, chkPS,
    ( we pre-dec to compensate for initialization )
    BEGIN,
        HL DECd, ( 6t )
        IFZ, ( 12t ) JPNEXT, THEN,
        A tickfactor @ LDri, ( 7t )
        BEGIN, A DECr, ( 4t ) JRNZ, ( 12t ) AGAIN,
    JR, ( 12t ) AGAIN, ( outer: 37t inner: 16t )
( ----- 322 )
CODE !
    HL POP, DE POP, chkPS,
    LD(HL)E*, HL INCd,
    E D LDrr, LD(HL)E*,
;CODE
CODE @
    HL POP, chkPS,
    E (HL) LDrr,
    HL INCd,
    D (HL) LDrr,
    DE PUSH,
;CODE
( ----- 323 )
CODE C!
    HL POP, DE POP, chkPS,
    LD(HL)E*, ;CODE
CODE C@
    HL POP, chkPS,
    L (HL) LDrr,
    H 0 LDri, HL PUSH, ;CODE
CODE ~C!
    HL POP, chkPS,
    SYSVARS 0x3f + LD(i)HL,
    HLZ, ( makes A zero if Z is set ) IFNZ,
        A 0xc3 ( JP ) LDri, THEN,
    ( A is either 0 or c3 ) SYSVARS 0x3e + LD(i)A,
;CODE
( ----- 324 )
CODE PC! EXX, ( protect BC )
    BC POP, HL POP, chkPS,
    L OUT(C)r,
EXX, ( unprotect BC ) ;CODE

CODE PC@ EXX, ( protect BC )
    BC POP, chkPS,
    H 0 LDri,
    L INr(C),
    HL PUSH,
EXX, ( unprotect BC ) ;CODE
( ----- 325 )
CODE I
    L 0 IX+ LDrIXY, H 1 IX+ LDrIXY,
    HL PUSH,
;CODE
CODE I'
    L 2 IX- LDrIXY, H 1 IX- LDrIXY,
    HL PUSH,
;CODE
CODE J
    L 4 IX- LDrIXY, H 3 IX- LDrIXY,
    HL PUSH,
;CODE
CODE >R
    HL POP, chkPS,
    IX INCd, IX INCd, 0 IX+ L LDIXYr, 1 IX+ H LDIXYr,
;CODE
( ----- 326 )
CODE R>
    L 0 IX+ LDrIXY, H 1 IX+ LDrIXY, IX DECd, IX DECd, HL PUSH,
;CODE
CODE 2>R
    DE POP, HL POP, chkPS,
    IX INCd, IX INCd, 0 IX+ L LDIXYr, 1 IX+ H LDIXYr,
    IX INCd, IX INCd, 0 IX+ E LDIXYr, 1 IX+ D LDIXYr,
;CODE
CODE 2R>
    L 0 IX+ LDrIXY, H 1 IX+ LDrIXY, IX DECd, IX DECd,
    E 0 IX+ LDrIXY, D 1 IX+ LDrIXY, IX DECd, IX DECd,
    DE PUSH, HL PUSH,
;CODE
( ----- 327 )
CODE BYE
    HALT,
;CODE

CODE (resSP)
    SP PS_ADDR LDdi,
;CODE

CODE (resRS)
    IX RS_ADDR LDdi,
;CODE
( ----- 328 )
CODE S= EXX, ( protect BC )
    DE POP, HL POP, chkPS,
    LDA(DE),
    (HL) CPr,
    IFZ, ( same size? )
        B A LDrr, ( loop A times )
        BEGIN,
            HL INCd, DE INCd,
            LDA(DE),
            (HL) CPr,
            JRNZ, BREAK, ( not equal? break early. NZ is set. )
        DJNZ, AGAIN,
    THEN,
    PUSHZ,
EXX, ( unprotect BC ) ;CODE
( ----- 329 )
CODE CMP
    HL  POP,
    DE  POP,
    chkPS,
    DE SUBHLd,
    DE 0 LDdi,
    IFNZ, ( < or > )
        DE INCd,
        IFNC, ( < )
            DE DECd,
            DE DECd,
        THEN,
    THEN,
    DE PUSH,
;CODE
( ----- 331 )
CODE (im1)
    IM1,
    EI,
;CODE

CODE 0 PUSH0, ;CODE
CODE 1 PUSH1, ;CODE

CODE -1
    HL -1 LDdi,
    HL PUSH,
;CODE
( ----- 332 )
CODE 1+
    HL POP,
    chkPS,
    HL INCd,
    HL PUSH,
;CODE

CODE 1-
    HL POP,
    chkPS,
    HL DECd,
    HL PUSH,
;CODE
( ----- 333 )
CODE 2+
    HL POP,
    chkPS,
    HL INCd,
    HL INCd,
    HL PUSH,
;CODE

CODE 2-
    HL POP,
    chkPS,
    HL DECd,
    HL DECd,
    HL PUSH,
;CODE
( ----- 334 )
CODE RSHIFT ( n u -- n )
    DE POP, ( u ) HL POP, ( n ) chkPS,
    A E LDrr,
    A ORr, IFNZ,
        BEGIN, H SRL, L RR, A DECr, JRNZ, AGAIN,
    THEN,
    HL PUSH, ;CODE
CODE LSHIFT ( n u -- n )
    DE POP, ( u ) HL POP, ( n ) chkPS,
    A E LDrr,
    A ORr, IFNZ,
        BEGIN, L SLA, H RL, A DECr, JRNZ, AGAIN,
    THEN,
    HL PUSH, ;CODE
( ----- 335 )
CODE |L ( n -- msb lsb )
    HL POP, chkPS,
    D 0 LDri, E H LDrr, DE PUSH,
    E L LDrr, DE PUSH, ;CODE
CODE |M ( n -- lsb msb )
    HL POP, chkPS,
    D 0 LDri, E L LDrr, DE PUSH,
    E H LDrr, DE PUSH, ;CODE
( ----- 350 )
Core words

This section contains arch-independent core words of Collapse
OS. Those words are written in a way that make them entirely
cross-compilable (see B260). When building Collapse OS, these
words come right after the boot binary (B280).

Because this unit is designed to be cross-compiled, things are
a little weird. It is compiling in the context of a full
Forth interpreter with all bells and whistles (and z80
assembler), but it has to obey strict rules:

1. Although it cannot compile a word that isn't defined yet,
   it can still execute an immediate from the host system.

                                                        (cont.)
( ----- 351 )
2. Immediate words that have been cross compiled *cannot* be
   used. Only immediates from the host system can be used.
3. If an immediate word compiles words, it can only be words
   that are part of the stable ABI.

All of this is because when cross compiling, all atom ref-
erences are offsetted to the target system and are thus
unusable directly. For the same reason, any reference to a word
in the host system will obviously be wrong in the target
system. More details in B260.





                                                        (cont.)
( ----- 352 )
This unit is loaded in two "low" and "high" parts. The low part
is the biggest chunk and has the most definitions. The high
part is the "sensitive" chunk and contains "LITN", ":" and ";"
definitions which, once defined, kind of make any more defs
impossible.

The gap between these 2 parts is the ideal place to put device
driver code. Load the low part with "353 LOAD", the high part
with "390 LOAD"
( ----- 353 )
: RAM+ [ SYSVARS LITN ] + ; : BIN+ [ BIN( @ LITN ] + ;
: HERE 0x04 RAM+ ; : ~C!ERR 0x41 RAM+ ;
: CURRENT* 0x51 RAM+ ; : CURRENT CURRENT* @ ;
: H@ HERE @ ;
: FIND ( w -- a f ) CURRENT @ SWAP _find ;
: IN> 0x30 RAM+ ; ( current position in INBUF )
: IN( 0x60 RAM+ ; ( points to INBUF )
: IN$ 0 IN( DUP IN> ! ! ; ( flush input buffer )
: C<* 0x0c RAM+  ;
: QUIT (resRS) 0 C<* ! IN$ LIT" (main)" FIND DROP EXECUTE ;
1 25 LOADR+
( ----- 354 )
: ABORT (resSP) QUIT ;
: = CMP NOT ; : < CMP -1 = ; : > CMP 1 = ;
: 0< 32767 > ; : >= < NOT ; : <= > NOT ; : 0>= 0< NOT ;
: >< ( n l h -- f ) 2 PICK > ( n l f ) ROT> > AND ;
: =><= 2 PICK >= ( n l f ) ROT> >= AND ;
: NIP SWAP DROP ; : TUCK SWAP OVER ;
: -^ SWAP - ;
: C@+ ( a -- a+1 c ) DUP C@ SWAP 1+ SWAP ;
: C!+ ( c a -- a+1 ) TUCK C! 1+ ;
: C@- ( a -- a-1 c ) DUP C@ SWAP 1- SWAP ;
: C!- ( c a -- a-1 ) TUCK C! 1- ;
: LEAVE R> R> DROP I 1- >R >R ; : UNLOOP R> 2R> 2DROP >R ;
( ----- 355 )
: +! TUCK @ + SWAP ! ;
: *! ( addr alias -- ) 1+ ! ;
: **! ( addr ialias -- ) 1+ @ ! ;
: / /MOD NIP ;
: MOD /MOD DROP ;
: ALLOT HERE +! ;
: FILL ( a n b -- )
    ROT> OVER ( b a n a ) + SWAP ( b a+n a ) DO ( b )
        DUP I C! LOOP DROP ;
: ALLOT0 ( n -- ) H@ OVER 0 FILL ALLOT ;
( ----- 356 )
SYSVARS 0x53 + :** EMIT
: STYPE C@+ ( a len ) 0 DO C@+ EMIT LOOP DROP ;
: EOT 0x4 ; : BS 0x8 ; : LF 0xa ; : CR 0xd ; : SPC 0x20 ;
: SPC> SPC EMIT ;
: NL> 0x50 RAM+ C@ ?DUP IF EMIT ELSE 13 EMIT 10 EMIT THEN ;
: ERR STYPE ABORT ;
: (uflw) LIT" stack underflow" ERR ;
XCURRENT @ _xapply ORG @ 0x06 ( stable ABI uflw ) + !
: (oflw) LIT" stack overflow" ERR ;
XCURRENT @ _xapply ORG @ 0x13 ( stable ABI oflw ) + !
: (wnf) STYPE LIT"  word not found" ERR ;
( ----- 357 )
( r c -- r f )
( Parse digit c and accumulate into result r.
  Flag f is true when c was a valid digit )
: _pdacc
    '0' - DUP 10 < IF ( good, add to running result )
        SWAP 10 * + 1 ( r*10+n f )
        ELSE ( bad ) DROP 0 THEN ;
( ----- 358 )
: _pd ( a -- n f, parse decimal )
    C@+ OVER C@ 0 ( a len firstchar startat )
( if we have '-', we only advance. more processing later. )
    SWAP '-' = IF 1+ THEN ( a len startat )
( if we can do the whole string, success. if _pdacc returns
  false before, failure. )
    0 ROT> ( len ) ( startat ) DO ( a r )
        OVER I + C@ ( a r c ) _pdacc ( a r f )
        NOT IF DROP 1- 0 UNLOOP EXIT THEN LOOP ( a r )
( if we had '-', we need to invert result. )
    SWAP C@ '-' = IF 0 -^ THEN 1 ( r 1 ) ;
( ----- 359 )
( strings being sent to parse routines are always null
  terminated )

: _pc ( a -- n f, parse character )
    ( apostrophe is ASCII 39 )
    DUP 1+ C@ 39 = OVER 3 + C@ 39 = AND  ( a f )
    NOT IF 0 EXIT THEN   ( a 0 )
    ( surrounded by apos, good, return )
    2+ C@ 1                             ( n 1 )
;
( ----- 360 )
( returns negative value on error )
: _            ( c -- n )
    DUP '0' '9' =><= IF '0' - EXIT THEN
    DUP 'a' 'f' =><= IF 0x57 ( 'a' - 10 ) - EXIT THEN
    DROP -1 ( bad )
;
: _ph          ( a -- n f, parse hex )
    ( '0': ASCII 0x30 'x': 0x78 0x7830 )
    DUP 1+ @ 0x7830 = NOT IF 0 EXIT THEN ( a 0 )
    ( We have "0x" prefix )
    DUP C@ ( a len )
    0 SWAP 1+ ( len+1 ) 3 DO ( a r )
        OVER I + C@ ( a r c ) _ ( a r n )
        DUP 0< IF 2DROP 0 UNLOOP EXIT THEN
        SWAP 4 LSHIFT + ( a r*16+n ) LOOP
    NIP 1 ;
( ----- 361 )
: _pb          ( a -- n f, parse binary )
    ( '0': ASCII 0x30 'b': 0x62 0x6230 )
    DUP 1+ @ 0x6230 = NOT IF 0 EXIT THEN ( a 0 )
    ( We have "0b" prefix )
    DUP C@ ( a len )
    0 SWAP 1+ ( len+1 ) 3 DO ( a r )
        OVER I + C@ ( a r c )
        DUP '0' '1' =><= NOT IF 2DROP 0 UNLOOP EXIT THEN
        '0' - SWAP 1 LSHIFT + ( a r*2+n ) LOOP
    NIP 1 ;
: (parse) ( a -- n )
    _pc IF EXIT THEN
    _ph IF EXIT THEN
    _pb IF EXIT THEN
    _pd IF EXIT THEN
    ( nothing works ) (wnf) ;
( ----- 362 )
: EOT? EOT = ;
SYSVARS 0x55 + :** KEY?
: KEY BEGIN KEY? UNTIL ;
( del is same as backspace )
: BS? DUP 0x7f = SWAP BS = OR ;
: RDLN ( Read 1 line in input buff and make IN> point to it )
    IN$ BEGIN
    ( buffer overflow? same as if we typed a newline )
    IN> @ IN( - 0x3e = IF CR ELSE KEY THEN ( c )
    DUP BS? IF
        IN> @ IN( > IF -1 IN> +! BS EMIT THEN SPC> BS EMIT
    ELSE DUP LF = IF DROP CR THEN ( same as CR )
        DUP SPC >= IF DUP EMIT ( echo back ) THEN
        DUP IN> @ ! 1 IN> +! THEN ( c )
    DUP CR = SWAP EOT? OR UNTIL NL> IN( IN> ! ;
( ----- 363 )
: RDLN<
    IN> @ C@ ( c )
    DUP IF ( not EOL? good, inc and return )
        1 IN> +!
    ELSE ( EOL ? readline. we still return null though )
        RDLN
    THEN ( c )
    ( update C<? flag )
    IN> @ C@ 0 > 0x06 RAM+ !  ( 06 == C<? ) ;
( ----- 364 )
: C<? 0x06 RAM+ @ ;
: C< C<* @ ?DUP NOT IF RDLN< ELSE EXECUTE THEN ;
: , H@ ! H@ 2+ HERE ! ;
: C, H@ C!+ HERE ! ;
: ,"
    BEGIN
        C< DUP 34 ( ASCII " ) = IF DROP EXIT THEN C,
    AGAIN ;
: EOT, EOT C, ;
: WS? SPC <= ;

: TOWORD ( -- c, c being the first letter of the word )
    0 ( dummy ) BEGIN
        DROP C< DUP WS? NOT OVER EOT? OR UNTIL ;
( ----- 365 )
( Read word from C<, copy to WORDBUF, null-terminate, and
  return WORDBUF. )
: _wb 0x0e RAM+ ;
: _eot 0x0401 _wb ! _wb ;
: WORD
    _wb 1+ TOWORD ( a c )
    DUP EOT? IF 2DROP _eot EXIT THEN
    BEGIN
        OVER C! 1+ C< ( a c )
        OVER 0x2e RAM+ = OVER WS? OR
    UNTIL ( a c )
    SWAP _wb - 1- ( ws len ) _wb C!
    EOT? IF _eot ELSE _wb THEN ;
( ----- 366 )
: IMMEDIATE
    CURRENT @ 1-
    DUP C@ 128 OR SWAP C! ;
: IMMED? 1- C@ 0x80 AND ;
: '? WORD FIND ;
: ' '? NOT IF (wnf) THEN ;
: ROLL
    ?DUP NOT IF EXIT THEN
    1+ DUP PICK          ( n val )
    SWAP 2 * (roll)      ( val )
    NIP ;
( ----- 367 )
: MOVE ( a1 a2 u -- )
    ?DUP IF ( u ) 0 DO ( a1 a2 )
        OVER I + C@ ( src dst x )
        OVER I + ( src dst x dst )
        C! ( src dst )
    LOOP THEN 2DROP ;
: MOVE- ( a1 a2 u -- )
    ?DUP IF ( u ) 0 DO ( a1 a2 )
        OVER I' + I - 1- C@ ( src dst x )
        OVER I' + I - 1- ( src dst x dst )
        C! ( src dst )
    LOOP THEN 2DROP ;
: MOVE, ( a u -- ) H@ OVER ALLOT SWAP MOVE ;
( ----- 368 )
: [entry] ( w -- )
    C@+ ( w+1 len ) TUCK MOVE, ( len )
    ( write prev value )
    H@ CURRENT @ - ,
    C, ( write size )
    H@ CURRENT ! ;
: (entry) WORD [entry] ;
: CREATE (entry) 2 ( cellWord ) C, ;
: VARIABLE CREATE 2 ALLOT ;
( ----- 369 )
: FORGET
    ' DUP ( w w )
    ( HERE must be at the end of prev's word, that is, at the
      beginning of w. )
    DUP 1- C@ ( name len field )
    0x7f AND  ( remove IMMEDIATE flag )
    3 +       ( fixed header len )
    - HERE !  ( w )
    ( get prev addr ) 3 - DUP @ - CURRENT ! ;
: EMPTY LIT" _sys" FIND IF DUP HERE ! CURRENT ! THEN ;
( ----- 370 )
: DOES>
    ( Overwrite cellWord in CURRENT )
    3 ( does ) CURRENT @ C!
    ( When we have a DOES>, we forcefully place HERE to 4
      bytes after CURRENT. This allows a DOES word to use ","
      and "C," without messing everything up. )
    CURRENT @ 3 + HERE !
    ( HERE points to where we should write R> )
    R> ,
    ( We're done. Because we've popped RS, we'll exit parent
      definition ) ;
: CONSTANT CREATE , DOES> @ ;
: [IF]
    IF EXIT THEN
    LIT" [THEN]" BEGIN DUP WORD S= UNTIL DROP ;
: [THEN] ;
( ----- 371 )
( n -- Fetches block n and write it to BLK( )
SYSVARS 0x34 + :** BLK@*
( n -- Write back BLK( to storage at block n )
SYSVARS 0x36 + :** BLK!*
( Current blk pointer in ( )
: BLK> 0x38 RAM+ ;
( Whether buffer is dirty )
: BLKDTY 0x3a RAM+ ;
: BLK( 0x3c RAM+ @ ;
: BLK) BLK( 1024 + ;
: BLK$
    H@ 0x3c ( BLK(* ) RAM+ !
    1024 ALLOT
    ( LOAD detects end of block with ASCII EOT. This is why
      we write it there. )
    EOT, 0 BLKDTY ! -1 BLK> ! ;
( ----- 372 )
: BLK! ( -- ) BLK> @ BLK!* 0 BLKDTY ! ;
: FLUSH BLKDTY @ IF BLK! THEN -1 BLK> ! ;
: BLK@ ( n -- )
    DUP BLK> @ = IF DROP EXIT THEN
    FLUSH DUP BLK> ! BLK@* ;
: BLK!! 1 BLKDTY ! ;
: WIPE BLK( 1024 0 FILL BLK!! ;
: WIPED? ( -- f )
    1 ( f ) BLK) BLK( DO
        I C@ IF DROP 0 ( f ) LEAVE THEN LOOP ;
: COPY ( src dst -- )
    FLUSH SWAP BLK@ BLK> ! BLK! ;
( ----- 373 )
: . ( n -- )
    ?DUP NOT IF '0' EMIT EXIT THEN ( 0 is a special case )
    ( handle negative )
    DUP 0< IF '-' EMIT -1 * THEN
    999 SWAP        ( stop indicator )
    BEGIN
        10 /MOD ( r q )
        SWAP '0' + SWAP ( d q )
        ?DUP NOT UNTIL
    BEGIN EMIT DUP '9' > UNTIL DROP ( drop stop ) ;
( ----- 374 )
: ? @ . ;
: _
    DUP 9 > IF 10 - 'a' +
    ELSE '0' + THEN ;
( For hex display, there are no negatives )
: .x
    0xff AND 16 /MOD ( l h )
    _ EMIT _ EMIT ;
: .X |M .x .x ;
( ----- 375 )
: _ ( a -- a+8 )
    DUP ( a a )
    ':' EMIT DUP .x SPC>
    4 0 DO DUP @ |L .x .x SPC> 2+ LOOP
    DROP ( a )
    8 0 DO
        C@+ DUP SPC 0x7e =><= NOT IF DROP '.' THEN EMIT
    LOOP NL> ;
: DUMP ( n a -- )
    SWAP 8 /MOD SWAP IF 1+ THEN
    0 DO _ LOOP ;
( ----- 376 )
: LIST
    BLK@
    16 0 DO
        I 1+ DUP 10 < IF SPC> THEN . SPC>
        64 I * BLK( + DUP 64 + SWAP DO
            I C@ DUP 0x1f > IF EMIT ELSE DROP LEAVE THEN
        LOOP
        NL>
    LOOP ;
( ----- 377 )
: INTERPRET
    BEGIN
    WORD DUP @ 0x0401 = ( EOT ) IF DROP EXIT THEN
    FIND NOT IF (parse) ELSE EXECUTE THEN
    C<? NOT IF SPC> LIT" ok" STYPE NL> THEN
    AGAIN ;
( Read from BOOT C< PTR and inc it. )
: (boot<)
    0x2e ( BOOT C< PTR ) RAM+ @ C@+ ( a+1 c )
    SWAP 0x2e RAM+ ! ( c ) ;
( ----- 378 )
: LOAD
    BLK> @ >R ( save restorable variables to RSP )
    C<* @ >R
    0x06 RAM+ ( C<? ) @ >R 0x2e RAM+ ( boot ptr ) @ >R
    BLK@ BLK( 0x2e RAM+ ! ( Point to beginning of BLK )
    ['] (boot<) 0x0c RAM+ !
    1 0x06 RAM+ !  ( 06 == C<? )
    INTERPRET
    R> 0x2e RAM+ ! R> 0x06 RAM+ !
    R> C<* ! R> BLK@ ;
: LOAD+ BLK> @ + LOAD ;
( b1 b2 -- )
: LOADR 1+ SWAP DO I DUP . SPC> LOAD LOOP ;
: LOADR+ BLK> @ + SWAP BLK> @ + SWAP LOADR ;
( ----- 390 )
( xcomp core high )
: (main) INTERPRET BYE ;
: BOOT
    0x02 RAM+ CURRENT* !
    CURRENT @ 0x2e RAM+ ! ( 2e == BOOT C< PTR )
    0 0x50 RAM+ C! ( NL> )
    ['] (emit) ['] EMIT **! ['] (key?) ['] KEY? **!
    ['] (boot<) C<* !
    ( boot< always has a char waiting. 06 == C<?* )
    1 0x06 RAM+ ! INTERPRET
    0 C<* ! IN$
    LIT" _sys" [entry]
    LIT" Collapse OS" STYPE NL> (main) ;
XCURRENT @ _xapply ORG @ 0x04 ( stable ABI BOOT ) + !
1 4 LOADR+
( ----- 391 )
( Now we have "as late as possible" stuff. See bootstrap doc. )
: :* ( addr -- ) (entry) 4 ( alias ) C, , ;
: :** ( addr -- ) (entry) 5 ( ialias ) C, , ;
( ----- 392 )
: _bchk DUP 0x7f + 0xff > IF LIT" br ovfl" STYPE ABORT THEN ;
: DO COMPILE 2>R H@ ; IMMEDIATE
: LOOP COMPILE (loop) H@ - _bchk C, ; IMMEDIATE
( LEAVE is implemented in low xcomp )
: LITN COMPILE (n) , ;
( gets its name at the very end. can't comment afterwards )
: _ BEGIN LIT" )" WORD S= UNTIL ; IMMEDIATE
: _ ( : will get its name almost at the very end )
    (entry) 1 ( compiled ) C,
    BEGIN
        WORD DUP LIT" ;" S= IF DROP COMPILE EXIT EXIT THEN
        FIND IF ( is word ) DUP IMMED? IF EXECUTE ELSE , THEN
        ELSE ( maybe number ) (parse) LITN THEN
    AGAIN ;
( ----- 393 )
: IF ( -- a | a: br cell addr )
    COMPILE (?br) H@ 1 ALLOT ( br cell allot )
; IMMEDIATE
: THEN ( a -- | a: br cell addr )
    DUP H@ -^ _bchk SWAP ( a-H a ) C!
; IMMEDIATE
: ELSE ( a1 -- a2 | a1: IF cell a2: ELSE cell )
    COMPILE (br)
    1 ALLOT
    [COMPILE] THEN
    H@ 1- ( push a. 1- for allot offset )
; IMMEDIATE
: LIT"
    COMPILE (s) H@ 0 C, ,"
    DUP H@ -^ 1- ( a len ) SWAP C!
; IMMEDIATE
( ----- 394 )
( We don't use ." and ABORT in core, they're not xcomp-ed )
: ." [COMPILE] LIT" COMPILE STYPE ; IMMEDIATE
: ABORT" [COMPILE] ." COMPILE ABORT ; IMMEDIATE
: BEGIN H@ ; IMMEDIATE
: AGAIN COMPILE (br) H@ - _bchk C, ; IMMEDIATE
: UNTIL COMPILE (?br) H@ - _bchk C, ; IMMEDIATE
: [ INTERPRET ; IMMEDIATE
: ] R> DROP ;
: COMPILE ' LITN ['] , , ; IMMEDIATE
: [COMPILE] ' , ; IMMEDIATE
: ['] ' LITN ; IMMEDIATE
':' X' _ 4 - C! ( give : its name )
'(' X' _ 4 - C!
( ----- 400 )
( Write byte E at addr HL, assumed to be an AT28 EEPROM. After
  that, poll repeatedly that address until writing is complete.
  If last polled value is different than orig, set ~C!ERR )
(entry) ~AT28 ( warning: don't touch D register )
    (HL) E LDrr, A E LDrr, ( orig ) EXAFAF', ( save )
    E (HL) LDrr, ( poll ) BEGIN,
        A (HL) LDrr, ( poll ) E CPr, ( same as old? )
        E A LDrr, ( save old poll, Z preserved )
    JRNZ, AGAIN,
    EXAFAF', ( orig ) E SUBr, ( equal? )
    IFNZ, SYSVARS 0x41 + ( ~C!ERR ) LD(i)A, THEN,
    RET,
( ----- 401 )
Grid subsystem

See doc/grid.txt.

Load range: B402-B403
( ----- 402 )
: XYPOS [ GRID_MEM LITN ] ; : XYMODE [ GRID_MEM LITN ] 2+ ;
'? CURSOR! NIP NOT [IF] : CURSOR! 2DROP ; [THEN]
: XYPOS! COLS LINES * MOD DUP XYPOS @ CURSOR! XYPOS ! ;
: AT-XY ( x y -- ) COLS * + XYPOS! ;
'? NEWLN NIP NOT [IF]
: NEWLN ( ln -- ) COLS * DUP COLS + SWAP DO SPC I CELL! LOOP ;
[THEN]
: _lf XYMODE C@ IF EXIT THEN
    XYPOS @ COLS / 1+ LINES MOD DUP NEWLN
    COLS * XYPOS! ;
: _bs SPC XYPOS @ TUCK CELL! ( pos ) 1- XYPOS! ;
( ----- 403 )
: (emit)
    DUP BS? IF DROP _bs EXIT THEN
    DUP CR = IF DROP _lf EXIT THEN
    DUP SPC < IF DROP EXIT THEN
    XYPOS @ CELL!
    XYPOS @ 1+ DUP COLS MOD IF XYPOS! ELSE DROP _lf THEN ;
: GRID$ 0 XYPOS ! 0 XYMODE C! ;
( ----- 410 )
PS/2 keyboard subsystem

Provides (key?) from a driver providing the PS/2 protocol. That
is, for a driver taking care of providing all key codes emanat-
ing from a PS/2 keyboard, this subsystem takes care of mapping
those keystrokes to ASCII characters. This code is designed to
be cross-compiled and loaded with drivers.

Requires PS2_MEM to be defined.

Load range: 411-414
( ----- 411 )
: PS2_SHIFT [ PS2_MEM LITN ] ;
: PS2$ 0 PS2_SHIFT C! ;

( A list of the values associated with the 0x80 possible scan
codes of the set 2 of the PS/2 keyboard specs. 0 means no
value. That value is a character that can be read in (key?)
No make code in the PS/2 set 2 reaches 0x80. )
CREATE PS2_CODES
( 00 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C,
( 08 ) 0 C, 0 C, 0 C, 0 C, 0 C, 9 C, '`' C, 0 C,
( 10 ) 0 C, 0 C, 0 C, 0 C, 0 C, 'q' C, '1' C, 0 C,
( I don't know why, but the key 2 is sent as 0x1f by 2 of my
  keyboards. Is it a timing problem on the ATtiny? TODO )
( 18 ) 0 C, 0 C, 'z' C, 's' C, 'a' C, 'w' C, '2' C, '2' C,
( 20 ) 0 C, 'c' C, 'x' C, 'd' C, 'e' C, '4' C, '3' C, 0 C,
( 28 ) 0 C, 32 C, 'v' C, 'f' C, 't' C, 'r' C, '5' C, 0 C,
( ----- 412 )
( 30 ) 0 C, 'n' C, 'b' C, 'h' C, 'g' C, 'y' C, '6' C, 0 C,
( 38 ) 0 C, 0 C, 'm' C, 'j' C, 'u' C, '7' C, '8' C, 0 C,
( 40 ) 0 C, ',' C, 'k' C, 'i' C, 'o' C, '0' C, '9' C, 0 C,
( 48 ) 0 C, '.' C, '/' C, 'l' C, ';' C, 'p' C, '-' C, 0 C,
( 50 ) 0 C, 0 C, ''' C, 0 C, '[' C, '=' C, 0 C, 0 C,
( 58 ) 0 C, 0 C, 13 C, ']' C, 0 C, '\' C, 0 C, 0 C,
( 60 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 8 C, 0 C,
( 68 ) 0 C, '1' C, 0 C, '4' C, '7' C, 0 C, 0 C, 0 C,
( 70 ) '0' C, '.' C, '2' C, '5' C, '6' C, '8' C, 27 C, 0 C,
( 78 ) 0 C, 0 C, '3' C, 0 C, 0 C, '9' C, 0 C, 0 C,
( Same values, but shifted )
( 00 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C,
( 08 ) 0 C, 0 C, 0 C, 0 C, 0 C, 9 C, '~' C, 0 C,
( 10 ) 0 C, 0 C, 0 C, 0 C, 0 C, 'Q' C, '!' C, 0 C,
( 18 ) 0 C, 0 C, 'Z' C, 'S' C, 'A' C, 'W' C, '@' C, '@' C,
( 20 ) 0 C, 'C' C, 'X' C, 'D' C, 'E' C, '$' C, '#' C, 0 C,
( ----- 413 )
( 28 ) 0 C, 32 C, 'V' C, 'F' C, 'T' C, 'R' C, '%' C, 0 C,
( 30 ) 0 C, 'N' C, 'B' C, 'H' C, 'G' C, 'Y' C, '^' C, 0 C,
( 38 ) 0 C, 0 C, 'M' C, 'J' C, 'U' C, '&' C, '*' C, 0 C,
( 40 ) 0 C, '<' C, 'K' C, 'I' C, 'O' C, ')' C, '(' C, 0 C,
( 48 ) 0 C, '>' C, '?' C, 'L' C, ':' C, 'P' C, '_' C, 0 C,
( 50 ) 0 C, 0 C, '"' C, 0 C, '{' C, '+' C, 0 C, 0 C,
( 58 ) 0 C, 0 C, 13 C, '}' C, 0 C, '|' C, 0 C, 0 C,
( 60 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 8 C, 0 C,
( 68 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C,
( 70 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 27 C, 0 C,
( 78 ) 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C, 0 C,
( ----- 414 )
: _shift? ( kc -- f ) DUP 0x12 = SWAP 0x59 = OR ;
: (key?) ( -- c? f )
    (ps2kc) DUP NOT IF EXIT THEN ( kc )
    DUP 0xe0 ( extended ) = IF ( ignore ) DROP 0 EXIT THEN
    DUP 0xf0 ( break ) = IF DROP ( )
        ( get next kc and see if it's a shift )
        BEGIN (ps2kc) ?DUP UNTIL ( kc )
        _shift? IF ( drop shift ) 0 PS2_SHIFT C! THEN
        ( whether we had a shift or not, we return the next )
        0 EXIT THEN
    DUP 0x7f > IF DROP 0 EXIT THEN
    DUP _shift? IF DROP 1 PS2_SHIFT C! 0 EXIT THEN
    ( ah, finally, we have a gentle run-of-the-mill KC )
    PS2_CODES PS2_SHIFT C@ IF 0x80 + THEN + C@ ( c, maybe 0 )
    ?DUP ( c? f ) ;
( ----- 418 )
SPI relay driver

This driver is designed for a ad-hoc adapter card that acts as a
SPI relay between the z80 bus and the SPI device. When writing
to SPI_CTL, we expect a bitmask of the device to select, with
0 meaning that everything is de-selected. Reading SPI_CTL
returns 0 if the device is ready or 1 if it's still running an
exchange. Writing to SPI_DATA initiates an exchange.

Provides the SPI relay protocol. Load driver with "419 LOAD".
( ----- 419 )
CODE (spix) ( n -- n )
    HL POP, chkPS, A L LDrr,
    SPI_DATA OUTiA,
    ( wait until xchg is done )
    BEGIN, SPI_CTL INAi, 1 ANDi, JRNZ, AGAIN,
    SPI_DATA INAi,
    L A LDrr,
    HL PUSH,
;CODE
CODE (spie) ( n -- )
    HL POP, chkPS, A L LDrr,
    SPI_CTL OUTiA,
;CODE
( ----- 420 )
SD Card subsystem

Load range: B423-B436

This subsystem is designed for a ad-hoc adapter card that acts
as a SPI relay between the z80 bus and the SD card. It requires
a driver providing the SPI Relay protocol. You need to define
SDC_DEVID to specify which ID will be supplied to (spie).

Through that layer, this driver implements the SDC protocol
allowing it to provide BLK@ and BLK!.
( ----- 423 )
( Computes n into crc c with polynomial 0x1021 )
CODE _crc16  ( c n -- c ) EXX, ( protect BC )
    HL POP, ( n ) DE POP, ( c )
    A L LDrr, D XORr, D A LDrr,
    B 8 LDri,
    BEGIN,
        E SLA, D RL,
        IFC, ( msb is set, apply polynomial )
            A D LDrr, 0x10 XORi, D A LDrr,
            A E LDrr, 0x21 XORi, E A LDrr,
        THEN,
    DJNZ, AGAIN,
    DE PUSH,
EXX, ( unprotect BC ) ;CODE
( ----- 424 )
( -- n )
: _idle 0xff (spix) ;

( -- n )
( spix 0xff until the response is something else than 0xff
  for a maximum of 20 times. Returns 0xff if no response. )
: _wait
    0 ( dummy ) 20 0 DO
        DROP _idle DUP 0xff = NOT IF LEAVE THEN
    LOOP ;
( ----- 425 )
( -- )
( The opposite of sdcWaitResp: we wait until response is 0xff.
  After a successful read or write operation, the card will be
  busy for a while. We need to give it time before interacting
  with it again. Technically, we could continue processing on
  our side while the card it busy, and maybe we will one day,
  but at the moment, I'm having random write errors if I don't
  do this right after a write, so I prefer to stay cautious
  for now. )
: _ready BEGIN _idle 0xff = UNTIL ;
( ----- 426 )
( c n -- c )
( Computes n into crc c with polynomial 0x09
  Note that the result is "left aligned", that is, that 8th
  bit to the "right" is insignificant (will be stop bit). )
: _crc7
    XOR           ( c )
    8 0 DO
        2 *       ( <<1 )
        DUP 255 > IF
            ( MSB was set, apply polynomial )
            0xff AND
            0x12 XOR  ( 0x09 << 1, we apply CRC on high bits )
        THEN
    LOOP
;
( ----- 427 )
( send-and-crc7 )
( n c -- c )
: _s+crc SWAP DUP (spix) DROP _crc7 ;
( ----- 428 )
( cmd arg1 arg2 -- resp )
( Sends a command to the SD card, along with arguments and
  specified CRC fields. (CRC is only needed in initial commands
  though). This does *not* handle CS. You have to
  select/deselect the card outside this routine. )
: _cmd
    _wait DROP ROT    ( a1 a2 cmd )
    0 _s+crc          ( a1 a2 crc )
    ROT |M ROT        ( a2 h l crc )
    _s+crc _s+crc     ( a2 crc )
    SWAP |M ROT       ( h l crc )
    _s+crc _s+crc     ( crc )
    1 OR              ( ensure stop bit )
    (spix) DROP       ( send CRC )
    _wait  ( wait for a valid response... )
;
( ----- 429 )
( cmd arg1 arg2 -- r )
( Send a command that expects a R1 response, handling CS. )
: SDCMDR1 [ SDC_DEVID LITN ] (spie) _cmd 0 (spie) ;

( cmd arg1 arg2 -- r arg1 arg2 )
( Send a command that expects a R7 response, handling CS. A R7
  is a R1 followed by 4 bytes. arg1 contains bytes 0:1, arg2
  has 2:3 )
: SDCMDR7
    [ SDC_DEVID LITN ] (spie)
    _cmd                 ( r )
    _idle 8 LSHIFT _idle +  ( r arg1 )
    _idle 8 LSHIFT _idle +  ( r arg1 arg2 )
    0 (spie)
;
( ----- 430 )
: _err 0 (spie) LIT" SDerr" ERR ;

( Tight definition ahead, pre-comment.

  Initialize a SD card. This should be called at least 1ms
  after the powering up of the card. We begin by waking up the
  SD card. After power up, a SD card has to receive at least
  74 dummy clocks with CS and DI high. We send 80.
  Then send cmd0 for a maximum of 10 times, success is when
  we get 0x01. Then comes the CMD8. We send it with a 0x01aa
  argument and expect a 0x01aa argument back, along with a
  0x01 R1 response. After that, we need to repeatedly run
  CMD55+CMD41 (0x40000000) until the card goes out of idle
  mode, that is, when it stops sending us 0x01 response and
  send us 0x00 instead. Any other response means that
  initialization failed. )
( ----- 431 )
: SDC$
    10 0 DO _idle DROP LOOP
    0 ( dummy ) 10 0 DO  ( r )
        DROP 0x40 0 0 SDCMDR1  ( CMD0 )
        1 = DUP IF LEAVE THEN
    LOOP NOT IF _err THEN
    0x48 0 0x1aa ( CMD8 ) SDCMDR7 ( r arg1 arg2 )
    ( expected 1 0 0x1aa )
    0x1aa = ROT ( arg1 f r ) 1 = AND SWAP ( f&f arg1 )
    NOT ( 0 expected ) AND ( f&f&f ) NOT IF _err THEN
    BEGIN
        0x77 0 0 SDCMDR1  ( CMD55 )
        1 = NOT IF _err THEN
        0x69 0x4000 0 SDCMDR1  ( CMD41 )
        DUP 1 > IF _err THEN
    NOT UNTIL ; ( out of idle mode, success! )
( ----- 432 )
: _  ( dstaddr blkno -- )
    [ SDC_DEVID LITN ] (spie)
    0x51 ( CMD17 ) 0 ROT ( a cmd 0 blkno ) _cmd
    IF _err THEN
    _wait 0xfe = NOT IF _err THEN
    0 SWAP               ( crc a )
    512 0 DO             ( crc a )
        _idle            ( crc a n )
        DUP ROT C!+      ( crc n a+1 )
        ROT> _crc16      ( a+1 crc )
        SWAP             ( crc a+1 )
    LOOP
    DROP                 ( crc1 )
    _idle 8 LSHIFT _idle +  ( crc2 )
    _wait DROP 0 (spie)
    = NOT IF _err THEN ;
( ----- 433 )
: SDC@
    2 * DUP BLK( SWAP ( b a b ) _
    1+ BLK( 512 + SWAP _
;
( ----- 434 )
: _  ( srcaddr blkno -- )
    [ SDC_DEVID LITN ] (spie)
    0x58 ( CMD24 ) 0 ROT ( a cmd 0 blkno ) _cmd
    IF _err THEN
    _idle DROP 0xfe (spix) DROP 0 SWAP ( crc a )
    512 0 DO         ( crc a )
        C@+          ( crc a+1 n )
        ROT OVER     ( a n crc n )
        _crc16       ( a n crc )
        SWAP         ( a crc n )
        (spix) DROP  ( a crc )
        SWAP         ( crc a )
    LOOP
    DROP ( crc ) |M ( lsb msb )
    (spix) DROP (spix) DROP
    _wait DROP 0 (spie) ;
( ----- 435 )
: SDC!
    2 * DUP BLK( SWAP ( b a b ) _
    1+ BLK( 512 + SWAP _
;
( ----- 440 )
8086 boot code

Code in the following blocks assemble into a binary that is
suitable to plug into Core words (B350) to achieve a fully
functional Collapse OS. It is structured in a way that is
very similar to Z80 boot code (B280) and requires the same
constants to be pre-declared.

RESERVED REGISTERS: SP is reserved for PSP, BP is for RSP and
DX is for IP. Whenever you use these registers for another
purpose, be sure to protect their initial value. Like with
Z80, you can use SP freely in native code, but you have to make
sure it goes back to its previous level before next is called.


                                                        (cont.)
( ----- 441 )
PS CHECKS: chkPS, is a bit different than in z80: it is para-
metrizable. The idea is that we always call chkPS, before pop-
ping, telling the expected size of stack. This allows for some
interesting optimization. For example, in SWAP, no need to pop,
chkPS, then push, we can chkPS and then proceed to optimized
swapping in PS.

Load range: B445-B461
( ----- 445 )
VARIABLE lblexec VARIABLE lblnext
H@ ORG !
JMPn, 0 , ( 00, main ) 0 C, ( 03, boot driveno )
0 , ( 04, BOOT )
0 , ( 06, uflw ) 0 , ( 08, LATEST ) 0 , ( unused )
0 C, 0 , ( 0b, EXIT )
0 , 0 , ( unused ) 0 , ( 13, oflw )
0 , 0 , 0 C, ( unused )
JMPn, 0 , ( 1a, next )
( ----- 446 )
( TODO: move these words with other native words. )
H@ 4 + XCURRENT ! ( make next CODE have 0 prev field )
CODE (br) L1 BSET ( used in ?br )
    DI DX MOVxx, AL [DI] MOVr[], AH AH XORrr, CBW,
    DX AX ADDxx,
;CODE
CODE (?br)
    AX POPx, AX AX ORxx, JZ, L1 @ RPCs, ( False, branch )
    ( True, skip next byte and don't branch )
    DX INCx,
;CODE
( ----- 447 )
CODE (loop)
    [BP] 0 INC[w]+, ( I++ )
    ( Jump if I <> I' )
    AX [BP] 0 MOVx[]+, AX [BP] -2 CMPx[]+,
    JNZ, L1 @ RPCs, ( branch )
    ( don't branch )
    BP 4 SUBxi, DX INCx,
;CODE
( ----- 448 )
lblnext BSET PC 0x1d - ORG @ 0x1b + ! ( next )
    ( ovfl check )
    BP SP CMPxx,
    IFNC, ( BP >= SP )
        SP PS_ADDR MOVxI, BP RS_ADDR MOVxI,
        DI 0x13 ( oflw ) MOVxm, JMPs, L1 FWRs ( execute )
    THEN,
    DI DX MOVxx, ( <-- IP ) DX INCx, DX INCx,
    DI [DI] MOVx[], ( wordref )
    ( continue to execute ) L1 FSET
( ----- 449 )
lblexec BSET ( DI -> wordref )
    AL [DI] MOVr[], DI INCx, ( PFA )
    AL AL ORrr, IFZ, DI JMPr, THEN, ( native )
    AL DECr, IFNZ, ( not compiled )
        AL DECr, IFZ, ( cell )
            DI PUSHx, JMPs, lblnext @ RPCs, THEN,
        AL DECr, IFZ, ( does )
            DI PUSHx, DI INCx, DI INCx, DI [DI] MOVx[], THEN,
        ( alias or ialias ) DI [DI] MOVx[],
        AL DECr, IFNZ, ( ialias ) DI [DI] MOVx[], THEN,
        JMPs, lblexec @ RPCs,
    THEN, ( continue to compiled )
    BP INCx, BP INCx, [BP] 0 DX MOV[]+x, ( pushRS )
    DX DI MOVxx, DX INCx, DX INCx, ( --> IP )
    DI [DI] MOVx[], JMPs, lblexec @ RPCs,
( ----- 450 )
lblchkPS BSET ( CX -> expected size )
    AX PS_ADDR MOVxI, AX SP SUBxx, 2 SUBAXI, ( CALL adjust )
    AX CX CMPxx,
    IFNC, ( we're good ) RET, THEN,
    ( underflow ) DI 0x06 MOVxm, JMPs, lblexec @ RPCs,

PC 3 - ORG @ 1+ ! ( main )
    DX POPx, ( boot drive no ) 0x03 DL MOVmr,
    SP PS_ADDR MOVxI, BP RS_ADDR MOVxI,
    DI 0x08 MOVxm, ( LATEST )
( HERE begins at CURRENT )
    SYSVARS 0x4 ( HERE ) + DI MOVmx,
    SYSVARS 0x2 ( CURRENT ) + DI MOVmx,
    DI 0x04 ( BOOT ) MOVxm,
    JMPn, lblexec @ RPCn, ( execute )
( ----- 451 )
( native words )
CODE EXECUTE 1 chkPS,
    DI POPx, JMPn, lblexec @ RPCn,
CODE EXIT
    DX [BP] 0 MOVx[]+, BP DECx, BP DECx, ( popRS )
;CODE
( ----- 452 )
CODE (n) ( number literal )
    DI DX MOVxx, DI [DI] MOVx[], DI PUSHx,
    DX INCx, DX INCx,
;CODE
CODE (s) ( string literal, see B287 )
    DI DX MOVxx, ( IP )
    AH AH XORrr, AL [DI] MOVr[], ( slen )
    DX PUSHx, DX INCx, DX AX ADDxx,
;CODE
( ----- 453 )
CODE >R 1 chkPS,
    BP INCx, BP INCx, [BP] 0 POP[w]+,
;CODE NOP, NOP, NOP,
CODE R>
    [BP] 0 PUSH[w]+, BP DECx, BP DECx,
;CODE
CODE 2>R
    [BP] 4 POP[w]+, [BP] 2 POP[w]+, BP 4 ADDxi,
;CODE
CODE 2R> 2 chkPS,
    [BP] -2 PUSH[w]+, [BP] 0 PUSH[w]+, BP 4 SUBxi,
;CODE
( ----- 454 )
CODE ROT ( a b c -- b c a ) 3 chkPS,
    CX POPx, BX POPx, AX POPx,
    BX PUSHx, CX PUSHx, AX PUSHx, ;CODE
CODE ROT> ( a b c -- c a b ) 3 chkPS,
    CX POPx, BX POPx, AX POPx,
    CX PUSHx, AX PUSHx, BX PUSHx, ;CODE
CODE DUP 1 chkPS, AX POPx, AX PUSHx, AX PUSHx, ;CODE
CODE ?DUP 1 chkPS, AX POPx, AX AX ORxx, AX PUSHx,
    IFNZ, AX PUSHx, THEN, ;CODE
CODE OVER ( a b -- a b a ) 2 chkPS,
    DI SP MOVxx, AX [DI] 2 MOVx[]+, AX PUSHx, ;CODE
CODE PICK
    DI POPx, DI SHLx1, ( x2 )
    CX DI MOVxx, CX 2 ADDxi, CALL, lblchkPS @ RPCn,
    DI SP ADDxx, DI [DI] MOVx[], DI PUSHx,
;CODE
( ----- 455 )
CODE (roll) ( "2 3 4 5 4 --> 2 4 5 5". See B311 )
    CX POPx, CX 2 ADDxi, CALL, lblchkPS @ RPCn, CX 2 SUBxi,
    SI SP MOVxx, SI CX ADDxx,
    DI SI MOVxx, DI 2 ADDxi, STD, REPZ, MOVSB,
;CODE
CODE SWAP AX POPx, BX POPx, AX PUSHx, BX PUSHx, ;CODE
CODE DROP 1 chkPS, AX POPx, ;CODE
CODE 2DROP 2 chkPS, SP 4 ADDxi, ;CODE
CODE 2DUP 2 chkPS,
    AX POPx, BX POPx,
    BX PUSHx, AX PUSHx, BX PUSHx, AX PUSHx,
;CODE
CODE S0 AX PS_ADDR MOVxI, AX PUSHx, ;CODE
CODE 'S SP PUSHx, ;CODE
CODE AND 2 chkPS,
    AX POPx, BX POPx, AX BX ANDxx, AX PUSHx, ;CODE
( ----- 456 )
CODE OR 2 chkPS,
    AX POPx, BX POPx, AX BX ORxx, AX PUSHx, ;CODE
CODE XOR 2 chkPS,
    AX POPx, BX POPx, AX BX XORxx, AX PUSHx, ;CODE
CODE NOT 1 chkPS,
    AX POPx, AX AX ORxx,
    IFNZ, AX -1 MOVxI, THEN, AX INCx, AX PUSHx, ;CODE
CODE + 2 chkPS,
    AX POPx, BX POPx, AX BX ADDxx, AX PUSHx, ;CODE
CODE - 2 chkPS,
    BX POPx, AX POPx, AX BX SUBxx, AX PUSHx, ;CODE
CODE * 2 chkPS,
    AX POPx, BX POPx,
    DX PUSHx, ( protect from MUL ) BX MULx, DX POPx,
    AX PUSHx, ;CODE
( ----- 457 )
CODE /MOD 2 chkPS,
    BX POPx, AX POPx, DX PUSHx, ( protect )
    DX DX XORxx, BX DIVx,
    BX DX MOVxx, DX POPx, ( unprotect )
    BX PUSHx, ( modulo ) AX PUSHx, ( division )
;CODE
CODE ! 2 chkPS, DI POPx, AX POPx, [DI] AX MOV[]x, ;CODE
CODE @ 1 chkPS, DI POPx, AX [DI] MOVx[], AX PUSHx, ;CODE
CODE C! 2 chkPS, DI POPx, AX POPx, [DI] AX MOV[]r, ;CODE
CODE C@ 1 chkPS,
    DI POPx, AH AH XORrr, AL [DI] MOVr[], AX PUSHx, ;CODE
CODE I [BP] 0 PUSH[w]+, ;CODE
CODE I' [BP] -2 PUSH[w]+, ;CODE
CODE J [BP] -4 PUSH[w]+, ;CODE
CODE (resSP) SP PS_ADDR MOVxI, ;CODE
CODE (resRS) BP RS_ADDR MOVxI, ;CODE
( ----- 458 )
CODE BYE HLT, BEGIN, JMPs, AGAIN, ;CODE
CODE S= 2 chkPS,
    SI POPx, DI POPx, CH CH XORrr, CL [SI] MOVr[],
    CL [DI] CMPr[],
    IFZ, ( same size? )
        SI INCx, DI INCx, CLD, REPZ, CMPSB,
    THEN,
    PUSHZ,
;CODE
CODE CMP 2 chkPS,
    BX POPx, AX POPx, CX CX XORxx, AX BX CMPxx,
    IFNZ, ( < or > )
        CX INCx, IFC, ( < ) CX DECx, CX DECx, THEN,
    THEN,
    CX PUSHx,
;CODE
( ----- 459 )
CODE _find ( cur w -- a f ) 2 chkPS,
    SI POPx, ( w ) DI POPx, ( cur )
    CH CH XORrr, CL [SI] MOVr[], ( CX -> strlen )
    SI INCx, ( first char ) AX AX XORxx, ( initial prev )
    BEGIN, ( loop )
        DI AX SUBxx, ( jump to prev wordref )
        AL [DI] -1 MOVr[]+, 0x7f ANDALi, ( strlen )
        CL AL CMPrr, IFZ, ( same len )
            SI PUSHx, DI PUSHx, CX PUSHx, ( --> lvl 3 )
            3 ADDALi, ( header ) AH AH XORrr, DI AX SUBxx,
            CLD, REPZ, CMPSB,
            CX POPx, DI POPx, SI POPx, ( <-- lvl 3 )
            IFZ, DI PUSHx, AX 1 MOVxI, AX PUSHx,
                JMPn, lblnext @ RPCn, THEN,
        THEN,
    DI 3 SUBxi, AX [DI] MOVx[], ( prev ) AX AX ORxx,  ( cont. )
( ----- 460 )
( cont. find ) JNZ, AGAIN, ( loop )
    SI DECx, SI PUSHx, AX AX XORrr, AX PUSHx,
;CODE
CODE 0 AX AX XORxx, AX PUSHx, ;CODE
CODE 1 AX 1 MOVxI, AX PUSHx, ;CODE
CODE -1 AX -1 MOVxI, AX PUSHx, ;CODE
CODE 1+ 1 chkPS, DI SP MOVxx, [DI] INC[w], ;CODE
CODE 1- 1 chkPS, DI SP MOVxx, [DI] DEC[w], ;CODE
CODE 2+ 1 chkPS, DI SP MOVxx, [DI] INC[w], [DI] INC[w], ;CODE
CODE 2- 1 chkPS, DI SP MOVxx, [DI] DEC[w], [DI] DEC[w], ;CODE
CODE RSHIFT ( n u -- n ) 2 chkPS,
    CX POPx, AX POPx, AX SHRxCL, AX PUSHx, ;CODE
CODE LSHIFT ( n u -- n ) 2 chkPS,
    CX POPx, AX POPx, AX SHLxCL, AX PUSHx, ;CODE
( ----- 461 )
( See comment in B321. TODO: test on real hardware. in qemu,
  the resulting delay is more than 10x too long. )
CODE TICKS 1 chkPS, ( n=100us )
    SI DX MOVxx, ( protect IP )
    AX POPx, BX 100 MOVxI, BX MULx,
    CX DX MOVxx, ( high ) DX AX MOVxx, ( low )
    AX 0x8600 MOVxI, ( 86h, WAIT ) 0x15 INT,
    DX SI MOVxx, ( restore IP )
;CODE
CODE |M ( n -- lsb msb ) 1 chkPS,
    CX POPx, AH 0 MOVri,
    AL CL MOVrr, AX PUSHx, AL CH MOVrr, AX PUSHx, ;CODE
CODE |L ( n -- msb lsb ) 1 chkPS,
    CX POPx, AH 0 MOVri,
    AL CH MOVrr, AX PUSHx, AL CL MOVrr, AX PUSHx, ;CODE
( ----- 470 )
( Z80 driver for TMS9918. Implements grid protocol. Requires
TMS_CTLPORT, TMS_DATAPORT and ~FNT from the Font compiler at
B520. Patterns are at addr 0x0000, Names are at 0x3800.
Load range B470-472 )
CODE _ctl ( a -- sends LSB then MSB )
    HL POP, chkPS,
    A L LDrr, TMS_CTLPORT OUTiA,
    A H LDrr, TMS_CTLPORT OUTiA,
;CODE
CODE _data
    HL POP, chkPS,
    A L LDrr, TMS_DATAPORT OUTiA,
;CODE
( ----- 471 )
: _zero ( x -- send 0 _data x times )
    ( x ) 0 DO 0 _data LOOP ;
( Each row in ~FNT is a row of the glyph and there is 7 of
them.  We insert a blank one at the end of those 7. )
: _sfont ( a -- Send font to TMS )
    7 0 DO C@+ _data LOOP DROP
    ( blank row ) 0 _data ;
: _sfont^ ( a -- Send inverted font to TMS )
    7 0 DO C@+ 0xff XOR _data LOOP DROP
    ( blank row ) 0xff _data ;
: CELL! ( c pos )
    0x7800 OR _ctl ( tilenum )
    SPC - ( glyph ) 0x5f MOD _data ;
( ----- 472 )
: CURSOR! ( new old -- )
    DUP 0x3800 OR _ctl [ TMS_DATAPORT LITN ] PC@
    0x7f AND ( new old glyph ) SWAP 0x7800 OR _ctl _data
    DUP 0x3800 OR _ctl [ TMS_DATAPORT LITN ] PC@
    0x80 OR ( new glyph ) SWAP 0x7800 OR _ctl _data ;
: COLS 40 ; : LINES 24 ;
: TMS$
    0x8100 _ctl ( blank screen )
    0x7800 _ctl COLS LINES * _zero
    0x4000 _ctl 0x5f 0 DO ~FNT I 7 * + _sfont LOOP
    0x4400 _ctl 0x5f 0 DO ~FNT I 7 * + _sfont^ LOOP
    0x820e _ctl ( name table 0x3800 )
    0x8400 _ctl ( pattern table 0x0000 )
    0x87f0 _ctl ( colors 0 and 1 )
    0x8000 _ctl 0x81d0 _ctl ( text mode, display on ) ;
( ----- 520 )
Fonts

Fonts are kept in "source" form in the following blocks and
then compiled to binary bitmasks by the following code. In
source form, fonts are a simple sequence of '.' and 'X'. '.'
means empty, 'X' means filled. Glyphs are entered one after the
other, starting at 0x21 and ending at 0x7e. To be space
efficient in blocks, we align glyphs horizontally in the blocks
to fit as many character as we can. For example, a 5x7 font
would mean that we would have 12x2 glyphs per block.

521 Font compiler              530 3x5 font
532 5x7 font                   536 7x7 font
( ----- 521 )
( Converts "dot-X" fonts to binary "glyph rows". One byte for
  each row. In a 5x7 font, each glyph thus use 7 bytes.
  Resulting bytes are aligned to the left of the byte.
  Therefore, for a 5-bit wide char, "X.X.X" translates to
  0b10101000. Left-aligned bytes are easier to work with when
  compositing glyphs. )
( ----- 522 )
: _g ( given a top-left of dot-X in BLK(, spit 5 bin lines )
    5 0 DO
    0 3 0 DO ( a r )
        1 LSHIFT
        OVER J 64 * I + + C@ 'X' = IF 1+ THEN
    LOOP 5 LSHIFT C, LOOP DROP ;
: _l ( a u -- a, spit a line of u glyphs )
    ( u ) 0 DO ( a )
        DUP I 3 * + _g
    LOOP ;
: CPFNT3x5
    0 , 0 , 0 C, ( space char )
    530 BLK@ BLK( 21 _l 320 + 21 _l 320 + 21 _l DROP ( 63 )
    531 BLK@ BLK( 21 _l 320 + 10 _l DROP ( 94! )
;
( ----- 523 )
: _g ( given a top-left of dot-X in BLK(, spit 7 bin lines )
    7 0 DO
    0 5 0 DO ( a r )
        1 LSHIFT
        OVER J 64 * I + + C@ 'X' = IF 1+ THEN
    LOOP 3 LSHIFT C, LOOP DROP ;
: _l ( a u -- a, spit a line of u glyphs )
    ( u ) 0 DO ( a )
        DUP I 5 * + _g
    LOOP ;
: CPFNT5x7
    0 , 0 , 0 , 0 C, ( space char )
    535 532 DO I BLK@ BLK( 12 _l 448 + 12 _l DROP LOOP ( 72 )
    535 BLK@ BLK( 12 _l 448 + 10 _l DROP ( 94! )
;
( ----- 524 )
: _g ( given a top-left of dot-X in BLK(, spit 7 bin lines )
    7 0 DO
    0 7 0 DO ( a r )
        1 LSHIFT
        OVER J 64 * I + + C@ 'X' = IF 1+ THEN
    LOOP 1 LSHIFT C, LOOP DROP ;
: _l ( a u -- a, spit a line of u glyphs )
    ( u ) 0 DO ( a )
        DUP I 7 * + _g
    LOOP ;
: CPFNT7x7
    0 , 0 , 0 , 0 C, ( space char )
    541 536 DO I BLK@ BLK( 9 _l 448 + 9 _l DROP LOOP ( 90 )
    542 BLK@ BLK( 4 _l DROP ( 94! )
;
( ----- 530 )
.X.X.XX.X.XXX...X..X...XX...X...............X.X..X.XX.XX.X.XXXX
.X.X.XXXXXX...XX.X.X..X..X.XXX.X............XX.XXX...X..XX.XX..
.X........XX.X..X.....X..X..X.XXX...XXX....X.X.X.X..X.XX.XXXXX.
......XXXXX.X..X.X....X..X.X.X.X..X.......X..X.X.X.X....X..X..X
.X....X.X.X...X.XX.....XX........X......X.X...X.XXXXXXXX...XXX.
.XXXXXXXXXXX........X...X..XX..X..X.XX..XXXX.XXXXXX.XXX.XXXXXXX
X....XX.XX.X.X..X..X.XXX.X...XXXXX.XX.XX..X.XX..X..X..X.X.X...X
XXX.X.XXXXXX......X.......X.X.XXXXXXXX.X..X.XXX.XX.X.XXXX.X...X
X.XX..X.X..X.X..X..X.XXX.X....X..X.XX.XX..X.XX..X..X.XX.X.X...X
XXXX..XXXXX....X....X...X...X..XXX.XXX..XXXX.XXXX...XXX.XXXXXX.
X.XX..X.XXX.XXXXX.XXXXX..XXXXXX.XX.XX.XX.XX.XXXXXXXX..XXX.X....
XX.X..XXXX.XX.XX.XX.XX.XX...X.X.XX.XX.XX.XX.X..XX..X....XX.X...
X..X..XXXX.XX.XXX.X.XXX..X..X.X.XX.XXXX.X..X..X.X...X...X......
XX.X..X.XX.XX.XX..XXXX.X..X.X.X.XX.XXXXX.X.X.X..X....X..X......
X.XXXXX.XX.XXXXX...XXX.XXX..X.XXX.X.X.XX.X.X.XXXXXX..XXXX...XXX
!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_
( ----- 531 )
X.....X.......X....XX...X...X...XX..XX.......................X.
.X.XX.X...XX..X.X.X...X.X........X.X.X.X.XXX..X.XX..XX.XX.XXXXX
.....XXX.X...XXX.XXX.X.XXX..X...XXX..X.XXXX.XX.XX.XX.XX..XX..X.
...XXXX.XX..X.XXX.X...XXX.X.X...XX.X.X.X.XX.XX.XXX..XXX....X.X.
...XXXXX..XX.XX.XXX..XX.X.X.X.XX.X.X.XXX.XX.X.X.X....XX..XX..XX
...................XX.X.XX.....................................
X.XX.XX.XX.XX.XXXX.X..X..X..XX
X.XX.XX.X.X..X..XXX...X...XXX.
X.XX.XXXX.X..X.XX..X..X..X....
XXX.X.X.XX.X.X.XXX.XX.X.XX....
`abcdefghijklmnopqrstuvwxyz{|}~
( ----- 532 )
..X...X.X........X..............X....X....X.................
..X...X.X..X.X..XXXXX...X.XX....X...X......X.X.X.X..X.......
..X.......XXXXXX.......X.X..X......X........X.XXX...X.......
..X........X.X..XXX...X...XX.......X........XXXXXXXXXXX.....
..........XXXXX....X.X....XX.X.....X........X.XXX...X.......
..X........X.X.XXXX.X...XX..X.......X......X.X.X.X..X.....X.
..X..............X.......XXX.X.......X....X..............X..
................XXX...XX..XXX..XXX...XX.XXXXX.XXX.XXXXX.XXX.
..............XX...X.X.X.X...XX...X.X.X.X....X........XX...X
.............X.X..XX...X.....X....XX..X.XXXX.X........XX...X
XXXXX.......X..X.X.X...X....X...XX.XXXXX....XXXXX....X..XXX.
...........X...XX..X...X...X......X...X.....XX...X..X..X...X
......XX..X....X...X...X..X...X...X...X.X...XX...X.X...X...X
......XX........XXX..XXXXXXXXX.XXX....X..XXX..XXX.X.....XXX.
!"#$%&'()*+,-./012345678
( ----- 533 )
.XXX...............X.....X.....XXX..XXX..XXX.XXXX..XXX.XXXX.
X...X..X....X....XX.......XX..X...XX...XX...XX...XX...XX...X
X...X..X....X...XX..XXXXX..XX.....XX..XXX...XX...XX....X...X
.XXX...........X.............X...X.X..XXXXXXXXXXX.X....X...X
....X..X....X...XX..XXXXX..XX...X..X....X...XX...XX....X...X
....X..X...X.....XX.......XX.......X...XX...XX...XX...XX...X
.XXX...............X.....X......X...XXX.X...XXXXX..XXX.XXXX.
XXXXXXXXXX.XXX.X...X.XXX....XXX..X.X....X...XX...X.XXX.XXXX.
X....X....X...XX...X..X......XX.X..X....XX.XXXX..XX...XX...X
X....X....X....X...X..X......XXX...X....X.X.XXX..XX...XX...X
XXXX.XXXX.X..XXXXXXX..X......XX....X....X...XX.X.XX...XXXXX.
X....X....X...XX...X..X......XXX...X....X...XX..XXX...XX....
X....X....X...XX...X..X..X...XX.X..X....X...XX..XXX...XX....
XXXXXX.....XXX.X...X.XXX..XXX.X..X.XXXXXX...XX...X.XXX.X....
9:;<=>?@ABCDEFGHIJKLMNOP
( ----- 534 )
.XXX.XXXX..XXX.XXXXXX...XX...XX...XX...XX...XXXXXXXXX.......
X...XX...XX...X..X..X...XX...XX...XX...XX...XX...XX....X....
X...XX...XX......X..X...XX...XX...X.X.X..X.X....X.X.....X...
X...XXXXX..XXX...X..X...XX...XX...X..X....X....X..X......X..
X.X.XX.X......X..X..X...XX...XX.X.X.X.X...X...X...X.......X.
X..XXX..X.X...X..X..X...X.X.X.X.X.XX...X..X..X...XX........X
.XXXXX...X.XXX...X...XXX...X...X.X.X...X..X..XXXXXXXX.......
..XXX..X.........X..........................................
....X.X.X.........X.........................................
....XX...X...........XXX.X.....XXX.....X.XXX..XX....XXXX....
....X...................XX....X...X....XX...XX..X..X..XX....
....X................XXXXXXX..X......XXXXXXXXX......XXXXXX..
....X...............X...XX..X.X...X.X..XX....XXX......XX..X.
..XXX.....XXXXX......XXXXXXX...XXX...XXX.XXXXX......XX.X..X.
QRSTUVWXYZ[\]^_`abcdefgh
( ----- 535 )
............................................................
............................................................
..X......XX..X..XX...X.X.XXX...XXX.XXX....XXXX.XX..XXX..X...
..........X.X....X..X.X.XX..X.X...XX..X..X..XXX...X....XXX..
..X......XXX.....X..X...XX...XX...XXXX....XXXX.....XXX..X...
..X...X..XX.X....X..X...XX...XX...XX........XX........X.X...
..X....XX.X..X...XX.X...XX...X.XXX.X........XX.....XXX...XX.
................................XX...X...XX.......
...............................X.....X.....X......
X...XX...XX...XX...XX...XXXXXX.X.....X.....X..X.X.
X...XX...XX...X.X.X..X.X....X.X......X......XX.X..
X...XX...XX...X..X....X....X...X.....X.....X......
X...X.X.X.X.X.X.X.X..X....X....X.....X.....X......
.XXX...X...X.X.X...XX....XXXXX..XX...X...XX.......
ijklmnopqrstuvwxyz{|}~
( ----- 536 )
..XX....XX.XX..XX.XX....XX..XX......XXX......XX.....XX...XX....
..XX....XX.XX..XX.XX..XXXXXXXX..XX.XX.XX....XX.....XX.....XX...
..XX....XX.XX.XXXXXXXXX.X......XX..XX.XX...XX.....XX.......XX..
..XX...........XX.XX..XXXXX...XX....XXX...........XX.......XX..
..XX..........XXXXXXX...X.XX.XX....XX.XX.X........XX.......XX..
...............XX.XX.XXXXXX.XX..XX.XX..XX..........XX.....XX...
..XX...........XX.XX...XX.......XX..XXX.XX..........XX...XX....
...........................................XXXX....XX....XXXX..
..XX.....XX............................XX.XX..XX..XXX...XX..XX.
XXXXXX...XX...........................XX..XX.XXX...XX.......XX.
.XXXX..XXXXXX........XXXXXX..........XX...XXXXXX...XX......XX..
XXXXXX...XX.........................XX....XXX.XX...XX.....XX...
..XX.....XX.....XX............XX...XX.....XX..XX...XX....XX....
...............XX.............XX...........XXXX..XXXXXX.XXXXXX.
!"#$%&'()*+,-./012
( ----- 537 )
.XXXX.....XX..XXXXXX...XXX..XXXXXX..XXXX...XXXX................
XX..XX...XXX..XX......XX........XX.XX..XX.XX..XX...............
....XX..XXXX..XXXXX..XX........XX..XX..XX.XX..XX...XX.....XX...
..XXX..XX.XX......XX.XXXXX....XX....XXXX...XXXXX...XX.....XX...
....XX.XXXXXX.....XX.XX..XX..XX....XX..XX.....XX...............
XX..XX....XX..XX..XX.XX..XX..XX....XX..XX....XX....XX.....XX...
.XXXX.....XX...XXXX...XXXX...XX.....XXXX...XXX.....XX....XX....
...XX.........XX......XXXX...XXXX...XXXX..XXXXX...XXXX..XXXX...
..XX...........XX....XX..XX.XX..XX.XX..XX.XX..XX.XX..XX.XX.XX..
.XX....XXXXXX...XX......XX..XX.XXX.XX..XX.XX..XX.XX.....XX..XX.
XX...............XX....XX...XX.X.X.XXXXXX.XXXXX..XX.....XX..XX.
.XX....XXXXXX...XX.....XX...XX.XXX.XX..XX.XX..XX.XX.....XX..XX.
..XX...........XX...........XX.....XX..XX.XX..XX.XX..XX.XX.XX..
...XX.........XX.......XX....XXXX..XX..XX.XXXXX...XXXX..XXXX...
3456789:;<=>?@ABCD
( ----- 538 )
XXXXXX.XXXXXX..XXXX..XX..XX.XXXXXX..XXXXX.XX..XX.XX.....XX...XX
XX.....XX.....XX..XX.XX..XX...XX......XX..XX.XX..XX.....XXX.XXX
XX.....XX.....XX.....XX..XX...XX......XX..XXXX...XX.....XXXXXXX
XXXXX..XXXXX..XX.XXX.XXXXXX...XX......XX..XXX....XX.....XX.X.XX
XX.....XX.....XX..XX.XX..XX...XX......XX..XXXX...XX.....XX.X.XX
XX.....XX.....XX..XX.XX..XX...XX...XX.XX..XX.XX..XX.....XX...XX
XXXXXX.XX......XXXX..XX..XX.XXXXXX..XXX...XX..XX.XXXXXX.XX...XX
XX..XX..XXXX..XXXXX...XXXX..XXXXX...XXXX..XXXXXX.XX..XX.XX..XX.
XX..XX.XX..XX.XX..XX.XX..XX.XX..XX.XX..XX...XX...XX..XX.XX..XX.
XXX.XX.XX..XX.XX..XX.XX..XX.XX..XX.XX.......XX...XX..XX.XX..XX.
XXXXXX.XX..XX.XXXXX..XX..XX.XXXXX...XXXX....XX...XX..XX.XX..XX.
XX.XXX.XX..XX.XX.....XX.X.X.XX.XX......XX...XX...XX..XX.XX..XX.
XX..XX.XX..XX.XX.....XX.XX..XX..XX.XX..XX...XX...XX..XX..XXXX..
XX..XX..XXXX..XX......XX.XX.XX..XX..XXXX....XX....XXXX....XX...
EFGHIJKLMNOPQRSTUVWXYZ[\]^_
( ----- 539 )
XX...XXXX..XX.XX..XX.XXXXXX.XXXXX.........XXXXX....XX..........
XX...XXXX..XX.XX..XX.....XX.XX.....XX........XX...XXXX.........
XX.X.XX.XXXX..XX..XX....XX..XX......XX.......XX..XX..XX........
XX.X.XX..XX....XXXX....XX...XX.......XX......XX..X....X........
XXXXXXX.XXXX....XX....XX....XX........XX.....XX................
XXX.XXXXX..XX...XX...XX.....XX.........XX....XX................
XX...XXXX..XX...XX...XXXXXX.XXXXX.........XXXXX.........XXXXXXX
.XX...........XX................XX..........XXX.........XX.....
..XX..........XX................XX.........XX.....XXXX..XX.....
...XX...XXXX..XXXXX...XXXX...XXXXX..XXXX...XX....XX..XX.XXXXX..
...........XX.XX..XX.XX..XX.XX..XX.XX..XX.XXXXX..XX..XX.XX..XX.
........XXXXX.XX..XX.XX.....XX..XX.XXXXXX..XX.....XXXXX.XX..XX.
.......XX..XX.XX..XX.XX..XX.XX..XX.XX......XX........XX.XX..XX.
........XXXXX.XXXXX...XXXX...XXXXX..XXXX...XX.....XXX...XX..XX.
WXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
( ----- 540 )
..XX.....XX...XX......XXX......................................
..............XX.......XX......................................
.XXX....XXX...XX..XX...XX....XX.XX.XXXXX...XXXX..XXXXX...XXXXX.
..XX.....XX...XX.XX....XX...XXXXXXXXX..XX.XX..XX.XX..XX.XX..XX.
..XX.....XX...XXXX.....XX...XX.X.XXXX..XX.XX..XX.XX..XX.XX..XX.
..XX.....XX...XX.XX....XX...XX.X.XXXX..XX.XX..XX.XXXXX...XXXXX.
.XXXX..XX.....XX..XX..XXXX..XX...XXXX..XX..XXXX..XX.........XX.
...............XX..............................................
...............XX..............................................
XX.XX...XXXXX.XXXXX..XX..XX.XX..XX.XX...XXXX..XX.XX..XX.XXXXXX.
XXX.XX.XX......XX....XX..XX.XX..XX.XX.X.XX.XXXX..XX..XX....XX..
XX......XXXX...XX....XX..XX.XX..XX.XX.X.XX..XX...XX..XX...XX...
XX.........XX..XX....XX..XX..XXXX..XXXXXXX.XXXX...XXXXX..XX....
XX.....XXXXX....XXX...XXXXX...XX....XX.XX.XX..XX.....XX.XXXXXX.
ijklmnopqrstuvwxyz{|}~
( ----- 541 )
...XX....XX...XX......XX...X
..XX.....XX....XX....XX.X.XX
..XX.....XX....XX....X...XX.
XXX......XX.....XXX.........
..XX.....XX....XX...........
..XX.....XX....XX...........
...XX....XX...XX............
{|}~
