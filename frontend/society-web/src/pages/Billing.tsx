import React from 'react';
import {
  Container,
  Typography,
  Box,
  AppBar,
  Toolbar,
  IconButton,
  Button,
  Card,
  CardContent,
  Grid,
} from '@mui/material';
import { Menu as MenuIcon, PlayArrow } from '@mui/icons-material';

const Billing: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro - Billing
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
            Billing
          </Typography>

          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Generate Bills
                  </Typography>
                  <Typography color="textSecondary" paragraph>
                    Generate maintenance bills for all flats for the current billing period.
                  </Typography>
                  <Button
                    variant="contained"
                    startIcon={<PlayArrow />}
                  >
                    Generate Bills
                  </Button>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} md={6}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Billing History
                  </Typography>
                  <Typography color="textSecondary" paragraph>
                    View and manage previous billing runs.
                  </Typography>
                  <Button variant="outlined">
                    View History
                  </Button>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
        </Container>
      </Box>
    </Box>
  );
};

export default Billing;
