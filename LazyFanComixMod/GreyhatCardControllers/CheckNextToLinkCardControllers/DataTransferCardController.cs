using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Greyhat
{
    public class DataTransferCardController : GreyhatSharedNetworkCardController
    {
        public DataTransferCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override IEnumerator UniquePlay()
        {
            IEnumerator coroutine;

            // Select players next to links to draw.
            SelectTurnTakersDecision sttd = new SelectTurnTakersDecision(this.GameController, this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => this.CardsLinksAreNextToHeroes.Where((Card c) => c.Owner == tt).Count() > 0), SelectionType.DrawCard, allowAutoDecide: true, cardSource: this.GetCardSource());
            coroutine = this.GameController.SelectTurnTakersAndDoAction(sttd, SelectedPlayerDrawsCards, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Damage Portion.
            coroutine = this.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, (Card c) => this.CardsLinksAreNextToNonHero.Contains(c), 2, DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SelectedPlayerDrawsCards(TurnTaker tt)
        {
            if (tt.IsHero)
            {
                HeroTurnTakerController httc = this.GameController.FindHeroTurnTakerController(tt.ToHero());
                IEnumerator coroutine = this.GameController.DrawCards(httc, this.CardsLinksAreNextToHeroes.Where((Card c) => c.Owner == tt).Count(), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}