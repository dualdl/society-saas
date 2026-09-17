import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Typography,
  Box,
  Grid,
  Card,
  CardContent,
  Alert,
  LinearProgress,
} from '@mui/material';
import {
  Home,
  Receipt,
  Payment,
  AccountBalance,
} from '@mui/icons-material';
import MobileCard from '../components/MobileCard';
import BottomNav from '../components/BottomNav';
import { dashboardApi } from '../services/api';

const ResidentDashboard: React.FC = () => {
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const result = await dashboardApi.get();
      setData(result);
    } catch (err: any) {
      setError(err.message || 'Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  return (
    <Box sx={{ pb: 8 }}>
      <Container maxWidth="sm" sx={{ py: 2 }}>
        <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
          My Dashboard
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        {loading && <LinearProgress sx={{ mb: 2 }} />}

        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid item xs={6}>
            <Card sx={{ textAlign: 'center', py: 2 }}>
              <CardContent>
                <AccountBalance sx={{ fontSize: 32, color: 'primary.main' }} />
                <Typography variant="h6">{data?.outstandingAmount ? `₹${data.outstandingAmount.toLocaleString()}` : '₹0'}</Typography>
                <Typography variant="body2" color="text.secondary">Outstanding</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={6}>
            <Card sx={{ textAlign: 'center', py: 2 }}>
              <CardContent>
                <Receipt sx={{ fontSize: 32, color: 'success.main' }} />
                <Typography variant="h6">{data?.lastPaymentDate ? new Date(data.lastPaymentDate).toLocaleDateString() : '—'}</Typography>
                <Typography variant="body2" color="text.secondary">Last Payment</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        <MobileCard
          title="My Flat"
          subtitle={data?.flatNumber || 'N/A'}
          icon={<Home />}
        />
        <MobileCard
          title="Current Bill"
          primaryValue={data?.currentBillAmount ? `₹${data.currentBillAmount.toLocaleString()}` : '₹0'}
          secondaryValue={data?.billingPeriod || ''}
          icon={<Receipt />}
          status={data?.billStatus || 'No Bill'}
          statusColor={data?.billStatus === 'Paid' ? 'success' : 'warning'}
        />
        <MobileCard
          title="Recent Payment"
          primaryValue={data?.lastPaymentAmount ? `₹${data.lastPaymentAmount.toLocaleString()}` : '₹0'}
          secondaryValue={data?.lastPaymentMode || ''}
          icon={<Payment />}
          status={data?.lastPaymentStatus || 'None'}
          statusColor={data?.lastPaymentStatus === 'Completed' ? 'success' : 'info'}
        />
      </Container>
      <BottomNav />
    </Box>
  );
};

export default ResidentDashboard;
