using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Models
{
    public class Funcionarios
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Foto { get; set; }
        public int Rg { get; set; }
        public int DepartamentosId { get; set; }
        //public virtual Departamentos Departamentos {get; set;}
    }
}
