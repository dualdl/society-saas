import React, { useState, useEffect, useCallback, useRef } from 'react';
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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  IconButton,
  Tooltip,
} from '@mui/material';
import { Add, CloudDownload, Upload, Delete, Refresh } from '@mui/icons-material';
import { superAdminApi } from '../../services/api';

const AdminSocieties: React.FC = () => {
  const [societies, setSocieties] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [deleteDialog, setDeleteDialog] = useState<{ open: boolean; id: string; name: string }>({ open: false, id: '', name: '' });
  const [importing, setImporting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

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

  const handleDelete = async () => {
    try {
      await superAdminApi.deleteSociety(deleteDialog.id);
      setSuccess(`Society "${deleteDialog.name}" deleted successfully`);
      setDeleteDialog({ open: false, id: '', name: '' });
      fetchSocieties();
    } catch (err: any) {
      setError(err.message || 'Delete failed');
    }
  };

  const handleBackup = async (id: string, name: string) => {
    try {
      const blob = await superAdminApi.backupSociety(id);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `society-backup-${name.replace(/\s+/g, '-')}-${new Date().toISOString().split('T')[0]}.json`;
      a.click();
      window.URL.revokeObjectURL(url);
      setSuccess(`Backup downloaded for "${name}"`);
    } catch (err: any) {
      setError(err.message || 'Backup failed');
    }
  };

  const handleImport = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setImporting(true);
    setError('');
    try {
      const result = await superAdminApi.importSocieties(file);
      setSuccess(`Imported ${result.imported || 0} societies successfully`);
      fetchSocieties();
    } catch (err: any) {
      setError(err.message || 'Import failed');
    } finally {
      setImporting(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Societies</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <input
            type="file"
            ref={fileInputRef}
            accept=".json"
            style={{ display: 'none' }}
            onChange={handleImport}
          />
          <Button
            variant="outlined"
            startIcon={<Upload />}
            onClick={() => fileInputRef.current?.click()}
            disabled={importing}
          >
            {importing ? 'Importing...' : 'Import JSON'}
          </Button>
          <Button variant="contained" startIcon={<Add />}>
            Create Society
          </Button>
        </Box>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess('')}>{success}</Alert>}

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>City</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Created</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {societies.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center"><Typography color="text.secondary">No societies found</Typography></TableCell></TableRow>
              ) : (
                societies.map((s: any) => (
                  <TableRow key={s.id}>
                    <TableCell>{s.name}</TableCell>
                    <TableCell>{s.city || '—'}</TableCell>
                    <TableCell>{s.email || '—'}</TableCell>
                    <TableCell>
                      <Chip label={s.isActive ? 'Active' : 'Inactive'} size="small" color={s.isActive ? 'success' : 'default'} />
                    </TableCell>
                    <TableCell>{s.createdAt ? new Date(s.createdAt).toLocaleDateString() : '—'}</TableCell>
                    <TableCell align="right">
                      <Tooltip title="Download Backup">
                        <IconButton size="small" onClick={() => handleBackup(s.id, s.name)}>
                          <CloudDownload fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete Society">
                        <IconButton size="small" color="error" onClick={() => setDeleteDialog({ open: true, id: s.id, name: s.name })}>
                          <Delete fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={deleteDialog.open} onClose={() => setDeleteDialog({ open: false, id: '', name: '' })}>
        <DialogTitle>Delete Society</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete "{deleteDialog.name}"? This action cannot be undone. All data including flats, members, bills, and payments will be soft-deleted.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog({ open: false, id: '', name: '' })}>Cancel</Button>
          <Button onClick={handleDelete} color="error" variant="contained">Delete</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default AdminSocieties;
