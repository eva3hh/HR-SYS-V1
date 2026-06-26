import React from 'react';
import { Box, Container, AppBar, Toolbar, Typography, Button, Menu, MenuItem, Avatar, CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { useAuth } from '@/context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { createContext, useState, useMemo } from 'react';

interface LayoutProps {
  children: React.ReactNode;
}

interface ThemeContextType {
  darkMode: boolean;
  toggleTheme: () => void;
}

export const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [darkMode, setDarkMode] = useState(false);

  const theme = useMemo(
    () =>
      createTheme({
        palette: {
          mode: darkMode ? 'dark' : 'light',
          primary: {
            main: '#1976d2',
          },
          secondary: {
            main: '#dc004e',
          },
        },
        direction: 'rtl',
      }),
    [darkMode]
  );

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
    handleMenuClose();
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{ display: 'flex' }}>
        <AppBar position="fixed">
          <Toolbar>
            <Typography variant="h6" sx={{ flexGrow: 1 }}>
              Smart HR ERP
            </Typography>
            <Button color="inherit" onClick={() => navigate('/dashboard')}>
              لوحة التحكم
            </Button>
            <Button color="inherit" onClick={() => navigate('/employees')}>
              الموظفين
            </Button>
            <Button color="inherit" onClick={() => navigate('/attendance')}>
              الحضور
            </Button>
            <Button color="inherit" onClick={() => navigate('/salaries')}>
              الرواتب
            </Button>
            <Button color="inherit" onClick={() => navigate('/reports')}>
              التقارير
            </Button>
            <Avatar
              onClick={handleMenuOpen}
              sx={{ cursor: 'pointer', ml: 2 }}
              src={user?.profilePicturePath}
            >
              {user?.firstName?.charAt(0)}
            </Avatar>
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
            >
              <MenuItem>{user?.firstName} {user?.lastName}</MenuItem>
              <MenuItem onClick={() => navigate('/settings')}>الإعدادات</MenuItem>
              <MenuItem onClick={handleLogout}>تسجيل الخروج</MenuItem>
            </Menu>
          </Toolbar>
        </AppBar>
        <Box sx={{ flexGrow: 1, pt: 8 }}>
          <Container maxWidth="lg">
            {children}
          </Container>
        </Box>
      </Box>
    </ThemeProvider>
  );
};

export default Layout;
