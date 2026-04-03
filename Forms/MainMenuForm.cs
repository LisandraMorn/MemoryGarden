using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class MainMenuForm : Form
    {
        private Profile currentProfile;
        private int currentProfileIndex = -1;

        private Button btnPlay;
        private Button btnGallery;
        private Button btnSettings;
        private Button btnExit;
        private Button btnProfile;
        private Label lblProfileName;

        public MainMenuForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Nonogram Garden";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 248, 220);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            // Заголовок
            Label lblTitle = new Label
            {
                Text = "🌸 Nonogram Garden 🌸",
                Font = new Font("Georgia", 36, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(centerX - 200, 150)
            };
            this.Controls.Add(lblTitle);

            // Кнопки меню
            btnPlay = CreateButton("Играть", centerX, 300, 200, 50);
            btnPlay.Click += BtnPlay_Click;

            btnGallery = CreateButton("Галерея", centerX, 360, 200, 50);
            btnGallery.Click += (s, e) => OpenGallery();

            btnSettings = CreateButton("Настройки", centerX, 420, 200, 50);
            btnSettings.Click += (s, e) => OpenSettings();

            btnExit = CreateButton("Выход", centerX, 480, 200, 50);
            btnExit.Click += BtnExit_Click;

            // Кнопка профиля
            btnProfile = CreateButton("👤 Профиль", 850, 50, 150, 40);
            btnProfile.Click += BtnProfile_Click;

            // Отображение имени профиля
            lblProfileName = new Label
            {
                Text = "Профиль не выбран",
                Font = new Font("Georgia", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(850, 100)
            };

            this.Controls.Add(btnPlay);
            this.Controls.Add(btnGallery);
            this.Controls.Add(btnSettings);
            this.Controls.Add(btnExit);
            this.Controls.Add(btnProfile);
            this.Controls.Add(lblProfileName);
        }

        private Button CreateButton(string text, int x, int y, int w, int h)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(w, h),
                Location = new Point(x - w / 2, y),
                BackColor = Color.FromArgb(255, 182, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(255, 105, 180);
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
                lblProfileName.Text = $"Профиль: {currentProfile.Name}";
                lblProfileName.ForeColor = Color.FromArgb(100, 100, 100);
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

            // Показываем туториал если первый раз
            if (!currentProfile.HasSeenTutorial1)
            {
                TutorialForm tutorial1 = new TutorialForm(1);
                tutorial1.ShowDialog(this);
                currentProfile.HasSeenTutorial1 = true;
                currentProfile.Save(currentProfileIndex);
            }

            if (!currentProfile.HasSeenTutorial2)
            {
                TutorialForm tutorial2 = new TutorialForm(2);
                tutorial2.ShowDialog(this);
                currentProfile.HasSeenTutorial2 = true;
                currentProfile.Save(currentProfileIndex);
            }

            // Переход к выбору уровня
            LevelSelectForm levelForm = new LevelSelectForm(currentProfile, currentProfileIndex);
            levelForm.ShowDialog(this);

            // Обновляем профиль после игры
            currentProfile = Profile.Load(currentProfileIndex);
        }

        private void OpenGallery()
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Сначала выберите профиль!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GalleryForm gallery = new GalleryForm(currentProfile);
            gallery.ShowDialog(this);
        }

        private void OpenSettings()
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog(this);
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
    }
}