namespace PlayerInformation
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
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lClient = new System.Windows.Forms.Label();
            this.cClients = new System.Windows.Forms.ComboBox();
            this.bGet = new System.Windows.Forms.Button();
            this.resultBox = new System.Windows.Forms.TextBox();
            this.bShowPlayersList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lClient
            // 
            this.lClient.AutoSize = true;
            this.lClient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lClient.Location = new System.Drawing.Point(10, 9);
            this.lClient.Name = "lClient";
            this.lClient.Size = new System.Drawing.Size(133, 15);
            this.lClient.TabIndex = 0;
            this.lClient.Text = "Данные о персонаже:";
            // 
            // cClients
            // 
            this.cClients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cClients.FormattingEnabled = true;
            this.cClients.Location = new System.Drawing.Point(149, 7);
            this.cClients.Name = "cClients";
            this.cClients.Size = new System.Drawing.Size(121, 21);
            this.cClients.TabIndex = 1;
            this.cClients.DropDown += new System.EventHandler(this.CClientsDropDown);
            // 
            // bGet
            // 
            this.bGet.Location = new System.Drawing.Point(270, 6);
            this.bGet.Name = "bGet";
            this.bGet.Size = new System.Drawing.Size(75, 23);
            this.bGet.TabIndex = 2;
            this.bGet.Text = "Получить";
            this.bGet.UseVisualStyleBackColor = true;
            this.bGet.Click += new System.EventHandler(this.BGetClick);
            // 
            // resultBox
            // 
            this.resultBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultBox.BackColor = System.Drawing.SystemColors.Window;
            this.resultBox.Location = new System.Drawing.Point(13, 34);
            this.resultBox.Multiline = true;
            this.resultBox.Name = "resultBox";
            this.resultBox.ReadOnly = true;
            this.resultBox.Size = new System.Drawing.Size(332, 147);
            this.resultBox.TabIndex = 3;
            // 
            // bShowPlayersList
            // 
            this.bShowPlayersList.Location = new System.Drawing.Point(13, 187);
            this.bShowPlayersList.Name = "bShowPlayersList";
            this.bShowPlayersList.Size = new System.Drawing.Size(332, 23);
            this.bShowPlayersList.TabIndex = 4;
            this.bShowPlayersList.Text = "Список игроков рядом";
            this.bShowPlayersList.UseVisualStyleBackColor = true;
            this.bShowPlayersList.Click += new System.EventHandler(this.bShowPlayersList_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 222);
            this.Controls.Add(this.bShowPlayersList);
            this.Controls.Add(this.resultBox);
            this.Controls.Add(this.bGet);
            this.Controls.Add(this.cClients);
            this.Controls.Add(this.lClient);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Данные о персонаже";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lClient;
        private System.Windows.Forms.ComboBox cClients;
        private System.Windows.Forms.Button bGet;
        private System.Windows.Forms.TextBox resultBox;
        private System.Windows.Forms.Button bShowPlayersList;
    }
}

