using System.ComponentModel.DataAnnotations;

namespace GLMS.Api.Dtos;

public class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactDetails { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int ContractCount { get; set; }
}

public class CreateClientDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string ContactDetails { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Region { get; set; } = string.Empty;
}
