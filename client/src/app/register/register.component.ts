import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Register } from '../models/register';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();

  regModel: Register = { username: '', password: '' };

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  register() {
    this.accountService.register(this.regModel).subscribe({
      next: () => {
        this.cancel();
      },
      error: (e) => this.toastr.error(e.error.title),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
