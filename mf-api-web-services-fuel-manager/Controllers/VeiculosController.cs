﻿using mf_api_web_services_fuel_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mf_api_web_services_fuel_manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        
        private readonly AppDbContext _context;

        public VeiculosController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Veiculos.ToListAsync();
            return Ok(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(Veiculo model)
        {

            if (model.AnoFabricacao <= 0 || model.AnoFabricacao <= 0)
            {
                return BadRequest(new { message = "Ano de Faabricação e Ano do Moddelo devem ser maios do que zero" });
            }
            _context.Veiculos.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = model.Id }, model);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById (int id) 
        {
            var model = await _context.Veiculos.Include(t => t.Consumos).
                FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) return NotFound ();

            GerarLinks(model);

            return Ok(model);
           
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update (int id, Veiculo model)
        {
            if (id != model.Id) return BadRequest();

            var modelDb = await _context.Veiculos.AsNoTracking()
                .FirstOrDefaultAsync( c => c.Id == id);

            if (modelDb == null) return NotFound ();   

            _context.Veiculos.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        [HttpDelete("{id}")]

        public async Task<ActionResult>Delete (int id)
        {
            var model = await _context.Veiculos.FindAsync(id);

            if (model == null) return NotFound ();

            _context.Veiculos.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        private void GerarLinks(Veiculo model) 
        {
            model.Links.Add(new LinkDto(model.Id,Url.ActionLink() , rel: "self", metodo: "GET"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "update", metodo: "PUT"));
            model.Links.Add(new LinkDto(model.Id, Url.ActionLink(), rel: "delete", metodo: "DELETE"));
        }
    }
}
