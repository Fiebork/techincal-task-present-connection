import { Alert, CircularProgress, Stack, Typography } from '@mui/material';

export function LoadingState() {
  return (
    <Stack alignItems="center" py={8}>
      <CircularProgress size={28} />
    </Stack>
  );
}

export function ErrorState({ message }: { message: string }) {
  return <Alert severity="error">{message}</Alert>;
}

export function EmptyState({ message }: { message: string }) {
  return (
    <Typography color="text.secondary" py={6} textAlign="center">
      {message}
    </Typography>
  );
}
