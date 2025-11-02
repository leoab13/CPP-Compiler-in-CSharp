%include "io.inc"
section .text
extern io_print_string
global main
main:
MOV EAX,1
PUSH EAX
MOV EAX,2
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JE etiquetaIF1
MOV EAX,cadPrint1
CALL io_print_string
GET_DEC 4,EAX
MOV [d],EAX
MOV AX,[d]
PUSH EAX
MOV EAX,2
PUSH EAX
POP EBX
POP EAX
MOV EDX,0
DIV EBX
PUSH EDX
MOV EAX,0
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JE etiquetaIF2
MOV EAX,0
PUSH EAX
POP EAX
MOV [i],EAX
for1:
MOV AX,[i]
PUSH EAX
MOV AX,[d]
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JAE FinFor1
MOV EAX,cadPrint2
CALL io_print_string
MOV ECX,[i]
INC ECX
MOV [i],ECX
JMP for1
FinFor1:
MOV EAX,cadPrint3
CALL io_print_string
MOV AX,[d]
PUSH EAX
POP EAX
MOV [i],EAX
etiquetaDo1:
MOV EAX,cadPrint4
CALL io_print_string
MOV EAX, [radio]
PRINT_DEC 4,EAX
MOV AX,[i]
SUB EAX,1
MOV [i],EAX
MOV AX,[i]
PUSH EAX
MOV EAX,0
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JBE etiquetaDo1fin
JMP etiquetaDo1
etiquetaDo1fin:
MOV EAX,cadPrint5
CALL io_print_string
MOV EAX,0
PUSH EAX
POP EAX
MOV [i],EAX
etiquetaWhile1:
MOV AX,[i]
PUSH EAX
MOV AX,[d]
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JAE etiquetaWhile1fin
MOV EAX,0
PUSH EAX
POP EAX
MOV [j],EAX
for2:
MOV AX,[j]
PUSH EAX
MOV AX,[i]
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JA FinFor2
MOV AX,[j]
PUSH EAX
MOV EAX,2
PUSH EAX
POP EBX
POP EAX
MOV EDX,0
DIV EBX
PUSH EDX
MOV EAX,0
PUSH EAX
POP EBX
POP EAX
CMP EAX,EBX
JNE etiquetaIF3
MOV EAX,cadPrint6
CALL io_print_string
JMP finIf3
etiquetaIF3:
MOV EAX,cadPrint7
CALL io_print_string
finIf3:
MOV ECX,[j]
INC ECX
MOV [j],ECX
JMP for2
FinFor2:
MOV EAX,cadPrint8
CALL io_print_string
MOV AX,[i]
ADD EAX,1
MOV [i],EAX
JMP etiquetaWhile1
etiquetaWhile1fin :
JMP finIf2
etiquetaIF2:
MOV EAX,cadPrint9
CALL io_print_string
finIf2:
JMP finIf1
etiquetaIF1:
finIf1:
ret
section .data
cadPrint1 DB '',0Dh,'Ingrese el valor de d = ',0Dh,'',0
cadPrint2 DB '-',0
cadPrint3 DB '',0Dh,'',0
cadPrint4 DB ' - ',0
cadPrint5 DB '',0Dh,'',0
cadPrint6 DB '+',0
cadPrint7 DB '-',0
cadPrint8 DB '',0Dh,'',0
cadPrint9 DB 'El valor de d debe ser impar.',0Dh,'',0
radio : DW 0
altura : DW 0
i : DW 0
j : DW 0
x : DD 0
y : DD 0
z : DD 0
c : DB 0
d : DB 0
e : DB 0
