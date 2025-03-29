using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace FindingHospitalsAutomation.Pages
{
    public class DiagnosticsPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public DiagnosticsPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void NavigateToDiagnosticsPage()
        {
            driver.Navigate().GoToUrl("https://www.practo.com/");
            HandleConsentPopup();

            // Click Surgeries tab
            var surgeriesTab = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[text()='Surgeries']")));
            surgeriesTab.Click();

            // Wait for Lab Tests to appear and click it
            var labTestsLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Lab Tests')]")));
            labTestsLink.Click();
        }

        public List<string> ExtractTopCities()
        {
            HandleConsentPopup(); // Just in case it appears again

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div.styles_cityName__3eGvH")));
            var cityElements = driver.FindElements(By.CssSelector("div.styles_cityName__3eGvH"));

            List<string> cityNames = new List<string>();
            foreach (var el in cityElements)
            {
                cityNames.Add(el.Text.Trim());
            }

            return cityNames;
        }

        private void HandleConsentPopup()
        {
            try
            {
                var consentBtn = driver.FindElement(By.CssSelector("button.fc-cta-consent"));
                if (consentBtn.Displayed)
                {
                    consentBtn.Click();
                    Thread.Sleep(300); // Allow popup to dismiss
                }
            }
            catch
            {
                // Popup not found, ignore
            }
        }
    }
}
