using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Memory.Data;

namespace Memory
{
    public abstract class PlatformerLevel : Form
    {
        protected Profile profile;
        protected int profileIndex;
        protected Panel parentPanel;
        protected Form mainForm;
        private int levelId;
        private Timer dialogueEndTimer;

        private Timer gameTimer;
        private const float Gravity = 0.8f;
        private const float MoveSpeed = 5f;
        private const float JumpForce = -14f;
        private bool leftDown, rightDown, jumpDown;
        private bool isOnGround;
        private PointF catVelocity;

        private RectangleF catBounds = new RectangleF(100, 0, 50, 45);
        private Image catStandImg, catRun1Img, catRun2Img;
        private bool facingRight = true;
        private Timer animTimer;
        private int currentRunFrame = 0;
        private const int AnimInterval = 200;

        protected List<RectangleF> platforms = new List<RectangleF>();
        protected List<int> platformTypes = new List<int>();
        protected List<RectangleF> invisiblePlatforms = new List<RectangleF>();
        protected List<RectangleF> decorations = new List<RectangleF>();
        protected List<Image> decorationImgs = new List<Image>();

        
        protected List<RectangleF> walkThroughSprites = new List<RectangleF>();
        protected List<Image> walkThroughImages = new List<Image>();

        protected RectangleF[] memoryItems = new RectangleF[3];
        protected bool[] memoryActive = new bool[3];
        protected Image[] memoryImgs = new Image[3];

        
        protected List<RectangleF> aliceTriggers = new List<RectangleF>();
        protected List<bool> aliceVisible = new List<bool>();
        protected List<bool> aliceDialogueDone = new List<bool>();
        protected List<string[]> aliceDialogues = new List<string[]>();
        protected Image aliceImg;

        protected RectangleF doorTrigger;
        protected Image doorImg, booksStackImg, dresserImg, hourglassImg;

        protected List<RectangleF> dialogueTriggers = new List<RectangleF>();
        protected List<string[]> dialogueTexts = new List<string[]>();
        protected List<bool> dialogueDone = new List<bool>();

        private Dialogue currentDialogue;
        private Timer dialogueTimer;
        private Timer pauseBetweenLinesTimer;
        private bool dialogueWillHideAlice;
        private RectangleF? dialoguePosition;
        private int dialogueAliceIndex = -1; 

        private int memoriesCollected = 0;
        private const int totalMemories = 3;
        private bool levelCompleted = false;

        private float zoom = 1.3f;
        protected int worldWidth = 4000;
        protected Image floorImg, blockImg, bookshelfImg;

        private bool[] memoryCooldown = new bool[3];

        public PlatformerLevel(Profile prof, int profIdx, Panel panel, Form main, int lvlId = 0)
        {
            profile = prof;
            profileIndex = profIdx;
            parentPanel = panel;
            mainForm = main;
            levelId = lvlId;

            this.Text = "Уровень";
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.Paint += OnPaint;
            this.Resize += (s, e) => { RebuildWorld(); Invalidate(); };
            this.FormClosing += OnFormClosing;

            catStandImg = LoadImage("cat_stand.png");
            catRun1Img = LoadImage("cat_run1.png");
            catRun2Img = LoadImage("cat_run2.png");
            aliceImg = LoadImage("alice.png");
            doorImg = LoadImage("door.png");
            floorImg = LoadImage("floor.png");
            blockImg = LoadImage("block.png");
            bookshelfImg = LoadImage("bookshelf.png");
            Image universalMemory = LoadImage("memory_item.png");
            memoryImgs[0] = universalMemory;
            memoryImgs[1] = universalMemory;
            memoryImgs[2] = universalMemory;
            booksStackImg = LoadImage("books_stack.png");
            dresserImg = LoadImage("dresser.png");
            hourglassImg = LoadImage("hourglass.png");
        }

        protected abstract void BuildWorld();

        protected void AddPlatform(RectangleF rect, int type = 0)
        {
            platforms.Add(rect);
            platformTypes.Add(type);
        }

        protected void AddWalkThroughObject(Image img, RectangleF bounds)
        {
            walkThroughSprites.Add(bounds);
            walkThroughImages.Add(img);
            float platHeight = 10;
            RectangleF topPlat = new RectangleF(bounds.X, bounds.Y, bounds.Width, platHeight);
            invisiblePlatforms.Add(topPlat);
        }

        protected void AddDecoration(string imgFile, float x, float y, float w, float h)
        {
            Image img = LoadImage(imgFile);
            if (img != null)
            {
                decorations.Add(new RectangleF(x, y, w, h));
                decorationImgs.Add(img);
            }
        }

