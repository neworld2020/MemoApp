using System;
using MemoApp.Models;
using Xamarin.Forms;

[assembly: ExportFont("SmileySans-Oblique.ttf", Alias = "Smiley Sans")]

namespace MemoApp.Views
{
    [QueryProperty(nameof(Word), nameof(Word))]
    public partial class WordConfirmPage : ContentPage
    {
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;

        public WordConfirmPage()
        {
            InitializeComponent();
        }

        public string Word { get; set; }

        private async void OnSkipButtonClicked(object sender, EventArgs e)
        {
            if (_localStorage.Current != null)
            {
                var currentFamiliar = _localStorage.Current.familiar_degree;
                if (currentFamiliar == 0)
                {
                    // skip and never shows again
                    //GlobalClasses.Index.WordsToStudy -= 1;
                    var hasNext = _localStorage.MoveNext();
                    if (!hasNext)
                    {
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        var nextWord = _localStorage.Current.word;
                        var nextWordConfirm = new WordConfirmPage
                        {
                            Word = nextWord
                        };
                        await Navigation.PushAsync(nextWordConfirm);
                    }
                }
                else
                {
                    // enter detail page
                    var thisDetailPage = new WordDetailPage();
                    thisDetailPage.Word = Word;
                    await Navigation.PushAsync(thisDetailPage);
                }
            }
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            if (_localStorage.Current != null)
            {
                // reset familiar degree
                _localStorage.Current.familiar_degree = 0;
                // enter detail page
                var thisDetailPage = new WordDetailPage();
                thisDetailPage.Word = Word;
                await Navigation.PushAsync(thisDetailPage);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // adjust font size automatically
            WordLabel.Text = Word;
            if (WordLabel.Text.Length <= 7)
                WordLabel.FontSize = 60;
            else if (WordLabel.Text.Length <= 10)
                WordLabel.FontSize = 50;
            else
                WordLabel.FontSize = 42;
            // set familiar label according to familiar degree
            if (_localStorage.Current != null)
                if (_localStorage.Current.familiar_degree > 0)
                    // SET TO: "LEVEL-x"
                    FamiliarLabel.Text = "LEVEL-" + _localStorage.Current.familiar_degree;
            UriBuilder audioUri = new UriBuilder("https://dict.youdao.com/dictvoice");
            audioUri.Query = $"type=0&audio={Word}";
            WordAudio.Source = audioUri.Uri;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                WordAudio.Play();
                return false;
            });
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void SwipeDownQuit(object sender, SwipedEventArgs e)
        {
            var quit =
                await DisplayAlert("Quit?", "Your progress will be saved", "Quit", "Continue");
            if (quit) await Navigation.PopToRootAsync();
        }
    }
}