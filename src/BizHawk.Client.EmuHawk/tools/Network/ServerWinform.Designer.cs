namespace BizHawk.Client.EmuHawk
{
	partial class NetServerWinform
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
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.portInput = new System.Windows.Forms.TextBox();
            this.netInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(176, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "port:";
            // 
            // portInput
            // 
            this.portInput.Location = new System.Drawing.Point(63, 12);
            this.portInput.Name = "portInput";
            this.portInput.Size = new System.Drawing.Size(100, 21);
            this.portInput.TabIndex = 2;
            this.portInput.Text = "10010";
            // 
            // netInfo
            // 
            this.netInfo.AutoSize = true;
            this.netInfo.Location = new System.Drawing.Point(98, 65);
            this.netInfo.Name = "netInfo";
            this.netInfo.Size = new System.Drawing.Size(65, 12);
            this.netInfo.TabIndex = 3;
            this.netInfo.Text = "服务未启动";
            // 
            // NetServerWinform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.netInfo);
            this.Controls.Add(this.portInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetServerWinform";
            this.Text = "Net Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NetServerWinform_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox portInput;
		private System.Windows.Forms.Label netInfo;
	}
}