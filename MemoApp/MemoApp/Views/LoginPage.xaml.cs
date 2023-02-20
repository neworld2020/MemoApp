﻿using System;
using MemoApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly ExtLogin _extLogin = GlobalClasses.ExtLogin;
        private readonly LocalStorage _localStorage = GlobalClasses.LocalStorage;
        private readonly RemoteStorage _remoteStorage = GlobalClasses.RemoteStorage;
        private readonly UserManagement _userMgmt = GlobalClasses.UserManagement;

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Fill Entry with Login Configs
            Username.Text = _extLogin.RememberedUsername;
            Password.Text = new string('*', _extLogin.RememberedPwdLength);
            SavePwd.IsChecked = _extLogin.SavePassword;
            AutoLogin.IsChecked = _extLogin.AutoLogin;
        }

        private async void LoginBtnReleased(object sender, EventArgs e)
        {
            if (!_extLogin.SavePassword || _extLogin.RememberedUsername != Username.Text)
            {
                // clear LocalStorage
                _localStorage.Clear();
                // submit form and login -- SavePassword = false
                var usernameText = Username.Text;
                var passwordText = Password.Text;
                if (usernameText == "" || passwordText == "") return;

                var salt = await _userMgmt.Salt(usernameText);
                var encryptedPwd = _userMgmt.Encrypt(passwordText, salt, "BCRYPT");
                var userkeyTask = await _userMgmt.Login(usernameText, encryptedPwd);
                // here we can add some animation
                if (userkeyTask != null)
                {
                    // 登录成功
                    _extLogin.SavePassword = SavePwd.IsChecked;
                    _extLogin.AutoLogin = AutoLogin.IsChecked;
                    if (SavePwd.IsChecked)
                    {
                        _extLogin.RememberedUsername = Username.Text;
                        _extLogin.RememberedPwdLength = Password.Text.Length;
                        _extLogin.UpdateUserkey = userkeyTask;
                    }

                    await DisplayAlert("Congratulations", "Login Succeed", "OK");
                }
                else
                {
                    // 登录失败 -- 询问是否注册
                    _remoteStorage.UserKey = null;
                    var register =
                        await DisplayAlert("Login Failed", "Do you want to register?", "Yes", "No");
                    if (register)
                    {
                        var registerSucceed = await _userMgmt.Register(usernameText, encryptedPwd, salt);
                        if (registerSucceed)
                            await DisplayAlert("Register Succeed!", "Please Login Again", "OK");
                        else
                            await DisplayAlert("Register Failed!", "The username has been registered", "OK");
                    }
                }
            }
            else
            {
                if (_extLogin.Update())
                {
                    _extLogin.SavePassword = SavePwd.IsChecked;
                    _extLogin.AutoLogin = AutoLogin.IsChecked;
                    await DisplayAlert("Congratulations", "Login Succeed", "OK");
                }
                else
                {
                    // login expired
                    _extLogin.AutoLogin = false;
                    _extLogin.SavePassword = false;
                    Password.Text = "";
                    await DisplayAlert("Login Expired", "Please login again", "OK");
                }
            }
        }

        private void OnSavePwdCheckChange(object sender, EventArgs e)
        {
            // "保存密码候选框"
            var isSavePwdChecked = SavePwd.IsChecked;
            if (isSavePwdChecked)
            {
                // 允许选中“自动登录”
                AutoLogin.Color = Color.FromHex("#67bafc");
                AutoLogin.IsEnabled = true;
            }
            else
            {
                AutoLogin.IsChecked = false;
                AutoLogin.Color = Color.Gray;
                AutoLogin.IsEnabled = false;
            }
        }

        private void AvatarChange(object sender, EventArgs e)
        {
            string username = Username.Text;
            UriBuilder avatarUriBuilder = new UriBuilder("https://ui-avatars.com/api/");
            avatarUriBuilder.Query = $"name={username}&background=2992e5&color=fff&uppercase=false";
            Avatar.Source = ImageSource.FromUri(avatarUriBuilder.Uri);
            base.OnAppearing();
        }

        private void Logout(object sender, EventArgs e)
        {
        }
    }
}