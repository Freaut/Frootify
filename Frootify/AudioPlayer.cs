using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Threading;

namespace Frootify
{
    public class AudioPlayer
    {
        private AudioFileReader? audioFile;
        private WaveOutEvent outputDevice;
        private Thread audioThread;
        public bool isPlaying;
        public bool isPaused;
        private float _volume;
        private string _loaded_file_path = string.Empty;
        private string _current_file_path = string.Empty;
        public static event EventHandler<string>? SongFinishedEvent;

        public AudioPlayer(float volume)
        {
            outputDevice = new WaveOutEvent();
            isPlaying = false;
            isPaused = false;
            _volume = volume;

            audioThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        PlayAudio();
                    }
                    catch (Exception e) { Debug.WriteLine(e); }
                }
            });
            audioThread.Start();
        }

        ~AudioPlayer()
        {
            Terminate();
        }

        public void Terminate()
        {
            try
            {
                // audioThread.Suspend();
                outputDevice.Stop();
                outputDevice.Dispose();
                audioFile?.Dispose();
            }
            catch { }
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }

        public void Play(string path)
        {
            _loaded_file_path = path;
            isPlaying = true;
            isPaused = false;
        }

        private void PlayAudio()
        {
            if (isPaused)
            {
                outputDevice.Pause();
                return;
            }

            if (!isPlaying)
                return;

            if (string.IsNullOrEmpty(_loaded_file_path))
                return;

            try
            {
                if (audioFile == null)
                {
                    audioFile = new AudioFileReader(_loaded_file_path);
                    _current_file_path = _loaded_file_path;
                    audioFile.Volume = _volume;
                    outputDevice.Init(audioFile);
                }

                isPlaying = true;
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (_current_file_path != _loaded_file_path)
                    {
                        outputDevice.Stop();
                        _current_file_path = _loaded_file_path;
                        audioFile = null;
                        break;
                    }

                    audioFile.Volume = _volume;
                    Thread.Sleep(100);
                }

                if (audioFile != null)
                    if (audioFile.CurrentTime >= audioFile.TotalTime)
                        SongFinishedEvent?.Invoke(this, _loaded_file_path);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                isPlaying = false;
                isPaused = true;
            }

            Thread.Sleep(1);
        }

        public void Skip(double time)
        {
            if (audioFile == null)
                return;

            try
            {
                audioFile.CurrentTime = TimeSpan.FromSeconds(time);
            }
            catch { }
        }

        public void Pause()
        {
            outputDevice.Pause();
            isPaused = true;
            isPlaying = false;
        }

        public void Resume()
        {
            isPaused = false;
            isPlaying = true;
            outputDevice.Play();
        }

        public void Stop()
        {
            outputDevice.Stop();
            isPlaying = false;
            isPaused = true;
        }
    }
}