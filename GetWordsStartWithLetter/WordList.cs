using System;
using System.Collections.Generic;

namespace GetWordsStartWithLetter
{
    public class WordList
    {
        public WordList()
        {
            Words = new List<string>();
        }

        public List<string> Words { get; set; }
    }
}
