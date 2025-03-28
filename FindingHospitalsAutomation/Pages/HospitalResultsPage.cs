using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FindingHospitalsAutomation.Models;
using FindingHospitalsAutomation.Utilities.Csv;

namespace FindingHospitalsAutomation.Pages
{
    public class HospitalResultsPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;
        private bool consentHandled = false;

        public HospitalResultsPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private By hospitalCards => By.CssSelector("div.c-estb-card");

        public void ProcessTopHospitals(int limit = 5)
        {
            var allCards = driver.FindElements(hospitalCards).ToList();

            if (allCards.Count < limit)
            {
                Console.WriteLine($"⚠️ Only {allCards.Count} hospitals found. Will process those.");
                limit = allCards.Count;
            }

            var hospitalList = new List<HospitalInfo>();
            int processed = 0;

            while (processed < limit)
            {
                var card = allCards[processed];

                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", card);
                    Thread.Sleep(1000);

                    var link = card.FindElement(By.CssSelector("a[target='_blank']"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.open(arguments[0].href, '_blank');", link);

                    var tabs = driver.WindowHandles;
                    if (tabs.Count < 2)
                    {
                        Console.WriteLine("❌ Failed to open new tab for hospital.");
                        processed++;
                        continue;
                    }

                    driver.SwitchTo().Window(tabs[1]);
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 150);");

                    wait.Until(d =>
                        d.Url.Contains("/hospital/") &&
                        d.FindElements(By.CssSelector("span[data-qa-id='read_more_info']")).Any()
                    );

                    if (!consentHandled)
                    {
                        try
                        {
                            var consentBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
                                By.CssSelector("button.fc-cta-consent")));
                            consentBtn.Click();
                            Console.WriteLine("✅ Consent popup dismissed.");
                            Thread.Sleep(1000);
                            consentHandled = true;
                        }
                        catch (WebDriverTimeoutException)
                        {
                            Console.WriteLine("ℹ️ No consent popup found.");
                            consentHandled = true;
                        }
                    }

                    try
                    {
                        var readMore = driver.FindElement(By.CssSelector("span[data-qa-id='read_more_info']"));
                        readMore.Click();
                        Thread.Sleep(1000);
                    }
                    catch { }

                    bool hasRating = false, isOpen24x7 = false, hasParking = false;
                    double rating = 0.0;
                    string hospitalLocation = "N/A";

                    try
                    {
                        var ratingElement = driver.FindElement(By.CssSelector("span.common__star-rating__value"));
                        if (double.TryParse(ratingElement.Text.Trim(), out rating))
                            hasRating = rating >= 4.5;
                    }
                    catch { }

                    try
                    {
                        var timingElement = driver.FindElement(By.CssSelector("p.u-green-text"));
                        isOpen24x7 = timingElement.Text.Trim().Contains("Open 24", StringComparison.OrdinalIgnoreCase);
                    }
                    catch { }

                    try
                    {
                        var amenities = driver.FindElements(By.CssSelector("span[data-qa-id='amenity_item']"));
                        hasParking = amenities.Any(a => a.Text.Trim().Equals("Parking", StringComparison.OrdinalIgnoreCase));
                    }
                    catch { }

                    try
                    {
                        var loc = driver.FindElement(By.CssSelector("p[data-qa-id='address_body']"));
                        hospitalLocation = loc.Text.Replace("\n", " ").Replace("\r", "").Trim();

                        if (hospitalLocation.EndsWith("Get Directions", StringComparison.OrdinalIgnoreCase))
                        {
                            hospitalLocation = hospitalLocation.Substring(0, hospitalLocation.Length - "Get Directions".Length).Trim();
                        }
                    }
                    catch { }

                    if (isOpen24x7 && hasParking && hasRating)
                    {
                        var name = driver.Title.Split('|')[0].Trim();

                        hospitalList.Add(new HospitalInfo
                        {
                            Name = name,
                            Rating = rating,
                            IsOpen24x7 = true,
                            HasParking = true,
                            Location = hospitalLocation
                        });

                        Console.WriteLine($"✅ Hospital #{processed + 1} meets all criteria.");
                        Console.WriteLine($"   ➤ {name}");
                        Console.WriteLine($"   ➤ Rating: {rating}");
                        Console.WriteLine($"   ➤ Location: {hospitalLocation}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Hospital #{processed + 1} does not meet all criteria.");
                    }

                    driver.Close();
                    driver.SwitchTo().Window(tabs[0]);

                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                window.scrollBy({
                    top: 200,
                    left: 0,
                    behavior: 'smooth'
                });
            ");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error processing hospital #{processed + 1}: {ex.Message}");
                }

                processed++;
            }

            CsvWriterHelper.WriteHospitalsToCsv(hospitalList);
            Console.WriteLine($"🎉 Processed {processed} hospitals and exported to CSV.");
        }
    }
}