        private void RebuildWorld()
        {
            platforms.Clear();
            platformTypes.Clear();
            decorations.Clear();
            decorationImgs.Clear();
            dialogueTriggers.Clear();
            dialogueTexts.Clear();
            dialogueDone.Clear();
            walkThroughSprites.Clear();
            walkThroughImages.Clear();
            invisiblePlatforms.Clear();

            aliceTriggers.Clear();
            aliceVisible.Clear();
            aliceDialogueDone.Clear();
            aliceDialogues.Clear();

            BuildWorld();
            if (platforms.Count > 0)
                catBounds.Y = platforms[0].Y - catBounds.Height;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RebuildWorld();
            LoadProgress();

            gameTimer = new Timer { Interval = 16 };
            gameTimer.Tick += UpdateGame;
            gameTimer.Start();

            animTimer = new Timer { Interval = AnimInterval };
            animTimer.Tick += (s, ev) => { currentRunFrame = (currentRunFrame + 1) % 2; };
            animTimer.Start();

            dialogueTimer = new Timer { Interval = 75 };
            dialogueTimer.Tick += DialogueTimer_Tick;

            pauseBetweenLinesTimer = new Timer { Interval = 1000 };
            pauseBetweenLinesTimer.Tick += PauseBetweenLinesTimer_Tick;

            dialogueEndTimer = new Timer { Interval = 1000 };
            dialogueEndTimer.Tick += DialogueEndTimer_Tick;

            this.Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: case Keys.Left: leftDown = true; break;
                case Keys.D: case Keys.Right: rightDown = true; break;
                case Keys.W: case Keys.Up: jumpDown = true; break;
                case Keys.Escape:
                    ShowPauseMenu();
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: case Keys.Left: leftDown = false; break;
                case Keys.D: case Keys.Right: rightDown = false; break;
                case Keys.W: case Keys.Up: jumpDown = false; break;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !levelCompleted)
            {
                e.Cancel = true;
                ShowPauseMenu();
            }
        }

        private void ShowPauseMenu()
        {
            gameTimer.Stop();
            animTimer.Stop();

            PauseMenuForm pauseForm = new PauseMenuForm(
                mainForm as Form1,
                profile,
                profileIndex,
                parentPanel,
                this
            );

            DialogResult result = pauseForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                gameTimer.Start();
                animTimer.Start();
                this.Focus();
            }
            else if (result == DialogResult.Abort)
            {
                GoBackToMenu();
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (levelCompleted) return;

            float move = 0;
            if (leftDown) move = -MoveSpeed;
            if (rightDown) move = MoveSpeed;
            catVelocity.X = move;

            catVelocity.Y += Gravity;

            if (jumpDown && isOnGround)
            {
                catVelocity.Y = JumpForce;
                isOnGround = false;
            }

            catBounds.X += catVelocity.X;
            HandleCollision(true);
            catBounds.Y += catVelocity.Y;
            isOnGround = false;
            HandleCollision(false);

            catBounds.X = Math.Max(0, Math.Min(catBounds.X, worldWidth - catBounds.Width));
            catBounds.Y = Math.Max(0, Math.Min(catBounds.Y, ClientSize.Height - catBounds.Height));

            
            for (int i = 0; i < aliceTriggers.Count; i++)
            {
                if (aliceVisible[i] && !aliceDialogueDone[i] && aliceDialogues[i] != null &&
                    catBounds.IntersectsWith(aliceTriggers[i]))
                {
                    aliceDialogueDone[i] = true;
                    StartDialogue(aliceDialogues[i], hideAliceAfter: true, pos: aliceTriggers[i], aliceIndex: i);
                    break;
                }
            }

            for (int i = 0; i < memoryItems.Length; i++)
            {
                if (!memoryCooldown[i] && memoryActive[i] && catBounds.IntersectsWith(memoryItems[i]))
                    StartMemoryPuzzle(i);
            }

            for (int i = 0; i < dialogueTriggers.Count; i++)
                if (!dialogueDone[i] && catBounds.IntersectsWith(dialogueTriggers[i]) && i < memoriesCollected)
                {
                    dialogueDone[i] = true;
                    StartDialogue(dialogueTexts[i], false, dialogueTriggers[i]);
                }

            if (!levelCompleted && memoriesCollected >= totalMemories && catBounds.IntersectsWith(doorTrigger))
            {
                levelCompleted = true;
                if (!profile.CompletedLevels.Contains(levelId))
                {
                    profile.CompletedLevels.Add(levelId);

                   
                    int nextLevelId = levelId + 1;
                    if (!profile.UnlockedLevels.Contains(nextLevelId) && nextLevelId < 9)
                    {
                        profile.UnlockedLevels.Add(nextLevelId);
                    }

                    profile.RemoveLevelProgress(levelId);
                    profile.Save(profileIndex);
                }
                MessageBox.Show("Уровень пройден!", "Поздравляем",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                GoBackToMenu();
                return;
            }

            if (catVelocity.X > 0) facingRight = true;
            else if (catVelocity.X < 0) facingRight = false;

            Invalidate();
        }

        private void HandleCollision(bool horizontal)
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                var plat = platforms[i];
                if (!catBounds.IntersectsWith(plat)) continue;

                int type = i < platformTypes.Count ? platformTypes[i] : 0;

                if (horizontal && type == 1) continue;

                RectangleF intersection = RectangleF.Intersect(catBounds, plat);
                if (horizontal)
                {
                    if (catVelocity.X > 0) catBounds.X -= intersection.Width;
                    else if (catVelocity.X < 0) catBounds.X += intersection.Width;
                    catVelocity.X = 0;
                }
                else
                {
                    if (catVelocity.Y > 0)
                    {
                        catBounds.Y -= intersection.Height;
                        catVelocity.Y = 0;
                        isOnGround = true;
                    }
                    else if (catVelocity.Y < 0)
                    {
                        catBounds.Y += intersection.Height;
                        catVelocity.Y = 0;
                    }
                }
            }

