using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteropDll
{
    // This is a singleton which is used to communicate between WebApp and WpfApp. Because the classes
    // in WebApp are created by the system using dependency injection, and they may have a short lifetime,
    // the code in WpfApp in unable to access them directly.
    public class Interop
    {
        public class PlaylistEventArgs : EventArgs
        {
            public PlayItem[] Playlist { get; set; }
        }
        public event EventHandler<PlaylistEventArgs> PlaylistAdd;

        private static Interop _instance = null;

        public static Interop Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Interop();
                }
                return _instance;
            }
        }

        public readonly string PortNumber = "31469";
        public event EventHandler OpenFileDialog;
        public event EventHandler Dragging;
        public event EventHandler ConnectChange;
        public Libraries.LibInfo Library { get; set; } = null;

        private Libraries libraries = new Libraries();

        private Interop() {}

        private PlayItem[] _playlist;
        public PlayItem[] Playlist
        {
            get { return _playlist; }
            set {
                _playlist = value;
                PlaylistAdd?.Invoke(this, new PlaylistEventArgs() { Playlist = _playlist });
            }
        }

        private bool isConnected = false;
        public bool IsConnected {
            get => isConnected;
            set {
                isConnected = value;
                ConnectChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetNewLibrary(Libraries.LibInfo libInfo)
        {
            Library = libInfo;
            _playlist = libInfo.items;
        }

        public void Clear()
        {
            Playlist = null;
            Library = null;
        }

        public void Drag()
        {
            Dragging?.Invoke(this, EventArgs.Empty);
        }

        public void OpenWpfFileDialog()
        {
            OpenFileDialog?.Invoke(this, EventArgs.Empty);
        }

        public void PlaylistToClient(string[] filePaths)
        {
            Playlist = libraries.MakePlaylist(filePaths);
        }

        public Libraries.LibHeader SaveLibrary(Libraries.LibInfo libInfo)
        {
            libInfo = libraries.Save(libInfo);
            return new Libraries.LibHeader() { libPath = libInfo.libPath, name = libInfo.name };
        }

        public void DeleteLibrary(Libraries.LibHeader libHeader)
        {
            libraries.Delete(libHeader.libPath);
        }

        public Libraries.LibHeader[] LibraryNames
        {
            get {
                return libraries.Names;
            }
        }

        public Libraries.LibInfo GetLibrary(string id)
        {
            var lib = libraries.GetLibrary(id);
            _playlist = lib.items;
            return lib;
        }
    }
}
