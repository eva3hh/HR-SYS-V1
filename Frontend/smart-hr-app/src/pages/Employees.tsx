import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  CircularProgress,
  Alert,
  Card,
  CardContent,
  Grid,
  Typography,
} from '@mui/material';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { employeeAPI, Employee } from '@/api/endpoints';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

const Employees: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [paginationModel, setPaginationModel] = useState({ pageSize: 10, page: 0 });
  const [formData, setFormData] = useState<Partial<Employee>>({
    employeeCode: '',
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    basicSalary: 0,
  });

  useEffect(() => {
    fetchEmployees();
  }, [paginationModel]);

  const fetchEmployees = async () => {
    try {
      setLoading(true);
      const response = await employeeAPI.getAll(
        paginationModel.page + 1,
        paginationModel.pageSize
      );
      setEmployees(response.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'حدث خطأ');
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (employee?: Employee) => {
    if (employee) {
      setEditingId(employee.id);
      setFormData(employee);
    } else {
      setEditingId(null);
      setFormData({
        employeeCode: '',
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        basicSalary: 0,
      });
    }
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleSave = async () => {
    try {
      if (editingId) {
        await employeeAPI.update(editingId, formData);
      } else {
        await employeeAPI.create(formData);
      }
      handleCloseDialog();
      fetchEmployees();
    } catch (err: any) {
      setError(err.response?.data?.message || 'حدث خطأ');
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('هل تريد حذف الموظف؟')) {
      try {
        await employeeAPI.delete(id);
        fetchEmployees();
      } catch (err: any) {
        setError(err.response?.data?.message || 'حدث خطأ');
      }
    }
  };

  const columns: GridColDef[] = [
    { field: 'employeeCode', headerName: 'الرقم الوظيفي', width: 130 },
    { field: 'firstName', headerName: 'الاسم الأول', width: 130 },
    { field: 'lastName', headerName: 'اللقب', width: 130 },
    { field: 'email', headerName: 'البريد الإلكتروني', width: 180 },
    { field: 'phoneNumber', headerName: 'الهاتف', width: 130 },
    { field: 'departmentName', headerName: 'القسم', width: 130 },
    { field: 'jobTitleName', headerName: 'الوظيفة', width: 130 },
    { field: 'basicSalary', headerName: 'الراتب الأساسي', width: 130 },
    { field: 'employmentStatus', headerName: 'الحالة', width: 100 },
    {
      field: 'actions',
      headerName: 'الإجراءات',
      width: 120,
      sortable: false,
      renderCell: (params) => (
        <Box>
          <Button
            startIcon={<EditIcon />}
            onClick={() => handleOpenDialog(params.row)}
            size="small"
          />
          <Button
            startIcon={<DeleteIcon />}
            onClick={() => handleDelete(params.row.id)}
            size="small"
            color="error"
          />
        </Box>
      ),
    },
  ];

  return (
    <Box sx={{ py: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">الموظفين</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
        >
          إضافة موظف
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {loading ? (
        <CircularProgress />
      ) : (
        <Card>
          <CardContent>
            <DataGrid
              rows={employees}
              columns={columns}
              paginationModel={paginationModel}
              onPaginationModelChange={setPaginationModel}
              pageSizeOptions={[5, 10, 20]}
              loading={loading}
            />
          </CardContent>
        </Card>
      )}

      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>
          {editingId ? 'تعديل الموظف' : 'إضافة موظف جديد'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
            <TextField
              label="الرقم الوظيفي"
              value={formData.employeeCode || ''}
              onChange={(e) =>
                setFormData({ ...formData, employeeCode: e.target.value })
              }
              fullWidth
            />
            <TextField
              label="الاسم الأول"
              value={formData.firstName || ''}
              onChange={(e) =>
                setFormData({ ...formData, firstName: e.target.value })
              }
              fullWidth
            />
            <TextField
              label="اللقب"
              value={formData.lastName || ''}
              onChange={(e) =>
                setFormData({ ...formData, lastName: e.target.value })
              }
              fullWidth
            />
            <TextField
              label="البريد الإلكتروني"
              value={formData.email || ''}
              onChange={(e) =>
                setFormData({ ...formData, email: e.target.value })
              }
              fullWidth
              type="email"
            />
            <TextField
              label="الهاتف"
              value={formData.phoneNumber || ''}
              onChange={(e) =>
                setFormData({ ...formData, phoneNumber: e.target.value })
              }
              fullWidth
            />
            <TextField
              label="الراتب الأساسي"
              value={formData.basicSalary || ''}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  basicSalary: parseFloat(e.target.value),
                })
              }
              fullWidth
              type="number"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>إلغاء</Button>
          <Button onClick={handleSave} variant="contained">
            حفظ
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Employees;
