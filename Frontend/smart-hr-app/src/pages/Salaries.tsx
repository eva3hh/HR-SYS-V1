import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  TextField,
  Grid,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  CircularProgress,
  Alert,
} from '@mui/material';
import { salaryAPI } from '@/api/endpoints';
import FileDownloadIcon from '@mui/icons-material/FileDownload';

const Salaries: React.FC = () => {
  const [salaries, setSalaries] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [startDate, setStartDate] = useState<string>('');
  const [endDate, setEndDate] = useState<string>('');
  const [departmentId, setDepartmentId] = useState<string>('');

  const handleGenerateReport = async () => {
    try {
      setLoading(true);
      const response = await salaryAPI.report(startDate, endDate, departmentId ? parseInt(departmentId) : undefined);
      setSalaries(response.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  const handleExportPDF = async () => {
    try {
      const blob = await salaryAPI.exportPDF('SalaryReport', {
        startDate,
        endDate,
        departmentId: departmentId ? parseInt(departmentId) : null,
      });
      const url = window.URL.createObjectURL(blob as any);
      const a = document.createElement('a');
      a.href = url;
      a.download = `تقرير_المرتبات_${new Date().toISOString()}.pdf`;
      a.click();
    } catch (err: any) {
      setError(err.response?.data?.message || 'حدث خطأ');
    }
  };

  const handleExportExcel = async () => {
    try {
      const blob = await salaryAPI.exportExcel('SalaryReport', {
        startDate,
        endDate,
        departmentId: departmentId ? parseInt(departmentId) : null,
      });
      const url = window.URL.createObjectURL(blob as any);
      const a = document.createElement('a');
      a.href = url;
      a.download = `تقرير_المرتبات_${new Date().toISOString()}.xlsx`;
      a.click();
    } catch (err: any) {
      setError(err.response?.data?.message || 'حدث خطأ');
    }
  };

  return (
    <Box sx={{ py: 3 }}>
      <Typography variant="h4" sx={{ mb: 3 }}>تقرير المرتبات</Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Grid container spacing={2} sx={{ mb: 2 }}>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="من تاريخ"
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="إلى تاريخ"
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                fullWidth
                InputLabelProps={{ shrink: true }}
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="القسم (اختياري)"
                type="number"
                value={departmentId}
                onChange={(e) => setDepartmentId(e.target.value)}
                fullWidth
              />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <Button
                variant="contained"
                fullWidth
                onClick={handleGenerateReport}
                sx={{ height: '56px' }}
              >
                عرض التقرير
              </Button>
            </Grid>
          </Grid>

          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              variant="outlined"
              startIcon={<FileDownloadIcon />}
              onClick={handleExportPDF}
            >
              تصدير PDF
            </Button>
            <Button
              variant="outlined"
              startIcon={<FileDownloadIcon />}
              onClick={handleExportExcel}
            >
              تصدير Excel
            </Button>
          </Box>
        </CardContent>
      </Card>

      {loading ? (
        <CircularProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                <TableCell align="right">الموظف</TableCell>
                <TableCell align="right">القسم</TableCell>
                <TableCell align="right">الراتب الإجمالي</TableCell>
                <TableCell align="right">الخصومات</TableCell>
                <TableCell align="right">صافي المرتب</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {salaries.map((salary) => (
                <TableRow key={salary.employeeId}>
                  <TableCell align="right">{salary.employeeName}</TableCell>
                  <TableCell align="right">{salary.department}</TableCell>
                  <TableCell align="right">{salary.totalSalary.toFixed(2)}</TableCell>
                  <TableCell align="right">{salary.totalDeductions.toFixed(2)}</TableCell>
                  <TableCell align="right">{salary.netPay.toFixed(2)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Box>
  );
};

export default Salaries;
