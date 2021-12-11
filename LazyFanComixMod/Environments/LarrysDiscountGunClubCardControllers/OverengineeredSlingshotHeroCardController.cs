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
                this.GetPowerNumeral(1, 1),
                this.GetPowerNumeral(2, 1),
                this.GetPowerNumeral(3, 1),
            };

            List<PlayCardAction> pcas = new List<PlayCardAction>();

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.DecisionMaker,
                new Function[] {
                    new Function(this.DecisionMaker,"Put a card from your hand under " + this.CharacterCard.Title,SelectionType.MoveCardToUnderCard,
                        () => this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, this.HeroTurnTaker.Hand, numerals[0], numerals[0], new LinqCardCriteria(), new MoveCardDestination[] { new MoveCardDestination(this.CharacterCard.UnderLocation) }, playIfMovingToPlayArea: false, cardSource: this.GetCardSource()),
                        this.HeroTurnTaker != null && this.HeroTurnTaker.HasCardsInHand
                    ),
                    new Function(this.DecisionMaker,"Play a card from under " + this.CharacterCard.Title, SelectionType.PlayCard,
                        () => this.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location == this.CharacterCard.UnderLocation, storedResults: pcas, cardSource: this.GetCardSource()),
                        this.CharacterCard.UnderLocation.HasCards && this.GameController.CanPlayCards(this.DecisionMaker, this.GetCardSource())
                    )
                },
                true, noSelectableFunctionMessage: "You cannot put a card from your hand under " + this.CharacterCard.Title + " or play a card from under " + this.CharacterCard.Title + ".", cardSource: this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            PlayCardAction pca = pcas.FirstOrDefault((PlayCardAction pca) => pca.WasCardPlayed && pca.CardToPlay.IsAmmo && pca.CardToPlay.Location.OwnerCard == this.Card);
            if (pca != null)
            {
                coroutine = ((AmmoCardController)this.FindCardController(pca.CardToPlay)).BeforeDamageResponse();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), numerals[3], DamageType.Fire, numerals[2], false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

    }
}