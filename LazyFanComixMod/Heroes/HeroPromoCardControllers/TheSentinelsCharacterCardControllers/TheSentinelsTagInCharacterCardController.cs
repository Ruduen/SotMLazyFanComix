using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.TheSentinels;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheSentinels
{
    public class TheSentinelsTagInInstructionsCardController : TheSentinelsInstructionsCardController
    {
        public TheSentinelsTagInInstructionsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }


        public override void AddStartOfGameTriggers()
        {
            // Start of Game trigger is the best trigger for handling potential oddities with Oblivaeon.
            SetupUnderCard();
        }

        private void SetupUnderCard()
        {
            // Sanity check: Move if this is a start. The only way this should happen is if all of the characters are active and at max HP, since
            // private variables are not yet set. 
            // Not perfect, since Guise can do weird things with numerology, but this is the best doable without more TurnTaker shenanigans.

            if (this.TurnTaker.CharacterCards.Where((Card c) => c.IsInPlayAndNotUnderCard && c.HitPoints == c.MaximumHitPoints && c.IsActive).Count() == 4)
            {
                Card move;
                move = FindCharacterCard("TheIdealistCharacter");
                if (move != null)
                {
                    this.TurnTaker.MoveCard(move, this.TurnTaker.OffToTheSide);
                }
                move = FindCharacterCard("WritheCharacter");
                if (move != null)
                {
                    this.TurnTaker.MoveCard(move, this.TurnTaker.OffToTheSide);
                }
            }
        }

        private Card FindCharacterCard(string identifier)
        {
            IEnumerable<Card> cardsWhere = this.TurnTaker.GetCardsWhere((Card c) => c.Identifier == identifier);
            Card card = (from c in cardsWhere
                         where c.IsAtLocationRecursive(this.TurnTaker.PlayArea)
                         select c).FirstOrDefault();
            if (card == null)
            {
                card = cardsWhere.FirstOrDefault();
            }
            if (card == null)
            {
                card = this.FindCard(identifier, true);
            }
            return card;
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                Func<GameAction, bool> criteria = (GameAction ga) => (ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !this.Card.IsFlipped && this.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsHeroCharacterCard && c.IsActive && c.IsRealCard && c.IsInPlayAndNotUnderCard, false, null, false).Count() == 0;
                this.AddSideTrigger(base.AddTrigger<GameAction>(criteria, (GameAction ga) => this.GameController.FlipCard(this.FindCardController(this.Card), false, false, null, null, this.GetCardSource(null), true), TriggerType.FlipCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false));
                return;
            }
            this.AddSideTriggers(base.AddTargetEntersPlayTrigger((Card c) => this.Card.IsFlipped && this.CharacterCards.Contains(c), (Card c) => this.GameController.FlipCard(this.FindCardController(this.Card), false, false, null, null, this.GetCardSource(null), true), TriggerType.Hidden, TriggerTiming.After, false, true));
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.TurnTaker == this.TurnTaker && cardController.Card.IsHeroCharacterCard && cardController.Card.IsRealCard && 
                !cardController.Card.IsIncapacitatedOrOutOfGame)
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "Play one of your set-aside Heroes. Play 1 card. If you have 3 or more active characters, set this Hero aside.", this.PowerResponse(cardController), 0, null, this.GetCardSource()) };
                return list;
            }
            return null;
        }


        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveAllTriggers(true, true, true, false, false);
            this.AddSideTriggers();
            yield break;
        }

        private IEnumerator PowerResponse(CardController cardController)
        {
            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 8), // HP
                this.GetPowerNumeral(1, 1), // Played cards
                this.GetPowerNumeral(2, 3)  // Threshold for Tagging Out
            };
            List<PlayCardAction> pcas = new List<PlayCardAction>();

            IEnumerator coroutine;

            coroutine = this.GameController.SelectAndPlayCard(cardController.DecisionMaker, (Card c) => c.IsOffToTheSide && c.IsHeroCharacterCard, cardSource: this.GetCardSource(), storedResults: pcas);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Card c in pcas.Where((PlayCardAction pca) => pca.WasCardPlayed && pca.CardToPlay != null).Select((PlayCardAction pca) => pca.CardToPlay))
            {
                coroutine = this.GameController.SetHP(c, numerals[0]);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(cardController.DecisionMaker, numerals[1], false, numerals[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.TurnTaker.CharacterCards.Where((Card c) => c.IsActive).Count() < numerals[2])
            {
                coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " does not have at least " + numerals[2] + " active characters.", Priority.Low, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.MoveCard(this.DecisionMaker, cardController.Card, this.TurnTaker.OffToTheSide, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}