import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  TextField,
  Button,
  Box,
  Link,
  Alert,
  CircularProgress,
} from '@mui/material';

const AdminLogin: React.FC = () => {
  const navigate = useNavigate();
  const [identifier, setIdentifier] = useState('');
  const [otp, setOtp] = useState('');
  const [otpSent, setOtpSent] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSendOtp = async () => {
    setLoading(true);
    setError('');
    try {
      // TODO: Call API to send OTP
      await new Promise(resolve => setTimeout(resolve, 1000));
      setOtpSent(true);
    } catch (err) {
      setError('Failed to send OTP. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOtp = async () => {
    setLoading(true);
    setError('');
    try {
      // TODO: Call API to verify OTP
      await new Promise(resolve => setTimeout(resolve, 1000));
      navigate('/admin/dashboard');
    } catch (err) {
      setError('Invalid OTP. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ py: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h5" align="center" gutterBottom sx={{ fontWeight: 'bold' }}>
          Super Admin Login
        </Typography>
        <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
          Platform-level admin access
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {!otpSent ? (
          <Box>
            <TextField
              fullWidth
              label="Email / Mobile"
              value={identifier}
              onChange={(e) => setIdentifier(e.target.value)}
              margin="normal"
            />
            <Button
              fullWidth
              variant="contained"
              onClick={handleSendOtp}
              disabled={loading || !identifier}
              sx={{ mt: 2 }}
            >
              {loading ? <CircularProgress size={24} /> : 'Send OTP'}
            </Button>
          </Box>
        ) : (
          <Box>
            <TextField
              fullWidth
              label="Enter OTP"
              value={otp}
              onChange={(e) => setOtp(e.target.value)}
              margin="normal"
            />
            <Button
              fullWidth
              variant="contained"
              onClick={handleVerifyOtp}
              disabled={loading || !otp}
              sx={{ mt: 2 }}
            >
              {loading ? <CircularProgress size={24} /> : 'Verify OTP'}
            </Button>
            <Button
              fullWidth
              variant="text"
              onClick={() => setOtpSent(false)}
              sx={{ mt: 1 }}
            >
              Change Email/Mobile
            </Button>
          </Box>
        )}

        <Box sx={{ mt: 3, textAlign: 'center' }}>
          <Link href="/society/login" variant="body2">
            Society Login
          </Link>
        </Box>
        <Box sx={{ mt: 2, textAlign: 'center' }}>
          <Link href="/" variant="body2">
            Back to Home
          </Link>
        </Box>
      </Paper>
    </Container>
  );
};

export default AdminLogin;
