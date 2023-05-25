import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { Login } from '../models/login';
import { Observable, of } from 'rxjs';
import { User } from '../models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  title: string = 'Social App';
  model: Login = { username: '', password: '' };

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {},
      error: (error) => console.log(error),
    });
  }

  logout() {
    this.accountService.logout();
  }
}
