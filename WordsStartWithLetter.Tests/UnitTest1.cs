using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace WordsStartWithLetter.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private IWebDriver _driver;
        [TestInitialize]
        public void Initialization()
        {
            _driver = new ChromeDriver();
        }

        [TestCleanup]
        public void EndTest()
        {
            //driver.Close();
        }

        [TestMethod]
        public void ExpandAllWords()
        {
            _driver.Url = "https://word.tips/words-start-with/a/";

            var autoEvent = new AutoResetEvent(false);
            var statusChecker = new StatusChecker(1);
            var stateTimer = new Timer(statusChecker.CheckStatus,
                                       autoEvent, 2000, 1000);

            AccetpCookies(autoEvent);

            while (true)
            {
                var elements = _driver.FindElements(By.ClassName("primary"));

                bool found = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    string text = elements[i].Text;
                    Console.Write(text);
                    if (text.ToLower() == "more")
                    {
                        Console.WriteLine(" --- Click");
                        elements[i].Click();
                        found = true;
                        autoEvent.WaitOne();
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                    if (found) break;
                }

                if (!found) break;
            }
            stateTimer.Dispose();
            Console.WriteLine("\nDestroying timer.");

        }

        private void AccetpCookies(AutoResetEvent autoEvent)
        {
            //onetrust-accept-btn-handler
            int count = 0;
            while(count < 5)
            {
                try
                {
                    var element = _driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                    if(element != null)
                    {
                        element.Click();
                        Console.WriteLine("Accept cookies.");
                        count = 5;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Failed to find button 'I Accept'.");
                }
                autoEvent.WaitOne();
                count++;
            }
        }

        [TestMethod]
        public void GetAllWords()
        {
            _driver.Url = "https://word.tips/words-start-with/a/";
            var elements = _driver.FindElements(By.XPath("//div[@class='letter-card__list']//span[@class='letter-card__word']//span//span"));
            Console.WriteLine($"Totally found {elements.Count} words:");
            for (int i=0; i<elements.Count; i++)
            {
                Console.Write(elements[i].Text+"  ");
                if (i % 10 == 9) Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Done!");
        }

        class StatusChecker
        {
            private int invokeCount;
            private int maxCount;

            public StatusChecker(int count)
            {
                invokeCount = 0;
                maxCount = count;
            }

            // This method is called by the timer delegate.
            public void CheckStatus(Object stateInfo)
            {
                AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
                Console.WriteLine("{0} Checking status {1,2}.",
                    DateTime.Now.ToString("h:mm:ss.fff"),
                    (++invokeCount).ToString());

                if (invokeCount == maxCount)
                {
                    // Reset the counter and signal the waiting thread.
                    invokeCount = 0;
                    autoEvent.Set();
                }
            }
        }
    }
}
