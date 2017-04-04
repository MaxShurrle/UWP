using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        List<string> _DownloadUrls = new List<string>
        {
            "https://hoge.blob.core.windows.net/movie/hoge1.mkv",
            "https://hoge.blob.core.windows.net/movie/hoge2.mkv",
            "https://hoge.blob.core.windows.net/movie/hoge3.mkv",
            "https://hoge.blob.core.windows.net/movie/hoge4.mkv",
            "https://hoge.blob.core.windows.net/movie/hoge5.mkv",
            "https://hoge.blob.core.windows.net/movie/hoge6.mkv",
        };

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            progress.Maximum = _DownloadUrls.Count;
            progress.Value = 0;

            for(int i = 0; i < _DownloadUrls.Count; i++)
            {
                try
                {
                    var download = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync($"movie{i}.mkv", CreationCollisionOption.ReplaceExisting);

                    var client = new HttpClient();
                    var buffer = await client.GetByteArrayAsync(_DownloadUrls[i]);

                    using (var stream = await download.OpenStreamForWriteAsync())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    files.Add(download);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                progress.Value++;
            }

            btnPlay.IsEnabled = true;

        }
        
        List<StorageFile> files = new List<StorageFile>();

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            pnlController.Visibility = Visibility.Collapsed;
            player.Visibility = Visibility.Visible;

            var index = 0;
            while (true)
            {
                var mediaPlayer = new MediaPlayer();
                mediaPlayer.RealTimePlayback = true;
                mediaPlayer.AutoPlay = false;
                mediaPlayer.MediaFailed += (sender2, e2) =>
                {
                    
                };

                mediaPlayer.Source = MediaSource.CreateFromStorageFile(files[index]);

                var old = player.MediaPlayer;
                if (old != null)
                {
                    old.Pause();
                }

                player.SetMediaPlayer(mediaPlayer);
                mediaPlayer.Play();

                if(old != null)
                {
                    var source = old.Source as MediaSource;
                    if (source != null)
                    {
                        source.Dispose();
                    }
                    old.Dispose();
                }

                index++;
                if(index == files.Count)
                {
                    index = 0;
                }
                await Task.Delay(10000);
            }
        }
    }
}
