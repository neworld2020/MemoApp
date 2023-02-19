using MemoApp.Models;
using MemoApp.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoApp
{
    public partial class App : Application
    {
        public static string FolderPath { get; private set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
          
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

    public static class GlobalClasses
    {
        public static readonly UserManagement UserManagement = new UserManagement("https://cloud-smx2003.fun");
        public static readonly RemoteStorage RemoteStorage = new RemoteStorage("https://cloud-smx2003.fun");
        public static readonly ExtLogin ExtLogin = new ExtLogin();
        public static readonly LocalStorage LocalStorage = new LocalStorage();

        public static IndexPage Index = new IndexPage();

        // 最大熟悉度
        public const int MaxDegreeFamiliar = 4;
        // 每组最大单词个数
        public const int MaxNumOfGroup = 7;
        // 每次学习单词个数 -- 可调
        public const int LearningWordsCount = 20;
    }
}