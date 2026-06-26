import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
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
import { BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, LineChart, Line } from 'recharts';
import { reportsAPI } from '@/api/endpoints';

const Dashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [summary, setSummary] = useState<any>(null);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        setLoading(true);
        const response = await reportsAPI.getDashboard();
        setSummary(response.data);
      } catch (err: any) {
        setError(err.response?.data?.message || 'حدث خطأ في تحميل البيانات');
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, []);

  if (loading) return <CircularProgress />;

  if (error) return <Alert severity="error">{error}</Alert>;

  const attendanceData = [
    { name: 'حاضر', value: summary?.presentToday || 0, fill: '#8884d8' },
    { name: 'غائب', value: summary?.absentToday || 0, fill: '#82ca9d' },
    { name: 'متأخر', value: summary?.lateToday || 0, fill: '#ffc658' },
  ];

  return (
    <Box sx={{ py: 3 }}>
      <Typography variant="h4" sx={{ mb: 3 }}>لوحة التحكم</Typography>

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                إجمالي الموظفين
              </Typography>
              <Typography variant="h5">
                {summary?.totalEmployees || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                الحاضرون اليوم
              </Typography>
              <Typography variant="h5" sx={{ color: '#8884d8' }}>
                {summary?.presentToday || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                الغائبون اليوم
              </Typography>
              <Typography variant="h5" sx={{ color: '#82ca9d' }}>
                {summary?.absentToday || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                المتأخرون
              </Typography>
              <Typography variant="h5" sx={{ color: '#ffc658' }}>
                {summary?.lateToday || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>إحصائيات الحضور</Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={attendanceData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={(entry) => `${entry.name}: ${entry.value}`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {attendanceData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.fill} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>الموظفين الأكثر تأخيراً</Typography>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell align="right">الاسم</TableCell>
                    <TableCell align="right">عدد أيام التأخير</TableCell>
                    <TableCell align="right">متوسط التأخير (دقائق)</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {summary?.topLateComers?.slice(0, 10).map((row: any) => (
                    <TableRow key={row.employeeId}>
                      <TableCell align="right">{row.employeeName}</TableCell>
                      <TableCell align="right">{row.totalLateDays}</TableCell>
                      <TableCell align="right">{row.averageLateMinutes}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Dashboard;
