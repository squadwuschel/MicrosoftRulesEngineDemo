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
              },
              {
                "RuleName": "r2",
                "Expression": "isEnabled && input.SumBestellungen > 0 && input.Acos >= input.ZielAcos && input.AktuellesGebot >= input.CostPerClick && input.SumKlicks >= input.Klicks && input.Mindestgebot >= input.AktuellesGebot",
                "SuccessEvent": "KeineGebotsanpassung"
              },
              {
                "RuleName": "r11",
                "Expression": "isEnabled && input.SumBestellungen > 0 && input.Acos >= input.ZielAcos && input.AktuellesGebot >= input.CostPerClick && input.SumKlicks < input.Klicks",
                "SuccessEvent": "KeineGebotsanpassung"
              },
              {
                "RuleName": "r3",
                "Expression": "isEnabled && input.SumBestellungen > 0 && input.Acos >= input.ZielAcos && input.AktuellesGebot < input.CostPerClick",
                "SuccessEvent": "KeineGebotsanpassung"
              },
              {
                "RuleName": "r4",
                "Expression": "isEnabled && input.SumBestellungen > 0 && input.Acos < input.ZielAcos && input.AktuellesGebot > input.CostPerClick",
                "SuccessEvent": "KeineGebotsanpassung"
              }
            ]
          }
        ]
        """;

    private static readonly List<RuleValues> RuleValuesList = new()
    {
        new RuleValues(2, 35, (decimal) 4.69, (decimal) 0.36, (decimal) 0.21, KampagnenArtEnum.SponsoredBrands, 8,
            36, 9, (decimal) 0.01, (decimal) 0.01, (decimal) 0.1, (decimal) 1, StatesEnum.enabled)
    };

    public static async Task ExceuteRules()
    {
        var workflow = JsonConvert.DeserializeObject<List<Workflow>>(Rule);

        //C:\Users\USERNAME\AppData\Local\RulesEngineDb\RulesEngineDemo.db

        var db = new RulesEngineDemoContext();
        //if (db.Database.EnsureCreated())
        //{
        //    db.Workflows.AddRange(workflow);
        //    db.SaveChanges();
        //}

        var workflows = db.Workflows.Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(p => p.GlobalParams).ToArray();

        var rulesEngine = new RulesEngine.RulesEngine(workflows, null);

        foreach (var inputValue in RuleValuesList)
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

public class RuleValues
{
    public RuleValues(
        int sumBestellungen,
        int sumKlicks,
        decimal? acos,
        decimal aktuellesGebot,
        decimal? costPerClick,
        KampagnenArtEnum kampagnenArt,
        int klicks,
        int klicksFuersPausieren,
        decimal zielAcos,
        decimal gebotErhoehenUm,
        decimal gebotReduzierenUm,
        decimal mindestgebot,
        decimal maximalgebot,
        StatesEnum state)
    {
        SumBestellungen = sumBestellungen;
        SumKlicks = sumKlicks;
        Acos = acos;
        AktuellesGebot = aktuellesGebot;
        CostPerClick = costPerClick;
        KampagnenArt = kampagnenArt;
        Klicks = klicks;
        KlicksFuersPausieren = klicksFuersPausieren;
        ZielAcos = zielAcos;
        GebotErhoehenUm = gebotErhoehenUm;
        GebotReduzierenUm = gebotReduzierenUm;
        Mindestgebot = mindestgebot;
        Maximalgebot = maximalgebot;
        State = state.ToString();
    }

    public int SumBestellungen { get; }
    public int SumKlicks { get; }
    public decimal? Acos { get; }
    public decimal AktuellesGebot { get; }
    public decimal? CostPerClick { get; }
    public KampagnenArtEnum KampagnenArt { get; }
    public int Klicks { get; }
    public int KlicksFuersPausieren { get; }
    public decimal ZielAcos { get; }
    public decimal GebotErhoehenUm { get; }
    public decimal GebotReduzierenUm { get; }
    public decimal Mindestgebot { get; }
    public decimal Maximalgebot { get; }
    public string State { get; }
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