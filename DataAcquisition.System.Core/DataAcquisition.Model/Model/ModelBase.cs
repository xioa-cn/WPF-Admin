using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAcquisition.Model.Model;

public class ModelBase
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
    public int Id { get; set; }
}