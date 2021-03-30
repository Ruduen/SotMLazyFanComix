using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class FlareRelayCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == this.TurnTaker, "hero character"); } }

        public FlareRelayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Fire, 3, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (this.GetCardThisCardIsNextTo() != null && cardController.Card == this.GetCardThisCardIsNextTo())
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "This hero deals 1 target 3 fire damage.", this.PowerResponse(cardController), 0, null, base.GetCardSource(null)) };
                return list;
            }
            return null;
        }

        private IEnumerator PowerResponse(CardController cardWithPower)
        {
            HeroTurnTakerController hero = cardWithPower.HeroTurnTakerController;
            int[] numerals = new int[]{
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 3)
            };

            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(hero, new DamageSource(this.GameController, cardWithPower.Card), numerals[1], DamageType.Fire, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}