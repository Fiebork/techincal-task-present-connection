import { AppBar, Button, Container, Stack, Toolbar, Typography } from '@mui/material';
import { NavLink, Outlet } from 'react-router-dom';

const links = [
  { label: 'Rooms', to: '/rooms' },
  { label: 'Reservations', to: '/reservations' },
  { label: 'Users', to: '/users' },
  { label: 'Usage report', to: '/reports/room-usage' },
];

export function AppLayout() {
  return (
    <>
      <AppBar position="static" color="inherit" elevation={0}>
        <Toolbar sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Typography variant="h6" color="text.primary" sx={{ mr: 4, fontWeight: 700 }}>
            Meeting rooms
          </Typography>
          <Stack direction="row" spacing={1} sx={{ overflowX: 'auto' }}>
            {links.map((link) => (
              <Button
                key={link.to}
                component={NavLink}
                to={link.to}
                sx={{ whiteSpace: 'nowrap', '&.active': { color: 'primary.main', fontWeight: 700 } }}
              >
                {link.label}
              </Button>
            ))}
          </Stack>
        </Toolbar>
      </AppBar>
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Outlet />
      </Container>
    </>
  );
}
