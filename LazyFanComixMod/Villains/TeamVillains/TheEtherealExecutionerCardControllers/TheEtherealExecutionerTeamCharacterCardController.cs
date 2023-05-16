using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class TheEtherealExecutionerTeamCharacterCardController : VillainTeamCharacterCardController
    {
        public TheEtherealExecutionerTeamCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.Card.MayRegainHPAboveMaxHP = true;
            this.SpecialStringMaker.ShowTokenPool(this.Card.FindTokenPool("RespawnPool")).Condition = (() => this.Card.IsInPlayAndNotUnderCard);
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                this.Card.UnderLocation.OverrideIsInPlay = null;
                // Play the top card of the deck.
                Func<PhaseChangeAction, IEnumerator> playTopCardResponse = new Func<PhaseChangeAction, IEnumerator>((PhaseChangeAction pca) => this.GameController.PlayTopCard(this.DecisionMaker, this.TurnTakerController, cardSource: this.GetCardSource()));

                this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, RealDamageResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.IncreaseDamage }));
                if (this.TurnTaker.IsAdvanced)
                {
                    this.AddSideTrigger(this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, playTopCardResponse, TriggerType.PlayCard));
                }
            }
            else
            {
                this.Card.UnderLocation.OverrideIsInPlay = false;
                this.AddSideTrigger(this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, GetCardOrReviveResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DestroyCard, TriggerType.FlipCard, TriggerType.GainHP }));
            }
        }

        private IEnumerator RealDamageResponse(PhaseChangeAction arg)
        {
            IEnumerator coroutine;

            // Trigger to increase damage dealt by 1 per card.
            ITrigger tempIncrease = this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.CardSource.CardController == this, (DealDamageAction dda) => this.CountObservationCards());

            coroutine = this.DealDamageToLowestHP(this.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => 1, DamageType.Sonic);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }

        public int CountObservationCards()
        {
            return this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("observation") && c.Owner == this.TurnTaker, true).Count();
        }

        private IEnumerator GetCardOrReviveResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            if (this.Card.UnderLocation.Cards.Count() >= 2)
            {
                coroutine = this.GameController.SendMessageAction("Destroying all cards from under and reviving {TheEtherealExecutionerTeam}!", Priority.Low, this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.FlipCard(this.CharacterCardController, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.MakeTargettable(this.CharacterCard, 14, 14, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.BulkMoveCards(this.TurnTakerController, this.TurnTaker.OutOfGame.Cards.Where((Card c) => c.Owner == this.TurnTaker), this.TurnTaker.Deck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.UpdateTurnPhasesForTurnTaker(this.TurnTakerController, false);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                Location envTrash = this.FindEnvironment().TurnTaker.Trash;
                if (envTrash.HasCards)
                {
                    coroutine = this.GameController.SendMessageAction("{TheEtherealExecutionerTeam} draws power from the environment, moving the top card of the environment trash under their card.", Priority.Low, this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    coroutine = this.GameController.MoveCard(this.TurnTakerController, envTrash.TopCard, this.Card.UnderLocation, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    coroutine = this.GameController.SendMessageAction("There are no cards in the Environment trash for {TheEtherealExecutionerTeam} to move.", Priority.Low, this.GetCardSource(), showCardSource: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
        }
    }
}