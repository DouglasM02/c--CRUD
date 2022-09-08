using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Company.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Data
{
    public class CompanyContext : DbContext
    {
        public DbSet<Funcionarios> Funcionarios { get; set; }
        public DbSet<Departamentos> Departamentos { get; set; }

        //public CompanyContext(DbContextOptions<CompanyContext> options): base(options){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("string de conexao");
        }
    }
}
