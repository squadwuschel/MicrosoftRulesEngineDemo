using System.Text.Json;
using RulesEngine.Models;
using RulesEngineDemo;

namespace Demo;

public class DemoOperatorRule
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
                       "Operator": "Or",
                       "Expression": "input.AmountCarAccidents > 0",
                       "SuccessEvent": "HatteUnfaelle",
                       "Rules": [
                          {
                             "RuleName": "r2_1",
                             "Expression": "input.HasDriversLicense == true",
                             "SuccessEvent": "HatteKeineUnfaelle_1" 
                          }, 
                          {
                             "RuleName": "r2_2",
                             "Expression": "input.Income > 3000",
                             "SuccessEvent": "HatteKeineUnfaelle_2" 
                          }
                       ]
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

                if (action.ChildResults != null)
                {
                    foreach (var childResult in action.ChildResults)
                    {
                        Console.WriteLine(
                            $"CName: {inputValue.Name} \t => Rule:{childResult.Rule.RuleName} \t IsSuccess: {childResult.IsSuccess} \t EventName: {childResult.Rule.SuccessEvent} \t Expression: {childResult.Rule.Expression}");
                    }
                }
            });
        }
    }
}