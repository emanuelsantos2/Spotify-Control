using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hotkeys;

namespace Spotify_Remote
{
    public partial class Form1 : Form
    {
        HandleKey play, previous, next;
        SpotifyControl spotify;
        public Form1()
        {
            InitializeComponent();
            spotify = new SpotifyControl();
            play = new HandleKey(Constants.ALT + Constants.SHIFT, Keys.F10, this);
            previous = new HandleKey(Constants.ALT + Constants.SHIFT, Keys.F11, this);
            next = new HandleKey(Constants.ALT + Constants.SHIFT, Keys.F12, this);
        }


        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.WParam.ToInt32() == play.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.PlayPause);
                    WriteLine("Play/Pause");
                }

                if (m.WParam.ToInt32() == next.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.NextTrack);
                    WriteLine("Next");
                }

                if (m.WParam.ToInt32() == previous.GetHashCode())
                {
                    spotify.SendCommand((long)SpotifyControl.SpotifyAction.PreviousTrack);
                    WriteLine("Previous");
                }
            }
            catch(Exception e)
            {

            }
                
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (play.Register() && next.Register() && previous.Register())
                WriteLine("Connected to Spotify");
            else
                WriteLine("Not Conected to Spotify, Try Again");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            play.Unregister();
            next.Unregister();
            previous.Unregister();
            spotify.CloseSpotify();
        }

        private void WriteLine(string text)
        {
            label1.ResetText();
            label1.Text = text;
            System.Threading.Thread.Sleep(500);
            label1.Text = "...";
        }
    }
}
