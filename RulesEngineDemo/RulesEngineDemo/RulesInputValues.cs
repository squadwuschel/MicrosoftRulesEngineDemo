namespace RulesEngineDemo;

public class RulesInputValues
{
    public static IReadOnlyList<RulesInputValue> RuleInputValuesList = new List<RulesInputValue>()
    {
        new()
        {
            Name = "Johannes R.",
            Age = 41,
            Birthdate = new DateTime(1982, 3, 15),
            AmountCarAccidents = 0,
            HasCar = true,
            HasDriversLicense = true,
            Income = (decimal) 2500.45,
            LicensePlate = "DD GG 395"
        },
        new()
        {
            Name = "Maximus L.",
            Age = 25,
            Birthdate = new DateTime(1998, 1, 19),
            AmountCarAccidents = 1,
            HasCar = true,
            HasDriversLicense = true,
            Income = (decimal) 3700.99,
            LicensePlate = "ASZ ER 1337"
        },
        new()
        {
            Name = "Thomas H.",
            Age = 45,
            Birthdate = new DateTime(1977, 6, 3),
            AmountCarAccidents = 3,
            HasCar = false,
            HasDriversLicense = true,
            Income = (decimal) 5790,
            LicensePlate = "B FK 934"
        },
        new()
        {
            Name = "Franziska W.",
            Age = 43,
            Birthdate = new DateTime(1980, 1, 7),
            AmountCarAccidents = 1,
            HasCar = true,
            HasDriversLicense = false,
            Income = (decimal) 2391,
            LicensePlate = "B GT 143"
        },
        new()
        {
            Name = "Sabine W.",
            Age = 12,
            Birthdate = new DateTime(2010, 6, 3),
            AmountCarAccidents = 0,
            HasCar = false,
            HasDriversLicense = false,
            Income = 0,
            LicensePlate = ""
        }
    };
}