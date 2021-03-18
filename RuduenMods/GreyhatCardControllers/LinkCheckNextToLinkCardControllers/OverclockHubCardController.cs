using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class OverclockHubCardController : GreyhatSharedLinkCheckNextToLinkCardController
    {
        public OverclockHubCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c != null && c.IsHero && c.IsCharacter && c.Owner == this.TurnTaker, "your hero"); } }
        protected override void AddUniqueTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card != null && this.CardsLinksAreNextToHeroes.Contains(dd.DamageSource.Card) && this.CardsLinksAreNextToNonHero.Contains(dd.Target), Amount);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Fire, 3, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private int Amount(DealDamageAction dda)
        {
            return 1;
        }
    }
}