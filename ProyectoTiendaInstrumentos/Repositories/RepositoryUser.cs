using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryUser
    {
        private ProyectoTiendaInstrumentosContext context;
        private HelperPathProvider helper;
        public RepositoryUser(ProyectoTiendaInstrumentosContext context, HelperPathProvider helper)
        {
            this.context = context;
            this.helper = helper;
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
        public async Task<Usuario> GetUserByIdAsync(int idUsuario)
        {
            var consulta = from datos in this.context.Usuarios
                           where datos.IdUsuario == idUsuario 
                           select datos;
            Usuario user = await consulta.FirstOrDefaultAsync();
            return user;
        }

        public async Task RegisterUserAsync(string nombre, string email, IFormFile imagen, string password, string telefono, string direccion)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.Nombre = nombre;

            if (imagen != null && imagen.Length > 0)
            {
                string fileName = imagen.FileName;
                string path = this.helper.MapPath(fileName, Folders.Users);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                user.Imagen = imagen.FileName;
            }
            else
            {
                user.Imagen = "default.png";
            }

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
        public async Task RegisterUserFakePassAsync(string nombre, string email, IFormFile imagen, string password, string telefono, string direccion)
        {
            Usuario user = new Usuario();
            user.IdUsuario = await this.GetMaxIdUsuarioAsync();
            user.Nombre = nombre;

            if (imagen != null && imagen.Length > 0)
            {
                string fileName = imagen.FileName;
                string path = this.helper.MapPath(fileName, Folders.Users);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                user.Imagen = imagen.FileName;
            }
            else
            {
                user.Imagen = "default.png";
            }

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
            await this.context. Usuarios.AddAsync(user);
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

        public async Task<bool> VerificarPasswordActualAsync(int idUsuario, string passwordActual)
        {
            SeguridadUsuario seguridad = await this.context.SeguridadUsuarios
                .FirstOrDefaultAsync(s => s.IdUsuario == idUsuario);

            if (seguridad == null)
            {
                return false;
            }

            byte[] passwordEncriptada = HelperCryptography.EncryptPassword(passwordActual, seguridad.Salt);
            return HelperTools.CompareArrays(passwordEncriptada, seguridad.Password);
        }

        public async Task<bool> CambiarPasswordAsync(int idUsuario, string passwordNueva)
        {
            SeguridadUsuario seguridad = await this.context.SeguridadUsuarios
                .FirstOrDefaultAsync(s => s.IdUsuario == idUsuario);

            if (seguridad == null)
            {
                return false;
            }

            string nuevoSalt = HelperTools.GenerateSalt();
            seguridad.Salt = nuevoSalt;
            seguridad.Password = HelperCryptography.EncryptPassword(passwordNueva, nuevoSalt);

            this.context.SeguridadUsuarios.Update(seguridad);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserImageAsync(int idUsuario, IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return false;
            }

            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (user == null)
            {
                return false;
            }

            string fileName = Path.GetFileName(imagen.FileName);
            string path = this.helper.MapPath(fileName, Folders.Users);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            await using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            user.Imagen = fileName;
            this.context.Usuarios.Update(user);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserProfileAsync(int idUsuario, string nombre, string direccion, string telefono)
        {
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (user == null)
            {
                return false;
            }

            user.Nombre = nombre;
            user.Direccion = direccion;
            user.Telefono = telefono;

            this.context.Usuarios.Update(user);
            await this.context.SaveChangesAsync();
            return true;
        }
    }
}
