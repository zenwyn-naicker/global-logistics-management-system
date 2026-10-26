/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Observer in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/observer/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Anthropic
 * Date: 2026
 * Title: Sample code of Observer pattern for MVC web app
 * Type: [Source Code]
 * Link to chat: <https://claude.ai/share/9278790e-8b3c-40e8-aabd-265a276db03b> [Accessed 12 April 2026]
 **************************************************************************************************************/

namespace GLMS.Api.Services.Observers;

public interface IContractObserver
{
    void Update(ContractStatusChangedEvent statusEvent);
}
