using System.Text.Json;
using RulesEngine.Models;
using RulesEngineDemo;

namespace Demo;

public class DemoCustomRule
{
    private const string Rule = """
            [
               {
                  "WorkflowName": "ProductionWorkflow", 
                  "Rules": [
                     {
                       "RuleName": "r0",
                       "Expression": "input.Age == AgeCalculator.GetAge(input.Birthdate)",
                       "SuccessEvent": "RichtigesAlter"
                     }
                  ]
                }
             ]
        """;

    private static readonly IEnumerable<Workflow>? WorkflowRules = JsonSerializer.Deserialize<IEnumerable<Workflow>>(Rule);

    public static async Task ExceuteRules()
    {
        //ACHTUNG bei CustomRules diese müssen registriert werden, sonst sieht man im RuleResult die Exceptions!
        var settings = new ReSettings() {CustomTypes = new Type[] {typeof(AgeCalculator)}};

        var rulesEngine = new RulesEngine.RulesEngine(WorkflowRules!.ToArray(), settings);

        foreach (var inputValue in RulesInputValues.RuleInputValuesList)
        {
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter("input", inputValue));
            //Debug Exception zeigen, wenn die Settings nicht gesetzt wurden!
            result.ForEach(action =>
            {
                Console.WriteLine(
                    $"Name: {inputValue.Name} \t => Rule:{action.Rule.RuleName} \t IsSuccess: {action.IsSuccess} \t EventName: {action.Rule.SuccessEvent} \t Expression: {action.Rule.Expression}");
            });
        }
    }
}

public static class AgeCalculator
{
    public static int GetAge(DateTime birthdate)
    {
        var now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        var dob = int.Parse(birthdate.ToString("yyyyMMdd"));
        var age = (now - dob) / 10000;
        return age;
    }
}