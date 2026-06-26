using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHRAPI.Core.DTOs;
using SmartHRAPI.Core.Interfaces;

namespace SmartHRAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalariesController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalariesController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpGet("{salaryId}")]
        public async Task<ActionResult<SalaryDto>> GetSalary(int salaryId)
        {
            try
            {
                var salary = await _salaryService.GetSalaryAsync(salaryId);
                return Ok(salary);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<SalaryDto>>> GetEmployeeSalaries(int employeeId)
        {
            try
            {
                var salaries = await _salaryService.GetEmployeeSalariesAsync(employeeId);
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("calculate/{employeeId}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<ActionResult<SalaryDto>> CalculateSalary(int employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var salary = await _salaryService.CalculateSalaryAsync(employeeId, month, year);
                return Ok(salary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("approve/{salaryId}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> ApproveSalary(int salaryId)
        {
            try
            {
                var result = await _salaryService.ApproveSalaryAsync(salaryId);
                if (!result)
                    return NotFound(new { message = "الراتب غير موجود" });

                return Ok(new { message = "تم الموافقة على الراتب" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("finalize/{salaryId}")]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> FinalizeSalary(int salaryId)
        {
            try
            {
                var result = await _salaryService.FinalizeSalaryAsync(salaryId);
                if (!result)
                    return NotFound(new { message = "الراتب غير موجود" });

                return Ok(new { message = "تم إنهاء الراتب" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin,Accountant,HR")]
        public async Task<ActionResult<IEnumerable<SalaryReportDto>>> GenerateSalaryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId = null)
        {
            try
            {
                var report = await _salaryService.GenerateSalaryReportAsync(startDate, endDate, departmentId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}