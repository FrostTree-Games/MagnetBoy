using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MagnetBoy
{
    class AudioFactory
    {
        private static ContentManager manager = null;

        private static Dictionary<string, Song> bgmLib = null;
        private static bool initalized = false;

        private static string currentSongName = null;

        public AudioFactory(ContentManager newManager)
        {
            initalize(newManager);
        }

        private void initalize(ContentManager newManager)
        {
            if (initalized)
            {
                return;
            }

            initalized = true;

            manager = newManager;

            bgmLib = new Dictionary<string, Song>();

            currentSongName = "";
        }

        public static void pushNewSong(string songName)
        {
            Song s = null;

            try
            {
                s = manager.Load<Song>(songName);
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("(a) Song not loaded" + songName);
                return;
            }

            if (s != null)
            {
                bgmLib.Add(songName, s);
            }
            else
            {
                Console.WriteLine("(b) Song not loaded" + songName);
                return;
            }
        }

        //stops any other song playing and plays the specified song
        public static void playSong(string songName)
        {
            MediaPlayer.Stop();

            currentSongName = songName;

            MediaPlayer.Play(bgmLib[songName]);
        }

        public static void stopSong()
        {
            currentSongName = "";

            MediaPlayer.Stop();
        }
    }
}
