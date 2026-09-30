import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface CreateEmployee {
    firstName: string;
    lastName: string;
    email: string;
    department: string;
    salary: number;
  }

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  private apiUrl = 'https://localhost:7071/api/v1/employees';

  constructor(private http: HttpClient) {}

  

  getEmployees(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getEmployeeById(id: number): Observable<any> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<any>(url);
  }
  
updateEmployee(id: number, employee: any): Observable<any> {
  const url = `${this.apiUrl}/${id}`;
  return this.http.put<any>(url, employee);
}
  createEmployee(employee: CreateEmployee): Observable<any> {
    return this.http.post<any>(this.apiUrl, employee);
  }

}