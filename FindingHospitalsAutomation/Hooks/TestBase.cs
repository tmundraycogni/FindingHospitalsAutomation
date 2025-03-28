using Reqnroll;
using OpenQA.Selenium;
using NUnit.Framework;
using FindingHospitalsAutomation.Drivers;
using FindingHospitalsAutomation.Utilities.Reporting;
using FindingHospitalsAutomation.Utilities.Screenshots;
using AventStack.ExtentReports;

namespace FindingHospitalsAutomation.Hooks
{
    [Binding]
    public sealed class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver driver;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            ExtentReportHelper.InitializeReport();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            driver = WebDriverManager.GetDriver();
            string scenarioName = _scenarioContext.ScenarioInfo.Title;
            ExtentReportHelper.CreateTest(scenarioName);
            ExtentReportHelper.LogInfo($"🔍 Starting Scenario: {scenarioName}");
        }

        [AfterStep]
        public void AfterEachStep()
        {
            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = _scenarioContext.StepContext.StepInfo.Text;

            if (_scenarioContext.TestError != null)
            {
                // 🟥 Log failure + attach screenshot
                ExtentReportHelper.LogFail($"❌ {stepType}: {stepText}");
                var screenshotBytes = ScreenshotHelper.CaptureScreenshotAsBytes(driver);
                ExtentReportHelper.AttachScreenshot("Failure Screenshot", screenshotBytes);
            }
            else
            {
                // ✅ Log step pass
                ExtentReportHelper.LogPass($"✔️ {stepType}: {stepText}");
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ExtentReportHelper.LogInfo("🧹 Closing browser after scenario...");
            WebDriverManager.QuitDriver();
            ExtentReportHelper.FlushReport();
        }
    }
}
