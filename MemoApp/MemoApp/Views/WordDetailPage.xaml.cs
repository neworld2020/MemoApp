using MemoApp.Models;
using System;

using Xamarin.Forms;


[assembly: ExportFont("SmileySans-Oblique.ttf", Alias = "Smiley Sans")]

namespace MemoApp.Views
{
    [QueryProperty(nameof(Word), nameof(Word))] 
    public partial class WordDetailPage : ContentPage
    {
        // create a static local storage
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;

        // the word of which the detail shows
        public string Word { get; set; }
        private WordDetail _wordDetail;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(_localStorage != null)
            {
                // read detail information from a local data
                _wordDetail = _localStorage.Detail;
                Sentences.ItemsSource = _wordDetail.contents;
                // set text of en_word & ch_word 
                EnWord.Text = Word;
                ChWord.Text = _wordDetail.word_translation;
            }
            
        }


        public WordDetailPage()
        {
            InitializeComponent(); 
        }

        protected override bool OnBackButtonPressed()
        {
            // 禁止用户使用返回键
            return true;
        }

        private async void NextWord(object sender, EventArgs e)
        {
            if (_localStorage.Current != null)
            {
                // familiar degree increase
                _localStorage.Current.familiar_degree++;
                // if familiar degree is not MAX -- repeat
                if (_localStorage.Current.familiar_degree != GlobalClasses.MaxDegreeFamiliar)
                {
                    _localStorage.Repeat();
                }
                else
                {
                    // if MAX -- today words - 1
                    GlobalClasses.Index.WordsToStudy -= 1;
                }
                
                // to next word
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