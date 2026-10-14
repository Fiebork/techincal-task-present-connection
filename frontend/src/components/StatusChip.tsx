import { Chip } from '@mui/material';
import type { ReservationStatus } from '../types';

const colors: Record<ReservationStatus, 'primary' | 'success' | 'default'> = {
  Booked: 'primary',
  Completed: 'success',
  Cancelled: 'default',
};

export function StatusChip({ status }: { status: ReservationStatus }) {
  return <Chip label={status} color={colors[status]} size="small" />;
}
