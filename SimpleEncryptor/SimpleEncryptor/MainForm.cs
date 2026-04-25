using SimpleEncryptor.Core;

namespace SimpleEncryptor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void inplaceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            outputTextBox.Enabled = !inplaceCheckBox.Checked;
        }

        private void outputBtn_Click(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputTextBox.Text = dialog.FileName;
                inplaceCheckBox.Checked = false;
            }
        }

        private void inputFileBtn_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                inputFileTextBox.Text = dialog.FileName;
            }
        }

        private EncryptionParameters GetParametersFromForm()
        {
            return new EncryptionParameters
            {
                InputFilePath = inputFileTextBox.Text,
                Password = passwordTextBox.Text,
                InPlace = inplaceCheckBox.Checked,
                OutputFilePath = outputTextBox.Text
            };
        }

        private bool ValidateParameters(EncryptionParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.InputFilePath) || !File.Exists(parameters.InputFilePath))
            {
                MessageBox.Show("Input file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!parameters.InPlace && string.IsNullOrWhiteSpace(parameters.OutputFilePath))
            {
                MessageBox.Show("Please specify an output file path or enable In-Place.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private async Task ExecuteOperationAsync(EncryptionParameters parameters,
            Action<EncryptionParameters> operation, string successMessage, string errorPrefix)
        {
            SetUiEnabled(false);
            try
            {
                await Task.Run(() => operation(parameters));
                MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{errorPrefix}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUiEnabled(true);
            }
        }

        private async void encryptBtn_Click(object? sender, EventArgs e)
        {
            var parameters = GetParametersFromForm();
            if (!ValidateParameters(parameters)) return;

            await ExecuteOperationAsync(parameters, p => new FileEncryptor().Encrypt(p), "Encryption finished.", "Encryption failed");
        }

        private async void decryptBtn_Click(object? sender, EventArgs e)
        {
            var parameters = GetParametersFromForm();
            if (!ValidateParameters(parameters)) return;

            await ExecuteOperationAsync(parameters, p => new FileEncryptor().Decrypt(p), "Decryption finished.", "Decryption failed");
        }

        private void SetUiEnabled(bool enabled)
        {
            encryptBtn.Enabled = enabled;
            decryptBtn.Enabled = enabled;
            inputFileBtn.Enabled = enabled;
            outputBtn.Enabled = enabled;
            inplaceCheckBox.Enabled = enabled;
            inputFileTextBox.Enabled = enabled;
            outputTextBox.Enabled = enabled && !inplaceCheckBox.Checked;
            passwordTextBox.Enabled = enabled;
        }
    }
}
