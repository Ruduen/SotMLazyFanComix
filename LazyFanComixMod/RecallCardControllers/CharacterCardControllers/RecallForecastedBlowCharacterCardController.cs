using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecallForecastedBlowCharacterCardController : PromoDefaultCharacterCardController
    {
        public string str;

        public RecallForecastedBlowCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1,1)
            };
            IEnumerator coroutine;

            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => CardsUnderSelf());

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), numerals[1], DamageType.Melee, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        protected int CardsUnderSelf()
        {
            // Add sanity check. Unknown how a unit test got into a bad state, but it happened.
            if ( this.CharacterCard?.UnderLocation?.Cards == null)
            {
                return 0;
            }
            return this.CharacterCard.UnderLocation.Cards.Count();
        }

        // TODO: Replace Incap with something more unique!
    }
}