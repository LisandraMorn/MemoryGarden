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
        private TabControl tabControl;

        public LevelSelectForm(Profile prof, int profIndex)
        {
            profile = prof;
            profileIndex = profIndex;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Выбор уровня";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);

            Label lblTitle = new Label
            {
                Text = "🌺 Выбери уровень",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(350, 20)
            };

            tabControl = new TabControl
            {
                Size = new Size(800, 500),
                Location = new Point(50, 70),
                Font = new Font("Georgia", 12)
            };

            for (int i = 1; i <= 6; i++)
            {
                TabPage tab = new TabPage(GetStars(i));
                tab.BackColor = Color.FromArgb(255, 248, 220);

                FlowLayoutPanel panel = new FlowLayoutPanel
                {
                    Size = new Size(780, 450),
                    Location = new Point(10, 10),
                    AutoScroll = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true
                };

                int levelsPerDifficulty = 5;
                for (int level = 0; level < levelsPerDifficulty; level++)
                {
                    int levelId = (i - 1) * levelsPerDifficulty + level;
                    Button levelBtn = CreateLevelButton(levelId, i);
                    panel.Controls.Add(levelBtn);
                }

                tab.Controls.Add(panel);
                tabControl.TabPages.Add(tab);
            }

            Button btnBack = new Button
            {
                Text = "← Назад",
                Font = new Font("Georgia", 12),
                Size = new Size(120, 40),
                Location = new Point(50, 580),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.FlatAppearance.BorderSize = 2;
            btnBack.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(tabControl);
            this.Controls.Add(btnBack);
        }

        private string GetStars(int count)
        {
            return new string('⭐', count);
        }

        private Button CreateLevelButton(int levelId, int difficulty)
        {
            bool isUnlocked = profile.UnlockedLevels.Contains(levelId);
            bool isCompleted = profile.CompletedLevels.Contains(levelId);

            Button btn = new Button
            {
                Size = new Size(150, 150),
                Margin = new Padding(10),
                BackColor = isCompleted ? Color.FromArgb(144, 238, 144) :
                           isUnlocked ? Color.FromArgb(255, 182, 193) :
                           Color.FromArgb(200, 200, 200),
                ForeColor = isCompleted ? Color.White : Color.FromArgb(100, 100, 100),
                FlatStyle = FlatStyle.Flat,
                Enabled = isUnlocked,
                Font = new Font("Georgia", 11, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 3;
            btn.FlatAppearance.BorderColor = isCompleted ? Color.FromArgb(34, 139, 34) :
                                          isUnlocked ? Color.FromArgb(255, 105, 180) :
                                          Color.FromArgb(150, 150, 150);

            if (isUnlocked)
            {
                btn.Text = $"Уровень {levelId + 1}\n\n14×15\n{GetStars(difficulty)}";
                btn.Click += (s, e) => StartLevel(levelId, difficulty);
            }
            else
            {
                btn.Text = "🔒\nЗакрыто";
            }

            btn.Cursor = isUnlocked ? Cursors.Hand : Cursors.Default;
            return btn;
        }

        private void StartLevel(int levelId, int difficulty)
        {
            Puzzle puzzle = CreateTestPuzzle(levelId, difficulty);

            GamePlayForm game = new GamePlayForm(puzzle, profile, profileIndex);
            game.ShowDialog(this);

            if (game.IsCompleted && !profile.CompletedLevels.Contains(levelId))
            {
                profile.CompletedLevels.Add(levelId);
                if (levelId + 1 < 30)
                    profile.UnlockedLevels.Add(levelId + 1);
                profile.Save(profileIndex);
            }
        }

        private Puzzle CreateTestPuzzle(int id, int difficulty)
        {
            Puzzle puzzle = new Puzzle
            {
                ID = id,
                Width = 14,
                Height = 15,
                Difficulty = difficulty,
                Name = $"Уровень {id + 1}",
                Colors = new Color[]
                {
                    Color.White,
                    Color.FromArgb(70, 130, 180),
                    Color.FromArgb(220, 20, 60),
                    Color.FromArgb(60, 179, 113),
                    Color.FromArgb(255, 215, 0),
                    Color.FromArgb(148, 0, 211),
                    Color.FromArgb(255, 140, 0)
                }
            };

            for (int y = 0; y < puzzle.Height; y++)
            {
                for (int x = 0; x < puzzle.Width; x++)
                {
                    puzzle.Solution[y, x] = 0;
                }
            }

            puzzle.HintsHorizontal = GenerateHints(puzzle.Solution, puzzle.Height, puzzle.Width, true);
            puzzle.HintsVertical = GenerateHints(puzzle.Solution, puzzle.Width, puzzle.Height, false);

            return puzzle;
        }

        private int[,] GenerateHints(int[,] data, int rows, int cols, bool horizontal)
        {
            int[,] hints = new int[rows, 10];

            for (int i = 0; i < rows; i++)
            {
                int hintIndex = 0;
                int count = 0;
                int lastColor = 0;

                for (int j = 0; j < cols; j++)
                {
                    int val = horizontal ? data[i, j] : data[j, i];

                    if (val != 0)
                    {
                        if (val == lastColor)
                        {
                            count++;
                        }
                        else
                        {
                            if (count > 0)
                            {
                                hints[i, hintIndex++] = lastColor * 100 + count;
                            }
                            count = 1;
                            lastColor = val;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            hints[i, hintIndex++] = lastColor * 100 + count;
                            count = 0;
                            lastColor = 0;
                        }
                    }
                }

                if (count > 0)
                {
                    hints[i, hintIndex++] = lastColor * 100 + count;
                }
            }

            return hints;
        }
        public class LevelSelectForm : Form
        {
            private Profile profile;
            private int profileIndex;
            private Panel parentPanel;
            private Form mainForm;
            private TabControl tabControl;

            // Новый конструктор для встраивания
            public LevelSelectForm(Profile prof, int profIndex, Panel panel, Form main)
            {
                profile = prof;
                profileIndex = profIndex;
                parentPanel = panel;
                mainForm = main;
                InitializeUI();
            }

            // Старый конструктор
            public LevelSelectForm(Profile prof, int profIndex)
            {
                profile = prof;
                profileIndex = profIndex;
                InitializeUI();
            }

            private void InitializeUI()
            {
                this.Text = "Выбор уровня";
                this.Size = new Size(900, 650);
                this.StartPosition = FormStartPosition.CenterParent;
                this.BackColor = Color.FromArgb(255, 248, 220);

                Label lblTitle = new Label
                {
                    Text = "🌺 Выбери уровень",
                    Font = new Font("Georgia", 24, FontStyle.Bold),
                    ForeColor = Color.FromArgb(255, 105, 180),
                    AutoSize = true,
                    Location = new Point(350, 20)
                };

                tabControl = new TabControl
                {
                    Size = new Size(800, 500),
                    Location = new Point(50, 70),
                    Font = new Font("Georgia", 12)
                };

                for (int i = 1; i <= 6; i++)
                {
                    TabPage tab = new TabPage(GetStars(i));
                    tab.BackColor = Color.FromArgb(255, 248, 220);

                    FlowLayoutPanel panel = new FlowLayoutPanel
                    {
                        Size = new Size(780, 450),
                        Location = new Point(10, 10),
                        AutoScroll = true,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true
                    };

                    int levelsPerDifficulty = 5;
                    for (int level = 0; level < levelsPerDifficulty; level++)
                    {
                        int levelId = (i - 1) * levelsPerDifficulty + level;
                        Button levelBtn = CreateLevelButton(levelId, i);
                        panel.Controls.Add(levelBtn);
                    }

                    tab.Controls.Add(panel);
                    tabControl.TabPages.Add(tab);
                }

                Button btnBack = new Button
                {
                    Text = "← В главное меню",
                    Font = new Font("Georgia", 12),
                    Size = new Size(180, 40),
                    Location = new Point(50, 580),
                    BackColor = Color.FromArgb(150, 150, 150),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnBack.FlatAppearance.BorderSize = 2;
                btnBack.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                btnBack.Cursor = Cursors.Hand;
                btnBack.Click += (s, e) => GoBackToMenu();

                this.Controls.Add(lblTitle);
                this.Controls.Add(tabControl);
                this.Controls.Add(btnBack);
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
            }

            private string GetStars(int count)
            {
                return new string('⭐', count);
            }

            private Button CreateLevelButton(int levelId, int difficulty)
            {
                bool isUnlocked = profile.UnlockedLevels.Contains(levelId);
                bool isCompleted = profile.CompletedLevels.Contains(levelId);

                Button btn = new Button
                {
                    Size = new Size(150, 150),
                    Margin = new Padding(10),
                    BackColor = isCompleted ? Color.FromArgb(144, 238, 144) :
                               isUnlocked ? Color.FromArgb(255, 182, 193) :
                               Color.FromArgb(200, 200, 200),
                    ForeColor = isCompleted ? Color.White : Color.FromArgb(100, 100, 100),
                    FlatStyle = FlatStyle.Flat,
                    Enabled = isUnlocked,
                    Font = new Font("Georgia", 11, FontStyle.Bold)
                };

                btn.FlatAppearance.BorderSize = 3;
                btn.FlatAppearance.BorderColor = isCompleted ? Color.FromArgb(34, 139, 34) :
                                              isUnlocked ? Color.FromArgb(255, 105, 180) :
                                              Color.FromArgb(150, 150, 150);

                if (isUnlocked)
                {
                    btn.Text = $"Уровень {levelId + 1}\n\n14×15\n{GetStars(difficulty)}";
                    btn.Click += (s, e) => StartLevel(levelId, difficulty);
                }
                else
                {
                    btn.Text = "🔒\nЗакрыто";
                }

                btn.Cursor = isUnlocked ? Cursors.Hand : Cursors.Default;
                return btn;
            }

            private void StartLevel(int levelId, int difficulty)
            {
                Puzzle puzzle = CreateTestPuzzle(levelId, difficulty);

                GamePlayForm game = new GamePlayForm(puzzle, profile, profileIndex);
                game.ShowDialog();

                if (game.IsCompleted && !profile.CompletedLevels.Contains(levelId))
                {
                    profile.CompletedLevels.Add(levelId);
                    if (levelId + 1 < 30)
                        profile.UnlockedLevels.Add(levelId + 1);
                    profile.Save(profileIndex);
                }
            }

            private Puzzle CreateTestPuzzle(int id, int difficulty)
            {
                Puzzle puzzle = new Puzzle
                {
                    ID = id,
                    Width = 14,
                    Height = 15,
                    Difficulty = difficulty,
                    Name = $"Уровень {id + 1}",
                    Colors = new Color[]
                    {
                Color.White,
                Color.FromArgb(70, 130, 180),
                Color.FromArgb(220, 20, 60),
                Color.FromArgb(60, 179, 113),
                Color.FromArgb(255, 215, 0),
                Color.FromArgb(148, 0, 211),
                Color.FromArgb(255, 140, 0)
                    }
                };

                for (int y = 0; y < puzzle.Height; y++)
                {
                    for (int x = 0; x < puzzle.Width; x++)
                    {
                        puzzle.Solution[y, x] = 0;
                    }
                }

                puzzle.HintsHorizontal = GenerateHints(puzzle.Solution, puzzle.Height, puzzle.Width, true);
                puzzle.HintsVertical = GenerateHints(puzzle.Solution, puzzle.Width, puzzle.Height, false);

                return puzzle;
            }

            private int[,] GenerateHints(int[,] data, int rows, int cols, bool horizontal)
            {
                int[,] hints = new int[rows, 10];

                for (int i = 0; i < rows; i++)
                {
                    int hintIndex = 0;
                    int count = 0;
                    int lastColor = 0;

                    for (int j = 0; j < cols; j++)
                    {
                        int val = horizontal ? data[i, j] : data[j, i];

                        if (val != 0)
                        {
                            if (val == lastColor)
                            {
                                count++;
                            }
                            else
                            {
                                if (count > 0)
                                {
                                    hints[i, hintIndex++] = lastColor * 100 + count;
                                }
                                count = 1;
                                lastColor = val;
                            }
                        }
                        else
                        {
                            if (count > 0)
                            {
                                hints[i, hintIndex++] = lastColor * 100 + count;
                                count = 0;
                                lastColor = 0;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        hints[i, hintIndex++] = lastColor * 100 + count;
                    }
                }

                return hints;
            }
        }
    }
}