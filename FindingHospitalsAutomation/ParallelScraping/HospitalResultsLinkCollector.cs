using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using SeleniumExtras.WaitHelpers;

namespace FindingHospitalsAutomation.ParallelScraping
{
    public static class HospitalResultsLinkCollector
    {
        private static By hospitalCards = By.CssSelector("div.c-estb-card");

        public static List<string> CollectHospitalLinks(IWebDriver driver, int limit)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(hospitalCards));

            var links = new List<string>();
            int processed = 0;

            while (processed < limit)
            {
                var cards = driver.FindElements(hospitalCards);

                if (processed >= cards.Count)
                {
                    Console.WriteLine("⚠️ No more hospitals to process.");
                    break;
                }

                try
                {
                    var card = cards[processed];
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", card);
                    Thread.Sleep(500);

                    var linkElement = card.FindElement(By.CssSelector("a[target='_blank']"));
                    string url = linkElement.GetAttribute("href");

                    if (!string.IsNullOrEmpty(url))
                    {
                        links.Add(url);
                        Console.WriteLine($"🔗 Collected: {url}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error collecting link at index {processed}: {ex.Message}");
                }

                processed++;

                // Scroll slightly to trigger lazy loading of new cards
                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                    window.scrollBy({
                        top: 200,
                        left: 0,
                        behavior: 'smooth'
                    });
                ");
                Thread.Sleep(500);
            }

            Console.WriteLine($"✅ Collected {links.Count} hospital links.");
            return links;
        }
    }
}
