using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Services
{
    public interface IProductivityService
    {
        Task<decimal> CalculateEmployeeCompletionRateAsync(int employeeId);
        Task<decimal> CalculateEmployeeProductivityScoreAsync(int employeeId);
        Task<decimal> CalculateProjectProgressAsync(int projectId);
        Task<List<EmployeeReportItem>> GetEmployeeReportsAsync();
        Task<List<ProjectReportItem>> GetProjectReportsAsync();
        Task<MonthlyReportViewModel> GetMonthlyReportAsync(int year, int month);
        Task<List<EmployeeProductivitySummary>> GetTeamProductivitySummariesAsync(int managerId);
    }
}
