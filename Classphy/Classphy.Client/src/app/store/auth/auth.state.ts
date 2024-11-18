// auth.state.ts
export interface AuthState {
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
    } | null;
    token: string | null;
  }
  
  export const initialAuthState: AuthState = {
    user: null,
    token: null,
  };
  