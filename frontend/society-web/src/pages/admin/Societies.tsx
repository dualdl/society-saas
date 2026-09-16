import React from 'react';
import {
  Container,
  Typography,
  Box,
  AppBar,
  Toolbar,
  IconButton,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
} from '@mui/material';
import { Menu as MenuIcon, Add } from '@mui/icons-material';

const AdminSocieties: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro - Societies
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
            <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
              Societies
            </Typography>
            <Button variant="contained" startIcon={<Add />}>
              Create Society
            </Button>
          </Box>

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>City</TableCell>
                  <TableCell>Flats</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                <TableRow>
                  <TableCell>Rajesh Apartments</TableCell>
                  <TableCell>Pune</TableCell>
                  <TableCell>120</TableCell>
                  <TableCell>Active</TableCell>
                  <TableCell>
                    <Button size="small">View</Button>
                    <Button size="small">Disable</Button>
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell>Green Valley Society</TableCell>
                  <TableCell>Mumbai</TableCell>
                  <TableCell>250</TableCell>
                  <TableCell>Active</TableCell>
                  <TableCell>
                    <Button size="small">View</Button>
                    <Button size="small">Disable</Button>
                  </TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </TableContainer>
        </Container>
      </Box>
    </Box>
  );
};

export default AdminSocieties;
