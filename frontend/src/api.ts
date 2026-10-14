import type {
  Reservation,
  ReservationRequest,
  ReservationStatus,
  Room,
  RoomEquipment,
  RoomUsage,
  User,
} from './types';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? '/api';

export interface ReservationFilters {
  roomId?: number;
  organizerId?: number;
  status?: ReservationStatus;
  from?: string;
  to?: string;
}

export interface RoomFilters {
  minimumCapacity?: number;
  equipment?: RoomEquipment[];
  floor?: number;
}

interface ProblemDetails {
  detail?: string;
  code?: string;
  errors?: Record<string, string[]>;
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: options?.body
      ? { 'Content-Type': 'application/json', ...options.headers }
      : options?.headers,
    ...options,
  });

  if (!response.ok) {
    const problem = (await response.json().catch(() => ({}))) as ProblemDetails;
    throw new Error(problem.detail ?? Object.values(problem.errors ?? {}).flat()[0] ?? 'Request failed');
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function queryString(values: object): string {
  const params = new URLSearchParams();
  Object.entries(values as Record<string, string | number | string[] | undefined>).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      value.forEach((item) => params.append(key, item));
    } else if (value !== undefined && value !== '') {
      params.set(key, String(value));
    }
  });
  const query = params.toString();
  return query ? `?${query}` : '';
}

export const api = {
  getRooms: (filters: RoomFilters = {}) =>
    request<Room[]>(`/rooms${queryString(filters)}`),
  getUsers: () => request<User[]>('/users'),
  getReservations: (filters: ReservationFilters = {}) =>
    request<Reservation[]>(`/reservations${queryString(filters)}`),
  createReservation: (body: ReservationRequest) =>
    request<Reservation>('/reservations', { method: 'POST', body: JSON.stringify(body) }),
  updateReservation: (id: number, body: ReservationRequest) =>
    request<Reservation>(`/reservations/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  cancelReservation: (id: number) =>
    request<void>(`/reservations/${id}/cancel`, { method: 'POST' }),
  endReservation: (id: number) =>
    request<Reservation>(`/reservations/${id}/end`, { method: 'POST' }),
  getRoomUsage: () => request<RoomUsage[]>('/reports/room-usage'),
};
