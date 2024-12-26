using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xioa.Admin.Model.Model;

public class ModelBase
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
    public int Id { get; set; }
}