using System;
using System.Windows.Forms;
using System.IO;

namespace Memory
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Создаём папку для сохранений
            if (!Directory.Exists("Saves"))
                Directory.CreateDirectory("Saves");

            // Используем Form1 как главную форму (пока что)
            Application.Run(new Form1());
        }
    }
}