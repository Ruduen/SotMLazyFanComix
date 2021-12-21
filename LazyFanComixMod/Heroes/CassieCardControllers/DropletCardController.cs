using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Cassie
{
    // TODO: TEST!
    public class DropletCardController : CassieRiverSharedCardController
    {
        public DropletCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(GetRiverbankString, null, null);
        }

        public override IEnumerator Play()
        {
            // Heal.
            IEnumerator coroutine;
            coroutine = this.GameController.GainHP(this.HeroTurnTakerController, (Card card) => card.IsHeroCharacterCard, 1, null, false, null, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Yes/No question to determine reset-move!
            YesNoAmountDecision yesNoDecision = new YesNoAmountDecision(this.GameController, this.HeroTurnTakerController, SelectionType.MoveCard, Riverbank().UnderLocation.Cards.Count(), cardSource: this.GetCardSource());
            coroutine = this.GameController.MakeDecisionAction(yesNoDecision);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yesNoDecision))
            {
                // Reset river into Deck.
                coroutine = this.GameController.MoveCards(this.DecisionMaker, this.Riverbank().UnderLocation.Cards, this.RiverDeck(), true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}