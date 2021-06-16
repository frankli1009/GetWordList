using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
    public class OperationResult
    {
        public OperationResult()
        {
            NotFounds = new List<string>();
            Conflicts = new List<Word>();
            Oks = new List<Word>();
        }

        public List<string> NotFounds { get; set; }
        public List<Word> Conflicts { get; set; }
        public List<Word> Oks { get; set; }
    }
}
