import { createReducer, on } from '@ngrx/store';
import { login, logout } from './auth.actions';
import { AuthState, initialAuthState } from './auth.state';

export const authReducer = createReducer(
  initialAuthState,
  on(login, (state, { user, token }) => ({
    ...state,
    user,
    token,
  })),
  on(logout, (state) => ({
    ...state,
    user: null,
    token: null,
  }))
);