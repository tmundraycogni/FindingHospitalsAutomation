using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Reporting;
using FindingHospitalsAutomation.Utilities.Screenshots;
using NUnit.Framework;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    public class SearchHospitalSteps
    {
        private readonly IWebDriver driver;
        private readonly HospitalSearchPage hospitalPage;

        public SearchHospitalSteps()
        {
            driver = WebDriverManager.GetDriver();
            hospitalPage = new HospitalSearchPage(driver);
        }

        [Given(@"I open the hospital search page")]
        public void GivenIOpenTheHospitalSearchPage()
        {
            ExtentReportHelper.CreateTest("Perform a hospital search via homepage");
            ExtentReportHelper.LogInfo("Navigating to homepage...");

            Log.Info("Navigating to homepage...");
            hospitalPage.OpenHomePage();
            hospitalPage.SetLocation("Bangalore");
            hospitalPage.SetSearchTerm("Hospital");
        }

        [Then(@"I should be taken to the hospital search results page")]
        public void ThenIShouldBeTakenToTheHospitalSearchResultsPage()
        {
            ExtentReportHelper.LogInfo("Verifying that hospital results have loaded...");
            bool resultsVisible = hospitalPage.IsResultsPageLoaded();

            var screenshotBytes = ScreenshotHelper.CaptureScreenshotAsBytes(driver, "SearchPage");
            ExtentReportHelper.AttachScreenshot("Search Result Page", screenshotBytes);

            if (resultsVisible)
            {
                ExtentReportHelper.LogPass("Search results successfully loaded.");
                Assert.That(resultsVisible, Is.True);

                Log.Info("Navigating back to homepage after search...");
                driver.Navigate().GoToUrl("https://www.practo.com/");
            }
            else
            {
                ExtentReportHelper.LogFail("Search results did not load correctly.");
                Assert.That(resultsVisible, Is.True, "Search results page failed to load.");
            }
        }
    }
}
