import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../service/auth';
import { Router } from '@angular/router';

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

  constructor(private authService: Auth, private router: Router) {}

  login(): void {
    this.authService.login(this.username, this.password).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        localStorage.setItem('token', response.token);
        this.router.navigate(['/employees']);
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }
}