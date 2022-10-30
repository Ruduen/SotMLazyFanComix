using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class MakeshiftDiversionCardController : SharedIsolateCardController
    {
        public MakeshiftDiversionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddUniqueTriggers()
        {
            this.AddReduceDamageTrigger((Card c) => c == this.CharacterCard, 1);
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, MoveCardResponse, TriggerType.MoveCard);
        }

        private IEnumerator MoveCardResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            TurnTakerController env = this.GameController.FindEnvironmentTurnTakerController();
            if (env.TurnTaker.Deck.IsEmpty)
            {
                if (env.TurnTaker.Trash.IsEmpty)
                {
                    coroutine = this.GameController.SendMessageAction("There are no cards in the environment deck or trash, so no card can be moved.", Priority.Low, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    coroutine = this.GameController.ShuffleTrashIntoDeck(env, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }

            if (env.TurnTaker.Deck.TopCard != null)
            {
                List<SelectLocationDecision> sldResults = new List<SelectLocationDecision>();
                coroutine = this.FindVillainDeck(this.DecisionMaker, SelectionType.MoveCardOnDeck, sldResults, (Location l) => true); ;
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCard(this.DecisionMaker, env.TurnTaker.Deck.TopCard, this.GetSelectedLocation(sldResults), showMessage: true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}