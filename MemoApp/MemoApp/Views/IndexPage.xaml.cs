using System;
using MemoApp.Models;
using MemoApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndexPage : ContentPage
    {
        private readonly IndexViewModel _indexViewModel;
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;
        public bool FirstStudy = true;

        public IndexPage()
        {
            InitializeComponent();
            BindingContext = _indexViewModel = new IndexViewModel();
            WordsToStudy = 0;
            WordsToReview = 0;
        }

        public int WordsToStudy { get; set; }

        public int WordsToReview { get; set; }

        private async void StartLearning(object sender, EventArgs e)
        {
            if (!FirstStudy)
            {
                // additional study words -- ask user
                var learningMore =
                    await DisplayAlert("Tip", "Learning more words than schedule?", "Yes", "No");
                if (!learningMore) return;
            }

            // TODO 实现复习功能
            // get first word
            if (!_localStorage.Init())
            {
                await DisplayAlert("Tip", "Please Login First", "OK");
                return;
            }

            var firstWord = _localStorage.Current;
            if (firstWord != null)
            {
                var firstPage = new WordConfirmPage
                {
                    Word = firstWord.word
                };
                await Navigation.PushAsync(firstPage);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // 其他加载项异步执行
            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                // 1. 更新当前日期
                var today = DateTime.Now;
                string[] monthArray =
                {
                    "Jan.", "Feb.", "Mar.", "Apr.", "May", "Jun.",
                    "Jul.", "Aug.", "Sept.", "Oct.", "Nov.", "Dec."
                };
                var monthStr = monthArray[today.Month - 1];
                CurrentDate.Text = monthStr + today.Day;
                // 2. Auto Login
                GlobalClasses.ExtLogin.Init();
                // 3. localStorage Initialize
                if (FirstStudy) _localStorage.Init();
                // 4. Update Nums
                _indexViewModel.WordsToStudy = GlobalClasses.Index.WordsToStudy;
                _indexViewModel.WordsToReview = GlobalClasses.Index.WordsToReview;
                // 5. Update Avatar
                string username = GlobalClasses.ExtLogin.RememberedUsername;
                bool HasLogin = !string.IsNullOrEmpty(GlobalClasses.RemoteStorage.UserKey);
                if (!HasLogin)
                {
                    // default avatar
                    _indexViewModel.AvatarSource = "https://cloud-smx2003.fun/file/0220160940_user.png";
                }
                else
                {
                    UriBuilder avatarUriBuilder = new UriBuilder("https://ui-avatars.com/api/")
                    {
                        Query = $"name={username}&background=2992e5&color=fff&uppercase=false"
                    };
                    _indexViewModel.AvatarSource = avatarUriBuilder.ToString();
                }
                return false;
            });
        }
    }
}