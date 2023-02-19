using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MemoApp.Models
{
    public class UserManagement
    {
        private readonly HttpClient _client;

        public UserManagement(string host)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(host);
        }

        public async Task<string> Salt(string username)
        {
            // get salt from server
            string saltPath = "/user/salt?username=" + username;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, saltPath);
            var response = await _client.SendAsync(httpRequestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string saltJson = await response.Content.ReadAsStringAsync();
                SaltResponse saltResponse = JsonConvert.DeserializeObject<SaltResponse>(saltJson);
                return saltResponse.salt;
            }

            return null;
        }

        public string Encrypt(string password, string salt, string method="SHA256")
        {
            if (method == "SHA256")
            {
                // method: SHA256
                string data = password + "_" + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                byte[] hash = SHA256Managed.Create().ComputeHash(bytes);
 
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }else if (method == "BCRYPT")
            {
                // method: BCrypt -- slow hash encrypt
                return BCrypt.Net.BCrypt.HashPassword(password, salt);
            }
            else
            {
                throw new NotSupportedException("暂时不支持该种加密方式");
            }
        }

        public async Task<string> Login(string username, string password)
        {
            string loginPath = "/v2/user/login";
            UserInfo userInfo = new UserInfo();
            userInfo.username = username;
            userInfo.password = password;
            userInfo.salt = "";
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, loginPath);
            string userInfoStr = JsonConvert.SerializeObject(userInfo);
            httpRequestMessage.Content = new StringContent(userInfoStr, Encoding.UTF8, "application/json");
            // get userkey from response
            var response = await _client.SendAsync(httpRequestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string userKeyJson = await response.Content.ReadAsStringAsync();
                UserkeyResponse userkeyResponse = JsonConvert.DeserializeObject<UserkeyResponse>(userKeyJson);
                GlobalClasses.RemoteStorage.UserKey = userkeyResponse.userkey;
                return userkeyResponse.userkey;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Register(string username, string password, string salt)
        {
            string registerPath = "/user/register";
            UserInfo userInfo = new UserInfo();
            userInfo.username = username;
            userInfo.password = password;
            userInfo.salt = salt;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, registerPath);
            string userInfoStr = JsonConvert.SerializeObject(userInfo);
            httpRequestMessage.Content = new StringContent(userInfoStr, Encoding.UTF8, "application/json");
            // get userkey from response
            var response = await _client.SendAsync(httpRequestMessage);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
