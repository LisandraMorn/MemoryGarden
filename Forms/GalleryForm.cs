using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class GalleryForm : Form
    {
        private Profile profile;

        public GalleryForm(Profile prof)
        {
            profile = prof;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Галерея";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);

            Label lblTitle = new Label
            {
                Text = "🖼️ Твои работы",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(300, 20)
            };

            FlowLayoutPanel gallery = new FlowLayoutPanel
            {
                Size = new Size(700, 450),
                Location = new Point(50, 80),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            if (profile.CompletedLevels.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Пока нет готовых работ.\nПройди уровни, чтобы увидеть их здесь!",
                    Font = new Font("Georgia", 14),
                    ForeColor = Color.FromArgb(150, 150, 150),
                    AutoSize = true,
                    Location = new Point(250, 250)
                };
                gallery.Controls.Add(lblEmpty);
            }
            else
            {
                foreach (int levelId in profile.CompletedLevels)
                {
                    PictureBox pic = new PictureBox
                    {
                        Size = new Size(100, 100),
                        Margin = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White
                    };
                    gallery.Controls.Add(pic);
                }
            }

            Button btnBack = new Button
            {
                Text = "← Назад",
                Font = new Font("Georgia", 12),
                Size = new Size(120, 40),
                Location = new Point(50, 540),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(gallery);
            this.Controls.Add(btnBack);
        }
    }
}