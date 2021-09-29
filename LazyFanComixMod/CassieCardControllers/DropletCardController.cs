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
            coroutine = this.GameController.GainHP(this.DecisionMaker, (Card card) => card.IsHeroCharacterCard, 1, null, false, null, null, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Yes/No question to determine reset-move!
            YesNoAmountDecision yesNoDecision = new YesNoAmountDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCard, Riverbank().UnderLocation.Cards.Count(), cardSource: this.GetCardSource());
            coroutine = this.GameController.MakeDecisionAction(yesNoDecision);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.DidPlayerAnswerYes(yesNoDecision))
            {
                // Reset river into Deck.
                Card remainingCard = Riverbank().UnderLocation.Cards.FirstOrDefault();
                while (remainingCard != null)
                {
                    coroutine = this.GameController.MoveCard(this.DecisionMaker, remainingCard, RiverDeck(), toBottom: true, evenIfIndestructible: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    remainingCard = Riverbank().UnderLocation.Cards.FirstOrDefault();
                }

                // Then, move the top four river cards to the riverbank. That should already exist due to being a non-real card.
                coroutine = this.GameController.MoveCards(this.DecisionMaker, RiverDeck().GetTopCards(4), Riverbank().UnderLocation);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}