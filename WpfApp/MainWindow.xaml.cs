using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using Path = System.IO.Path;
using InteropDll;

//--< using >--
using Microsoft.Win32;
using System.Windows.Media;
//--</ using >--

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process process;
        private string chromePath;
        private SolidColorBrush backColor = new SolidColorBrush(Colors.Transparent);
        private SolidColorBrush foreColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5D5C5C"));
        private SolidColorBrush hoverBackColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffd740"));
        private SolidColorBrush hoverForeColor = new SolidColorBrush(Colors.Black);

        public Visibility ShowDropMessage { get; set; } = Visibility.Visible;
        public Visibility ShowReopen { get; set; } = Visibility.Collapsed;

        public MainWindow()
        {
            InitializeComponent();

            chromePath = FindChrome();
            if (string.IsNullOrEmpty(chromePath))
            {
                lblDropMessage.Content = "Chrome not found";
                lblDropMessage.FontSize = 30;
                return;
            }

            Interop.Instance.OpenFileDialog += (s, e) => OpenFileDialog();
            Interop.Instance.Dragging += (s, e) => OnDragging();
            Interop.Instance.ConnectChange += onConnectChange;

            TryStartBrowser();
        }

        private string FindChrome()
        {
            var path = FindChrome(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            if (string.IsNullOrEmpty(path)) {
                path = FindChrome(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            }
            return path;
        }

        private string FindChrome(string dirPath)
        {
            dirPath += @"\Google";
            if (Directory.Exists(dirPath))
            {
                return SearchDir(dirPath, "chrome.exe");
            }
            return null;
        }

        private string SearchDir(string dirPath, string target)
        {
            try
            {
                string[] path = Directory.GetFiles(dirPath, "chrome.exe", SearchOption.TopDirectoryOnly);
                if (path != null && path.Length > 0)
                {
                    return path[0];
                }
            }
            catch
            {
                // We have no access to this directory
                return null;
            }
            foreach (var dir in Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly))
            {
                string path = SearchDir(dir, target);
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }
            return null;
        }

        private void TryStartBrowser()
        {
            if (Interop.Instance.IsConnected) return;

            int tryCntr = 0;
            var t = new System.Timers.Timer(250);
            t.Elapsed += (s, e) => {
                tryCntr++;
                if (Interop.Instance.IsConnected)
                {
                    t.Close();
                    return;
                }
                if (tryCntr > 7)
                {
                    t.Close();
                    startBrowser();
                }
            };
            t.Start();
        }

        private void startBrowser()
        {
            //var args = @"--headless --autoplay-policy=no-user-gesture-required --remote-debugging-port=9222 --allow-hidden-media-playback --ignore-autoplay-restrictions --enable-internal-media-session --app http://localhost:5000/index.html ";
            //var args = @"http://localhost:4200";
            var args = @"http://localhost:" + Interop.Instance.PortNumber + @"/index.html";

            var startInfo = new ProcessStartInfo();
            startInfo.Arguments = args;
            startInfo.FileName = chromePath;

            try
            {
                process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void onConnectChange(object s, EventArgs e)
        {
            Dispatcher.Invoke(() => {
                grdBrowser.Visibility = Interop.Instance.IsConnected ? Visibility.Collapsed : Visibility.Visible;
                lblDropMessage.Visibility = !Interop.Instance.IsConnected ? Visibility.Collapsed : Visibility.Visible;
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }
                Activate();
            });
        }

        private void sendPlaylist(string[] files)
        {
            Interop.Instance.PlaylistToClient(files);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CloseBrowser();
            base.OnClosing(e);
        }

        private void CloseBrowser() {
            Interop.Instance.ConnectChange -= onConnectChange;
            if (process != null && !process.HasExited)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Kill error: " + ex.Message);
                }
                process = null;
            }
        }

        private Point ptStart = new Point(-1, -1);

        // Move the window under the mouse so we can intercept the file drop
        private void OnDragging()
        {
            Dispatcher.Invoke(() => {
                Mouse.Capture(this);
                var relative = Mouse.GetPosition(this);
                var point = PointToScreen(relative);
                Mouse.Capture(null);

                if (ptStart.X < 0)
                {
                    ptStart = new Point(this.Left, this.Top);
                }

                Activate();
                Top = point.Y - (this.Height / 2);
                Left = point.X - (this.Width / 2);
            });
        }

        private void OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                sendPlaylist(openFileDialog.FileNames);
            }
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            sendPlaylist(files);

            if (ptStart.X >= 0)
            {
                // Move window back to where it was (see OnDragging())
                this.Top = ptStart.Y;
                this.Left = ptStart.X;
                ptStart = new Point(-1, -1);
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            startBrowser();
        }

        private void MainGrid_MouseEnter(object sender, DragEventArgs e)
        {
            lblDropMessage.Foreground = hoverForeColor;
        }

        private void MainGrid_MouseLeave(object sender, EventArgs e)
        {
            lblDropMessage.Foreground = foreColor;
        }

        private void BtnGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            btnGrid.Background = hoverBackColor;
            xpath.Stroke = hoverForeColor;
            xpath.Fill = hoverForeColor;
        }

        private void BtnGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            btnGrid.Background = backColor;
            xpath.Stroke = foreColor;
            xpath.Fill = foreColor;
        }

        private void BtnBrowser_MouseEnter(object sender, MouseEventArgs e)
        {
            btnBrowser.Background = hoverBackColor;
            bpath.Stroke = hoverForeColor;
            bpath.Fill = hoverForeColor;
        }

        private void BtnBrowser_MouseLeave(object sender, MouseEventArgs e)
        {
            btnGrid.Background = backColor;
            bpath.Stroke = foreColor;
            bpath.Fill = foreColor;
        }

        private void BtnGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Handle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #region Drag window
        // Because this is a frameless window, we have to take care of moving the window ourselves
        private double dx;
        private double dy;

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = PointToScreen(e.GetPosition(this));
                dx = Left - pt.X;
                dy = Top - pt.Y;
                Mouse.Capture(this);
                MouseUp += onMouseUp;
                MouseMove += onMouseMove;
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            ReleaseMouseCapture();
            MouseUp -= onMouseUp;
            MouseMove -= onMouseMove;
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pt = PointToScreen(e.GetPosition(this));
                Left = pt.X + dx;
                Top = pt.Y + dy;
            }
        }
        #endregion

    }
}
