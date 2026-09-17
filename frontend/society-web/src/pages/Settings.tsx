import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  Grid,
  TextField,
  Button,
  Alert,
  LinearProgress,
  Divider,
  Switch,
  FormControlLabel,
} from '@mui/material';
import { Save } from '@mui/icons-material';

const Settings: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [form, setForm] = useState({
    societyName: localStorage.getItem('tenantName') || '',
    email: '',
    phone: '',
    address: '',
    city: '',
    pincode: '',
    financialYearStart: '04',
    enableEmailNotifications: true,
    enableSmsNotifications: false,
  });

  const fetchSettings = useCallback(async () => {
    setLoading(true);
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${process.env.REACT_APP_API_URL || 'https://society-saas-api.azurewebsites.net'}/api/v1/settings`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.ok) {
        const data = await res.json();
        setForm((prev) => ({ ...prev, ...data }));
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load settings');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchSettings(); }, [fetchSettings]);

  const handleSave = async () => {
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      const token = localStorage.getItem('token');
      const res = await fetch(`${process.env.REACT_APP_API_URL || 'https://society-saas-api.azurewebsites.net'}/api/v1/settings`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify(form),
      });
      if (!res.ok) throw new Error('Save failed');
      setSuccess('Settings saved successfully');
    } catch (err: any) {
      setError(err.message || 'Save failed');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Container maxWidth="md">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
        Society Settings
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess('')}>{success}</Alert>}

      {loading ? (
        <LinearProgress />
      ) : (
        <Card>
          <CardContent>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>Society Information</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Society Name" value={form.societyName} onChange={(e) => setForm({ ...form, societyName: e.target.value })} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Phone" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
              </Grid>
              <Grid item xs={12}>
                <TextField fullWidth label="Address" value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="City" value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField fullWidth label="Pincode" value={form.pincode} onChange={(e) => setForm({ ...form, pincode: e.target.value })} />
              </Grid>

              <Grid item xs={12}><Divider /></Grid>

              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>Billing</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  select
                  label="Financial Year Start Month"
                  value={form.financialYearStart}
                  onChange={(e) => setForm({ ...form, financialYearStart: e.target.value })}
                  SelectProps={{ native: true }}
                >
                  <option value="01">January</option>
                  <option value="04">April</option>
                  <option value="07">July</option>
                  <option value="10">October</option>
                </TextField>
              </Grid>

              <Grid item xs={12}><Divider /></Grid>

              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>Notifications</Typography>
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={<Switch checked={form.enableEmailNotifications} onChange={(e) => setForm({ ...form, enableEmailNotifications: e.target.checked })} />}
                  label="Enable Email Notifications"
                />
              </Grid>
              <Grid item xs={12}>
                <FormControlLabel
                  control={<Switch checked={form.enableSmsNotifications} onChange={(e) => setForm({ ...form, enableSmsNotifications: e.target.checked })} />}
                  label="Enable SMS Notifications"
                />
              </Grid>

              <Grid item xs={12}>
                <Button variant="contained" startIcon={<Save />} onClick={handleSave} disabled={saving}>
                  {saving ? 'Saving...' : 'Save Settings'}
                </Button>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      )}
    </Container>
  );
};

export default Settings;
