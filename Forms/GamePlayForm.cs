using System;
using System.Drawing;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public class GamePlayForm : Form
    {
        public bool IsCompleted { get; private set; } = false;

        private Puzzle puzzle;
        private Profile profile;
        private int profileIndex;
        private int[,] playerGrid;
        private int selectedColor = 1;

        private Panel gamePanel;
        private FlowLayoutPanel colorPalette;
        private Button btnCheck;
        private Button btnBack;

        public GamePlayForm(Puzzle pz, Profile prof, int profIndex)
        {
            puzzle = pz;
            profile = prof;
            profileIndex = profIndex;
            playerGrid = new int[puzzle.Height, puzzle.Width];
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = puzzle.Name;
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);

            // Панель игры (адаптивная)
            int cellSize = 30;
            int boardWidth = puzzle.Width * cellSize;
            int boardHeight = puzzle.Height * cellSize;

            gamePanel = new Panel
            {
                Size = new Size(boardWidth + 200, boardHeight + 100),
                Location = new Point(200, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Рисуем сетку
            DrawGrid(cellSize);

            // Палитра цветов
            colorPalette = new FlowLayoutPanel
            {
                Size = new Size(60, boardHeight),
                Location = new Point(130, 50),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            for (int i = 1; i < puzzle.Colors.Length; i++)
            {
                Button colorBtn = new Button
                {
                    Size = new Size(50, 50),
                    BackColor = puzzle.Colors[i],
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5)
                };
                colorBtn.FlatAppearance.BorderSize = i == selectedColor ? 3 : 1;
                colorBtn.FlatAppearance.BorderColor = Color.Black;
                colorBtn.Click += (s, e) => SelectColor(i, colorBtn);
                colorPalette.Controls.Add(colorBtn);
            }

            // Кнопка проверки
            btnCheck = new Button
            {
                Text = "✓ Проверка",
                Font = new Font("Georgia", 12, FontStyle.Bold),
                Size = new Size(150, 40),
                Location = new Point(200, 700),
                BackColor = Color.FromArgb(144, 238, 144),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCheck.Click += BtnCheck_Click;

            // Кнопка назад
            btnBack = new Button
            {
                Text = "← Выход",
                Font = new Font("Georgia", 12),
                Size = new Size(120, 40),
                Location = new Point(850, 700),
                BackColor = Color.FromArgb(255, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(gamePanel);
            this.Controls.Add(colorPalette);
            this.Controls.Add(btnCheck);
            this.Controls.Add(btnBack);
        }

        private void DrawGrid(int cellSize)
        {
            // Рисуем числа и сетку
            gamePanel.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                int offsetX = 150; // Место для чисел слева
                int offsetY = 50;  // Место для чисел сверху

                // Числа слева
                for (int y = 0; y < puzzle.Height; y++)
                {
                    int hintCount = 0;
                    for (int h = 0; h < 10; h++)
                    {
                        if (puzzle.HintsHorizontal[y, h] > 0) hintCount++;
                    }

                    for (int h = 0; h < hintCount; h++)
                    {
                        int val = puzzle.HintsHorizontal[y, h];
                        int color = val / 100;
                        int count = val % 100;

                        using (Brush brush = new SolidBrush(puzzle.Colors[color]))
                        {
                            g.DrawString(count.ToString(), new Font("Arial", 10), brush,
                                new PointF(10, offsetY + y * cellSize + h * 12));
                        }
                    }
                }

                // Числа сверху
                for (int x = 0; x < puzzle.Width; x++)
                {
                    int hintCount = 0;
                    for (int h = 0; h < 10; h++)
                    {
                        if (puzzle.HintsVertical[x, h] > 0) hintCount++;
                    }

                    for (int h = 0; h < hintCount; h++)
                    {
                        int val = puzzle.HintsVertical[x, h];
                        int color = val / 100;
                        int count = val % 100;

                        using (Brush brush = new SolidBrush(puzzle.Colors[color]))
                        {
                            g.DrawString(count.ToString(), new Font("Arial", 10), brush,
                                new PointF(offsetX + x * cellSize + 5, h * 12));
                        }
                    }
                }

                // Сетка
                for (int y = 0; y <= puzzle.Height; y++)
                {
                    Pen pen = (y % 5 == 0) ? new Pen(Color.Black, 2) : new Pen(Color.LightGray, 1);
                    g.DrawLine(pen, offsetX, offsetY + y * cellSize,
                              offsetX + puzzle.Width * cellSize, offsetY + y * cellSize);
                }

                for (int x = 0; x <= puzzle.Width; x++)
                {
                    Pen pen = (x % 5 == 0) ? new Pen(Color.Black, 2) : new Pen(Color.LightGray, 1);
                    g.DrawLine(pen, offsetX + x * cellSize, offsetY,
                              offsetX + x * cellSize, offsetY + puzzle.Height * cellSize);
                }

                // Клетки игрока
                for (int y = 0; y < puzzle.Height; y++)
                {
                    for (int x = 0; x < puzzle.Width; x++)
                    {
                        if (playerGrid[y, x] > 0)
                        {
                            using (Brush brush = new SolidBrush(puzzle.Colors[playerGrid[y, x]]))
                            {
                                g.FillRectangle(brush, offsetX + x * cellSize + 1,
                                    offsetY + y * cellSize + 1, cellSize - 2, cellSize - 2);
                            }
                        }
                    }
                }
            };

            // Клик по сетке
            gamePanel.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                   
                    int offsetX = 150;
                    int offsetY = 50;

                    int x = (e.X - offsetX) / cellSize;
                    int y = (e.Y - offsetY) / cellSize;

                    if (x >= 0 && x < puzzle.Width && y >= 0 && y < puzzle.Height)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            playerGrid[y, x] = selectedColor;
                        }
                        else // ПКМ - крестик
                        {
                            playerGrid[y, x] = -1; // -1 = крестик
                        }
                        gamePanel.Invalidate();
                    }
                }
            };
        }

        private void SelectColor(int color, Button btn)
        {
            selectedColor = color;

            // Обновляем обводку кнопок
            foreach (Control c in colorPalette.Controls)
            {
                if (c is Button b)
                {
                    b.FlatAppearance.BorderSize = 1;
                }
            }
            btn.FlatAppearance.BorderSize = 3;
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            bool allCorrect = true;

            for (int y = 0; y < puzzle.Height; y++)
            {
                for (int x = 0; x < puzzle.Width; x++)
                {
                    int playerVal = playerGrid[y, x] > 0 ? playerGrid[y, x] : 0;
                    if (playerVal != puzzle.Solution[y, x])
                    {
                        allCorrect = false;
                        break;
                    }
                }
            }

            if (allCorrect)
            {
                IsCompleted = true;
                MessageBox.Show("🎉 Поздравляем! Уровень пройден!", "Победа!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Есть ошибки. Попробуй ещё!", "Неверно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}