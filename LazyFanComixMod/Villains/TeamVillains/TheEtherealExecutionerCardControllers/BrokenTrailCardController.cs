using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class BrokenTrailCardController : TheEtherealExecutionerSharedCardController
    {
        public BrokenTrailCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<MoveCardAction> mcaResults = new List<MoveCardAction>();
            IEnumerator coroutine;

            coroutine = this.DiscardCardsFromTopOfDeck(this.FindEnvironment(), 1 + this.CountObservationCards(), storedResults: mcaResults, showMessage: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            bool hasTarget = mcaResults.Any((MoveCardAction mca) => mca.WasCardMoved && mca.CardToMove.IsTarget);
            bool hasNonTarget = mcaResults.Any((MoveCardAction mca) => mca.WasCardMoved && !mca.CardToMove.IsTarget);

            string message = "No target or non-target was discarded.";
            if (hasTarget && hasNonTarget)
            {
                message = "Both a target and a non-Target were discarded, so {TheEtherealExecutionerTeam} will heal and deal damage.";
            }
            else if (hasTarget) 
            {
                message = "A target was discarded, so {TheEtherealExecutionerTeam} will heal.";
            }
            else if (hasNonTarget)
            {
                message = "A non-target was discarded, so {TheEtherealExecutionerTeam} will deal damage.";
            }

            coroutine = this.GameController.SendMessageAction(message, Priority.Low, this.GetCardSource(), showCardSource: true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (hasTarget)
            {
                // Heal 4. 
                coroutine = this.GameController.GainHP(this.CharacterCard, 4, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            if (hasNonTarget)
            {
                // 2 Damage to Each Other Target. 
                coroutine = this.DealDamage(this.CharacterCard, (Card c) => c.IsTarget && c != this.CharacterCard, 2, DamageType.Toxic);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}