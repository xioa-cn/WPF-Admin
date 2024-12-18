using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAcquisition.Model.Model.Login;

/// <summary>
/// @author Xioa
/// @date  2024年11月29日
/// </summary>
[Table("LoginUser")]
public class LoginUser : ModelBase
{
    [Required] [MaxLength(255)] public string? UserName { get; set; }
    [Required] [MaxLength(255)] public string? Password { get; set; }
    [MaxLength(255)] public string? Header { get; set; }
    public LoginAuth LoginAuth { get; set; }
}