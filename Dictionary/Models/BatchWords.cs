using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
    public class BatchWords
    {
        public BatchWords()
        {
            Words = new List<string>();
        }

        public List<string> Words { get; set; }
    }
}
