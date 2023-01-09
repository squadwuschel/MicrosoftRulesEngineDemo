// See https://aka.ms/new-console-template for more information

using Demo;
using RulesEngine.Models;
using RulesEngineDemo;

var rulesEngine = new RulesEngine.RulesEngine(DemoSimpleRule.WorkflowRules!.ToArray());

foreach (var inputValue in RulesInputValues.RuleInputValuesList)
{
    var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter("input", inputValue));
    result.ForEach(action => { Console.WriteLine($"Name: {inputValue.Name} => Rule:{action.Rule.RuleName} IsSuccess: {action.IsSuccess}"); });
}

Console.WriteLine("THE END");