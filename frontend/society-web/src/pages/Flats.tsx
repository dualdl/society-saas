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
  LinearProgress,
  Alert,
  Chip,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { flatsApi } from '../services/api';

interface Flat {
  id: string;
  wing: string;
  flatNumber: string;
  floor: number;
  ownerName: string;
  status: string;
}

const Flats: React.FC = () => {
  const [flats, setFlats] = useState<Flat[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');
  const [page] = useState(1);

  const fetchFlats = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await flatsApi.list(page, 50, search);
      setFlats(data.flats || data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load flats');
    } finally {
      setLoading(false);
    }
  }, [page, search]);

  useEffect(() => { fetchFlats(); }, [fetchFlats]);

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Flats</Typography>
        <Button variant="contained" startIcon={<Add />}>Add Flat</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TextField
        fullWidth
        label="Search flats..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        size="small"
        sx={{ mb: 2 }}
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
                <TableCell>Floor</TableCell>
                <TableCell>Owner</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {flats.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center"><Typography color="text.secondary">No flats found</Typography></TableCell></TableRow>
              ) : (
                flats.map((flat) => (
                  <TableRow key={flat.id}>
                    <TableCell>{flat.wing}</TableCell>
                    <TableCell>{flat.flatNumber}</TableCell>
                    <TableCell>{flat.floor}</TableCell>
                    <TableCell>{flat.ownerName || '—'}</TableCell>
                    <TableCell>
                      <Chip label={flat.status || 'Active'} size="small" color={flat.status === 'Vacant' ? 'warning' : 'success'} />
                    </TableCell>
                    <TableCell>
                      <Button size="small">Edit</Button>
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

export default Flats;
