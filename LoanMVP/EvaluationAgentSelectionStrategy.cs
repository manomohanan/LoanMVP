﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;

namespace LoanMVP
{
    #pragma warning disable SKEXP0110
    public class EvaluationAgentSelectionStrategy : SelectionStrategy
    {
        protected override Task<Agent> SelectAgentAsync(IReadOnlyList<Agent> agents, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var lastMessage = history.LastOrDefault()?.Content?.ToString() ?? string.Empty;
                var targetAgentName = string.IsNullOrEmpty(lastMessage) ? "BorrowerDataCollectionAgent" : lastMessage.Substring(1, lastMessage.IndexOf(']') - 1);
                var agent = agents.First(x => x.Name == targetAgentName);
                return agent;
            });
        }
    }
}
