using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class StoryForm : Form
    {
        public StoryForm(string message)
        {
            this.Text = "";
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(500, 300);

            
            Panel borderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Padding = new Padding(1)
            };
            borderPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, borderPanel.ClientRectangle,
                    Color.White, ButtonBorderStyle.Solid);
            };

            Label lblMessage = new Label
            {
                Text = message,
                ForeColor = Color.White,
                Font = new Font("Georgia", 12, FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            borderPanel.Controls.Add(lblMessage);
            this.Controls.Add(borderPanel);

            
            Timer closeTimer = new Timer { Interval = 4000 };
            closeTimer.Tick += (s, e) => { this.Close(); };
            closeTimer.Start();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoryForm));
            this.SuspendLayout();
            // 
            // StoryForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StoryForm";
            this.ResumeLayout(false);

        }
    }
}