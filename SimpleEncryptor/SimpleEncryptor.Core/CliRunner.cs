namespace SimpleEncryptor.Core
{
    public static class CliRunner
    {
        public static int Run(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return 1;
            }

            string mode = args[0].ToLowerInvariant();
            string filePath = args[1];
            string password = args[2];

            string? outputPath = null;
            bool inPlace = false;

            for (int i = 3; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--inplace":
                        inPlace = true;
                        break;
                    case "--output" when i + 1 < args.Length:
                        outputPath = args[++i];
                        break;
                }
            }

            if (inPlace && !string.IsNullOrWhiteSpace(outputPath))
            {
                Console.Error.WriteLine("Error: Specify either --inplace or --output <path>, not both.");
                Console.Error.WriteLine();
                PrintUsage();
                return 1;
            }

            if (!inPlace && string.IsNullOrWhiteSpace(outputPath))
            {
                Console.Error.WriteLine("Error: Specify either --inplace or --output <path>.");
                Console.Error.WriteLine();
                PrintUsage();
                return 1;
            }

            var parameters = new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = password,
                InPlace = inPlace,
                OutputFilePath = string.IsNullOrWhiteSpace(outputPath) ? string.Empty : outputPath.Trim()
            };

            if (!File.Exists(parameters.InputFilePath))
            {
                Console.Error.WriteLine($"Error: File not found: {parameters.InputFilePath}");
                return 1;
            }

            var encryptor = new FileEncryptor();

            try
            {
                switch (mode)
                {
                    case "encrypt":
                        encryptor.Encrypt(parameters);
                        Console.WriteLine(inPlace ? $"Encrypted in-place: {filePath}" : $"Encrypted to: {outputPath}");
                        break;
                    case "decrypt":
                        encryptor.Decrypt(parameters);
                        Console.WriteLine(inPlace ? $"Decrypted in-place: {filePath}" : $"Decrypted to: {outputPath}");
                        break;
                    default:
                        Console.Error.WriteLine($"Error: Unknown mode '{mode}'. Use 'encrypt' or 'decrypt'.");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }

            return 0;
        }

        private static void PrintUsage()
        {
            Console.Error.WriteLine("Usage: SimpleEncryptor <encrypt|decrypt> <file> <password> --inplace|--output <path>");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Options:");
            Console.Error.WriteLine("  --inplace        Encrypt/decrypt the file in-place.");
            Console.Error.WriteLine("  --output <path>  Write result to a separate file.");
        }
    }
}
