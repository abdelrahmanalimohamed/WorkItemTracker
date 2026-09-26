import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  getMessage(error: unknown, fallback = 'Something went wrong. Please try again.'): string {
    if (!(error instanceof HttpErrorResponse)) return fallback;

    if (error.status === 0) {
      return 'Unable to reach the API. Make sure the backend is running and the API URL is correct.';
    }

    if (error.status === 404) return 'The requested work item was not found.';
    if (error.status === 409) return this.extractDetail(error) ?? 'This status transition is not allowed.';
    if (error.status === 400) return this.extractValidation(error) ?? this.extractDetail(error) ?? 'The submitted data is invalid.';

    return this.extractDetail(error) ?? fallback;
  }

  private extractDetail(error: HttpErrorResponse): string | null {
    const body = error.error as { detail?: string; title?: string } | null;
    return body?.detail ?? body?.title ?? null;
  }

  private extractValidation(error: HttpErrorResponse): string | null {
    const body = error.error as { errors?: Record<string, string[]> } | null;
    if (!body?.errors) return null;

    const messages = Object.values(body.errors).flat();
    return messages.length > 0 ? messages.join(' ') : null;
  }
}
