using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Cassie
{
    public class StreamShotCardController : CassieRiverSharedCardController
    {
        public StreamShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<MoveCardAction> mcaResults = new List<MoveCardAction>();
            Location riverDeck = this.RiverDeck();

            // Select and move a card from the riverbank to the bottom of the River deck.
            coroutine = this.GameController.SelectCardAndDoAction(new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCard, this.GameController.FindCardsWhere((Card c) => c.Location == this.Riverbank().UnderLocation)),
                (SelectCardDecision d) => this.GameController.MoveCard(this.DecisionMaker, d.SelectedCard, RiverDeck(), toBottom: true, evenIfIndestructible: true, storedResults: mcaResults, cardSource: this.GetCardSource()),
                false);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (mcaResults.Count > 0 && mcaResults.FirstOrDefault().CardToMove.MagicNumber != null)
            {
                // Deal damage equal to the moved card.
                coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), (int)mcaResults.FirstOrDefault().CardToMove.MagicNumber, DamageType.Cold, 1, false, 1, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Move a card from the River deck to the Riverbank.
            coroutine = this.GameController.MoveCards(this.DecisionMaker, RiverDeck().GetTopCards(1), Riverbank().UnderLocation, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}