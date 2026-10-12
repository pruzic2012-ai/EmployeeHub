
using EmployeeHub.Blazor.Models;
using EmployeeHub.Blazor.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Xml;

namespace EmployeeHub.Blazor.Components.Pages;

public class DepartmentReportBase : ComponentBase
{
    [Inject]
    protected DepartmentReportService ReportApi { get; set; } = default!;

    protected string SelectedDepartment { get; set; } = "Finance";

    protected DepartmentReportModel? Report { get; set; }

    protected string ErrorMessage { get; set; } = string.Empty;

    protected bool IsLoading { get; set; }

    protected async Task GenerateReportAsync()
    {
        if (IsLoading)
        {
            return;
        }

        ErrorMessage = string.Empty;
        Report = null;
        IsLoading = true;

        try
        {
            var html = await ReportApi.GenerateReportAsync(SelectedDepartment);

            if (string.IsNullOrWhiteSpace(html))
            {
                ErrorMessage = "The department report was empty.";
                return;
            }

            Report = ParseReport(html);
        }
        catch (HttpRequestException)
        {
            ErrorMessage =
                "Unable to generate the department report. Please try again.";
        }
        catch (System.Text.Json.JsonException)
        {
            ErrorMessage =
                "The department report could not be read.";
        }
        finally
        {
            IsLoading = false;
        }
    }


    private static DepartmentReportModel ParseReport(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var reportText = HtmlEntity.DeEntitize(
            document.DocumentNode.InnerText);

        var lines = reportText
            .Split(new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        string FindValue(params string[] labels)
        {
            foreach (var line in lines)
            {
                var normalized = line
                    .TrimStart('-', '*', '•', ' ')
                    .Trim();

                foreach (var label in labels)
                {
                    var prefix = label + ":";

                    if (normalized.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return normalized[prefix.Length..].Trim();
                    }
                }
            }

            return string.Empty;
        }

        var observation = string.Empty;

        var observationLabels = new[]
        {
            "Brief observation:",
            "Business Observation:",
            "Observation:"
        };

        foreach (var label in observationLabels)
        {
            var index = reportText.IndexOf(
                label,
                StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                observation = reportText[(index + label.Length)..]
                    .Trim();

                // If there is another section after the observation,
                // keep only the observation text before that section.
                var nextSection = observation.IndexOf(
                    "\n\n",
                    StringComparison.Ordinal);

                if (nextSection >= 0)
                {
                    observation = observation[..nextSection].Trim();
                }

                break;
            }
        }


        return new DepartmentReportModel
        {
            Department = FindValue("Department"),
            EmployeeCount = FindValue(
                "Employee count", "Employees", "Employee Count"),
            TotalSalary = FindValue("Total salary"),
            AverageSalary = FindValue("Average salary"),
            Observation = observation ?? string.Empty
        };
    }

}
