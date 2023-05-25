import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { User } from '../models/user';
import { Login } from '../models/login';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';

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
            localStorage.setItem('user', JSON.stringify(user));
          }
        })
      );
  }

  lougout() {
    localStorage.removeItem('user');
  }
}
