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
        private Label lblMusicValue;

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
            lblMusicValue.Text = $"{tbMusic.Value}%";
        }

        private void InitializeUI()
        {
            this.Text = "Настройки";
            this.Size = new Size(570, 300);
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

            Button btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(175, 200),
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.SuspendLayout();
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.ResumeLayout(false);

        }
    }
}