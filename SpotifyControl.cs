using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

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
        }

        private IntPtr GetSpotifyWindowsHandle()
        {
            bool contSearch = true;
            IntPtr hWnd = IntPtr.Zero;
            spotifyProcess = Process.Start("C:/Users/emaev/AppData/Roaming/Spotify/spotify.exe");
            System.Threading.Thread.Sleep(1000);

            while (contSearch)
            {
                Process[] processes = Process.GetProcesses();

                for (int i = 0; i < processes.GetLength(0); i++)
                {
                    if (processes[i].MainWindowTitle.Contains("Spotify"))
                    {
                        hWnd = processes[i].MainWindowHandle;
                        contSearch = false;
                    }
                }
            }
            isConnected = true;
            return hWnd;
        }

        public void SendCommand(long command)
        {
            SendMessage(windowHandle, WM_APPCOMMAND, IntPtr.Zero, new IntPtr(command));
        }
        
        public void CloseSpotify()
        {
            
                spotifyProcess.Kill();
            
        }
        
    }
}

