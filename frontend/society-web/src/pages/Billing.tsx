import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
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
import { PlayArrow, History } from '@mui/icons-material';
import { billsApi } from '../services/api';

const Billing: React.FC = () => {
  const [bills, setBills] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const fetchBills = useCallback(async () => {
    setLoading(true);
    try {
      const data = await billsApi.list(1, 20);
      setBills(data.bills || data || []);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchBills(); }, [fetchBills]);

  const handleGenerate = async () => {
    setGenerating(true);
    setError('');
    setSuccess('');
    try {
      await billsApi.bulkGenerate({});
      setSuccess('Bills generated successfully');
      fetchBills();
    } catch (err: any) {
      setError(err.message || 'Generation failed');
    } finally {
      setGenerating(false);
    }
  };

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>Billing</Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess('')}>{success}</Alert>}

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Generate Bills</Typography>
              <Typography color="textSecondary" paragraph>
                Generate maintenance bills for all flats for the current billing period.
              </Typography>
              <Button variant="contained" startIcon={<PlayArrow />} onClick={handleGenerate} disabled={generating}>
                {generating ? 'Generating...' : 'Generate Bills'}
              </Button>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Billing History</Typography>
              <Typography color="textSecondary" paragraph>
                View and manage previous billing runs.
              </Typography>
              <Button variant="outlined" startIcon={<History />}>View History</Button>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {loading ? (
        <LinearProgress />
      ) : bills.length > 0 && (
        <>
          <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>Recent Bills</Typography>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Bill #</TableCell>
                  <TableCell>Flat</TableCell>
                  <TableCell>Amount</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Period</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {bills.map((bill: any) => (
                  <TableRow key={bill.id}>
                    <TableCell>{bill.billNumber || bill.id}</TableCell>
                    <TableCell>{bill.flatNumber || bill.flatId}</TableCell>
                    <TableCell>₹{bill.totalAmount?.toLocaleString() || '—'}</TableCell>
                    <TableCell>
                      <Chip
                        label={bill.status || 'Pending'}
                        size="small"
                        color={bill.status === 'Paid' ? 'success' : bill.status === 'Partial' ? 'warning' : 'default'}
                      />
                    </TableCell>
                    <TableCell>{bill.billingPeriod || '—'}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </>
      )}
    </Container>
  );
};

export default Billing;
