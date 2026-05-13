using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Memory.Data;

namespace Memory
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
        }
    }

    public class GamePlayForm : Form
    {
        public bool IsCompleted { get; private set; } = false;

        private Puzzle puzzle;
        private Profile profile;
        private int profileIndex;
        private int[,] playerGrid;
        private int selectedColor = 1;
        private Panel parentPanel;
        private Form mainForm;
        private int cellSize;
        private int hintBoxSize;
        private int leftSpace;
        private int topSpace;
        private int offsetX;
        private int offsetY;

        private Panel gamePanel;
        private FlowLayoutPanel colorPalette;
        private Button btnCheck;
        private Button btnBack;

        private int maxHorizontalHints;
        private int maxVerticalHints;

        private bool isPainting = false;
        private int lastPaintedX = -1;
        private int lastPaintedY = -1;

        private HashSet<string> completedHorizontalHints = new HashSet<string>();
        private HashSet<string> completedVerticalHints = new HashSet<string>();

        public GamePlayForm(Puzzle pz, Profile prof, int profIndex, Panel panel = null, Form main = null)
        {
            puzzle = pz;
            profile = prof;
            this.profileIndex = profIndex;
            parentPanel = panel;
            mainForm = main;
            playerGrid = new int[puzzle.Height, puzzle.Width];
            InitializeUI();
        }

        public GamePlayForm(Puzzle pz, Profile prof, int profIndex) : this(pz, prof, profIndex, null, null)
        {
        }

        private void InitializeUI()
        {
            this.Text = puzzle.Name;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);

            this.Resize += GamePlayForm_Resize;

            colorPalette = new FlowLayoutPanel
            {
                Size = new Size(400, 50),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Black
            };

            for (int i = 1; i < puzzle.Colors.Length; i++)
            {
                Button colorBtn = new Button
                {
                    Size = new Size(35, 35),
                    BackColor = puzzle.Colors[i],
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(3)
                };
                colorBtn.FlatAppearance.BorderSize = i == selectedColor ? 3 : 1;
                colorBtn.FlatAppearance.BorderColor = Color.White;
                colorBtn.Cursor = Cursors.Hand;
                int colorIndex = i;
                colorBtn.Click += (s, e) => SelectColor(colorIndex, colorBtn);
                colorPalette.Controls.Add(colorBtn);
            }
            this.Controls.Add(colorPalette);

            btnCheck = new Button
            {
                Text = "✓ Проверка",
                Font = new Font("Georgia", 12, FontStyle.Bold),
                Size = new Size(150, 45),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCheck.FlatAppearance.BorderSize = 2;
            btnCheck.FlatAppearance.BorderColor = Color.White;
            btnCheck.Cursor = Cursors.Hand;
            btnCheck.Click += BtnCheck_Click;

            btnBack = new Button
            {
                Text = "← Назад",
                Font = new Font("Georgia", 12),
                Size = new Size(130, 45),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.FlatAppearance.BorderSize = 2;
            btnBack.FlatAppearance.BorderColor = Color.White;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => GoBack();

            this.Controls.Add(btnCheck);
            this.Controls.Add(btnBack);

            gamePanel = new DoubleBufferedPanel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(gamePanel);
        }

        private void CalculateLayout()
        {
            int maxHintsH = puzzle.HintsHorizontal.GetLength(1);
            int maxHintsV = puzzle.HintsVertical.GetLength(1);

            maxHorizontalHints = 0;
            maxVerticalHints = 0;

            for (int y = 0; y < puzzle.Height; y++)
            {
                int count = 0;
                for (int h = 0; h < maxHintsH; h++)
                    if (puzzle.HintsHorizontal[y, h] > 0) count++;
                if (count > maxHorizontalHints) maxHorizontalHints = count;
            }

            for (int x = 0; x < puzzle.Width; x++)
            {
                int count = 0;
                for (int h = 0; h < maxHintsV; h++)
                    if (puzzle.HintsVertical[x, h] > 0) count++;
                if (count > maxVerticalHints) maxVerticalHints = count;
            }

            int paletteHeight = 50;
            int buttonsHeight = 45;
            int margin = 20;

            int availableWidth = this.ClientSize.Width - margin * 2;
            int availableHeight = this.ClientSize.Height - paletteHeight - buttonsHeight - margin * 2;

            cellSize = 20;
            while (cellSize >= 5)
            {
                hintBoxSize = Math.Max(12, cellSize - 4);
                leftSpace = maxHorizontalHints * (hintBoxSize + 2);
                topSpace = maxVerticalHints * (hintBoxSize + 2);

                int currentBoardWidth = puzzle.Width * cellSize;
                int currentBoardHeight = puzzle.Height * cellSize;
                int totalWidth = currentBoardWidth + leftSpace + 10;
                int totalHeight = currentBoardHeight + topSpace + 10;

                if (totalWidth <= availableWidth && totalHeight <= availableHeight)
                    break;

                cellSize--;
            }

            hintBoxSize = Math.Max(12, cellSize - 4);
            leftSpace = maxHorizontalHints * (hintBoxSize + 2);
            topSpace = maxVerticalHints * (hintBoxSize + 2);

            int finalBoardWidth = puzzle.Width * cellSize;
            int finalBoardHeight = puzzle.Height * cellSize;

            offsetX = leftSpace + 10;
            offsetY = topSpace + 10;

            gamePanel.Size = new Size(finalBoardWidth + leftSpace + 20, finalBoardHeight + topSpace + 20);

            gamePanel.Controls.Clear();
            DrawGrid();

            UpdateCompletedHints();
            gamePanel.Invalidate();
        }

        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CalculateLayout();  
            PositionControls(); 
        }
        private void GamePlayForm_Resize(object sender, EventArgs e)
        {
            CalculateLayout();
            PositionControls();
        }

        private void PositionControls()
        {
            if (colorPalette != null)
                colorPalette.Location = new Point((this.ClientSize.Width - colorPalette.Width) / 2, 10);

            btnCheck.Location = new Point(20, this.ClientSize.Height - 65);
            btnBack.Location = new Point(this.ClientSize.Width - 170, this.ClientSize.Height - 65);

            if (gamePanel != null)
            {
                
                int panelX = (this.ClientSize.Width - gamePanel.Width) / 2;
                int panelY = (this.ClientSize.Height - gamePanel.Height) / 2;
                gamePanel.Location = new Point(panelX, panelY);
            }
        }

        private void UpdateCompletedHints()
        {
            completedHorizontalHints.Clear();
            completedVerticalHints.Clear();

            for (int y = 0; y < puzzle.Height; y++)
            {
                int h = 0;
                for (int x = 0; x < puzzle.Width; x++)
                {
                    if (puzzle.Solution[y, x] != 0)
                    {
                        int color = puzzle.Solution[y, x];
                        int startX = x;
                        int len = 0;

                        while (x < puzzle.Width && puzzle.Solution[y, x] == color)
                        {
                            len++;
                            x++;
                        }

                        while (h < puzzle.HintsHorizontal.GetLength(1) && puzzle.HintsHorizontal[y, h] <= 0) h++;
                        if (h >= puzzle.HintsHorizontal.GetLength(1)) break;

                        int hintVal = puzzle.HintsHorizontal[y, h];
                        int hintColor = hintVal / 100;
                        int hintLen = hintVal % 100;

                        if (hintColor == color && hintLen == len)
                        {
                            bool blockCorrect = true;
                            for (int bx = 0; bx < len; bx++)
                            {
                                if (playerGrid[y, startX + bx] != color)
                                {
                                    blockCorrect = false;
                                    break;
                                }
                            }
                            if (blockCorrect)
                            {
                                if (startX > 0 && playerGrid[y, startX - 1] == color) blockCorrect = false;
                                if (startX + len < puzzle.Width && playerGrid[y, startX + len] == color) blockCorrect = false;
                            }

                            if (blockCorrect)
                                completedHorizontalHints.Add($"{y}_{h}");
                        }
                        h++;
                        x--;
                    }
                }
            }

            for (int x = 0; x < puzzle.Width; x++)
            {
                int h = 0;
                for (int y = 0; y < puzzle.Height; y++)
                {
                    if (puzzle.Solution[y, x] != 0)
                    {
                        int color = puzzle.Solution[y, x];
                        int startY = y;
                        int len = 0;

                        while (y < puzzle.Height && puzzle.Solution[y, x] == color)
                        {
                            len++;
                            y++;
                        }

                        while (h < puzzle.HintsVertical.GetLength(1) && puzzle.HintsVertical[x, h] <= 0) h++;
                        if (h >= puzzle.HintsVertical.GetLength(1)) break;

                        int hintVal = puzzle.HintsVertical[x, h];
                        int hintColor = hintVal / 100;
                        int hintLen = hintVal % 100;

                        if (hintColor == color && hintLen == len)
                        {
                            bool blockCorrect = true;
                            for (int by = 0; by < len; by++)
                            {
                                if (playerGrid[startY + by, x] != color)
                                {
                                    blockCorrect = false;
                                    break;
                                }
                            }
                            if (blockCorrect)
                            {
                                if (startY > 0 && playerGrid[startY - 1, x] == color) blockCorrect = false;
                                if (startY + len < puzzle.Height && playerGrid[startY + len, x] == color) blockCorrect = false;
                            }

                            if (blockCorrect)
                                completedVerticalHints.Add($"{x}_{h}");
                        }
                        h++;
                        y--;
                    }
                }
            }
        }

        private bool IsHorizontalHintCompleted(int row, int hintIndex)
        {
            return completedHorizontalHints.Contains($"{row}_{hintIndex}");
        }

        private bool IsVerticalHintCompleted(int col, int hintIndex)
        {
            return completedVerticalHints.Contains($"{col}_{hintIndex}");
        }

        private void DrawGrid()
        {
            int maxHintsH = puzzle.HintsHorizontal.GetLength(1);
            int maxHintsV = puzzle.HintsVertical.GetLength(1);

            gamePanel.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int fontSize = Math.Max(8, cellSize / 2);
                Font hintFont = new Font("Arial", fontSize, FontStyle.Bold);

                // Горизонтальные подсказки
                for (int y = 0; y < puzzle.Height; y++)
                {
                    int hintCount = 0;
                    for (int h = 0; h < maxHintsH; h++)
                        if (puzzle.HintsHorizontal[y, h] > 0) hintCount++;

                    int totalHintsWidth = hintCount * (hintBoxSize + 2);
                    int startY = offsetY + y * cellSize + (cellSize - hintBoxSize) / 2;
                    int startX = offsetX - totalHintsWidth;

                    int currentHintIndex = 0;
                    for (int h = 0; h < maxHintsH; h++)
                    {
                        int val = puzzle.HintsHorizontal[y, h];
                        if (val <= 0) continue;

                        int colorIndex = val / 100;
                        int count = val % 100;

                        if (count > 0 && colorIndex < puzzle.Colors.Length)
                        {
                            Color boxColor = puzzle.Colors[colorIndex];
                            bool isCompleted = IsHorizontalHintCompleted(y, currentHintIndex);
                            if (isCompleted) boxColor = Color.White;

                            int boxX = startX + currentHintIndex * (hintBoxSize + 2);
                            int boxY = startY;

                            using (Brush brush = new SolidBrush(boxColor))
                                g.FillRectangle(brush, boxX, boxY, hintBoxSize, hintBoxSize);
                            using (Pen pen = new Pen(Color.Gray, 1))
                                g.DrawRectangle(pen, boxX, boxY, hintBoxSize, hintBoxSize);

                            Color textColor = IsColorLight(boxColor) ? Color.Black : Color.White;
                            SizeF textSize = g.MeasureString(count.ToString(), hintFont);
                            float textX = boxX + (hintBoxSize - textSize.Width) / 2;
                            float textY = boxY + (hintBoxSize - textSize.Height) / 2;
                            using (Brush textBrush = new SolidBrush(textColor))
                                g.DrawString(count.ToString(), hintFont, textBrush, textX, textY);
                        }
                        currentHintIndex++;
                    }
                }

                // Вертикальные подсказки
                for (int x = 0; x < puzzle.Width; x++)
                {
                    int hintCount = 0;
                    for (int h = 0; h < maxHintsV; h++)
                        if (puzzle.HintsVertical[x, h] > 0) hintCount++;

                    int totalHintsHeight = hintCount * (hintBoxSize + 2);
                    int startX = offsetX + x * cellSize + (cellSize - hintBoxSize) / 2;
                    int startY = offsetY - totalHintsHeight;

                    int currentHintIndex = 0;
                    for (int h = 0; h < maxHintsV; h++)
                    {
                        int val = puzzle.HintsVertical[x, h];
                        if (val <= 0) continue;

                        int colorIndex = val / 100;
                        int count = val % 100;

                        if (count > 0 && colorIndex < puzzle.Colors.Length)
                        {
                            Color boxColor = puzzle.Colors[colorIndex];
                            bool isCompleted = IsVerticalHintCompleted(x, currentHintIndex);
                            if (isCompleted) boxColor = Color.White;

                            int boxX = startX;
                            int boxY = startY + currentHintIndex * (hintBoxSize + 2);

                            using (Brush brush = new SolidBrush(boxColor))
                                g.FillRectangle(brush, boxX, boxY, hintBoxSize, hintBoxSize);
                            using (Pen pen = new Pen(Color.Gray, 1))
                                g.DrawRectangle(pen, boxX, boxY, hintBoxSize, hintBoxSize);

                            Color textColor = IsColorLight(boxColor) ? Color.Black : Color.White;
                            SizeF textSize = g.MeasureString(count.ToString(), hintFont);
                            float textX = boxX + (hintBoxSize - textSize.Width) / 2;
                            float textY = boxY + (hintBoxSize - textSize.Height) / 2;
                            using (Brush textBrush = new SolidBrush(textColor))
                                g.DrawString(count.ToString(), hintFont, textBrush, textX, textY);
                        }
                        currentHintIndex++;
                    }
                }

                // Сетка
                for (int y = 0; y <= puzzle.Height; y++)
                {
                    using (Pen pen = (y % 5 == 0) ? new Pen(Color.Black, 2) : new Pen(Color.FromArgb(200, 200, 200), 1))
                    {
                        g.DrawLine(pen, offsetX, offsetY + y * cellSize,
                                 offsetX + puzzle.Width * cellSize, offsetY + y * cellSize);
                    }
                }
                for (int x = 0; x <= puzzle.Width; x++)
                {
                    using (Pen pen = (x % 5 == 0) ? new Pen(Color.Black, 2) : new Pen(Color.FromArgb(200, 200, 200), 1))
                    {
                        g.DrawLine(pen, offsetX + x * cellSize, offsetY,
                                 offsetX + x * cellSize, offsetY + puzzle.Height * cellSize);
                    }
                }
                using (Pen borderPen = new Pen(Color.Black, 3))
                {
                    g.DrawRectangle(borderPen,
                        offsetX, offsetY,
                        puzzle.Width * cellSize,
                        puzzle.Height * cellSize);
                }

                // Закраска клеток игрока
                for (int y = 0; y < puzzle.Height; y++)
                {
                    for (int x = 0; x < puzzle.Width; x++)
                    {
                        if (playerGrid[y, x] > 0)
                        {
                            int colorIndex = playerGrid[y, x];
                            if (colorIndex < puzzle.Colors.Length)
                            {
                                using (Brush brush = new SolidBrush(puzzle.Colors[colorIndex]))
                                    g.FillRectangle(brush, offsetX + x * cellSize + 1,
                                                   offsetY + y * cellSize + 1, cellSize - 2, cellSize - 2);
                            }
                        }
                        else if (playerGrid[y, x] == -1)
                        {
                            using (Pen pen = new Pen(Color.Black, 2))
                            {
                                int margin = Math.Max(1, cellSize / 6);
                                g.DrawLine(pen,
                                    offsetX + x * cellSize + margin,
                                    offsetY + y * cellSize + margin,
                                    offsetX + x * cellSize + cellSize - margin,
                                    offsetY + y * cellSize + cellSize - margin);
                                g.DrawLine(pen,
                                    offsetX + x * cellSize + cellSize - margin,
                                    offsetY + y * cellSize + margin,
                                    offsetX + x * cellSize + margin,
                                    offsetY + y * cellSize + cellSize - margin);
                            }
                        }
                    }
                }
                hintFont.Dispose();
            };

            gamePanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isPainting = true;
                    lastPaintedX = -1; lastPaintedY = -1;
                    TryPaintCell(e.X, e.Y);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    isPainting = true;
                    lastPaintedX = -1; lastPaintedY = -1;
                    TryToggleCross(e.X, e.Y);
                }
            };

            gamePanel.MouseMove += (s, e) =>
            {
                if (isPainting)
                {
                    if (Control.MouseButtons == MouseButtons.Left)
                        TryPaintCell(e.X, e.Y);
                    else if (Control.MouseButtons == MouseButtons.Right)
                        TryToggleCross(e.X, e.Y);
                    else
                        isPainting = false;
                }
            };

            gamePanel.MouseUp += (s, e) =>
            {
                isPainting = false;
                lastPaintedX = -1; lastPaintedY = -1;
            };

            gamePanel.MouseClick += (s, e) =>
            {
                if (!isPainting)
                {
                    if (e.Button == MouseButtons.Left)
                        TryPaintCell(e.X, e.Y);
                    else if (e.Button == MouseButtons.Right)
                        TryToggleCross(e.X, e.Y);
                }
            };
        }

        private void TryPaintCell(int mouseX, int mouseY)
        {
            int x = (mouseX - offsetX) / cellSize;
            int y = (mouseY - offsetY) / cellSize;

            if (x >= 0 && x < puzzle.Width && y >= 0 && y < puzzle.Height)
            {
                if (lastPaintedX == x && lastPaintedY == y) return;
                if (playerGrid[y, x] != selectedColor)
                {
                    playerGrid[y, x] = selectedColor;
                    lastPaintedX = x; lastPaintedY = y;
                    UpdateCompletedHints();
                    gamePanel.Invalidate();
                }
            }
        }

        private void TryToggleCross(int mouseX, int mouseY)
        {
            int x = (mouseX - offsetX) / cellSize;
            int y = (mouseY - offsetY) / cellSize;

            if (x >= 0 && x < puzzle.Width && y >= 0 && y < puzzle.Height)
            {
                if (lastPaintedX == x && lastPaintedY == y) return;
                int newValue = (playerGrid[y, x] == -1) ? 0 : -1;
                if (playerGrid[y, x] != newValue)
                {
                    playerGrid[y, x] = newValue;
                    lastPaintedX = x; lastPaintedY = y;
                    UpdateCompletedHints();
                    gamePanel.Invalidate();
                }
            }
        }

        private void SelectColor(int color, Button btn)
        {
            selectedColor = color;
            foreach (Control c in colorPalette.Controls)
                if (c is Button b) b.FlatAppearance.BorderSize = 1;
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
                GoBack();
            }
            else
            {
                MessageBox.Show(" Есть ошибки. Попробуй ещё!", "Неверно",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GoBack()
        {
            if (parentPanel != null && mainForm is Form1 form1)
            {
                parentPanel.Controls.Clear();
                LevelSelectForm levelForm = new LevelSelectForm(profile, profileIndex, parentPanel, mainForm);
                levelForm.Dock = DockStyle.Fill;
                levelForm.TopLevel = false;
                levelForm.FormBorderStyle = FormBorderStyle.None;
                parentPanel.Controls.Add(levelForm);
                levelForm.Show();
            }
            else
            {
                this.Close();
            }
        }

        private bool IsColorLight(Color c)
        {
            int brightness = (c.R * 299 + c.G * 587 + c.B * 114) / 1000;
            return brightness > 128;
        }
    }
}