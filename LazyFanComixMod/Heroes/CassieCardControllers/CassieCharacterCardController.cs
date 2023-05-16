using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Cassie
{
    public class CassieCharacterCardController : CassieSharedCharacterCardController
    {
        public CassieCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[] { this.GetPowerNumeral(0, 3), this.GetPowerNumeral(1, 3) };
            IEnumerator coroutine;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            int? spellValue = 0;

            if (this.HeroTurnTakerController == null)
            {
                coroutine = this.GameController.SendMessageAction(this.Card.Title + " does not have a hand, so they cannot discard or draw cards.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Discard up to three cards.
                coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, powerNumerals[0], false, 0, storedResults, allowAutoDecide: false, cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                foreach (DiscardCardAction discard in storedResults)
                {
                    spellValue += discard.CardToDiscard.MagicNumber;
                }

                // If the Riverbank exists, do stuff. It might not - blame the Celestial Tribunal.
                if (Riverbank() != null && RiverDeck() != null)
                {
                    // Select a card under the riverbank whose cost is less than the total value of discarded cards
                    // Yes, this is messy, but it's still the cleanest way of mimicing the official SelectCardAndDoAction without access to the evenIfIndestructable flag. Battle Zones shouldn't be an issue.
                    // Do null checks first for short circuiting purposes!
                    coroutine = this.GameController.SelectCardAndDoAction(
                        new SelectCardDecision(this.GameController, this.HeroTurnTakerController, SelectionType.MoveCard, this.GameController.FindCardsWhere((Card c) => c.Location == Riverbank().UnderLocation && c.FindTokenPool("CassieCostPool") != null && c.FindTokenPool("CassieCostPool").MaximumValue != null && c.FindTokenPool("CassieCostPool").MaximumValue <= spellValue)),
                        (SelectCardDecision d) => this.GameController.MoveCard(this.HeroTurnTakerController, d.SelectedCard, this.HeroTurnTaker.Hand, false, false, false, null, true, cardSource: this.GetCardSource()),
                        false);
                    if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    coroutine = this.GameController.SendMessageAction("The Riverbank is not available, so no cards can be moved from under it.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }

                // Draw until you have 3 cards.
                coroutine = this.DrawCardsUntilHandSizeReached(this.HeroTurnTakerController, powerNumerals[1]);
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}