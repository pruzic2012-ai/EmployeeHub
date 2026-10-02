import { Component, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import {CurrencyPipe} from "@angular/common";
import { EmployeeService } from '../service/employee.service';
import { RouterLink } from '@angular/router';

@Component({
  imports: [CurrencyPipe, RouterLink],
  selector: 'app-employees',
  standalone: true,
  styleUrl: './employees.css',
  templateUrl: './employees.html',
})
export class Employees implements OnInit {

  employees: any[] = [];
  loading = signal(false);
  errorMessage = signal('');

  constructor(private employeeService: EmployeeService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.employeeService.getEmployees().subscribe({

      next: (data) => {
        this.employees = data;
        this.loading.set(false);

        console.log('Employees:', data);
      },

      error: (error) => {
        console.error('Error loading employees:', error);

        this.errorMessage.set('Unable to load employees.');
        this.loading.set(false);
      }

    });
  }

  deleteEmployee(id: number): void {

    const confirmed = window.confirm(
    'Are you sure you want to delete this employee?'
  );

  if (!confirmed) {
    return;
  }

  this.employeeService.deleteEmployee(id).subscribe({
    next: () => {
      console.log('Employee deleted successfully:', id);

      this.employees = this.employees.filter(
        employee => employee.id !== id
      );

      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error deleting employee:', error);
    }
  });
}
}