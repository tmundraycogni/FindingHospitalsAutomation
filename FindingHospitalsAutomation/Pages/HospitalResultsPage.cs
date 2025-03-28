using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using FindingHospitalsAutomation.Models;
using SeleniumExtras.WaitHelpers;
using FindingHospitalsAutomation.Utilities;

namespace FindingHospitalsAutomation.Pages
{
    public class HospitalResultsPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public HospitalResultsPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void ProcessTopHospitals(int limit)
        {
            List<HospitalInfo> hospitals = new List<HospitalInfo>();
            int processed = 0;

            while (processed < limit)
            {
                var hospitalCards = wait.Until(driver =>
                    driver.FindElements(By.CssSelector("div[data-qa-id='hospital_card']")));

                foreach (var card in hospitalCards.Skip(processed))
                {
                    try
                    {
                        ScrollIntoView(card);

                        string name = card.FindElement(By.CssSelector("h2")).Text;
                        string ratingStr = TryGetText(card, "span.common__star-rating__value");
                        string location = TryGetText(card, "div.u-color--grey");
                        string openStatus = TryGetText(card, "p.u-green-text");
                        bool hasParking = card.FindElements(By.CssSelector("span[data-qa-id='amenity_item']"))
                                              .Any(a => a.Text.Trim().Equals("Parking", StringComparison.OrdinalIgnoreCase));

                        double.TryParse(ratingStr, out double rating);

                        if (rating >= 4.5 && openStatus.Contains("24", StringComparison.OrdinalIgnoreCase) && hasParking)
                        {
                            hospitals.Add(new HospitalInfo
                            {
                                Name = name,
                                Rating = rating,
                                IsOpen24x7 = true,
                                HasParking = true,
                                Location = location.Replace("Get Directions", "").Replace("\r", "").Replace("\n", " ").Trim()
                            });
                        }

                        processed++;
                        if (processed >= limit) break;
                    }
                    catch
                    {
                        processed++;
                    }
                }

                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 500);");
                Thread.Sleep(1000);
            }

            // ✅ Corrected method name
            CsvWriterHelper.WriteToCsv(hospitals);
        }

        private void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(300);
        }

        private string TryGetText(IWebElement card, string selector)
        {
            try
            {
                return card.FindElement(By.CssSelector(selector)).Text.Trim();
            }
            catch
            {
                return "";
            }
        }
    }
}
