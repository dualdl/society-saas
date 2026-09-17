import React from 'react';
import {
  Container,
  Typography,
  Box,
  Button,
  Grid,
  Card,
  CardContent,
} from '@mui/material';
import { GetApp } from '@mui/icons-material';
import { reportApi } from '../services/api';

const reports = [
  { title: 'Flat Register', category: 'Society', type: 'flat-register' },
  { title: 'Member Register', category: 'Society', type: 'member-register' },
  { title: 'Bill Register', category: 'Billing', type: 'bill-register' },
  { title: 'Payment Register', category: 'Collection', type: 'payment-register' },
  { title: 'Outstanding Report', category: 'Outstanding', type: 'outstanding' },
  { title: 'Audit Trail', category: 'Audit', type: 'audit-trail' },
];

const Reports: React.FC = () => {
  const handleDownload = async (reportType: string) => {
    try {
      const blob = await reportApi.downloadExcel(reportType);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${reportType}.xlsx`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      alert(err.message || 'Download failed');
    }
  };

  return (
    <Container maxWidth="lg">
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
        Reports
      </Typography>

      <Grid container spacing={3}>
        {reports.map((report, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>{report.title}</Typography>
                <Typography variant="body2" color="textSecondary" gutterBottom>
                  {report.category}
                </Typography>
                <Button
                  variant="outlined"
                  startIcon={<GetApp />}
                  size="small"
                  onClick={() => handleDownload(report.type)}
                >
                  Download Excel
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
};

export default Reports;
