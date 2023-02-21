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
        private string _avatarSource;
        public event PropertyChangedEventHandler PropertyChanged;

        public IndexViewModel()
        {
            _wordsToStudy = GlobalClasses.LearningWordsCount;
            _wordsToReview = 0;
            _avatarSource = "https://cloud-smx2003.fun/file/0220160940_user.png";
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
                if (_wordsToReview != value)
                {
                    _wordsToReview = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("WordsToReview"));
                    }
                }
            }
        }

        public string AvatarSource
        {
            get => _avatarSource;
            set
            {
                if (_avatarSource != value)
                {
                    _avatarSource = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("AvatarSource"));
                    }
                }
            }
        }
    }
}