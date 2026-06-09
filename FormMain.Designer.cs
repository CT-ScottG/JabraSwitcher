namespace JabraSwitcher
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.comboBoxOutputs = new System.Windows.Forms.ComboBox();
            this.labelDefaultOutput = new System.Windows.Forms.Label();
            this.labelJabraDongle = new System.Windows.Forms.Label();
            this.labelJabraDongleName = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.labelDefaultInput = new System.Windows.Forms.Label();
            this.comboBoxInputs = new System.Windows.Forms.ComboBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxOutputs
            // 
            this.comboBoxOutputs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOutputs.FormattingEnabled = true;
            this.comboBoxOutputs.Location = new System.Drawing.Point(91, 11);
            this.comboBoxOutputs.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxOutputs.Name = "comboBoxOutputs";
            this.comboBoxOutputs.Size = new System.Drawing.Size(285, 21);
            this.comboBoxOutputs.TabIndex = 0;
            this.comboBoxOutputs.SelectedIndexChanged += new System.EventHandler(this.ComboBoxOutputs_SelectedIndexChanged);
            // 
            // labelDefaultOutput
            // 
            this.labelDefaultOutput.AutoSize = true;
            this.labelDefaultOutput.Location = new System.Drawing.Point(11, 14);
            this.labelDefaultOutput.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDefaultOutput.Name = "labelDefaultOutput";
            this.labelDefaultOutput.Size = new System.Drawing.Size(76, 13);
            this.labelDefaultOutput.TabIndex = 1;
            this.labelDefaultOutput.Text = "Default Output";
            // 
            // labelJabraDongle
            // 
            this.labelJabraDongle.AutoSize = true;
            this.labelJabraDongle.Location = new System.Drawing.Point(11, 62);
            this.labelJabraDongle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelJabraDongle.Name = "labelJabraDongle";
            this.labelJabraDongle.Size = new System.Drawing.Size(70, 13);
            this.labelJabraDongle.TabIndex = 2;
            this.labelJabraDongle.Text = "Jabra Dongle";
            // 
            // labelJabraDongleName
            // 
            this.labelJabraDongleName.AutoSize = true;
            this.labelJabraDongleName.Location = new System.Drawing.Point(88, 62);
            this.labelJabraDongleName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelJabraDongleName.Name = "labelJabraDongleName";
            this.labelJabraDongleName.Size = new System.Drawing.Size(57, 13);
            this.labelJabraDongleName.TabIndex = 3;
            this.labelJabraDongleName.Text = "Not Found";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Jabra Switcher";
            this.notifyIcon.Visible = true;
            // 
            // labelDefaultInput
            // 
            this.labelDefaultInput.AutoSize = true;
            this.labelDefaultInput.Location = new System.Drawing.Point(11, 39);
            this.labelDefaultInput.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelDefaultInput.Name = "labelDefaultInput";
            this.labelDefaultInput.Size = new System.Drawing.Size(68, 13);
            this.labelDefaultInput.TabIndex = 5;
            this.labelDefaultInput.Text = "Default Input";
            // 
            // comboBoxInputs
            // 
            this.comboBoxInputs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInputs.FormattingEnabled = true;
            this.comboBoxInputs.Location = new System.Drawing.Point(91, 36);
            this.comboBoxInputs.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxInputs.Name = "comboBoxInputs";
            this.comboBoxInputs.Size = new System.Drawing.Size(285, 21);
            this.comboBoxInputs.TabIndex = 4;
            this.comboBoxInputs.SelectedIndexChanged += new System.EventHandler(this.ComboBoxInputs_SelectedIndexChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(301, 72);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 107);
            this.Controls.Add(this.labelDefaultInput);
            this.Controls.Add(this.comboBoxInputs);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelJabraDongleName);
            this.Controls.Add(this.labelJabraDongle);
            this.Controls.Add(this.labelDefaultOutput);
            this.Controls.Add(this.comboBoxOutputs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Jabra Switcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxOutputs;
        private System.Windows.Forms.Label labelDefaultOutput;
        private System.Windows.Forms.Label labelJabraDongle;
        private System.Windows.Forms.Label labelJabraDongleName;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label labelDefaultInput;
        private System.Windows.Forms.ComboBox comboBoxInputs;
        private System.Windows.Forms.Button buttonSave;
    }
}

