
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DepartmentReportService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7071/api/department-report';

  generateReport(department: string): Observable<string> {
    return this.http.post(
      this.apiUrl,
      { department },
      { responseType: 'text' }
    );
  }
}
