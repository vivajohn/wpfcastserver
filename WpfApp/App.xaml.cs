using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebApp;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //string[] args = new string[] { "applicationUrl", "http://localhost:5000" };
            string[] args = new string[] {};
            var task = WebApp.Startup.Main(args);
            System.Diagnostics.Debug.WriteLine("*** END WebDll.Startup");
            host = task.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Task.Run(() => {
                host.StopAsync(TimeSpan.FromSeconds(2)).ContinueWith(x =>
                {
                    host.Dispose();
                });
            }).Wait();
            base.OnExit(e);
        }
    }
}
