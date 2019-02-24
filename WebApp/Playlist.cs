using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    public interface IPlaylist {
        string Next();
        string PeekNext();
        //List<PlayItem> FileList { get; set; }
    }

    public class Playlist : IPlaylist
    {
        //private string[] mp3 = new string[] { };
        //private string[] mp3 = new string[] {
        //    "C:/Users/John/Music/mix1/fire-crackling.mp3",
        //    "C:/Users/John/Music/mix1/01 - Tiroler Holzhackermarsch.mp3",
        //    "C:/Users/John/Music/mix1/fire-crackling.mp3",
        //    "C:/Users/John/Music/mix2/01 Jimmy Somerville - Comment Te Dire AdieuÂ¿.flac",
        //    "C:/Users/John/Music/mix2/02 Ohne dich.m4a",
        //    "C:/Users/John/Music/mix1/01 - a clarinet-dsharp-major.mp3",
        //    "C:/Users/John/Music/mix1/07 - Non Succedera Piu.mp3"
        //};
        private int index = -1;

        public List<PlayItem> FileList { get; set; }

        public Playlist()
        {
            FileList = new List<PlayItem>();
        }

        public string Next()
        {
            index = (index + 1) % FileList.Count;
            return FileList[index].url;
        }

        public string PeekNext()
        {
            var i = index;
            var next = Next();
            index = i;
            return next;
            //}
        }
    }
}
