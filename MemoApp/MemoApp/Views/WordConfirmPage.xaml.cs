using MemoApp.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: ExportFont("SmileySans-Oblique.ttf", Alias = "Smiley Sans")]

namespace MemoApp.Views
{

    [QueryProperty(nameof(Word), nameof(Word))]
    public partial class WordConfirmPage : ContentPage
    {
        public string Word { get; set; }
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;

        private async void OnSkipButtonClicked(object sender, EventArgs e)
        {
            if (_localStorage.Current != null)
            {
                int currentFamiliar = _localStorage.Current.familiar_degree;
                if (currentFamiliar == 0)
                {
                    // skip and never shows again
                    GlobalClasses.Index.WordsToStudy -= 1;
                    bool hasNext = _localStorage.MoveNext();
                    if (!hasNext)
                    {
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        string nextWord = _localStorage.Current.word;
                        WordConfirmPage nextWordConfirm = new WordConfirmPage
                        {
                            Word = nextWord
                        };
                        await Navigation.PushAsync(nextWordConfirm);
                    }
                }
                else
                {
                    // enter detail page
                    WordDetailPage thisDetailPage = new WordDetailPage();
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
                WordDetailPage thisDetailPage = new WordDetailPage();
                thisDetailPage.Word = Word;
                await Navigation.PushAsync(thisDetailPage);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // adjust font size automatically
            WordLabel.Text = Word;
            if (WordLabel.Text.Length <= 9)
            {
                WordLabel.FontSize = 60;
            }
            else if(WordLabel.Text.Length <= 16)
            {
                WordLabel.FontSize = 50;
            }
            else
            {
                WordLabel.FontSize = 42;
            }
            // set familiar label according to familiar degree
            if (_localStorage.Current != null)
            {
                if (_localStorage.Current.familiar_degree > 0)
                {
                    // SET TO: "LEVEL-x"
                    FamiliarLabel.Text = "LEVEL-" + _localStorage.Current.familiar_degree;
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            
            return true;
        }

        public WordConfirmPage()
        {
            InitializeComponent();
        }

        private async void SwipeDownQuit(object sender, SwipedEventArgs e)
        {
            bool quit = 
                await DisplayAlert("Quit?", "Your progress will be saved", "Quit", "Continue");
            if (quit)
            {
                await Navigation.PopToRootAsync();
            }
        }
    }
}