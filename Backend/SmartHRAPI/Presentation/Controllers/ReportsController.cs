using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHRAPI.Core.DTOs;
using SmartHRAPI.Core.Interfaces;

namespace SmartHRAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
        {
            try
            {
                var summary = await _reportService.GetDashboardSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("attendance")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<ActionResult<IEnumerable<AttendanceReportDto>>> GenerateAttendanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId = null)
        {
            try
            {
                var report = await _reportService.GenerateAttendanceReportAsync(startDate, endDate, departmentId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("departments")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<ActionResult<IEnumerable<DepartmentReportDto>>> GenerateDepartmentReport()
        {
            try
            {
                var report = await _reportService.GenerateDepartmentReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("export-pdf")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ExportReportToPdf([FromQuery] string reportName, [FromBody] Dictionary<string, object> parameters)
        {
            try
            {
                var pdfContent = await _reportService.ExportReportToPdfAsync(reportName, parameters);
                return File(pdfContent, "application/pdf", $"{reportName}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("export-excel")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ExportReportToExcel([FromQuery] string reportName, [FromBody] Dictionary<string, object> parameters)
        {
            try
            {
                var excelContent = await _reportService.ExportReportToExcelAsync(reportName, parameters);
                return File(excelContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}