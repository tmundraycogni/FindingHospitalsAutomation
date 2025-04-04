using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Reporting;
using FindingHospitalsAutomation.Utilities.Screenshots;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using FindingHospitalsAutomation;
using FindingHospitalsAutomation.Utilities.Config;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    [Parallelizable]
    [Category("parallel")]
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

            TryBringBrowserToFront();
            Thread.Sleep(800);

            hospitalPage.SetLocation("Bangalore");
            Thread.Sleep(500);
            hospitalPage.SetSearchTerm("Hospital");

            var screenshotBytes = ScreenshotHelper.CaptureScreenshotAsBytes(driver, "SearchPage");
            ExtentReportHelper.AttachScreenshot("Search Result Page", screenshotBytes);
        }

        [Then(@"I should be taken to the hospital search results page")]
        public void ThenIShouldBeTakenToTheHospitalSearchResultsPage()
        {
            ExtentReportHelper.LogInfo("Verifying that hospital results have loaded...");

            bool resultsVisible = false;
            for (int i = 0; i < 5; i++)
            {
                if (hospitalPage.IsResultsPageLoaded())
                {
                    resultsVisible = true;
                    break;
                }
                Thread.Sleep(2000);
            }

            if (resultsVisible)
            {
                ExtentReportHelper.LogPass("Search results successfully loaded.");
                Assert.That(resultsVisible, Is.True);
            }
            else
            {
                ExtentReportHelper.LogFail("Search results did not load correctly.");
                Assert.That(resultsVisible, Is.True, "Search results page failed to load.");
            }
        }

        [Then(@"I should be navigated back to the homepage")]
        public void ThenIShouldBeNavigatedBackToTheHomepage()
        {
            ExtentReportHelper.LogInfo("Navigating back to the homepage...");
            Log.Info("Navigating back to the homepage...");

            var homepageUrl = PageUrlConfig.GetUrl("homepage");
            driver.Navigate().GoToUrl(homepageUrl);

            Assert.That(driver.Url, Does.Contain("practo.com"), "Did not navigate back to homepage.");
            ExtentReportHelper.LogPass("Successfully navigated back to the homepage.");
        }

        private void TryBringBrowserToFront()
        {
            try
            {
                var windowHandle = ((OpenQA.Selenium.Chrome.ChromeDriver)driver).CurrentWindowHandle;
                driver.SwitchTo().Window(windowHandle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not bring browser to front: {ex.Message}");
            }
        }
    }
}
