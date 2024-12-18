using DataAcquisition.Model.Model.Login;
using Microsoft.EntityFrameworkCore;

namespace LoginService.Mapper;

/// <summary>
/// @author Xioa
/// @date  2024年12月4日
/// </summary>
public partial class DbDataContext
{
    public DbSet<LoginUser> Users { get; set; }
}