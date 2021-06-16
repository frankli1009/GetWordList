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

        public OperationResult Append(OperationResult or)
        {
            if (or != null)
            {
                this.NotFounds.AddRange(or.NotFounds);
                this.Conflicts.AddRange(or.Conflicts);
                this.Oks.AddRange(or.Oks);
            }

            return this;
        }
    }
}
