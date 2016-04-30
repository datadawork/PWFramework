namespace PlayerInformation
{
    partial class frmPlayersList
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
            this.tResult = new System.Windows.Forms.TextBox();
            this.bRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tResult
            // 
            this.tResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tResult.Location = new System.Drawing.Point(12, 41);
            this.tResult.Multiline = true;
            this.tResult.Name = "tResult";
            this.tResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tResult.Size = new System.Drawing.Size(470, 319);
            this.tResult.TabIndex = 0;
            this.tResult.TabStop = false;
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(12, 12);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(470, 23);
            this.bRefresh.TabIndex = 1;
            this.bRefresh.Text = "Обновить";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // frmPlayersList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 372);
            this.Controls.Add(this.bRefresh);
            this.Controls.Add(this.tResult);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmPlayersList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Список игроков рядом";
            this.Load += new System.EventHandler(this.frmPlayersList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tResult;
        private System.Windows.Forms.Button bRefresh;
    }
}