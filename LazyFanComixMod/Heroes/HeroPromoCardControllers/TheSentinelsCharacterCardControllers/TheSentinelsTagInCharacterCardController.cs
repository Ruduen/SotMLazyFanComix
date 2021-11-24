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
        }

        // TODO: Under card breaks too many things (active characters under cards are still active), so will be necessary to use 'out of play' instead! 

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

            if (this.TurnTaker.CharacterCards.Where((Card c)=>c.IsInPlayAndNotUnderCard && c.HitPoints == c.MaximumHitPoints).Count() == 4)
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
                    this.TurnTaker.MoveCard(move, this.Card.UnderLocation);
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

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveAllTriggers(true, true, true, false, false);
            this.AddSideTriggers();
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 8), // HP
                this.GetPowerNumeral(1, 1), // Played cards
                this.GetPowerNumeral(2, 3) // Threshold for Tagging Out
            };
            List<PlayCardAction> pcas = new List<PlayCardAction>();

            IEnumerator coroutine;

            // TODO: Revamp such that HP is set prior to playing, due to innate oddity with SetHP not making a target.

            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.CardWithoutReplacements.UnderLocation && c.IsCharacter,cardSource: this.GetCardSource(), storedResults: pcas);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            foreach (Card c in pcas.Where((PlayCardAction pca) => pca.WasCardPlayed && pca.CardToPlay != null).Select((PlayCardAction pca) => pca.CardToPlay))
            {
                coroutine = this.GameController.SetHP(c, numerals[0]);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(this.DecisionMaker, numerals[1], false, numerals[1], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (this.TurnTaker.CharacterCards.Where((Card c) => c.IsActive).Count() < numerals[2])
            {
                coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " does not have at least " + numerals[2] + " active characters.", Priority.Low, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SelectAndMoveCard(this.DecisionMaker, (Card c) => c.IsActive && this.TurnTaker.CharacterCards.Contains(c), this.CardWithoutReplacements.UnderLocation, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        // TODO: Weird Incap-undoing power which plays a card from under this? 
    }
}