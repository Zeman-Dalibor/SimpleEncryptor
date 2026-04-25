using System.Security.Cryptography;

namespace SimpleEncryptor.Tests
{
    [Trait("Category", "LargeFile")]
    public class LargeFileEncryptorTests : IDisposable
    {
        private readonly string _testDir;
        private readonly FileEncryptor _encryptor;
        private const string Password = "LargeFileTestPassword!";

        public LargeFileEncryptorTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "SimpleEncryptorLargeTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
            _encryptor = new FileEncryptor();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }

        private string TestPath(string name) => Path.Combine(_testDir, name);

        private static void CreateLargeFile(string path, long size)
        {
            const int chunkSize = 1024 * 1024; // 1 MB
            var chunk = new byte[chunkSize];
            using var rng = RandomNumberGenerator.Create();
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, chunkSize);

            long remaining = size;
            while (remaining > 0)
            {
                int toWrite = (int)Math.Min(chunkSize, remaining);
                rng.GetBytes(chunk, 0, toWrite);
                fs.Write(chunk, 0, toWrite);
                remaining -= toWrite;
            }
        }

        private static byte[] HashFile(string path)
        {
            using var sha = SHA256.Create();
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024);
            return sha.ComputeHash(fs);
        }

        [Theory]
        [InlineData(100L * 1024 * 1024)]        // 100 MB
        [InlineData(1024L * 1024 * 1024)]        // 1 GB
        public void InPlace_LargeFileRoundtrip(long size)
        {
            var filePath = TestPath($"large_inplace_{size}.bin");
            CreateLargeFile(filePath, size);
            var originalHash = HashFile(filePath);

            var p = new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = true
            };

            _encryptor.Encrypt(p);

            // File size must have changed (footer added)
            Assert.NotEqual(size, new FileInfo(filePath).Length);

            _encryptor.Decrypt(p);

            Assert.Equal(size, new FileInfo(filePath).Length);
            Assert.Equal(originalHash, HashFile(filePath));

            File.Delete(filePath);
        }

        [Theory]
        [InlineData(100L * 1024 * 1024)]
        [InlineData(1024L * 1024 * 1024)]
        public void ToFile_LargeFileRoundtrip(long size)
        {
            var inputPath = TestPath($"large_input_{size}.bin");
            var encryptedPath = TestPath($"large_encrypted_{size}.bin");
            var decryptedPath = TestPath($"large_decrypted_{size}.bin");
            CreateLargeFile(inputPath, size);
            var originalHash = HashFile(inputPath);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            File.Delete(inputPath); // free disk space

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = encryptedPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = decryptedPath
            });

            File.Delete(encryptedPath); // free disk space

            Assert.Equal(size, new FileInfo(decryptedPath).Length);
            Assert.Equal(originalHash, HashFile(decryptedPath));

            File.Delete(decryptedPath);
        }
    }
}
