import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../models/user';
import { Login } from '../models/login';
import { Register } from '../models/register';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | undefined>(undefined);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  login(login: Login) {
    return this.http
      .post<User>(this.baseUrl + 'account/login', {
        username: login.username,
        password: login.password,
      })
      .pipe(
        map((response: User) => {
          const user = response;
          if (user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }

  register(regModel: Register) {
    return this.http
      .post<User>(this.baseUrl + 'account/register', {
        username: regModel.username,
        password: regModel.password,
        gender: regModel.gender,
        dateOfBirth: regModel.dateOfBirth,
        knownAs: regModel.knownAs,
        city: regModel.city,
        country: regModel.country,
      })
      .pipe(
        map((response: User) => {
          const user = response;
          if (user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(undefined);
  }
}
