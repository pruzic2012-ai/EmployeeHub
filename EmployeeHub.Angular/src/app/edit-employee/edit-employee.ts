import { Component, OnInit, ChangeDetectorRef, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { EmployeeService } from '../service/employee.service';

@Component({
  selector: 'app-edit-employee',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './edit-employee.html',
  styleUrl: './edit-employee.css'
})
export class EditEmployee implements OnInit {

  employeeId: number | null = null;
  firstName = '';
  lastName = '';
  email = '';
  department = '';
  salary: number | null = null;

  errorMessage = signal('');
  submitted = false;
  
  constructor(private route: ActivatedRoute, 
              private employeeService: EmployeeService,
              private cdr: ChangeDetectorRef,
              private router: Router) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

     this.employeeId = Number(id);

      console.log('Employee ID:', this.employeeId);

      this.employeeService.getEmployeeById(this.employeeId).subscribe({
        next: (employee) => {
          console.log('Employee loaded:', employee);
          this.firstName = employee.firstName;
          this.lastName = employee.lastName;
          this.email = employee.email;
          this.department = employee.department;
          this.salary = employee.salary;
          this.cdr.detectChanges(); // Trigger change detection after updating properties
          
        },
        error: (error) => {
          console.error('Error loading employee:', error);
        }
      });      
  }

  saveChanges(): void {

    this.submitted = true;
    this.errorMessage.set('');

    if (
    !this.firstName ||
    !this.lastName ||
    !this.email ||
    !this.department ||
    this.salary === null ||
    this.salary < 30000 ||
    !this.isValidEmail()
  ) {
    return;
  }

  if (this.employeeId === null) {
    return;
  }

  const updatedEmployee = {
    firstName: this.firstName,
    lastName: this.lastName,
    email: this.email,
    department: this.department,
    salary: this.salary
  };

  this.employeeService.updateEmployee(
    this.employeeId,
    updatedEmployee
  ).subscribe({
    next: (response) => {
      console.log('Employee updated successfully:', response);

      this.router.navigate(['/employees']);
    },
    error: (error) => {
      console.error('Error updating employee:', error);
      this.errorMessage.set('Failed to update employee. Please try again.');
    }
  });
}
isValidEmail(): boolean {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email);
}
}