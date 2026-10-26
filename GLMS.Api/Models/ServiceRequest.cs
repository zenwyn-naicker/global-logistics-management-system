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

public enum ServiceRequestStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled
}

public class ServiceRequest
{
    public int Id { get; set; }

    public int ContractId { get; set; }
    public Contract? Contract { get; set; }

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
