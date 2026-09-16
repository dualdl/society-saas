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
import { Menu as MenuIcon, GetApp } from '@mui/icons-material';

const Receipts: React.FC = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton edge="start" color="inherit" sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            SocietyPro - Receipts
          </Typography>
          <Button color="inherit">Logout</Button>
        </Toolbar>
      </AppBar>

      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
            Receipts
          </Typography>

          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Receipt #</TableCell>
                  <TableCell>Date</TableCell>
                  <TableCell>Flat</TableCell>
                  <TableCell>Amount</TableCell>
                  <TableCell>Mode</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                <TableRow>
                  <TableCell>RCT/26-27/000125</TableCell>
                  <TableCell>10-Sep-2026</TableCell>
                  <TableCell>A-101</TableCell>
                  <TableCell>₹5,250</TableCell>
                  <TableCell>UPI</TableCell>
                  <TableCell>
                    <Button size="small" startIcon={<GetApp />}>
                      PDF
                    </Button>
                    <Button size="small">Email</Button>
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

export default Receipts;
