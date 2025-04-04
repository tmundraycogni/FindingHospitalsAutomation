using Reqnroll;
using OpenQA.Selenium;
using NUnit.Framework;
using System.IO;
using System;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.ParallelScraping;
using FindingHospitalsAutomation.Utilities.Csv;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Reporting;
using FindingHospitalsAutomation.Utilities.Screenshots;
using FindingHospitalsAutomation.Utilities.Config;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    [Category("scraper")]
    public class HospitalScraperSteps
    {
        private readonly IWebDriver driver;

        public HospitalScraperSteps()
        {
            driver = WebDriverManager.GetDriver();
        }

        [Given(@"I navigate to the hospital results page from config")]
        public void GivenINavigateToTheHospitalResultsPageFromConfig()
        {
            ExtentReportHelper.CreateTest("Filter top-rated 24x7 hospitals using parallel scraping");

            string url = TestConfigLoader.GetSearchUrl();

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url), "URL cannot be null.");

            ExtentReportHelper.LogInfo($"Navigating to: {url}");
            driver.Navigate().GoToUrl(url);
        }

        [When(@"I extract and scrape the top (.*) hospital links in parallel")]
        public void WhenIExtractAndScrapeTheTopHospitalLinksInParallel(int count)
        {
            var hospitalLinks = HospitalResultsLinkCollector.CollectHospitalLinks(driver, count);
            ParallelHospitalScraper.ScrapeHospitalsInParallel(hospitalLinks);

            var screenshotBytes = ScreenshotHelper.CaptureScreenshotAsBytes(driver, "HospitalScrape");
            ExtentReportHelper.AttachScreenshot("Scraped Hospitals", screenshotBytes);
        }

        [Then(@"the valid hospital data should be saved to the CSV")]
        public void ThenTheValidHospitalDataShouldBeSavedToTheCSV()
        {
            string filePath = CsvWriterHelper.GetCsvFilePath();
            Assert.That(File.Exists(filePath), Is.True, "CSV file was not created.");

            ExtentReportHelper.AttachTextFile("Hospital Results CSV", filePath);
        }

        [AfterScenario("HospitalScrape")]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after hospital scrape scenario...");
            WebDriverManager.QuitDriver();
        }
    }
}
