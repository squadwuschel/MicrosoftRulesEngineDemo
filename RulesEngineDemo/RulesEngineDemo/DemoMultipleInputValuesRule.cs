using System.Text.Json;
using RulesEngine.Models;
using RulesEngineDemo;

namespace Demo;

public class DemoMultipleInputValuesRule
{
    private const string Rule = """
            [
               {
                  "WorkflowName": "ProductionWorkflow",
                  "Rules": [
                     {
                       "RuleName": "r0",
                       "Expression": "input.LicensePlate.Contains(myConst.Dresden) == true",
                       "SuccessEvent": "StadtDresden"
                     },
                     {
                       "RuleName": "r1",
                       "Expression": "input.LicensePlate.Contains(myConst.Berlin) == true",
                       "SuccessEvent": "StadtBerlin"
                     },
                     {
                       "RuleName": "r2",
                       "Expression": "input.LicensePlate.Contains(myConst.AueSchwarzenberg) == true",
                       "SuccessEvent": "StadtAueSchwarzb."
                     }
                  ]
                }
             ]
        """;

    private static readonly IEnumerable<Workflow>? WorkflowRules = JsonSerializer.Deserialize<IEnumerable<Workflow>>(Rule);

    public static async Task ExceuteRules()
    {
        var rulesEngine = new RulesEngine.RulesEngine(WorkflowRules!.ToArray());
        var constInputs = new RulesConstInputs();

        foreach (var inputValue in RulesInputValues.RuleInputValuesList)
        {
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow",
                new RuleParameter("input", inputValue),
                new RuleParameter("myConst", constInputs));
            result.ForEach(action =>
            {
                Console.WriteLine(
                    $"Name: {inputValue.Name} \t => Rule:{action.Rule.RuleName} \t IsSuccess: {action.IsSuccess} \t EventName: {action.Rule.SuccessEvent} \t Expression: {action.Rule.Expression}");
            });
        }
    }
}

public class RulesConstInputs
{
    public string AueSchwarzenberg = "ASZ";
    public string Berlin = "B";
    public string Dresden = "DD";
}