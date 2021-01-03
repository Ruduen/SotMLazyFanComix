using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: TEST!

namespace RuduenWorkshop.Cassie
{
    public class CassieEssenceFlowCharacterCardController : HeroCharacterCardController, ICassieRiverSharedCardController
    {
        public string str;
        protected static Location _riverDeck;
        protected static Card _riverbank;

        public CassieEssenceFlowCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            _riverbank = null;
            _riverDeck = null;
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

            // Move a card to the bottom of the river deck.
            coroutine = this.GameController.SelectAndMoveCard(this.DecisionMaker, (Card c) => this.HeroTurnTaker.Hand.Cards.Contains(c), RiverDeck(), true, storedResults: scdResults, cardSource: this.GetCardSource());
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If there is an aqua cost on it, then select a card with a value of up to 2 more. 
            selectedCard = this.GetSelectedCard(scdResults);
            if (selectedCard != null && selectedCard.FindTokenPool("CassieCostPool").MaximumValue != null)
            {
                int spellValue = (int)selectedCard.FindTokenPool("CassieCostPool").MaximumValue + powerNumerals[0];

                // Select a card under the riverbank whose cost qualifies.
                // Yes, this is messy, but it's still the cleanest way of mimicing the official SelectCardAndDoAction without access to the evenIfIndestructable flag. Battle Zones shouldn't be an issue.

                coroutine = this.GameController.SelectCardAndDoAction(
                    new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCard, this.GameController.FindCardsWhere((Card c) => c.Location == this.Riverbank().UnderLocation && c.FindTokenPool("CassieCostPool") != null && c.FindTokenPool("CassieCostPool").MaximumValue != null && c.FindTokenPool("CassieCostPool").MaximumValue <= spellValue)),
                    (SelectCardDecision d) => this.GameController.MoveCard(this.DecisionMaker, d.SelectedCard, this.HeroTurnTaker.Trash, false, false, false, null, true, null, null, null, true, false, null, false, false, false, false, this.GetCardSource()),
                    false);
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Draw until you have 3 cards.
            coroutine = this.DrawCardsUntilHandSizeReached(this.DecisionMaker, powerNumerals[1]);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        }


        // TODO: Replace with something more unique!
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.DecisionMaker);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }

        public Location RiverDeck()
        {
            if (CassieEssenceFlowCharacterCardController._riverDeck == null)
            {
                CassieEssenceFlowCharacterCardController._riverDeck = this.TurnTaker.FindSubDeck("RiverDeck");
            }
            return CassieEssenceFlowCharacterCardController._riverDeck;
        }

        public Card Riverbank()
        {
            if (CassieEssenceFlowCharacterCardController._riverbank == null)
            {
                CassieEssenceFlowCharacterCardController._riverbank = this.FindCard("Riverbank", false);
            }
            return CassieEssenceFlowCharacterCardController._riverbank;
        }
    }
}