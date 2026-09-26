import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ChangeWorkItemStatusRequest,
  CreateWorkItemRequest,
  PagedResult,
  WorkItem,
  WorkItemSearchRequest
} from '../models/work-item.model';

@Injectable({ providedIn: 'root' })
export class WorkItemsApiService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/work-items`;

  create(request: CreateWorkItemRequest): Observable<WorkItem> {
    return this.http.post<WorkItem>(this.endpoint, request);
  }

  search(request: WorkItemSearchRequest): Observable<PagedResult<WorkItem>> {
    let params = new HttpParams()
      .set('Page', request.Page)
      .set('PageSize', request.PageSize);

    if (request.Title?.trim()) {
      params = params.set('Title', request.Title.trim());
    }

    if (request.Status) {
      params = params.set('Status', request.Status);
    }

    return this.http.get<PagedResult<WorkItem>>(this.endpoint, { params });
  }

  getById(id: number): Observable<WorkItem> {
    return this.http.get<WorkItem>(`${this.endpoint}/${id}`);
  }

  changeStatus(id: number, status: ChangeWorkItemStatusRequest): Observable<WorkItem> {
    return this.http.patch<WorkItem>(`${this.endpoint}/${id}/status`, status);
  }
}
