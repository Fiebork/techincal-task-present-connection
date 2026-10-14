import { useEffect, useState } from 'react';
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import type { Reservation, ReservationRequest, Room, User } from '../types';
import { toIsoDateTime, toLocalInputValue } from '../utils';

interface Props {
  open: boolean;
  rooms: Room[];
  users: User[];
  reservation?: Reservation;
  error?: string;
  onClose: () => void;
  onSubmit: (request: ReservationRequest) => Promise<void>;
}

export function ReservationDialog({ open, rooms, users, reservation, error, onClose, onSubmit }: Props) {
  const [title, setTitle] = useState('');
  const [roomId, setRoomId] = useState('');
  const [organizerId, setOrganizerId] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [attendeeCount, setAttendeeCount] = useState('1');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!open) return;
    setTitle(reservation?.title ?? '');
    setRoomId(String(reservation?.roomId ?? rooms.find((room) => room.isActive)?.id ?? ''));
    setOrganizerId(String(reservation?.organizerId ?? users[0]?.id ?? ''));
    setStartDate(reservation ? toLocalInputValue(reservation.startDate) : '');
    setEndDate(reservation ? toLocalInputValue(reservation.endDate) : '');
    setAttendeeCount(String(reservation?.attendeeCount ?? 1));
  }, [open, reservation, rooms, users]);

  async function submit() {
    setSubmitting(true);
    try {
      await onSubmit({
        title,
        roomId: Number(roomId),
        organizerId: Number(organizerId),
        startDate: toIsoDateTime(startDate),
        endDate: toIsoDateTime(endDate),
        attendeeCount: Number(attendeeCount),
      });
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{reservation ? 'Edit reservation' : 'New reservation'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} pt={1}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField label="Meeting title" value={title} onChange={(event) => setTitle(event.target.value)} required />
          <TextField select label="Room" value={roomId} onChange={(event) => setRoomId(event.target.value)} required>
            {rooms.filter((room) => room.isActive || room.id === reservation?.roomId).map((room) => (
              <MenuItem key={room.id} value={room.id}>{room.name} · capacity {room.capacity}</MenuItem>
            ))}
          </TextField>
          <TextField select label="Organizer" value={organizerId} onChange={(event) => setOrganizerId(event.target.value)} required>
            {users.map((user) => <MenuItem key={user.id} value={user.id}>{user.firstName} {user.lastName}</MenuItem>)}
          </TextField>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField label="Starts" type="datetime-local" value={startDate} onChange={(event) => setStartDate(event.target.value)} InputLabelProps={{ shrink: true }} helperText="Lithuania time" fullWidth required />
            <TextField label="Ends" type="datetime-local" value={endDate} onChange={(event) => setEndDate(event.target.value)} InputLabelProps={{ shrink: true }} helperText="Lithuania time" fullWidth required />
          </Stack>
          <TextField label="Attendees" type="number" value={attendeeCount} onChange={(event) => setAttendeeCount(event.target.value)} inputProps={{ min: 1 }} required />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" onClick={submit} disabled={submitting}>{submitting ? 'Saving…' : 'Save'}</Button>
      </DialogActions>
    </Dialog>
  );
}
