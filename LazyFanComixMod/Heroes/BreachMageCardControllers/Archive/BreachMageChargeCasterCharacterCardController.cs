using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.BreachMage
{
    public class BreachMageChargeCasterCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public BreachMageChargeCasterCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Break down into two powers.
            IEnumerator coroutine;
            if (index == 1)
            {
                coroutine = this.GameController.SendMessageAction("This is the first thing that does nothing.", Priority.Medium, GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                // Stanard power.
                List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

                // Bounce an equipment.
                coroutine = this.GameController.SelectAndMoveCard(this.HeroTurnTakerController, (Card c) => c.IsInPlay && this.IsEquipment(c) && c.Owner == this.TurnTaker, this.HeroTurnTaker.Hand);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                // Play an equipment.
                coroutine = this.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => this.IsEquipment(c), "equipment", true));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        // TODO: Replace with something more unique!
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}