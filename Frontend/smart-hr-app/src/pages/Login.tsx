import React, { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  TextField,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import api from '@/api/client';

const Login: React.FC = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setLoading(true);
      setError(null);
      const response = await api.post('/auth/login', { username, password });
      login(response.data.token, response.data);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'فشل تسجيل الدخول');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        bgcolor: '#f5f5f5',
      }}
    >
      <Card sx={{ width: 400, maxWidth: '90%' }}>
        <CardContent>
          <Typography variant="h4" sx={{ textAlign: 'center', mb: 3 }}>
            Smart HR ERP
          </Typography>

          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

          <form onSubmit={handleLogin}>
            <TextField
              label="اسم المستخدم"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              fullWidth
              sx={{ mb: 2 }}
              disabled={loading}
            />
            <TextField
              label="كلمة المرور"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              fullWidth
              sx={{ mb: 3 }}
              disabled={loading}
            />
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={loading}
              sx={{ height: 48 }}
            >
              {loading ? <CircularProgress size={24} /> : 'تسجيل الدخول'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};

export default Login;
