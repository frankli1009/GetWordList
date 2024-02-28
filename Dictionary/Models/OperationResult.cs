using System;
using System.Collections.Generic;

namespace Dictionary.Models
{
    public class OperationResult<T>
    {
        public OperationResult()
        {
            NotFounds = new List<string>();
            Conflicts = new List<T>();
            Oks = new List<T>();
            UnknownErrors = new List<T>();
        }

        public List<string> NotFounds { get; set; }
        public List<T> Conflicts { get; set; }
        public List<T> Oks { get; set; }
        public List<T> UnknownErrors { get; set; }

        public OperationResult<T> Append(OperationResult<T> or)
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

        public OperationResult<T> Clear()
        {
            this.NotFounds.Clear();
            this.Conflicts.Clear();
            this.Oks.Clear();
            this.UnknownErrors.Clear();

            return this;
        }
    }
}
