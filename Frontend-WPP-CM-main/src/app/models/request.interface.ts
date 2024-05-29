import { Id } from './id.type';
import { IReservation } from './reservation.interface';
import { IUser } from './user.interface';

export enum Status {
  Pending = 'PENDING',
  Accepted = 'ACCEPTED',
  Denied = 'DENIED',
}

export interface IRequest {
  id: Id;
  seats: number;
  status: Status;
  creationDate: Date;
  user: IUser;
  reservation: IReservation;
}

export type IRequest2 = {
  id: Id;
  seats: number;
  status: Status;
  creationDate: Date;
  user: IUser;
  reservation: IReservation;
  token: string;
};

export type ICreateRequest = {
  seats: number;
  reservationId: Id;
};
