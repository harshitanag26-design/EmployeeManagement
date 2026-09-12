using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly IProductivityService _productivityService;

        public ReportsController(IProductivityService productivityService)
        {
            _productivityService = productivityService;
        }

        public async Task<IActionResult> Index(int? year, int? month)
        {
            int selectedYear = year ?? DateTime.Today.Year;
            int selectedMonth = month ?? DateTime.Today.Month;

            var employeeReports = await _productivityService.GetEmployeeReportsAsync();
            var projectReports = await _productivityService.GetProjectReportsAsync();
            var monthlyReport = await _productivityService.GetMonthlyReportAsync(selectedYear, selectedMonth);

            var vm = new GlobalReportsViewModel
            {
                EmployeeReports = employeeReports,
                ProjectReports = projectReports,
                MonthlyReport = monthlyReport
            };

            return View(vm);
        }
    }
}
