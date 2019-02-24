using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using InteropDll;
//using Microsoft.Extensions.FileProviders;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        //public MusicController() { }

        // GET api/music
        [HttpGet]
        public ActionResult<string> Get()
        {
            return JsonConvert.SerializeObject(new { running = true });
        }

        // POST api/playitem
        [HttpPost("playitem")]
        public FileStreamResult Playitem([FromBody] PlayItem item)
        {
            var stream = new FileStream(item.url, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "application/octet-stream");
        }

        // POST api/radio
        [HttpGet("radio/{id}")]
        public async Task<FileStreamResult> Radio(string id)
        {
            long i = long.Parse(id);
            var item = Interop.Instance.Playlist.Single(x => x.id == i);
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(item.url);
            return new FileStreamResult(stream, "audio/mpeg");
        }

        // POST api/trace
        [HttpPost("trace")]
        public void Trace([FromBody] string value)
        {
            System.Diagnostics.Debug.WriteLine("**** " + value);
        }

        // GET api/music
        //[HttpGet("peeknext")]
        //public ActionResult<object> PeekNext()
        //{
        //    var type = playlist.PeekNext().StartsWith("http") ? "audio/mpeg" : "application/octet-stream";
        //    return new { contentType = type };
        //}

        // Make method for returning just radio stream
        //[HttpGet("radio/{id}")]
        //public async Task<FileStreamResult> Radio(int id)
        //{
        //    var item = this.playlist.FileList.Single(x => x.id == id);
        //    var client = new HttpClient();
        //    var stream = await client.GetStreamAsync(item.url);
        //    var fs = new FileStreamResult(stream, "audio/mpeg");
        //    return fs;
        //}
        //public async Task<FileStreamResult> Radio()
        //{
        //    string contentType;
        //    contentType = "audio/mpeg";
        //    //contentType = "application/octet-stream";
        //    try
        //    {
        //        var client = new HttpClient();
        //        Stream stream;
        //        var name = playlist.Next();
        //        stream = await client.GetStreamAsync(name);
        //        var fs = new FileStreamResult(stream, contentType);
        //        return fs;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        throw;
        //    }
        //    //var fs = new FileStreamResult(stream, contentType);
        //    //return fs;
        //}

        // GET next file to play
        //[HttpGet("next")]
        //public async Task<FileStreamResult> Next()
        //{
        //    //if (fstream != null && fstream.Position < fstream.Length)
        //    //{
        //    //    return fstream;
        //    //}
        //    var name = playlist.Next();
        //    Stream stream;
        //    string contentType;
        //    if (name.StartsWith("http")) 
        //    {
        //        contentType = "audio/mpeg";
        //        //contentType = "application/octet-stream";
        //        var client = new HttpClient();
        //        stream = await client.GetStreamAsync(name);
        //    }
        //    else
        //    {
        //        contentType = "application/octet-stream";
        //        stream = new FileStream(name, FileMode.Open, FileAccess.Read);
        //    }
        //    var fstream = new FileStreamResult(stream, contentType);
        //    return fstream;
        //    //return new FileStreamResult(stream, "application/octet-stream");
        //}

        // POST api/playfile
        //[HttpGet("playfile/{id}")]
        //public FileStreamResult Playfile(int id)
        //{
        //    var item = this.playlist.FileList.Single(x => x.id == id);
        //    var stream = new FileStream(item.url, FileMode.Open, FileAccess.Read);
        //    return new FileStreamResult(stream, "application/octet-stream");
        //}

        // POST api/playlist
        //[HttpPost("playlist")]
        //public void Playlist([FromBody] PlayItem[] items)
        //{
        //    // TO DO: test for duplicates?
        //    playlist.FileList.AddRange(items);
        //    hub.AddItems(playlist.FileList);
        //}

        // DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
