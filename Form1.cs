using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Hotkeys;

namespace Spotify_Remote
{
    public partial class Form1 : Form
    {
        HandleKey play, previous, next, volumeDown, volumeUp;
        SpotifyControl spotify;

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));
        System.Threading.Thread labelUpdate;
        public Form1()
        {
            InitializeComponent();
            spotify = new SpotifyControl();
            play = new HandleKey(Constants.ALT + Constants.SHIFT, (Keys)converter.ConvertFromString(spotify.playKey), this);
            previous = new HandleKey(Constants.ALT + Constants.SHIFT, (Keys)converter.ConvertFromString(spotify.previousKey), this);
            next = new HandleKey(Constants.ALT + Constants.SHIFT, (Keys)converter.ConvertFromString(spotify.nextKey), this);
            volumeDown = new HandleKey(Constants.ALT + Constants.SHIFT, Keys.Down, this);
            volumeUp = new HandleKey(Constants.ALT + Constants.SHIFT, Keys.Up, this);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string[] arraySettings = new string[3] {comboPlay.Text, comboPrevious.Text, comboNext.Text };


            if (KeyIsNotRepeated(arraySettings))
            {
                spotify.SetSettings(arraySettings);
                WriteLine("Settings Saved");
                UnregisterAll();
                RegisterAll();
            }
            else
            {
                WriteLine("One HotKey is Repeated");
            }
        }

        public void RegisterAll()
        {
            play.Register();
            previous.Register();
            next.Register();
        }

        public void UnregisterAll()
        {
            play.Unregister();
            previous.Unregister();
            next.Unregister();
        }

        public static bool KeyIsNotRepeated(string[] valueArray)
        {
            string[] key = new string[5];
            int[] keyOccurences = new int[5] {0,0,0,0,0};
            key[0] = "F8";
            key[1] = "F9";
            key[2] = "F10";
            key[3] = "F11";
            key[4] = "F12";

            for (int b = 0; b < key.Length; b++)
            {
                for (int i = 0; i < valueArray.Length; i++)
                {
                    if (valueArray[i] == key[b])
                    {
                        keyOccurences[b] ++;
                    }
                }
            }


            if (keyOccurences.Contains(2) || keyOccurences.Contains(3) || keyOccurences.Contains(4))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            spotify.DeleteSettings();
            WriteLine("Settings Deleted");
            UnregisterAll();
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.WParam.ToInt32() == play.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.PlayPause);
                }

                if (m.WParam.ToInt32() == next.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.NextTrack);
                }

                if (m.WParam.ToInt32() == previous.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.PreviousTrack);
                }

                if (m.WParam.ToInt32() == volumeDown.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.VolumeDown);
                }

                if (m.WParam.ToInt32() == volumeUp.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.VolumeUp);
                }
            }
            catch(Exception e)
            {

            }
                
            base.WndProc(ref m);
        }
        

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
            this.Show();
        }

        private void trayButton_MouseClick(object sender, MouseEventArgs e)
        {
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
                this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboPlay.SelectedText = spotify.playKey;
            comboPrevious.SelectedText = spotify.previousKey;
            comboNext.SelectedText = spotify.nextKey;
            labelUpdate = new System.Threading.Thread(UpdateLabel);
            labelUpdate.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            play.Unregister();
            next.Unregister();
            previous.Unregister();
            volumeDown.Unregister();
            volumeUp.Unregister();
            spotify.CloseSpotify();
            labelUpdate.Abort();
        }

        private void WriteLine(string text)
        {
            
            label1.ResetText();
            label1.Text = text;
        }

        private void UpdateLabel()
        {
            while (true) {
                System.Threading.Thread.Sleep(200);
                Process spotifyProcess = Process.GetProcessById(spotify.processId);
                string spotifyTitle = spotifyProcess.MainWindowTitle;
                try
                {
                    BeginInvoke((MethodInvoker)delegate ()
                    {
                        label1.Text = spotifyTitle;
                        System.Threading.Thread.Sleep(5);
                    });
                }
                catch (Exception) { }
                
                
            }
            
        }
    }
}
