using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Tempest
{
    public class TempestRisingWindsCharacterCardController : PromoDefaultCharacterCardController
    {
        private readonly List<Card> actedEnvironments;

        public TempestRisingWindsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.actedEnvironments = new List<Card>();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 1); // Damage to deal.
            IEnumerator coroutine;
            // Draw a card.
            coroutine = this.DrawCards(this.HeroTurnTakerController, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.DealDamageToHighestHP(this.CharacterCard, 1, (Card c) => this.IsVillainTarget(c) && this.GameController.IsCardVisibleToCardSource(c, this.GetCardSource()), (Card c) => powerNumeral, DamageType.Projectile);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectCardsDecision selectCardsDecision = new SelectCardsDecision(this.GameController, this.HeroTurnTakerController, (Card c) => c.IsInPlay && c.IsEnvironment && !c.IsTarget, SelectionType.CardToDealDamage, null, false, null, true, true, false, new Func<int>(this.NumEnvironmentToDamage), null, null, null, this.GetCardSource());

            coroutine = this.GameController.SelectCardsAndDoAction(selectCardsDecision, (SelectCardDecision sc) => this.EnvironmentDamageResponse(sc, powerNumeral), null, null, this.GetCardSource(), null, false, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.actedEnvironments.Clear();
        }

        private int NumEnvironmentToDamage()
        {
            if (!this.Card.IsIncapacitatedOrOutOfGame)
            {
                int num = base.FindCardsWhere((Card c) => c.IsEnvironment && !c.IsTarget && c.IsInPlay, false, null, false).Except(this.actedEnvironments).Count<Card>();
                return this.actedEnvironments.Count() + num;
            }
            return 0;
        }

        private IEnumerator EnvironmentDamageResponse(SelectCardDecision sc, int? damageAmount)
        {
            Card selectedCard = sc.SelectedCard;
            this.actedEnvironments.Add(selectedCard);

            IEnumerator coroutine = this.DealDamageToHighestHP(selectedCard, 1, (Card c) => this.IsVillainTarget(c) && this.GameController.IsCardVisibleToCardSource(c, this.GetCardSource()), (Card c) => damageAmount, DamageType.Projectile);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}