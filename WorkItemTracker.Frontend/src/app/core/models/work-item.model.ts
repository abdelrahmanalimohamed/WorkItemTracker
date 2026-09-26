export type WorkItemStatus = 'Todo' | 'InProgress' | 'Done';

export interface WorkItem {
  id: number;
  title: string;
  description?: string | null;
  status: WorkItemStatus;
  createdAt: string;
}

export interface CreateWorkItemRequest {
  title: string;
  description?: string | null;
}

export interface ChangeWorkItemStatusRequest {
  status: WorkItemStatus;
}

export interface WorkItemSearchRequest {
  Title?: string;
  Status?: WorkItemStatus;
  Page: number;
  PageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
