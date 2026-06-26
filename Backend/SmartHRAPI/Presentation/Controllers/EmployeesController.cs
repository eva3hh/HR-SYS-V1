using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHRAPI.Core.DTOs;
using SmartHRAPI.Core.Interfaces;

namespace SmartHRAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync(page, pageSize);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeRequest request)
        {
            try
            {
                var employee = await _employeeService.CreateEmployeeAsync(request);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, CreateEmployeeRequest request)
        {
            try
            {
                var employee = await _employeeService.UpdateEmployeeAsync(id, request);
                return Ok(employee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (!result)
                    return NotFound(new { message = "الموظف غير موجود" });

                return Ok(new { message = "تم حذف الموظف بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> SearchEmployees(string searchTerm)
        {
            try
            {
                var employees = await _employeeService.SearchEmployeesAsync(searchTerm);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}