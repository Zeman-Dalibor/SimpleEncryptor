namespace SimpleEncryptor.Core
{
    public class EncryptionParameters
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool InPlace { get; set; }
        public string OutputFilePath { get; set; } = string.Empty;
    }
}
