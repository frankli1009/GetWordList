using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GetWordsConsole
{
    public class GetWords
    {
        private List<string> _words;

        public List<string> Words
        {
            get
            {
                return _words;
            }
        }

        public GetWords()
        {
        }

        public GetWords(string letters, int iWordLength, string extraRegEx, string jsonRejected)
        {
            List<string> rejected = string.IsNullOrEmpty(jsonRejected) ? null : JsonConvert.DeserializeObject<List<string>>(jsonRejected);
            ExecGetWords(letters, iWordLength, extraRegEx, rejected);
        }

        public void ExecGetWords(string letters, int iWordLength, string extraRegEx, List<string> rejected)
        {
            _words = InnerGetWords(letters, iWordLength, extraRegEx, rejected);
        }

        private List<string> InnerGetWords(string letters, int iWordLength, string extraRegEx, List<string> rejected)
        {
            List<string> lettersForWord = GetLettersForWord(letters, iWordLength);
            List<string> words = FormWords(lettersForWord, extraRegEx, rejected);
            return words;
        }

        private List<string> FormWords(List<string> lettersForWord, string extraRegEx, List<string> rejected)
        {
            List<string> results = new List<string>();
            foreach (var letters in lettersForWord)
            {
                results.AddRange(FormWords(letters, rejected));
            }
            if (extraRegEx != null && extraRegEx.Length > 0)
                return results.Distinct().HasVowel().MatchExtraRegex(extraRegEx).OrderBy(c => c).ToList();
            else
                return results.Distinct().HasVowel().OrderBy(c => c).ToList();
        }

        private List<string> FormWords(string letters, List<string> rejected, string pre = "")
        {
            List<string> results = new List<string>();
            for (int i = 0; i < letters.Length; i++)
            {
                string letter = letters.Substring(i, 1);
                string lettersForwards = letters.Substring(0, i) + letters.Substring(i + 1);

                if (letters.Length > 1)
                    results.AddRange(FormWords(lettersForwards, rejected, pre + letter));
                else
                {
                    string word = pre + letter;
                    if (rejected == null) results.Add(word);
                    else if (rejected.IndexOf(word) == -1)
                    {
                        results.Add(word);
                    }
                }
            }
            return results.Distinct().ToList();
        }

        private List<string> GetLettersForWord(string letters, int iWordLength, string pre = "")
        {
            List<string> results = new List<string>();
            for (int i = 0; i <= letters.Length - iWordLength; i++)
            {
                string lettersTemp = letters.Substring(i, 1);

                if (iWordLength > 1)
                    results.AddRange(GetLettersForWord(letters.Substring(i + 1), iWordLength - 1, pre + lettersTemp));
                else
                    results.Add(String.Concat((pre + lettersTemp).OrderBy(c => c)));
            }

            return results.Distinct().ToList();
        }
    }
}
