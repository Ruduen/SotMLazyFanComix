using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class BandwidthHubCardController : GreyhatSharedLinkCheckNextToLinkCardController
    {
        public BandwidthHubCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c != null && c.IsHero && c.IsCharacter && c.Owner == this.TurnTaker, "your hero"); } }

        protected override void AddUniqueTriggers()
        {
            this.AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card != null && this.CardsLinksAreNextToHeroes.Contains(dd.Target) && this.CardsLinksAreNextToNonHero.Contains(dd.DamageSource.Card), Amount);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private int Amount(DealDamageAction dda)
        {
            return 1;
        }
    }
}