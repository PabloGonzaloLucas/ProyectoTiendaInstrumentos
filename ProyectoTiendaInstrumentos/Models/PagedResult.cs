namespace ProyectoTiendaInstrumentos.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public int TamañoPagina { get; set; }

        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        public static PagedResult<T> Create(List<T> source, int pagina, int tamañoPagina)
        {
            int total = source.Count;
            int totalPaginas = (int)Math.Ceiling(total / (double)tamañoPagina);
            pagina = Math.Max(1, Math.Min(pagina, Math.Max(1, totalPaginas)));

            List<T> items = source
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            return new PagedResult<T>
            {
                Items = items,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                TotalItems = total,
                TamañoPagina = tamañoPagina
            };
        }
    }
}
