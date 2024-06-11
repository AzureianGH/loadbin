section .data
	hello db 'Hello World!', 0

section .text
	global _start

_start:
	; Write the string to stdout
	mov rax, 1                  ; System call number for write
	mov rdi, 1                  ; File descriptor for stdout
	mov rsi, hello              ; Pointer to the string
	mov rdx, 13                 ; Length of the string
	syscall

	; Exit the program
	mov rax, 60                 ; System call number for exit
	xor rdi, rdi                ; Exit status 0
	syscall

; Compile for 64-bit Linux
; nasm -f elf64 -o main.o main.asm
; ld -o main main.o