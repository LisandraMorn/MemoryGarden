using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Настройки";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblTitle = new Label
            {
                Text = "⚙️ Настройки",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(175, 30)
            };

            // Звук
            CheckBox chkSound = new CheckBox
            {
                Text = "🔊 Звук",
                Font = new Font("Georgia", 14),
                Location = new Point(50, 100),
                AutoSize = true,
                Checked = true
            };

            // Музыка
            CheckBox chkMusic = new CheckBox
            {
                Text = "🎵 Музыка",
                Font = new Font("Georgia", 14),
                Location = new Point(50, 150),
                AutoSize = true,
                Checked = true
            };

            // Размер клеток
            Label lblCellSize = new Label
            {
                Text = "Размер клеток:",
                Font = new Font("Georgia", 14),
                Location = new Point(50, 200),
                AutoSize = true
            };

            ComboBox cmbCellSize = new ComboBox
            {
                Location = new Point(220, 200),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCellSize.Items.AddRange(new object[] { "20px", "25px", "30px", "35px", "40px" });
            cmbCellSize.SelectedIndex = 2;

            Button btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(175, 280),
                BackColor = Color.FromArgb(144, 238, 144),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(chkSound);
            this.Controls.Add(chkMusic);
            this.Controls.Add(lblCellSize);
            this.Controls.Add(cmbCellSize);
            this.Controls.Add(btnSave);
        }
    }
}