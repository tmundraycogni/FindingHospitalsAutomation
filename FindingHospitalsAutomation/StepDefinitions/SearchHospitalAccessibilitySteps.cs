using Reqnroll;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities.Accessibility;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Reporting;
using NUnit.Framework;
using System;
using System.Threading;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    [Parallelizable]
    [Category("parallel")]
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

            // ✅ Bring window to front just like in search test
            TryBringBrowserToFront();

            Thread.Sleep(800); // Let page fully render

            hospitalPage.SetLocation("Bangalore");
            Thread.Sleep(500);

            hospitalPage.SetSearchTerm("Hospital");
        }

        [Then(@"I perform an accessibility scan on the hospital search results page")]
        public void ThenIPerformAnAccessibilityScanOnTheHospitalSearchResultsPage()
        {
            ExtentReportHelper.LogInfo("Running Axe accessibility scan...");
            AxeAccessibilityHelper.RunAxeScan(driver, "HospitalSearch_Accessibility");
        }

        [AfterScenario]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after accessibility test...");
            WebDriverManager.QuitDriver();
        }

        // ✅ Foreground helper copied from SearchHospitalSteps
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
