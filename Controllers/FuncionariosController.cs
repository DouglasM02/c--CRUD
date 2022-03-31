using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Data;
using Company.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("v1/funcionarios")]
    public class FuncionariosController : ControllerBase
    {
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> FindAll([FromServices] CompanyContext context, [FromRoute]int id) {
            var DepartamentDoestnExists = CheckDepartament(context,id);
            var funcionarios = await context.Funcionarios.Where(x => x.DepartamentosId == id).ToListAsync();

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            return funcionarios.Count > 0 ? Ok(funcionarios) : NoContent();

        }

        [HttpGet]
        [Route("{id}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> findById([FromServices] CompanyContext context, [FromRoute]int id, [FromRoute] int idFuncionario) {
            try
            {
                var funcionario = await context.Funcionarios.Where(x => x.DepartamentosId == id).Where(y => y.Id == idFuncionario).FirstOrDefaultAsync();
                return funcionario != null ? Ok(funcionario) : NotFound("Funcionário não encontrado");

            }
            catch (Exception e)
            {
                return NotFound("Funcionário não encontrado");
            }
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Insert([FromServices] CompanyContext context, [FromRoute]int id, Funcionarios funcionario) {

            var DepartamentDoestnExists = CheckDepartament(context,id);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            if(!ModelState.IsValid) {return BadRequest();}

            var func = new Funcionarios {
                Nome = funcionario.Nome,
                Foto = funcionario.Foto,
                Rg = funcionario.Rg,
                DepartamentosId = id
            };

            try
            {
                await context.AddAsync(func);
                await context.SaveChangesAsync();
                return Created($"v1/funcionarios/{id}","Criado Com Sucesso");
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPut]
        [Route("{id}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> Put([FromServices] CompanyContext context, [FromRoute]int id, [FromRoute] int idFuncionario, Funcionarios funcionario) {

            var DepartamentDoestnExists = CheckDepartament(context,id);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            var func = await context.Funcionarios.Where(x => x.DepartamentosId == id).Where(x => x.Id == idFuncionario).FirstOrDefaultAsync();

            if(func == null) {
                return NotFound("Funcionário não encontrado nesse setor");
            }

            func.Nome = func.Nome != funcionario.Nome ? funcionario.Nome : func.Nome;
            func.Foto = func.Foto != funcionario.Foto ? funcionario.Foto : func.Foto;
            func.Rg = func.Rg != funcionario.Rg ? funcionario.Rg : func.Rg;
            func.DepartamentosId = func.DepartamentosId != funcionario.DepartamentosId ? funcionario.DepartamentosId : func.DepartamentosId;

            if(CheckDepartament(context,func.DepartamentosId)) {
                return NotFound("Departamento não existe, por favor escolha um departamento existente");
            }

            try
            {
                context.Update(func);
                await context.SaveChangesAsync();
                return Ok("Atualização feita com sucesso");
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpDelete]
        [Route("{id}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> Delete([FromServices] CompanyContext context, [FromRoute]int id, [FromRoute] int idFuncionario) {

            var DepartamentDoestnExists = CheckDepartament(context,id);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            var func = await context.Funcionarios.Where(x => x.DepartamentosId == id).Where(x => x.Id == idFuncionario).FirstOrDefaultAsync();

            if(func == null) {
                return NotFound("Funcionário não encontrado nesse setor");
            }

            try
            {
                context.Remove(func);
                await context.SaveChangesAsync();
                return Ok("Funcionario Removido");
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }


        private bool CheckDepartament([FromServices] CompanyContext context, int id) {
            var departament = context.Departamentos.Where(x => x.Id == id).FirstOrDefault();

            return departament == null ? true : false;
        }



    }
}
