using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanMVP
{
    public class EvaluationFunctions
    {
        #pragma warning disable SKEXP0110

        private Kernel _Kernel;
        public EvaluationFunctions(Kernel kernel)
        {
            this._Kernel = kernel;
        }

        //[KernelFunction, Description("Evaluating property-specific details in the given data and returns a summary")]
        public void EvaluatePQIScore(string json)
        {
            //var agentMetadata = new AgentMetadata();
            //var pqiDoerMetadata = agentMetadata.GetEvaluationAgent("PQIInspectorAgent");
            //var pqiCriticMetadata = agentMetadata.GetEvaluationAgent("PQIReviewerAgent");

            //var args = new KernelArguments();
            //args.Add("input_json", json);

            //ChatCompletionAgent inspectorAgent = new ChatCompletionAgent()
            //{
            //    Instructions = pqiDoerMetadata.Instructions,
            //    Name = pqiDoerMetadata.Name,
            //    Kernel = this._Kernel,
            //    Arguments = args,
            //};

            //ChatCompletionAgent reviewerAgent = new ChatCompletionAgent()
            //{
            //    Instructions = pqiCriticMetadata.Instructions,
            //    Name = pqiCriticMetadata.Name,
            //    Kernel = this._Kernel,
            //    Arguments = args,
            //};

            
        }

        public void EvaluateBCIScore()
        {
        }

        public void EvaluateCreditDecisionScore()
        {
        }

        public void EvaluateFinalCreditDecision()
        {
        }
    }
}
