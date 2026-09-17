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
  TextField,
  Alert,
  LinearProgress,
  Grid,
  Card,
  CardContent,
} from '@mui/material';
import { Save } from '@mui/icons-material';
import { openingBalanceApi } from '../services/api';

interface OpeningBalance {
  flatId: string;
  flatNumber: string;
  wing: string;
  balance: number;
}

const OpeningBalances: React.FC = () => {
  const [balances, setBalances] = useState<OpeningBalance[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [search, setSearch] = useState('');

  const fetchBalances = useCallback(async () => {
    setLoading(true);
    try {
      const data = await openingBalanceApi.getAll();
      setBalances(data.balances || data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load opening balances');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchBalances(); }, [fetchBalances]);

  const handleAmountChange = (flatId: string, value: string) => {
    setBalances((prev) =>
      prev.map((b) => (b.flatId === flatId ? { ...b, balance: parseFloat(value) || 0 } : b))
    );
  };

  const handleSave = async () => {
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      await openingBalanceApi.set(balances);
      setSuccess('Opening balances saved successfully');
      fetchBalances();
    } catch (err: any) {
      setError(err.message || 'Failed to save');
    } finally {
      setSaving(false);
    }
  };

  const filtered = balances.filter(
    (b) =>
      b.flatNumber.toLowerCase().includes(search.toLowerCase()) ||
      b.wing.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3, alignItems: 'center' }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Opening Balances</Typography>
        <Button variant="contained" startIcon={<Save />} onClick={handleSave} disabled={saving}>
          {saving ? 'Saving...' : 'Save All'}
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess('')}>{success}</Alert>}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="body2" color="text.secondary">
            Set opening balances for each flat at the start of a new financial year. Positive values represent amount owed by the flat; negative values represent credit.
          </Typography>
        </CardContent>
      </Card>

      <TextField
        fullWidth
        label="Search flats..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        sx={{ mb: 2 }}
        size="small"
      />

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Wing</TableCell>
                <TableCell>Flat Number</TableCell>
                <TableCell>Opening Balance (₹)</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filtered.length === 0 ? (
                <TableRow><TableCell colSpan={3} align="center"><Typography color="text.secondary">No flats found</Typography></TableCell></TableRow>
              ) : (
                filtered.map((b) => (
                  <TableRow key={b.flatId}>
                    <TableCell>{b.wing}</TableCell>
                    <TableCell>{b.flatNumber}</TableCell>
                    <TableCell>
                      <TextField
                        type="number"
                        size="small"
                        value={b.balance}
                        onChange={(e) => handleAmountChange(b.flatId, e.target.value)}
                        sx={{ width: 150 }}
                        inputProps={{ step: 0.01 }}
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

export default OpeningBalances;
