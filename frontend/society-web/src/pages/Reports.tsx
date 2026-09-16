import React from 'react';
import {
  Container,
  Typography,
  Box,
  AppBar,
  Toolbar,
  IconButton,
  Button,
  Grid,
  Card,
  CardContent,
} from '@mui/material';
import { Menu as MenuIcon, GetApp } from '@mui/icons-material';

const reports = [
  { title: 'Flat Register', category: 'Society' },
  { title: 'Member Register', category: 'Society' },
  { title: 'Bill Register', category: 'Billing' },
  { title: 'Payment Register', category: 'Collection' },
  { title: 'Outstanding Report', category: 'Outstanding' },
  { title: 'Audit Trail', category: 'Audit' },
];

const Reports: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro - Reports
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
            Reports
          </Typography>

          <Grid container spacing={3}>
            {reports.map((report, index) => (
              <Grid item xs={12} sm={6} md={4} key={index}>
                <Card>
                  <CardContent>
                    <Typography variant="h6" gutterBottom>
                      {report.title}
                    </Typography>
                    <Typography variant="body2" color="textSecondary" gutterBottom>
                      {report.category}
                    </Typography>
                    <Button
                      variant="outlined"
                      startIcon={<GetApp />}
                      size="small"
                    >
                      Download Excel
                    </Button>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Container>
      </Box>
    </Box>
  );
};

export default Reports;
