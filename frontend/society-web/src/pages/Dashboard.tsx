import React from 'react';
import {
  Container,
  Grid,
  Card,
  CardContent,
  Typography,
  Box,
  Button,
  AppBar,
  Toolbar,
  IconButton,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Home,
  Receipt,
  Payment,
  Assessment,
  Menu as MenuIcon,
} from '@mui/icons-material';

const Dashboard: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      {/* App Bar */}
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      {/* Main Content */}
      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
            Dashboard
          </Typography>

          {/* Stats Cards */}
          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid item xs={12} sm={6} md={3}>
              <Card>
                <CardContent>
                  <Typography color="textSecondary" gutterBottom>
                    Total Flats
                  </Typography>
                  <Typography variant="h4">480</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <Card>
                <CardContent>
                  <Typography color="textSecondary" gutterBottom>
                    Total Members
                  </Typography>
                  <Typography variant="h4">725</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <Card>
                <CardContent>
                  <Typography color="textSecondary" gutterBottom>
                    Collection
                  </Typography>
                  <Typography variant="h4">₹21.20L</Typography>
                  <Typography variant="body2" color="primary">
                    86%
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <Card>
                <CardContent>
                  <Typography color="textSecondary" gutterBottom>
                    Outstanding
                  </Typography>
                  <Typography variant="h4">₹3.30L</Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Quick Actions */}
          <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
            Quick Actions
          </Typography>
          <Grid container spacing={2} sx={{ mb: 4 }}>
            <Grid item xs={6} sm={4} md={2}>
              <Button
                fullWidth
                variant="contained"
                startIcon={<Receipt />}
                sx={{ py: 2 }}
              >
                Generate Bills
              </Button>
            </Grid>
            <Grid item xs={6} sm={4} md={2}>
              <Button
                fullWidth
                variant="contained"
                color="secondary"
                startIcon={<Payment />}
                sx={{ py: 2 }}
              >
                Record Payment
              </Button>
            </Grid>
            <Grid item xs={6} sm={4} md={2}>
              <Button
                fullWidth
                variant="outlined"
                startIcon={<Home />}
                sx={{ py: 2 }}
              >
                Import Excel
              </Button>
            </Grid>
            <Grid item xs={6} sm={4} md={2}>
              <Button
                fullWidth
                variant="outlined"
                startIcon={<Assessment />}
                sx={{ py: 2 }}
              >
                Reports
              </Button>
            </Grid>
          </Grid>

          {/* Recent Activity */}
          <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
            Recent Activity
          </Typography>
          <Card>
            <CardContent>
              <Typography color="textSecondary">
                No recent activity to display
              </Typography>
            </CardContent>
          </Card>
        </Container>
      </Box>
    </Box>
  );
};

export default Dashboard;
