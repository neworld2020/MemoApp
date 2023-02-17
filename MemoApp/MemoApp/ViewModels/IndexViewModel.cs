using System;
using System.ComponentModel;
using Xamarin.Forms;
using MemoApp.Views;


namespace MemoApp.ViewModels
{
    // MVVM Structure To Update NumsToStudy and NumsToReview Automatically
    class IndexViewModel : INotifyPropertyChanged
    {
        private int _wordsToStudy;
        private int _wordsToReview;
        public event PropertyChangedEventHandler PropertyChanged;

        public IndexViewModel()
        {
            _wordsToStudy = GlobalClasses.LearningWordsCount;
            _wordsToReview = 0;
        }

        public int WordsToStudy
        {
            get => _wordsToStudy;
            set
            {
                if (_wordsToStudy != value)
                {
                    _wordsToStudy = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("WordsToStudy"));
                    }
                }
            }
        }

        public int WordsToReview
        {
            get => _wordsToReview;
            set
            {
                if (_wordsToStudy != value)
                {
                    _wordsToReview = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("WordsToReview"));
                    }
                }
            }
        }
    }
}