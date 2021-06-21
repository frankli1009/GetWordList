using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GetWordsStartWithLetter
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                string fileName = String.Format("log_{0}.log", DateTime.Now.ToString("yyMMddHHmmss"));
                Console.WriteLine("Start to get words...");
                Console.WriteLine("To view logs/console output please refer to file: "+fileName);
                Console.SetOut(new StreamWriter(fileName, append: File.Exists(fileName)));

                GetAllWordsByMultipleBS(driver);
                Console.WriteLine("All done!");

                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                Console.WriteLine("All done!");
            }
            finally
            {
                driver.Quit();
            }

        }

        private static void GetAllWordsByMultipleBS(IWebDriver driver)
        {
            string urlFormat = "https://word.tips/words-start-with/{0}/";
            char fromA = 'a';

            //List<Tuple<int, List<int>>> toDoList = new List<Tuple<int, List<int>>>
            //{
            //    new Tuple<int, List<int>>(15, new List<int> { 3 }),
            //    new Tuple<int, List<int>>(16, new List<int> { 12 }),
            //    new Tuple<int, List<int>>(24, new List<int> { 10, 9 })
            //};

            for (int ii = 0; ii < 26; ii++) 
            //for (int ii = 14; ii <= 14; ii++) // 'o'
            //for(int jj=0; jj<toDoList.Count; jj++)
            {
                //int ii = toDoList[jj].Item1;

                char startLetter = Convert.ToChar(fromA + ii);
                string url = string.Format(urlFormat, startLetter);
                Console.WriteLine(url);
                Console.WriteLine();

                driver.Url = url;

                int iMaxLength = GetMaxLength(driver);
                //int iMaxLength = 15;

                for (int len = iMaxLength; len >= 2; len--)
                //for (int len = 10; len >= 10; len--)
                //for(int j=0; j<toDoList[jj].Item2.Count; j++)
                {
                    //int len = toDoList[jj].Item2[j];
                    Console.WriteLine($"Start of length {len}");

                    var autoEvent = new AutoResetEvent(false);
                    var statusChecker = new StatusChecker(1);
                    var stateTimer = new Timer(statusChecker.CheckStatus,
                                               autoEvent, 2000, 1000);
                    try
                    {
                        // Accept cookies
                        AccetpCookies(driver, autoEvent);
                    }
                    finally
                    {
                        stateTimer.Dispose();
                    }

                    // Locate body of words of length = len
                    var bodyElement = LocateBodyOfWordsOfLength(driver, len);
                    if (bodyElement == null) continue;

                    StatusChecker.ResetTotalCount();
                    stateTimer = new Timer(statusChecker.CheckStatus,
                                               autoEvent, 2000, 1000);
                    try
                    {
                        // Expand all words
                        while (true)
                        {
                            try
                            {
                                var element = bodyElement.FindElement(By.XPath(".//button"));
                                if (element != null)
                                {
                                    Console.WriteLine($"Find button to click: {element.Text} ['{startLetter}', Length: {len}]");
                                    element.Click();
                                    autoEvent.WaitOne();
                                }
                                else
                                {
                                    Console.WriteLine($"Finished to expand words of len {len}");
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Finished to expand words of len {len}");
                                Console.WriteLine(e.Message);
                                Console.WriteLine();
                                break;
                            }
                        }
                    }
                    finally
                    {
                        stateTimer.Dispose();
                    }

                    // Get all words and make a request to batch add to database
                    var el = bodyElement.FindElements(By.XPath(".//div[@class='letter-card__list']/span[@class='letter-card__word']/span/span"));
                    Console.WriteLine($"Totally found {el.Count} words to add to DB:");
                    int wordLineNo = 1;
                    Console.Write($"[{wordLineNo}] ");
                    List<string> words = new List<string>();
                    for (int i = 0; i < el.Count; i++)
                    {
                        words.Add(el[i].Text);
                        Console.Write(el[i].Text + "  ");
                        if (i % 10 == 9)
                        {
                            Console.WriteLine();
                            RequestBatchAddWords(words, ii);
                            words.Clear();
                            Console.Write($"[{++wordLineNo}] ");
                            if (wordLineNo % 10 == 0)
                            {
                                Thread.Sleep(3000);
                            }
                        }
                    }
                    if (words.Any())
                    {
                        RequestBatchAddWords(words, ii);
                        words.Clear();
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Done of length {len}!");
                    Console.WriteLine();

                    //Close and reopen the chrome and reload page
                    driver.Quit();

                    stateTimer = new Timer(statusChecker.CheckStatus,
                                               autoEvent, 2000, 1000);
                    try
                    {
                        autoEvent.WaitOne();
                    }
                    finally
                    {
                        stateTimer.Dispose();
                    }

                    driver = new ChromeDriver();
                    driver.Url = url;
                }
            }
        }

        private static void RequestBatchAddWords(List<string> words, int i)
        {
            HttpClient client = new HttpClient();
            WordList wordList = new WordList();
            wordList.Words.AddRange(words);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(wordList), Encoding.UTF8, "application/json");
            var result = client.PostAsync("http://words.franklidev.com/dictionary/add", content: content).Result;
            string resultStr = result.Content.ReadAsStringAsync().Result;
            string fileName = string.Format("Log_{0}.log", Convert.ToChar('a' + i));
            if (File.Exists(fileName))
            {
                using StreamWriter file = new StreamWriter(fileName, append: true);
                file.WriteLine("{0} -- {1}", DateTime.Now, resultStr);
            }
            else
            {
                File.WriteAllText(fileName, string.Format("{0} -- {1}", DateTime.Now, resultStr));
            }
        }

        private static void AccetpCookies(IWebDriver driver, AutoResetEvent autoEvent)
        {
            //onetrust-accept-btn-handler
            int count = 0;
            bool accepted = false;
            while (count < 5)
            {
                try
                {
                    var element = driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                    if (element != null)
                    {
                        element.Click();
                        Console.WriteLine("Accept cookies.");
                        accepted = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.StackTrace);
                }
                autoEvent.WaitOne();
                count++;
            }
            if (!accepted)
            {
                Console.WriteLine("Failed to find button 'I Accept'.");
            }
        }

        private static IWebElement LocateBodyOfWordsOfLength(IWebDriver driver, int len)
        {
            try
            {
                string text = $"{len} Letter Words";
                string path = $"//div[@class='letter-card__head'][starts-with(., '{text}')]/following-sibling::div[@class='letter-card__body']";
                var element = driver.FindElement(By.XPath(path));
                Console.WriteLine(element.Text);

                if (element != null)
                {
                    return element;
                }
                else
                {
                    Console.WriteLine($"Failed in LocateWordOfLength {len}");
                    Console.WriteLine($"XPath: {path}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed in LocateWordOfLength {len}");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        private static int GetMaxLength(IWebDriver driver)
        {
            try
            {
                var element = driver.FindElement(By.XPath("//div[@class='letter-card__head']/h2"));
                Console.WriteLine(element.Text);
                int spaceLoc = element.Text.IndexOf(' ');
                string numOfLetters = element.Text.Substring(0, spaceLoc);
                //Console.WriteLine(numOfLetters);
                int num = Int32.Parse(numOfLetters);
                Console.WriteLine($"GetMaxLength: {num}");
                return num;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed in GetMaxLength");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return 15;
            }
        }
    }
}
