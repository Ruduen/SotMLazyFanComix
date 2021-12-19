using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Expatriette;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class OverengineeredSlingshotHeroCardController : SharedHeroGunEarnedCardController
    {
        public OverengineeredSlingshotHeroCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            List<int> numerals = new List<int>()
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };

            List<Card> pcs = new List<Card>();

            coroutine = this.GameController.PlayTopCardOfLocation(this.DecisionMaker, this.TurnTaker.Deck, playedCards: pcs);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            Card pc = pcs.FirstOrDefault((Card pc) => pc.IsAmmo && pc.Location.OwnerCard == this.Card);
            if (pc != null && this.FindCardController(pc) is AmmoCardController)
            {
                coroutine = ((AmmoCardController)this.FindCardController(pc)).BeforeDamageResponse();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), numerals[1], DamageType.Fire, numerals[0], false, numerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = PutCardUnderTopOfDeck();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator PutCardUnderTopOfDeck()
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scdResult = new List<SelectCardDecision>();
            Location deck = this.TurnTaker.Deck;

            // Select a card from hand. 
            coroutine = this.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardUnderTopCardOfDeck, this.HeroTurnTaker.Hand.Cards, scdResult, false, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResult?.FirstOrDefault()?.SelectedCard != null)
            {
                Card card = scdResult.FirstOrDefault().SelectedCard;
                Card[] topCards = deck.GetTopCards(1).Reverse().ToArray();

                string message = string.Format("{0} moves {1} under the top card of their deck.", this.CharacterCard.Title, card.Title);
                coroutine = this.GameController.SendMessageAction(message, Priority.Medium, cardSource: this.GetCardSource(), new Card[] { card }, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Move it on top of the environment deck, then move the stored top card of the environment deck. If none exists, top card will not exist.
                coroutine = this.GameController.MoveCard(this.HeroTurnTakerController, card, deck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MoveCards(this.HeroTurnTakerController, topCards, deck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

    }
}