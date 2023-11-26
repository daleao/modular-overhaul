namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using System.Globalization;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var taxpayer = Game1.player;
        if (!taxpayer.ShouldPayTaxes())
        {
            // clear any outdated data just in case
            taxpayer.Write(DataKeys.SeasonIncome, null);
            taxpayer.Write(DataKeys.BusinessExpenses, null);
            taxpayer.Write(DataKeys.PercentDeductions, null);
            taxpayer.Write(DataKeys.DebtOutstanding, null);
            return;
        }

        if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
        {
            PostalService.Send(Mail.FrsIntro);
        }

        var amountSold = Game1.game1.GetTotalSoldByPlayer(taxpayer);
        if (amountSold > 0 && !PostalService.HasSent(Mail.FrsIntro))
        {
            PostalService.Send(Mail.FrsIntro);
        }

        Log.D(
            amountSold > 0
                ? $"[TXS]: {Game1.player} sold items worth a total of {amountSold}g on day {Game1.dayOfMonth} of {Game1.currentSeason}."
                : $"[TXS]: No items were sold on day {Game1.dayOfMonth} of {Game1.currentSeason}.");

        var dayIncome = amountSold;
        switch (Game1.dayOfMonth)
        {
            case 28 when ProfessionsModule.ShouldEnable && taxpayer.professions.Contains(Farmer.mariner):
            {
                CheckDeductions(taxpayer);
                goto default;
            }

            case 1:
            {
                if (Game1.currentSeason == "spring" && Game1.year == 1)
                {
                    break;
                }

                CheckIncomeStatement(taxpayer, ref dayIncome);
                if (taxpayer.IsMainPlayer && Game1.currentSeason == "spring")
                {
                    CheckPropertyStatement(taxpayer, ref dayIncome);
                }

                goto default;
            }

            case 8:
            case 22:
            {
                if (taxpayer.IsMainPlayer)
                {
                    Game1.getFarm().Appraise();
                }

                goto default;
            }

            default:
            {
                CheckOutstanding(taxpayer, ref dayIncome);
            }

            break;
        }

        if (dayIncome < amountSold)
        {
            Game1.player.Write(DataKeys.LatestAmountWithheld, (amountSold - dayIncome).ToString());
            ModEntry.EventManager.Enable<TaxDayStartedEvent>();
        }

        taxpayer.Increment(DataKeys.SeasonIncome, dayIncome);
        Log.T(dayIncome > 0
            ? $"[TXS]: Actual income was increased by {dayIncome}g after debts and payments."
            : "[TXS]: Day's income was entirely consumed by debts and payments.");
    }

    private static void CheckDeductions(Farmer taxpayer)
    {
        var deductible = taxpayer.GetConservationistPriceMultiplier() - 1;
        if (deductible <= 0f)
        {
            return;
        }

        deductible = Math.Min(deductible, ProfessionsModule.Config.ConservationistTaxDeductionCeiling);
        taxpayer.Write(DataKeys.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
        taxpayer.Write(DataKeys.LatestTaxDeductions, deductible.ToString(CultureInfo.InvariantCulture));
        PostalService.Send(Mail.FrsDeduction);
        Log.I(
            FormattableString.CurrentCulture(
                $"[TXS]: Farmer {taxpayer.Name} is eligible for income tax deductions of {deductible:0.0%}.") +
            (deductible >= 1f
                ? $" No income taxes will be charged for {Game1.currentSeason}."
                : string.Empty));
    }

    private static void CheckIncomeStatement(Farmer taxpayer, ref int dayIncome)
    {
        var (amountDue, _, _, _, _) = RevenueService.CalculateTaxes(taxpayer);
        Game1.player.Write(DataKeys.LatestDueIncomeTax, amountDue.ToString());
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            PostalService.Send(Mail.FrsNotice);
            Log.I("[TXS]: Income tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue >= dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * TaxesModule.Config.IncomeTaxLatenessFine), 100);
                taxpayer.Increment(DataKeys.DebtOutstanding, outstanding + penalties);
                Game1.player.Write(DataKeys.LatestOutstandingIncomeTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.FrsOutstanding);
                Log.I(
                    $"[TXS]: {taxpayer.Name} did not carry enough funds to cover the income tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        taxpayer.Write(DataKeys.SeasonIncome, "0");
        taxpayer.Write(DataKeys.BusinessExpenses, "0");
    }

    private static void CheckPropertyStatement(Farmer taxpayer, ref int dayIncome)
    {
        var farm = Game1.getFarm();
        var agricultureValue = farm.Read<int>(DataKeys.AgricultureValue);
        var livestockValue = farm.Read<int>(DataKeys.LivestockValue);
        var buildingValue = farm.Read<int>(DataKeys.BuildingValue);
        var usableTiles = farm.Read(DataKeys.UsableTiles, -1);
        if (usableTiles < 0)
        {
            var origin = farm.GetMainFarmHouseEntry();
            usableTiles = origin.FloodFill(
                farm.Map.DisplayWidth,
                farm.Map.DisplayHeight,
                p => farm.doesTileHaveProperty(p.X, p.Y, "Diggable", "Back") is not null).Count;
            farm.Write(DataKeys.UsableTiles, usableTiles.ToString());
        }

        var currentUsePct = farm.Read<float>(DataKeys.UsedTiles) / usableTiles;
        var amountDue = (int)(((agricultureValue + livestockValue) * currentUsePct * TaxesModule.Config.UsedTileTaxRate) +
                          ((agricultureValue + livestockValue) * (1f - currentUsePct) * TaxesModule.Config.UnusedTileTaxRate) +
                          (buildingValue * TaxesModule.Config.BuildingTaxRate));
        Game1.player.Write(DataKeys.LatestDuePropertyTax, amountDue.ToString());
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            PostalService.Send(Mail.LewisNotice);
            Log.I("[TXS]: Property tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue > dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * TaxesModule.Config.PropertyTaxLatenessFine), 500);
                taxpayer.Increment(DataKeys.DebtOutstanding, outstanding + penalties);
                Game1.player.Write(DataKeys.LatestOutstandingPropertyTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.LewisOutstanding);
                Log.I(
                    $"[TXS]: {taxpayer.Name} did not carry enough funds to cover the property tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        farm.Write(DataKeys.AgricultureValue, "0");
        farm.Write(DataKeys.LivestockValue, "0");
        farm.Write(DataKeys.BuildingValue, "0");
        farm.Write(DataKeys.UsedTiles, "0");
    }

    private static void CheckOutstanding(Farmer taxpayer, ref int dayIncome)
    {
        var debtOutstanding = taxpayer.Read<int>(DataKeys.DebtOutstanding);
        if (debtOutstanding <= 0)
        {
            return;
        }

        // only seize shipping bin income
        int withheld;
        if (dayIncome >= debtOutstanding)
        {
            withheld = debtOutstanding;
            debtOutstanding = 0;
            Log.I(
                $"[TXS]: {taxpayer.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
        }
        else
        {
            withheld = dayIncome;
            debtOutstanding -= withheld;
            var interest = (int)Math.Round(debtOutstanding * TaxesModule.Config.AnnualInterest / 112f);
            debtOutstanding += interest;
            Log.I(
                $"[TXS]: {taxpayer.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
        }

        dayIncome -= withheld;
        taxpayer.Write(DataKeys.DebtOutstanding, debtOutstanding.ToString());
    }
}
