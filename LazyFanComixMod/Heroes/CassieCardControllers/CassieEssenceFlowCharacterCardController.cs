using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace LazyFanComix.Cassie
{
    public class CassieEssenceFlowCharacterCardController : CassieSharedCharacterCardController
    {
        public CassieEssenceFlowCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 2),
                this.GetPowerNumeral(1, 3)
            };
            IEnumerator coroutine;
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            Card selectedCard;

            if (!this.TurnTaker.IsHero)
            {
                coroutine = this.GameController.SendMessageAction(this.Card.Title + " does not have a hand, so they cannot move or add cards.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                if (Riverbank() != null && RiverDeck() != null)
                {
                    // Move a card to the bottom of the river deck.
                    coroutine = this.GameController.SelectAndMoveCard(this.HeroTurnTakerController, (Card c) => this.HeroTurnTaker.Hand.Cards.Contains(c), RiverDeck(), true, storedResults: scdResults, cardSource: this.GetCardSource());
                    if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    // If there is an aqua cost on it, then select a card with a value of up to 2 more.
                    selectedCard = this.GetSelectedCard(scdResults);
                    if (selectedCard != null && selectedCard.FindTokenPool("CassieCostPool") != null && selectedCard.FindTokenPool("CassieCostPool").MaximumValue != null)
                    {
                        int spellValue = (int)selectedCard.FindTokenPool("CassieCostPool").MaximumValue + powerNumerals[0];

                        // Select a card under the riverbank whose cost qualifies.
                        // Yes, this is messy, but it's still the cleanest way of mimicing the official SelectCardAndDoAction without access to the evenIfIndestructable flag. Battle Zones shouldn't be an issue.

                        coroutine = this.GameController.SelectCardAndDoAction(
                            new SelectCardDecision(this.GameController, this.HeroTurnTakerController, SelectionType.MoveCard, this.GameController.FindCardsWhere((Card c) => c.Location == this.Riverbank().UnderLocation && c.FindTokenPool("CassieCostPool") != null && c.FindTokenPool("CassieCostPool").MaximumValue != null && c.FindTokenPool("CassieCostPool").MaximumValue <= spellValue)),
                            (SelectCardDecision d) => this.GameController.MoveCard(this.HeroTurnTakerController, d.SelectedCard, this.HeroTurnTaker.Hand, false, false, false, null, true, null, null, null, true, false, null, false, false, false, false, this.GetCardSource()),
                            false);
                        if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else
                    {
                        coroutine = this.GameController.SendMessageAction("The selected card does not have a Aqua Cost, so no new card can be gained.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
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
                        coroutine = this.GameController.SelectAndGainHP(this.DecisionMaker, 2, false, cardSource: this.GetCardSource());
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