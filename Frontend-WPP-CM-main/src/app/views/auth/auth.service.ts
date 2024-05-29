import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import {
  ApiResponse,
  loginReturnObject,
} from '../../models/api-reponse-interface';
import { url } from './../../common/environment/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private readonly http: HttpClient) {}

  public login(signInModule: any): Observable<loginReturnObject> {
    return this.http
      .post<ApiResponse<loginReturnObject>>(`${url}auth/login`, signInModule)
      .pipe(map((response: any) => response.data as loginReturnObject));
  }
}
