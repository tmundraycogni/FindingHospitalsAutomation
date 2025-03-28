using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.ParallelScraping;
using NUnit.Framework;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    public class HospitalScraperSteps
    {
        private readonly IWebDriver driver;
        private readonly HospitalResultsPage resultsPage;

        public HospitalScraperSteps()
        {
            driver = WebDriverManager.GetDriver();
            resultsPage = new HospitalResultsPage(driver);
        }

        [Given(@"I navigate to the hospital results page from config")]
        public void GivenINavigateToTheHospitalResultsPageFromConfig()
        {
            var data = HospitalSearchDataReader.LoadData();
            Log.Info($"Navigating to: {data.searchUrl}");
            driver.Navigate().GoToUrl(data.searchUrl);
        }

        [When(@"I extract and scrape the top (.*) hospital links in parallel")]
        public void WhenIExtractAndScrapeTheTopHospitalLinksInParallel(int count)
        {
            Log.Info($"⏳ Collecting top {count} hospital links...");
            var links = HospitalResultsLinkCollector.CollectHospitalLinks(driver, count);
            Log.Info($"✅ Collected {links.Count} hospital links.");

            if (links.Count == 0)
            {
                Log.Warning("⚠️ No links were collected. Skipping scraping.");
                return;
            }

            ParallelHospitalScraper.ScrapeHospitalPagesInParallel(links);
        }

        [Then(@"the valid hospital data should be saved to the CSV")]
        public void ThenTheValidHospitalDataShouldBeSavedToTheCSV()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HospitalResults.csv");

            Assert.That(File.Exists(filePath), Is.True, "CSV file was not created.");
            var lines = File.ReadAllLines(filePath);
            Assert.That(lines.Length, Is.GreaterThan(1), "CSV file exists but is empty.");
        }

        [AfterScenario]
        public void CleanUp()
        {
            if (ScenarioContext.Current.TestError != null)
            {
                Log.Error("❌ Test failed: " + ScenarioContext.Current.TestError.Message);
            }

            Log.Info("🧹 Closing browser after scenario...");
            WebDriverManager.DisposeDriver();
        }
    }
}
