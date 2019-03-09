namespace ScalarAddon.Forms
{
    partial class ScalarAddonPanel
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblAuthor = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSet = new System.Windows.Forms.Button();
            this.numUdbValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUdbValue)).BeginInit();
            this.SuspendLayout();
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(132, 0);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(97, 13);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Athor: MichaelProg";
            this.lblAuthor.Click += new System.EventHandler(this.lblAuthor_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(143, 37);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(143, 170);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(75, 23);
            this.btnSet.TabIndex = 2;
            this.btnSet.Text = "Set value";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // numUdbValue
            // 
            this.numUdbValue.Location = new System.Drawing.Point(120, 144);
            this.numUdbValue.Name = "numUdbValue";
            this.numUdbValue.Size = new System.Drawing.Size(120, 20);
            this.numUdbValue.TabIndex = 3;
            this.numUdbValue.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // ScalarAddonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numUdbValue);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.lblAuthor);
            this.Name = "ScalarAddonPanel";
            ((System.ComponentModel.ISupportInitialize)(this.numUdbValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.NumericUpDown numUdbValue;
    }
}
