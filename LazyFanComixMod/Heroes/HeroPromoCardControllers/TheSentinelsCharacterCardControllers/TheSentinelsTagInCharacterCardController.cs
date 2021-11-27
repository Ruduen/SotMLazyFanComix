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
        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.TurnTaker == this.TurnTaker && cardController.Card.IsHeroCharacterCard && cardController.Card.IsRealCard &&
                !cardController.Card.IsIncapacitatedOrOutOfGame)
            {
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "This Hero deals 1 Target 1 Melee Damage. Switch this Hero with your set aside Hero. Draw 1 Card or Play 1 Card.", this.PowerResponse(cardController), 0, null, this.GetCardSource()) };
                return list;
            }
            return null;
        }

        private IEnumerator PowerResponse(CardController cardController)
        {
            Func<Card, IEnumerator> switchToHero = (Card c) =>
            {
                return this.GameController.SwitchCards(cardController.Card, c, cardSource: this.GetCardSource());
            };

            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 1), // Played cards
                this.GetPowerNumeral(0, 1), // Played cards
                this.GetPowerNumeral(0, 1), // Played cards
                this.GetPowerNumeral(0, 1), // Played cards
            };

            IEnumerator coroutine;

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, cardController.Card), numerals[1], DamageType.Melee, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            List<Function> list = new List<Function>();
            foreach (Card c in this.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location == this.TurnTaker.OffToTheSide && c.IsHeroCharacterCard)))
            {
                list.Add(new Function(this.DecisionMaker, "Switch " + cardController.Card.Title + " with " + c.Title, SelectionType.SwitchToHero, () => switchToHero(c)));
            }

            coroutine = this.GameController.SelectAndPerformFunction(
                new SelectFunctionDecision(this.GameController, this.DecisionMaker, list, false, null, this.TurnTaker.Name + " is unable to switch to another Hero.", cardSource: this.GetCardSource())
                );
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.DrawACardOrPlayACard(this.DecisionMaker, false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override void AddSideTriggers()
        {
            // TODO: The Sentinels treat Off to the Side differently. Note that this also means the default handling for Guise is probably a bit busted.
            if (!this.Card.IsFlipped)
            {
                Func<GameAction, bool> criteria = (GameAction ga) => (ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !this.Card.IsFlipped && this.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsHeroCharacterCard && c.IsActive && c.IsRealCard && !c.IsOffToTheSide).Count() == 0;
                this.AddSideTrigger(this.AddTrigger<GameAction>(criteria, (GameAction ga) => this.GameController.FlipCard(this.FindCardController(this.Card), false, false, null, null, this.GetCardSource(null), true), TriggerType.FlipCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false));
                return;
            }
            this.AddSideTriggers(this.AddTargetEntersPlayTrigger((Card c) => this.Card.IsFlipped && this.CharacterCards.Contains(c), (Card c) => this.GameController.FlipCard(this.FindCardController(this.Card), false, false, null, null, this.GetCardSource(null), true), TriggerType.Hidden, TriggerTiming.After, false, true));
        }
    }
}