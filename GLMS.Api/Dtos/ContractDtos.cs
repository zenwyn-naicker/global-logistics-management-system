using System.ComponentModel.DataAnnotations;
using GLMS.Api.Models;

namespace GLMS.Api.Dtos;

public class ContractDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractStatus Status { get; set; }
    public string ServiceLevel { get; set; } = string.Empty;
    public string? SignedAgreementPath { get; set; }
    public int ServiceRequestCount { get; set; }
}

public class CreateContractDto
{
    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    [Required, StringLength(100)]
    public string ServiceLevel { get; set; } = string.Empty;

    [StringLength(500)]
    public string? SignedAgreementPath { get; set; }
}

public class UpdateContractStatusDto
{
    public ContractStatus Status { get; set; }
}

public class ContractFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ContractStatus? Status { get; set; }
}
