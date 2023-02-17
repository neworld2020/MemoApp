using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MemoApp.Models
{
    public class RemoteStorage
    {
        // This class contains all operations for reading from remote files.
        private readonly HttpClient _client;
        public string UserKey { get; set; }

        public RemoteStorage(string host, string userKey = null)
        {
            _client = new HttpClient();
            // set base URI
            _client.BaseAddress = new Uri(host);
            this.UserKey = userKey;
        }

        public bool Connect()
        {
            // This method is used to test if the remote server is available.
            // If the server is available, it will return true.
            // If the server is not available, it will return false
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/");
            Task<HttpResponseMessage> response = _client.SendAsync(httpRequestMessage);
            return response.Result.StatusCode == System.Net.HttpStatusCode.OK;
        }


        public async Task<String> GetVocabulary()
        {
            if (string.IsNullOrEmpty(UserKey))
            {
                return null;
            }
            // This method is used to get the vocabulary from the remote server.
            // If the server is available, it will return the vocabulary.
            // If the server is not available, it will return null.
            string getVocabularyPath = "/corpus/vocabulary?userkey=" + UserKey;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getVocabularyPath);
            Task<HttpResponseMessage> response = _client.SendAsync(httpRequestMessage);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseStr = await response.Result.Content.ReadAsStringAsync();
                return responseStr;
            }
            else
            {
                return null;
            }
        }

        public async Task<String> GetWordDetails()
        {
            if (string.IsNullOrEmpty(UserKey))
            {
                return null;
            }
            // This method is used to get the corpus from the remote server.
            string getWordDetailsPath = "/corpus/word-details?userkey=" + UserKey;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getWordDetailsPath);
            Task<HttpResponseMessage> response = _client.SendAsync(httpRequestMessage);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Result.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<String> GetVocabularyAndDetails(int count = 20)
        {
            if (string.IsNullOrEmpty(UserKey))
            {
                return null;
            }
            // This method is used to get the corpus from the remote server.
            string getWordDetailsPath = "/v2/corpus/vocabulary-and-details?userkey=" + UserKey + "&count=" + count;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getWordDetailsPath);
            Task<HttpResponseMessage> response = _client.SendAsync(httpRequestMessage);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Result.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }
        
        public string UpdateUserkey(string userkeyInFile)
        {
            string updatePath = "/v2/user/update-userkey?userkey=" + userkeyInFile;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, updatePath);
            Task<HttpResponseMessage> response = _client.SendAsync(httpRequestMessage);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string userKeyJson = response.Result.Content.ReadAsStringAsync().Result;
                UserkeyResponse userkeyResponse = JsonConvert.DeserializeObject<UserkeyResponse>(userKeyJson);
                UserKey = userkeyResponse.userkey;
                GlobalClasses.RemoteStorage.UserKey = userkeyResponse.userkey;
                return UserKey;
            }
            else
            {
                return null;
            }
        }
    }
}
