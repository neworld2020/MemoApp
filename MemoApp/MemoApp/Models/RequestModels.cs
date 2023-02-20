using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace MemoApp.Models
{
    // 用户信息
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
    }

    // 返回一个userkey和token
    public class UserkeyResponse
    {
        public string userkey { get; set; }
    }
    
    // 返回一个salt
    public class SaltResponse
    {
        public string salt { get; set; }
    }

    // 一个句子
    public class Content
    {
        public string speaker { get; set; }
        // speaker color: use the speaker's main theme color
        // eg. Venti: #359697
        public string speakerColor { get; set; } = "Black";
        public string content { get; set; }
        public string translation { get; set; }
    }

    // 一个单词的细节信息
    public class WordDetail
    {
        // Detail Information Storage Class
        public string word { get; set; }
        public string word_translation { get; set; }
        public List<Content> contents { get; set; }
    }

    // 多个单词的细节信息
    public class WordDetails
    {
        private List<WordDetail> data;

        public WordDetails(List<WordDetail> wordDetails) 
        {
            this.data = wordDetails;
        }

        public int Len() { return data.Count; }

        public WordDetail this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        public WordDetails Preprocess()
        {
            foreach (var wordDetail in data)
            {
                foreach (var content in wordDetail.contents)
                {
                    content.content = PreProcessStr(content.content, "en");
                    content.translation = PreProcessStr(content.translation, "ch");
                }

                wordDetail.word_translation = PreProcTranslation(wordDetail.word_translation);
            }
            return this;
        }
        
        private static string PreProcessStr(string rawString, string language="en")
        {
            // {NICKNAME} -> username
            string processString = rawString
                .Replace("{NICKNAME}", GlobalClasses.ExtLogin.RememberedUsername)
                .Replace("\\n", "&#10;");
            return processString;
        }

        private static string PreProcTranslation(string translation)
        {
            // 先分组、分类
            // n. T1;T2; ...;Tn v. V1;V2;...;Vn adj.
            const int trimLength = 2;
            var group = translation.Split('\t');
            var processedStr = new StringBuilder();
            foreach (var splitStr in group)
            {
                string wordType = splitStr.Split(' ')[0];
                IEnumerable<string> explains = splitStr
                    .Split(' ')[1]
                    .Split('；')
                    .Take(trimLength);
                processedStr = processedStr.Append(wordType);
                foreach (var explain in explains)
                {
                    processedStr = processedStr.Append(explain + "；");
                }

                processedStr = processedStr.Append("\n");
            }

            return processedStr.ToString();
        }
    }

    // 一个单词与其熟悉度
    public class Word
    {
        public Word(string word, int familiar_degree)
        {
            this.word = word;
            this.familiar_degree = familiar_degree;
        }

        public string word { get; set; }
        public int familiar_degree { get; set; }
    }

    // 多个单词及其对应的熟悉度
    public class Vocabulary
    {
        private List<Word> data = new List<Word>();

        public Vocabulary(List<Word> words)
        {
            data.AddRange(words);
        }
        
        public int Count => data.Count; 

        // 索引器
        public Word this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }
    }
    // 同时获取Vocabulary和WordDetails
    public class VocabularyAndDetails
    {
        public List<Word> vocabulary;
        public List<WordDetail> word_details;
    }
}
