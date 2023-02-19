using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MemoApp.Models;
using MemoApp.ViewModels;

namespace MemoApp.Views
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IndexPage : ContentPage
	{
		private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;
		private readonly IndexViewModel _indexViewModel;
		private int _wordsToStudy, _wordsToReview;
		public bool FirstStudy = true;

		public int WordsToStudy
		{
			get => _wordsToStudy;
			set => _wordsToStudy = value;
		}

		public int WordsToReview
		{
			get => _wordsToReview;
			set => _wordsToReview = value;
		}

		public IndexPage ()
		{
			InitializeComponent();
			BindingContext = _indexViewModel = new IndexViewModel();
			WordsToStudy = 0;
			WordsToReview = 0;
		}

        private async void StartLearning(object sender, EventArgs e)
        {
	        if (!FirstStudy)
	        {
		        // additional study words -- ask user
		        bool learningMore = 
			        await DisplayAlert("Tip", "Learning more words than schedule?", "Yes", "No");
		        if (!learningMore) return;
	        }
	        else
	        {
		        FirstStudy = false;
	        }
	        // TODO 实现复习功能
			// get first word
			if (!_localStorage.Init())
			{
				await DisplayAlert("Tip", "Please Login First", "OK");
				return;
			}
			Word firstWord = _localStorage.Current;
			if (firstWord != null)
			{
				WordConfirmPage firstPage = new WordConfirmPage
				{
					Word = firstWord.word
				};
				await Navigation.PushAsync(firstPage);
			}
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
			// 1. 更新当前日期
			DateTime today = DateTime.Now;
			string[] monthArray = new string[] {
				"Jan.", "Feb.", "Mar.", "Apr.", "May", "Jun.",
				 "Jul.", "Aug.", "Sept.", "Oct.", "Nov.", "Dec." };
			string monthStr = monthArray[today.Month - 1];
			CurrentDate.Text = monthStr + today.Day;
			// 2. Auto Login
			GlobalClasses.ExtLogin.Init();
			// 3. localStorage Initialize
			if (FirstStudy)
			{
				_localStorage.Init();
			}
			// 4. Update Nums
			_indexViewModel.WordsToStudy = GlobalClasses.Index.WordsToStudy;
			_indexViewModel.WordsToReview = GlobalClasses.Index.WordsToReview;
		}
    }
}