using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class SettingsForm : Form
    {
        private Panel parentPanel;
        private Form mainForm;
        private TrackBar tbMusic;
        private TrackBar tbSound;
        private Label lblMusicValue;
        private Label lblSoundValue;

        public SettingsForm(Panel panel, Form main)
        {
            parentPanel = panel;
            mainForm = main;
            InitializeUI();
            LoadSettings();
        }

        public SettingsForm()
        {
            InitializeUI();
            LoadSettings();
        }

        private void LoadSettings()
        {
            Settings settings = Settings.Load();
            tbMusic.Value = settings.MusicVolume;
            tbSound.Value = settings.SoundVolume;
            lblMusicValue.Text = $"{tbMusic.Value}%";
            lblSoundValue.Text = $"{tbSound.Value}%";
        }

        private void InitializeUI()
        {
            this.Text = "Настройки";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblTitle = new Label
            {
                Text = "⚙️ Настройки",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(175, 30)
            };

            
            Label lblMusic = new Label
            {
                Text = "🎵 Музыка",
                Font = new Font("Georgia", 14),
                ForeColor = Color.White,
                Location = new Point(50, 100),
                AutoSize = true
            };

            tbMusic = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10,
                Location = new Point(50, 130),
                Size = new Size(300, 45),
                BackColor = Color.Black,
                ForeColor = Color.White
            };
            tbMusic.Scroll += (s, e) =>
            {
                lblMusicValue.Text = $"{tbMusic.Value}%";
                MusicManager.SetVolume(tbMusic.Value);
                Settings settings = Settings.Load();
                settings.MusicVolume = tbMusic.Value;
                settings.Save();
            };

            lblMusicValue = new Label
            {
                Text = "100%",
                Font = new Font("Georgia", 12),
                ForeColor = Color.White,
                Location = new Point(360, 135),
                AutoSize = true
            };

            
            Label lblSound = new Label
            {
                Text = "🔊 Звук",
                Font = new Font("Georgia", 14),
                ForeColor = Color.White,
                Location = new Point(50, 190),
                AutoSize = true
            };

            tbSound = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                TickFrequency = 10,
                Location = new Point(50, 220),
                Size = new Size(300, 45),
                BackColor = Color.Black,
                ForeColor = Color.White
            };
            tbSound.Scroll += (s, e) =>
            {
                lblSoundValue.Text = $"{tbSound.Value}%";
                Settings settings = Settings.Load();
                settings.SoundVolume = tbSound.Value;
                settings.Save();
            };

            lblSoundValue = new Label
            {
                Text = "100%",
                Font = new Font("Georgia", 12),
                ForeColor = Color.White,
                Location = new Point(360, 225),
                AutoSize = true
            };

            
            Label lblCellSize = new Label
            {
                Text = "Размер клеток:",
                Font = new Font("Georgia", 14),
                ForeColor = Color.White,
                Location = new Point(50, 290),
                AutoSize = true
            };

            ComboBox cmbCellSize = new ComboBox
            {
                Location = new Point(220, 290),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.Black,
                ForeColor = Color.White
            };
            cmbCellSize.Items.AddRange(new object[] { "20px", "25px", "30px", "35px", "40px" });
            cmbCellSize.SelectedIndex = 2;

            Button btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(175, 350),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 2;
            btnSave.FlatAppearance.BorderColor = Color.White;
            btnSave.Click += (s, e) => GoBack();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMusic);
            this.Controls.Add(tbMusic);
            this.Controls.Add(lblMusicValue);
            this.Controls.Add(lblSound);
            this.Controls.Add(tbSound);
            this.Controls.Add(lblSoundValue);
            this.Controls.Add(lblCellSize);
            this.Controls.Add(cmbCellSize);
            this.Controls.Add(btnSave);
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
            else
            {
                this.Close();
            }
        }
    }
}