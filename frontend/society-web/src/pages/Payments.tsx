import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  LinearProgress,
  Alert,
  Chip,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { paymentsApi } from '../services/api';

const Payments: React.FC = () => {
  const [payments, setPayments] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const fetchPayments = useCallback(async () => {
    setLoading(true);
    try {
      const data = await paymentsApi.list(1, 50);
      setPayments(data.items || data.payments || data || []);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchPayments(); }, [fetchPayments]);

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Payments</Typography>
        <Button variant="contained" startIcon={<Add />}>Record Payment</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Payment #</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Flat</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Mode</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {payments.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center"><Typography color="text.secondary">No payments found</Typography></TableCell></TableRow>
              ) : (
                payments.map((p: any) => (
                  <TableRow key={p.id}>
                    <TableCell>{p.paymentNumber || p.id}</TableCell>
                    <TableCell>{p.paymentDate ? new Date(p.paymentDate).toLocaleDateString() : '—'}</TableCell>
                    <TableCell>{p.flatNumber || p.flatId}</TableCell>
                    <TableCell>₹{p.amount?.toLocaleString() || '—'}</TableCell>
                    <TableCell>{p.paymentMode || '—'}</TableCell>
                    <TableCell>
                      <Chip
                        label={p.status || 'Completed'}
                        size="small"
                        color={p.status === 'Completed' ? 'success' : p.status === 'Reversed' ? 'error' : 'default'}
                      />
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Container>
  );
};

export default Payments;
