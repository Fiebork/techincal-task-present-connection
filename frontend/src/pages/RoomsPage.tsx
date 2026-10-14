import { useEffect, useState } from 'react';
import { Box, Card, CardContent, Checkbox, Chip, FormControl, FormControlLabel, FormGroup, Stack, TextField, Typography } from '@mui/material';
import { api } from '../api';
import type { Room, RoomEquipment } from '../types';
import { EmptyState, ErrorState, LoadingState } from '../components/PageState';

export function RoomsPage() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [capacity, setCapacity] = useState('');
  const [floor, setFloor] = useState('');
  const [equipment, setEquipment] = useState<RoomEquipment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    setLoading(true);
    api.getRooms({ minimumCapacity: capacity ? Number(capacity) : undefined, floor: floor ? Number(floor) : undefined, equipment })
      .then(setRooms).catch((reason: Error) => setError(reason.message)).finally(() => setLoading(false));
  }, [capacity, floor, equipment]);

  return (
    <Stack spacing={3}>
      <Box><Typography variant="h4" fontWeight={700}>Meeting rooms</Typography><Typography color="text.secondary">Find an available room for your meeting.</Typography></Box>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'flex-start' }}>
        <TextField label="Minimum capacity" type="number" value={capacity} onChange={(event) => setCapacity(event.target.value)} />
        <TextField label="Floor" type="number" value={floor} onChange={(event) => setFloor(event.target.value)} />
        <FormControl component="fieldset" sx={{ minWidth: 220 }}>
          <Typography variant="body2" color="text.secondary">Equipment</Typography>
          <FormGroup row>
            {(['Projector', 'TvScreen', 'VideoConferencing', 'Whiteboard'] as RoomEquipment[]).map((item) => (
              <FormControlLabel
                key={item}
                label={item}
                control={
                  <Checkbox
                    size="small"
                    checked={equipment.includes(item)}
                    onChange={(event) => setEquipment((current) => event.target.checked
                      ? [...current, item]
                      : current.filter((selected) => selected !== item))}
                  />
                }
              />
            ))}
          </FormGroup>
        </FormControl>
      </Stack>
      {loading && rooms.length === 0 ? <LoadingState /> : error ? <ErrorState message={error} /> : rooms.length === 0 ? <EmptyState message="No rooms match these filters." /> : <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))', gap: 2 }}>{rooms.map((room) => <Card key={room.id} variant="outlined"><CardContent><Stack spacing={1.5}><Stack direction="row" justifyContent="space-between"><Typography variant="h6">{room.name}</Typography><Chip label={room.isActive ? 'Active' : 'Inactive'} color={room.isActive ? 'success' : 'default'} size="small" /></Stack><Typography color="text.secondary">ID {room.id} · Floor {room.floor} · Capacity {room.capacity}</Typography><Stack direction="row" flexWrap="wrap" gap={0.75}>{room.equipment.map((item) => <Chip key={item} label={item} size="small" variant="outlined" />)}</Stack></Stack></CardContent></Card>)}</Box>}
    </Stack>
  );
}
