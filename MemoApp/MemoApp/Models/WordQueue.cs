using System;
using System.Collections;
using System.Collections.Generic;

namespace MemoApp.Models
{
    // for debug
    /*
    public static class GlobalClasses
    {
        public static readonly RemoteStorage RemoteStorage = new RemoteStorage("https://www.cloud-smx2003.fun");
        public static readonly LocalStorage LocalStorage = new LocalStorage(RemoteStorage);

        // 最大熟悉度
        public const int MaxDegreeFamiliar = 4;
        // 每组最大单词个数
        public const int MaxNumOfGroup = 7;
    }
    */
    
    public class WordQueue: IEnumerable<Word>
    {
        /*
         * This Class should be initialized with LocalStorage Instance.
         * This Class can help you with
         * 1. Provide the next word you should learn or review
         * 2. Flexible config
         */
        
        private readonly List<List<Word>> _wordList;
        private bool _initialized;
        
        // _iterator always points to the first word that hasn't shown yet

        public WordQueue()
        {
            _wordList = new List<List<Word>>();
            _initialized = false;
        }

        public void Init(Vocabulary vocabulary)
        {
            _wordList.Clear();
            int span = GlobalClasses.MaxNumOfGroup;
            for (int index = 0; index < vocabulary.Count; index++)
            {
                if (index % span == 0)
                {
                    _wordList.Add(new List<Word>());
                }
                _wordList[index / span].Add(vocabulary[index]);
            }

            _initialized = true;
        }

        public string Print()
        {
            // for debug
            string printStr = "";
            foreach (var word in this)
            {
                Console.Write(word.word + ",");
                printStr += (word.word + ",");
            }
            Console.Write("\n");

            return printStr;
        }

        public IEnumerator<Word> GetEnumerator()
        {
            if (!_initialized)
            {
                throw new NullReferenceException("Please Initialize WordQueue using Init() first");
            }
            return new WordQueueIterator(_wordList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                throw new NullReferenceException("Please Initialize WordQueue using Init() first");
            }
            return new WordQueueIterator(_wordList);
        }
    }
    
    public class WordQueueIterator : IEnumerator<Word>
    {
        private readonly List<List<Word>> _data;
        private int _groupId;
        private int _seqId;
        
        public Word Current
        {
            get
            {
                try
                {
                    return _data[_groupId][_seqId];
                }
                catch(ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }
        object IEnumerator.Current => Current;

        public WordQueueIterator(List<List<Word>> inputData)
        {
            _data = inputData;
            _groupId = 0;
            _seqId = -1;
        }

        public void RepeatCurrentWord()
        {
            if (Current != null)
            {
                Word newWord = new Word(Current.word, Current.familiar_degree);
                // repeat the word at the tail of next group
                if (_groupId == _data.Count - 1)
                {
                    _data[_groupId].Add(newWord);
                }
                else
                {
                    _data[_groupId + 1].Add(newWord);
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public bool MoveNext()
        {
            return EnumeratorNext();
        }

        private bool EnumeratorNext()
        {
            // simplest movement of enumerator -- move to next word without complex word operation
            if (_seqId == _data[_groupId].Count - 1)
            {
                if (_groupId == _data.Count - 1) return false;
                _groupId++;
                _seqId = -1;
            }

            _seqId++;
            return true;
        }

        public void Reset()
        {
            _groupId = 0;
            _seqId = 0;
        }

        public void Dispose() { }
    }
}