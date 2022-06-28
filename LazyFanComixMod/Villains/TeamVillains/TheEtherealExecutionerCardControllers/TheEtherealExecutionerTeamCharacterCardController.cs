using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class TheEtherealExecutionerTeamCharacterCardController : VillainTeamCharacterCardController
    {
        public TheEtherealExecutionerTeamCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.Card.MayRegainHPAboveMaxHP = true;
            this.SpecialStringMaker.ShowTokenPool(this.Card.FindTokenPool("RespawnPool")).Condition = (() => this.Card.IsInPlayAndNotUnderCard);
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                // Play the top card of the deck.
                Func<PhaseChangeAction, IEnumerator> playTopCardResponse = new Func<PhaseChangeAction, IEnumerator>((PhaseChangeAction pca) => this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, cardSource: this.GetCardSource()));

                this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, playTopCardResponse, TriggerType.PlayCard));
                if (this.TurnTaker.IsAdvanced)
                {
                    this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, playTopCardResponse, TriggerType.PlayCard));
                }
            }
            else
            {
                this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, IncreaseTokenOrReviveResponse, new TriggerType[] { TriggerType.AddTokensToPool, TriggerType.ModifyTokens, TriggerType.FlipCard, TriggerType.GainHP }));
            }
        }

        private IEnumerator IncreaseTokenOrReviveResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;
            TokenPool respawnPool = this.CharacterCard.FindTokenPool("RespawnPool");
            if (respawnPool != null)
            {
                if (respawnPool.CurrentValue >= 3)
                {
                    coroutine = this.GameController.SendMessageAction("Removing " + respawnPool.CurrentValue + " Respawn tokens and reviving {TheEtherealExecutionerTeam}!", Priority.Low, this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.RemoveTokensFromPool(respawnPool, respawnPool.CurrentValue, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.FlipCard(this.CharacterCardController, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.MakeTargettable(this.CharacterCard, 12, 12, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.BulkMoveCards(this.TurnTakerController, this.TurnTaker.OutOfGame.Cards.Where((Card c) => c.Owner == this.TurnTaker), this.TurnTaker.Deck, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.UpdateTurnPhasesForTurnTaker(this.TurnTakerController, false);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    // Otherwise, add 1 token.
                    coroutine = this.GameController.AddTokensToPool(respawnPool, 1, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.SendMessageAction("Adding a respawn token to {TheEtherealExecutionerTeam} for a total of " + respawnPool.CurrentValue + ". [i]This is not shown due to engine restrictions.[/i]", Priority.Low, this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}