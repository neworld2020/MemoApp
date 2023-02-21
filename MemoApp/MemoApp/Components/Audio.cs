using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.SimpleAudioPlayer;
using Xamarin.Forms;

namespace MemoApp.Components
{
    public class Audio : View
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            propertyName: "Source",
            returnType: typeof(Uri),
            declaringType: typeof(Audio),
            defaultValue: new Uri("http://cloud-smx2003.fun/")
        );

        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set
            {
                if (value.ToString().StartsWith("https"))
                {
                    SetValue(SourceProperty, value);
                }
                else
                {
                    Console.WriteLine("Only Support HTTPS Protocol!");
                }
            }
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create(
            propertyName: "AutoPlay",
            returnType: typeof(bool),
            declaringType: typeof(Audio),
            defaultValue: false
        );

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        private static readonly ISimpleAudioPlayer Player = CrossSimpleAudioPlayer.Current;

        public bool Play()
        {
            bool loadSucceed =  Player.Load(GetStreamAsync().Result);
            if (!loadSucceed) return false;
            Player.Play();
            return true;
        }

        private async Task<Stream> GetStreamAsync()
        {
            // uri
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, Source.ToString());
            var result = client.SendAsync(request).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                Stream audioStream = await result.Content.ReadAsStreamAsync();
                return audioStream;
            }
            return null;
        }
    }
}