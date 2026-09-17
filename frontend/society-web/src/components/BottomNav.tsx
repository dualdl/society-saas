import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  BottomNavigation,
  BottomNavigationAction,
  Paper,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import {
  Home,
  Receipt,
  Payment,
  MoreVert,
  People,
  Assessment,
  Settings,
  Upload,
  History,
} from '@mui/icons-material';

const moreMenuItems = [
  { text: 'Members', icon: <People />, path: '/app/members' },
  { text: 'Reports', icon: <Assessment />, path: '/app/reports' },
  { text: 'Import', icon: <Upload />, path: '/app/imports' },
  { text: 'Audit Trail', icon: <History />, path: '/app/audit' },
  { text: 'Settings', icon: <Settings />, path: '/app/settings' },
];

const BottomNav: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const getValue = () => {
    if (location.pathname === '/app') return 0;
    if (location.pathname === '/app/flats') return 1;
    if (location.pathname === '/app/billing') return 2;
    if (location.pathname === '/app/payments') return 3;
    return 4;
  };

  const handleChange = (_: React.SyntheticEvent, newValue: number) => {
    if (newValue === 4) {
      setAnchorEl(document.getElementById('more-button'));
    } else {
      const paths = ['/app', '/app/flats', '/app/billing', '/app/payments'];
      navigate(paths[newValue]);
    }
  };

  const handleMoreClick = (path: string) => {
    setAnchorEl(null);
    navigate(path);
  };

  return (
    <Paper sx={{ position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000 }} elevation={3}>
      <BottomNavigation
        value={getValue()}
        onChange={handleChange}
        showLabels
      >
        <BottomNavigationAction label="Home" icon={<Home />} />
        <BottomNavigationAction label="Flats" icon={<Receipt />} />
        <BottomNavigationAction label="Billing" icon={<Payment />} />
        <BottomNavigationAction label="Payments" icon={<Payment />} />
        <BottomNavigationAction label="More" icon={<MoreVert />} id="more-button" />
      </BottomNavigation>
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={() => setAnchorEl(null)}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
        transformOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        {moreMenuItems.map((item) => (
          <MenuItem key={item.path} onClick={() => handleMoreClick(item.path)}>
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText>{item.text}</ListItemText>
          </MenuItem>
        ))}
      </Menu>
    </Paper>
  );
};

export default BottomNav;
