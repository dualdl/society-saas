import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline, Box } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Pages
import Landing from './pages/Landing';
import SocietyLogin from './pages/auth/SocietyLogin';
import AdminLogin from './pages/auth/AdminLogin';
import Dashboard from './pages/Dashboard';
import Flats from './pages/Flats';
import Members from './pages/Members';
import Billing from './pages/Billing';
import Payments from './pages/Payments';
import Receipts from './pages/Receipts';
import Reports from './pages/Reports';
import Imports from './pages/Imports';
import Audit from './pages/Audit';
import Charges from './pages/Charges';
import OpeningBalances from './pages/OpeningBalances';
import LateFee from './pages/LateFee';
import Settings from './pages/Settings';
import ResidentDashboard from './pages/ResidentDashboard';
import AdminDashboard from './pages/admin/Dashboard';
import AdminSocieties from './pages/admin/Societies';
import AdminUsers from './pages/admin/Users';
import AdminAudit from './pages/admin/Audit';

// Components
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import Sidebar from './components/Sidebar';
import AdminSidebar from './components/AdminSidebar';

// Theme
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
      light: '#42a5f5',
      dark: '#1565c0',
    },
    secondary: {
      main: '#9c27b0',
      light: '#ba68c8',
      dark: '#7b1fa2',
    },
    background: {
      default: '#f5f5f5',
    },
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          borderRadius: 8,
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 12,
        },
      },
    },
  },
});

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
          <Router>
            <Routes>
              {/* Public Routes */}
              <Route path="/" element={<Landing />} />
              <Route path="/society/login" element={<SocietyLogin />} />
              <Route path="/admin/login" element={<AdminLogin />} />
              <Route path="/resident" element={<ResidentDashboard />} />

              {/* Protected Society Routes */}
              <Route
                path="/app"
                element={
                  <ProtectedRoute>
                    <Sidebar><Dashboard /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/flats"
                element={
                  <ProtectedRoute>
                    <Sidebar><Flats /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/members"
                element={
                  <ProtectedRoute>
                    <Sidebar><Members /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/billing"
                element={
                  <ProtectedRoute>
                    <Sidebar><Billing /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/payments"
                element={
                  <ProtectedRoute>
                    <Sidebar><Payments /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/receipts"
                element={
                  <ProtectedRoute>
                    <Sidebar><Receipts /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/reports"
                element={
                  <ProtectedRoute>
                    <Sidebar><Reports /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/imports"
                element={
                  <ProtectedRoute>
                    <Sidebar><Imports /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/audit"
                element={
                  <ProtectedRoute>
                    <Sidebar><Audit /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/charges"
                element={
                  <ProtectedRoute>
                    <Sidebar><Charges /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/opening-balances"
                element={
                  <ProtectedRoute>
                    <Sidebar><OpeningBalances /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/late-fee"
                element={
                  <ProtectedRoute>
                    <Sidebar><LateFee /></Sidebar>
                  </ProtectedRoute>
                }
              />
              <Route
                path="/app/settings"
                element={
                  <ProtectedRoute>
                    <Sidebar><Settings /></Sidebar>
                  </ProtectedRoute>
                }
              />

              {/* Protected Admin Routes */}
              <Route
                path="/admin/dashboard"
                element={
                  <AdminRoute>
                    <AdminSidebar><AdminDashboard /></AdminSidebar>
                  </AdminRoute>
                }
              />
              <Route
                path="/admin/societies"
                element={
                  <AdminRoute>
                    <AdminSidebar><AdminSocieties /></AdminSidebar>
                  </AdminRoute>
                }
              />
              <Route
                path="/admin/users"
                element={
                  <AdminRoute>
                    <AdminSidebar><AdminUsers /></AdminSidebar>
                  </AdminRoute>
                }
              />
              <Route
                path="/admin/audit"
                element={
                  <AdminRoute>
                    <AdminSidebar><AdminAudit /></AdminSidebar>
                  </AdminRoute>
                }
              />

              {/* Fallback */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </Router>
        </Box>
      </ThemeProvider>
    </QueryClientProvider>
  );
}

export default App;
