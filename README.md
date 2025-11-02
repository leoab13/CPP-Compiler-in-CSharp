# CPP-Compiler-in-CSharp

A small educational C++-to-Assembly compiler written in C#.

This project parses a subset of C/C++ source files and generates NASM-style assembly plus a log of lexical tokens. It was built for learning compiler construction and producing simple NASM-compatible output that uses `io.inc` macros for input/output.

## Repository layout

- `cpp/` - Put C/C++ source files you want the compiler to read (e.g. `suma.cpp`).
- `asm/` - Generated assembly output files are written here (e.g. `suma.asm`).
- `logs/` - Lexer/token logs are written here (e.g. `suma.log`).
- `*.cs` - C# source for the compiler (lexer, parser, code generator).

## How to generate assembly and logs

1. Build the project:

```bash
dotnet build
```

2. Run the compiler:

```bash
dotnet run
```

When the program runs it will prompt:

```
Ingrese el nombre del archivo C++ (por ejemplo suma.cpp):
```

Enter the filename (for example `suma.cpp`). If you enter a base filename without a path the tool will look in the `cpp/` folder. The compiler will create the assembly file under `asm/<base>.asm` and the token log under `logs/<base>.log`.

## Running the generated assembly

The generated assembly uses `io.inc` (the project includes `io.inc` in the assembly via an `%include`), which provides simple macros for input/output used by the generated code.

Recommended ways to run the generated `.asm`:

- SASM (recommended for ease of use):
	- SASM is an IDE that bundles NASM and can assemble and run NASM code that uses `io.inc`. Open the generated `.asm` file in SASM and use the Assemble/Run commands.

- NASM + linker (advanced):
	- You can also assemble and link the file using NASM and your system linker, but details depend on the macros in `io.inc` and target architecture (32-bit vs 64-bit). If you choose this route, make sure to use the correct NASM format (e.g. `-f elf`, `-f elf64` on Linux) and the proper linking flags for your platform. The project-generated assembly expects `io.inc` macros to be available.

Example (Linux, may need adjustments depending on `io.inc` and target):

```bash
# Assemble (example for 64-bit; adjust format if needed)
nasm -f elf64 asm/suma.asm -o suma.o
# Link
ld -o suma suma.o
# Run
./suma
```

If the generated `.asm` targets 32-bit or uses specific macros, adjust the `-f` and linking commands accordingly or prefer opening the file in SASM which handles these details for you.

## Notes and limitations

- This compiler implements a small subset of C/C++ features for educational purposes. It may not support full language semantics.
- The runtime behavior (input/output) depends on `io.inc` and the assembler/linker used.
- The program currently prompts for the input filename; in future you may prefer passing the filename as a command-line argument.

## Contributing

Feel free to open issues or pull requests. Suggestions:

- Add command-line argument support to skip the prompt and pass the source file directly.
- Improve error messages and add more language features.
- Add unit tests and CI to verify generated assembly for small programs.

## License

This repository does not include a license file. Add one if you want to make the project open source with a specific license.

