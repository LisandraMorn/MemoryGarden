using System;
using System.IO;
using System.Xml.Serialization;

namespace Memory.Data
{
    [Serializable]
    public class Settings
    {
        public int LastProfileIndex { get; set; }
        public bool SoundEnabled { get; set; }
        public bool MusicEnabled { get; set; }
        public int MusicVolume { get; set; }
        public int SoundVolume { get; set; }

        public Settings()
        {
            LastProfileIndex = -1;
            SoundEnabled = true;
            MusicEnabled = true;
            MusicVolume = 100;
            SoundVolume = 100;
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream("Saves/settings.dat", FileMode.Create))
                {
                    serializer.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения настроек: {ex.Message}");
            }
        }

        public static Settings Load()
        {
            try
            {
                string path = "Saves/settings.dat";
                if (File.Exists(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        return (Settings)serializer.Deserialize(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки настроек: {ex.Message}");
            }

            return new Settings();
        }
    }
}