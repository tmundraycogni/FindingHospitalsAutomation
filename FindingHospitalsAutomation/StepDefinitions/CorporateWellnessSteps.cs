using Reqnroll;
using NUnit.Framework;
using OpenQA.Selenium;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Pages;
using FindingHospitalsAutomation.Utilities.Logger;

namespace FindingHospitalsAutomation.StepDefinitions
{
    [Binding]
    [Parallelizable]
    [Category("parallel")]
    public class CorporateWellnessSteps
    {
        private readonly IWebDriver driver;
        private readonly CorporateWellnessPage wellnessPage;

        public CorporateWellnessSteps()
        {
            driver = WebDriverManager.GetDriver();
            wellnessPage = new CorporateWellnessPage(driver);
        }

        [Given("I navigate to the Corporate Wellness form")]
        public void GivenINavigateToTheCorporateWellnessForm()
        {
            Log.Info("Navigating to Corporate Wellness form...");
            wellnessPage.NavigateToFormViaHeader();
        }

        [When("I submit the form with invalid details")]
        public void WhenISubmitTheFormWithInvalidDetails()
        {
            Log.Info("Filling and submitting invalid details...");
            wellnessPage.FillInvalidFormDetails();
        }

        [Then("a validation error should be shown")]
        public void ThenAValidationErrorShouldBeShown()
        {
            Log.Info("Checking for validation feedback...");
            bool isValidationShown = wellnessPage.IsFormValidationTriggered();
            Assert.That(isValidationShown, Is.True, "Validation error not triggered as expected.");
        }

        [AfterScenario("CorporateWellness")]
        public void CleanUp()
        {
            Log.Info("🧹 Closing browser after scenario...");
            WebDriverManager.QuitDriver();
        }
    }
}
