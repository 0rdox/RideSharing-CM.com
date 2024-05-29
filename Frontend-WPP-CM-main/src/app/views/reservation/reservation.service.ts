import { Injectable } from '@angular/core';
import {
  ICreateReservation,
  IReservation,
} from '../../models/reservation.interface';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../models/api-reponse-interface';
import { url } from '../../common/environment/environment';
import { Observable, map, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ReservationService {
  constructor(private readonly http: HttpClient) {}

  public createReservation(
    reservation: ICreateReservation
  ): Observable<IReservation> {
    return this.http
      .post<ApiResponse<ICreateReservation>>(`${url}reservation`, reservation)
      .pipe(map((response: any) => response.data as IReservation));
  }
}
