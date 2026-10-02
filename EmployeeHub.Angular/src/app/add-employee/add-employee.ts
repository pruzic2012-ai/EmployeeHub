import { Component, signal } from '@angular/core';
import {Router, RouterLink} from '@angular/router';
import {FormsModule} from '@angular/forms';
import { EmployeeService } from '../service/employee.service';

@Component({
  imports: [FormsModule, RouterLink],
  standalone: true,
  selector: 'app-add-employee',
  styleUrl: './add-employee.css',
  templateUrl: './add-employee.html',
})
export class AddEmployee {
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  department: string = '';
  salary: number | null = null;
  
  submitted = false;
  successMessage = signal('');
  errorMessage = signal('');

  constructor(private employeeService: EmployeeService, private router: Router) {}

  createEmployee(): void {

    this.submitted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.firstName && this.lastName && this.email && this.department && this.salary !== null) {
      const newEmployee = {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        department: this.department,
        salary: this.salary
      };

      this.employeeService.createEmployee(newEmployee).subscribe({
        next: (response) => {
          console.log('Employee created successfully:', response);
          // Optionally, you can reset the form fields after successful creation
          this.firstName = '';
          this.lastName = '';
          this.email = '';
          this.department = '';
          this.salary = null;

          this.successMessage.set('Employee created successfully!');
          setTimeout(() => {
            this.router.navigate(['/employees']); // Navigate to the employee list page after 2 seconds
          }, 1000)

        },
        error: (error) => {
          console.error('Error creating employee:', error);
          this.errorMessage.set('Error creating employee. Please try again.');
        }
      });
    } else {
      console.error('Please fill in all required fields.');
      return;
    }
  }
  
  isValidEmail(): boolean {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email);
}
}
