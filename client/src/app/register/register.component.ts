import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Register } from '../models/register';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: string = new Date().toISOString().split('T')[0];

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      gender: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(128),
        ],
      ],
      confirmPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(128),
          this.validatorMatchStrings('password'),
        ],
      ],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      },
    });
  }

  validatorMatchStrings(match: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(match)?.value
        ? null
        : { notMatching: true };
    };
  }

  register() {
    const formValues = {
      ...this.registerForm.value,
      dateOfBirth: this.getDateOnly(
        this.registerForm.controls['dateOfBirth'].value
      ),
    };
    console.log(formValues);
    this.accountService.register(formValues).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: (e) => {
        for (let ee of e) {
          this.toastr.error(ee.toString().split('.')[0]);
        }
      },
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    let dDob = new Date(dob);
    return new Date(dDob).toISOString().split('T')[0];
  }
}
