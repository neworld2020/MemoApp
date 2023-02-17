using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MemoApp.Models
{
    public class LocalStorage
    {
        /*
            This class contains all operations for writing and reading to 
            local files.
            1. get full words list
            2. get example sentences of words
            3. write words and dictionaries into the file
            The local files are encoded as json, and can be read as wanted.
         */


        private WordDetails WordDetails { get; set; }
        private Vocabulary Vocabulary { get; set; }
        private readonly RemoteStorage _rs = GlobalClasses.RemoteStorage;
        private readonly WordQueue _wordQueue = new WordQueue();
        private WordQueueIterator _wordQueueIt = null;

        private const string LocalQueueFileName = "WordQueue.txt";
        
        // Exposing APIs
        public bool Init()
        {
            if (_wordQueueIt == null || _wordQueueIt.Current == null)
            {
                // get from local file
                string fileAdr =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        LocalQueueFileName);
                if (File.Exists(fileAdr))
                {
                    string wordQueueStr = File.ReadAllText(fileAdr);
                    if (!string.IsNullOrEmpty(wordQueueStr))
                    {
                        if (InitWithLocalString(wordQueueStr)) return true;
                    }
                }
                // get from server
                Task<string> vocabularyAndDetailsJson = _rs.GetVocabularyAndDetails();
                if (vocabularyAndDetailsJson.Result == null)
                {
                    return false;
                }

                VocabularyAndDetails vocabularyAndDetails = 
                    JsonConvert.DeserializeObject<VocabularyAndDetails>(vocabularyAndDetailsJson.Result);
                WordDetails = new WordDetails(vocabularyAndDetails.word_details);
                Vocabulary = new Vocabulary(vocabularyAndDetails.vocabulary);
                _wordQueue.Init(Vocabulary);
                _wordQueueIt = (WordQueueIterator)_wordQueue.GetEnumerator();
                _wordQueueIt.MoveNext();
                GlobalClasses.Index.WordsToStudy = Vocabulary.Count;
                
                // write to file
                File.WriteAllText(fileAdr, JsonConvert.SerializeObject(vocabularyAndDetails));
            }
            return true;
        }

        private bool InitWithLocalString(string wordQueueStr)
        {
            try
            {
                // string -> Vocabulary
                VocabularyAndDetails vocabularyAndDetails =
                    JsonConvert.DeserializeObject<VocabularyAndDetails>(wordQueueStr);
                WordDetails = new WordDetails(vocabularyAndDetails.word_details);
                Vocabulary = new Vocabulary(vocabularyAndDetails.vocabulary);
                _wordQueue.Init(Vocabulary);
                _wordQueueIt = (WordQueueIterator)_wordQueue.GetEnumerator();
                _wordQueueIt.MoveNext();
                GlobalClasses.Index.WordsToStudy = Vocabulary.Count;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public Word Current
        {
            get => _wordQueueIt.Current;
        }
        
        public WordDetail Detail
        {
            get => GetDetail(Current.word);
        }

        public bool MoveNext()
        {
            bool hasNext = _wordQueueIt.MoveNext();
            if (!hasNext)
            {
                // last one -- clear local storage
                string fileAdr =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        LocalQueueFileName);
                File.Delete(fileAdr);
                _wordQueueIt = null;
            }

            return hasNext;
        }

        public void Repeat()
        {
            _wordQueueIt.RepeatCurrentWord();
        }

        public void Clear()
        {
            _wordQueueIt = null;
        }
        
        // private methods
        private WordDetail GetDetail(string word)
        {
            // get detail information of a word
            for (int i = 0; i < WordDetails.Len(); i++)
            {
                WordDetail wordDetail = WordDetails[i];
                if (wordDetail.word == word)
                {
                    return wordDetail;
                }
            }
            return null;
        }
    }
}
