﻿namespace ConfigAPIClient.Panels
{
	partial class SliderPropertyUserControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.labelOfProperty = new System.Windows.Forms.Label();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelOfProperty
            // 
            this.labelOfProperty.AutoSize = true;
            this.labelOfProperty.Location = new System.Drawing.Point(10, 10);
            this.labelOfProperty.Name = "labelOfProperty";
            this.labelOfProperty.Size = new System.Drawing.Size(16, 13);
            this.labelOfProperty.TabIndex = 2;
            this.labelOfProperty.Text = "...";
            this.labelOfProperty.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.labelOfProperty.MouseHover += new System.EventHandler(this.OnMouseHover);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(328, 7);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(167, 18);
            this.hScrollBar1.TabIndex = 3;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            // 
            // textBoxValue
            // 
            this.textBoxValue.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxValue.ForeColor = System.Drawing.Color.Black;
            this.textBoxValue.Location = new System.Drawing.Point(280, 6);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.ReadOnly = true;
            this.textBoxValue.Size = new System.Drawing.Size(40, 20);
            this.textBoxValue.TabIndex = 4;
            // 
            // SliderPropertyUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.labelOfProperty);
            this.Name = "SliderPropertyUserControl";
            this.Size = new System.Drawing.Size(510, 32);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelOfProperty;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.TextBox textBoxValue;
	}
}
