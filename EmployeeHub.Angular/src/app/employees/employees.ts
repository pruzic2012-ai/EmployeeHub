import { Component, OnInit } from '@angular/core';
import {CurrencyPipe} from "@angular/common";
import { EmployeeService } from '../service/employee';

@Component({
  imports: [CurrencyPipe],
  selector: 'app-employees',
  standalone: true,
  styleUrl: './employees.css',
  templateUrl: './employees.html',
})
export class Employees implements OnInit {

  employees: any[] = [];
  loading = false;
  errorMessage = '';

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {

    this.loading = true;
    this.errorMessage = '';

    this.employeeService.getEmployees().subscribe({

      next: (data) => {
        this.employees = data;
        this.loading = false;

        console.log('Employees:', data);
      },

      error: (error) => {
        console.error('Error loading employees:', error);

        this.errorMessage = 'Unable to load employees.';
        this.loading = false;
      }

    });
  }
}