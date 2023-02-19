using System;
using MemoApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogPage : ContentPage
    {
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;
        private WordDetail _wordDetail;

        public DialogPage()
        {
            InitializeComponent();
        }

        private async void SwipeDownQuit(object sender, SwipedEventArgs e)
        {
            var quit =
                await DisplayAlert("Quit?", "Your progress will be saved", "Quit", "Continue");
            if (quit) await Navigation.PopToRootAsync();
        }

        private async void BackToDetail(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_localStorage != null)
            {
                // read detail information from a local data
                _wordDetail = _localStorage.Detail;
                Sentences.ItemsSource = _wordDetail.contents;
            }
            Device.StartTimer(TimeSpan.FromSeconds(1), (Func<bool>)(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if(Sentences is null) return;
                    var children = Sentences.LogicalChildren;
                    double allChildrenHeight = 0;
                    foreach (var child in children)
                    {
                        double height = (double)child.GetValue(HeightProperty);
                        allChildrenHeight += height;
                    }

                    if (allChildrenHeight > Sentences.Height)
                    {
                        MiddleStackLayout.InputTransparent = false;
                    }
                    Console.WriteLine($"Height={Sentences.Height}, ChildrenHeight={allChildrenHeight}");
                });             
                   
                return false;
            }));
        }
    }
}