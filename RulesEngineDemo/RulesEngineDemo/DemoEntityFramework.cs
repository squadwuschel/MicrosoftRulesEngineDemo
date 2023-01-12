// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using EFDataExample;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RulesEngine.Models;

namespace Demo;

public class DemoEntityFramework
{
    private const string Rule = """
            [
          {
            "WorkflowName": "ProductionWorkflow",
            "GlobalParams": [
              {
                "Name": "isPaused",
                "Expression": "input.State == \"paused\""
              },
              {
                "Name": "isEnabled",
                "Expression": "input.State == \"enabled\""
              }
            ],
            "Rules": [
              {
                "RuleName": "r0",
                "Expression": "isPaused",
                "SuccessEvent": "KeineGebotsanpassung"
              },
              {
                "RuleName": "r1",
                "Expression": "isEnabled && input.SumBestellungen > 0 && input.Acos >= input.ZielAcos && input.AktuellesGebot >= input.CostPerClick && input.SumKlicks >= input.Klicks && input.Mindestgebot < input.AktuellesGebot",
                "SuccessEvent": "GebotsReduzierung"
              }
            ]
          }
        ]
        """;

    private static readonly List<RuleInputValues> RuleInputValuesList = new()
    {
        new RuleInputValues(0, 40, 8, 0.8m, 0.01m, KampagnenArtEnum.SponsoredBrands, StatesEnum.paused),
        new RuleInputValues(1, 9, 10, 1, 1, KampagnenArtEnum.SponsoredBrands, StatesEnum.enabled),
        new RuleInputValues(1, 1, 10, 1, 1, KampagnenArtEnum.SponsoredBrands, StatesEnum.enabled)
    };

    public static async Task ExceuteRules()
    {
        var workflow = JsonConvert.DeserializeObject<List<Workflow>>(Rule);

        //C:\Users\USERNAME\AppData\Local\RulesEngineDb\RulesEngineDemo.db

        var db = new RulesEngineDemoContext();
        if (db.Database.EnsureCreated())
        {
            db.Workflows.AddRange(workflow);
            db.SaveChanges();
        }

        var workflows = db.Workflows.Include(i => i.Rules).ThenInclude(i => i.Rules).ToArray();

        var rulesEngine = new RulesEngine.RulesEngine(workflows, null);

        foreach (var inputValue in RuleInputValuesList)
        {
            var result = await rulesEngine.ExecuteAllRulesAsync("ProductionWorkflow", new RuleParameter("input", inputValue));
            result.ForEach(action =>
            {
                Console.WriteLine(
                    $"KampagnenArt: {inputValue.KampagnenArt} \t => Rule:{action.Rule.RuleName} \t IsSuccess: {action.IsSuccess} \t EventName: {action.Rule.SuccessEvent}");
            });
        }
    }
}

public class RuleInputValues
{
    public RuleInputValues(
        int sumBestellungen,
        int sumKlicks,
        decimal? acos,
        decimal aktuellesGebot,
        decimal? costPerClick,
        KampagnenArtEnum kampagnenArt,
        StatesEnum state)
    {
        SumBestellungen = sumBestellungen;
        SumKlicks = sumKlicks;
        Acos = acos;
        AktuellesGebot = aktuellesGebot;
        CostPerClick = costPerClick;
        KampagnenArt = kampagnenArt;
        State = state;
    }

    public int SumBestellungen { get; }

    public int SumKlicks { get; }

    public decimal? Acos { get; }

    public decimal AktuellesGebot { get; }

    public decimal? CostPerClick { get; }
    public KampagnenArtEnum KampagnenArt { get; }
    public StatesEnum State { get; }
}

public enum KampagnenArtEnum
{
    SponsoredProducts = 1,
    SponsoredBrands = 2,
    SponsoredDisplay = 3
}

public enum StatesEnum
{
    enabled = 0,
    paused = 1,
    archived = 2
}