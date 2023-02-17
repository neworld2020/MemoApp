using System;
using System.Collections.Generic;
using System.Text;

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
        private List<Word> data;
        // 分组索引
        private List<int> group_sep_indexes = new List<int>();

        public Vocabulary(List<Word> words)
        {
            this.data = words;
            // 自动分组 -- 按照设定的MAX_NUM_OF_GROUP

        }
        
        public int Count => data.Count; 

        // 索引器
        public Word this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        // 查找器
        public int find(string word)
        {
            int index = data.IndexOf(data.Find(p=>p.word==word));
            return index;
        }
    }
    // 同时获取Vocabulary和WordDetails
    public class VocabularyAndDetails
    {
        public List<Word> vocabulary;
        public List<WordDetail> word_details;
    }
}
