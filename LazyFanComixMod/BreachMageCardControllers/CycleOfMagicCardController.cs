using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.BreachMage
{
    public class CycleOfMagicCardController : CardController
    {
        public CycleOfMagicCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

            // Draw 2 cards.
            coroutine = this.DrawCards(this.HeroTurnTakerController, 2);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            //// Use a Cast.
            //CardSource cardSource = this.GetCardSource();
            //if (cardSource == null || cardSource.Card.IsInPlayAndHasGameText || (cardSource.PowerSource.CardSource.Card.IsUnderCard && this.GameController.IsInCardControllerList(cardSource.PowerSource.CardSource.Card.Location.OwnerCard, CardControllerListType.UnderCardsCanHaveText)))
            //{
            //    yield break;
            //}

            // NOTE: REMOVE IF ENGINE ISSUE IS EVER FIXED!
            if (this.GetCardSource().Card.IsInPlayAndHasGameText)
            {
                coroutine = this.GameController.SelectAndActivateAbility(this.HeroTurnTakerController, "cast", null, storedResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0 && storedResults.FirstOrDefault().Completed)
                {
                    Card card = storedResults.FirstOrDefault().SelectedCard;
                    // Move the top card back to top of deck.
                    coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, card, card.Owner.Deck, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}