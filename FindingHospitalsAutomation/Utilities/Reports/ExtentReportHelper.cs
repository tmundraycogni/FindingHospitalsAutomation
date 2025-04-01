using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;

namespace FindingHospitalsAutomation.Utilities.Reporting
{
    public static class ExtentReportHelper
    {
        private static ExtentReports? extent;
        private static ExtentTest? test;
        private static ExtentSparkReporter? sparkReporter;

        private static readonly string reportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
        private static readonly string reportPath = Path.Combine(reportDirectory, $"ExtentReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

        public static void InitializeReport()
        {
            if (!Directory.Exists(reportDirectory))
                Directory.CreateDirectory(reportDirectory);

            sparkReporter = new ExtentSparkReporter(reportPath);
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);

            sparkReporter.Config.DocumentTitle = "Finding Hospitals Report";
            sparkReporter.Config.ReportName = "BDD Hospital Results";

            extent.AddSystemInfo("Tested By", "QA Engineer");
            extent.AddSystemInfo("Environment", "Practo");
            extent.AddSystemInfo("Browser", "Chrome");

            Console.WriteLine("✅ Extent Report Initialized");
        }

        public static void CreateTest(string testName)
        {
            test = extent?.CreateTest(testName);
            Console.WriteLine($"📝 Test Created: {testName}");
        }

        public static void LogInfo(string message)
        {
            test?.Info(message);
            Console.WriteLine($"[INFO] {message}");
        }

        public static void LogPass(string message)
        {
            test?.Pass(message);
            Console.WriteLine($"[PASS] {message}");
        }

        public static void LogFail(string message)
        {
            test?.Fail(message);
            Console.WriteLine($"[FAIL] {message}");
        }

        public static void LogWarning(string message)
        {
            test?.Warning(message);
            Console.WriteLine($"[WARN] {message}");
        }

        public static void AttachScreenshot(string title, byte[] screenshotBytes)
        {
            if (screenshotBytes.Length > 0)
            {
                string base64 = Convert.ToBase64String(screenshotBytes);
                test?.AddScreenCaptureFromBase64String(base64, title);
                Console.WriteLine("📸 Screenshot attached to report.");
            }
            else
            {
                test?.Warning("⚠️ Screenshot was not captured (empty byte array)");
            }
        }

        public static void AttachTextFile(string title, string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                test?.Info(title).Info(MarkupHelper.CreateCodeBlock(content));
                Console.WriteLine("📄 Text file attached to report.");
            }
            else
            {
                test?.Warning("⚠️ Text file not found: " + filePath);
            }
        }

        public static void FlushReport()
        {
            extent?.Flush();
            Console.WriteLine($"📁 Report saved to: {Path.GetFullPath(reportPath)}");
        }
    }
}
