namespace SendPacketTest
{
    partial class Main
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
            this.bSendPacket = new System.Windows.Forms.Button();
            this.cClients = new System.Windows.Forms.ComboBox();
            this.lClient = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bSendPacket
            // 
            this.bSendPacket.Location = new System.Drawing.Point(208, 6);
            this.bSendPacket.Name = "bSendPacket";
            this.bSendPacket.Size = new System.Drawing.Size(95, 23);
            this.bSendPacket.TabIndex = 5;
            this.bSendPacket.Text = "Медитировать";
            this.bSendPacket.UseVisualStyleBackColor = true;
            this.bSendPacket.Click += new System.EventHandler(this.BSendPacketClick);
            // 
            // cClients
            // 
            this.cClients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cClients.FormattingEnabled = true;
            this.cClients.Location = new System.Drawing.Point(87, 7);
            this.cClients.Name = "cClients";
            this.cClients.Size = new System.Drawing.Size(121, 21);
            this.cClients.TabIndex = 4;
            this.cClients.DropDown += new System.EventHandler(this.CClientsDropDown);
            // 
            // lClient
            // 
            this.lClient.AutoSize = true;
            this.lClient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lClient.Location = new System.Drawing.Point(12, 9);
            this.lClient.Name = "lClient";
            this.lClient.Size = new System.Drawing.Size(69, 15);
            this.lClient.TabIndex = 3;
            this.lClient.Text = "Персонаж:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 37);
            this.Controls.Add(this.bSendPacket);
            this.Controls.Add(this.cClients);
            this.Controls.Add(this.lClient);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Отправка пакета на медитацию";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSendPacket;
        private System.Windows.Forms.ComboBox cClients;
        private System.Windows.Forms.Label lClient;
    }
}

