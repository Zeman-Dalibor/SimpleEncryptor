using System.Security.Cryptography;

namespace SimpleEncryptor
{
    public class FileEncryptor
    {
        private const int BufferSize = 64 * 1024;
        private const int FooterSize = 16 + 8; // IV + original size

        public void Encrypt(EncryptionParameters parameters)
        {
            using var aes = CreateAes(DeriveKey(parameters.Password));
            aes.GenerateIV();

            if (parameters.InPlace)
                EncryptInPlace(parameters.InputFilePath, aes);
            else
                EncryptToFile(parameters.InputFilePath, parameters.OutputFilePath, aes);
        }

        public void Decrypt(EncryptionParameters parameters)
        {
            var key = DeriveKey(parameters.Password);

            if (parameters.InPlace)
                DecryptInPlace(parameters.InputFilePath, key);
            else
                DecryptToFile(parameters.InputFilePath, parameters.OutputFilePath, key);
        }

        private void EncryptInPlace(string filePath, Aes aes)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            long originalSize = fs.Length;

            TransformInPlace(fs, originalSize, aes.CreateEncryptor());
            WriteFooter(fs, aes.IV, originalSize);
        }

        private void DecryptInPlace(string filePath, byte[] key)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            var (iv, originalSize) = ReadFooter(fs);
            long encryptedLength = fs.Length - FooterSize;

            using var aes = CreateAes(key, iv);
            TransformInPlace(fs, encryptedLength, aes.CreateDecryptor());
            fs.SetLength(originalSize);
        }

        private void EncryptToFile(string inputPath, string outputPath, Aes aes)
        {
            using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            long originalSize = input.Length;
            using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            using (var cs = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true))
                input.CopyTo(cs, BufferSize);

            WriteFooter(output, aes.IV, originalSize);
        }

        private void DecryptToFile(string inputPath, string outputPath, byte[] key)
        {
            using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            var (iv, originalSize) = ReadFooter(input);
            long encryptedLength = input.Length - FooterSize;

            using var aes = CreateAes(key, iv);
            input.Position = 0;
            using var bounded = new BoundedStream(input, encryptedLength);
            using var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            using var cs = new CryptoStream(bounded, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cs.CopyTo(output, BufferSize);
            output.SetLength(originalSize);
        }

        private static void TransformInPlace(FileStream fs, long dataLength, ICryptoTransform transform)
        {
            using var _ = transform;
            var readBuf = new byte[BufferSize];
            var writeBuf = new byte[BufferSize];
            long readPos = 0;
            long writePos = 0;

            while (readPos < dataLength)
            {
                fs.Position = readPos;
                int toRead = (int)Math.Min(BufferSize, dataLength - readPos);
                int bytesRead = fs.Read(readBuf, 0, toRead);
                bool isFinal = readPos + bytesRead >= dataLength;

                byte[] result;
                int resultLen;

                if (isFinal)
                {
                    result = transform.TransformFinalBlock(readBuf, 0, bytesRead);
                    resultLen = result.Length;
                }
                else
                {
                    resultLen = transform.TransformBlock(readBuf, 0, bytesRead, writeBuf, 0);
                    result = writeBuf;
                }

                fs.Position = writePos;
                fs.Write(result, 0, resultLen);
                readPos += bytesRead;
                writePos += resultLen;
            }

            fs.Flush();
        }

        private static void WriteFooter(Stream stream, byte[] iv, long originalSize)
        {
            stream.Write(iv, 0, iv.Length);
            stream.Write(BitConverter.GetBytes(originalSize), 0, 8);
        }

        private static (byte[] iv, long originalSize) ReadFooter(Stream stream)
        {
            stream.Position = stream.Length - FooterSize;
            var iv = new byte[16];
            stream.Read(iv, 0, 16);
            var sizeBytes = new byte[8];
            stream.Read(sizeBytes, 0, 8);
            return (iv, BitConverter.ToInt64(sizeBytes, 0));
        }

        private static Aes CreateAes(byte[] key, byte[]? iv = null)
        {
            var aes = Aes.Create();
            aes.Key = key;
            if (iv != null) aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        private static byte[] DeriveKey(string password)
        {
            return SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
