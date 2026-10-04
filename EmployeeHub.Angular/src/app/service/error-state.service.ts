import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorStateService {

  private errorMessageSubject = new BehaviorSubject<string>('');

  errorMessage$: Observable<string> =
    this.errorMessageSubject.asObservable();

  setError(message: string): void {
    this.errorMessageSubject.next(message);
  }

  clearError(): void {
    this.errorMessageSubject.next('');
  }
}