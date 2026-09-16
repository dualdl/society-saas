import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Container,
  Grid,
  Card,
  CardContent,
  Box,
  Avatar,
  IconButton,
  Menu,
  MenuItem,
  Link,
} from '@mui/material';
import {
  AccountBalance,
  Payment,
  Receipt,
  Assessment,
  ImportExport,
  PhoneAndroid,
  ChevronRight,
  Menu as MenuIcon,
} from '@mui/icons-material';

const features = [
  {
    icon: <AccountBalance sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Smart Billing',
    description: 'Generate monthly maintenance bills easily with configurable charges.',
  },
  {
    icon: <Payment sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Payments',
    description: 'Track every payment and receipt with complete audit trail.',
  },
  {
    icon: <Receipt sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Audit Ready',
    description: 'Complete audit trail and financial exports for statutory compliance.',
  },
  {
    icon: <ImportExport sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Excel Import',
    description: 'Onboard your society in minutes with our smart Excel import.',
  },
  {
    icon: <Assessment sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Reports',
    description: 'Download all reports in Excel with filters and previews.',
  },
  {
    icon: <PhoneAndroid sx={{ fontSize: 48, color: 'primary.main' }} />,
    title: 'Mobile',
    description: 'Manage your society from your phone with our mobile card UI.',
  },
];

const Landing: React.FC = () => {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      {/* Header */}
      <AppBar position="static" sx={{ bgcolor: 'transparent', boxShadow: 'none' }}>
        <Toolbar>
          <Typography
            variant="h6"
            sx={{ flexGrow: 1, color: 'primary.main', fontWeight: 'bold' }}
          >
            SocietyPro
          </Typography>
          <Button color="inherit" sx={{ color: 'text.primary' }}>
            Features
          </Button>
          <Button color="inherit" sx={{ color: 'text.primary' }}>
            Benefits
          </Button>
          <Button color="inherit" sx={{ color: 'text.primary' }}>
            Pricing
          </Button>
          <Button color="inherit" sx={{ color: 'text.primary' }}>
            Contact
          </Button>
          <Button
            color="inherit"
            onClick={handleMenu}
            sx={{ color: 'text.primary' }}
          >
            Login ▼
          </Button>
          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleClose}
          >
            <MenuItem onClick={() => { navigate('/society/login'); handleClose(); }}>
              Society Login
            </MenuItem>
            <MenuItem onClick={() => { navigate('/admin/login'); handleClose(); }}>
              Super Admin Login
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      {/* Hero Section */}
      <Container maxWidth="lg" sx={{ py: 8 }}>
        <Grid container spacing={4} alignItems="center">
          <Grid item xs={12} md={6}>
            <Typography variant="h3" component="h1" gutterBottom sx={{ fontWeight: 'bold' }}>
              Smart Society Management
            </Typography>
            <Typography variant="h5" color="text.secondary" paragraph>
              Made Simple
            </Typography>
            <Typography variant="body1" color="text.secondary" paragraph>
              Manage billing, payments, members, receipts, reports and audits from one platform.
            </Typography>
            <Box sx={{ mt: 4 }}>
              <Button
                variant="contained"
                size="large"
                sx={{ mr: 2 }}
                onClick={() => navigate('/society/login')}
              >
                Get Started
              </Button>
              <Button
                variant="outlined"
                size="large"
                onClick={() => navigate('/society/login')}
              >
                Society Login
              </Button>
            </Box>
          </Grid>
          <Grid item xs={12} md={6}>
            <Card sx={{ p: 3, boxShadow: 3 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Dashboard Preview
                </Typography>
                <Box sx={{ p: 2, bgcolor: 'primary.main', borderRadius: 2, color: 'white' }}>
                  <Typography variant="h4">₹24.5L</Typography>
                  <Typography>Collection - 86%</Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Container>

      {/* Features Section */}
      <Container maxWidth="lg" sx={{ py: 8 }}>
        <Typography variant="h4" align="center" gutterBottom sx={{ fontWeight: 'bold' }}>
          Features
        </Typography>
        <Typography variant="body1" align="center" color="text.secondary" paragraph>
          Everything your society needs
        </Typography>
        <Grid container spacing={4} sx={{ mt: 4 }}>
          {features.map((feature, index) => (
            <Grid item xs={12} md={4} key={index}>
              <Card sx={{ height: '100%', textAlign: 'center', p: 3 }}>
                <CardContent>
                  <Avatar sx={{ mx: 'auto', mb: 2, bgcolor: 'primary.light', width: 80, height: 80 }}>
                    {feature.icon}
                  </Avatar>
                  <Typography variant="h6" gutterBottom>
                    {feature.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {feature.description}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Container>

      {/* Pricing Section */}
      <Container maxWidth="md" sx={{ py: 8 }}>
        <Card sx={{ p: 4, textAlign: 'center' }}>
          <CardContent>
            <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
              Society Management Plan
            </Typography>
            <Typography variant="body1" color="text.secondary" paragraph>
              Everything your society needs
            </Typography>
            <Box sx={{ textAlign: 'left', my: 3 }}>
              {[
                'Society Management',
                'Flat & Member Management',
                'Maintenance Billing',
                'Late Payment Rules',
                'Payments',
                'Receipts',
                'Accounting',
                'Audit Module',
                'Excel Import',
                'Excel Reports',
                'Email Notifications',
                'Web UI',
                'Mobile Card UI',
              ].map((feature, index) => (
                <Typography key={index} sx={{ py: 0.5 }}>
                  ✓ {feature}
                </Typography>
              ))}
            </Box>
            <Button
              variant="contained"
              size="large"
              onClick={() => navigate('/society/login')}
            >
              Get Started
            </Button>
          </CardContent>
        </Card>
      </Container>

      {/* Footer */}
      <Box sx={{ bgcolor: 'grey.900', color: 'white', py: 4 }}>
        <Container maxWidth="lg">
          <Grid container spacing={4}>
            <Grid item xs={12} md={4}>
              <Typography variant="h6" gutterBottom>
                SocietyPro
              </Typography>
              <Typography variant="body2" color="grey.400">
                Smart Society Management Made Simple
              </Typography>
            </Grid>
            <Grid item xs={12} md={4}>
              <Typography variant="h6" gutterBottom>
                Quick Links
              </Typography>
              <Link href="/society/login" color="inherit" underline="hover">
                Society Login
              </Link>
              <br />
              <Link href="/admin/login" color="inherit" underline="hover">
                Super Admin Login
              </Link>
            </Grid>
            <Grid item xs={12} md={4}>
              <Typography variant="h6" gutterBottom>
                Contact
              </Typography>
              <Typography variant="body2" color="grey.400">
                support@societypro.com
              </Typography>
            </Grid>
          </Grid>
          <Box sx={{ mt: 4, textAlign: 'center' }}>
            <Typography variant="body2" color="grey.400">
              © 2026 SocietyPro. All rights reserved.
            </Typography>
          </Box>
        </Container>
      </Box>
    </Box>
  );
};

export default Landing;
