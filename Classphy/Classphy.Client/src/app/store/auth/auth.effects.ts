import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { tap } from 'rxjs/operators';
import { ApiService } from '../../services/api.service';
import { login, logout } from './auth.actions';

@Injectable()
export class AuthEffects {
  constructor(private actions$: Actions, private apiService: ApiService) {}

  saveToken$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(login),
        tap(({ token }) => {
          localStorage.setItem('token', token);
        })
      ),
    { dispatch: false }
  );

  removeToken$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(logout),
        tap(() => {
          localStorage.removeItem('token');
        })
      ),
    { dispatch: false }
  );
}