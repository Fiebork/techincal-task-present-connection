import { useEffect, useState } from 'react';
import { Alert, Box, Button, IconButton, MenuItem, Paper, Select, Stack, Table, TableBody, TableCell, TableHead, TableRow, TextField, ToggleButton, ToggleButtonGroup, Tooltip, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import CancelIcon from '@mui/icons-material/Cancel';
import DoneIcon from '@mui/icons-material/Done';
import { api } from '../api';
import type { Reservation, ReservationRequest, ReservationStatus, Room, User } from '../types';
import { ErrorState, LoadingState } from '../components/PageState';
import { StatusChip } from '../components/StatusChip';
import { formatDateTime, officeToday, toIsoDateTime } from '../utils';
import { ReservationDialog } from '../components/ReservationDialog';
import { RoomTimeline } from '../components/RoomTimeline';

export function ReservationsPage() {
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [roomId, setRoomId] = useState('');
  const [organizerId, setOrganizerId] = useState('');
  const [status, setStatus] = useState('');
  const [from, setFrom] = useState('');
  const [to, setTo] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<Reservation>();
  const [error, setError] = useState('');
  const [dialogError, setDialogError] = useState('');
  const [referenceDataError, setReferenceDataError] = useState('');
  const [referenceDataLoading, setReferenceDataLoading] = useState(true);
  const [loading, setLoading] = useState(true);
  const [view, setView] = useState<'list' | 'timeline'>('list');
  const [timelineDay, setTimelineDay] = useState(officeToday);

  async function load() {
    setLoading(true);
    try {
      setReservations(await api.getReservations({
        roomId: roomId ? Number(roomId) : undefined,
        organizerId: organizerId ? Number(organizerId) : undefined,
        status: status as ReservationStatus || undefined,
        from: from ? toIsoDateTime(`${from}T00:00`) : undefined,
        to: to ? toIsoDateTime(`${to}T23:59:59`) : undefined,
      }));
    } catch (reason) {
      setError((reason as Error).message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    setReferenceDataLoading(true);
    Promise.all([api.getRooms(), api.getUsers()])
      .then(([loadedRooms, loadedUsers]) => {
        setRooms(loadedRooms);
        setUsers(loadedUsers);
      })
      .catch((reason: Error) => setReferenceDataError(reason.message))
      .finally(() => setReferenceDataLoading(false));
  }, []);

  useEffect(() => { void load(); }, [roomId, organizerId, status, from, to]);

  async function save(request: ReservationRequest) {
    try {
      if (editing) await api.updateReservation(editing.id, request);
      else await api.createReservation(request);
      setDialogOpen(false);
      setEditing(undefined);
      setDialogError('');
      await load();
    } catch (reason) {
      setDialogError((reason as Error).message);
    }
  }

  async function cancel(id: number) {
    if (!window.confirm('Cancel this reservation?')) return;
    try { await api.cancelReservation(id); await load(); } catch (reason) { setError((reason as Error).message); }
  }

  async function end(id: number) {
    if (!window.confirm('End this meeting now?')) return;
    try { await api.endReservation(id); await load(); } catch (reason) { setError((reason as Error).message); }
  }

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" gap={2}>
        <Box><Typography variant="h4" fontWeight={700}>Reservations</Typography><Typography color="text.secondary">Manage meetings and room bookings.</Typography></Box>
        <Button variant="contained" startIcon={<AddIcon />} disabled={referenceDataLoading || Boolean(referenceDataError)} onClick={() => { setEditing(undefined); setDialogError(''); setDialogOpen(true); }}>New reservation</Button>
      </Stack>
      <Stack direction={{ xs: 'column', sm: 'row' }} flexWrap="wrap" spacing={2} alignItems={{ sm: 'center' }}>
        <Select displayEmpty value={roomId} onChange={(event) => setRoomId(event.target.value)} sx={{ minWidth: 180 }}><MenuItem value="">All rooms</MenuItem>{rooms.map((room) => <MenuItem key={room.id} value={room.id}>{room.name}</MenuItem>)}</Select>
        <Select displayEmpty value={organizerId} onChange={(event) => setOrganizerId(event.target.value)} sx={{ minWidth: 180 }}><MenuItem value="">All organizers</MenuItem>{users.map((user) => <MenuItem key={user.id} value={user.id}>{user.firstName} {user.lastName}</MenuItem>)}</Select>
        <Select displayEmpty value={status} onChange={(event) => setStatus(event.target.value)} sx={{ minWidth: 180 }}><MenuItem value="">All statuses</MenuItem><MenuItem value="Booked">Booked</MenuItem><MenuItem value="Completed">Completed</MenuItem><MenuItem value="Cancelled">Cancelled</MenuItem></Select>
        <TextField type="date" label="From" value={from} onChange={(event) => setFrom(event.target.value)} InputLabelProps={{ shrink: true }} />
        <TextField type="date" label="To" value={to} onChange={(event) => setTo(event.target.value)} InputLabelProps={{ shrink: true }} />
        <ToggleButtonGroup exclusive value={view} onChange={(_, nextView: 'list' | 'timeline' | null) => nextView && setView(nextView)} size="small"><ToggleButton value="list">List</ToggleButton><ToggleButton value="timeline">Timeline</ToggleButton></ToggleButtonGroup>
      </Stack>
      {referenceDataError && <Alert severity="error">Could not load rooms and organizers: {referenceDataError}</Alert>}
      {view === 'timeline' && <Stack direction="row" spacing={2} alignItems="center"><TextField type="date" label="Timeline day" value={timelineDay} onChange={(event) => setTimelineDay(event.target.value)} InputLabelProps={{ shrink: true }} /><Typography color="text.secondary">Showing Lithuania time (Europe/Vilnius), 00:00–24:00</Typography></Stack>}
      {error && <ErrorState message={error} />}
      {loading && reservations.length === 0 ? <LoadingState /> : view === 'timeline' ? <RoomTimeline rooms={rooms} reservations={reservations} day={timelineDay} /> : <Paper variant="outlined" sx={{ overflowX: 'auto' }}><Table sx={{ minWidth: 900 }}><TableHead><TableRow><TableCell>Title</TableCell><TableCell>Room</TableCell><TableCell>Organizer</TableCell><TableCell>When</TableCell><TableCell>People</TableCell><TableCell>Status</TableCell><TableCell>Actions</TableCell></TableRow></TableHead><TableBody>{reservations.map((reservation) => <TableRow key={reservation.id}><TableCell>{reservation.title}</TableCell><TableCell>{reservation.roomName}</TableCell><TableCell>{reservation.organizerName}</TableCell><TableCell>{formatDateTime(reservation.startDate)}<br />– {formatDateTime(reservation.endDate)}</TableCell><TableCell>{reservation.attendeeCount}</TableCell><TableCell><StatusChip status={reservation.status} /></TableCell><TableCell>{reservation.status === 'Booked' && <Stack direction="row"><Tooltip title="Edit"><IconButton onClick={() => { setEditing(reservation); setDialogError(''); setDialogOpen(true); }}><EditIcon fontSize="small" /></IconButton></Tooltip><Tooltip title="Cancel"><IconButton onClick={() => void cancel(reservation.id)}><CancelIcon fontSize="small" /></IconButton></Tooltip><Tooltip title="End early"><IconButton onClick={() => void end(reservation.id)}><DoneIcon fontSize="small" /></IconButton></Tooltip></Stack>}</TableCell></TableRow>)}</TableBody></Table></Paper>}
      <ReservationDialog open={dialogOpen} rooms={rooms} users={users} reservation={editing} error={dialogError} onClose={() => setDialogOpen(false)} onSubmit={save} />
    </Stack>
  );
}
