import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tokenKey, roleKey, idKey, userKey } from './localstorage.keys';
import { IUser, Role } from '../../models/user.interface';
import { Id } from '../../models/id.type';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  private userToken$ = new BehaviorSubject<string | null>(null);
  private userRole$ = new BehaviorSubject<Role | null>(null);
  private userId$ = new BehaviorSubject<Number | null>(null);
  private user$ = new BehaviorSubject<IUser | null>(null);

  constructor() {
    //set if it is in storageRole
    this.userToken$.next(localStorage.getItem(tokenKey));
    this.userRole$.next(localStorage.getItem(roleKey) as Role | null);
    this.userId$.next(localStorage.getItem(idKey) as Number | null);
    this.user$.next(JSON.parse(localStorage.getItem(userKey) as string));
  }

  setUserToken(token: string | null) {
    this.userToken$.next(token);
    if (token == null) localStorage.removeItem(tokenKey);
    else localStorage.setItem(tokenKey, token);
  }

  getUserToken(): Observable<string | null> {
    return this.userToken$;
  }

  setUserRole(role: Role | null) {
    this.userRole$.next(role);
    if (role == null) localStorage.removeItem(roleKey);
    else localStorage.setItem(roleKey, role);
  }

  getUserRole(): Observable<Role | null> {
    return this.userRole$;
  }

  setUserId(id: Id | null) {
    this.userId$.next(id);
    if (id == null) localStorage.removeItem(idKey);
    else localStorage.setItem(idKey, id.toString());
  }

  getUserId(): Observable<Number | null> {
    return this.userId$;
  }

  setUser(user: IUser | null) {
    this.user$.next(user);
    if (user == null) localStorage.removeItem(userKey);
    else localStorage.setItem(userKey, JSON.stringify(user));
  }

  getUser(): Observable<IUser | null> {
    return this.user$;
  }
}
