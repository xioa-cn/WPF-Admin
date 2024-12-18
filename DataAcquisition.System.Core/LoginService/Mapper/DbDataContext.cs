using Microsoft.EntityFrameworkCore;

namespace LoginService.Mapper;

public partial class DbDataContext : DbContext
{
  
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "DB.db");
        optionsBuilder.UseSqlite("Data Source=" + configString);
    }
   
}