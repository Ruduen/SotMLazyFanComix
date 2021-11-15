﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.BreachMage
{
    public abstract class BreachMageSharedBreachController : CardController
    {
        protected TokenPool FocusPool;

        public BreachMageSharedBreachController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            FocusPool = this.Card.FindTokenPool("FocusPool");
            this.SpecialStringMaker.ShowTokenPool(FocusPool, null, null).Condition = (() => this.Card.IsInPlay);
        }

        public virtual IEnumerator UseOpenPower()
        {
            // Play card.
            IEnumerator coroutine = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public virtual IEnumerator UseFocusPower(int powerNumeral)
        {
            IEnumerator coroutine = this.GameController.RemoveTokensFromPool(this.Card.FindTokenPool("FocusPool"), powerNumeral);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int powerNumeral = this.GetPowerNumeral(0, 1);

            if (FocusPool.CurrentValue == 0)
            {
                // Power when open.
                coroutine = this.UseOpenPower();
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.UseFocusPower(powerNumeral);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        // Make this card indestructible if any other card asks. This is true on both sides!
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == this.Card;
        }

        // One spell per breach if there are no focus tokens!
        public override bool CanOtherCardGoNextToThisCard(Card card)
        {
            return !card.IsSpell || this.GetNumberOfSpellCardsNextToThisCard() < 1;
        }

        private int GetNumberOfSpellCardsNextToThisCard()
        {
            return (from c in this.Card.NextToLocation.Cards
                    where c.IsSpell
                    select c).Count<Card>();
        }
    }
}