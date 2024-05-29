import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IReservation } from '../../models/reservation.interface';
import { url } from './../../common/environment/environment';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../models/api-reponse-interface';
import { Id } from '../../models/id.type';
import { BehaviorSubject } from 'rxjs';
import { Role } from '../../models/user.interface';
import { UserService } from '../user/user.service';
import { IRequest, IRequest2 } from '../../models/request.interface';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(
    private readonly http: HttpClient,
    private readonly userService: UserService
  ) {}

  public getReservations(id: Id): Observable<IReservation[]> {
    return this.http
      .get<ApiResponse<IReservation[]>>(`${url}reservation?userId=${id}`)
      .pipe(map((response: any) => response.data as IReservation[]));
  }

  public deleteReservation(id: Id): Observable<boolean> {
    return this.http
      .delete<ApiResponse<Object>>(`${url}reservation/${id}`)
      .pipe(map((response: any) => true));
  }

  public getRequests(id: Id): Observable<IRequest[]> {
    return this.http
      .get<ApiResponse<IRequest[]>>(`${url}request?userId=${id}`)
      .pipe(
        map((response: any) => {
          const requests: IRequest[] = response.data as IRequest[];
          return requests.sort((a, b) => a.status.localeCompare(b.status));
        })
      );
  }
  public getIncomingRequests(id: Id): Observable<IRequest2[]> {
    return this.http
      .get<ApiResponse<IRequest2[]>>(`${url}request/user`)
      .pipe(map((response: any) => response.data as IRequest2[]));
  }

  public responseRequest(
    id: Id,
    token: string,
    status: string
  ): Observable<boolean> {
    return this.http
      .get<ApiResponse<Object>>(
        `${url}request/${id}/verify?token=${token}&status=${status}`
      )
      .pipe(map((response: any) => true));
  }

  public deleteRequest(id: Id): Observable<boolean> {
    return this.http
      .delete<ApiResponse<Object>>(`${url}request/${id}`)
      .pipe(map((response: any) => true));
  }
}
