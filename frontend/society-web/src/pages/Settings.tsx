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
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import { Save, Email } from '@mui/icons-material';
import { settingsApi } from '../services/api';

const Settings: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [form, setForm] = useState({
    name: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    state: '',
    pincode: '',
    logoUrl: '',
    emailProvider: 'smtp',
    smtpHost: '',
    smtpPort: 587,
    smtpUser: '',
    smtpPassword: '',
    smtpUseSsl: true,
    gmailAddress: '',
    gmailAppPassword: '',
  });

  const fetchSettings = useCallback(async () => {
    setLoading(true);
    try {
      const data = await settingsApi.get();
      setForm({
        name: data.name || '',
        email: data.email || '',
        phone: data.phone || '',
        address: data.address || '',
        city: data.city || '',
        state: data.state || '',
        pincode: data.pinCode || '',
        logoUrl: data.logoUrl || '',
        emailProvider: data.emailProvider || 'smtp',
        smtpHost: data.smtpHost || '',
        smtpPort: data.smtpPort || 587,
        smtpUser: data.smtpUser || '',
        smtpPassword: '',
        smtpUseSsl: data.smtpUseSsl ?? true,
        gmailAddress: data.gmailAddress || '',
        gmailAppPassword: '',
      });
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
      await settingsApi.update(form);
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
        <>
          <Card sx={{ mb: 3 }}>
            <CardContent>
              <Grid container spacing={3}>
                <Grid item xs={12}>
                  <Typography variant="h6" gutterBottom>Society Information</Typography>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField fullWidth label="Society Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
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
                <Grid item xs={12} sm={4}>
                  <TextField fullWidth label="City" value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} />
                </Grid>
                <Grid item xs={12} sm={4}>
                  <TextField fullWidth label="State" value={form.state} onChange={(e) => setForm({ ...form, state: e.target.value })} />
                </Grid>
                <Grid item xs={12} sm={4}>
                  <TextField fullWidth label="Pincode" value={form.pincode} onChange={(e) => setForm({ ...form, pincode: e.target.value })} />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField fullWidth label="Logo URL" value={form.logoUrl} onChange={(e) => setForm({ ...form, logoUrl: e.target.value })} />
                </Grid>
              </Grid>
            </CardContent>
          </Card>

          <Card sx={{ mb: 3 }}>
            <CardContent>
              <Grid container spacing={3}>
                <Grid item xs={12}>
                  <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Email /> Email Configuration
                  </Typography>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <FormControl fullWidth>
                    <InputLabel>Email Provider</InputLabel>
                    <Select
                      value={form.emailProvider}
                      label="Email Provider"
                      onChange={(e) => setForm({ ...form, emailProvider: e.target.value })}
                    >
                      <MenuItem value="smtp">Custom SMTP</MenuItem>
                      <MenuItem value="gmail">Gmail</MenuItem>
                      <MenuItem value="outlook">Outlook</MenuItem>
                    </Select>
                  </FormControl>
                </Grid>

                {form.emailProvider === 'smtp' && (
                  <>
                    <Grid item xs={12} sm={8}>
                      <TextField fullWidth label="SMTP Host" value={form.smtpHost} onChange={(e) => setForm({ ...form, smtpHost: e.target.value })} placeholder="smtp.gmail.com" />
                    </Grid>
                    <Grid item xs={12} sm={4}>
                      <TextField fullWidth label="SMTP Port" type="number" value={form.smtpPort} onChange={(e) => setForm({ ...form, smtpPort: parseInt(e.target.value) || 587 })} />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="SMTP Username" value={form.smtpUser} onChange={(e) => setForm({ ...form, smtpUser: e.target.value })} />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="SMTP Password" type="password" value={form.smtpPassword} onChange={(e) => setForm({ ...form, smtpPassword: e.target.value })} placeholder="Leave blank to keep existing" />
                    </Grid>
                    <Grid item xs={12}>
                      <FormControlLabel
                        control={<Switch checked={form.smtpUseSsl} onChange={(e) => setForm({ ...form, smtpUseSsl: e.target.checked })} />}
                        label="Use SSL/TLS"
                      />
                    </Grid>
                  </>
                )}

                {form.emailProvider === 'gmail' && (
                  <>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Gmail Address" value={form.gmailAddress} onChange={(e) => setForm({ ...form, gmailAddress: e.target.value })} placeholder="your@gmail.com" />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Gmail App Password" type="password" value={form.gmailAppPassword} onChange={(e) => setForm({ ...form, gmailAppPassword: e.target.value })} placeholder="Leave blank to keep existing" />
                    </Grid>
                  </>
                )}

                {form.emailProvider === 'outlook' && (
                  <>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Outlook Email" value={form.smtpUser} onChange={(e) => setForm({ ...form, smtpUser: e.target.value })} placeholder="your@outlook.com" />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField fullWidth label="Outlook Password" type="password" value={form.smtpPassword} onChange={(e) => setForm({ ...form, smtpPassword: e.target.value })} placeholder="Leave blank to keep existing" />
                    </Grid>
                  </>
                )}
              </Grid>
            </CardContent>
          </Card>

          <Button variant="contained" startIcon={<Save />} onClick={handleSave} disabled={saving} size="large">
            {saving ? 'Saving...' : 'Save Settings'}
          </Button>
        </>
      )}
    </Container>
  );
};

export default Settings;
