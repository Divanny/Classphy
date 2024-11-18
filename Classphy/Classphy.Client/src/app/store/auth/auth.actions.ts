import { createAction, props } from '@ngrx/store';

// Acción para iniciar sesión
export const login = createAction(
  '[Auth] Login',
  props<{
    user: {
      idUsuario: number;
      idPerfil: number;
      nombrePerfil: string | null;
      nombreUsuario: string;
      correoElectronico: string;
      contraseñaHashed: string | null;
      contraseña: string | null;
      fechaRegistro: string;
      activo: boolean;
    };
    token: string;
  }>()
);

// Acción para cerrar sesión
export const logout = createAction('[Auth] Logout');
