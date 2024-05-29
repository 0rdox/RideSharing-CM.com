import { ICar } from "../models/car.interface";

export interface IReservationInputInfoModel {
  startingTime: Date;
  endingTime: Date;
  car: ICar;
}
