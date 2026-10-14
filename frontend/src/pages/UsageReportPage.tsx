import { useEffect, useState } from 'react';
import { Alert, Box, Chip, LinearProgress, Paper, Stack, Table, TableBody, TableCell, TableHead, TableRow, Typography } from '@mui/material';
import { api } from '../api';
import type { RoomUsage } from '../types';
import { LoadingState } from '../components/PageState';

export function UsageReportPage() {
  const [rows, setRows] = useState<RoomUsage[]>([]);
  const [error, setError] = useState('');
  useEffect(() => { api.getRoomUsage().then(setRows).catch((reason: Error) => setError(reason.message)); }, []);
  return <Stack spacing={3}><Box><Typography variant="h4" fontWeight={700}>Room usage</Typography><Typography color="text.secondary">Historical reservations only: cancelled and future reservations are excluded. Completed reservations count as used.</Typography></Box>{error ? <Alert severity="error">{error}</Alert> : rows.length === 0 ? <LoadingState /> : <Paper variant="outlined" sx={{ overflowX: 'auto' }}><Table sx={{ minWidth: 700 }}><TableHead><TableRow><TableCell>Room</TableCell><TableCell>Past reservations</TableCell><TableCell>Completed</TableCell><TableCell sx={{ minWidth: 220 }}>Completed rate</TableCell><TableCell>Signal</TableCell></TableRow></TableHead><TableBody>{rows.map((row) => <TableRow key={row.roomId}><TableCell>{row.roomName}</TableCell><TableCell>{row.reservationCount}</TableCell><TableCell>{row.usedReservationCount}</TableCell><TableCell><Stack direction="row" spacing={1} alignItems="center"><LinearProgress variant="determinate" value={row.usageRate * 100} sx={{ flex: 1 }} /><Typography variant="body2">{Math.round(row.usageRate * 100)}%</Typography></Stack></TableCell><TableCell><Chip label={row.reservationCount > 0 && row.usageRate < 0.5 ? 'Often booked, rarely used' : 'Healthy'} color={row.reservationCount > 0 && row.usageRate < 0.5 ? 'warning' : 'success'} size="small" /></TableCell></TableRow>)}</TableBody></Table></Paper>}</Stack>;
}
