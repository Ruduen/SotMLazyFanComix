using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class DisruptiveFlashCardController : TheEtherealExecutionerSharedCardController
    {
        public DisruptiveFlashCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DealDamageAction> ddaResults = new List<DealDamageAction>();

            coroutine = this.DealDamageToLowestHP(this.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => 1, DamageType.Infernal, storedResults: ddaResults);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            HeroTurnTaker targetPlayer = ddaResults?.FirstOrDefault()?.Target?.Owner?.ToHero();
            if (targetPlayer != null)
            {
                int cardsToDraw = this.CountObservationCards();

                if (cardsToDraw <= 0)
                {
                    coroutine = this.GameController.SendMessageAction("There are no observation cards in play, so no cards will be drawn or discarded.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    List<DrawCardAction> dcaResults = new List<DrawCardAction>();
                    HeroTurnTakerController targetPlayerController = this.GameController.FindHeroTurnTakerController(targetPlayer);

                    coroutine = this.GameController.DrawCards(targetPlayerController, cardsToDraw, storedResults: dcaResults, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    int drawCount = dcaResults.Count((DrawCardAction dca) => dca.DidDrawCard);
                    int handCount = targetPlayer.Hand.Cards.Count();

                    if (handCount > 0 && drawCount > 0)
                    {
                        int cardsToDiscard = Math.Min(handCount, drawCount);

                        IEnumerable<Card> randomCards = targetPlayer.Hand.Cards.TakeRandom(handCount, this.GameController.Game.RNG).Take(cardsToDiscard);

                        coroutine = this.GameController.SendMessageAction(targetPlayer.Name + " must discard " + cardsToDiscard + " random card" + (cardsToDiscard == 1 ? "" : "s") + ".", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                        coroutine = this.GameController.DiscardCards(targetPlayerController, randomCards, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else if (drawCount <= 0)
                    {
                        coroutine = this.GameController.SendMessageAction("No cards were drawn, so no cards will be discarded.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else
                    {
                        coroutine = this.GameController.SendMessageAction(targetPlayer.Name + "does not have any cards in hand, so no cards will be discarded.", Priority.Low, cardSource: this.GetCardSource(), showCardSource: true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }
            }
        }
    }
}