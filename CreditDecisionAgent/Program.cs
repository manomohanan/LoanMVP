using Azure.AI.Projects;
using Azure.Identity;
using CreditDecisionAgent;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Nodes;

#pragma warning disable SKEXP0110

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

kernel.Plugins.AddFromType<CreditDecisionPlugin>("CreditDecisionPlugin");

var jsonData = File.ReadAllText("C:\\Users\\Manomohanan.Rv\\Downloads\\InputFiles\\InputFiles\\Borrower111000\\Data.json");
var configData = File.ReadAllText("C:\\Users\\Manomohanan.Rv\\Downloads\\Configuration\\Configuration\\Bangalore\\Data.json");

// Assuming jsonData contains the JSON string
var creditDecisionConfig = JsonSerializer.Deserialize<CreditDecisionConfig>(configData);
Console.WriteLine(JsonSerializer.Serialize(creditDecisionConfig) + "\n");

var promptTemplateConfig = new PromptTemplateConfig
{
    Template = """
    You are the Credit Decision Agent responsible for evaluating credit score of a borrower.

    Your input will be Property Quality Index score (PQI), Borrower Creditworthiness Index score (BCI) and Traditional score.
    Get the PQI score from score_pqi field of the JSON input
    Get the BCI score from score_bci field of the JSON input
    Get the Traditional score from creditScore field of the JSON inpiut
    
    Examples:
    { "score_pqi": 100 }
    { "score_bci": 200 }
    then PQI Score is 100 and BCI Score is 200.

    Use the weights and thresholds from the below configuration:
    {{$credit_decision_config}}

    Evaluate credit score for a borrower using the above inputs and weights.

    Outputs:
    You must return output in the following format:
    {
      "inputs": <input>,
      "weights": <weights>,
      "score": <overall_score>,
      "justification": "<justification>"
    }
    In the justification, mention the contribution of each score to the overall score.
    """
};

var templateFactory = new KernelPromptTemplateFactory();
var template = templateFactory.Create(promptTemplateConfig);
var instruction = await template.RenderAsync(kernel,
    new KernelArguments()
    {
        { "credit_decision_config", JsonSerializer.Serialize(creditDecisionConfig) }
    });

//Your tasks include:
//1.Read property quality score (PQI) and borrower creditworthiness score (BCI) provided.
//2. Identify weights for PQI, BCI and Traditional scores from the credit evaluation configuration using the 'weights' field.
//3.evaluate credit score by providing pqi, bci and traditional score and weights.
//4. Providing a rationale for your decision based on the input data.
//5. You should always respond in this format - [needed agent name]: [message].
//6.Respond with least number of tokens as much as possible.
ChatCompletionAgent agent = new ChatCompletionAgent()
{
    Instructions = instruction,
    Name = "CreditDecisionAgent",
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    })
};

var pqi_score = new JsonObject();
pqi_score.Add("score_pqi", 600);

var bci_score = new JsonObject();
bci_score.Add("score_bci", 800);

var groupChat = new AgentGroupChat(agent);
groupChat.AddChatMessage(new ChatMessageContent { Content = "PQI Score is " + pqi_score.ToJsonString(), Role = AuthorRole.User });
groupChat.AddChatMessage(new ChatMessageContent { Content = "BCI Score is " + bci_score.ToJsonString(), Role = AuthorRole.User });
groupChat.AddChatMessage(new ChatMessageContent { Content = "Borrower Data is " + jsonData, Role = AuthorRole.User });

//groupChat.AddChatMessage(new ChatMessageContent { Content = "Calculate the overall score", Role = AuthorRole.User });
await foreach (var item in groupChat.InvokeAsync())
{
    Console.WriteLine(item.Content);
}

//AIProjectClient client = AzureAIAgent.CreateAzureAIClient("<your connection-string>", new AzureCliCredential());
//AgentsClient agentsClient = client.GetAgentsClient();

//// 1. Define an agent on the Azure AI agent service
//Azure.AI.Projects.Agent definition = await agentsClient.CreateAgentAsync(
//    "gpt-4o",
//    name: "CreditDecisionAgent",
//    description: "Credit Decision Agent",
//    instructions: "<agent instructions>");

//// 2. Create a Semantic Kernel agent based on the agent definition
//AzureAIAgent agent = new(definition, agentsClient);