            for (int i = 0; i < invisiblePlatforms.Count; i++)
            {
                var plat = invisiblePlatforms[i];
                if (!catBounds.IntersectsWith(plat)) continue;

                RectangleF intersection = RectangleF.Intersect(catBounds, plat);
                if (horizontal)
                {
                    continue;
                }
                else
                {
                    if (catVelocity.Y > 0)
                    {
                        catBounds.Y -= intersection.Height;
                        catVelocity.Y = 0;
                        isOnGround = true;
                    }
                    else if (catVelocity.Y < 0)
                    {
                        catBounds.Y += intersection.Height;
                        catVelocity.Y = 0;
                    }
                }
            }
        }

        private void StartMemoryPuzzle(int index)
        {
            leftDown = rightDown = jumpDown = false;
            gameTimer.Stop();
            animTimer.Stop();

            string imagePath = $"Assets/Images/level_{index + 1}.png";
            Puzzle puzzle;
            if (System.IO.File.Exists(imagePath))
                puzzle = ImageLoader.LoadFromImage(imagePath, 1);
            else
                puzzle = CreateSmallPuzzle(index);

            GamePlayForm game = new GamePlayForm(puzzle, profile, profileIndex);
            game.ShowDialog();

            if (game.IsCompleted)
            {
                memoryActive[index] = false;
                memoriesCollected++;

                string caption = dialogueTexts.Count > index ? dialogueTexts[index][0] : "";
                if (!profile.Gallery.Exists(r => r.ImagePath == imagePath))
                    profile.Gallery.Add(new PuzzleRecord(imagePath, caption));

                SaveProgress();
            }
            else
            {
                memoryCooldown[index] = true;
                Timer cooldownTimer = new Timer { Interval = 2000 };
                cooldownTimer.Tick += (s, args) =>
                {
                    memoryCooldown[index] = false;
                    cooldownTimer.Stop();
                    cooldownTimer.Dispose();
                };
                cooldownTimer.Start();
            }

            animTimer.Start();
            gameTimer.Start();
            Invalidate();
        }

        private Puzzle CreateSmallPuzzle(int index)
        {
            Puzzle p = new Puzzle
            {
                Width = 5,
                Height = 5,
                Name = "Мини-пазл",
                Colors = new Color[] { Color.White, Color.Black },
                Solution = new int[5, 5]
            };
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    p.Solution[y, x] = (x + y + index) % 3 == 0 ? 1 : 0;
            p.HintsHorizontal = ImageLoader.GenerateHints(p.Solution, 5, 5, true, 10);
            p.HintsVertical = ImageLoader.GenerateHints(p.Solution, 5, 5, false, 10);
            return p;
        }

        private void StartDialogue(string[] lines, bool hideAliceAfter = false, RectangleF? pos = null, int aliceIndex = -1)
        {
            dialogueWillHideAlice = hideAliceAfter;
            dialoguePosition = pos;
            dialogueAliceIndex = aliceIndex;
            currentDialogue = new Dialogue(lines);
            dialogueTimer.Start();
            Invalidate();
        }

        private void DialogueTimer_Tick(object sender, EventArgs e)
        {
            if (currentDialogue == null) return;
            currentDialogue.Update();
            if (currentDialogue.IsLineComplete())
            {
                dialogueTimer.Stop();
                pauseBetweenLinesTimer.Start();
            }
            Invalidate();
        }

        private void PauseBetweenLinesTimer_Tick(object sender, EventArgs e)
        {
            pauseBetweenLinesTimer.Stop();
            if (currentDialogue == null) { ClearDialogue(); return; }

            if (currentDialogue.IsComplete())
            {
                dialogueEndTimer.Start();
            }
            else
            {
                currentDialogue.NextLine();
                dialogueTimer.Start();
            }
            Invalidate();
        }

        private void DialogueEndTimer_Tick(object sender, EventArgs e)
        {
            dialogueEndTimer.Stop();
            if (currentDialogue == null) return;

            if (dialogueWillHideAlice && dialogueAliceIndex >= 0 && dialogueAliceIndex < aliceVisible.Count)
            {
                aliceVisible[dialogueAliceIndex] = false;
            }
            ClearDialogue();
            Invalidate();
        }

        private void ClearDialogue()
        {
            currentDialogue = null;
            dialogueWillHideAlice = false;
            dialoguePosition = null;
            dialogueAliceIndex = -1;
            Invalidate();
        }

        private void SaveProgress()
        {
            if (profile == null) return;
            var p = profile.GetLevelProgress(levelId);
            for (int i = 0; i < 3; i++)
                p.MemoriesSolved[i] = !memoryActive[i];
            p.AliceDialogueDoneList = new List<bool>(aliceDialogueDone);
            p.AliceVisibleList = new List<bool>(aliceVisible);
            p.PostDialogueDone = new List<bool>(dialogueDone);
            p.CatPosX = catBounds.X;
            p.CatPosY = catBounds.Y;
            p.PositionSaved = true;
            profile.SetLevelProgress(levelId, p);
            profile.Save(profileIndex);
        }

        private void LoadProgress()
        {
            if (profile == null) return;
            var p = profile.GetLevelProgress(levelId);
            if (p == null) return;

            for (int i = 0; i < 3 && i < p.MemoriesSolved.Length; i++)
            {
                memoryActive[i] = !p.MemoriesSolved[i];
                if (!memoryActive[i]) memoriesCollected++;
            }

            if (p.AliceDialogueDoneList != null)
            {
                for (int i = 0; i < p.AliceDialogueDoneList.Count && i < aliceDialogueDone.Count; i++)
                    aliceDialogueDone[i] = p.AliceDialogueDoneList[i];
            }
            if (p.AliceVisibleList != null)
            {
                for (int i = 0; i < p.AliceVisibleList.Count && i < aliceVisible.Count; i++)
                    aliceVisible[i] = p.AliceVisibleList[i];
            }

            if (p.PostDialogueDone != null && p.PostDialogueDone.Count == dialogueDone.Count)
            {
                for (int i = 0; i < dialogueDone.Count; i++)
                    dialogueDone[i] = p.PostDialogueDone[i];
            }

            if (p.PositionSaved)
            {
                catBounds.X = p.CatPosX;
                catBounds.Y = p.CatPosY;
            }
        }

        private void GoBackToMenu()
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

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.Clear(Color.Black);

            g.ScaleTransform(zoom, zoom);

            int viewWidth = (int)(ClientSize.Width / zoom);
            int viewHeight = (int)(ClientSize.Height / zoom);
            int worldHeight = ClientSize.Height;

            float camX = catBounds.X + catBounds.Width / 2 - viewWidth / 2;
            camX = Math.Max(0, Math.Min(camX, worldWidth - viewWidth));
            float camY = catBounds.Y + catBounds.Height / 2 - viewHeight / 2;
            camY = Math.Max(0, Math.Min(camY, worldHeight - viewHeight));

            if (viewWidth >= worldWidth) camX = 0;
            if (viewHeight >= worldHeight) camY = 0;

            g.TranslateTransform(-camX, -camY);

            
            for (int i = 0; i < decorations.Count; i++)
                DrawScaledImage(g, decorationImgs[i], decorations[i]);

            
            if (platforms.Count > 0)
                DrawScaledImage(g, floorImg, platforms[0]);

            
            for (int i = 1; i < platforms.Count; i++)
            {
                var plat = platforms[i];
                int type = i < platformTypes.Count ? platformTypes[i] : 0;

                if (type == 1)
                    DrawScaledImage(g, bookshelfImg, plat);
                else if (plat.Height == 20 && plat.Width == 100)
                    DrawScaledImage(g, blockImg, plat);
            }

             
            for (int i = 0; i < walkThroughSprites.Count; i++)
                DrawScaledImage(g, walkThroughImages[i], walkThroughSprites[i]);

            
            if (doorImg != null)
                DrawScaledImage(g, doorImg, doorTrigger);

            
            for (int i = 0; i < aliceTriggers.Count; i++)
            {
                if (aliceVisible[i] && aliceImg != null && aliceTriggers[i].Width > 0)
                    DrawScaledImage(g, aliceImg, aliceTriggers[i]);
            }

            
            for (int i = 0; i < memoryItems.Length; i++)
                if (memoryActive[i] && memoryImgs[i] != null)
                    DrawScaledImage(g, memoryImgs[i], memoryItems[i]);

            
            Image catImg = (Math.Abs(catVelocity.X) > 0.5f) ?
                           ((currentRunFrame == 0) ? catRun1Img : catRun2Img) :
                           catStandImg;

            if (catImg != null)
            {
                if (!facingRight)
                {
                    GraphicsState state = g.Save();
                    g.ScaleTransform(-1, 1);
                    g.TranslateTransform(-(catBounds.X * 2 + catBounds.Width), 0);
                    DrawScaledImage(g, catImg, catBounds);
                    g.Restore(state);
                }
                else
                    DrawScaledImage(g, catImg, catBounds);
            }
            else
                g.FillRectangle(Brushes.White, catBounds);

            
            if (currentDialogue != null)
            {
                string text = currentDialogue.GetCurrentText();
                string fullLine = currentDialogue.GetFullLine();

                if (!string.IsNullOrEmpty(fullLine))
                {
                    float boxWidth = 350f;
                    float boxHeight = 50f;
                    float boxX, boxY;

                    if (dialoguePosition.HasValue)
                    {
                        var pos = dialoguePosition.Value;
                        boxX = pos.X + pos.Width / 2 - boxWidth / 2;
                        boxY = pos.Y - boxHeight - 4;
                    }
                    else
                    {
                        boxX = catBounds.X + catBounds.Width / 2 - boxWidth / 2;
                        boxY = catBounds.Y - boxHeight - 10;
                    }

                    if (boxX < 10) boxX = 10;
                    if (boxY < 10) boxY = 10;

                    using (Font font = new Font("Georgia", 12, FontStyle.Bold))
                    {
                        SizeF fullSize = g.MeasureString(fullLine, font);
                        float textWidth = fullSize.Width;
                        float offsetX = (boxWidth - textWidth) / 2;
                        float startX = boxX + offsetX;

                        RectangleF textRect = new RectangleF(startX, boxY, textWidth, boxHeight);
                        using (Brush brush = new SolidBrush(Color.White))
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = StringAlignment.Center;
                            sf.Trimming = StringTrimming.None;
                            g.DrawString(text, font, brush, textRect, sf);
                        }
                    }
                }
            }

            g.ResetTransform();

            string counter = $"Воспоминания: {memoriesCollected}/{totalMemories}";
            g.DrawString(counter, new Font("Georgia", 14, FontStyle.Bold), Brushes.White, 10, 10);
        }

        private void DrawScaledImage(Graphics g, Image img, RectangleF dest)
        {
            if (img == null) return;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(img, dest.X, dest.Y, dest.Width, dest.Height);
        }

        private Image LoadImage(string fileName)
        {
            string path = System.IO.Path.Combine("Assets", "Images", fileName);
            try
            {
                if (System.IO.File.Exists(path)) return Image.FromFile(path);
            }
            catch { }
            return null;
        }
    }

    public class Dialogue
    {
        private string[] lines;
        private int currentLineIndex = 0;
        private int charIndex = 0;
        private string currentText = "";

        public Dialogue(string[] lines) => this.lines = lines;

        public void Update()
        {
            if (currentLineIndex >= lines.Length) return;
            if (charIndex < lines[currentLineIndex].Length)
            {
                currentText += lines[currentLineIndex][charIndex];
                charIndex++;
            }
        }

        public string GetCurrentText() => currentText;
        public string GetFullLine() => currentLineIndex < lines.Length ? lines[currentLineIndex] : "";
        public bool IsLineComplete() => currentLineIndex < lines.Length && charIndex >= lines[currentLineIndex].Length;
        public bool IsComplete() => currentLineIndex == lines.Length - 1 && IsLineComplete();
        public void NextLine()
        {
            if (currentLineIndex < lines.Length - 1)
            {
                currentLineIndex++;
                charIndex = 0;
                currentText = "";
            }
        }
    }
}