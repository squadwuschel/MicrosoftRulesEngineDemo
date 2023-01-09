using System.Text.Json;
using RulesEngine.Models;
using RulesEngineDemo;

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

    private static readonly IEnumerable<Workflow>? WorkflowRules = JsonSerializer.Deserialize<IEnumerable<Workflow>>(Rule);

    public static async Task ExceuteSimpleRule()
    {
        var rulesEngine = new RulesEngine.RulesEngine(WorkflowRules!.ToArray());

        foreach (var inputValue in RulesInputValues.RuleInputValuesList)
        {
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter("input", inputValue));
            result.ForEach(action => { Console.WriteLine($"Name: {inputValue.Name} => Rule:{action.Rule.RuleName} IsSuccess: {action.IsSuccess}"); });
        }
    }
}