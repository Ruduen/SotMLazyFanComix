using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class SeeItAgainCardController : CardController
    {
        public SeeItAgainCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // One other player may draw. 
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => OneOtherPlayerDraws(), TriggerType.DrawCard);
        }

        private IEnumerator OneOtherPlayerDraws()
        {
            List<SelectLocationDecision> sldResults = new List<SelectLocationDecision>();
            IEnumerator coroutine;
            coroutine = this.GameController.SelectADeck(this.DecisionMaker, SelectionType.DiscardFromDeck, (Location l) => l.IsHero, sldResults, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Location l in sldResults.Select((SelectLocationDecision sld) => sld.SelectedLocation.Location))
            {
                if (l != null)
                {
                    coroutine = this.GameController.DiscardTopCardsOfLocations(this.DecisionMaker, new List<Location>() { l }, 3, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.IsInTrash && this.IsEquipment(c));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}