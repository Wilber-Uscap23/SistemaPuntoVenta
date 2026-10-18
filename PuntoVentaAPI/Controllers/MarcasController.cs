
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaAPI.Data;
using PuntoVentaAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace PuntoVentaAPI.Controllers;

public class CrearMarcaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Descripcion { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class MarcasController : ControllerBase
{
    private readonly PuntoVentaDbContext _context;

    public MarcasController(PuntoVentaDbContext context)
    {
        _context = context;
    }

    // GET: api/Marcas
    [HttpGet]
    public async Task<IActionResult> ObtenerMarcas()
    {
        var marcas = await _context.Marcas
            .AsNoTracking()
            .Select(m => new
            {
                m.IdMarca,
                m.Nombre,
                m.Descripcion,
                m.Estado
            })
            .ToListAsync();

        return Ok(marcas);
    }

    // POST: api/Marcas
    [HttpPost]
    public async Task<IActionResult> RegistrarMarca(
        [FromBody] CrearMarcaDto datos)
    {
        string nombre = datos.Nombre.Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            return BadRequest(new
            {
                mensaje = "El nombre es obligatorio."
            });
        }

        bool existe = await _context.Marcas
            .AnyAsync(m => m.Nombre == nombre);

        if (existe)
        {
            return Conflict(new
            {
                mensaje = "Esta marca ya está registrada."
            });
        }

        var marca = new Marca
        {
            Nombre = nombre,
            Descripcion = datos.Descripcion,
            Estado = true
        };

        _context.Marcas.Add(marca);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Marca registrada correctamente.",
            marca.IdMarca,
            marca.Nombre,
            marca.Descripcion,
            marca.Estado
        });
    }
}
