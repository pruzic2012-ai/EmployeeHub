import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { ErrorMessageService } from '../service/error-message.service';
import { ErrorStateService } from '../service/error-state.service';


export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const errorMessageService = inject(ErrorMessageService);
  const errorStateService = inject(ErrorStateService);

    return next(req).pipe( 
      tap(() => {
        errorStateService.clearError();
      }),

      catchError((error: HttpErrorResponse) => {

        const message = errorMessageService.getMessage(error);
        errorStateService.setError(message);

        if (error.status === 401) {
          localStorage.removeItem('token');
          router.navigate(['/login']);
        }

        console.error(message);
        console.error('HTTP Error:', error);

        return throwError(() => error);
      })
   );
    
};