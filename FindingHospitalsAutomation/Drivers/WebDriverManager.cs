using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace FindingHospitalsAutomation.Drivers
{
    public static class WebDriverManager
    {
        private static AsyncLocal<IWebDriver?> driver = new();

        public static IWebDriver GetDriver()
        {
            if (driver.Value == null || IsDriverInvalid(driver.Value))
            {
                var options = new ChromeOptions();
                options.AddArgument("start-maximized");

                driver.Value = new ChromeDriver(options);
            }

            return driver.Value;
        }

        public static void QuitDriver()
        {
            if (driver.Value != null)
            {
                try
                {
                    driver.Value.Quit();
                }
                catch
                {
                    // Ignore cleanup errors
                }
                driver.Value = null;
            }
        }

        private static bool IsDriverInvalid(IWebDriver? currentDriver)
        {
            try
            {
                _ = currentDriver?.WindowHandles;
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
