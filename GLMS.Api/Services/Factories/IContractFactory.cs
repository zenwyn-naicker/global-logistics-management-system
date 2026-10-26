/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Factory Method in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/factory-method/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Anthropic
 * Date: 2026
 * Title: Sample code of Factory Method pattern for MVC web app
 * Type: [Source Code]
 * Link to chat: <https://claude.ai/share/c93df5eb-9b5f-4d60-bc68-26f2f323537a> [Accessed 12 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Models;

namespace GLMS.Api.Services.Factories;

public interface IContractFactory
{
    Contract CreateContract(int clientId, DateTime startDate, DateTime endDate, string serviceLevel);
}
