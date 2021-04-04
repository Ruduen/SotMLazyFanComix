using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecallUnstableJumpCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public RecallUnstableJumpCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            List<DealDamageAction> ddaResult = new List<DealDamageAction>();
            int[] numerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };
            IEnumerator coroutine;

            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => CardsUnderSelf());

            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, numerals[0], DamageType.Psychic,storedResults: ddaResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);

            if (ddaResult.Any() && ddaResult.FirstOrDefault().Target == this.CharacterCard && ddaResult.FirstOrDefault().DidDealDamage)
            {
                coroutine = this.GameController.MoveCards(this.DecisionMaker, this.TurnTaker.Deck.Cards.Take(numerals[1]), this.CharacterCard.UnderLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.TakeAFullTurnNow(this.DecisionMaker);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }
        protected int CardsUnderSelf()
        {
            return this.CharacterCard.UnderLocation.Cards.Count();
        }

        // TODO: Replace Incap with something more unique!
    }
}