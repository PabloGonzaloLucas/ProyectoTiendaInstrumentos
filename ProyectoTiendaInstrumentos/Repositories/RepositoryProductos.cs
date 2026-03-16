using Microsoft.EntityFrameworkCore;
using ProyectoTiendaInstrumentos.Data;
using ProyectoTiendaInstrumentos.Helpers;
using ProyectoTiendaInstrumentos.Models;

namespace ProyectoTiendaInstrumentos.Repositories
{
    public class RepositoryProductos
    {
        private ProyectoTiendaInstrumentosContext context;
        private HelperPathProvider helper;

        public RepositoryProductos(ProyectoTiendaInstrumentosContext context, HelperPathProvider helper)
        {
            this.context = context;
            this.helper = helper;
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            var consulta = from datos in this.context.Productos
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<Producto> GetProductoAsync(int idProducto)
        {
            var consulta = from datos in this.context.Productos
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<VwDetallesProducto> GetDetallesProductoAsync(int idProducto)
        {
            var consulta = from datos in this.context.DetallesProductos
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task<List<VwEspecificacionesProducto>> GetEspecificacionesAsync(int idProducto)
        {
            var consulta = from datos in this.context.EspecificacionesProducto
                           where datos.IdProducto == idProducto
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoAsync()
        {
            var consulta = from datos in this.context.CatalogoProductos
                           select datos;
            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }
            return catalogo;
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoBySubtipoAsync(int idSubtipo)
        {
            var consulta = from datos in this.context.CatalogoProductos
                           where datos.IdSubtipo == idSubtipo
                           select datos;
            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }
            return catalogo;
        }
        public async Task<List<VwCatalogoProducto>> BuscarProductosPorNombreAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return new List<VwCatalogoProducto>();
            }

            var consulta = from datos in this.context.CatalogoProductos
                           where datos.Modelo.Contains(termino)
                              || datos.Marca.Contains(termino)
                              || datos.Tipo.Contains(termino)
                              || datos.Subtipo.Contains(termino)
                              || datos.Familia.Contains(termino)
                           select datos;

            List<VwCatalogoProducto> catalogo = await consulta.ToListAsync();

            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }

            return catalogo;
        }
        public async Task<List<ProductoImagen>> GetImagenesProductoByIdAsync(int idProducto)
        {
            var consulta = from datos in this.context.ProductosImagenes
                           where datos.IdProducto == idProducto
                           select datos;

            return await consulta.ToListAsync();
        }
        public async Task<List<string>> GetNombresImagenesProductoByIdAsync(int idProducto)
        {
            var consulta = from datos in this.context.ProductosImagenes
                           where datos.IdProducto == idProducto
                           select datos.Imagen;

            return await consulta.ToListAsync();
        }
        public async Task<Subtipo> GetSubtipoByIdAsync(int idSubtipo)
        {

            var consulta = from datos in this.context.Subtipos
                           where datos.IdSubtipo == idSubtipo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Especificacion>> GetEspecificacionesBySubtipoAsync(int idSubtipo)
        {
            var consulta = from datos in this.context.Especificaciones
                           join categoriaFiltro in this.context.CategoriaFiltros on datos.IdEspecificacion equals categoriaFiltro.IdEspecificacion
                           where categoriaFiltro.IdSubtipo == idSubtipo
                           select datos;

            List<Especificacion> specs = await consulta.ToListAsync();
            return specs;
        }

        public async Task<List<ValoresEspecificacion>> GetValoresEspecificacionesByEspecifiacionAsync(int idEspecificacion)
        {
            var consulta = from datos in this.context.ValoresEspecifiaciones
                           where datos.IdEspecificacion == idEspecificacion
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<ValoresEspecificacion>> GetValoresEspecificacionesByEspecifiacionSubtipoAsync(int idEspecificacion, int idSubtipo)
        {
            var valores = await (
                from datos in this.context.ValoresEspecifiaciones
                join categoriaFiltro in this.context.CategoriaFiltros
                    on datos.IdEspecificacion equals categoriaFiltro.IdEspecificacion
                where datos.IdEspecificacion == idEspecificacion
                   && categoriaFiltro.IdSubtipo == idSubtipo
                group datos by datos.Valor into g
                select g.First()
            ).ToListAsync();

            return valores;
        }
        public async Task<List<VwCatalogoProducto>> GetVistaCatalogoBySubtipoAndFiltroAsync(int idSubtipo, int idEspecificacion, string valor)
        {
            var consulta =
                from producto in this.context.CatalogoProductos
                join spec in this.context.EspecificacionesProducto on producto.IdProducto equals spec.IdProducto
                where producto.IdSubtipo == idSubtipo
                   && spec.IdEspecificacion == idEspecificacion
                   && spec.Valor == valor
                select producto;

            List<VwCatalogoProducto> catalogo = await consulta.Distinct().ToListAsync();
            foreach (VwCatalogoProducto producto in catalogo)
            {
                producto.Especificaciones = await this.GetEspecificacionesAsync(producto.IdProducto);
            }

            return catalogo;
        }
        public async Task DeleteProductoAsync(int idProducto)
        {
            Producto? producto = await this.context.Productos.FirstOrDefaultAsync(p => p.IdProducto == idProducto);
            if (producto == null)
            {
                return;
            }

            var imagenes = await this.context.ProductosImagenes
                .Where(i => i.IdProducto == idProducto)
                .ToListAsync();
            if (imagenes.Count > 0)
            {
                this.context.ProductosImagenes.RemoveRange(imagenes);
            }

            var valoraciones = await this.context.Valoraciones
                .Where(v => v.IdProducto == idProducto)
                .ToListAsync();
            if (valoraciones.Count > 0)
            {
                this.context.Valoraciones.RemoveRange(valoraciones);
            }

            // elimina specs asociadas
            var specs = await this.context.ValoresEspecifiaciones
                .Where(v => v.IdProducto == idProducto)
                .ToListAsync();
            if (specs.Count > 0)
            {
                this.context.ValoresEspecifiaciones.RemoveRange(specs);
            }
            await this.context.SaveChangesAsync();
            this.context.Productos.Remove(producto);
            await this.context.SaveChangesAsync();
        }

        public async Task<SelectSubtiposProducto> GetArbolTiposAsync(int idSubtipo)
        {
            SelectSubtiposProducto selectSubtiposProducto = new SelectSubtiposProducto();
            if (idSubtipo != null)
            {
                selectSubtiposProducto.IdSubtipo = idSubtipo;
                Subtipo subtipo = await this.context.Subtipos.FindAsync(idSubtipo);
                if (subtipo != null)
                {
                    selectSubtiposProducto.NombreSubtipo = subtipo.Nombre;
                    Tipo tipo = await this.context.Tipos.FindAsync(subtipo.IdTipo);
                    selectSubtiposProducto.IdTipo = tipo.IdTipo;
                    selectSubtiposProducto.NombreTipo = tipo.Nombre;
                    Familia familia = await this.context.Familias.FindAsync(tipo.IdFamilia);
                    selectSubtiposProducto.IdFamilia = familia.IdFamilia;
                    selectSubtiposProducto.NombreFamilia = familia.Nombre;
                }
            }
            return selectSubtiposProducto;
        }

        public async Task<List<Marca>> GetMarcasAsync()
        {
            return await this.context.Marcas.ToListAsync();
        }

        // --- CREATE PRODUCTO ---
        public async Task<int> InsertProductoAsync(
            string modelo,
            int idMarca,
            decimal precio,
            int stock,
            int idSubtipo,
            string descripcion,
            IFormFile? imagenPrincipal,
            List<IFormFile>? imagenesSecundarias,
            List<int>? specIds,
            List<string>? specValores)
        {
            int newIdProducto = 1;
            if (await this.context.Productos.AnyAsync())
            {
                newIdProducto = await this.context.Productos.MaxAsync(p => p.IdProducto) + 1;
            }

            Producto producto = new Producto
            {
                IdProducto = newIdProducto,
                Modelo = modelo,
                IdMarca = idMarca,
                Precio = precio,
                Stock = stock,
                IdSubtipo = idSubtipo,
                Descripcion = descripcion,
                Puntuacion = 5,
                Ventas = 0,
                FechaCreacion = DateTime.UtcNow
            };

            // Imagen principal -> images/productosMain
            if (imagenPrincipal != null && imagenPrincipal.Length > 0)
            {
                // Para evitar colisiones, prefijamos con el IdProducto
                string fileName = $"{newIdProducto}_main_{Path.GetFileName(imagenPrincipal.FileName)}";
                string path = this.helper.MapPath(fileName, Folders.ProductosMain);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagenPrincipal.CopyToAsync(stream);
                }
                producto.Imagen = fileName;
            }
            else
            {
                producto.Imagen = "default.png";
            }

            await this.context.Productos.AddAsync(producto);
            await this.context.SaveChangesAsync();

            // Imágenes secundarias -> images/productos
            if (imagenesSecundarias != null && imagenesSecundarias.Count > 0)
            {
                int newIdProductoImagen = 1;
                if (await this.context.ProductosImagenes.AnyAsync())
                {
                    newIdProductoImagen = await this.context.ProductosImagenes.MaxAsync(i => i.IdProductoImagen) + 1;
                }

                foreach (IFormFile img in imagenesSecundarias)
                {
                    if (img == null || img.Length == 0) continue;

                    string fileName = $"{newIdProducto}_{Guid.NewGuid():N}_{Path.GetFileName(img.FileName)}";
                    string path = this.helper.MapPath(fileName, Folders.Productos);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    ProductoImagen productoImagen = new ProductoImagen
                    {
                        IdProductoImagen = newIdProductoImagen++,
                        IdProducto = newIdProducto,
                        Imagen = fileName
                    };

                    await this.context.ProductosImagenes.AddAsync(productoImagen);
                }

                await this.context.SaveChangesAsync();
            }

            // Especificaciones -> ValoresEspecificaciones
            if (specIds != null && specValores != null && specIds.Count > 0 && specIds.Count == specValores.Count)
            {
                int newIdValor = 1;
                if (await this.context.ValoresEspecifiaciones.AnyAsync())
                {
                    newIdValor = await this.context.ValoresEspecifiaciones.MaxAsync(v => v.Id) + 1;
                }

                for (int i = 0; i < specIds.Count; i++)
                {
                    ValoresEspecificacion ve = new ValoresEspecificacion
                    {
                        Id = newIdValor++,
                        IdProducto = newIdProducto,
                        IdEspecificacion = specIds[i],
                        Valor = specValores[i]
                    };

                    await this.context.ValoresEspecifiaciones.AddAsync(ve);
                }

                await this.context.SaveChangesAsync();
            }

            return newIdProducto;
        }

        public async Task<int> InsertEspecificacionAsync(string nombre, string? unidadMedida, int idSubtipo)
        {
            // 1) Crear la especificación
            int newIdEspecificacion = 1;
            if (await this.context.Especificaciones.AnyAsync())
            {
                newIdEspecificacion = await this.context.Especificaciones.MaxAsync(e => e.IdEspecificacion) + 1;
            }


            string medida;
            if (unidadMedida != "ud")
            {
                medida = null;
            }
            else
            {
                medida = "ud";
            }
            Especificacion espec = new Especificacion
            {
                IdEspecificacion = newIdEspecificacion,
                Nombre = nombre,
                UnidadMedida = medida
            };

            await this.context.Especificaciones.AddAsync(espec);
            await this.context.SaveChangesAsync();

            // 2) Crear la relación (CategoriaFiltro) con el id creado y el subtipo seleccionado
            // (PK compuesta: IdSubtipo + IdEspecificacion)
            bool exists = await this.context.CategoriaFiltros.AnyAsync(cf =>
                cf.IdSubtipo == idSubtipo && cf.IdEspecificacion == newIdEspecificacion);

            if (!exists)
            {
                CategoriaFiltro categoriaFiltro = new CategoriaFiltro
                {
                    IdSubtipo = idSubtipo,
                    IdEspecificacion = newIdEspecificacion
                };

                await this.context.CategoriaFiltros.AddAsync(categoriaFiltro);
                await this.context.SaveChangesAsync();
            }

            return newIdEspecificacion;
        }

        public async Task UpdateProductoAsync(
            int idProducto,
            string modelo,
            int idMarca,
            decimal precio,
            int stock,
            int idSubtipo,
            string descripcion,
            IFormFile? imagenPrincipal,
            List<IFormFile>? imagenesSecundarias,
            List<int>? imagenesExistentesIds,
            List<int>? specIds,
            List<string>? specValores)
        {
            Producto? producto = await this.context.Productos.FirstOrDefaultAsync(p => p.IdProducto == idProducto);
            if (producto == null)
            {
                return;
            }

            // Actualiza campos base
            producto.Modelo = modelo;
            producto.IdMarca = idMarca;
            producto.Precio = precio;
            producto.Stock = stock;
            producto.IdSubtipo = idSubtipo;
            producto.Descripcion = descripcion;

            // Imagen principal (si viene nueva)
            if (imagenPrincipal != null && imagenPrincipal.Length > 0)
            {
                string fileName = $"{idProducto}_main_{Path.GetFileName(imagenPrincipal.FileName)}";
                string path = this.helper.MapPath(fileName, Folders.ProductosMain);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagenPrincipal.CopyToAsync(stream);
                }
                producto.Imagen = fileName;
            }

            await this.context.SaveChangesAsync();

            // --- IMÁGENES SECUNDARIAS: sincronizar (eliminar faltantes + añadir nuevas) ---
            var actuales = await this.context.ProductosImagenes
                .Where(i => i.IdProducto == idProducto)
                .ToListAsync();

            var keepIds = (imagenesExistentesIds ?? new List<int>()).Distinct().ToHashSet();

            // Elimina las que existían en BD pero ya no vienen en el form
            var aEliminar = actuales
                .Where(i => i.IdProductoImagen != 0 && !keepIds.Contains(i.IdProductoImagen))
                .ToList();

            if (aEliminar.Count > 0)
            {
                this.context.ProductosImagenes.RemoveRange(aEliminar);
                await this.context.SaveChangesAsync();
            }

            // Añade nuevas IFormFiles
            if (imagenesSecundarias != null && imagenesSecundarias.Count > 0)
            {
                int newIdProductoImagen = 1;
                if (await this.context.ProductosImagenes.AnyAsync())
                {
                    newIdProductoImagen = await this.context.ProductosImagenes.MaxAsync(i => i.IdProductoImagen) + 1;
                }

                foreach (IFormFile img in imagenesSecundarias)
                {
                    if (img == null || img.Length == 0) continue;

                    string fileName = $"{idProducto}_{Guid.NewGuid():N}_{Path.GetFileName(img.FileName)}";
                    string path = this.helper.MapPath(fileName, Folders.Productos);
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    ProductoImagen productoImagen = new ProductoImagen
                    {
                        IdProductoImagen = newIdProductoImagen++,
                        IdProducto = idProducto,
                        Imagen = fileName
                    };

                    await this.context.ProductosImagenes.AddAsync(productoImagen);
                }

                await this.context.SaveChangesAsync();
            }

            // Especificaciones: si se mandan, reemplazamos las existentes del producto
            if (specIds != null && specValores != null && specIds.Count > 0 && specIds.Count == specValores.Count)
            {
                var existentes = await this.context.ValoresEspecifiaciones
                    .Where(v => v.IdProducto == idProducto)
                    .ToListAsync();

                if (existentes.Count > 0)
                {
                    this.context.ValoresEspecifiaciones.RemoveRange(existentes);
                    await this.context.SaveChangesAsync();
                }

                int newIdValor = 1;
                if (await this.context.ValoresEspecifiaciones.AnyAsync())
                {
                    newIdValor = await this.context.ValoresEspecifiaciones.MaxAsync(v => v.Id) + 1;
                }

                for (int i = 0; i < specIds.Count; i++)
                {
                    ValoresEspecificacion ve = new ValoresEspecificacion
                    {
                        Id = newIdValor++,
                        IdProducto = idProducto,
                        IdEspecificacion = specIds[i],
                        Valor = specValores[i]
                    };

                    await this.context.ValoresEspecifiaciones.AddAsync(ve);
                }

                await this.context.SaveChangesAsync();
            }
        }

