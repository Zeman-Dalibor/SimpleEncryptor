namespace SimpleEncryptor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            inputFileTextBox = new TextBox();
            passwordTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            inputFileBtn = new Button();
            inplaceCheckBox = new CheckBox();
            outputTextBox = new TextBox();
            label3 = new Label();
            outputBtn = new Button();
            encryptBtn = new Button();
            decryptBtn = new Button();
            SuspendLayout();
            // 
            // inputFileTextBox
            // 
            inputFileTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputFileTextBox.Location = new Point(12, 35);
            inputFileTextBox.Name = "inputFileTextBox";
            inputFileTextBox.Size = new Size(477, 27);
            inputFileTextBox.TabIndex = 0;
            inputFileTextBox.AllowDrop = true;
            inputFileTextBox.DragEnter += TextBox_DragEnter;
            inputFileTextBox.DragDrop += TextBox_DragDrop;
            inputFileTextBox.DragLeave += TextBox_DragLeave;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passwordTextBox.Location = new Point(12, 116);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(619, 27);
            passwordTextBox.TabIndex = 0;
            passwordTextBox.AllowDrop = true;
            passwordTextBox.DragEnter += TextBox_DragEnter;
            passwordTextBox.DragDrop += TextBox_DragDrop;
            passwordTextBox.DragLeave += TextBox_DragLeave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 93);
            label1.Name = "label1";
            label1.Size = new Size(73, 20);
            label1.TabIndex = 1;
            label1.Text = "Password:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 12);
            label2.Name = "label2";
            label2.Size = new Size(35, 20);
            label2.TabIndex = 1;
            label2.Text = "File:";
            // 
            // inputFileBtn
            // 
            inputFileBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            inputFileBtn.Location = new Point(495, 35);
            inputFileBtn.Name = "inputFileBtn";
            inputFileBtn.Size = new Size(131, 29);
            inputFileBtn.TabIndex = 2;
            inputFileBtn.Text = "Open";
            inputFileBtn.UseVisualStyleBackColor = true;
            inputFileBtn.Click += inputFileBtn_Click;
            // 
            // inplaceCheckBox
            // 
            inplaceCheckBox.AutoSize = true;
            inplaceCheckBox.Checked = true;
            inplaceCheckBox.CheckState = CheckState.Checked;
            inplaceCheckBox.Location = new Point(12, 170);
            inplaceCheckBox.Name = "inplaceCheckBox";
            inplaceCheckBox.Size = new Size(84, 24);
            inplaceCheckBox.TabIndex = 3;
            inplaceCheckBox.Text = "In-Place";
            inplaceCheckBox.UseVisualStyleBackColor = true;
            inplaceCheckBox.CheckedChanged += inplaceCheckBox_CheckedChanged;
            // 
            // outputTextBox
            // 
            outputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            outputTextBox.Enabled = false;
            outputTextBox.Location = new Point(12, 226);
            outputTextBox.Name = "outputTextBox";
            outputTextBox.Size = new Size(477, 27);
            outputTextBox.TabIndex = 0;
            outputTextBox.AllowDrop = true;
            outputTextBox.DragEnter += TextBox_DragEnter;
            outputTextBox.DragDrop += TextBox_DragDrop;
            outputTextBox.DragLeave += TextBox_DragLeave;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 203);
            label3.Name = "label3";
            label3.Size = new Size(35, 20);
            label3.TabIndex = 1;
            label3.Text = "File:";
            // 
            // outputBtn
            // 
            outputBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            outputBtn.Location = new Point(495, 226);
            outputBtn.Name = "outputBtn";
            outputBtn.Size = new Size(131, 29);
            outputBtn.TabIndex = 2;
            outputBtn.Text = "Open";
            outputBtn.UseVisualStyleBackColor = true;
            outputBtn.Click += this.outputBtn_Click;
            // 
            // encryptBtn
            // 
            encryptBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            encryptBtn.Location = new Point(12, 283);
            encryptBtn.Name = "encryptBtn";
            encryptBtn.Size = new Size(131, 29);
            encryptBtn.TabIndex = 2;
            encryptBtn.Text = "Encrypt";
            encryptBtn.UseVisualStyleBackColor = true;
            encryptBtn.Click += encryptBtn_Click;
            // 
            // decryptBtn
            // 
            decryptBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            decryptBtn.Location = new Point(149, 283);
            decryptBtn.Name = "decryptBtn";
            decryptBtn.Size = new Size(131, 29);
            decryptBtn.TabIndex = 2;
            decryptBtn.Text = "Decrypt";
            decryptBtn.UseVisualStyleBackColor = true;
            decryptBtn.Click += decryptBtn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(638, 342);
            Controls.Add(inplaceCheckBox);
            Controls.Add(outputBtn);
            Controls.Add(decryptBtn);
            Controls.Add(encryptBtn);
            Controls.Add(inputFileBtn);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(passwordTextBox);
            Controls.Add(outputTextBox);
            Controls.Add(inputFileTextBox);
            Name = "MainForm";
            Text = "Simple Fast Encryptor";
            AllowDrop = true;
            DragEnter += MainForm_DragEnter;
            DragDrop += MainForm_DragDrop;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox inputFileTextBox;
        private TextBox passwordTextBox;
        private Label label1;
        private Label label2;
        private Button inputFileBtn;
        private CheckBox inplaceCheckBox;
        private TextBox outputTextBox;
        private Label label3;
        private Button outputBtn;
        private Button encryptBtn;
        private Button decryptBtn;
    }
}
