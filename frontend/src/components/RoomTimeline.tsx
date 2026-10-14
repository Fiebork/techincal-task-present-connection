import { Box, Paper, Stack, Typography } from '@mui/material';
import type { Reservation, Room } from '../types';
import { officeDayBounds, overlapsDay } from '../utils';

const hours = Array.from({ length: 25 }, (_, index) => index);

export function RoomTimeline({ rooms, reservations, day }: { rooms: Room[]; reservations: Reservation[]; day: string }) {
  return (
    <Stack spacing={2}>
      {rooms.map((room) => {
        const roomReservations = reservations.filter(
          (reservation) => reservation.roomId === room.id && reservation.status !== 'Cancelled' && overlapsDay(reservation.startDate, reservation.endDate, day),
        );
        return (
          <Paper key={room.id} variant="outlined" sx={{ p: 2, overflowX: 'auto' }}>
            <Typography fontWeight={700} mb={1}>{room.name}</Typography>
            <Box sx={{ minWidth: 1440, position: 'relative' }}>
              <Stack direction="row" sx={{ borderBottom: 1, borderColor: 'divider' }}>
                {hours.map((hour) => <Box key={hour} sx={{ flex: 1, py: 0.5, color: 'text.secondary', fontSize: 12 }}>{String(hour).padStart(2, '0')}:00</Box>)}
              </Stack>
              <Box sx={{ height: 64, position: 'relative', background: 'repeating-linear-gradient(to right, transparent 0, transparent calc(100% / 24 - 1px), #e5e7eb calc(100% / 24 - 1px), #e5e7eb calc(100% / 24))' }}>
                {roomReservations.map((reservation) => {
                  const { start: dayStart, end: dayEnd } = officeDayBounds(day);
                  const start = new Date(Math.max(new Date(reservation.startDate).getTime(), dayStart.getTime()));
                  const end = new Date(Math.min(new Date(reservation.endDate).getTime(), dayEnd.getTime()));
                  const startMinutes = (start.getTime() - dayStart.getTime()) / 60000;
                  const duration = Math.max((end.getTime() - start.getTime()) / 60000, 15);
                  return (
                    <Box key={reservation.id} sx={{ position: 'absolute', left: `${(startMinutes / 1440) * 100}%`, width: `${Math.min(duration / 1440, 1) * 100}%`, top: 10, height: 44, px: 1, borderRadius: 1, bgcolor: reservation.status === 'Completed' ? 'success.light' : 'primary.light', overflow: 'hidden' }}>
                      <Typography variant="caption" noWrap fontWeight={700}>{reservation.title}</Typography>
                    </Box>
                  );
                })}
              </Box>
            </Box>
          </Paper>
        );
      })}
    </Stack>
  );
}
