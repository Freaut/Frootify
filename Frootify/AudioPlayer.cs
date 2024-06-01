using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

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
        private bool KillThread = false;
        
        public List<DevicePair> Devices { get; set; }

        public AudioPlayer(float volume, int devicenumber = -1)
        {
            Devices = new List<DevicePair>();
            outputDevice = new WaveOutEvent() { DeviceNumber = devicenumber };
            isPlaying = false;
            isPaused = false;
            _volume = volume;
            KillThread = false;

            // NAudio.Wave.DirectSoundOut.Devices.

            Debug.WriteLine($"Default: {WaveOut.GetCapabilities(outputDevice.DeviceNumber).ProductName}");

            try
            {
                for (int idx = 0; idx <= NAudio.Wave.WaveOut.DeviceCount; idx++)
                {
                    string devName = NAudio.Wave.WaveOut.GetCapabilities(idx).ProductName;
                    // Debug.WriteLine(devName);
                    Devices.Add(new DevicePair(idx, devName));
                }
            }
            catch { }

            audioThread = new Thread(() =>
            {
                while (!KillThread)
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

        public List<DevicePair> GetDevices()
        {
            List<DevicePair> temp = new List<DevicePair>();

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                temp.Add(new DevicePair(i, capabilities.ProductName));
                Debug.WriteLine(capabilities.ProductName);
            }

            return temp;
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
                KillThread = true;
                outputDevice.Stop();
                outputDevice.Dispose();
                audioFile?.Dispose();
                audioFile = null;
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