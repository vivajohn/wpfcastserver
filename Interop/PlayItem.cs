using System;
using System.Collections.Generic;
using System.Text;

namespace InteropDll
{
    public class PlayItem
    {
        private static long _id = DateTime.Now.Ticks /10000;

        public long id;
        public string name;
        public FileTypes type;
        public string url;

        public PlayItem(string name, FileTypes type, string url)
        {
            this.id = _id;
            _id++;

            this.name = name;
            this.type = type;
            this.url = url;
        }
    }
}
