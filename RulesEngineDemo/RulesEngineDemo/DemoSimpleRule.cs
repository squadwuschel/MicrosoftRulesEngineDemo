using System.Text.Json;
using RulesEngine.Models;

namespace Demo;

public class DemoSimpleRule
{
    private const string Rule = """
            [
               {
                  "WorkflowName": "ProductionWorkflow",
                  "Rules": [
                     {
                       "RuleName": "r0",
                       "Expression": "input.AmountCarAccidents == 0",
                       "SuccessEvent": "HatteKeineUnfaelle"
                     },
                     {
                       "RuleName": "r2",
                       "Expression": "input.AmountCarAccidents > 0",
                       "SuccessEvent": "HatteUnfaelle"
                     }                     
                  ]
                }
             ]
        """;

    public static readonly IEnumerable<Workflow>? WorkflowRules = JsonSerializer.Deserialize<IEnumerable<Workflow>>(Rule);
}