using System;
using System.Drawing;

namespace Memory.Data
{
    [Serializable]
    public class Puzzle
    {
        public int ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Difficulty { get; set; }
        public string Name { get; set; }
        public int[,] Solution { get; set; }
        public Color[] Colors { get; set; }
        public int[,] HintsHorizontal { get; set; }
        public int[,] HintsVertical { get; set; }

        public Puzzle()
        {
            Width = 14;
            Height = 15;
            Difficulty = 1;
            Name = "Уровень 1";
            Solution = new int[15, 14];
            Colors = new Color[] { Color.White, Color.Black };
            HintsHorizontal = new int[15, 10];
            HintsVertical = new int[14, 10];
        }
    }
}