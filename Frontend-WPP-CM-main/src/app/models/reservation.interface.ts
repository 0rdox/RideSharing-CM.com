import { ICar } from './car.interface';
import { Id } from './id.type';
import { IUser } from './user.interface';

export interface IReservation {
  id: Id;
  departureDate: Date;
  arrivalDate: Date;
  destination: string;
  willReturn: boolean;
  seats: number;
  creationDate: Date;
  car: ICar;
  user: IUser;
}

export type ICreateReservation = {
  departureDate: Date;
  arrivalDate: Date;
  destination: string;
  willReturn: boolean;
  seats: number;
  carId: number;
};
