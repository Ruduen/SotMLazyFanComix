using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Synthesist
{
    public class SynthesistCharacterCardController : PromoDefaultCharacterCardController
    {
        private Location _relicDeck;

        public SynthesistCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PutRelicIntoPlayTrigger, TriggerType.PutIntoPlay);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3),
                this.GetPowerNumeral(2, 1)
            };
            IEnumerator coroutine;

            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.CardToDealDamage,
                new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && c != this.Card),
                storedDecision, false, false,
                new DealDamageAction(this.GetCardSource(), null, null, powerNumerals[2], DamageType.Infernal)
            );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


            if (storedDecision.FirstOrDefault().SelectedCard != null)
            {
                Card target = storedDecision.FirstOrDefault().SelectedCard;
                // The selected target deals another 3 damage and themselves 1 damage. 
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, target), powerNumerals[1], DamageType.Infernal, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, target), target, powerNumerals[2], DamageType.Infernal, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        protected IEnumerator PutRelicIntoPlayTrigger(PhaseChangeAction action)
        {
            // If there are no other targets in play and the relic deck is not empty...
            if (this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndNotUnderCard && c != this.Card).Count() == 0)
            {
                if (RelicDeck().Cards.Count() > 0)
                {
                    IEnumerator coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, RelicDeck(), 1, 1, new LinqCardCriteria((Card c) => c.IsRelic, "relic"), new List<MoveCardDestination> { new MoveCardDestination(this.HeroTurnTaker.PlayArea, false, true) }, true, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }

        protected Location RelicDeck()
        {
            if (_relicDeck == null)
            {
                _relicDeck = this.HeroTurnTaker.FindSubDeck("SynthesistRelicDeck");
            }
            return _relicDeck;
        }

        // TODO: Replace Incap with something more unique!
    }
}