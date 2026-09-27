import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../service/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  username = '';
  password = '';

  constructor(private authService: Auth) {}

  login(): void {
    this.authService.login(this.username, this.password).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        localStorage.setItem('token', response.token);
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }
}