using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class ProxyRelayCardController : GreyhatSharedLinkCardController
    {
        protected override LinqCardCriteria NextToCriteria { get { return new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == this.TurnTaker, "hero character"); } }

        public ProxyRelayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (this.GetCardThisCardIsNextTo() != null && cardController.Card == this.GetCardThisCardIsNextTo())
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, " Draw 1 Card. Until the start of your next turn, reduce damage dealt to this hero by 1.", this.PowerResponse(cardController), 0, null, base.GetCardSource(null)) };
                return list;
            }
            return null;
        }

        private IEnumerator PowerResponse(CardController cardWithPower)
        {
            HeroTurnTakerController hero = cardWithPower.HeroTurnTakerController;
            int[] numerals = {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };

            IEnumerator coroutine;
            coroutine = this.GameController.DrawCards(hero, numerals[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(numerals[1]);
            rdse.TargetCriteria.IsSpecificCard = cardWithPower.Card;
            rdse.TargetCriteria.OwnedBy = hero.TurnTaker;
            rdse.TargetCriteria.OutputString = hero.Name;
            rdse.UntilStartOfNextTurn(hero.TurnTaker);
            coroutine = this.AddStatusEffect(rdse, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}