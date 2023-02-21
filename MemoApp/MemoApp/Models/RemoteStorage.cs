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

        public async Task<String> GetVocabularyAndDetailsAsync(bool review = false, int count = 20)
        {
            if (string.IsNullOrEmpty(UserKey))
            {
                return null;
            }
            // This method is used to get the corpus from the remote server.
            string reviewUri = $"/v2/corpus/review_vocabulary_and_words?userkey={UserKey}";
            string studyUri = $"/v2/corpus/vocabulary-and-details?userkey={UserKey}&count={count}";
            string getWordDetailsPath = review ? reviewUri : studyUri;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, getWordDetailsPath);
            var response = _client.SendAsync(httpRequestMessage).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
        }
        
        public async Task<string> UpdateUserkeyAsync(string userkeyInFile)
        {
            string updatePath = "/v2/user/update-userkey?userkey=" + userkeyInFile;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, updatePath);
            var response = _client.SendAsync(httpRequestMessage).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string userKeyJson = await response.Content.ReadAsStringAsync();
                UserkeyResponse userkeyResponse = JsonConvert.DeserializeObject<UserkeyResponse>(userKeyJson);
                UserKey = userkeyResponse.userkey;
                GlobalClasses.RemoteStorage.UserKey = userkeyResponse.userkey;
                return UserKey;
            }

            return null;
        }
    }
}
