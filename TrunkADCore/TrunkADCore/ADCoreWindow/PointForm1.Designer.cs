using System.ComponentModel;

namespace TrunkADCore.ADCoreWindow
{
    partial class PointForm1_
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.flp1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flp2 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flp1
            // 
            this.flp1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flp1.Location = new System.Drawing.Point(0, 0);
            this.flp1.Name = "flp1";
            this.flp1.Size = new System.Drawing.Size(324, 28);
            this.flp1.TabIndex = 0;
            // 
            // flp2
            // 
            this.flp2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flp2.Location = new System.Drawing.Point(0, 28);
            this.flp2.Name = "flp2";
            this.flp2.Size = new System.Drawing.Size(324, 28);
            this.flp2.TabIndex = 1;
            // 
            // PointForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 56);
            this.Controls.Add(this.flp2);
            this.Controls.Add(this.flp1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PointForm1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "定标选点";
            this.Load += new System.EventHandler(this.PointForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp1;
        private System.Windows.Forms.FlowLayoutPanel flp2;
    }
}