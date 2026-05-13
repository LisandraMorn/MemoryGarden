using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class LevelSelectForm : Form
    {
        private Profile profile;
        private int profileIndex;
        private Panel parentPanel;
        private Form mainForm;
        private Panel[] tabPanels;
        private Button[] tabButtons;
        private int currentTabIndex = 0;

        private string[] tabNames = { "Ребёнок", "Молодая", "Взрослая" };

        public LevelSelectForm(Profile prof, int profIdx, Panel panel, Form main)
        {
            profile = prof;
            profileIndex = profIdx;
            parentPanel = panel;
            mainForm = main;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Выбор уровня";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;

            Label lblTitle = new Label
            {
                Text = "Выбери этап",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(350, 20)
            };

            Panel tabsPanel = new Panel
            {
                Size = new Size(800, 40),
                Location = new Point(50, 70),
                BackColor = Color.Black
            };

            tabButtons = new Button[3];
            tabPanels = new Panel[3];

            for (int i = 0; i < 3; i++)
            {
                tabButtons[i] = new Button
                {
                    Text = tabNames[i],
                    Size = new Size(266, 40),
                    Location = new Point(i * 266, 0),
                    Font = new Font("Georgia", 11, FontStyle.Bold),
                    BackColor = i == 0 ? Color.FromArgb(60, 60, 60) : Color.Black,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                tabButtons[i].FlatAppearance.BorderSize = 2;
                tabButtons[i].FlatAppearance.BorderColor = Color.White;
                tabButtons[i].Cursor = Cursors.Hand;
                int tabIndex = i;
                tabButtons[i].Click += (s, e) => SwitchTab(tabIndex);
                tabsPanel.Controls.Add(tabButtons[i]);

                tabPanels[i] = CreateLevelsPanel(i);
                tabPanels[i].Visible = (i == 0);
                tabPanels[i].Location = new Point(50, 120);
            }

            this.Controls.Add(tabsPanel);
            for (int i = 0; i < 3; i++)
                this.Controls.Add(tabPanels[i]);

            Button btnBack = new Button
            {
                Text = "← Назад",
                Font = new Font("Georgia", 12),
                Size = new Size(180, 40),
                Location = new Point(50, 580),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.FlatAppearance.BorderSize = 2;
            btnBack.FlatAppearance.BorderColor = Color.White;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => GoBackToMenu();

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnBack);
        }

        private Panel CreateLevelsPanel(int difficulty)
        {
            Panel panel = new Panel
            {
                Size = new Size(800, 450),
                BackColor = Color.Black
            };

            FlowLayoutPanel flowPanel = new FlowLayoutPanel
            {
                Size = new Size(800, 450),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Black
            };

            int levelsPerTab = 3; 

            for (int level = 0; level < levelsPerTab; level++)
            {
                int levelId = difficulty * levelsPerTab + level;
                Button levelBtn = CreateLevelButton(levelId);
                flowPanel.Controls.Add(levelBtn);
            }

            panel.Controls.Add(flowPanel);
            return panel;
        }

        private Button CreateLevelButton(int levelId)
        {
            bool isUnlocked = profile.UnlockedLevels.Contains(levelId) || levelId == 0;
            bool isCompleted = profile.CompletedLevels.Contains(levelId);

            Button btn = new Button
            {
                Size = new Size(150, 150),
                Margin = new Padding(10),
                BackColor = isUnlocked ? Color.Black : Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 18, FontStyle.Bold),
                Text = (levelId % 4 + 1).ToString(),
                Enabled = isUnlocked,
                Cursor = isUnlocked ? Cursors.Hand : Cursors.Default
            };
            btn.FlatAppearance.BorderSize = 3;
            btn.FlatAppearance.BorderColor = Color.White;

            if (isCompleted)
            {
                btn.BackColor = Color.FromArgb(40, 40, 40);
                btn.Text += " ✓";
            }

            btn.Click += (s, e) =>
            {
                if (isCompleted)
                {
                    string story = GetStoryForLevel(levelId);
                    if (!string.IsNullOrEmpty(story))
                    {
                        StoryForm storyForm = new StoryForm(story);
                        storyForm.ShowDialog(this);
                    }
                }
                else
                {
                    StartLevel(levelId);
                }
            };

            return btn;
        }

        private void StartLevel(int levelId)
        {
            if (parentPanel != null && mainForm != null)
            {
                parentPanel.Controls.Clear();
                PlatformerLevel level = CreateLevelById(levelId);
                level.Dock = DockStyle.Fill;
                level.TopLevel = false;
                level.FormBorderStyle = FormBorderStyle.None;
                parentPanel.Controls.Add(level);
                level.Show();
            }
            else
            {
                PlatformerLevel level = CreateLevelById(levelId);
                level.ShowDialog();
            }
        }

        private PlatformerLevel CreateLevelById(int id)
        {
            switch (id)
            {
                case 0: return new Level1(profile, profileIndex, parentPanel, mainForm, id);
                case 1: return new Level2(profile, profileIndex, parentPanel, mainForm, id);
                // case 2: return new Level3(profile, profileIndex, parentPanel, mainForm, id);
                // ...
                default: return new Level1(profile, profileIndex, parentPanel, mainForm, id);
            }
        }

        private void SwitchTab(int tabIndex)
        {
            for (int i = 0; i < 3; i++)
            {
                tabPanels[i].Visible = false;
                tabButtons[i].BackColor = Color.Black;
            }
            tabPanels[tabIndex].Visible = true;
            tabButtons[tabIndex].BackColor = Color.FromArgb(60, 60, 60);
            currentTabIndex = tabIndex;
        }

        private void GoBackToMenu()
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

        private string GetStoryForLevel(int levelId)
        {
            switch (levelId)
            {
                case 0: return "Пока я рисовала дом, я по неосторожности пролила на него какао. Мама аккуратно очистила бумагу и просушила, но разводы от какао остались.";
                case 1: return "История для уровня 2...";
                case 2: return "История для уровня 3...";
                case 3: return "История для уровня 4...";
                default: return "Воспоминание пока скрыто.";
            }
        }
    }
}