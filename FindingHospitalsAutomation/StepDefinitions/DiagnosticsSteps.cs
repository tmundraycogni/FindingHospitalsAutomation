using Reqnroll;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Screenshots;
using FindingHospitalsAutomation.Utilities.Reporting;
using FindingHospitalsAutomation.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Threading;
using SeleniumExtras.WaitHelpers;
using FindingHospitalsAutomation.Utilities.Config;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    [Parallelizable]
    [Category("parallel")]
    public class DiagnosticsSteps
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public DiagnosticsSteps()
        {
            driver = WebDriverManager.GetDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(12));
        }

        [Given(@"I navigate to the diagnostics page from the homepage")]
        public void GivenINavigateToTheDiagnosticsPageFromTheHomepage()
        {
            Log.Info("🔍 Navigating to homepage...");
            string homepageUrl = PageUrlConfig.GetUrl("homepage");
            driver.Navigate().GoToUrl(homepageUrl);
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.focus();");

            try
            {
                var consentButton = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("button.fc-cta-consent")));
                consentButton.Click();
                Log.Info("✅ Consent popup dismissed on homepage.");
                Thread.Sleep(300);
            }
            catch (WebDriverTimeoutException) { }

            try
            {
                Log.Info("📍 Navigating to Lab Tests via Surgeries tab...");

                var surgeriesTab = wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class='product-tab']//div[text()='Surgeries']")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", surgeriesTab);
                Thread.Sleep(300);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", surgeriesTab);
                Thread.Sleep(2500);

                var labTestsTab = wait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class='product-tab']//div[text()='Lab Tests']")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", labTestsTab);
                Thread.Sleep(300);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", labTestsTab);
                Thread.Sleep(2500);
            }
            catch (Exception ex)
            {
                Log.Error("❌ Failed to navigate to Lab Tests: " + ex.Message);
                ScreenshotHelper.CaptureScreenshotAsBytes(driver, "FailedDiagnosticsNavigation");
                Assert.Fail("Navigation to Diagnostics failed.");
            }
        }

        [Then(@"I extract the top city names and save them")]
        public void ThenIExtractTheTopCityNamesAndSaveThem()
        {
            try
            {
                try
                {
                    var consentBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.fc-cta-consent")));
                    consentBtn.Click();
                    Log.Info("✅ Consent popup dismissed on Lab Tests page.");
                    Thread.Sleep(300);
                }
                catch (WebDriverTimeoutException) { }

                Log.Info("🔍 Extracting top cities...");
                var cityElements = wait.Until(d =>
                    d.FindElements(By.CssSelector("div.u-margint--standard.o-f-color--primary"))
                );

                var cities = cityElements.Select(e => e.Text.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList();

                string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DiagnosticsTopCities.csv");
                File.WriteAllLines(outputPath, new[] { "Top Cities" }.Concat(cities));

                Log.Info($"✅ Extracted {cities.Count} cities.");
                cities.ForEach(c => Console.WriteLine("📍 " + c));

                if (File.Exists(outputPath))
                {
                    ExtentReportHelper.AttachTextFile("Top Cities CSV", outputPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error("❌ Error extracting city names: " + ex.Message);
                ScreenshotHelper.CaptureScreenshotAsBytes(driver, "Diagnostics_City_Extraction_Failure");
                Assert.Fail("Failed to extract city names.");
            }
        }

        [AfterScenario("Diagnostics")]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after diagnostics scenario...");
            WebDriverManager.QuitDriver();
        }
    }
}
