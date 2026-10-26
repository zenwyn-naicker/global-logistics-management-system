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

public class ContractFactoryResolver
{
    private readonly Dictionary<ContractStatus, IContractFactory> _factories;

    public ContractFactoryResolver(IEnumerable<IContractFactory> factories)
    {
        _factories = new Dictionary<ContractStatus, IContractFactory>
        {
            { ContractStatus.Draft, factories.OfType<DraftContractFactory>().First() },
            { ContractStatus.Active, factories.OfType<ActiveContractFactory>().First() },
            { ContractStatus.Expired, factories.OfType<ExpiredContractFactory>().First() },
            { ContractStatus.OnHold, factories.OfType<OnHoldContractFactory>().First() },
            { ContractStatus.Declined, factories.OfType<DeclinedContractFactory>().First() }
        };
    }

    public IContractFactory Resolve(ContractStatus status)
    {
        if (!_factories.TryGetValue(status, out var factory))
            throw new ArgumentException($"No factory registered for status: {status}", nameof(status));

        return factory;
    }
}
