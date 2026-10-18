
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaAPI.Data;
using PuntoVentaAPI.DTOs;
using PuntoVentaAPI.Models;

namespace PuntoVentaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly PuntoVentaDbContext _context;

    public ProductosController(PuntoVentaDbContext context)
    {
        _context = context;
    }

    // ============================================
    // GET: CONSULTAR TODOS LOS PRODUCTOS
    // ============================================

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos()
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .Select(p => new
            {
                p.IdProducto,
                p.Codigo,
                p.Descripcion,
                p.PrecioCosto,
                p.PrecioVenta,
                p.Existencia,
                p.IdMarca,
                p.ImagenUrl,
                p.Estado
            })
            .ToListAsync();

        return Ok(productos);
    }

    // ============================================
    // GET: CONSULTAR UN PRODUCTO POR ID
    // ============================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerProductoPorId(int id)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .Where(p => p.IdProducto == id)
            .Select(p => new
            {
                p.IdProducto,
                p.Codigo,
                p.Descripcion,
                p.PrecioCosto,
                p.PrecioVenta,
                p.Existencia,
                p.IdMarca,
                p.ImagenUrl,
                p.Estado
            })
            .FirstOrDefaultAsync();

        if (producto == null)
        {
            return NotFound(new
            {
                mensaje = "Producto no encontrado."
            });
        }

        return Ok(producto);
    }

    // ============================================
    // POST: REGISTRAR NUEVO PRODUCTO
    // ============================================

    [HttpPost]
    public async Task<IActionResult> RegistrarProducto(
        [FromBody] CrearProductoDto datos)
    {
        string codigo = datos.Codigo?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(codigo))
        {
            return BadRequest(new
            {
                mensaje = "El código es obligatorio."
            });
        }

        bool codigoExiste = await _context.Productos
            .AnyAsync(p => p.Codigo == codigo);

        if (codigoExiste)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un producto con ese código."
            });
        }

        bool marcaExiste = await _context.Marcas
            .AnyAsync(m => m.IdMarca == datos.IdMarca
                           && m.Estado == true);

        if (!marcaExiste)
        {
            return BadRequest(new
            {
                mensaje = "La marca indicada no existe o está inactiva."
            });
        }

        var producto = new Producto
        {
            Codigo = codigo,
            Descripcion = datos.Descripcion ?? string.Empty,
            PrecioCosto = datos.PrecioCosto,
            PrecioVenta = datos.PrecioVenta,
            Existencia = datos.Existencia,
            IdMarca = datos.IdMarca,
            ImagenUrl = datos.ImagenUrl,
            Estado = true
        };

        _context.Productos.Add(producto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObtenerProductoPorId),
            new { id = producto.IdProducto },
            new
            {
                mensaje = "Producto registrado correctamente.",
                producto.IdProducto,
                producto.Codigo
            });
    }

    // ============================================
    // PUT: ACTUALIZAR PRODUCTO EXISTENTE
    // ============================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarProducto(
        int id,
        [FromBody] ActualizarProductoDto datos)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null)
        {
            return NotFound(new
            {
                mensaje = "Producto no encontrado."
            });
        }

        string codigo = datos.Codigo?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(codigo))
        {
            return BadRequest(new
            {
                mensaje = "El código es obligatorio."
            });
        }

        bool codigoDuplicado = await _context.Productos
            .AnyAsync(p => p.Codigo == codigo &&
                           p.IdProducto != id);

        if (codigoDuplicado)
        {
            return Conflict(new
            {
                mensaje = "El código ya pertenece a otro producto."
            });
        }

        bool marcaExiste = await _context.Marcas
            .AnyAsync(m => m.IdMarca == datos.IdMarca
                           && m.Estado == true);

        if (!marcaExiste)
        {
            return BadRequest(new
            {
                mensaje = "La marca indicada no existe o está inactiva."
            });
        }

        producto.Codigo = codigo;
        producto.Descripcion = datos.Descripcion ?? string.Empty;
        producto.PrecioCosto = datos.PrecioCosto;
        producto.PrecioVenta = datos.PrecioVenta;
        producto.Existencia = datos.Existencia;
        producto.IdMarca = datos.IdMarca;
        producto.ImagenUrl = datos.ImagenUrl;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Producto actualizado correctamente.",
            producto.IdProducto,
            producto.Codigo,
            producto.Descripcion,
            producto.PrecioCosto,
            producto.PrecioVenta,
            producto.Existencia,
            producto.IdMarca,
            producto.ImagenUrl,
            producto.Estado
        });
    }

    // ============================================
    // DELETE: DESACTIVAR PRODUCTO
    // ============================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DesactivarProducto(int id)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.IdProducto == id);

        if (producto == null)
        {
            return NotFound(new
            {
                mensaje = "Producto no encontrado."
            });
        }

        if (producto.Estado == false)
        {
            return BadRequest(new
            {
                mensaje = "El producto ya está desactivado."
            });
        }

        producto.Estado = false;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Producto desactivado correctamente.",
            producto.IdProducto,
            producto.Codigo,
            estado = producto.Estado
        });
    }
}
