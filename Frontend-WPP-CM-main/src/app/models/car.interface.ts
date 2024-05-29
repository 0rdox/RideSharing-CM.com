import { Id } from './id.type';

export interface ICar {
  id: Id;
  licensePlate: string; 
  brand: string;
  model: string;
  seats: number;
  imageUrl: string;
  location: string;
  isAvailable: boolean;
}

export type ICreateCar = Pick<
  ICar,
  | 'licensePlate'
  | 'brand'
  | 'model'
  | 'seats'
  | 'imageUrl'
  | 'location'
  | 'isAvailable'
>;
