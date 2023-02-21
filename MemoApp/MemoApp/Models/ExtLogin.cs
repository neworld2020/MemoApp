using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace MemoApp.Models
{
    public class LoginConfigs
    {
        public bool SavePassword;
        public bool AutoLogin;
        public string UpdateUserkey;
        public string RememberedUsername;
        public int RememberedPwdLength;
    };
    
    public class ExtLogin
    {
        // Remember Password & Auto Login
        private readonly LoginConfigs _loginConfigs;
        private readonly RemoteStorage _rs = GlobalClasses.RemoteStorage;
        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private readonly string _fileName = "v2_loginConfigs.txt";

        public bool SavePassword
        {
            get => _loginConfigs.SavePassword;
            set
            {
                _loginConfigs.SavePassword = value;
                WriteToFile();
            } 
        }

        public bool AutoLogin
        {
            get => _loginConfigs.AutoLogin;
            set
            {
                _loginConfigs.AutoLogin = value;
                WriteToFile();
            }
        }

        public string RememberedUsername
        {
            get => _loginConfigs.RememberedUsername;
            set => _loginConfigs.RememberedUsername = value;
                
        }

        public int RememberedPwdLength
        {
            get => _loginConfigs.RememberedPwdLength;
            set => _loginConfigs.RememberedPwdLength = value;
        }

        public string UpdateUserkey
        {
            get => _loginConfigs.UpdateUserkey;
            set
            {
                _loginConfigs.UpdateUserkey = value;
                WriteToFile();
            }
        }

        public ExtLogin()
        {   
            _loginConfigs = ReadFromFile();
        }

        public void Init()
        {
            if (_loginConfigs.AutoLogin)
            {
                Update();
            }
        }

        public bool Update()
        {
            // use UpdateUserkeyToken to get and update userkey
            UpdateUserkey = _rs.UpdateUserkeyAsync(UpdateUserkey).Result;
            if (_loginConfigs.UpdateUserkey == null)
            {
                return false;
            }

            return true;
        }

        private LoginConfigs ReadFromFile()
        {
            LoginConfigs defaultConfigs = new LoginConfigs
            {
                SavePassword = false,
                AutoLogin = false,
                UpdateUserkey = "",
                RememberedUsername = "",
                RememberedPwdLength = 0
            };
            string fullPath = Path.Combine(_filePath, _fileName);
            // detect if the file exists
            if (!File.Exists(fullPath)) return defaultConfigs;
            // read from file to configs
            // file language -- json
            string configsJson = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<LoginConfigs>(configsJson);
        }
        
        private void WriteToFile()
        {
            string fullPath = Path.Combine(_filePath, _fileName);
            string configsJson = JsonConvert.SerializeObject(_loginConfigs);
            File.WriteAllText(fullPath, configsJson);
        }
    }
}