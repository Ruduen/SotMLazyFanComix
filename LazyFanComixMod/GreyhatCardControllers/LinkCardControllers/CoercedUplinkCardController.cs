using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

// Manually tested!

namespace LazyFanComix.Greyhat
{
    public class CoercedUplinkCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsTarget && !c.IsHero, "non-hero target"); } }

        public CoercedUplinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator UniquePlay()
        {
            Card nextTo = this.GetCardThisCardIsNextTo(true);
            IEnumerator coroutine;
            if (nextTo != null)
            {
                // Damage.
                coroutine = this.DealDamage(this.CharacterCard, nextTo, 2, DamageType.Lightning, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}