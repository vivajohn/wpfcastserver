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
        // POST api/playitem - plays back an audio file
        [HttpPost("playitem")]
        public FileStreamResult Playitem([FromBody] PlayItem item)
        {
            var stream = new FileStream(item.url, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, "application/octet-stream");
        }

        // GET api/radio - plays back internet radio
        [HttpGet("radio/{id}")]
        public async Task<FileStreamResult> Radio(string id)
        {
            long i = long.Parse(id);
            var item = Interop.Instance.Playlist.Single(x => x.id == i);
            var client = new HttpClient();
            var stream = await client.GetStreamAsync(item.url);
            return new FileStreamResult(stream, "audio/mpeg");
        }

        // GET api/music - can be used to test if the server is running
        [HttpGet]
        public ActionResult<string> Get()
        {
            return JsonConvert.SerializeObject(new { running = true });
        }

        // POST api/trace - for debugging
        [HttpPost("trace")]
        public void Trace([FromBody] string value)
        {
            System.Diagnostics.Debug.WriteLine("**** " + value);
        }
    }
}
