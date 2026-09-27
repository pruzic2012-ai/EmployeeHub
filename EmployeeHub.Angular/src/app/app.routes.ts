import { Routes } from '@angular/router';
import { Employees } from './employees/employees';
import { Login } from './login/login';

export const routes: Routes = [
  {
    path: 'employees',
    component: Employees
  },
  {
    path: 'login',
    component: Login
  }
];