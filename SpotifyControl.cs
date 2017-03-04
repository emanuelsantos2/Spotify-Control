using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Spotify_Remote
{

    
    class SpotifyControl
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        public IntPtr windowHandle = IntPtr.Zero;
        public bool isConnected = false;
        private const uint WM_APPCOMMAND = 0x0319;
        private Process spotifyProcess;
        private string windowsUser = Environment.UserName;
        private const string settingsFile = "settings.ini";
        private const string backgroundFile = "Temp/tempBackground.jpg";
        public string nextKey, playKey, previousKey;
        public int processId;

        public enum SpotifyAction : long
        {
            PlayPause = 917504,
            Mute = 524288,
            VolumeDown = 589824,
            VolumeUp = 655360,
            Stop = 851968,
            PreviousTrack = 786432,
            NextTrack = 720896
        }


        public SpotifyControl()
        {
            this.windowHandle = GetSpotifyWindowsHandle();
            ReadSettings();
            CreateTempFolder();
        }

        private IntPtr GetSpotifyWindowsHandle()
        {
            bool contSearch = true;
            IntPtr hWnd = IntPtr.Zero;
            spotifyProcess = Process.Start("C:/Users/" + windowsUser +"/AppData/Roaming/Spotify/spotify.exe");
            System.Threading.Thread.Sleep(1000);

            while (contSearch)
            {
                Process[] processes = Process.GetProcesses();

                for (int i = 0; i < processes.GetLength(0); i++)
                {
                    if (processes[i].MainWindowTitle == "Spotify")
                    {
                        hWnd = processes[i].MainWindowHandle;
                        processId = processes[i].Id;
                        contSearch = false;
                    }
                }
            }
            isConnected = true;
            return hWnd;
        }


        private void CreateTempFolder()
        {
            if (!Directory.Exists("Temp"))
            {
                Directory.CreateDirectory("Temp");
            }
        }

        public void DownloadTempBackground(string song)
        {
            WebClient web = new WebClient();
            string imageUrl = "";
            bool imageFound = false;

            string json = web.DownloadString("https://api.spotify.com/v1/search?q=" + song + "&type=track&limit=1");
            string[] data = json.Split('"');

            for(int i = 0; i < data.Length; i++)
            {
                if(data[i] == "images")
                {
                    imageUrl = data[i + 6];
                    imageFound = true;
                }
            }

            if (imageFound)
            {
                if (File.Exists(backgroundFile))
                {
                    File.Delete(backgroundFile);
                }
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(imageUrl, backgroundFile);
                    client.Dispose();
                }
            }
        }

        private void ReadSettings()
        {
            if (File.Exists(settingsFile))
            {
                string keys = File.ReadAllText(settingsFile);
                string[] keysArray = keys.Split(',');

                playKey = keysArray[0];
                previousKey = keysArray[1];
                nextKey = keysArray[2];
            }
            else
            {
                playKey = "F8";
                previousKey = "F9";
                nextKey = "F10";
            }
        }

        public void SetSettings(string[] keys)
        {
            if (!File.Exists(settingsFile))
            {
                string content = ",,";
                File.WriteAllText(settingsFile, content);
                    
            }

            string fileText = File.ReadAllText(settingsFile);
            string[] arraySettings = fileText.Split(',');

            for(int i = 0; i < keys.Length; i++)
            {
                arraySettings[i] = keys[i];
            }

            File.WriteAllText(settingsFile, String.Join(",", arraySettings));
            ReadSettings();
        }

        public void DeleteSettings()
        {
            if (File.Exists(settingsFile))
            {
                File.Delete(settingsFile);
            }
        }

        public void SendCommand(long command)
        {
            SendMessage(windowHandle, WM_APPCOMMAND, IntPtr.Zero, new IntPtr(command));
        }
        
        public void CloseSpotify()
        {

            try
            {
                spotifyProcess.Kill();
            }
            catch(Exception e)
            {

            }
                
            
        }
        
    }
}

