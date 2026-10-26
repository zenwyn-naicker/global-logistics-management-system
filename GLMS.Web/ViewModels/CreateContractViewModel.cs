using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GLMS.Web.ViewModels;

public class CreateContractViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a client.")]
    public int ClientId { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

    public ContractStatusViewModel Status { get; set; } = ContractStatusViewModel.Draft;

    [Required, StringLength(100)]
    public string ServiceLevel { get; set; } = string.Empty;

    public IFormFile? SignedAgreement { get; set; }
}
