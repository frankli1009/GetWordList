using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Dictionary.Models;
using Newtonsoft.Json;

namespace Dictionary.Utilities
{
    public class GetWords
    {
        private List<string> _words = new List<string>();

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

        public GetWords(WordDbContext context, string letters, int iWordLength, string extraRegEx)
        {
            _words = InnerGetWords(context, letters, iWordLength, extraRegEx);
        }

        private List<string> InnerGetWords(WordDbContext context, string letters, int iWordLength, string extraRegEx)
        {
            List<string> lettersForWord = GetLettersForWord(letters, iWordLength);
            List<string> words = FormWords(context, lettersForWord, extraRegEx);
            return words;
        }

        private List<string> FormWords(WordDbContext context, List<string> lettersForWord, string extraRegEx)
        {
            List<string> results = new List<string>();
            foreach (var letters in lettersForWord)
            {
                results.AddRange(GetWordsFromDBContext(context, letters));
            }
            if (extraRegEx != null && extraRegEx.Length > 0)
                return results.Distinct().HasVowel().MatchExtraRegex(extraRegEx).OrderBy(c => c).ToList();
            else
                return results.Distinct().HasVowel().OrderBy(c => c).ToList();
        }

        private List<string> GetWordsFromDBContext(WordDbContext context, string letters)
        {
            int len = letters.Length;
            Regex rgx = new Regex(String.Format("^[{0}]{{{1}}}$", letters, len));
            List<string> results = new List<string>();
            List<Tuple<char, int>> letterCount = new List<Tuple<char, int>>();
            foreach (var letter in letters)
            {
                int count = 1;
                if (letterCount.Any(l => l.Item1 == letter))
                {
                    var item = letterCount.Find(l => l.Item1 == letter);
                    count = item.Item2;
                    letterCount.Remove(item);
                    count++;
                }
                letterCount.Add(new Tuple<char, int>(letter, count));
            }
            foreach (var letter in letters)
            {
                var selWords = context.Words.Where(w => w.Length == len && w.StartLetter == letter && rgx.IsMatch(w.WordW));
                foreach (var item in letterCount)
                {
                    Regex regex = new Regex(CheckLetterCountRegularExpress(item));
                    selWords = selWords.Where(w => rgx.IsMatch(w.WordW));
                }
                results.AddRange(selWords.Select(s => s.WordW));
            }
            return results.Distinct().ToList();
        }

        private string CheckLetterCountRegularExpress(Tuple<char, int> item)
        {
            string notLetter = $"[^{item.Item1}]*";
            string onceLetter = $"{notLetter}{item.Item1}";
            string exp = $"^{onceLetter}";
            for (int i = 1; i < item.Item2; i++)
            {
                exp += onceLetter;
            }
            exp += $"{notLetter}$";
            return exp;
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
