using Memory.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class PauseMenuForm : Form
    {
        public DialogResult Result { get; private set; } = DialogResult.None;

        private Form1 mainForm;
        private Profile profile;
        private int profileIndex;
        private Panel contentPanel;
        private PlatformerLevel gameLevel;

        public PauseMenuForm(Form1 main, Profile prof, int profIdx, Panel panel, PlatformerLevel level)
        {
            mainForm = main;
            profile = prof;
            profileIndex = profIdx;
            contentPanel = panel;
            gameLevel = level;

            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Пауза";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;

            
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            
            Panel border = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Padding = new Padding(2)
            };
            border.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, border.ClientRectangle,
                    Color.White, ButtonBorderStyle.Solid);
            };

            
            Label lblTitle = new Label
            {
                Text = "⏸ Пауза",
                Font = new Font("Georgia", 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            int btnWidth = 280;
            int btnHeight = 45;
            int startX = (400 - btnWidth) / 2;
            int startY = 30;

            Button btnContinue = CreateMenuButton("▶ Продолжить", startX, startY, btnWidth, btnHeight);
            btnContinue.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            Button btnSettings = CreateMenuButton("⚙ Настройки", startX, startY + 55, btnWidth, btnHeight);
            btnSettings.Click += (s, e) =>
            {
                SettingsForm settings = new SettingsForm();
                settings.ShowDialog(this);
            };

            Button btnGallery = CreateMenuButton("🖼 Галерея", startX, startY + 110, btnWidth, btnHeight);
            btnGallery.Click += (s, e) =>
            {
                GalleryForm gallery = new GalleryForm(profile);
                gallery.ShowDialog(this);
            };

            Button btnHowTo = CreateMenuButton("❔ Обучение", startX, startY + 165, btnWidth, btnHeight);
            btnHowTo.Click += (s, e) =>
            {
                HowToPlayForm howTo = new HowToPlayForm();
                howTo.ShowDialog(this);
            };

            Button btnMainMenu = CreateMenuButton("🏠 Главное меню", startX, startY + 220, btnWidth, btnHeight);
            btnMainMenu.Click += (s, e) =>
            {
                var result = MessageBox.Show("Вернуться в главное меню?\nНесохранённый прогресс будет потерян.",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                }
            };

            Button btnExit = CreateMenuButton("🚪 Выход", startX, startY + 275, btnWidth, btnHeight);
            btnExit.Click += (s, e) =>
            {
                var result = MessageBox.Show("Выйти из игры?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };

            buttonPanel.Controls.Add(btnContinue);
            buttonPanel.Controls.Add(btnSettings);
            buttonPanel.Controls.Add(btnGallery);
            buttonPanel.Controls.Add(btnHowTo);
            buttonPanel.Controls.Add(btnMainMenu);
            buttonPanel.Controls.Add(btnExit);

            border.Controls.Add(buttonPanel);
            border.Controls.Add(lblTitle);
            this.Controls.Add(border);

           
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
        }

        private Button CreateMenuButton(string text, int x, int y, int w, int h)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Georgia", 12, FontStyle.Bold),
                Size = new Size(w, h),
                Location = new Point(x, y),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.White;
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}