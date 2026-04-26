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
            if (inplaceCheckBox.Checked)
            {
                outputTextBox.Text = string.Empty;
            }
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

        private void TextBox_DragEnter(object? sender, DragEventArgs e)
        {
            if (sender is not TextBox tb) return;

            // Password accepts only text drops
            if (tb == passwordTextBox)
            {
                if (e.Data != null && (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text)))
                {
                    e.Effect = DragDropEffects.Copy;
                    tb.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tb.BackColor = System.Drawing.SystemColors.Window;
                }

                return;
            }

            // Input and Output accept file drops or plain text paths
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                tb.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }
            else if (e.Data != null && (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text)))
            {
                e.Effect = DragDropEffects.Copy;
                tb.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            }
            else
            {
                e.Effect = DragDropEffects.None;
                tb.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        private void TextBox_DragDrop(object? sender, DragEventArgs e)
        {
            if (sender is not TextBox tb) return;

            // Password: only text
            if (tb == passwordTextBox)
            {
                if (e.Data != null && (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text)))
                {
                    var text = (string?)e.Data.GetData(DataFormats.UnicodeText) ?? (string?)e.Data.GetData(DataFormats.Text) ?? string.Empty;
                    tb.Text = text.TrimEnd('\r', '\n');
                }

                tb.BackColor = System.Drawing.SystemColors.Window;
                return;
            }

            // Input/Output: prefer file drops, else accept text (path)
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (files.Length > 0)
                {
                    tb.Text = files[0];
                }
            }
            else if (e.Data != null && (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text)))
            {
                var text = (string?)e.Data.GetData(DataFormats.UnicodeText) ?? (string?)e.Data.GetData(DataFormats.Text);
                if (!string.IsNullOrWhiteSpace(text))
                    tb.Text = text.Trim();
            }

            if (tb == inputFileTextBox)
            {
                passwordTextBox.Focus();
            }

            tb.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TextBox_DragLeave(object? sender, EventArgs e)
        {
            if (sender is Control c)
                c.BackColor = System.Drawing.SystemColors.Window;
        }
    }
}
