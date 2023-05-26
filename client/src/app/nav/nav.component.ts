import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { Login } from '../models/login';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  title: string = 'Social App';
  model: Login = { username: '', password: '' };

  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      error: (e) => this.toastr.error(e.error),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
