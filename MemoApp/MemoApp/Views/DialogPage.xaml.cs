using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private int FindExample()
        {
            IEnumerable<string> sentences =
                from content in _wordDetail.contents
                select content.content;
            int index = 0;
            foreach (var sentence in sentences)
            {
                if (sentence.Contains($" {_localStorage.Current.word} "))
                {
                    return index;
                }

                index++;
            }
            // Impossible
            return -1;
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
            // After OnAppearing
            Device.StartTimer(TimeSpan.FromMilliseconds(100), (Func<bool>)(() =>
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
                    
                    // change color
                    Sentences
                        .LogicalChildren[FindExample()]
                        .FindByName<Label>("dialogue")
                        .TextColor = Color.FromHex("#2992e5");
                });             
                   
                return false;
            }));
        }
    }
}