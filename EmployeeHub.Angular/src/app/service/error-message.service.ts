import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ErrorMessageService {

  getMessage(error: HttpErrorResponse): string {

    switch (error.status) {

      case 401:
        return 'Your session has expired. Please log in again.';

      case 403:
        return 'You do not have permission to perform this action.';

      case 404:
        return 'The requested resource was not found.';

      case 409:
        return 'The request could not be completed because of a conflict.';

      case 500:
        return 'Something went wrong on the server. Please try again later.';

      default:
        return 'An unexpected error occurred. Please try again.';
    }
  }
}