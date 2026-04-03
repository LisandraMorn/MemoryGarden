using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Memory.Data
{
    [Serializable]
    public class Profile
    {
        public string Name { get; set; }
        public List<int> CompletedLevels { get; set; }
        public List<int> UnlockedLevels { get; set; }
        public bool HasSeenTutorial1 { get; set; }
        public bool HasSeenTutorial2 { get; set; }
        public Profile()
        {
            Name = "";
            CompletedLevels = new List<int>();
            UnlockedLevels = new List<int> { 0 };
            HasSeenTutorial1 = false;
            HasSeenTutorial2 = false;
        }

        public void Save(int profileIndex)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                using (FileStream fs = new FileStream($"Saves/user_{profileIndex}.dat", FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static Profile Load(int profileIndex)
        {
            try
            {
                string path = $"Saves/user_{profileIndex}.dat";
                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        return (Profile)serializer.Deserialize(fs);
                    }
                }
            }
            catch { }

            return new Profile();
        }

        public static bool Exists(int profileIndex)
        {
            return File.Exists($"Saves/user_{profileIndex}.dat");
        }

        public static void Delete(int profileIndex)
        {
            string path = $"Saves/user_{profileIndex}.dat";
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}