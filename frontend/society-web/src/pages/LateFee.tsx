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
  Switch,
  FormControlLabel,
  Divider,
} from '@mui/material';
import { Save } from '@mui/icons-material';
import { lateFeeApi } from '../services/api';

interface LateFeeConfig {
  enabled: boolean;
  graceDays: number;
  ratePercent: number;
  maxAmount: number;
  applyFromDay: number;
  calculateOn: string;
}

const LateFee: React.FC = () => {
  const [config, setConfig] = useState<LateFeeConfig>({
    enabled: false,
    graceDays: 0,
    ratePercent: 0,
    maxAmount: 0,
    applyFromDay: 1,
    calculateOn: 'outstanding',
  });
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const fetchConfig = useCallback(async () => {
    setLoading(true);
    try {
      const data = await lateFeeApi.get();
      if (data) setConfig(data);
    } catch (err: any) {
      setError(err.message || 'Failed to load late fee config');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchConfig(); }, [fetchConfig]);

  const handleSave = async () => {
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      await lateFeeApi.save(config);
      setSuccess('Late fee configuration saved');
    } catch (err: any) {
      setError(err.message || 'Save failed');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Container maxWidth="md">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
        Late Fee Configuration
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
                <FormControlLabel
                  control={
                    <Switch
                      checked={config.enabled}
                      onChange={(e) => setConfig({ ...config, enabled: e.target.checked })}
                    />
                  }
                  label="Enable Late Fees"
                />
              </Grid>

              <Grid item xs={12}>
                <Divider />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Grace Days"
                  type="number"
                  value={config.graceDays}
                  onChange={(e) => setConfig({ ...config, graceDays: parseInt(e.target.value) || 0 })}
                  helperText="Days after due date before late fee applies"
                  disabled={!config.enabled}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Apply From Day"
                  type="number"
                  value={config.applyFromDay}
                  onChange={(e) => setConfig({ ...config, applyFromDay: parseInt(e.target.value) || 1 })}
                  helperText="Day of month to start applying"
                  disabled={!config.enabled}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Rate (%)"
                  type="number"
                  value={config.ratePercent}
                  onChange={(e) => setConfig({ ...config, ratePercent: parseFloat(e.target.value) || 0 })}
                  helperText="Percentage per month on outstanding"
                  disabled={!config.enabled}
                />
              </Grid>

              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Max Amount (₹)"
                  type="number"
                  value={config.maxAmount}
                  onChange={(e) => setConfig({ ...config, maxAmount: parseFloat(e.target.value) || 0 })}
                  helperText="Maximum late fee cap (0 = no cap)"
                  disabled={!config.enabled}
                />
              </Grid>

              <Grid item xs={12}>
                <TextField
                  fullWidth
                  select
                  label="Calculate On"
                  value={config.calculateOn}
                  onChange={(e) => setConfig({ ...config, calculateOn: e.target.value })}
                  SelectProps={{ native: true }}
                  disabled={!config.enabled}
                >
                  <option value="outstanding">Outstanding Amount</option>
                  <option value="bill">Total Bill Amount</option>
                </TextField>
              </Grid>

              <Grid item xs={12}>
                <Button
                  variant="contained"
                  startIcon={<Save />}
                  onClick={handleSave}
                  disabled={saving}
                >
                  {saving ? 'Saving...' : 'Save Configuration'}
                </Button>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      )}
    </Container>
  );
};

export default LateFee;
