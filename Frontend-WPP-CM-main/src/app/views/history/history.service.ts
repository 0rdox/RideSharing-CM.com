import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IReservation } from '../../models/reservation.interface';
import { url } from './../../common/environment/environment';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../models/api-reponse-interface';
import { Id } from '../../models/id.type';


@Injectable({
  providedIn: 'root',
})
export class HistoryService {
  constructor(
    private readonly http: HttpClient,
  ) {}

  public getReservations(id: Id): Observable<IReservation[]> {
    return this.http
      .get<ApiResponse<IReservation[]>>(`${url}reservation?userId=${id}`)
      .pipe(map((response: any) => response.data as IReservation[]));
  }
}