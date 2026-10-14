export type RoomEquipment =
  | 'Projector'
  | 'TvScreen'
  | 'VideoConferencing'
  | 'Whiteboard';

export type ReservationStatus = 'Booked' | 'Completed' | 'Cancelled';

export interface Room {
  id: number;
  name: string;
  capacity: number;
  floor: number;
  equipment: RoomEquipment[];
  isActive: boolean;
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  activeReservationCount: number;
}

export interface Reservation {
  id: number;
  roomId: number;
  roomName: string;
  organizerId: number;
  organizerName: string;
  title: string;
  startDate: string;
  endDate: string;
  attendeeCount: number;
  status: ReservationStatus;
}

export interface ReservationRequest {
  title: string;
  roomId: number;
  organizerId: number;
  startDate: string;
  endDate: string;
  attendeeCount: number;
}

export interface RoomUsage {
  roomId: number;
  roomName: string;
  reservationCount: number;
  usedReservationCount: number;
  usageRate: number;
}
