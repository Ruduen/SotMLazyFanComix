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
                List<Power> list = new List<Power>() { new Power(cardController.DecisionMaker, cardController, "Play 1 Card. Switch this Hero with your set aside Hero.", this.PowerResponse(cardController), 0, null, this.GetCardSource()) };
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
                this.GetPowerNumeral(0, 1), // Played cards
            };
            List<SelectCardDecision> scd = new List<SelectCardDecision>();

            IEnumerator coroutine;


            coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SwitchToHero, new LinqCardCriteria((Card c) => c.Location == this.TurnTaker.OffToTheSide && c.IsHeroCharacterCard), scd, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scd?.FirstOrDefault()?.SelectedCard != null)
            {
                coroutine = this.GameController.SwitchCards(cardController.Card, scd.FirstOrDefault().SelectedCard, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(this.TurnTaker.Name + " does not have any Heroes to switch to.", Priority.Low, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectAndPlayCardsFromHand(cardController.DecisionMaker, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        List<SelectCardDecision> scd = new List<SelectCardDecision>();
                        coroutine = this.GameController.SelectAndMoveCard(this.DecisionMaker, (Card c) => c.Owner == this.TurnTaker && c.IsHeroCharacterCard && c.IsRealCard && c.IsIncapacitated, this.TurnTaker.OutOfGame, storedResults: scd, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                        if (scd.Count > 0 && scd.FirstOrDefault().SelectedCard != null)
                        {
                            coroutine = this.GameController.SelectAndUnincapacitateHero(this.DecisionMaker, 6, 4, null, this.GetCardSource(),
                                new LinqTurnTakerCriteria((TurnTaker tt) => tt == this.TurnTaker));
                            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                            coroutine = this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.IsHeroCharacterCard && c.Location == this.TurnTaker.OffToTheSide, cardSource: this.GetCardSource());
                            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                            coroutine = this.GameController.SelectAndPlayCardFromHand(this.DecisionMaker, true, cardSource: this.GetCardSource());
                            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        }


                        break;
                    }
                case 1:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = this.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}