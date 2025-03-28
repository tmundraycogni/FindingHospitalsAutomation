using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using SeleniumExtras.WaitHelpers;
using System.Linq;

namespace FindingHospitalsAutomation.Pages
{
    public class HospitalSearchPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public HospitalSearchPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Input fields
        private By locationInput => By.XPath("//*[@id='c-omni-container']/div/div[1]/div/input");
        private By searchInput => By.XPath("//*[@id='c-omni-container']/div/div[2]/div[1]/input");

        // Dropdown suggestion items
        private By suggestionItems => By.CssSelector(".c-omni-suggestion-item");

        // Hospital result cards
        private By hospitalCards => By.CssSelector(".listing-row");

        public void OpenHomePage()
        {
            driver.Navigate().GoToUrl("https://www.practo.com/");
        }

        public void SetLocation(string location)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(locationInput)).Clear();
            driver.FindElement(locationInput).SendKeys(location);
            Thread.Sleep(1500); // Let suggestions load

            SelectSuggestionByContains(suggestionItems, "Bangalore");

            wait.Until(ExpectedConditions.ElementIsVisible(searchInput));
        }

        public void SetSearchTerm(string term)
        {
            var input = wait.Until(ExpectedConditions.ElementIsVisible(searchInput));
            input.Clear();
            input.SendKeys(term);

            // Wait for dropdown to begin loading
            wait.Until(driver => driver.FindElements(suggestionItems).Count > 0);

            // Optional: wait just a bit longer to ensure full DOM structure loads
            Thread.Sleep(1000);

            SelectSearchTypeByLabel("TYPE");
        }


        private void SelectSuggestionByContains(By suggestionLocator, string keyword)
        {
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
                    catch (StaleElementReferenceException)
                    {
                        return false; // DOM refreshed, retry
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                });
            });

            // Refetch the dropdown items to get a fresh copy
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
                catch (StaleElementReferenceException)
                {
                    continue;
                }
                catch (NoSuchElementException)
                {
                    continue;
                }
            }

            throw new Exception($"No suggestion marked as type '{label}' was found.");
        }



        public void ScrollUntilFirstCardWithoutRating()
        {
            int previousCount = 0;
            int unchangedAttempts = 0;
            int sameCountLimit = 3;

            while (unchangedAttempts < sameCountLimit)
            {
                var cards = driver.FindElements(hospitalCards);
                int currentCount = cards.Count;

                // Stop if we find a card without rating
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

                // Scroll smoothly to 80% of the page
                ((IJavaScriptExecutor)driver).ExecuteScript(@"
            window.scrollTo({
                top: document.body.scrollHeight * 0.8,
                behavior: 'smooth'
            });
        ");

                Thread.Sleep(2000); // Allow time for lazy loading

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
                    continue; // Skip any cards missing elements
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


    }
}
