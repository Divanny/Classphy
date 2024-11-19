import { Injectable } from '@angular/core';
import axios, { AxiosInstance } from 'axios';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private axiosInstance: AxiosInstance;

  constructor(private router: Router) {
    this.axiosInstance = axios.create({
      baseURL: `${window.location.origin}/api`,
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    this.axiosInstance.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response) {
          const { status } = error.response;

          if (status === 401) {
            console.log(
              'Sesión expirada. Por favor, inicia sesión nuevamente.'
            );
            localStorage.removeItem('token');
            this.router.navigate(['/sign-in']);
          }
          else if (status === 403) {
            this.router.navigate(['/']);
            console.error('No tienes permisos para realizar esta acción');
          }
          else {
            console.error('Operación fallida', error.message);
          }
        } else {
          console.error('Error de red', error.message);
        }
        return Promise.reject(error);
      }
    );
  }

  get api() {
    return this.axiosInstance;
  }
}
