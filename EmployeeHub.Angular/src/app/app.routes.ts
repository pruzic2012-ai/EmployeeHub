import { Routes } from '@angular/router';
import { Employees } from './employees/employees';
import { Login } from './login/login';
import {AddEmployee} from './add-employee/add-employee';

export const routes: Routes = [
  {
    path: 'employees',
    component: Employees
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'add-employee',
    component: AddEmployee
  }
];