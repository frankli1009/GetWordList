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
            UnknownErrors = new List<Word>();
        }

        public List<string> NotFounds { get; set; }
        public List<Word> Conflicts { get; set; }
        public List<Word> Oks { get; set; }
        public List<Word> UnknownErrors { get; set; }

        public OperationResult Append(OperationResult or)
        {
            if (or != null)
            {
                this.NotFounds.AddRange(or.NotFounds);
                this.Conflicts.AddRange(or.Conflicts);
                this.Oks.AddRange(or.Oks);
                this.UnknownErrors.AddRange(or.UnknownErrors);
            }

            return this;
        }

        public OperationResult Clear()
        {
            this.NotFounds.Clear();
            this.Conflicts.Clear();
            this.Oks.Clear();
            this.UnknownErrors.Clear();

            return this;
        }
    }
}
