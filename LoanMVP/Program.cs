using LoanMVP;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0001

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var deploymentName = builder["deploymentName"];
var endpoint = builder["endpoint"];
var apiKey = builder["apiKey"];
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

var jsonData = File.ReadAllText("C:\\Users\\Manomohanan.Rv\\Downloads\\InputFiles\\InputFiles\\Borrower111000\\Data.json");

#region Full Orchestration
var agentMetadata = new AgentMetadata();

var CreateAgent = (string instructions, string name, KernelArguments args) =>
{
    ChatCompletionAgent agent = new ChatCompletionAgent()
    {
        Instructions = instructions,
        Name = name,
        Kernel = kernel,
        Arguments = args,
    };
    return agent;
};

var allAgents = agentMetadata.Agents.Select(agent =>
{
    var args = new KernelArguments();
    //args.Add("input_json", jsonData);
    var newAgent = CreateAgent(agent.Instructions, agent.Name, args);
    return newAgent;
});

#pragma warning disable SKEXP0110
var agent = new AgentGroupChat(allAgents.ToArray())
{
    ExecutionSettings = new AgentGroupChatSettings
    {
        TerminationStrategy = new EvaluationTerminationStrategy
        {
            MaximumIterations = 100,
            AutomaticReset = true,
            Agents = allAgents.ToArray()
        },
        SelectionStrategy = new EvaluationAgentSelectionStrategy()
    }
};
agent.AddChatMessage(new ChatMessageContent()
{
    Role = AuthorRole.User,
    Content = $"[BorrowerDataCollectionAgent]: This is the details for loan evaluation: ${jsonData}",
});

await foreach (var item in agent.InvokeAsync())
{
    await Task.Delay(15000);
    Console.WriteLine(item.Content);
}
#endregion