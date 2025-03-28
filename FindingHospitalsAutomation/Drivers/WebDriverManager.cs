using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FindingHospitalsAutomation.Drivers
{
    public static class WebDriverManager
    {
        private static IWebDriver driver;

        public static IWebDriver GetDriver()
        {
            if (driver == null || IsDriverInvalid())
            {
                driver = new ChromeDriver();
                driver.Manage().Window.Maximize();
            }

            return driver;
        }

        public static void DisposeDriver()
        {
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                }
                catch
                {
                    // Ignore any errors during cleanup
                }
                driver = null;
            }
        }

        private static bool IsDriverInvalid()
        {
            try
            {
                _ = driver?.WindowHandles;
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static void QuitDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null!;
            }
        }

    }
}
