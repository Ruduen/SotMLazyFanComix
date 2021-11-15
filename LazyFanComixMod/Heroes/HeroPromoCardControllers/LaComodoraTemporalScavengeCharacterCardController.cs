using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LaComodora
{
    public class LaComodoraTemporalScavengeCharacterCardController : PromoDefaultCharacterCardController
    {
        public LaComodoraTemporalScavengeCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] numerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };
            List<Function> list = new List<Function>();
            SelectFunctionDecision sfd;
            IEnumerator coroutine;

            list.Add(new Function(this.HeroTurnTakerController, "Draw " + numerals[0] + " Card", SelectionType.DrawCard,
                () => this.GameController.DrawCards(this.HeroTurnTakerController, numerals[0], cardSource: this.GetCardSource()),
                this.HeroTurnTakerController != null && this.CanDrawCards(this.HeroTurnTakerController), this.TurnTaker.Name + " cannot play any cards from under " + this.CardWithoutReplacements.Title + ", so they must draw " + numerals[0] + " card."));
            list.Add(new Function(this.HeroTurnTakerController, "Play " + numerals[1] + " card from under " + this.CardWithoutReplacements.Title, SelectionType.PlayCard,
                () => SelectAndPlayFromUnder(numerals[1]),
                this.TurnTakerController != null && this.CardWithoutReplacements.UnderLocation.HasCards && this.CanPlayCards(this.TurnTakerController),
                this.TurnTaker.Name + " cannot draw any cards, so they must play " + numerals[1] + " card from under " + this.CardWithoutReplacements.Title + "."));
            sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, false, null, this.TurnTaker.Name + " play any equipment or cards from under " + this.CardWithoutReplacements.Title + ", so" + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Set up an effect to respond when your equipment is destroyed. The ToHero here is an exception since it has to track the environment turn sometimes.
            WhenCardIsDestroyedStatusEffect whenCardIsDestroyedStatusEffect = new WhenCardIsDestroyedStatusEffect(this.CardWithoutReplacements, "WhenEquipIsDestroyed", "Whenever an Equipment is destroyed, you may put it under " + this.CardWithoutReplacements.Title + ".", new TriggerType[]
            { TriggerType.MoveCard }, this.TurnTaker.ToHero(), this.Card, null);
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "equipment" };
            whenCardIsDestroyedStatusEffect.UntilEndOfNextTurn(this.TurnTaker);
            whenCardIsDestroyedStatusEffect.Priority = new StatusEffectPriority?(StatusEffectPriority.High);
            whenCardIsDestroyedStatusEffect.CanEffectStack = false;
            coroutine = this.AddStatusEffect(whenCardIsDestroyedStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public IEnumerator WhenEquipIsDestroyed(DestroyCardAction destroy, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            if (destroy.PostDestroyDestinationCanBeChanged)
            {
                List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
                IEnumerator coroutine = this.GameController.MakeYesNoCardDecision(this.HeroTurnTakerController, SelectionType.MoveCardToUnderCard, destroy.CardToDestroy.Card, storedResults: storedResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (this.DidPlayerAnswerYes(storedResults))
                {
                    destroy.SetPostDestroyDestination(this.Card.UnderLocation);
                    destroy.PostDestroyDestinationCanBeChanged = false;
                }
            }
        }

        private IEnumerator SelectAndPlayFromUnder(int numeral)
        {
            IEnumerator coroutine;
            Func<Card, bool> isUnderCard = (Card c) => c.Location == this.CardWithoutReplacements.UnderLocation;

            // Based on SelectAndPlayCardsFromHand, just... not from hand.
            string tryToPlayCardMessage = null;
            if (!this.CardWithoutReplacements.UnderLocation.HasCards)
            {
                tryToPlayCardMessage = "There are no cards underneath " + this.CardWithoutReplacements.Title + " to play.";
            }
            else if (HeroTurnTakerController != null)
            {
                tryToPlayCardMessage = this.GameController.GetTryToPlayCardMessage(this.HeroTurnTakerController, false, isUnderCard, null, true, false);
            }
            if (tryToPlayCardMessage == null)
            {
                SelectCardsDecision scd = new SelectCardsDecision(this.GameController, this.HeroTurnTakerController, isUnderCard, SelectionType.PlayCard, numeral, false, numeral, cardSource: this.GetCardSource());
                coroutine = this.GameController.SelectCardsAndDoAction(scd, PlayCardDelegate, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction(tryToPlayCardMessage, Priority.High, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        private IEnumerator PlayCardDelegate(SelectCardDecision d)
        {
            IEnumerator coroutine = this.GameController.PlayCard(this.HeroTurnTakerController, d.SelectedCard, reassignPlayIndex: true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}