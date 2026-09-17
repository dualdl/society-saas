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
import { superAdminApi } from '../../services/api';

const AdminSocieties: React.FC = () => {
  const [societies, setSocieties] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const fetchSocieties = useCallback(async () => {
    setLoading(true);
    try {
      const data = await superAdminApi.societies();
      setSocieties(data.societies || data || []);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchSocieties(); }, [fetchSocieties]);

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Societies</Typography>
        <Button variant="contained" startIcon={<Add />}>Create Society</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>City</TableCell>
                <TableCell>Flats</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {societies.length === 0 ? (
                <TableRow><TableCell colSpan={5} align="center"><Typography color="text.secondary">No societies found</Typography></TableCell></TableRow>
              ) : (
                societies.map((s: any) => (
                  <TableRow key={s.id}>
                    <TableCell>{s.name}</TableCell>
                    <TableCell>{s.city || '—'}</TableCell>
                    <TableCell>{s.flatCount ?? '—'}</TableCell>
                    <TableCell>
                      <Chip label={s.status || 'Active'} size="small" color={s.status === 'Active' ? 'success' : 'default'} />
                    </TableCell>
                    <TableCell>
                      <Button size="small">View</Button>
                      <Button size="small">Disable</Button>
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

export default AdminSocieties;
