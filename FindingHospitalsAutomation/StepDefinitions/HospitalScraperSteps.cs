using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities;
using FindingHospitalsAutomation.Utilities.Logger;
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

        [When(@"I process the top (.*) hospitals")]
        public void WhenIProcessTheTopHospitals(int count)
        {
            resultsPage.ProcessTopHospitals(count);
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
