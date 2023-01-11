using System.Text.Json;
using RulesEngine.Models;
using RulesEngineDemo;

namespace Demo;

public class DemoPostActionsRule
{
    //Zum Erstellen von z.B. Regelbäumen
    private const string Rule = """
            [
               {
                  "WorkflowName": "ProductionWorkflow",
                  "Rules": [
                     {
                       "RuleName": "r0",
                       "Expression": "input.AmountCarAccidents == 0",
                       "SuccessEvent": "HatteKeineUnfaelle",
                       "Actions" : {
                           "OnSuccess": {
                                "Name": "OutputExpression",
                                "Context": {
                                    "Expression": "input.Income * 0.5"
                                }
                            },
                            "OnFailure": {
                                "Name": "EvaluateRule",
                                "Context": {
                                    "WorkflowName": "ProductionWorkflow",
                                    "RuleName": "r3"
                                }
                            }
                       }
                     },
                     {
                       "RuleName": "r2",
                       "Expression": "input.HasCar == false",
                       "SuccessEvent": "KeineUnfaelle"
                     },
                     {
                       "RuleName": "r3",
                       "Expression": "input.HasCar == true",
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
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter[] {new("input", inputValue)});

            result.ForEach(action =>
            {
                Console.WriteLine(
                    $"Name: {inputValue.Name} \t => Rule:{action.Rule.RuleName} \t IsSuccess: {action.IsSuccess} \t EventName: {action.Rule.SuccessEvent} \t Expression: {action.Rule.Expression}");

                if (action.ActionResult != null)
                {
                    Console.WriteLine($"Reduzierte Kosten: {action.ActionResult.Output}");
                }
            });
        }
    }
}