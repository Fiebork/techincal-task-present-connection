import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { App } from './App';

const theme = createTheme({
  palette: {
    primary: { main: '#315efb' },
    background: { default: '#f7f8fc' },
  },
  shape: { borderRadius: 10 },
  typography: { fontFamily: 'Inter, Roboto, Arial, sans-serif' },
});

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </ThemeProvider>
  </StrictMode>,
);
