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

const Members: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro - Members
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
            <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
              Members
            </Typography>
            <Button variant="contained" startIcon={<Add />}>
              Add Member
            </Button>
          </Box>

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Flat</TableCell>
                  <TableCell>Name</TableCell>
                  <TableCell>Mobile</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Type</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                <TableRow>
                  <TableCell>A-101</TableCell>
                  <TableCell>Rajesh Patil</TableCell>
                  <TableCell>98XXXX</TableCell>
                  <TableCell>r@email.com</TableCell>
                  <TableCell>Owner</TableCell>
                  <TableCell>
                    <Button size="small">Edit</Button>
                  </TableCell>
                </TableRow>
                <TableRow>
                  <TableCell>A-102</TableCell>
                  <TableCell>Amit Shah</TableCell>
                  <TableCell>97XXXX</TableCell>
                  <TableCell>a@email.com</TableCell>
                  <TableCell>Tenant</TableCell>
                  <TableCell>
                    <Button size="small">Edit</Button>
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

export default Members;
