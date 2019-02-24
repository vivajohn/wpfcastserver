using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InteropDll
{
    // Manages libraries, which are files containing lists of file paths which make up
    // a playlist.
    public class Libraries
    {
        public class LibHeader
        {
            public string libPath;  // Note that this acts as a unique id
            public string name;
        }
        public class LibInfo : LibHeader
        {
            public PlayItem[] items;
        }

        private readonly string AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CastClient";

        public Libraries()
        {
            Directory.CreateDirectory(AppFolder);
        }

        public LibHeader[] Names
        {
            get {
                //var infos = new List<LibInfo>();
                var files = Directory.GetFiles(AppFolder, "cc_*.json");
                var infos = files.Select<string, LibHeader>(filePath => {
                    var info = JsonConvert.DeserializeObject<LibInfo>(File.ReadAllText(filePath));
                    return new LibHeader() { libPath = info.libPath, name = info.name };
                });
                var dbug = infos.ToArray();
                return infos.ToArray();
            }
        }

        public LibInfo GetLibrary(string libPath)
        {
            var info = JsonConvert.DeserializeObject<LibInfo>(File.ReadAllText(libPath));
            return info;
        }

        public LibInfo Save(LibInfo libInfo)
        {
            if (string.IsNullOrEmpty(libInfo.libPath))
            {
                // 10000 ticks per millisecond
                libInfo.libPath = $"{ AppFolder}\\cc_{(DateTime.Now.Ticks / 10000).ToString()}.json";
            }
            File.WriteAllText(libInfo.libPath, JsonConvert.SerializeObject(libInfo));
            return libInfo;
        }

        public void Delete(string libPath)
        {
            if (string.IsNullOrEmpty(libPath)) return;
            File.Delete(libPath);
        }

        public PlayItem[] MakePlaylist(string[] paths)
        {
            var list = new List<PlayItem>();
            foreach (var path in paths)
            {
                string url = null;
                FileTypes fileType;
                if (Path.GetExtension(path) == ".m3u")
                {
                    fileType = FileTypes.Radio;
                    url = extractRadioUrl(path);
                }
                else
                {
                    fileType = FileTypes.Stream;
                    var mimeType = MimeMapping.MimeUtility.GetMimeMapping(path);
                    if (mimeType.StartsWith(@"audio/"))
                    {
                        url = path;
                    }
                }
                if (!string.IsNullOrEmpty(url))
                {
                    list.Add(new PlayItem(Path.GetFileNameWithoutExtension(path), fileType, url));
                }
            }

            return list.ToArray();
        }

        private string extractRadioUrl(string path)
        {
            var lines = File.ReadLines(path).ToArray();
            return lines != null && lines.Length > 0 ? lines[0] : null;
        }
    }
}
