using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryUser
    {
        private ProyectoTiendaInstrumentosContext context;
        public RepositoryUser(ProyectoTiendaInstrumentosContext context)
        {
            this.context = context;
        }
        private async Task<int> GetMaxIdUsuarioAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Usuarios.MaxAsync(x => x.IdUsuario) + 1;
            }
        }
        private async Task<Usuario> GetUserByIdAsync(int idUsuario)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.IdUsuario == idUsuario 
                           select datos;
            Usuario user = await consulta.FirstOrDefaultAsync();
            return user;
        }

        public async Task RegisterUserAsync(string nombre, string email, string imagen, string password, string telefono, string direccion)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.Nombre = nombre;
            user.Imagen = imagen;
            user.Email = email;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.FechaRegistro = DateTime.Now;
            user.RolId = 2;
            SeguridadUsuario usuarioSeguridad = new SeguridadUsuario();
            usuarioSeguridad.IdUsuario = user.IdUsuario;
            usuarioSeguridad.Salt = HelperTools.GenerateSalt();
            usuarioSeguridad.Password = HelperCryptography.EncryptPassword(password, usuarioSeguridad.Salt);
            await this.context.Usuarios.AddAsync(user);
            await this.context.SeguridadUsuarios.AddAsync(usuarioSeguridad);
            await this.context.SaveChangesAsync();
        }
        public async Task RegisterUserFakePassAsync(string nombre, string email, string imagen, string password, string telefono, string direccion)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.Nombre = nombre;
            user.Imagen = imagen;
            user.Email = email;
            user.Telefono = telefono;
            user.Direccion = direccion;
            user.FechaRegistro = DateTime.Now;
            user.PasswordFake = password;
            user.RolId = 2;
            SeguridadUsuario usuarioSeguridad = new SeguridadUsuario();
            usuarioSeguridad.IdUsuario = user.IdUsuario;
            usuarioSeguridad.Salt = HelperTools.GenerateSalt();
            usuarioSeguridad.Password = HelperCryptography.EncryptPassword(password, usuarioSeguridad.Salt);
            await this.context.Usuarios.AddAsync(user);
            await this.context.SaveChangesAsync();
            await this.context.SeguridadUsuarios.AddAsync(usuarioSeguridad);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> LogInUserAsync(string email, string password)
        {
            var consulta = from datos in this.context.ValidacionesUsuarios
                           where datos.Email == email
                           select datos;
            VwLoginUser user = await consulta.FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                string salt = user.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passBytes = user.Password;
                bool response = HelperTools.CompareArrays(temp, passBytes);
                if (response == true)
                {
                    Usuario usuario = await this.GetUserByIdAsync(user.IdUsuario);
                    return usuario;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<Usuario> LogInUserFakePassAsync(string email, string password)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.Email == email && datos.PasswordFake == password
                           select datos;
            Usuario user = await consulta.FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
            }
        }
    }
}
