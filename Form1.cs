using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public partial class Form1 : Form
    {
        private Profile currentProfile;
        private int currentProfileIndex = -1;
        private Settings gameSettings;
        private Panel contentPanel;

        private Button btnPlay;
        private Button btnGallery;
        private Button btnSettings;
        private Button btnHowToPlay;
        private Button btnExit;
        private Button btnProfile;
        private Label lblProfileName;

        public Form1()
        {
            InitializeMainForm();
        }

        private void InitializeMainForm()
        {
            this.Text = "Memory Garden";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            
            gameSettings = Settings.Load();

            
            if (gameSettings.LastProfileIndex >= 0 && Profile.Exists(gameSettings.LastProfileIndex))
            {
                currentProfileIndex = gameSettings.LastProfileIndex;
                currentProfile = Profile.Load(currentProfileIndex);
            }

            
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            this.Controls.Add(contentPanel);

            
            ShowMainMenu();

            this.FormClosing += Form1_FormClosing;
        }

        private void ShowMainMenu()
        {
            contentPanel.Controls.Clear();
            InitializeUI();

            if (currentProfile != null)
            {
                lblProfileName.Text = currentProfile.Name;
                lblProfileName.ForeColor = Color.White;
            }
        }

        private void InitializeUI()
        {
            int centerX = contentPanel.ClientSize.Width / 2;

            
            Label lblTitle = new Label
            {
                Text = "🌸 Memory Garden 🌸",
                Font = new Font("Georgia", 36, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(contentPanel.ClientSize.Width, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 120)
            };
            contentPanel.Controls.Add(lblTitle);

            
            lblProfileName = new Label
            {
                Text = currentProfile != null ? currentProfile.Name : "",
                Font = new Font("Georgia", 16, FontStyle.Italic),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(centerX - 200, 260)
            };
            contentPanel.Controls.Add(lblProfileName);

            
            btnPlay = CreateButton("Играть", centerX, 300, 200, 50);
            btnPlay.Click += BtnPlay_Click;
            contentPanel.Controls.Add(btnPlay);

            btnGallery = CreateButton("Галерея", centerX, 360, 200, 50);
            btnGallery.Click += (s, e) => ShowGallery();
            contentPanel.Controls.Add(btnGallery);

            btnSettings = CreateButton("Настройки", centerX, 420, 200, 50);
            btnSettings.Click += (s, e) => ShowSettings();
            contentPanel.Controls.Add(btnSettings);

            btnHowToPlay = CreateButton("Обучение", centerX, 480, 200, 50);
            btnHowToPlay.Click += (s, e) => ShowHowToPlay();
            contentPanel.Controls.Add(btnHowToPlay);

            btnExit = CreateButton("Выход", centerX, 540, 200, 50);
            btnExit.Click += BtnExit_Click;
            contentPanel.Controls.Add(btnExit);

            
            btnProfile = CreateButton("👤 Профиль", 850, 50, 150, 40);
            btnProfile.Font = new Font("Georgia", 12);
            btnProfile.Click += BtnProfile_Click;
            contentPanel.Controls.Add(btnProfile);
        }

        private Button CreateButton(string text, int x, int y, int w, int h)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(w, h),
                Location = new Point(x - w / 2, y),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.White;
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void BtnProfile_Click(object sender, EventArgs e)
        {
            ProfileSelectForm profileForm = new ProfileSelectForm();
            profileForm.ShowDialog(this);

            if (profileForm.SelectedProfileIndex >= 0)
            {
                currentProfileIndex = profileForm.SelectedProfileIndex;
                currentProfile = Profile.Load(currentProfileIndex);
                lblProfileName.Text = currentProfile.Name;
                lblProfileName.ForeColor = Color.White;

                gameSettings.LastProfileIndex = currentProfileIndex;
                gameSettings.Save();
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Сначала выберите профиль!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           

            contentPanel.Controls.Clear();
            LevelSelectForm levelSelect = new LevelSelectForm(currentProfile, currentProfileIndex, contentPanel, this);
            levelSelect.Dock = DockStyle.Fill;
            levelSelect.TopLevel = false;
            levelSelect.FormBorderStyle = FormBorderStyle.None;
            contentPanel.Controls.Add(levelSelect);
            levelSelect.Show();
        }

        private void ShowGallery()
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Сначала выберите профиль!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            contentPanel.Controls.Clear();
            GalleryForm gallery = new GalleryForm(currentProfile, contentPanel, this);
            gallery.Dock = DockStyle.Fill;
            gallery.TopLevel = false;
            gallery.FormBorderStyle = FormBorderStyle.None;
            contentPanel.Controls.Add(gallery);
            gallery.Show();
        }

        private void ShowSettings()
        {
            contentPanel.Controls.Clear();
            SettingsForm settings = new SettingsForm(contentPanel, this);
            settings.Dock = DockStyle.Fill;
            settings.TopLevel = false;
            settings.FormBorderStyle = FormBorderStyle.None;
            contentPanel.Controls.Add(settings);
            settings.Show();
        }

        private void ShowHowToPlay()
        {
            HowToPlayForm howTo = new HowToPlayForm();
            howTo.ShowDialog(this);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            string[] questions = new string[]
            {
                "🌸 Уходишь уже? Сад будет скучать...",
                "🎨 Кроссворды сами себя не разгадают!",
                "💭 Ты точно хочешь оставить цветы нераскрытыми?",
                "🦋 Может, ещё одна головоломка перед уходом?"
            };

            Random rnd = new Random();
            string question = questions[rnd.Next(questions.Length)];

            DialogResult result = MessageBox.Show(
                question + "\n\nТочно выйти?",
                "Прощание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            if (currentProfileIndex >= 0)
            {
                gameSettings.LastProfileIndex = currentProfileIndex;
                gameSettings.Save();
            }

            string[] questions = new string[]
            {
                "🌸 Уходишь уже? Сад будет скучать...",
                "🎨 Кроссворды сами себя не разгадают!",
                "💭 Ты точно хочешь оставить цветы нераскрытыми?",
                "🦋 Может, ещё одна головоломка перед уходом?"
            };

            Random rnd = new Random();
            string question = questions[rnd.Next(questions.Length)];

            DialogResult result = MessageBox.Show(
                question + "\n\nТочно выйти?",
                "Прощание",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}