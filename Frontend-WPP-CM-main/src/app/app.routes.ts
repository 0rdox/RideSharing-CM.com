import { Routes } from '@angular/router';
import { AboutComponent } from './ui/other/about/about.component';
import { ContactCompontent } from './ui/other/contact/contact.component';
import { LoreComponent } from './ui/other/lore/lore.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard',
  },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./views/dashboard/dashboard.module').then(
        (m) => m.DashboardModule
      ),
  },
  {
    path: 'cars',
    loadChildren: () =>
      import('./views/car/car.module').then((m) => m.CarModule),
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./views/auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: 'users',
    loadChildren: () =>
      import('./views/user/user.module').then((m) => m.UserModule),
  },
  {
    path: 'history',
    loadChildren: () =>
      import('./views/history/history.module').then((m) => m.HistoryModule),
  },
  {
    path: 'about',
    component: AboutComponent,
  },
  {
    path: 'contact',
    component: ContactCompontent,
  },
  {
    path: 'lore',
    component: LoreComponent,
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
