import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { ICreateUser, IUser, Role } from '../../models/user.interface';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../../models/api-reponse-interface';
import { url } from './../../common/environment/environment';
import { Id } from '../../models/id.type';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private readonly http: HttpClient) {}

  public getUsers(): Observable<IUser[]> {
    return this.http
      .get<ApiResponse<IUser[]>>(`${url}user`)
      .pipe(map((response: any) => response.data as IUser[]));
  }

  public createUser(user: ICreateUser): Observable<IUser> {
    return this.http
      .post<ApiResponse<IUser>>(`${url}user`, user)
      .pipe(map((response: any) => response.data as IUser));
  }

  public updateUser(updatedUser: IUser): Observable<IUser> {
    return this.http
      .put<ApiResponse<IUser>>(`${url}user/${updatedUser.id}`, updatedUser)
      .pipe(map((response: any) => response.data as IUser));
  }

  public deleteUser(id: Id): Observable<boolean> {
    return this.http
      .delete<ApiResponse<Object>>(`${url}user/${id}`)
      .pipe(map((response: any) => true));
  }

  public getUserById(id: string): Observable<IUser> {
    return this.http
      .get<ApiResponse<IUser>>(`${url}user/${id}`)
      .pipe(map((response: any) => response.data as IUser));
  }

  public getProfile(): Observable<IUser> {
    return this.http
      .get<ApiResponse<IUser>>(`${url}auth/profile`)
      .pipe(map((response: any) => response.data as IUser));
  }
}
