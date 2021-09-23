using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

// TODO: TEST!

namespace LazyFanComix.Cassie
{
    public class CassieCharacterCardController : HeroCharacterCardController, ICassieRiverSharedCardController
    {
        public string str;
        protected static Location _riverDeck;
        protected static Card _riverbank;

        public CassieCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            _riverbank = null;
            _riverDeck = null;
        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[] { this.GetPowerNumeral(0, 3), this.GetPowerNumeral(1, 3) };
            IEnumerator coroutine;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            int? spellValue = 0;

            if (this.DecisionMaker == null)
            {
                coroutine = this.GameController.SendMessageAction(this.Card.Title + " does not have a hand, so they cannot discard or draw cards.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Discard up to three cards.
                coroutine = this.GameController.SelectAndDiscardCards(this.DecisionMaker, powerNumerals[0], false, 0, storedResults, allowAutoDecide: false, cardSource: this.GetCardSource());
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
                        new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.MoveCard, this.GameController.FindCardsWhere((Card c) => c.Location == Riverbank().UnderLocation && c.FindTokenPool("CassieCostPool") != null && c.FindTokenPool("CassieCostPool").MaximumValue != null && c.FindTokenPool("CassieCostPool").MaximumValue <= spellValue)),
                        (SelectCardDecision d) => this.GameController.MoveCard(this.DecisionMaker, d.SelectedCard, this.HeroTurnTaker.Hand, false, false, false, null, true, null, null, null, true, false, null, false, false, false, false, this.GetCardSource()),
                        false);
                    if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }

                // Draw until you have 3 cards.
                coroutine = this.DrawCardsUntilHandSizeReached(this.DecisionMaker, powerNumerals[1]);
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        //public IEnumerator MoveCardTest(SelectCardDecision d)
        //{
        //    IEnumerator coroutine = this.GameController.MoveCard(this.DecisionMaker, d.SelectedCard, this.HeroTurnTaker.Trash, false, false, false, null, false, null, null, null, true, false, null, false, false, false, false, this.GetCardSource());
        //    if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        //}

        //public bool ConfirmCost(Card c, int? spellValue)
        //{
        //    if (c.Location == this.Riverbank().UnderLocation)
        //    {
        //        int? tokenPoolValue = c.FindTokenPool("CassieCostPool").MaximumValue;
        //        if (tokenPoolValue <= spellValue)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }

        public Location RiverDeck()
        {
            if (CassieCharacterCardController._riverDeck == null)
            {
                CassieCharacterCardController._riverDeck = this.TurnTaker.FindSubDeck("RiverDeck");
            }
            return CassieCharacterCardController._riverDeck;
        }

        public Card Riverbank()
        {
            if (CassieCharacterCardController._riverbank == null)
            {
                CassieCharacterCardController._riverbank = this.FindCard("Riverbank", false);
            }
            return CassieCharacterCardController._riverbank;
        }
    }
}