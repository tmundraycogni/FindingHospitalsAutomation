using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Threading;

namespace FindingHospitalsAutomation.Utilities.Reporting
{
    public static class ExtentReportHelper
    {
        private static AsyncLocal<ExtentReports> extent = new();
        private static AsyncLocal<ExtentTest> test = new();
        private static AsyncLocal<ExtentSparkReporter> reporter = new();

        private static readonly string reportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

        public static void InitializeReport(string scenarioName)
        {
            if (!Directory.Exists(reportDirectory))
                Directory.CreateDirectory(reportDirectory);

            string safeName = string.Join("_", scenarioName.Split(Path.GetInvalidFileNameChars()));
            string reportPath = Path.Combine(reportDirectory, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmssfff}.html");

            reporter.Value = new ExtentSparkReporter(reportPath);
            extent.Value = new ExtentReports();
            extent.Value.AttachReporter(reporter.Value);

            reporter.Value.Config.DocumentTitle = "Finding Hospitals Report";
            reporter.Value.Config.ReportName = "BDD Hospital Results";

            extent.Value.AddSystemInfo("Tested By", "QA Engineer");
            extent.Value.AddSystemInfo("Environment", "Practo");
            extent.Value.AddSystemInfo("Browser", "Chrome");

            Console.WriteLine("✅ Extent Report Initialized for scenario: " + scenarioName);
        }

        public static void CreateTest(string testName)
        {
            test.Value = extent.Value.CreateTest(testName);
            Console.WriteLine($"📝 Test Created: {testName}");
        }

        public static void LogInfo(string message)
        {
            test.Value?.Info(message);
            Console.WriteLine($"[INFO] {message}");
        }

        public static void LogPass(string message)
        {
            test.Value?.Pass(message);
            Console.WriteLine($"[PASS] {message}");
        }

        public static void LogFail(string message)
        {
            test.Value?.Fail(message);
            Console.WriteLine($"[FAIL] {message}");
        }

        public static void LogWarning(string message)
        {
            test.Value?.Warning(message);
            Console.WriteLine($"[WARN] {message}");
        }

        public static void AttachScreenshot(string title, byte[] screenshotBytes)
        {
            if (screenshotBytes.Length > 0)
            {
                string base64 = Convert.ToBase64String(screenshotBytes);
                test.Value?.AddScreenCaptureFromBase64String(base64, title);
                Console.WriteLine("📸 Screenshot attached to report.");
            }
            else
            {
                test.Value?.Warning("⚠️ Screenshot was not captured (empty byte array)");
            }
        }

        public static void AttachTextFile(string title, string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                test.Value?.Info(title).Info(MarkupHelper.CreateCodeBlock(content));
                Console.WriteLine("📄 Text file attached to report.");
            }
            else
            {
                test.Value?.Warning("⚠️ Text file not found: " + filePath);
            }
        }

        public static void FlushReport()
        {
            extent.Value?.Flush();
            Console.WriteLine("📁 Extent Report flushed.");
        }
    }
}
