using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Ra
{
    public class RaPiercingBlastCharacterCardController : PromoDefaultCharacterCardController
    {
        public RaPiercingBlastCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1), // Number of targets.
                this.GetPowerNumeral(1, 1), // Amount of damage.
            };
            List<DealDamageAction> storedResults = new List<DealDamageAction>();

            IEnumerator coroutine;

            // Deal up to 1 targets damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Fire, powerNumerals[0], false, powerNumerals[0], true, storedResultsDamage: storedResults, cardSource: this.GetCardSource(null));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If damaged targets are destroyed. This is weird in case of redirectors, but blame Wrecking Uppercut for doing it this way, opposed to 'original target'.
            
            IEnumerable<Card> damageTargets = from dd in storedResults
                                              select dd.Target;

            if (damageTargets.Count<Card>() > 0)
            {
                DestroyCardJournalEntry destroyCardJournalEntry = (from dcje in this.Journal.DestroyCardEntriesThisTurn()
                                                                   where damageTargets.Contains(dcje.Card)
                                                                   select dcje).LastOrDefault<DestroyCardJournalEntry>();
                if (destroyCardJournalEntry != null && destroyCardJournalEntry.Card != null)
                {
                    coroutine = this.GameController.UsePower(this.CardWithoutReplacements, 0, true, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}