using Company.Data;
using Company.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("v1/departamentos")]
    public class DepartamentosController : ControllerBase
    {
        private CompanyContext _context;
        public DepartamentosController(CompanyContext context) {
            _context = context;

        }
//[FromServices] CompanyContext context
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var departaments = await _context.Departamentos
            .OrderBy(c => c.Id)
            .ToListAsync();
            return departaments.Count > 0  ? Ok(departaments) : NoContent();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> FindById(int id)
        {
            try
            {
                var departament = await _context.Departamentos.Where(x => x.Id == id).FirstOrDefaultAsync();
                return departament != null ? Ok(departament) : NotFound("Departamento não encontrado");
            }
            catch (Exception)
            {
                return NotFound("Departamento não encontrado");
            };
        }

        [HttpPost]
        public async Task<IActionResult> Insert(Departamentos objeto)
        {
            if(!ModelState.IsValid) {return BadRequest();}

            try
            {
                await _context.Departamentos.AddAsync(objeto);
                await _context.SaveChangesAsync();
                return Created($"v1/departamentos/{objeto.Id}","Criado com sucesso");
            }
            catch (Exception)
            {
                return BadRequest();

            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(int id, Departamentos objeto)
        {
            var departament = await _context.Departamentos.Where(x => x.Id == id).FirstOrDefaultAsync();

            if(departament == null) {
                return NotFound("Departamento não encontrado");
            }

            departament.Nome = departament.Nome != objeto.Nome ? objeto.Nome : departament.Nome;
            departament.Sigla = departament.Sigla != objeto.Sigla ? objeto.Sigla : departament.Sigla;

            try
            {
                _context.Update(departament);
                await _context.SaveChangesAsync();
                return Ok("Departamento atualizado com sucesso");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
             var departament = await _context.Departamentos.Where(x => x.Id == id).FirstOrDefaultAsync();

             if(departament == null) {
                return NotFound("Departamento não encontrado");
            }

            try
            {
                _context.Departamentos.Remove(departament);
                await _context.SaveChangesAsync();
                return Ok("Departamento Removido com sucesso");
            }
            catch (Exception)
            {

                return BadRequest();
            }

        }

    }
}
