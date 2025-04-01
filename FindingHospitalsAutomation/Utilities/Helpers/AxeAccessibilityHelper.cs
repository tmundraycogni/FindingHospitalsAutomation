using OpenQA.Selenium;
using Selenium.Axe;
using System;
using System.IO;
using System.Text;
using FindingHospitalsAutomation.Utilities.Reporting;

namespace FindingHospitalsAutomation.Utilities.Accessibility
{
    public static class AxeAccessibilityHelper
    {
        public static void RunAxeScan(IWebDriver driver, string reportNamePrefix = "AxeReport")
        {
            var axeBuilder = new AxeBuilder(driver);
            var results = axeBuilder.Analyze();

            if (results.Violations.Length > 0)
            {
                Console.WriteLine($"⚠️ {results.Violations.Length} accessibility violations found.");
            }
            else
            {
                Console.WriteLine("✅ No accessibility violations found.");
                return;
            }

            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AccessibilityReports");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, $"{reportNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            var sb = new StringBuilder();

            foreach (var violation in results.Violations)
            {
                sb.AppendLine($"❌ {violation.Id} - {violation.Description}");
                sb.AppendLine($"Help: {violation.HelpUrl}");
                foreach (var node in violation.Nodes)
                {
                    sb.AppendLine($"  ▶ Affected Element: {node.Html}");
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
            Console.WriteLine($"📄 Accessibility report saved to: {filePath}");

            // 🔗 Attach to Extent Report
            ExtentReportHelper.AttachTextFile("Accessibility Report", filePath);
        }
    }
}
