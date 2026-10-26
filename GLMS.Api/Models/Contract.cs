/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: W3Schools
 * Date: 2026
 * Title: C# Tutorial (C Sharp)
 * Type: [Source Code]
 * Available at: <https://www.w3schools.com/cs/index.php> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: One-to-many relationships
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many> [Accessed 6 April 2026]
 **************************************************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace GLMS.Api.Models;

public class Contract
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    [Required, StringLength(100)]
    public string ServiceLevel { get; set; } = string.Empty;

    [StringLength(500)]
    public string? SignedAgreementPath { get; set; }

    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
