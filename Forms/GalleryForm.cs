using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class GalleryForm : Form
    {
        private Profile profile;
        private Panel parentPanel;
        private Form mainForm;

        
        public GalleryForm(Profile prof, Panel panel, Form main)
        {
            profile = prof;
            parentPanel = panel;
            mainForm = main;
            InitializeUI(true);
        }

        
        public GalleryForm(Profile prof)
        {
            profile = prof;
            InitializeUI(false);
        }

        private void InitializeUI(bool showBackButton)
        {
            this.Text = "Воспоминания";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;

            if (!showBackButton)
            {
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
            }

            
            Label lblTitle = new Label
            {
                Text = "🖼 Воспоминания",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(300, 20)
            };

           
            FlowLayoutPanel galleryPanel = new FlowLayoutPanel
            {
                Size = new Size(700, 450),
                Location = new Point(50, 80),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Black
            };

            if (profile.Gallery.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Пока нет сохранённых воспоминаний.\nРешай головоломки, чтобы увидеть их здесь!",
                    Font = new Font("Georgia", 14),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(250, 250)
                };
                galleryPanel.Controls.Add(lblEmpty);
            }
            else
            {
                foreach (var record in profile.Gallery)
                {
                    Panel thumb = new Panel
                    {
                        Size = new Size(100, 100),
                        Margin = new Padding(5),
                        BackColor = Color.Black,
                        Cursor = Cursors.Hand
                    };
                    Image img = File.Exists(record.ImagePath) ? Image.FromFile(record.ImagePath) : null;
                    thumb.Paint += (s, pe) =>
                    {
                        if (img == null) return;
                        pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                        pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                        float scale = Math.Min((float)thumb.Width / img.Width, (float)thumb.Height / img.Height);
                        int w = (int)(img.Width * scale);
                        int h = (int)(img.Height * scale);
                        int x = (thumb.Width - w) / 2;
                        int y = (thumb.Height - h) / 2;
                        pe.Graphics.DrawImage(img, x, y, w, h);
                    };
                    thumb.Click += (s, e) => ShowFullImage(record);
                    galleryPanel.Controls.Add(thumb);
                }
            }

            this.Controls.Add(lblTitle);
            this.Controls.Add(galleryPanel);

            
            if (showBackButton)
            {
                Button btnBack = new Button
                {
                    Text = "← Назад",
                    Font = new Font("Georgia", 12),
                    Size = new Size(120, 40),
                    Location = new Point(50, 540),
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnBack.FlatAppearance.BorderSize = 2;
                btnBack.FlatAppearance.BorderColor = Color.White;
                btnBack.Click += (s, e) => GoBack();
                this.Controls.Add(btnBack);
            }
        }

        private void ShowFullImage(PuzzleRecord record)
        {
            Form fullForm = new Form
            {
                Text = "",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.Black,
                FormBorderStyle = FormBorderStyle.None
            };

            Image original = File.Exists(record.ImagePath) ? Image.FromFile(record.ImagePath) : null;
            Panel picturePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            picturePanel.Paint += (s, e) =>
            {
                if (original == null) return;
                Graphics g = e.Graphics;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                float scale = Math.Min((float)picturePanel.Width / original.Width,
                                       (float)picturePanel.Height / original.Height);
                int w = (int)(original.Width * scale);
                int h = (int)(original.Height * scale);
                int x = (picturePanel.Width - w) / 2;
                int y = (picturePanel.Height - h) / 2;
                g.DrawImage(original, x, y, w, h);
            };

            Label caption = new Label
            {
                Text = record.Caption,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Georgia", 12, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 60
            };

            fullForm.Controls.Add(picturePanel);
            fullForm.Controls.Add(caption);
            fullForm.Click += (s, ev) => fullForm.Close();
            picturePanel.Click += (s, ev) => fullForm.Close();
            fullForm.ShowDialog();
        }

        private void GoBack()
        {
            if (parentPanel != null && mainForm is Form1 form1)
            {
                parentPanel.Controls.Clear();
                form1.GetType().GetMethod("ShowMainMenu",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)?.Invoke(form1, null);
            }
        }
    }
}