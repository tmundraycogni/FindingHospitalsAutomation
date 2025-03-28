using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FindingHospitalsAutomation.Models;
using FindingHospitalsAutomation.Utilities;

namespace FindingHospitalsAutomation.ParallelScraping
{
    public class ParallelHospitalScraper
    {
        public static void ScrapeHospitalPagesInParallel(List<string> urls)
        {
            var results = new List<HospitalInfo>();
            var locker = new object();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(urls, options, url =>
            {
                IWebDriver driver = null!;

                try
                {
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("start-maximized");

                    driver = new ChromeDriver(chromeOptions);
                    driver.Navigate().GoToUrl(url);

                    // ✅ Dismiss data consent popup if present
                    try
                    {
                        var consentBtn = new WebDriverWait(driver, TimeSpan.FromSeconds(3))
                            .Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.fc-cta-consent")));
                        consentBtn.Click();
                        Console.WriteLine("✅ Consent popup dismissed.");

                        // ✅ Scroll down slightly to hide sticky header
                        ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 200);");
                        Thread.Sleep(500);
                    }
                    catch (WebDriverTimeoutException)
                    {
                        // No popup shown
                    }

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector("span[data-qa-id='read_more_info']")));

                    try
                    {
                        var readMore = driver.FindElement(By.CssSelector("span[data-qa-id='read_more_info']"));
                        readMore.Click();
                        Thread.Sleep(1000);
                    }
                    catch { }

                    bool hasRating = false, isOpen24x7 = false, hasParking = false;
                    double rating = 0.0;
                    string location = "N/A";

                    try
                    {
                        var ratingEl = driver.FindElement(By.CssSelector("span.common__star-rating__value"));
                        if (double.TryParse(ratingEl.Text.Trim(), out rating))
                            hasRating = rating >= 4.5;
                    }
                    catch { }

                    try
                    {
                        var timeEl = driver.FindElement(By.CssSelector("p.u-green-text"));
                        isOpen24x7 = timeEl.Text.Trim().Contains("Open 24", StringComparison.OrdinalIgnoreCase);
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
                        var locationElement = driver.FindElement(By.CssSelector("p[data-qa-id='address_body']"));
                        location = locationElement.Text
                            .Replace("Get Directions", "")
                            .Replace("\r", "")
                            .Replace("\n", " ")
                            .Trim();
                    }
                    catch { }

                    if (hasRating && isOpen24x7 && hasParking)
                    {
                        var name = driver.Title.Split('|')[0].Trim();

                        var hospital = new HospitalInfo
                        {
                            Name = name,
                            Rating = rating,
                            IsOpen24x7 = true,
                            HasParking = true,
                            Location = location
                        };

                        lock (locker)
                        {
                            results.Add(hospital);
                        }

                        Console.WriteLine($"✅ Matched: {name}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Hospital did not meet criteria.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Parallel scrape error: {ex.Message}");
                }
                finally
                {
                    driver?.Quit();
                }
            });

            if (results.Count > 0)
            {
                CsvWriterHelper.WriteToCsv(results);
                Console.WriteLine($"✅ Saved {results.Count} hospitals to CSV.");
            }
            else
            {
                Console.WriteLine("⚠️ No valid hospitals to write.");
            }
        }
    }
}
