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

public class Client
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string ContactDetails { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Region { get; set; } = string.Empty;

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
