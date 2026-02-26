using Microsoft.AspNetCore.Mvc;

namespace ProyectoTiendaInstrumentos.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Obtener cantidad de items del carrito desde la sesión
            var cartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
            return View(cartCount);
        }
    }
}