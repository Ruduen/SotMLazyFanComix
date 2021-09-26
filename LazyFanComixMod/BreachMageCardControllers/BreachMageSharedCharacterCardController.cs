using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.BreachMage
{
    public class BreachMageSharedCharacterCardController : HeroCharacterCardController
    {
        protected virtual int[] BreachInitialFocus { get { return new int[] { 4, 4, 4, 4 }; } }

        public BreachMageSharedCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            SetupBreaches();
        }

        // Start of turn trigger to optionally cast spells.
        public override void AddTriggers()
        {
            // add start-of-turn trigger to cast a spell
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.CastResponse), TriggerType.DestroyCard);
        }

        private void SetupBreaches()
        {
            Card[] breaches = this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.DoKeywordsContain("breach")).ToArray();

            for (int i = 0; i < BreachInitialFocus.Count() && i < breaches.Count(); i++)
            {
                TokenPool focusPool = breaches[i].FindTokenPool("FocusPool");
                if (focusPool != null)
                {
                    if (BreachInitialFocus[i] < 0 || BreachInitialFocus[i] > 4)
                    {
                        // Out of bounds breach - remove the breach from the game.
                        this.TurnTaker.MoveCard(breaches[i], this.TurnTaker.OutOfGame);
                    }
                    else if (focusPool.CurrentValue == 4) // If breach has not yet been initialized. Used for sanity checking on loads.
                    {
                        focusPool.RemoveTokens(4 - BreachInitialFocus[i]);
                    }
                }
            }
        }

        protected virtual IEnumerator CastResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();
            List<Card> usedAbilityCards = new List<Card>();
            bool finishedCasting = false;

            // Only allow each card to be used once. This is to prevent indestructible shenanigans. 
            while (this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.Owner == this.TurnTaker && c.HasActivatableAbility("cast") && c.IsSpell && !usedAbilityCards.Contains(c)).Count() > 0 && !finishedCasting)
            {
                storedResults.Clear();
                // Could use sanity check for non-power activation, but while loop and checks on destroying cast cards handle gracefully.

                // Use a Cast.
                coroutine = this.GameController.SelectAndActivateAbility(this.HeroTurnTakerController, "cast", new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.TurnTaker && c.IsSpell && !usedAbilityCards.Contains(c)), storedResults, true, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0 && storedResults.FirstOrDefault().SelectedAbility != null)
                {
                    Card castCard = storedResults.FirstOrDefault().SelectedCard;

                    // Track for future iterations if appropriate, to avoid indestructible edge cases. 
                    if (castCard.IsInPlay)
                    {
                        usedAbilityCards.Add(castCard);
                    }

                    // Destroy the cast card.
                    coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, castCard, cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
                else
                {
                    // No spell cast was done, so abort. 
                    finishedCasting = true;
                }
            }
        }

        protected IEnumerator ActivateCast(Card card)
        {
            IEnumerator coroutine;
            if (card.GetNumberOfActivatableAbilities("cast") == 1)
            {
                coroutine = this.GameController.ActivateAbility(
                    this.GameController.FindCardController(card).GetActivatableAbilities("cast").FirstOrDefault(),
                    cardSource: this.GetCardSource()
                );
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", new LinqCardCriteria(card), null, false, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        //// Cast response that's viable when activating abilities. However, since it's not Handelabra supported right now, go ahead and fix that.
        //protected virtual IEnumerator CastResponse(PhaseChangeAction phaseChange)
        //{
        //    IEnumerator coroutine;
        //    List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();
        //    List<Card> usedAbilityCards = new List<Card>();
        //    bool finishedCasting = false;

        //    // Only allow each card to be used once. This is to prevent indestructible shenanigans.
        //    while (this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.HasActivatableAbility("cast") && !usedAbilityCards.Contains(c)).Count() > 0 && !finishedCasting)
        //    {
        //        storedResults.Clear();

        //        // Use a Cast.
        //        coroutine = this.GameController.SelectAndActivateAbility(this.DecisionMaker, "cast", new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && c.IsSpell && !usedAbilityCards.Contains(c)), storedResults, true, this.GetCardSource());
        //        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        //        if (storedResults.Count > 0 && storedResults.FirstOrDefault().Completed)
        //        {
        //            Card castCard = storedResults.FirstOrDefault().SelectedCard;

        //            // Destroy the cast card.
        //            coroutine = this.GameController.DestroyCard(this.DecisionMaker, castCard);
        //            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

        //            // Track for future iterations if appropriate, to avoid indestructible edge cases.
        //            if (castCard.IsInPlay)
        //            {
        //                usedAbilityCards.Add(castCard);
        //            }
        //        }
        //        else if (storedResults.Count == 0 || !storedResults.FirstOrDefault().Completed)
        //        {
        //            // No spell cast was done, so abort.
        //            finishedCasting = true;
        //        }
        //        storedResults.Clear();
        //    }
        //}

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
                        coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}