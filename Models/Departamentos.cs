using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Models
{
    public class Departamentos
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sigla { get; set; }
        //public virtual ICollection<Funcionarios> Funcionarios { get; set; }
    }
}
