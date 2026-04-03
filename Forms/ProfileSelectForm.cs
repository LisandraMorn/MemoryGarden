using Memory.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class ProfileSelectForm : Form
    {
        public int SelectedProfileIndex { get; private set; } = -1;

        private TextBox[] nameInputs;
        private Button[] selectButtons;
        private Button[] deleteButtons;

        public ProfileSelectForm()
        {
            SelectedProfileIndex = -1;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Выбор профиля";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitle = new Label
            {
                Text = "🌸 Выберите профиль",
                Font = new Font("Georgia", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(100, 30)
            };

            nameInputs = new TextBox[3];
            selectButtons = new Button[3];
            deleteButtons = new Button[3];

            for (int i = 0; i < 3; i++)
            {
                int profileIndex = i;
                int y = 100 + i * 80;

                nameInputs[i] = new TextBox
                {
                    Size = new Size(200, 30),
                    Location = new Point(50, y),
                    Font = new Font("Georgia", 12),
                    MaxLength = 20,
                    TextAlign = HorizontalAlignment.Center
                };

                if (Profile.Exists(i + 1))
                {
                    Profile p = Profile.Load(i + 1);
                    nameInputs[i].Text = p.Name;
                }
                else
                {
                    nameInputs[i].Text = $"Профиль {i + 1}";
                }

                selectButtons[i] = new Button
                {
                    Text = "Выбрать",
                    Size = new Size(100, 35),
                    Location = new Point(270, y + 5),
                    BackColor = Color.FromArgb(122, 138, 87),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Georgia", 10, FontStyle.Bold)
                };
                selectButtons[i].FlatAppearance.BorderSize = 2;
                selectButtons[i].FlatAppearance.BorderColor = Color.FromArgb(34, 139, 34);
                selectButtons[i].Cursor = Cursors.Hand;
                selectButtons[i].Click += (s, e) => SelectProfile(profileIndex);

                deleteButtons[i] = new Button
                {
                    Text = "Удалить",
                    Size = new Size(80, 35),
                    Location = new Point(380, y + 5),
                    BackColor = Color.FromArgb(138, 87, 105),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Georgia", 10)
                };
                deleteButtons[i].FlatAppearance.BorderSize = 2;
                deleteButtons[i].FlatAppearance.BorderColor = Color.FromArgb(220, 20, 60);
                deleteButtons[i].Cursor = Cursors.Hand;
                deleteButtons[i].Click += (s, e) => DeleteProfile(profileIndex);

                this.Controls.Add(nameInputs[i]);
                this.Controls.Add(selectButtons[i]);
                this.Controls.Add(deleteButtons[i]);
            }

            this.Controls.Add(lblTitle);
        }

        private void SelectProfile(int index)
        {
            string name = nameInputs[index].Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                ShowCenteredMessage("Введите имя профиля!", "Ошибка", MessageBoxIcon.Warning);
                return;
            }

            Profile profile = new Profile();
            profile.Name = name;
            profile.CompletedLevels = new List<int>();
            profile.UnlockedLevels = new List<int> { 0 };
            profile.HasSeenTutorial1 = false;
            profile.HasSeenTutorial2 = false;

            profile.Save(index + 1);

            SelectedProfileIndex = index + 1;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DeleteProfile(int index)
        {
            if (Profile.Exists(index + 1))
            {
                DialogResult result = MessageBox.Show(
                    "Удалить этот профиль?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    Profile.Delete(index + 1);
                    nameInputs[index].Text = $"Профиль {index + 1}";
                }
            }
        }
        private void ShowCenteredMessage(string message, string title, MessageBoxIcon icon)
        {
            // Создаём форму для центрирования MessageBox
            Form parent = new Form
            {
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(1, 1),
                ShowInTaskbar = false,
                Opacity = 0
            };

            parent.Show();
            MessageBox.Show(parent, message, title, MessageBoxButtons.OK, icon);
            parent.Close();
        }
    }
}
   