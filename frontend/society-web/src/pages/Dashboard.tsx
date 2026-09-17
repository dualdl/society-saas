import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Grid,
  Card,
  CardContent,
  Typography,
  Box,
  Button,
  LinearProgress,
} from '@mui/material';
import {
  Home,
  Receipt,
  Payment,
  Assessment,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { dashboardApi } from '../services/api';

const Dashboard: React.FC = () => {
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const result = await dashboardApi.get();
      setData(result);
    } catch {
      // silent
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchData(); }, [fetchData]);

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
        Dashboard
      </Typography>

      {loading && <LinearProgress sx={{ mb: 2 }} />}

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>Total Flats</Typography>
              <Typography variant="h4">{data?.totalFlats ?? '—'}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>Total Members</Typography>
              <Typography variant="h4">{data?.totalMembers ?? '—'}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>Collection</Typography>
              <Typography variant="h4">
                {data?.totalCollected != null ? `₹${data.totalCollected.toLocaleString()}` : '—'}
              </Typography>
              {data?.collectionPercent != null && (
                <Typography variant="body2" color="primary">{data.collectionPercent}%</Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>Outstanding</Typography>
              <Typography variant="h4">
                {data?.totalOutstanding != null ? `₹${data.totalOutstanding.toLocaleString()}` : '—'}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
        Quick Actions
      </Typography>
      <Grid container spacing={2} sx={{ mb: 4 }}>
        <Grid item xs={6} sm={4} md={2}>
          <Button fullWidth variant="contained" startIcon={<Receipt />} sx={{ py: 2 }} onClick={() => navigate('/app/billing')}>
            Generate Bills
          </Button>
        </Grid>
        <Grid item xs={6} sm={4} md={2}>
          <Button fullWidth variant="contained" color="secondary" startIcon={<Payment />} sx={{ py: 2 }} onClick={() => navigate('/app/payments')}>
            Record Payment
          </Button>
        </Grid>
        <Grid item xs={6} sm={4} md={2}>
          <Button fullWidth variant="outlined" startIcon={<Home />} sx={{ py: 2 }} onClick={() => navigate('/app/imports')}>
            Import Excel
          </Button>
        </Grid>
        <Grid item xs={6} sm={4} md={2}>
          <Button fullWidth variant="outlined" startIcon={<Assessment />} sx={{ py: 2 }} onClick={() => navigate('/app/reports')}>
            Reports
          </Button>
        </Grid>
      </Grid>

      <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
        Recent Activity
      </Typography>
      <Card>
        <CardContent>
          {data?.recentActivity?.length ? (
            data.recentActivity.map((item: any, i: number) => (
              <Typography key={i} variant="body2" sx={{ py: 0.5 }}>
                {item.description || `${item.action} — ${item.entityType}`}
              </Typography>
            ))
          ) : (
            <Typography color="textSecondary">No recent activity to display</Typography>
          )}
        </CardContent>
      </Card>
    </Container>
  );
};

export default Dashboard;
