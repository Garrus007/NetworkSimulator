﻿namespace NetworkComponents.Controls
{
	partial class PC
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
			this.txtIP = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtIP
			// 
			this.txtIP.Location = new System.Drawing.Point(7, 60);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(112, 20);
			this.txtIP.TabIndex = 3;
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(7, 86);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(112, 23);
			this.btnSend.TabIndex = 4;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// PC
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.txtIP);
			this.Name = "PC";
			this.Size = new System.Drawing.Size(130, 140);
			this.Controls.SetChildIndex(this.txtIP, 0);
			this.Controls.SetChildIndex(this.btnSend, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtIP;
		private System.Windows.Forms.Button btnSend;
	}
}
