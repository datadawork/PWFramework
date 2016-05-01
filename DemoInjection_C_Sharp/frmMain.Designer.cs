namespace DemoInjection_C_Sharp
{
    partial class frmMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.cClients = new System.Windows.Forms.ComboBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cClients
            // 
            this.cClients.FormattingEnabled = true;
            this.cClients.Location = new System.Drawing.Point(12, 12);
            this.cClients.Name = "cClients";
            this.cClients.Size = new System.Drawing.Size(159, 21);
            this.cClients.TabIndex = 0;
            this.cClients.DropDown += new System.EventHandler(this.cClients_DropDown);
            this.cClients.DropDownClosed += new System.EventHandler(this.cClients_DropDownClosed);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(12, 159);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(144, 38);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "GO";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(12, 39);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(159, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(12, 114);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(83, 20);
            this.txtX.TabIndex = 4;
            this.txtX.Text = "0";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(101, 114);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(83, 20);
            this.txtY.TabIndex = 5;
            this.txtY.Text = "0";
            // 
            // txtZ
            // 
            this.txtZ.Location = new System.Drawing.Point(189, 114);
            this.txtZ.Name = "txtZ";
            this.txtZ.Size = new System.Drawing.Size(83, 20);
            this.txtZ.TabIndex = 6;
            this.txtZ.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "CoordX";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "CoordY";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(186, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "CoordZ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "View coordinates";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZ);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.cClients);
            this.Name = "frmMain";
            this.Text = "Inject Example C#";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cClients;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.TextBox txtZ;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

