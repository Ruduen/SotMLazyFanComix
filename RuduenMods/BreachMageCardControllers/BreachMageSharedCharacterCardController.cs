using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.BreachMage
{
    public class BreachMageSharedCharacterCardController : HeroCharacterCardController
    {
        public int[] BreachInitialFocus { get; protected set; }

        public BreachMageSharedCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            BreachInitialFocus = new int[] { 4, 4, 4, 4 };
        }

        // Start of turn trigger to optionally cast spells. 
        public override void AddTriggers()
        {
            // add start-of-turn trigger to cast a spell
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.CastResponse), TriggerType.DestroyCard, null, false);
        }

        protected virtual IEnumerator CastResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();
            List<Card> usedAbilityCards = new List<Card>();
            bool finishedCasting = false;

            // Only allow each card to be used once. This is to prevent indestructible shenanigans. 
            while (this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.HasActivatableAbility("cast") && !usedAbilityCards.Contains(c)).Count() > 0 && !finishedCasting)
            {
                // Use a Cast.
                coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && c.IsSpell && !usedAbilityCards.Contains(c)), storedResults, true, this.GetCardSource(null));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0 && storedResults.FirstOrDefault().Completed)
                {
                    Card castCard = storedResults.FirstOrDefault().SelectedCard;

                    // Destroy the cast card.
                    coroutine = this.GameController.DestroyCard(this.DecisionMaker, castCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    // Track for future iterations if appropriate, to avoid indestructible edge cases. 
                    if (castCard.IsInPlay)
                    {
                        usedAbilityCards.Add(castCard);
                    }
                }
                else if (storedResults.Count == 0)
                {
                    // No spell cast was done, so abort. 
                    finishedCasting = true;
                }
                storedResults.Clear();
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
                        coroutine = this.SelectHeroToPlayCard(this.DecisionMaker);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource(null));
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}