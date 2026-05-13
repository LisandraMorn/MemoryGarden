using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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

        [XmlArray]
        public List<LevelProgressEntry> LevelProgresses { get; set; }

        [XmlArray]
        public List<PuzzleRecord> Gallery { get; set; }

        public Profile()
        {
            Name = "";
            CompletedLevels = new List<int>();
            UnlockedLevels = new List<int> { 0 };
            HasSeenTutorial1 = false;
            HasSeenTutorial2 = false;
            LevelProgresses = new List<LevelProgressEntry>();
            Gallery = new List<PuzzleRecord>();
        }

        public LevelProgress GetLevelProgress(int levelId)
        {
            var entry = LevelProgresses.Find(e => e.LevelId == levelId);
            if (entry == null)
            {
                entry = new LevelProgressEntry { LevelId = levelId, Progress = new LevelProgress() };
                LevelProgresses.Add(entry);
            }
            return entry.Progress;
        }

        public void SetLevelProgress(int levelId, LevelProgress progress)
        {
            var entry = LevelProgresses.Find(e => e.LevelId == levelId);
            if (entry != null)
                entry.Progress = progress;
            else
                LevelProgresses.Add(new LevelProgressEntry { LevelId = levelId, Progress = progress });
        }

        public void RemoveLevelProgress(int levelId)
        {
            LevelProgresses.RemoveAll(e => e.LevelId == levelId);
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
                System.Windows.Forms.MessageBox.Show($"Ошибка сохранения: {ex.Message}");
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

    [Serializable]
    public class LevelProgressEntry
    {
        public int LevelId { get; set; }
        public LevelProgress Progress { get; set; }

        public LevelProgressEntry() { }
    }

    [Serializable]
    public class LevelProgress
    {
        public bool[] MemoriesSolved;
        public List<bool> PostDialogueDone;
        public float CatPosX;
        public float CatPosY;
        public bool PositionSaved;

        
        public List<bool> AliceDialogueDoneList;
        public List<bool> AliceVisibleList;

        public LevelProgress()
        {
            MemoriesSolved = new bool[3];
            PostDialogueDone = new List<bool>();
            AliceDialogueDoneList = new List<bool>();
            AliceVisibleList = new List<bool>();
        }
    }

    [Serializable]
    public class PuzzleRecord
    {
        public string ImagePath { get; set; }
        public string Caption { get; set; }

        public PuzzleRecord() { }
        public PuzzleRecord(string path, string caption)
        {
            ImagePath = path;
            Caption = caption;
        }
    }
}