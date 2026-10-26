using System.ComponentModel.DataAnnotations;

namespace GLMS.Web.ViewModels;

public class ClientViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactDetails { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int ContractCount { get; set; }
}

public class CreateClientViewModel
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string ContactDetails { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Region { get; set; } = string.Empty;
}
