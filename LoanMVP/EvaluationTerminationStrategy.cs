using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;

namespace LoanMVP
{
    #pragma warning disable SKEXP0110
    public class EvaluationTerminationStrategy : TerminationStrategy
    {
        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return agent.Name == "BorrowerDataCollectionAgent" && (history.LastOrDefault()?.Content?.Contains("[EVALUATIONCOMPLETE]") ?? false);
            });
        }
    }
}
