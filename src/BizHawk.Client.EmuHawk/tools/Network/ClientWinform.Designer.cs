namespace BizHawk.Client.EmuHawk
{
	partial class NetClienWinform
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
            this.linkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // linkButton
            // 
            this.linkButton.Location = new System.Drawing.Point(105, 109);
            this.linkButton.Name = "linkButton";
            this.linkButton.Size = new System.Drawing.Size(75, 23);
            this.linkButton.TabIndex = 0;
            this.linkButton.Text = "link";
            this.linkButton.UseVisualStyleBackColor = true;
            this.linkButton.Click += new System.EventHandler(this.linkButton_Click);
            // 
            // NetCliendWinform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.linkButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetClienWinform";
            this.Text = "Net Client";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button linkButton;
	}
}