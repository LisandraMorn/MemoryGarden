using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Memory.Data
{
    public static class ImageLoader
    {
        public static Puzzle LoadFromImage(string imagePath, int difficulty = 1)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(imagePath))
                {
                    Puzzle puzzle = new Puzzle
                    {
                        Width = bmp.Width,
                        Height = bmp.Height,
                        Difficulty = difficulty,
                        Name = $"Уровень из {System.IO.Path.GetFileName(imagePath)}"
                    };

                    
                    List<Color> uniqueColors = new List<Color>();
                    uniqueColors.Add(Color.White);

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color pixel = bmp.GetPixel(x, y);

                            if (pixel.A < 128)
                            {
                                continue;
                            }

                            int colorIndex = FindColorIndex(uniqueColors, pixel, 25);

                            if (colorIndex == -1)
                            {
                                if (uniqueColors.Count < 11)
                                {
                                    uniqueColors.Add(pixel);
                                }
                                else
                                {
                                    MessageBox.Show(
                                        $"⚠️ Превышено количество цветов!\n\n" +
                                        $"В картинке больше 10 уникальных цветов.\n" +
                                        $"Текущий: {uniqueColors.Count}\n\n" +
                                        $"Упростите палитру или уменьшите допуск.",
                                        "Внимание",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }

                    puzzle.Colors = uniqueColors.ToArray();

                    
                    puzzle.Solution = new int[bmp.Height, bmp.Width];

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color pixel = bmp.GetPixel(x, y);

                            if (pixel.A < 128)
                            {
                                puzzle.Solution[y, x] = 0;
                            }
                            else
                            {
                                int colorIndex = FindColorIndex(uniqueColors, pixel, 25);
                                puzzle.Solution[y, x] = colorIndex >= 0 ? colorIndex : 0;
                            }
                        }
                    }

                    
                    int maxHints = Math.Max(bmp.Width, bmp.Height) / 2 + 5;
                    puzzle.HintsHorizontal = GenerateHints(puzzle.Solution, puzzle.Height, puzzle.Width, true, maxHints);
                    puzzle.HintsVertical = GenerateHints(puzzle.Solution, puzzle.Width, puzzle.Height, false, maxHints);

                   

                    return puzzle;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка загрузки изображения:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static int FindColorIndex(List<Color> colors, Color target, int tolerance)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                Color c = colors[i];

                int diff = Math.Abs(c.R - target.R) +
                          Math.Abs(c.G - target.G) +
                          Math.Abs(c.B - target.B);

                if (diff <= tolerance * 3)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int[,] GenerateHints(int[,] data, int rows, int cols, bool horizontal, int maxHints = 20)
        {
            int[,] hints = new int[rows, maxHints];

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
                                if (hintIndex < maxHints)
                                {
                                    hints[i, hintIndex++] = lastColor * 100 + count;
                                }
                            }
                            count = 1;
                            lastColor = val;
                        }
                    }
                    else
                    {
                        if (count > 0)
                        {
                            if (hintIndex < maxHints)
                            {
                                hints[i, hintIndex++] = lastColor * 100 + count;
                            }
                            count = 0;
                            lastColor = 0;
                        }
                    }
                }

                if (count > 0)
                {
                    if (hintIndex < maxHints)
                    {
                        hints[i, hintIndex++] = lastColor * 100 + count;
                    }
                }
            }

            return hints;
        }
    }
}