import { addDays, format, parseISO } from 'date-fns';
import { formatInTimeZone, fromZonedTime } from 'date-fns-tz';

export const OFFICE_TIME_ZONE = 'Europe/Vilnius';

export function formatDateTime(value: string): string {
  return new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'medium',
    timeStyle: 'short',
    timeZone: OFFICE_TIME_ZONE,
  }).format(new Date(value));
}

export function toLocalInputValue(value: string): string {
  return formatInTimeZone(value, OFFICE_TIME_ZONE, "yyyy-MM-dd'T'HH:mm");
}

export function toIsoDateTime(value: string): string {
  return fromZonedTime(value, OFFICE_TIME_ZONE).toISOString();
}

export function officeToday(): string {
  return formatInTimeZone(new Date(), OFFICE_TIME_ZONE, 'yyyy-MM-dd');
}

export function officeDayBounds(day: string): { start: Date; end: Date } {
  const dayDate = parseISO(day);
  return {
    start: fromZonedTime(`${day}T00:00:00`, OFFICE_TIME_ZONE),
    end: fromZonedTime(`${format(addDays(dayDate, 1), 'yyyy-MM-dd')}T00:00:00`, OFFICE_TIME_ZONE),
  };
}

export function overlapsDay(startValue: string, endValue: string, day: string): boolean {
  const { start: dayStart, end: dayEnd } = officeDayBounds(day);
  return new Date(startValue) < dayEnd && new Date(endValue) > dayStart;
}
