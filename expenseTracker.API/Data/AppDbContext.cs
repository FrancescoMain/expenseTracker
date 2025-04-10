using Microsoft.EntityFrameworkCore;
using GestioneSpese.API.Models;

namespace GestioneSpese.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Utente> Utenti => Set<Utente>();
        public DbSet<Conto> Conti => Set<Conto>();
        public DbSet<Categoria> Categorie => Set<Categoria>();
        public DbSet<Sottocategoria> Sottocategorie => Set<Sottocategoria>();
        public DbSet<Spesa> Spese => Set<Spesa>();
        public DbSet<Trasferimento> Trasferimenti => Set<Trasferimento>();
    }
}
