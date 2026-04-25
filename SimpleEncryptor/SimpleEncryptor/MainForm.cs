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

        private void encryptBtn_Click(object? sender, EventArgs e)
        {
            var parameters = GetParametersFromForm();
            // TODO: implement encryption logic
        }

        private void decryptBtn_Click(object? sender, EventArgs e)
        {
            var parameters = GetParametersFromForm();
            // TODO: implement decryption logic
        }
    }
}
