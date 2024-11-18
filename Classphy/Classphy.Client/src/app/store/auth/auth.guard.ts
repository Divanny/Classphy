import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, from, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from '../../services/api.service';
import { AuthState } from './auth.state';
import { login, logout } from './auth.actions';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private store: Store<{ auth: AuthState }>,
    private router: Router,
    private apiService: ApiService
  ) {}

  canActivate(): Observable<boolean> {
    const token = localStorage.getItem('token');
    if (!token) {
      this.router.navigate(['/sign-in']);
      return of(false);
    }

    return from(this.apiService.api.get('/account')).pipe(
      map((response: any) => {
        if (response.data) {
          const { data } = response;
          this.store.dispatch(login({ user: data, token }));
          return true;
        } else {
          this.store.dispatch(logout());
          this.router.navigate(['/sign-in']);
          return false;
        }
      }),
      catchError((error) => {
        this.router.navigate(['/sign-in']);
        return of(false);
      })
    );
  }
}
