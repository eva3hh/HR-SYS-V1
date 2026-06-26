using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHRAPI.Core.DTOs;
using SmartHRAPI.Core.Interfaces;

namespace SmartHRAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceRecordDto>>> GetAttendance([FromQuery] DateTime date, [FromQuery] int? employeeId = null)
        {
            try
            {
                var records = await _attendanceService.GetAttendanceAsync(date, employeeId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("record")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<AttendanceRecordDto>> RecordAttendance([FromQuery] int employeeId, [FromQuery] DateTime checkIn, [FromQuery] DateTime? checkOut = null)
        {
            try
            {
                var record = await _attendanceService.RecordAttendanceAsync(employeeId, checkIn, checkOut);
                return Ok(record);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<ActionResult<IEnumerable<AttendanceReportDto>>> GetAttendanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var records = await _attendanceService.GetAttendanceReportAsync(startDate, endDate);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("sync-device/{deviceId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> SyncDevice(int deviceId)
        {
            try
            {
                await _attendanceService.SyncDeviceAsync(deviceId);
                return Ok(new { message = "تم مزامنة الجهاز بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}