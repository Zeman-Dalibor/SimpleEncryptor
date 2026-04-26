# SimpleEncryptor

SimpleEncryptor is a small file encryption utility that provides both a
Windows Forms desktop GUI and a command-line interface (CLI). It supports
encrypting files to a separate output file as well as encrypting/decrypting
files in-place.

## Main features

- Encrypt a file to a separate output file.
- Encrypt/decrypt a file in-place (modify the original file).
- Desktop application: Windows Forms GUI for interactive use.
- Command-line interface (CLI) for automation and scripting.

## Desktop (WinForms)

The desktop application is a WinForms app (targeting .NET 6). Run the
`SimpleEncryptor` project to launch the graphical interface.

## Command-line interface (CLI)

The CLI binary is provided by the `SimpleEncryptor.Cli` project. Basic usage:

```
SimpleEncryptor <encrypt|decrypt> <file> <password> --inplace|--output <path>
```

Arguments and options:

- `<encrypt|decrypt>` — Mode: `encrypt` to encrypt, `decrypt` to decrypt.
- `<file>` — Path to the input file to process.
- `<password>` — Password used for encryption/decryption.
- `--inplace` — Modify the input file in-place.
- `--output <path>` — Write the result to the specified output file.

Notes:

- You must specify either `--inplace` or `--output <path>`, not both.
- If the input file does not exist the CLI exits with an error.
- Error messages are written to stderr.

Examples:

Encrypt to a new file:

```
SimpleEncryptor.exe encrypt "C:\data\secret.txt" "MyPassword123" --output "C:\data\secret.txt.enc"
```

Encrypt in-place:

```
SimpleEncryptor.exe encrypt "C:\data\secret.txt" "MyPassword123" --inplace
```

Decrypt to a new file:

```
SimpleEncryptor.exe decrypt "C:\data\secret.txt.enc" "MyPassword123" --output "C:\data\secret.txt"
```

## Building and running

- Run the GUI (WinForms):

```
dotnet run --project SimpleEncryptor/SimpleEncryptor/SimpleEncryptor.csproj
```

- Run the CLI:

```
dotnet run --project SimpleEncryptor/SimpleEncryptor.Cli -- <arguments>
```

## Notes

- Targets .NET 6 for Windows compatibility (Windows 7 support).
- Use strong passwords and manage them securely; this tool does not store passwords.
