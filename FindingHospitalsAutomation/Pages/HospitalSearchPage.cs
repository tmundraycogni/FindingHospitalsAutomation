using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using SeleniumExtras.WaitHelpers;
using System.Linq;
using System.Threading;
using FindingHospitalsAutomation;
using FindingHospitalsAutomation.Utilities.Config;

namespace FindingHospitalsAutomation.Pages
{
    public class HospitalSearchPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public HospitalSearchPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        private By locationInput => By.XPath("//*[@id='c-omni-container']/div/div[1]/div/input");
        private By searchInput => By.XPath("//*[@id='c-omni-container']/div/div[2]/div[1]/input");
        private By suggestionItems => By.CssSelector(".c-omni-suggestion-item");
        private By hospitalCards => By.CssSelector(".listing-row");

        public void OpenHomePage()
        {
            string homepageUrl = PageUrlConfig.GetUrl("homepage");
            driver.Navigate().GoToUrl(homepageUrl);
            FocusWindow();
            Thread.Sleep(1000);
        }

        public void SetLocation(string location)
        {
            FocusWindow();
            int attempts = 0;
            bool success = false;

            while (attempts < 3 && !success)
            {
                try
                {
                    var input = wait.Until(ExpectedConditions.ElementIsVisible(locationInput));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", input);
                    input.Click();
                    input.Clear();
                    input.SendKeys(location);
                    Thread.Sleep(2000);

                    SelectSuggestionByContains(suggestionItems, "Bangalore");
                    wait.Until(ExpectedConditions.ElementIsVisible(searchInput));
                    success = true;
                }
                catch
                {
                    attempts++;
                    Thread.Sleep(1000);
                }
            }

            if (!success)
                throw new Exception("❌ Failed to set location input field after multiple retries.");
        }

        public void SetSearchTerm(string term)
        {
            FocusWindow();
            var input = wait.Until(ExpectedConditions.ElementIsVisible(searchInput));
            input.Clear();
            input.SendKeys(term);

            wait.Until(driver => driver.FindElements(suggestionItems).Count > 0);
            Thread.Sleep(1500);

            SelectSearchTypeByLabel("TYPE");
        }

        private void SelectSuggestionByContains(By suggestionLocator, string keyword)
        {
            FocusWindow();
            wait.Until(ExpectedConditions.ElementIsVisible(suggestionLocator));
            var items = driver.FindElements(suggestionLocator);

            foreach (var item in items)
            {
                if (item.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    item.Click();
                    Thread.Sleep(1000);
                    return;
                }
            }

            throw new Exception($"No suggestion containing '{keyword}' was found.");
        }

        private void SelectSearchTypeByLabel(string label)
        {
            wait.Until(driver =>
            {
                var elements = driver.FindElements(suggestionItems);
                return elements.Any(el =>
                {
                    try
                    {
                        var rightLabel = el.FindElement(By.CssSelector("[data-qa-id='omni-suggestion-right']"));
                        return rightLabel.Text.Trim().Equals(label, StringComparison.OrdinalIgnoreCase);
                    }
                    catch { return false; }
                });
            });

            var items = driver.FindElements(suggestionItems);

            foreach (var item in items)
            {
                try
                {
                    var rightLabel = item.FindElement(By.CssSelector("[data-qa-id='omni-suggestion-right']"));
                    if (rightLabel.Text.Trim().Equals(label, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Click();
                        Thread.Sleep(1000);
                        return;
                    }
                }
                catch { continue; }
            }

            throw new Exception($"No suggestion marked as type '{label}' was found.");
        }

        public void ScrollUntilFirstCardWithoutRating()
        {
            FocusWindow();
            int previousCount = 0;
            int unchangedAttempts = 0;
            int sameCountLimit = 3;

            while (unchangedAttempts < sameCountLimit)
            {
                var cards = driver.FindElements(hospitalCards);
                int currentCount = cards.Count;

                foreach (var card in cards)
                {
                    string cardText = card.Text;

                    if (!cardText.Contains("stars", StringComparison.OrdinalIgnoreCase) &&
                        !cardText.Contains("Rated", StringComparison.OrdinalIgnoreCase) &&
                        !cardText.Contains("Patient Stories", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Found card without rating. Stopping scroll.");
                        return;
                    }
                }

                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                    window.scrollTo({
                        top: document.body.scrollHeight * 0.8,
                        behavior: 'smooth'
                    });");

                Thread.Sleep(2500);

                cards = driver.FindElements(hospitalCards);
                if (cards.Count > previousCount)
                {
                    previousCount = cards.Count;
                    unchangedAttempts = 0;
                }
                else
                {
                    unchangedAttempts++;
                }
            }

            Console.WriteLine("Stopped scrolling — no more hospitals loaded.");
        }

        public bool AreHospitalResultsVisible()
        {
            try
            {
                var titleElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("h1.title")));
                string titleText = titleElement.Text;

                return titleText.Contains("Hospitals", StringComparison.OrdinalIgnoreCase)
                    && titleText.Contains("Bangalore", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetTopRatedHospitalsOpen24x7(double minRating)
        {
            List<string> result = new List<string>();
            ReadOnlyCollection<IWebElement> cards = driver.FindElements(hospitalCards);

            foreach (var card in cards)
            {
                try
                {
                    string cardText = card.Text;

                    if (!cardText.Contains("Open 24x7", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var ratingElement = card.FindElement(By.CssSelector(".u-bold.u-c-theme"));
                    if (double.TryParse(ratingElement.Text.Trim(), out double rating) && rating >= minRating)
                    {
                        var nameElement = card.FindElement(By.CssSelector("h2"));
                        result.Add(nameElement.Text);
                    }
                }
                catch
                {
                    continue;
                }
            }

            return result;
        }

        public bool IsResultsPageLoaded()
        {
            try
            {
                var title = wait.Until(driver => driver.FindElement(By.CssSelector("h1.title")));
                return title.Text.Contains("Hospitals", StringComparison.OrdinalIgnoreCase)
                    && title.Text.Contains("Bangalore", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void FocusWindow()
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.focus();");
                Thread.Sleep(300);
            }
            catch { }
        }
    }
}
