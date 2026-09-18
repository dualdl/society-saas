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
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Alert,
  LinearProgress,
  Grid,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { chargesApi } from '../services/api';

interface Charge {
  id: string;
  name: string;
  description: string;
  amount: number;
  frequency: string;
  isActive: boolean;
}

const Charges: React.FC = () => {
  const [charges, setCharges] = useState<Charge[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState({ name: '', description: '', amount: '', frequency: 'Monthly' });

  const fetchCharges = useCallback(async () => {
    setLoading(true);
    try {
      const data = await chargesApi.list();
      setCharges(data.items || data.charges || data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load charges');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchCharges(); }, [fetchCharges]);

  const handleSave = async () => {
    try {
      const payload = { ...form, amount: parseFloat(form.amount) };
      if (editingId) {
        await chargesApi.update(editingId, payload);
      } else {
        await chargesApi.create(payload);
      }
      setDialogOpen(false);
      setForm({ name: '', description: '', amount: '', frequency: 'Monthly' });
      setEditingId(null);
      fetchCharges();
    } catch (err: any) {
      setError(err.message || 'Save failed');
    }
  };

  const handleEdit = (charge: Charge) => {
    setEditingId(charge.id);
    setForm({
      name: charge.name,
      description: charge.description,
      amount: charge.amount.toString(),
      frequency: charge.frequency,
    });
    setDialogOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this charge?')) return;
    try {
      await chargesApi.delete(id);
      fetchCharges();
    } catch (err: any) {
      setError(err.message || 'Delete failed');
    }
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Charges Master</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => { setEditingId(null); setForm({ name: '', description: '', amount: '', frequency: 'Monthly' }); setDialogOpen(true); }}
        >
          Add Charge
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {loading && <LinearProgress sx={{ mb: 2 }} />}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Amount (₹)</TableCell>
              <TableCell>Frequency</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {charges.length === 0 ? (
              <TableRow><TableCell colSpan={6} align="center"><Typography color="text.secondary">No charges defined</Typography></TableCell></TableRow>
            ) : (
              charges.map((charge) => (
                <TableRow key={charge.id}>
                  <TableCell>{charge.name}</TableCell>
                  <TableCell>{charge.description}</TableCell>
                  <TableCell>₹{charge.amount.toLocaleString()}</TableCell>
                  <TableCell>{charge.frequency}</TableCell>
                  <TableCell>{charge.isActive ? 'Yes' : 'No'}</TableCell>
                  <TableCell>
                    <IconButton size="small" onClick={() => handleEdit(charge)}><Edit /></IconButton>
                    <IconButton size="small" onClick={() => handleDelete(charge.id)}><Delete /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editingId ? 'Edit Charge' : 'Add Charge'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}>
              <TextField fullWidth label="Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            </Grid>
            <Grid item xs={12}>
              <TextField fullWidth label="Description" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField fullWidth label="Amount" type="number" value={form.amount} onChange={(e) => setForm({ ...form, amount: e.target.value })} />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField fullWidth select label="Frequency" value={form.frequency} onChange={(e) => setForm({ ...form, frequency: e.target.value })}>
                <MenuItem value="Monthly">Monthly</MenuItem>
                <MenuItem value="Quarterly">Quarterly</MenuItem>
                <MenuItem value="Yearly">Yearly</MenuItem>
                <MenuItem value="OneTime">One Time</MenuItem>
              </TextField>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Charges;
