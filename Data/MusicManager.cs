using System;
using System.IO;
using NAudio.Wave;
using Memory.Data;

namespace Memory.Data
{
    public static class MusicManager
    {
        private static WaveOutEvent waveOut;
        private static AudioFileReader audioFile;
        private static bool isPlaying = false;
        private static string musicFilePath = "Assets/Sounds/background.mp3";

        static MusicManager()
        {
            try
            {
                Settings settings = Settings.Load();
                IsEnabled = settings.MusicEnabled;
                CurrentVolume = settings.MusicVolume;
            }
            catch
            {
                IsEnabled = true;
                CurrentVolume = 100;
            }
        }

        public static bool IsEnabled { get; set; } = true;
        public static int CurrentVolume { get; set; } = 100;

        public static void Play()
        {
            if (!IsEnabled) return;
            if (isPlaying) return;

            try
            {
                if (File.Exists(musicFilePath))
                {
                    waveOut = new WaveOutEvent();
                    audioFile = new AudioFileReader(musicFilePath);
                    waveOut.PlaybackStopped += OnPlaybackStopped;
                    waveOut.Init(audioFile);
                    SetVolume(CurrentVolume);
                    waveOut.Play();
                    isPlaying = true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Music file not found: {musicFilePath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing music: {ex.Message}");
            }
        }

        private static void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (IsEnabled && audioFile != null)
            {
                audioFile.Position = 0;
                waveOut?.Play();
            }
        }

        public static void Stop()
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.PlaybackStopped -= OnPlaybackStopped;
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }
                isPlaying = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping music: {ex.Message}");
            }
        }

        public static void SetVolume(int volumePercent)
        {
            CurrentVolume = Math.Max(0, Math.Min(100, volumePercent));
            if (audioFile != null)
                audioFile.Volume = CurrentVolume / 100f;
        }

        public static void Toggle()
        {
            IsEnabled = !IsEnabled;
            if (IsEnabled)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }
}