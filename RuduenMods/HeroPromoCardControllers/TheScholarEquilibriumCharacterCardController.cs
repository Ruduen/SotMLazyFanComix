using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.TheScholar
{
    public class TheScholarEquilibriumCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheScholarEquilibriumCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            
            // Draw a card.
            coroutine = this.DrawCards(this.HeroTurnTakerController, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Draw an additional card for each elemental.
            coroutine = this.DrawCards(this.HeroTurnTakerController, this.GameController.FindCardsWhere((Card c)=>c.IsInPlayAndHasGameText && c.IsElemental).Count());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Deal self damage. Re-check elemental count since it could've changed as a result of draw->damage->shenanigans! 
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, this.GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsElemental).Count(), DamageType.Psychic, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}