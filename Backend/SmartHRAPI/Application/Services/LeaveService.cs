using SmartHRAPI.Core.Entities;
using SmartHRAPI.Core.Interfaces;

namespace SmartHRAPI.Application.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LeaveRequest> RequestLeaveAsync(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, string reason)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("الموظف غير موجود");

            var daysRequested = (endDate.Date - startDate.Date).TotalDays + 1;

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                StartDate = startDate,
                EndDate = endDate,
                DaysRequested = (decimal)daysRequested,
                Reason = reason,
                Status = "Pending"
            };

            await _unitOfWork.LeaveRepository.AddAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<bool> ApproveLeaveAsync(int leaveRequestId, int approvedBy)
        {
            var leaveRequest = await _unitOfWork.LeaveRepository.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null)
                return false;

            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedBy = approvedBy;
            leaveRequest.ApprovalDate = DateTime.UtcNow;

            await _unitOfWork.LeaveRepository.UpdateAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectLeaveAsync(int leaveRequestId, string reason)
        {
            var leaveRequest = await _unitOfWork.LeaveRepository.GetByIdAsync(leaveRequestId);
            if (leaveRequest == null)
                return false;

            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovalNotes = reason;
            leaveRequest.ApprovalDate = DateTime.UtcNow;

            await _unitOfWork.LeaveRepository.UpdateAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<LeaveBalance> GetLeaveBalanceAsync(int employeeId, int year)
        {
            var leaveBalance = new LeaveBalance
            {
                EmployeeId = employeeId,
                Year = year,
                TotalEntitlement = 30,
                LastUpdated = DateTime.UtcNow
            };

            return await Task.FromResult(leaveBalance);
        }
    }
}