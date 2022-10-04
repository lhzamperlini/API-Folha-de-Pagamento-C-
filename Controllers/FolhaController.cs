using System.Buffers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/folha")]
    public class FolhaController : ControllerBase
    {
        private readonly DataContext _context;
        public FolhaController(DataContext context) =>
            _context = context;

        // GET: /api/folha/listar
        [HttpGet]
        [Route("listar")]
        public IActionResult Listar() => Ok(_context.Folhas.Include(f => f.funcionario).ToList());

        // POST: /api/folha/cadastrar
        [HttpPost]
        [Route("cadastrar")]
        public IActionResult Cadastrar([FromBody] FolhaPagamento folha)
        {
        //    Temos: funcionarioid, valorHora, quantidadehoras

        // Imposto de Renda
        folha.salariobruto = folha.quantidadehoras * folha.valorhora;
        if(folha.salariobruto<1903.98)
        {
            folha.impostorenda = 0;
        }
        else if(folha.salariobruto>=1903.99 && folha.salariobruto<= 2826.65)
        {
            folha.impostorenda = folha.salariobruto * 0.075;
        }
        else if(folha.salariobruto>=2828.66 && folha.salariobruto<= 3751.05)
        {
            folha.impostorenda = folha.salariobruto * 0.15;
        }
        else if(folha.salariobruto>=3751.06 && folha.salariobruto<= 4664.08)
        {
            folha.impostorenda = folha.salariobruto * 0.225;
        }
        else
        {
            folha.impostorenda = folha.salariobruto * 0.275;
        }

        // INSS
        if(folha.salariobruto<1693.72)
        {
            folha.impostoinss = folha.salariobruto * 0.08;
        }
        else if(folha.salariobruto>=1693.73 && folha.salariobruto<= 2822.90)
        {
            folha.impostoinss = folha.salariobruto * 0.09;
        }
        else if(folha.salariobruto>=2822.91 && folha.salariobruto<= 5645.80)
        {
            folha.impostoinss = folha.salariobruto * 0.11;
        }
        else
        {
            folha.impostoinss = 621.03;
        }

        //FGTS
        folha.impostofgts = folha.salariobruto * 0.08;

        //SALARIO LIQUIDO
        folha.salarioliquido = folha.salariobruto - folha.impostofgts - folha.impostoinss - folha.impostorenda;

        //FUNCIONARIO
        Funcionario funcionario = _context.Funcionarios.
                FirstOrDefault(f => f.Id.Equals(folha.funcionarioid));
        if(funcionario == null){
            return NotFound();
        }
        folha.funcionario = funcionario;
        
        //SALVANDO NO BANCO DE DADOS
        try
        {
            _context.Folhas.Add(folha);
            _context.SaveChanges();
            return Created("", folha);
        }
        catch{
            return NotFound();
        }

        }

     // GET: /api/folha/buscar/{cpf}
        [HttpGet]
        [Route("buscar/{cpf}/{mes}/{ano}")]
        public IActionResult Buscar([FromRoute] string cpf, int mes, int ano)
        {
            Funcionario funcionario = _context.Funcionarios.
                FirstOrDefault(f => f.Cpf.Equals(cpf));
            FolhaPagamento folha = _context.Folhas.
                FirstOrDefault(f => f.funcionario.Equals(funcionario) && f.mes.Equals(mes) && f.ano.Equals(ano));

            return folha != null ? Ok(folha) : NotFound();
        }

        
     // GET: /api/folha/filtrar/{cpf}/{mes}/{ano}
        [HttpGet]
        [Route("filtrar/{mes}/{ano}")]
        public IActionResult Filtrar([FromRoute] int mes, int ano)
        {
            return Ok(_context.Folhas.Include(f => f.funcionario).Where(f=> f.mes.Equals(mes) && f.ano.Equals(ano)).ToList());
        }

}
}