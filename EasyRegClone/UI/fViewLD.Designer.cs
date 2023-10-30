
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace easy.UI
{
    partial class fViewLD
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fViewLD));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelListDevice = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 0;
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 0;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ShowAlways = true;
            // 
            // panelListDevice
            // 
            this.panelListDevice.AutoScroll = true;
            this.panelListDevice.BackColor = System.Drawing.Color.White;
            this.panelListDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelListDevice.Location = new System.Drawing.Point(0, 0);
            this.panelListDevice.Name = "panelListDevice";
            this.panelListDevice.Padding = new System.Windows.Forms.Padding(10);
            this.panelListDevice.Size = new System.Drawing.Size(1065, 591);
            this.panelListDevice.TabIndex = 2;
            // 
            // fViewLD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 591);
            this.Controls.Add(this.panelListDevice);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "fViewLD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View LDPlayer - EASY SOFTWARE";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fViewChrome_FormClosing);
            this.ResumeLayout(false);

        }

        private ToolTip toolTip1;

        private FlowLayoutPanel panelListDevice;

        #endregion
    }
}