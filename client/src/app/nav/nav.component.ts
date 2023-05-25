import { Component, OnInit } from '@angular/core';

class Model {
  username: string;
  password: string;
  constructor() {
    this.username = "";
    this.password = "";
  }
}

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  title: string = 'Social App';
  model: Model = new Model();

  constructor() { }

  ngOnInit(): void {
  }

  login() {
    console.log(this.model);
  }
}