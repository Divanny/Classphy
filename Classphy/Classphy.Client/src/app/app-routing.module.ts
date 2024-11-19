import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { SignInComponent } from './pages/signin/signin.component';
import { UsersComponent } from './pages/users/users.component';
import { AuthGuard } from './store/auth/auth.guard';
import { LayoutComponent } from './layouts/layout/layout.component';
import { SubjectsComponent } from './pages/subjects/subjects.component';
import { StudentsComponent } from './pages/students/students.component';

const routes: Routes = [
  { path: 'sign-in', component: SignInComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', component: HomeComponent },
      { path: 'users', component: UsersComponent },
      { path: 'subjects', component: SubjectsComponent },
      { path: 'students', component: StudentsComponent },
      { path: '**', redirectTo: '' }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}