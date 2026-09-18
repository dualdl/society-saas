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
import { membersApi } from '../services/api';

interface Member {
  id: string;
  firstName: string;
  lastName: string | null;
  mobile: string;
  email: string | null;
  memberType: string;
  isPrimary: boolean;
  isActive: boolean;
  flatId: string;
  flatNumber: string | null;
}

const Members: React.FC = () => {
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');

  const fetchMembers = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const data = await membersApi.list(1, 50, search);
      setMembers(data.items || data.members || data || []);
    } catch (err: any) {
      setError(err.message || 'Failed to load members');
    } finally {
      setLoading(false);
    }
  }, [search]);

  useEffect(() => { fetchMembers(); }, [fetchMembers]);

  return (
    <Container maxWidth="lg">
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>Members</Typography>
        <Button variant="contained" startIcon={<Add />}>Add Member</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <TextField
        fullWidth
        label="Search members..."
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
                <TableCell>Flat</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Mobile</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Primary</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {members.length === 0 ? (
                <TableRow><TableCell colSpan={7} align="center"><Typography color="text.secondary">No members found</Typography></TableCell></TableRow>
              ) : (
                members.map((member) => (
                  <TableRow key={member.id}>
                    <TableCell>{member.flatNumber || '—'}</TableCell>
                    <TableCell>{member.firstName} {member.lastName || ''}</TableCell>
                    <TableCell>{member.mobile || '—'}</TableCell>
                    <TableCell>{member.email || '—'}</TableCell>
                    <TableCell>
                      <Chip label={member.memberType || 'Owner'} size="small" />
                    </TableCell>
                    <TableCell>
                      <Chip label={member.isPrimary ? 'Yes' : 'No'} size="small" color={member.isPrimary ? 'primary' : 'default'} />
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

export default Members;
