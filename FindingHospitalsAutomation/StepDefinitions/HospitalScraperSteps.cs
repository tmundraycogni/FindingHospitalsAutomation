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

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
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
            ExtentReportHelper.LogInfo("Navigating to: https://www.practo.com/search/hospitals?results_type=hospital&q=[{\"word\":\"hospital\",\"autocompleted\":true,\"category\":\"type\"}]&city=Bangalore");

            driver.Navigate().GoToUrl("https://www.practo.com/search/hospitals?results_type=hospital&q=[{\"word\":\"hospital\",\"autocompleted\":true,\"category\":\"type\"}]&city=Bangalore");
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
        }

        [AfterScenario("HospitalScrape")]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after hospital scrape scenario...");
            WebDriverManager.DisposeDriver();
        }
    }
}
