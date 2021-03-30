using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.MrFixer
{
    public class MrFixerFlowingStrikeCharacterCardController : PromoDefaultCharacterCardController
    {
        public MrFixerFlowingStrikeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<MoveCardAction> mcaResult = new List<MoveCardAction>();
            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1), // Number of targets.
                this.GetPowerNumeral(1, 1), // Amount of damage.
                this.GetPowerNumeral(2, 3) // Amount to discard.
            };

            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            IEnumerator coroutine;

            // Deal target damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), numerals[1], DamageType.Melee, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, numerals[1], DamageType.Melee, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Discard 3 cards.
            coroutine = this.GameController.DiscardTopCards(this.DecisionMaker, this.TurnTaker.Deck, numerals[2], mcaResult, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            IEnumerable<Card> validPlays = mcaResult.Where((MoveCardAction mca) => mca.WasCardMoved && (mca.CardToMove.IsTool || mca.CardToMove.IsStyle || mca.CardToMove.Identifier == "Harmony")).Select((MoveCardAction mca) => mca.CardToMove);

            if (validPlays.Count() > 0)
            {
                coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, validPlays, true, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("No Tool, Style, or copy of {Harmony} was discarded.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}