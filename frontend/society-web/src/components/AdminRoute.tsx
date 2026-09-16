import React from 'react';
import { Navigate } from 'react-router-dom';

interface AdminRouteProps {
  children: React.ReactNode;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
  // TODO: Implement actual admin authentication check
  const isAuthenticated = localStorage.getItem('adminToken') !== null;
  const isAdmin = localStorage.getItem('isAdmin') === 'true';

  if (!isAuthenticated || !isAdmin) {
    return <Navigate to="/admin/login" replace />;
  }

  return <>{children}</>;
};

export default AdminRoute;
