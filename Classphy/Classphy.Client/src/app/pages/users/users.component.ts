import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { ApiService } from '../../services/api.service';
import { MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { Table } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    DialogModule,
    DropdownModule,
  ],
  providers: [DatePipe],
})
export class UsersComponent {
  users: {
    idUsuario: number;
    idPerfil: string;
    nombrePerfil: string;
    nombreUsuario: string;
    nombres: string;
    apellidos: string;
    correoElectronico: string;
    fechaRegistro: string;
    activo: boolean;
    contrasena: string;
  }[] = [];
  perfiles: any[] = [];
  selectedUsers: any[] = [];
  cols: any[];
  loading: boolean = true;
  searchValue: string | undefined;
  userDialog: boolean = false;
  user: any = {};
  isNewUser: boolean = false;
  errors: { [key: string]: string } = {};
  password: string = '';

  constructor(
    private apiService: ApiService,
    private messageService: MessageService,
    private datePipe: DatePipe
  ) {
    this.loadUsers();
    this.loadPerfiles();
    this.cols = [
      { field: 'idUsuario', header: 'ID' },
      { field: 'nombreUsuario', header: 'Nombre de Usuario' },
      { field: 'nombres', header: 'Nombres' },
      { field: 'apellidos', header: 'Apellidos' },
      { field: 'correoElectronico', header: 'Correo electrónico' },
      { field: 'nombrePerfil', header: 'Perfil' },
      { field: 'fechaRegistro', header: 'Fecha de registro' },
      { field: 'activo', header: 'Activo' },
    ];
  }

  async loadUsers() {
    const response = await this.apiService.api.get('/Usuarios');
    this.users = response.data;
    this.loading = false;
  }

  async loadPerfiles() {
    const response = await this.apiService.api.get('/Usuarios/GetPerfiles');
    this.perfiles = response.data;
  }

  openNew() {
    this.user = { activo: true };
    this.password = '';
    this.isNewUser = true;
    this.userDialog = true;
    this.errors = {};
  }

  editUser(userId: number) {
    this.user = this.users.find((user) => user.idUsuario === userId);
    this.isNewUser = false;
    this.userDialog = true;
  }

  hideDialog() {
    this.errors = {};
    this.userDialog = false;
  }

  async saveUser() {
    if (this.isNewUser) {
      this.user.contraseña = this.password;
      const response = await this.apiService.api.post('/usuarios', this.user);

      if (response.data.success) {
        this.messageService.add({
          severity: 'success',
          summary: 'Usuario creado',
          detail: 'El usuario ha sido creado exitosamente.',
        });
        this.loadUsers();
        this.userDialog = false;
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Error al crear usuario',
          detail: response.data.message,
        });
        this.errors = response.data.errors || {};
      }
    } else {
      const response = await this.apiService.api.put(
        `/usuarios/${this.user.idUsuario}`,
        this.user
      );
      if (response.data.success) {
        this.messageService.add({
          severity: 'success',
          summary: 'Usuario actualizado',
          detail: 'El usuario ha sido actualizado exitosamente.',
        });
        this.loadUsers();
        this.userDialog = false;
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Error al actualizar usuario',
          detail: response.data.message,
        });
        this.errors = response.data.errors || {};
      }
    }
  }

  async changePassword(userId: number, newPassword: string) {
    const response = await this.apiService.api.put(
      `/usuarios/${userId}/changePassword`,
      { password: newPassword }
    );
    if (response.data.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Contraseña actualizada',
        detail: 'La contraseña ha sido actualizada exitosamente.',
      });
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error al actualizar contraseña',
        detail: 'Ha ocurrido un error al actualizar la contraseña.',
      });
    }
  }

  clear(table: Table) {
    table.clear();
    this.searchValue = '';
  }

  formatFechaRegistro(fecha: string): string {
    return this.datePipe.transform(fecha, 'dd/MM/yyyy') || '';
  }
}
