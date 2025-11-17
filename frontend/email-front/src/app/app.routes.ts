import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { EmailsListComponent } from './emails/emails-list/emails-list.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'emails',
    component: EmailsListComponent
  }
];
