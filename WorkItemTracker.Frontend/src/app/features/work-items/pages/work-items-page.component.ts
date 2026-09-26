import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  BehaviorSubject,
  Subject,
  catchError,
  debounceTime,
  distinctUntilChanged,
  finalize,
  of,
  startWith,
  switchMap,
  tap
} from 'rxjs';
import { ApiErrorService } from '../../../core/services/api-error.service';
import { WorkItemsApiService } from '../../../core/services/work-items-api.service';
import { PagedResult, WorkItem, WorkItemStatus } from '../../../core/models/work-item.model';


@Component({
  selector: 'app-work-items-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './work-items-page.component.html',
  styleUrl: './work-items-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class WorkItemsPageComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(WorkItemsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cdr = inject(ChangeDetectorRef);

  readonly statuses: WorkItemStatus[] = ['Todo', 'InProgress', 'Done'];
  readonly pageSizes = [5, 10, 20, 50];

  readonly createForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(120)]],
    description: ['']
  });

  readonly filterForm = this.fb.nonNullable.group({
    title: [''],
    status: ['' as '' | WorkItemStatus],
    pageSize: [10]
  });

  private readonly page$ = new BehaviorSubject<number>(1);
  private readonly refresh$ = new Subject<void>();

  workItems: WorkItem[] = [];
  currentPage = 1;
  totalPages = 0;
  totalCount = 0;
  pageSize = 10;
  loading = false;

  constructor() {
    this.bindSearch();
  }

  private bindSearch(): void {
    this.refresh$.pipe(
      startWith(undefined),
      switchMap(() => this.filterForm.valueChanges.pipe(
        startWith(this.filterForm.getRawValue()),
        debounceTime(300),
        distinctUntilChanged((a, b) =>
          (a.title ?? '').trim() === (b.title ?? '').trim() && a.status === b.status && a.pageSize === b.pageSize
        ),
        tap(() => this.page$.next(1)),
        switchMap(() => this.page$)
      )),
      tap(() => { this.loading = true; this.cdr.markForCheck(); }),
      switchMap(page => this.api.search({
        Title: this.filterForm.controls.title.value.trim() || undefined,
        Status: this.filterForm.controls.status.value || undefined,
        Page: page,
        PageSize: this.filterForm.controls.pageSize.value
      }).pipe(
        catchError(error => of({
          items: [],
          page,
          pageSize: this.filterForm.controls.pageSize.value,
          totalCount: 0,
          totalPages: 0,
          __error: this.errors.getMessage(error, 'Unable to load work items.')
        } as PagedResult<WorkItem> & { __error: string }))
      )),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(result => {
      this.loading = false;
      const withError = result as PagedResult<WorkItem> & { __error?: string };
      if (withError.__error) {
        this.loadError = withError.__error;
        this.workItems = [];
        this.totalPages = 0;
        this.totalCount = 0;
      } else {
        this.loadError = null;
        this.workItems = result.items;
        this.currentPage = result.page;
        this.pageSize = result.pageSize;
        this.totalPages = result.totalPages;
        this.totalCount = result.totalCount;
      }
      this.cdr.markForCheck();
    });
  }

  loadError: string | null = null;
  creating = false;
  updatingId: number | null = null;
  createError: string | null = null;
  actionError: string | null = null;
  createdMessage: string | null = null;

  onCreate(): void {
    this.createError = null;
    this.createdMessage = null;

    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const value = this.createForm.getRawValue();
    this.creating = true;

    this.api.create({
      title: value.title.trim(),
      description: value.description.trim() || null
    }).pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.creating = false)
    ).subscribe({
      next: () => {
        this.createForm.reset({ title: '', description: '' });
        this.createdMessage = 'Work item created successfully.';
        this.refresh$.next();
      },
      error: error => this.createError = this.errors.getMessage(error, 'Unable to create the work item.')
    });
  }

  onPage(page: number): void {
    const current = this.currentPage;
    if (page < 1 || page > this.totalPages || page === current) return;
    this.page$.next(page);
  }

  onAdvance(item: WorkItem): void {
    const nextStatus = this.nextStatus(item.status);
    if (!nextStatus) return;

    this.actionError = null;
    this.updatingId = item.id;

    this.api.changeStatus(item.id, { status: nextStatus }).pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.updatingId = null)
    ).subscribe({
      next: () => this.refresh$.next(),
      error: error => this.actionError = this.errors.getMessage(error, 'Unable to update the work item status.')
    });
  }

  retry(): void {
    this.refresh$.next();
  }

  clearFilters(): void {
    this.filterForm.setValue({ title: '', status: '', pageSize: 10 });
    this.page$.next(1);
  }

  nextStatus(status: WorkItemStatus): WorkItemStatus | null {
    if (status === 'Todo') return 'InProgress';
    if (status === 'InProgress') return 'Done';
    return null;
  }

  statusLabel(status: WorkItemStatus): string {
    return status === 'InProgress' ? 'In Progress' : status;
  }

  trackById(_: number, item: WorkItem): number { return item.id; }
}
