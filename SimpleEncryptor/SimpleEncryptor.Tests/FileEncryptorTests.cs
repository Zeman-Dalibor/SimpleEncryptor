using System.Security.Cryptography;
using System.Text;

namespace SimpleEncryptor.Tests
{
    public class FileEncryptorTests : IDisposable
    {
        private readonly string _testDir;
        private readonly FileEncryptor _encryptor;
        private const string Password = "TestPassword123!";

        public FileEncryptorTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "SimpleEncryptorTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
            _encryptor = new FileEncryptor();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }

        private string TestPath(string name) => Path.Combine(_testDir, name);

        private static byte[] GenerateRandomBytes(long size)
        {
            var data = new byte[size];
            RandomNumberGenerator.Fill(data);
            return data;
        }

        private void CreateTestFile(string path, byte[] content)
        {
            File.WriteAllBytes(path, content);
        }

        private void AssertFilesEqual(byte[] expected, string actualPath)
        {
            var actual = File.ReadAllBytes(actualPath);
            Assert.Equal(expected.Length, actual.Length);
            Assert.True(expected.AsSpan().SequenceEqual(actual), "File content differs after decrypt.");
        }

        // ===================== In-Place Tests =====================

        [Theory]
        [InlineData(1)]
        [InlineData(15)]       // < AES block
        [InlineData(16)]       // = AES block
        [InlineData(17)]       // > AES block, < 2 blocks
        [InlineData(1000)]
        [InlineData(64 * 1024 - 1)]       // < buffer size
        [InlineData(64 * 1024)]           // = buffer size
        [InlineData(64 * 1024 + 1)]       // buffer + 1
        [InlineData(256 * 1024)]
        [InlineData(1024 * 1024)]         // 1 MB
        [InlineData(10 * 1024 * 1024)]    // 10 MB
        public void InPlace_BinaryRoundtrip(int size)
        {
            var original = GenerateRandomBytes(size);
            var filePath = TestPath($"binary_{size}.bin");
            CreateTestFile(filePath, original);

            var p = new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = true
            };

            _encryptor.Encrypt(p);

            // Encrypted file must differ from original
            var encrypted = File.ReadAllBytes(filePath);
            Assert.NotEqual(original.Length, encrypted.Length); // footer appended

            _encryptor.Decrypt(p);
            AssertFilesEqual(original, filePath);
        }

        [Fact]
        public void InPlace_TextFile()
        {
            var text = "Příliš žluťoučký kůň úpěl ďábelské ódy.\n" +
                       string.Join("\n", Enumerable.Range(0, 1000).Select(i => $"Line {i}: The quick brown fox jumps over the lazy dog."));
            var original = Encoding.UTF8.GetBytes(text);
            var filePath = TestPath("text.txt");
            CreateTestFile(filePath, original);

            var p = new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = true
            };

            _encryptor.Encrypt(p);
            _encryptor.Decrypt(p);
            AssertFilesEqual(original, filePath);
        }

        [Fact]
        public void InPlace_SimulatedImage()
        {
            // PNG header + random binary payload to simulate an image
            var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var payload = GenerateRandomBytes(50_000);
            var original = pngHeader.Concat(payload).ToArray();
            var filePath = TestPath("image.png");
            CreateTestFile(filePath, original);

            var p = new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = true
            };

            _encryptor.Encrypt(p);
            _encryptor.Decrypt(p);
            AssertFilesEqual(original, filePath);
        }

        // ===================== File-to-File Tests =====================

        [Theory]
        [InlineData(1)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(1000)]
        [InlineData(64 * 1024)]
        [InlineData(64 * 1024 + 1)]
        [InlineData(256 * 1024)]
        [InlineData(1024 * 1024)]
        [InlineData(10 * 1024 * 1024)]
        public void ToFile_BinaryRoundtrip(int size)
        {
            var original = GenerateRandomBytes(size);
            var inputPath = TestPath($"input_{size}.bin");
            var encryptedPath = TestPath($"encrypted_{size}.bin");
            var decryptedPath = TestPath($"decrypted_{size}.bin");
            CreateTestFile(inputPath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            // Original must remain unchanged
            AssertFilesEqual(original, inputPath);

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = encryptedPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = decryptedPath
            });

            AssertFilesEqual(original, decryptedPath);
        }

        [Fact]
        public void ToFile_TextFile()
        {
            var text = "Příliš žluťoučký kůň úpěl ďábelské ódy.\n" +
                       string.Join("\n", Enumerable.Range(0, 1000).Select(i => $"Line {i}: data"));
            var original = Encoding.UTF8.GetBytes(text);
            var inputPath = TestPath("text_input.txt");
            var encryptedPath = TestPath("text_encrypted.bin");
            var decryptedPath = TestPath("text_decrypted.txt");
            CreateTestFile(inputPath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = encryptedPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = decryptedPath
            });

            AssertFilesEqual(original, decryptedPath);
        }

        [Fact]
        public void ToFile_SimulatedImage()
        {
            var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var payload = GenerateRandomBytes(50_000);
            var original = pngHeader.Concat(payload).ToArray();
            var inputPath = TestPath("image_input.png");
            var encryptedPath = TestPath("image_encrypted.bin");
            var decryptedPath = TestPath("image_decrypted.png");
            CreateTestFile(inputPath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = encryptedPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = decryptedPath
            });

            AssertFilesEqual(original, decryptedPath);
        }

        // ===================== Cross-mode Tests =====================

        [Fact]
        public void CrossMode_EncryptToFile_DecryptInPlace()
        {
            var original = GenerateRandomBytes(5000);
            var inputPath = TestPath("cross_input.bin");
            var encryptedPath = TestPath("cross_encrypted.bin");
            CreateTestFile(inputPath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = encryptedPath,
                Password = Password,
                InPlace = true
            });

            AssertFilesEqual(original, encryptedPath);
        }

        [Fact]
        public void CrossMode_EncryptInPlace_DecryptToFile()
        {
            var original = GenerateRandomBytes(5000);
            var filePath = TestPath("cross_inplace.bin");
            var decryptedPath = TestPath("cross_decrypted.bin");
            CreateTestFile(filePath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = true
            });

            _encryptor.Decrypt(new EncryptionParameters
            {
                InputFilePath = filePath,
                Password = Password,
                InPlace = false,
                OutputFilePath = decryptedPath
            });

            AssertFilesEqual(original, decryptedPath);
        }

        // ===================== Wrong Password =====================

        [Fact]
        public void WrongPassword_ThrowsOnDecrypt()
        {
            var original = GenerateRandomBytes(1000);
            var inputPath = TestPath("wrong_pw_input.bin");
            var encryptedPath = TestPath("wrong_pw_encrypted.bin");
            CreateTestFile(inputPath, original);

            _encryptor.Encrypt(new EncryptionParameters
            {
                InputFilePath = inputPath,
                Password = Password,
                InPlace = false,
                OutputFilePath = encryptedPath
            });

            Assert.ThrowsAny<System.Security.Cryptography.CryptographicException>(() =>
            {
                _encryptor.Decrypt(new EncryptionParameters
                {
                    InputFilePath = encryptedPath,
                    Password = "WrongPassword!",
                    InPlace = false,
                    OutputFilePath = TestPath("wrong_pw_decrypted.bin")
                });
            });
        }
    }
}
