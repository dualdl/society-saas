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
} from '@mui/material';
import { GetApp } from '@mui/icons-material';
import { receiptsApi } from '../services/api';

const Receipts: React.FC = () => {
  const [receipts, setReceipts] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const fetchReceipts = useCallback(async () => {
    setLoading(true);
    try {
      const data = await receiptsApi.list(1, 50);
      setReceipts(data.receipts || data || []);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchReceipts(); }, [fetchReceipts]);

  const handleDownloadPdf = async (id: string) => {
    try {
      const blob = await receiptsApi.getPdf(id);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `receipt_${id}.pdf`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      setError(err.message || 'Download failed');
    }
  };

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>Receipts</Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {loading ? (
        <LinearProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Receipt #</TableCell>
                <TableCell>Date</TableCell>
                <TableCell>Flat</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Mode</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {receipts.length === 0 ? (
                <TableRow><TableCell colSpan={6} align="center"><Typography color="text.secondary">No receipts found</Typography></TableCell></TableRow>
              ) : (
                receipts.map((r: any) => (
                  <TableRow key={r.id}>
                    <TableCell>{r.receiptNumber || r.id}</TableCell>
                    <TableCell>{r.receiptDate ? new Date(r.receiptDate).toLocaleDateString() : '—'}</TableCell>
                    <TableCell>{r.flatNumber || r.flatId}</TableCell>
                    <TableCell>₹{r.amount?.toLocaleString() || '—'}</TableCell>
                    <TableCell>{r.paymentMode || '—'}</TableCell>
                    <TableCell>
                      <Button size="small" startIcon={<GetApp />} onClick={() => handleDownloadPdf(r.id)}>
                        PDF
                      </Button>
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

export default Receipts;
