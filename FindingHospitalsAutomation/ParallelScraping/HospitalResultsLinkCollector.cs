using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace FindingHospitalsAutomation.ParallelScraping
{
    public static class HospitalResultsLinkCollector
    {
        private static readonly By hospitalCards = By.CssSelector("div.c-estb-card");

        public static List<string> CollectHospitalLinks(IWebDriver driver, int limit)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(hospitalCards));

            var links = new List<string>();
            var seenUrls = new HashSet<string>();

            while (links.Count < limit)
            {
                var cards = driver.FindElements(hospitalCards);

                foreach (var card in cards)
                {
                    try
                    {
                        var linkEl = card.FindElement(By.CssSelector("a[target='_blank']"));
                        var url = linkEl.GetAttribute("href");

                        if (!string.IsNullOrEmpty(url) && !seenUrls.Contains(url))
                        {
                            links.Add(url);
                            seenUrls.Add(url);
                            Console.WriteLine($"🔗 Collected: {url}");

                            if (links.Count >= limit)
                                break;
                        }
                    }
                    catch
                    {
                        // Skip cards with missing or malformed links
                    }
                }

                // ⚖️ Medium speed scroll
                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                    window.scrollBy({
                        top: 250,
                        left: 0,
                        behavior: 'smooth'
                    });
                ");
                Thread.Sleep(1000);
            }

            Console.WriteLine($"✅ Collected {links.Count} hospital links.");
            return links;
        }
    }
}
