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
        private IWebHostEnvironment _webHostEnviroment;
        private CompanyContext _context;
        public FuncionariosController(IWebHostEnvironment webHostEnviroment, CompanyContext context) {
            _webHostEnviroment = webHostEnviroment;
            _context = context;
        }

        [HttpGet]
        [Route("{idDepartamento}")]
        public async Task<IActionResult> FindAll([FromRoute]int idDepartamento) {
            var DepartamentDoestnExists = CheckDepartament(idDepartamento);
            var funcionarios = await _context.Funcionarios.Where(x => x.DepartamentosId == idDepartamento).ToListAsync();

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            return funcionarios.Count > 0 ? Ok(funcionarios) : NotFound();

        }

        [HttpGet]
        [Route("{idDepartamento}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> findById([FromRoute]int idDepartamento, [FromRoute] int idFuncionario) {
            try
            {
                var funcionario = await _context.Funcionarios.Where(x => x.DepartamentosId == idDepartamento).Where(y => y.Id == idFuncionario).FirstOrDefaultAsync();
                return funcionario != null ? Ok(funcionario) : NotFound("Funcionário não encontrado");

            }
            catch (Exception)
            {
                return NotFound("Funcionário não encontrado");
            }
        }

        [HttpPost]
        [Route("{idDepartamento}")]
        public async Task<IActionResult> Insert([FromServices] CompanyContext context, [FromRoute]int idDepartamento, Funcionarios funcionario) {

            var DepartamentDoestnExists = CheckDepartament(idDepartamento);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            if(!ModelState.IsValid) {return BadRequest();}

            var func = new Funcionarios {
                Nome = funcionario.Nome,
                Foto = funcionario.Foto,
                Rg = funcionario.Rg,
                DepartamentosId = idDepartamento
            };

            try
            {
                await context.AddAsync(func);
                await context.SaveChangesAsync();
                return Created($"v1/funcionarios/{idDepartamento}","Criado Com Sucesso");
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPut]
        [Route("{idDepartamento}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> Put([FromServices] CompanyContext context, [FromRoute]int idDepartamento, [FromRoute] int idFuncionario, Funcionarios funcionario) {

            var DepartamentDoestnExists = CheckDepartament(idDepartamento);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            var func = await context.Funcionarios.Where(x => x.DepartamentosId == idDepartamento).Where(x => x.Id == idFuncionario).FirstOrDefaultAsync();

            if(func == null) {
                return NotFound("Funcionário não encontrado nesse setor");
            }

            func.Nome = func.Nome != funcionario.Nome ? funcionario.Nome : func.Nome;
            func.Foto = func.Foto != funcionario.Foto ? funcionario.Foto : func.Foto;
            func.Rg = func.Rg != funcionario.Rg ? funcionario.Rg : func.Rg;
            func.DepartamentosId = func.DepartamentosId != funcionario.DepartamentosId ? funcionario.DepartamentosId : func.DepartamentosId;

            // if(CheckDepartament(_context,func.DepartamentosId)) {
            //     return NotFound("Departamento não existe, por favor escolha um departamento existente");
            // }

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
        [Route("{idDepartamento}/funcionario/{idFuncionario}")]
        public async Task<IActionResult> Delete([FromRoute]int idDepartamento, [FromRoute] int idFuncionario) {

            var DepartamentDoestnExists = CheckDepartament(idDepartamento);

            if(DepartamentDoestnExists) {
                return NotFound("Departamento não encontrado");
            }

            var func = await _context.Funcionarios.Where(x => x.DepartamentosId == idDepartamento).Where(x => x.Id == idFuncionario).FirstOrDefaultAsync();

            if(func == null) {
                return NotFound("Funcionário não encontrado nesse setor");
            }

            try
            {
                _context.Remove(func);
                await _context.SaveChangesAsync();
                return Ok("Funcionario Removido");
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        [Route("upload")]
        public Task<string> PostImage([FromForm] FileUpload objectFile)
        {
            try
            {
                if(objectFile.files.Length > 0) {
                    string path = _webHostEnviroment.ContentRootPath + "\\uploads\\";

                    if(!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path + objectFile.files.FileName)) {
                        objectFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        return Task.FromResult( path + objectFile.files.FileName);
                        //path +
                    }
                }
                else {
                    return Task.FromResult("Failed");
                }
            }
            catch (Exception ex)
            {

                return Task.FromResult(ex.Message);
            }
        }

        [HttpGet]
        [Route("upload/{idFuncionario}")]
        public async Task<IActionResult> GetImage([FromRoute] int idFuncionario) {
             try
            {
                var funcionario = await _context.Funcionarios.Where(y => y.Id == idFuncionario).FirstOrDefaultAsync();
                string path = _webHostEnviroment.ContentRootPath + funcionario.Foto;

                return funcionario != null? PhysicalFile(funcionario.Foto, contentType: "image/jpeg", enableRangeProcessing: true) : NotFound();

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }



        private bool CheckDepartament(int id) {
            var departament = _context.Departamentos.Where(x => x.Id == id).FirstOrDefault();

            return departament == null ? true : false;
        }





    }
}
