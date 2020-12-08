using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Bunker
{
    public class BunkerFullSalvoCharacterCardController : PromoDefaultCharacterCardController
    {
        public BunkerFullSalvoCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 3);

            List<PlayCardAction> storedResults = new List<PlayCardAction>();

            IEnumerator coroutine;
            List<Card> cardsWithPowers = GetCardsWithPowersInPlay();

            // Draw 3 cards.
            coroutine = this.DrawCards(this.HeroTurnTakerController, powerNumeral);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Discard 1 for each of your powers, including base.
            coroutine = SelectAndDiscardCards(this.HeroTurnTakerController, cardsWithPowers.Count());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            CardCriteria cardCriteria = new CardCriteria(null)
            {
                IsOneOfTheseCards = cardsWithPowers
            };
            coroutine = this.SetPhaseActionCountThisTurn(this.TurnTaker, Phase.UsePower, cardCriteria);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (cardsWithPowers.Count() == 1)
            {
                // If you only have the base power in play, indicate you may not be able to use more powers. 
                coroutine = base.GameController.SendMessageAction(this.TurnTaker.Name + " has no other cards with powers in play, so " + this.TurnTaker.Name + " may not be able to use any more powers this turn.", Priority.High, this.GetCardSource(), null, false);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private List<Card> GetCardsWithPowersInPlay()
        {
            return this.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.HasPowers && c.Owner == this.TurnTaker).ToList();
        }
    }
}