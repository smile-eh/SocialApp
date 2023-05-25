import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { Login } from '../models/login';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  title: string = 'Social App';
  model: Login = { username: '', password: '' };
  loggedIn: boolean = false;

  constructor(private accountService: AccountService) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
        this.loggedIn = true;
      },
      error: (error) => console.log(error),
    });
  }

  logout() {
    this.loggedIn = false;
  }
}
