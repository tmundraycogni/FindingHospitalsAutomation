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

        [BeforeScenario]
        public void BeforeScenario()
        {
            string scenarioName = _scenarioContext.ScenarioInfo.Title;

            // 💡 Initialize a unique report for each scenario
            ExtentReportHelper.InitializeReport(scenarioName);
            ExtentReportHelper.CreateTest(scenarioName);
            ExtentReportHelper.LogInfo($"🔍 Starting Scenario: {scenarioName}");

            driver = WebDriverManager.GetDriver();
        }

        [AfterStep]
        public void AfterEachStep()
        {
            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = _scenarioContext.StepContext.StepInfo.Text;

            if (_scenarioContext.TestError != null)
            {
                ExtentReportHelper.LogFail($"❌ {stepType}: {stepText}");
                var screenshotBytes = ScreenshotHelper.CaptureScreenshotAsBytes(driver);
                ExtentReportHelper.AttachScreenshot("Failure Screenshot", screenshotBytes);
            }
            else
            {
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
