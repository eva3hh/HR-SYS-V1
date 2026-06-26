import api from './client';

export interface Employee {
  id: number;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  departmentName: string;
  jobTitleName: string;
  basicSalary: number;
  employmentStatus: string;
  hiringDate: string;
  profilePicturePath?: string;
}

export interface Salary {
  id: number;
  employeeId: number;
  employeeName: string;
  month: number;
  year: number;
  basicSalary: number;
  totalAllowances: number;
  totalBonuses: number;
  totalDeductions: number;
  grossAmount: number;
  netAmount: number;
  status: string;
}

export interface AttendanceRecord {
  id: number;
  employeeName: string;
  checkInTime: string;
  checkOutTime?: string;
  status: string;
  lateMinutes: number;
  overtimeHours: number;
}

export interface DashboardSummary {
  totalEmployees: number;
  presentToday: number;
  absentToday: number;
  lateToday: number;
  totalOvertimeHours: number;
  totalPayableThisMonth: number;
  onLeaveToday: number;
  topLateComers: Array<{
    employeeId: number;
    employeeName: string;
    totalLateDays: number;
    averageLateMinutes: number;
  }>;
}

// Employee API
export const employeeAPI = {
  getAll: (page = 1, pageSize = 10) =>
    api.get<Employee[]>(`/employees?page=${page}&pageSize=${pageSize}`),
  getById: (id: number) => api.get<Employee>(`/employees/${id}`),
  create: (data: Partial<Employee>) => api.post<Employee>('/employees', data),
  update: (id: number, data: Partial<Employee>) =>
    api.put<Employee>(`/employees/${id}`, data),
  delete: (id: number) => api.delete(`/employees/${id}`),
  search: (term: string) => api.get<Employee[]>(`/employees/search/${term}`),
};

// Salary API
export const salaryAPI = {
  calculate: (employeeId: number, month: number, year: number) =>
    api.post<Salary>(`/salaries/calculate/${employeeId}?month=${month}&year=${year}`),
  getById: (id: number) => api.get<Salary>(`/salaries/${id}`),
  getByEmployee: (employeeId: number) =>
    api.get<Salary[]>(`/salaries/employee/${employeeId}`),
  approve: (salaryId: number) => api.post(`/salaries/approve/${salaryId}`),
  finalize: (salaryId: number) => api.post(`/salaries/finalize/${salaryId}`),
  report: (startDate: string, endDate: string, departmentId?: number) =>
    api.get(`/salaries/report?startDate=${startDate}&endDate=${endDate}&departmentId=${departmentId}`),
};

// Attendance API
export const attendanceAPI = {
  getByDate: (date: string, employeeId?: number) =>
    api.get<AttendanceRecord[]>(`/attendance?date=${date}&employeeId=${employeeId}`),
  record: (employeeId: number, checkIn: string, checkOut?: string) =>
    api.post<AttendanceRecord>(
      `/attendance/record?employeeId=${employeeId}&checkIn=${checkIn}&checkOut=${checkOut}`
    ),
  report: (startDate: string, endDate: string) =>
    api.get(`/attendance/report?startDate=${startDate}&endDate=${endDate}`),
  syncDevice: (deviceId: number) => api.post(`/attendance/sync-device/${deviceId}`),
};

// Reports API
export const reportsAPI = {
  getDashboard: () => api.get<DashboardSummary>('/reports/dashboard'),
  attendanceReport: (startDate: string, endDate: string, departmentId?: number) =>
    api.get(`/reports/attendance?startDate=${startDate}&endDate=${endDate}&departmentId=${departmentId}`),
  departmentReport: () => api.get('/reports/departments'),
  exportPDF: (reportName: string, parameters: any) =>
    api.post(`/reports/export-pdf?reportName=${reportName}`, parameters, {
      responseType: 'blob',
    }),
  exportExcel: (reportName: string, parameters: any) =>
    api.post(`/reports/export-excel?reportName=${reportName}`, parameters, {
      responseType: 'blob',
    }),
};
