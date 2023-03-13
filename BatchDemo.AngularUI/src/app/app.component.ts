import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Batch AngularUI';
  batch: any;
  users: any;
  constructor(private http: HttpClient) { }
  ngOnInit(): void {
   // this.http.get('https://localhost:44308/batch/3212c418-7c52-437d-bc82-601468fd875a').subscribe({
    this.http.get('https://localhost:44308/Users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed!')
    });
  }

}
