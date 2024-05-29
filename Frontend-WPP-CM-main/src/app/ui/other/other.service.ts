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
export class OtherService {
  constructor(private readonly http: HttpClient) {}

  public getLore(): Observable<string> {
    return this.http
      .get<ApiResponse<string>>(`${url}lore`)
      .pipe(map((response: any) => response.data as string));
  }
}
