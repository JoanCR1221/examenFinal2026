using System;
using System.Threading.Tasks;
using LibraryService.WebAPI.DTO;
using LibraryService.WebAPI.Entities;
using LibraryService.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FraudController : ControllerBase
    {
        private readonly IFraudService _fraudService;

        public FraudController(IFraudService fraudService)
        {
            _fraudService = fraudService;
        }

        /// <summary>Consulta todos los reportes de fraude registrados.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var frauds = await _fraudService.GetAll();
            return Ok(frauds);
        }

        /// <summary>Registra un nuevo reporte de fraude.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(FraudForm form)
        {
            // [ApiController] responde 400 automaticamente cuando el modelo es
            // invalido; esta validacion extra evita envios con solo espacios.
            if (string.IsNullOrWhiteSpace(form.ImpostorDetails) ||
                string.IsNullOrWhiteSpace(form.ContactInfo))
            {
                return BadRequest(new
                {
                    error = "Los detalles del impostor y la informacion de contacto son obligatorios."
                });
            }

            var fraud = new Fraud
            {
                ImpostorDetails = form.ImpostorDetails.Trim(),
                ContactInfo = form.ContactInfo.Trim(),
                Comments = string.IsNullOrWhiteSpace(form.Comments) ? null : form.Comments.Trim()
            };

            var created = await _fraudService.Add(fraud);

            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
