
import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DepartmentReportService } from '../service/department-report.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-department-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './department-report.html',
  styleUrl: './department-report.css'
})
export class DepartmentReport {
  private readonly reportService = inject(DepartmentReportService);

  department = 'Finance';
  readonly reportHtml = signal('');
  readonly isLoading = signal(false);
  readonly errorMessage = signal('');


  generateReport(): void {
    if (this.isLoading()) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.reportHtml.set('');

    this.reportService.generateReport(this.department)
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        })
      )
      .subscribe({
        next: (html) => {
         const document = new DOMParser().parseFromString(html, 'text/html');
        const reportText = document.querySelector('pre')?.textContent?.trim() ?? '';

        this.reportHtml.set(reportText);
        },
        error: (error) => {
          console.error('Department report request failed:', error);

          this.errorMessage.set(
            error.status === 401
              ? 'Your session may have expired. Please log in again.'
              : error.status === 403
                ? 'You do not have permission to generate this report.'
                : error.status === 0
                  ? 'Cannot connect to EmployeeHub. Make sure the API is running.'
                  : 'Unable to generate the department report. Please try again.'
          );
        }
      });
  }

  


getReportValue(label: string): string {
  const report = this.reportHtml();

  const aliases: Record<string, string[]> = {
    'Employee count': ['Employee count', 'Employees', 'Employee Count']
  };

  const labels = aliases[label] ?? [label];

  const line = report
    .split(/\r?\n/)
    .find(line => {
      const normalizedLine = line.trim().replace(/^[-*•]\s*/, '');

      return labels.some(name =>
        normalizedLine.toLowerCase().startsWith(`${name.toLowerCase()}:`)
      );
    });

  if (!line) {
    return '—';
  }

  const normalizedLine = line.trim().replace(/^[-*•]\s*/, '');
  return normalizedLine.substring(normalizedLine.indexOf(':') + 1).trim();
}



getObservation(): string {
  const report = this.reportHtml();

  const match = report.match(
    /(?:Brief observation|Observation):\s*([\s\S]*?)(?=\r?\n\s*\r?\n|$)/i
  );

  return match?.[1]?.trim() ?? 'No observation was provided.';
}

}
