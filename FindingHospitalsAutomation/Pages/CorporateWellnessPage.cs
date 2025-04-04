using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;
using FindingHospitalsAutomation.Utilities.Logger;
using FindingHospitalsAutomation.Utilities.Config;

namespace FindingHospitalsAutomation.Pages
{
    public class CorporateWellnessPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public CorporateWellnessPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void NavigateToFormViaHeader()
        {
            var homepageUrl = PageUrlConfig.GetUrl("homepage");
            driver.Navigate().GoToUrl(homepageUrl);

            try
            {
                var consentBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.fc-cta-consent")));
                consentBtn.Click();
                Log.Info("✅ Consent popup dismissed.");
            }
            catch { }

            var forCorporates = wait.Until(ExpectedConditions.ElementExists(By.XPath("//span[contains(text(),'For Corporates')]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", forCorporates);
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", forCorporates);
            Thread.Sleep(1000);

            var wellnessLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Health & Wellness Plans')]")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", wellnessLink);
            Thread.Sleep(3000);

            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 200);");
            Thread.Sleep(1000);
        }

        public void FillInvalidFormDetails()
        {
            driver.FindElement(By.Id("name")).SendKeys("");
            driver.FindElement(By.Id("organizationName")).SendKeys("");
            driver.FindElement(By.Id("contactNumber")).SendKeys("123");
            driver.FindElement(By.Id("officialEmailId")).SendKeys("invalidemail");

            var orgSizeDropdown = new SelectElement(driver.FindElement(By.Id("organizationSize")));
            orgSizeDropdown.SelectByIndex(1);

            var interestDropdown = new SelectElement(driver.FindElement(By.Id("interestedIn")));
            interestDropdown.SelectByIndex(1);
        }

        public bool IsFormValidationTriggered()
        {
            try
            {
                var demoButton = wait.Until(ExpectedConditions.ElementExists(
                    By.XPath("//button[contains(text(),'Schedule a demo')]")));

                bool isDisabled = demoButton.GetAttribute("disabled") != null;
                Log.Info("🔍 Schedule a demo button is " + (isDisabled ? "disabled" : "enabled"));

                return isDisabled;
            }
            catch (Exception ex)
            {
                Log.Error("❌ Error checking demo button state: " + ex.Message);
                return false;
            }
        }
    }
}
