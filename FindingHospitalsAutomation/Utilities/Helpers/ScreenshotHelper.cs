using OpenQA.Selenium;
using System;
using System.IO;
using FindingHospitalsAutomation.Utilities.Reporting;

namespace FindingHospitalsAutomation.Utilities.Screenshots
{
    public static class ScreenshotHelper
    {
        public static byte[] CaptureScreenshotAsBytes(IWebDriver driver, string namePrefix = "screenshot")
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                byte[] bytes = screenshot.AsByteArray;

                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, $"{namePrefix}_{DateTime.Now:yyyyMMdd_HHmmssfff}.png");
                File.WriteAllBytes(filePath, bytes);

                Console.WriteLine($"📸 Screenshot saved: {filePath}");
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Screenshot failed: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public static void CaptureAndAttach(IWebDriver driver, string namePrefix)
        {
            var bytes = CaptureScreenshotAsBytes(driver, namePrefix);
            if (bytes.Length > 0)
            {
                ExtentReportHelper.AttachScreenshot(namePrefix, bytes);
            }
        }
    }
}
