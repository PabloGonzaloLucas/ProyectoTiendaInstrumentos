using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace ProyectoTiendaInstrumentos.Helpers
{
    //Enumeracion con las carpetas que deseemos subir ficheros
    public enum Folders { Marcas, Productos, ProductosMain, Users}
    public class HelperPathProvider
    {
        private IWebHostEnvironment hostEnvironment;
        private IServer server;

        public HelperPathProvider
            (IWebHostEnvironment hostEnvironment
            , IServer server)
        {
            this.server = server;
            this.hostEnvironment = hostEnvironment;
        }

        //TENDREMOS UN METODO QUE SE ENCARGAR DE RESOLVER LA RUTA 
        //COMO STRING CUANDO RECIBAMOS EL FICHERO Y LA CARPETA
        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.Marcas)
            {
                carpeta = Path.Combine("images", "marcas");
            }
            else if (folder == Folders.Productos)
            {
                carpeta = Path.Combine("images", "productos");
            }
            else if (folder == Folders.Users)
            {
                carpeta = Path.Combine("images", "users");

            }
            else if (folder == Folders.ProductosMain)
            {
                carpeta = Path.Combine("images", "productosMain");
            }
            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);
            return path;
        }

        public string MapUrlPath(string fileName, Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.Marcas)
            {
                carpeta = "images/marcas";
            }
            else if (folder == Folders.Productos)
            {
                carpeta = "images/productos";
            }
            else if (folder == Folders.Users)
            {
                carpeta = "images/users";
            }
            else if (folder == Folders.ProductosMain)
            {
                carpeta = "images/productos";
            }
            else if (folder == Folders.Productos)
            {
                carpeta = "images/productos";
            }
            //http:localhost:999/images/productos/1.png
            //Quiero buscar la forma de recuperar la URL de nuestro Server 
            //en MVC Net Core.
            var addresses = this.server.Features.Get<IServerAddressesFeature>()?.Addresses;
            string serverUrl = addresses?.FirstOrDefault() ?? string.Empty;
            //DEVOLVEMOS LA RUTA URL
            string urlPath = serverUrl + "/" + carpeta + "/" + fileName;
            return urlPath;
        }
    }
}
