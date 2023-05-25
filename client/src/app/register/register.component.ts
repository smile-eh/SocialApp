import { Component, OnInit } from '@angular/core';
import { Register } from '../models/register';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  regModel: Register = { username: '', password: '' };

  constructor() {}

  ngOnInit(): void {}

  register() {
    console.log(this.regModel);
  }

  cancel() {
    console.log('cancel');
  }
}
