namespace Mount_Tutorial
{
    partial class MainPanel
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
            System.Windows.Forms.TextBox textBox1;
            textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.SystemColors.Window;
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox1.Enabled = false;
            textBox1.HideSelection = false;
            textBox1.Location = new System.Drawing.Point(16, 6);
            textBox1.Margin = new System.Windows.Forms.Padding(6);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new System.Drawing.Size(336, 108);
            textBox1.TabIndex = 1;
            textBox1.Text = "Этот плагин разработан MichaelProg для выполнения обучающего квеста \"Ваш собствен" +
    "ный скакун!\"";
            textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(textBox1);
            this.Name = "MainPanel";
            this.Size = new System.Drawing.Size(367, 307);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}