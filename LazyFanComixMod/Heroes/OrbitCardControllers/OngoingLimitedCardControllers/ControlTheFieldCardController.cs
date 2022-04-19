using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class ControlTheFieldCardController : CardController
    {
        public ControlTheFieldCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker,
                (PhaseChangeAction pca) => this.GameController.SelectAndDestroyCards(this.DecisionMaker,new LinqCardCriteria((Card c)=>c.IsCover,"cover"),2,false,2,cardSource: this.GetCardSource()),
                TriggerType.DestroyCard);
            this.AddReduceDamageTrigger((DealDamageAction dda) => dda.Target == this.CharacterCard, (DealDamageAction dda) => this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("cover")).Count());
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int powerNumeral = this.GetPowerNumeral(0, 2);

            // Search the deck for 2 cover and play them. 
            coroutine = this.SearchForCards(this.DecisionMaker, true, false, powerNumeral, powerNumeral, new LinqCardCriteria((Card c) => c.DoKeywordsContain("cover")), true, false, false, shuffleAfterwards: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}