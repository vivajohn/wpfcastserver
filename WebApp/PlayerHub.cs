using InteropDll;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace WebApp
{
    public class PlayerHub : Hub
    {
        protected IHubContext<PlayerHub> _context;

        public PlayerHub(IHubContext<PlayerHub> context)
        {
            _context = context as IHubContext<PlayerHub>;
        }


        private void onListChanged(object s, Interop.PlaylistEventArgs e)
        {
            AddItems(e.Playlist).Wait();
        }

        public override Task OnConnectedAsync()
        {
            Interop.Instance.IsConnected = true;
            Interop.Instance.PlaylistAdd += onListChanged;

            if (Interop.Instance.Library != null)
            {
                SendLibrary(Interop.Instance.Library).Wait(0);
            }
            else
            {
                var list = Interop.Instance.Playlist;
                if (list != null) {
                    AddItems(list).Wait(0);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Interop.Instance.IsConnected = false;
            Interop.Instance.PlaylistAdd -= onListChanged;
            return base.OnDisconnectedAsync(exception);
        }

        #region Messages FROM client

        public async Task Clear()
        {
            Interop.Instance.Clear();
            await SendLibrary(null);
        }

        public async Task GetLibrary(string id)
        {
            var libInfo = Interop.Instance.GetLibrary(id);
            Interop.Instance.SetNewLibrary(libInfo);
            await SendLibrary(libInfo);
        }

        public async Task SaveLibrary(Libraries.LibInfo libInfo)
        {
            await SendLibrary(Interop.Instance.SaveLibrary(libInfo));
            await SendLibraryNames(Interop.Instance.LibraryNames);
        }

        public async Task DeleteLibrary(Libraries.LibHeader libHeader)
        {
            Interop.Instance.DeleteLibrary(libHeader);
            var libCurrent = Interop.Instance.Library;
            if (libCurrent != null && libHeader.libPath == libCurrent.libPath)
            {
                await Clear();
            }
            await SendLibraryNames(Interop.Instance.LibraryNames);
        }

        public void OpenWpfFileDialog()
        {
            Interop.Instance.OpenWpfFileDialog();
        }

        #endregion


        #region Messages TO client

        public async Task AddItems(PlayItem[] list)
        {
            await _context.Clients.All.SendAsync("AddItems", JsonConvert.SerializeObject(list));
        }

        public async Task SendLibraryNames(Libraries.LibHeader[] names)
        {
            await _context.Clients.All.SendAsync("LibraryNames", JsonConvert.SerializeObject(names));
        }

        public async Task MakeNewLibrary(Libraries.LibInfo libInfo)
        {
            libInfo = Interop.Instance.SaveLibrary(libInfo);
            await SendLibrary(libInfo);
            await GetLibraryNames();
        }

        public async Task SendLibrary(Libraries.LibInfo libInfo)
        {
            await _context.Clients.All.SendAsync("Library", JsonConvert.SerializeObject(libInfo));
        }

        public async Task GetLibraryNames()
        {
            await SendLibraryNames(Interop.Instance.LibraryNames);
        }

        public void Drag()
        {
            Interop.Instance.Drag();
        }

        public void GetPlaylist(string listName)
        {
            System.Diagnostics.Debug.WriteLine("GetPlaylist: " + listName);
        }

        #endregion
    }

}
