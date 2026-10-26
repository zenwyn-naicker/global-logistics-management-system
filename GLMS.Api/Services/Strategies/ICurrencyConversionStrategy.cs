/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Strategy in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/strategy/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Anthropic
 * Date: 2026
 * Title: Sample code of Strategy pattern for MVC web app
 * Type: [Source Code]
 * Link to chat: <https://claude.ai/share/d67b55f8-8902-42d6-9787-94b14994261c> [Accessed 12 April 2026]
 **************************************************************************************************************/

namespace GLMS.Api.Services.Strategies;

public interface ICurrencyConversionStrategy
{
    bool IsAvailable();
    Task<decimal> Convert(decimal amountInUsd);
}
