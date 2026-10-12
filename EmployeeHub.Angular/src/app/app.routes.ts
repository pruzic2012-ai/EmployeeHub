import { Routes } from '@angular/router';
import { Employees } from './employees/employees';
import { Login } from './login/login';
import {AddEmployee} from './add-employee/add-employee';
import { EditEmployee } from './edit-employee/edit-employee';
import { DepartmentReport } from './department-report/department-report';

export const routes: Routes = [
  {
  path: '',
  redirectTo: 'login',
  pathMatch: 'full'
  },
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
  },
  {
    path: 'edit-employee/:id',
    component: EditEmployee
  },
  {
  path: 'department-report',
  component: DepartmentReport
  }
  
  
];