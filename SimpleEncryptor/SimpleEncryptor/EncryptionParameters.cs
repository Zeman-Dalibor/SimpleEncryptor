namespace SimpleEncryptor
{
    public class EncryptionParameters
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool InPlace { get; set; } = false;
        public string OutputFilePath { get; set; } = string.Empty;
    }
}
