using Handelabra.Sentinels.Engine.Controller;
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

        public abstract IEnumerator UniquePower();

        public IEnumerator FocusPower(int powerNumeral)
        {
            IEnumerator coroutine = this.GameController.RemoveTokensFromPool(this.Card.FindTokenPool("FocusPool"), powerNumeral);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int powerNumeral = this.GetPowerNumeral(0, 1);

            if (FocusPool.CurrentValue > 0)
            {
                // Always remove token. 
                coroutine = FocusPower(powerNumeral);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Power when open.
            coroutine = UniquePower();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
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