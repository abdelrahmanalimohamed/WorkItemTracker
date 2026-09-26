import { Routes } from '@angular/router';
import { WorkItemsPageComponent } from './features/work-items/pages/work-items-page.component';

export const routes: Routes = [
  { path: '', component: WorkItemsPageComponent },
  { path: '**', redirectTo: '' }
];
