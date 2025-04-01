using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities.Accessibility;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Reporting;
using NUnit.Framework;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    public class SearchHospitalAccessibilitySteps
    {
        private readonly IWebDriver driver;
        private readonly HospitalSearchPage hospitalPage;

        public SearchHospitalAccessibilitySteps()
        {
            driver = WebDriverManager.GetDriver();
            hospitalPage = new HospitalSearchPage(driver);
        }

        [Given(@"I open the hospital search page for accessibility testing")]
        public void GivenIOpenTheHospitalSearchPageForAccessibilityTesting()
        {
            ExtentReportHelper.CreateTest("Accessibility scan: Hospital search page");
            ExtentReportHelper.LogInfo("Navigating to homepage...");

            Log.Info("Navigating to homepage...");
            hospitalPage.OpenHomePage();
            hospitalPage.SetLocation("Bangalore");
            hospitalPage.SetSearchTerm("Hospital");
        }

        [Then(@"I perform an accessibility scan on the hospital search results page")]
        public void ThenIPerformAnAccessibilityScanOnTheHospitalSearchResultsPage()
        {
            ExtentReportHelper.LogInfo("Running Axe accessibility scan...");
            AxeAccessibilityHelper.RunAxeScan(driver, "HospitalSearch_Accessibility");
        }

        [AfterScenario("Accessibility")]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after accessibility test...");
            WebDriverManager.DisposeDriver();
        }
    }
}
