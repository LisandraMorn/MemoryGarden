using System;
using System.Windows.Forms;
using System.IO;
using Memory.Data;

namespace Memory
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            if (!Directory.Exists("Saves"))
                Directory.CreateDirectory("Saves");

            
            if (!Directory.Exists("Assets/Sounds"))
                Directory.CreateDirectory("Assets/Sounds");

            
            MusicManager.Play();

            Application.Run(new Form1());

            
            MusicManager.Stop();
        }
    }
}