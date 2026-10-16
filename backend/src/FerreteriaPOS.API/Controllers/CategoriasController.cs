using FerreteriaPOS.Domain.Entities;
using FerreteriaPOS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerreteriaPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(
        ApplicationDbContext context,
        ILogger<CategoriasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las categorías activas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCategorias()
    {
        try
        {
            var categorias = await _context.Categorias
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.CategoriaPadreId,
                    c.CreadoEn
                })
                .ToListAsync();

            return Ok(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una categoría por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCategoria(Guid id)
    {
        try
        {
            var categoria = await _context.Categorias
                .Where(c => c.Id == id && c.Activo)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.CategoriaPadreId,
                    c.CreadoEn,
                    c.ModificadoEn
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
                return NotFound(new { error = "Categoría no encontrada" });

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría {Id}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<object>> CreateCategoria([FromBody] CreateCategoriaDto dto)
    {
        try
        {
            var categoria = Categoria.Crear(dto.Nombre, dto.Descripcion, dto.CategoriaPadreId);

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var resultado = new
            {
                categoria.Id,
                categoria.Nombre,
                categoria.Descripcion,
                categoria.CategoriaPadreId,
                categoria.CreadoEn
            };

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.Id },
                resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}

public record CreateCategoriaDto(
    string Nombre,
    string? Descripcion = null,
    Guid? CategoriaPadreId = null);
