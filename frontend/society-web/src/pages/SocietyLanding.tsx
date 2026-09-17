import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Grid,
  Avatar,
  LinearProgress,
} from '@mui/material';
import {
  AccountBalance,
  Payment,
  Receipt,
  PhoneAndroid,
} from '@mui/icons-material';

const API_BASE = process.env.REACT_APP_API_URL || 'https://society-saas-api.azurewebsites.net';

const SocietyLanding: React.FC = () => {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();
  const [society, setSociety] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchSociety = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/v1/tenants?slug=${slug}`);
        if (res.ok) {
          const data = await res.json();
          setSociety(data);
        } else {
          setError('Society not found');
        }
      } catch {
        setError('Failed to load society');
      } finally {
        setLoading(false);
      }
    };
    if (slug) fetchSociety();
  }, [slug]);

  if (loading) return <LinearProgress />;
  if (error) {
    return (
      <Container maxWidth="sm" sx={{ py: 8, textAlign: 'center' }}>
        <Typography variant="h5" gutterBottom>{error}</Typography>
        <Button variant="contained" onClick={() => navigate('/')}>Go Home</Button>
      </Container>
    );
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.50' }}>
      <Box sx={{ bgcolor: 'primary.main', color: 'white', py: 3 }}>
        <Container maxWidth="lg">
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            {society?.logoUrl ? (
              <Avatar src={society.logoUrl} sx={{ width: 56, height: 56 }} />
            ) : (
              <Avatar sx={{ width: 56, height: 56, bgcolor: 'primary.dark' }}>
                <AccountBalance />
              </Avatar>
            )}
            <Box>
              <Typography variant="h5" sx={{ fontWeight: 'bold' }}>{society?.name || 'Society'}</Typography>
              <Typography variant="body2" sx={{ opacity: 0.9 }}>
                {[society?.city, society?.state].filter(Boolean).join(', ')}
              </Typography>
            </Box>
          </Box>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ py: 6 }}>
        <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold', textAlign: 'center', mb: 4 }}>
          Welcome to {society?.name}
        </Typography>

        <Grid container spacing={3} sx={{ mb: 6 }}>
          {[
            { icon: <AccountBalance sx={{ fontSize: 40 }} />, title: 'Billing', desc: 'View and pay your maintenance bills' },
            { icon: <Payment sx={{ fontSize: 40 }} />, title: 'Payments', desc: 'Track your payment history' },
            { icon: <Receipt sx={{ fontSize: 40 }} />, title: 'Receipts', desc: 'Download payment receipts' },
            { icon: <PhoneAndroid sx={{ fontSize: 40 }} />, title: 'Mobile', desc: 'Manage from your phone' },
          ].map((item, i) => (
            <Grid item xs={12} sm={6} md={3} key={i}>
              <Card sx={{ textAlign: 'center', p: 3, height: '100%' }}>
                <CardContent>
                  <Avatar sx={{ mx: 'auto', mb: 2, bgcolor: 'primary.light', width: 64, height: 64 }}>
                    {item.icon}
                  </Avatar>
                  <Typography variant="h6" gutterBottom>{item.title}</Typography>
                  <Typography variant="body2" color="text.secondary">{item.desc}</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>

        <Box sx={{ textAlign: 'center', display: 'flex', gap: 2, justifyContent: 'center' }}>
          <Button variant="contained" size="large" onClick={() => navigate('/society/login')}>
            Member Login
          </Button>
          <Button variant="outlined" size="large" onClick={() => navigate('/admin/login')}>
            Admin Login
          </Button>
        </Box>
      </Container>

      <Box sx={{ bgcolor: 'grey.900', color: 'white', py: 3, mt: 6 }}>
        <Container maxWidth="lg">
          <Typography variant="body2" align="center" sx={{ opacity: 0.7 }}>
            © {new Date().getFullYear()} {society?.name}. Powered by SocietyPro
          </Typography>
        </Container>
      </Box>
    </Box>
  );
};

export default SocietyLanding;
