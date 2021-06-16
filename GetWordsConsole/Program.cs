using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GetWordsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get all words from the letters given");
            Console.WriteLine();
            Console.WriteLine("Letters (each letter will be used only once in a word, multiple same letters must be provided for multiple uses in a word):");
            int iLen = 0;
            string letters = null; 
            do
            {
                letters = Console.ReadLine();
                iLen = letters.Length;
                if (iLen <= 3)
                {
                    Console.WriteLine("Letters given are too less, please give more than 3 letters.");
                }
            } while (iLen <= 3);
            Console.WriteLine("Word length (>0 and <={0}):", iLen);
            int iWordLength = 0;
            do
            {
                if (!(Int32.TryParse(Console.ReadLine(), out iWordLength) && iWordLength > 0 && iWordLength <= iLen))
                {
                    Console.WriteLine("Not a valid number");
                    iWordLength = 0;
                }
            } while (iWordLength == 0);
            Console.WriteLine("Extra REGEX expression to match:");
            string extraRegEx = Console.ReadLine();
            Console.WriteLine("Rejected words (press enter to start a new one or finish the input):");
            List<string> rejected = new List<string>();
            string rejectedWord;
            while (!string.IsNullOrEmpty(rejectedWord  = Console.ReadLine()))
            {
                rejected.Add(rejectedWord);
            }
            rejectedWord = rejected.Count > 0 ? JsonConvert.SerializeObject(rejected) : null;

            Console.WriteLine();
            Console.WriteLine("Preparing words...");
            
            List<string> words = new List<string>();
            words = new GetWords(letters.ToLower(), iWordLength, extraRegEx, rejectedWord).Words;
            Console.WriteLine("Words are listed below:");
            foreach (var (word, index) in words.WithIndex())
            {
                Console.WriteLine("{0}: {1}", index+1, word);
            }
            Console.WriteLine();
            Console.WriteLine("There are total {0} possible words found!", words.Count);
            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();
        }

    }
}