        public async Task<Especificacion?> GetEspecificacionByIdAsync(int idEspecificacion)
        {
            return await this.context.Especificaciones
                .FirstOrDefaultAsync(e => e.IdEspecificacion == idEspecificacion);
        }

        public async Task UpdateEspecificacionAsync(int idEspecificacion, string nombre, string? unidadMedida, int idSubtipo)
        {
            Especificacion? espec = await this.context.Especificaciones
                .FirstOrDefaultAsync(e => e.IdEspecificacion == idEspecificacion);
            if (espec == null)
            {
                return;
            }

            // Normaliza unidad de medida igual que InsertEspecificacionAsync
            string? medida = unidadMedida == "ud" ? "ud" : null;

            espec.Nombre = nombre;
            espec.UnidadMedida = medida;
            await this.context.SaveChangesAsync();

            // Actualiza la relación CategoriaFiltro (mover la spec al subtipo seleccionado)
            var relaciones = await this.context.CategoriaFiltros
                .Where(cf => cf.IdEspecificacion == idEspecificacion)
                .ToListAsync();

            // Si ya existe la relación con el subtipo elegido, eliminamos el resto
            bool existeRelacion = relaciones.Any(r => r.IdSubtipo == idSubtipo);

            if (existeRelacion)
            {
                var aBorrar = relaciones.Where(r => r.IdSubtipo != idSubtipo).ToList();
                if (aBorrar.Count > 0)
                {
                    this.context.CategoriaFiltros.RemoveRange(aBorrar);
                    await this.context.SaveChangesAsync();
                }
            }
            else
            {
                if (relaciones.Count > 0)
                {
                    this.context.CategoriaFiltros.RemoveRange(relaciones);
                    await this.context.SaveChangesAsync();
                }

                CategoriaFiltro nueva = new CategoriaFiltro
                {
                    IdSubtipo = idSubtipo,
                    IdEspecificacion = idEspecificacion
                };

                await this.context.CategoriaFiltros.AddAsync(nueva);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<bool> TieneComprasAsync(int idProducto)
        {
            return await this.context.DetallePedidos.AnyAsync(d => d.IdProducto == idProducto);
        }


        public async Task InsertMarcaAsync(string nombre, IFormFile logo)
        {
            Marca marca = new Marca();
            marca.Nombre = nombre;
            marca.IdMarca = await this.context.Marcas.MaxAsync(f => f.IdMarca) + 1;
            marca.Puntuacion = 5;
            if (logo != null && logo.Length > 0)
            {
                string fileName = logo.FileName;
                string path = this.helper.MapPath(fileName, Folders.Marcas);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }
                marca.Logo = logo.FileName;
            }
            else
            {
                marca.Logo = "default.png";
            }
            await this.context.Marcas.AddAsync(marca);
            await this.context.SaveChangesAsync();
        }

        public async Task<Marca> GetMarcaAsync(int idMarca)
        {
            return await this.context.Marcas.FindAsync(idMarca);
        }

        public async Task UpdateMarcaAsync(int idMarca, string nombre, IFormFile? logo)
        {
            Marca? marca = await this.context.Marcas.FindAsync(idMarca);
            if (marca == null)
            {
                return;
            }
            marca.Nombre = nombre;
            if (logo != null && logo.Length > 0)
            {
                string fileName = logo.FileName;
                string path = this.helper.MapPath(fileName, Folders.Marcas);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await logo.CopyToAsync(stream);
                }
                marca.Logo = logo.FileName;
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> TieneProductosAsync(int idMarca)
        {
            return await this.context.Productos.AnyAsync(p => p.IdMarca == idMarca);
        }

        public async Task DeleteMarcaAsync(int idMarca)
        {
            Marca? marca = await this.context.Marcas.FindAsync(idMarca);
            if (marca == null)
            {
                return;
            }
            this.context.Marcas.Remove(marca);
            await this.context.SaveChangesAsync();
        }
    }
}
