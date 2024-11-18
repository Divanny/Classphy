using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.idUsuario,
                NombreUsuario = u.NombreUsuario,
                CorreoElectronico = u.CorreoElectronico,
                idPerfil = u.idPerfil,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Activo = u.Activo,
                Contraseña = u.ContraseñaHashed,
                FechaRegistro = u.FechaRegistro
            }),
            (DB, filter) =>
            {
                return (from u in DB.Set<Usuarios>().Where(filter)
                        join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                        select new UsuariosModel()
                        {
                            idUsuario = u.idUsuario,
                            NombreUsuario = u.NombreUsuario,
                            CorreoElectronico = u.CorreoElectronico,
                            Nombres = u.Nombres,
                            Apellidos = u.Apellidos,
                            idPerfil = u.idPerfil,
                            NombrePerfil = p.Nombre,
                            Activo = u.Activo,
                            FechaRegistro = u.FechaRegistro,
                            ContraseñaHashed = u.Contraseña
                        });
            }
        )
        {

        }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            var usuarioModel = this.Get(x => x.NombreUsuario == nombreUsuario).FirstOrDefault();

            if (usuarioModel == null) return new UsuariosModel();

            usuarioModel.ContraseñaHashed = null;
            return this.Get(x => x.NombreUsuario == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.idUsuario == id).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }
        public override Usuarios Add(UsuariosModel model)
        {
            try
            {
                var result = base.Add(model);
                return result;
            }
            catch (Exception E)
            {
                throw;
            }
        }

        public override void Edit(UsuariosModel model)
        {
            try
            {
                var usuario = this.Get(model.idUsuario);

                if (usuario == null) throw new Exception("El usuario no se ha encontrado");

                if (usuario.NombreUsuario != model.NombreUsuario && this.Any(x => x.NombreUsuario == model.NombreUsuario)) throw new Exception("Este usuario ya existe en el sistema");
                if (usuario.CorreoElectronico != model.CorreoElectronico && this.Any(x => x.CorreoElectronico == model.CorreoElectronico)) throw new Exception("Este correo electrónico ya está registrado");

                if (usuario.idPerfil != model.idPerfil)
                {
                    var perfil = dbContext.Set<Perfiles>().Find(model.idPerfil);
                    if (perfil == null) throw new Exception("Este perfil no se ha encontrado");
                }

                usuario.NombreUsuario = model.NombreUsuario;
                usuario.CorreoElectronico = model.CorreoElectronico;
                usuario.idPerfil = model.idPerfil;
                usuario.Activo = model.Activo;

                base.Edit(usuario);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}