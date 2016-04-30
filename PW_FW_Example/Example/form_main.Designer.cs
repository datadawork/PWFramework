namespace GreecePriest
{
    partial class form_main
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
            this.combo_clients = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.statusInfo = new System.Windows.Forms.StatusStrip();
            this.lbl_messages = new System.Windows.Forms.ToolStripStatusLabel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.offsetName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.offsetValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.statusInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // combo_clients
            // 
            this.combo_clients.FormattingEnabled = true;
            this.combo_clients.Location = new System.Drawing.Point(18, 18);
            this.combo_clients.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.combo_clients.Name = "combo_clients";
            this.combo_clients.Size = new System.Drawing.Size(184, 28);
            this.combo_clients.TabIndex = 1;
            this.combo_clients.SelectedIndexChanged += new System.EventHandler(this.clients_SelectedIndexChanged);
            this.combo_clients.Click += new System.EventHandler(this.clients_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(210, 13);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(156, 36);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Подключить";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // statusInfo
            // 
            this.statusInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_messages});
            this.statusInfo.Location = new System.Drawing.Point(0, 355);
            this.statusInfo.Name = "statusInfo";
            this.statusInfo.Size = new System.Drawing.Size(873, 22);
            this.statusInfo.TabIndex = 8;
            // 
            // lbl_messages
            // 
            this.lbl_messages.Name = "lbl_messages";
            this.lbl_messages.Size = new System.Drawing.Size(76, 17);
            this.lbl_messages.Text = "lbl_messages";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.offsetName,
            this.offsetValue});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listView1.Location = new System.Drawing.Point(373, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(500, 355);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // offsetName
            // 
            this.offsetName.Text = "Название";
            // 
            // offsetValue
            // 
            this.offsetValue.Text = "Значение";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(55, 69);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(147, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(55, 98);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(147, 23);
            this.progressBar2.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "HP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "MP";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(18, 177);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(102, 24);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "Meditation";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "TargetWID";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(110, 130);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(92, 26);
            this.textBox1.TabIndex = 16;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(210, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(156, 50);
            this.button1.TabIndex = 17;
            this.button1.Text = "Найти смещения и адреса";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 377);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.statusInfo);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.combo_clients);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "form_main";
            this.Text = "PW FW Example";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_main_FormClosing);
            this.Load += new System.EventHandler(this.form_main_Load);
            this.statusInfo.ResumeLayout(false);
            this.statusInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox combo_clients;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.StatusStrip statusInfo;
        private System.Windows.Forms.ToolStripStatusLabel lbl_messages;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ColumnHeader offsetName;
        private System.Windows.Forms.ColumnHeader offsetValue;
        private System.Windows.Forms.Button button1;
    }
}

