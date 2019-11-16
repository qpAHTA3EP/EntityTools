namespace VariableTools.Forms
{
    partial class VariablesAddonPanel
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
            this.btnVariables = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(131, 12);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(97, 13);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Athor: MichaelProg";
            // 
            // btnVariables
            // 
            this.btnVariables.Location = new System.Drawing.Point(143, 37);
            this.btnVariables.Name = "btnVariables";
            this.btnVariables.Size = new System.Drawing.Size(75, 23);
            this.btnVariables.TabIndex = 1;
            this.btnVariables.Text = "Variables";
            this.btnVariables.UseVisualStyleBackColor = true;
            // 
            // VariablesAddonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnVariables);
            this.Controls.Add(this.lblAuthor);
            this.Name = "VariablesAddonPanel";
            this.Size = new System.Drawing.Size(370, 416);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Button btnVariables;
    }
}
