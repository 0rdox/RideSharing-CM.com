import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, map } from 'rxjs';
import { ICar, ICreateCar } from '../../models/car.interface';
import { IReservation } from '../../models/reservation.interface';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ApiResponse } from '../../models/api-reponse-interface';
import { url } from './../../common/environment/environment';
import { Id } from '../../models/id.type';
import { ICreateRequest, IRequest } from '../../models/request.interface';

@Injectable({
  providedIn: 'root',
})
export class CarService {
  constructor(private readonly http: HttpClient) { }

  public getCars(): Observable<ICar[]> {
    return this.http
      .get<ApiResponse<ICar[]>>(`${url}car`)
      .pipe(map((response: any) => response.data as ICar[]));
  }

  public createCar(car: ICreateCar): Observable<ICar> {
    return this.http
      .post<ApiResponse<ICar>>(`${url}car`, car)
      .pipe(map((response: any) => response.data as ICar));
  }

  updateCar(updatedCar: ICar): Observable<ICar> {
    return this.http
      .put<ApiResponse<ICar>>(`${url}car/${updatedCar.id}`, updatedCar)
      .pipe(map((response: any) => response.data as ICar));
  }

  public getCar(id: Id): Observable<ICar> {
    return this.http
      .get<ApiResponse<ICar>>(`${url}car/${id}`)
      .pipe(map((response: any) => response.data as ICar));
  }

  public deleteCar(id: Id): Observable<boolean> {
    return this.http
      .delete<ApiResponse<Object>>(`${url}car/${id}`)
      .pipe(map((response: any) => true));
  }

  public getReservations(
    id: string,
    startingDate: Date,
    endingDate: Date
  ): Observable<IReservation[]> {
    return this.http
      .get<ApiResponse<IReservation[]>>(
        `${url}reservation?carId=${id}&beginDate=${startingDate.toISOString()}&endDate=${endingDate.toISOString()}`
      )
      .pipe(
        map((response: any) => {
          let results: IReservation[] = response.data;

          results.forEach((r) => {
            r.departureDate = new Date(r.departureDate);
            r.arrivalDate = new Date(r.arrivalDate);
            r.creationDate = new Date(r.creationDate);
          });

          return response.data as IReservation[];
        })
      );
  }


  public createRequest(request: ICreateRequest): Observable<ICreateRequest> {
   return this.http
      .post<ApiResponse<IRequest>>(`${url}request`, request)
      .pipe(map((response: any) => response.data as ICreateRequest))

  };




  public getAcceptedRequestsByReservationId(id: Id): Observable<IRequest[]> {
    return this.http
      .get<ApiResponse<IRequest[]>>(`${url}request?reservationId=${id}&requestStatus=ACCEPTED`)
      .pipe(map((response: any) => response.data as IRequest[]));
  }


}
