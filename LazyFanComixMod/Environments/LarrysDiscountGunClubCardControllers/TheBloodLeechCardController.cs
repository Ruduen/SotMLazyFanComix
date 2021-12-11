using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class TheBloodLeechCardController : CardController
    {
        public TheBloodLeechCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, StartOfTurnDamageTrigger, new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP, TriggerType.DestroyCard });
        }
        private IEnumerator StartOfTurnDamageTrigger(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            Func<int> AmmoNextToThisHeal = () => this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count() * 5;
            List<DealDamageAction> ddas = new List<DealDamageAction>();

            coroutine = this.DealDamage(null, (Card c) => c.IsHero, 1, DamageType.Infernal, storedResults: ddas, damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsVillainCharacterCard, "the villain character card with the highest HP")));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Trigger to increase healing by 5 for each ammo next to this.
            ITrigger tempIncrease = this.AddTrigger<GainHPAction>((GainHPAction g) => g.CardSource.CardController == this, (GainHPAction g) => this.GameController.IncreaseHPGain(g, AmmoNextToThisHeal(), this.GetCardSource()), new TriggerType[] { TriggerType.IncreaseHPGain, TriggerType.ModifyHPGain }, TriggerTiming.Before);

            IEnumerable<Card> damageDealers = ddas.Where((DealDamageAction dda) => dda.DamageSource.IsTarget).Select((DealDamageAction dda) => dda.DamageSource.Card).Distinct();
            if (damageDealers.Any())
            {
                coroutine = this.GameController.GainHP(this.DecisionMaker, (Card c) => damageDealers.Contains(c), 2, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            this.RemoveTrigger(tempIncrease);

            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsAmmo && c.Location == this.Card.NextToLocation), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}