using System.Text.Json;
using RulesEngine.Extensions;
using RulesEngine.Models;
using RulesEngineDemo;

namespace Demo;

public class DemoExtendedRule
{
    private const string Rule = """
            [
               {
                  "WorkflowName": "ProductionWorkflow",
                  "Rules": [
                     {
                       "RuleName": "r0",
                       "Expression": "input.AmountCarAccidents == 0 && input.HasCar == false",
                       "SuccessEvent": "HatteKeineUnfaelle"
                     },
                     {
                       "RuleName": "r2",
                       "Expression": "input.AmountCarAccidents > 0 AND input.HasCar == true",
                       "SuccessEvent": "HatteUnfaelle"
                     }                     
                  ]
                }
             ]
        """;

    private static readonly IEnumerable<Workflow>? WorkflowRules = JsonSerializer.Deserialize<IEnumerable<Workflow>>(Rule);

    public static async Task ExceuteRules()
    {
        var rulesEngine = new RulesEngine.RulesEngine(WorkflowRules!.ToArray());

        foreach (var inputValue in RulesInputValues.RuleInputValuesList)
        {
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter("input", inputValue));
            result.ForEach(action =>
            {
                Console.WriteLine(
                    $"Name: {inputValue.Name} \t => Rule:{action.Rule.RuleName} \t IsSuccess: {action.IsSuccess} \t EventName: {action.Rule.SuccessEvent} \t Expression: {action.Rule.Expression}");
            });

            result.OnSuccess((eventname) => { Console.WriteLine($"Success: {eventname}"); });
            result.OnFail(() => Console.WriteLine("Hat nicht geklappt!"));
        }
    }
}