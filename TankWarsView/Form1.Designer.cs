
namespace GameView
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.IPAddressTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.IPAddressLabel = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IPAddressTextBox});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(817, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "MenuStrip";
            // 
            // IPAddressTextBox
            // 
            this.IPAddressTextBox.Name = "IPAddressTextBox";
            this.IPAddressTextBox.Size = new System.Drawing.Size(100, 23);
            this.IPAddressTextBox.Text = "localhost";
            // 
            // IPAddressLabel
            // 
            this.IPAddressLabel.AutoSize = true;
            this.IPAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPAddressLabel.Location = new System.Drawing.Point(108, 6);
            this.IPAddressLabel.Name = "IPAddressLabel";
            this.IPAddressLabel.Size = new System.Drawing.Size(64, 13);
            this.IPAddressLabel.TabIndex = 1;
            this.IPAddressLabel.Text = "IPAddress";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(202, 3);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(100, 20);
            this.NameTextBox.TabIndex = 2;
            this.NameTextBox.Text = "player";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameLabel.Location = new System.Drawing.Point(302, 6);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(78, 13);
            this.NameLabel.TabIndex = 3;
            this.NameLabel.Text = "Player Name";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectButton.Location = new System.Drawing.Point(409, 1);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseMnemonic = false;
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 497);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.IPAddressLabel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "TankWars";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripTextBox IPAddressTextBox;
        private System.Windows.Forms.Label IPAddressLabel;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Button ConnectButton;
    }
}

