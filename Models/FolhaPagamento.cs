using System;

namespace API.Models
{
    public class FolhaPagamento
    {
        public int Id { get; set; }
        public int funcionarioid {get; set;}
        public int mes {get; set;}
        public int ano {get; set;}
        public int valorhora {get; set;}
        public int quantidadehoras {get; set;}
        public double salariobruto {get; set;}
        public double impostorenda {get; set;}
        public double impostoinss {get; set;}
        public double impostofgts {get; set;}
        public double salarioliquido {get; set;}
        public Funcionario funcionario {get; set;}
    }
}