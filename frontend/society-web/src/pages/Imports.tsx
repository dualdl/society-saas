import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Alert,
  LinearProgress,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import { Upload, Download, Visibility, CheckCircle, Error as ErrorIcon, HourglassEmpty } from '@mui/icons-material';
import { importApi } from '../services/api';

interface ImportJob {
  id: string;
  entityType: string;
  fileName: string;
  status: string;
  totalRows: number;
  successRows: number;
  errorRows: number;
  createdAt: string;
  errors?: Array<{ row: number; message: string }>;
}

const Imports: React.FC = () => {
  const [jobs, setJobs] = useState<ImportJob[]>([]);
  const [loading, setLoading] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState('');
  const [previewOpen, setPreviewOpen] = useState(false);
  const [selectedJob, setSelectedJob] = useState<ImportJob | null>(null);
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const fetchJobs = useCallback(async () => {
    setLoading(true);
    try {
      const data = await importApi.getJobs();
      setJobs(data.jobs || data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load import history');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchJobs(); }, [fetchJobs]);

  const handleUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;
    setUploading(true);
    setError('');
    try {
      await importApi.upload(file);
      fetchJobs();
    } catch (err: any) {
      setError(err.message || 'Upload failed');
    } finally {
      setUploading(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleDownloadTemplate = async () => {
    try {
      const blob = await importApi.getTemplate();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'society_import_template.xlsx';
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      setError(err.message || 'Download failed');
    }
  };

  const handleConfirm = async (jobId: string) => {
    try {
      await importApi.confirm(jobId);
      fetchJobs();
    } catch (err: any) {
      setError(err.message || 'Confirm failed');
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed': return 'success';
      case 'processing': return 'warning';
      case 'failed': return 'error';
      default: return 'info';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed': return <CheckCircle fontSize="small" />;
      case 'processing': return <HourglassEmpty fontSize="small" />;
      case 'failed': return <ErrorIcon fontSize="small" />;
      default: return null;
    }
  };

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
        Data Import
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {uploading && <LinearProgress sx={{ mb: 2 }} />}

      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Upload Excel File</Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Upload flats, members, charges, or opening balances from an Excel file.
              </Typography>
              <input
                ref={fileInputRef}
                type="file"
                accept=".xlsx,.xls,.csv"
                style={{ display: 'none' }}
                onChange={handleUpload}
              />
              <Button
                variant="contained"
                startIcon={<Upload />}
                onClick={() => fileInputRef.current?.click()}
                disabled={uploading}
              >
                Upload File
              </Button>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Download Template</Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Download the Excel template with headers pre-filled for easy data entry.
              </Typography>
              <Button
                variant="outlined"
                startIcon={<Download />}
                onClick={handleDownloadTemplate}
              >
                Download Template
              </Button>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
        Import History
      </Typography>

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>File</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Total</TableCell>
                <TableCell>Success</TableCell>
                <TableCell>Errors</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {jobs.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={8} align="center">
                    <Typography color="text.secondary">No imports yet</Typography>
                  </TableCell>
                </TableRow>
              ) : (
                jobs.map((job) => (
                  <TableRow key={job.id}>
                    <TableCell>{job.fileName}</TableCell>
                    <TableCell>{job.entityType}</TableCell>
                    <TableCell>
                      <Chip
                        icon={getStatusIcon(job.status) || undefined}
                        label={job.status}
                        size="small"
                        color={getStatusColor(job.status) as any}
                      />
                    </TableCell>
                    <TableCell>{job.totalRows}</TableCell>
                    <TableCell>{job.successRows}</TableCell>
                    <TableCell>
                      {job.errorRows > 0 ? (
                        <Chip label={job.errorRows} size="small" color="error" variant="outlined" />
                      ) : '0'}
                    </TableCell>
                    <TableCell>{new Date(job.createdAt).toLocaleDateString()}</TableCell>
                    <TableCell>
                      {job.errorRows > 0 && (
                        <IconButton
                          size="small"
                          onClick={() => { setSelectedJob(job); setPreviewOpen(true); }}
                        >
                          <Visibility />
                        </IconButton>
                      )}
                      {job.status === 'completed' && (
                        <Button size="small" onClick={() => handleConfirm(job.id)}>
                          Confirm
                        </Button>
                      )}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={previewOpen} onClose={() => setPreviewOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>Import Errors — {selectedJob?.fileName}</DialogTitle>
        <DialogContent>
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Row</TableCell>
                  <TableCell>Error</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {selectedJob?.errors?.map((err, i) => (
                  <TableRow key={i}>
                    <TableCell>{err.row}</TableCell>
                    <TableCell>{err.message}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPreviewOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Imports;
