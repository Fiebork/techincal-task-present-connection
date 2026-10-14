import { useEffect, useState } from 'react';
import { Paper, Stack, Table, TableBody, TableCell, TableHead, TableRow, Typography } from '@mui/material';
import { api } from '../api';
import type { User } from '../types';
import { ErrorState, LoadingState } from '../components/PageState';

export function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [error, setError] = useState('');
  useEffect(() => { api.getUsers().then(setUsers).catch((reason: Error) => setError(reason.message)); }, []);
  return <Stack spacing={3}><Typography variant="h4" fontWeight={700}>Users</Typography>{error ? <ErrorState message={error} /> : users.length === 0 ? <LoadingState /> : <Paper variant="outlined"><Table><TableHead><TableRow><TableCell>ID</TableCell><TableCell>Name</TableCell><TableCell>Active reservations</TableCell></TableRow></TableHead><TableBody>{users.map((user) => <TableRow key={user.id}><TableCell>{user.id}</TableCell><TableCell>{user.firstName} {user.lastName}</TableCell><TableCell>{user.activeReservationCount}</TableCell></TableRow>)}</TableBody></Table></Paper>}</Stack>;
}
