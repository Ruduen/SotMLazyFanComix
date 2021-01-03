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
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.CastResponse), TriggerType.DestroyCard);
        }

        // Temporary method - select a card and activate its cast effect. Not great, but it will do while Handelabra is fixing the issue. 
        protected virtual IEnumerator CastResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            List<Card> usedAbilityCards = new List<Card>();
            bool finishedCasting = false;

            // Only allow each card to be used once. This is to prevent indestructible shenanigans. 
            while (this.GameController.FindCardsWhere((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.HasActivatableAbility("cast") && !usedAbilityCards.Contains(c)).Count() > 0 && !finishedCasting)
            {
                scdResults.Clear();

                // Select a card and use a cast on it. 

                coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.ActivateAbility, new LinqCardCriteria((Card c) => c.IsInPlay && c.Owner == this.HeroTurnTaker && c.IsSpell && c.HasActivatableAbility("cast") && !usedAbilityCards.Contains(c)), scdResults, true, cardSource: this.GetCardSource());
                if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (scdResults.Count == 0 || !scdResults.FirstOrDefault().Completed || scdResults.FirstOrDefault().SelectedCard == null)
                {
                    // No spell cast was selected for whatever reason, so abort. 
                    finishedCasting = true;
                }
                else
                {
                    Card castCard = scdResults.FirstOrDefault().SelectedCard;

                    // Use the cast on it. 
                    coroutine = this.ActivateCast(castCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    // Destroy the cast card.
                    coroutine = this.GameController.DestroyCard(this.DecisionMaker, castCard);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                    // Track for future iterations if appropriate, to avoid indestructible edge cases. 
                    if (castCard.IsInPlay)
                    {
                        usedAbilityCards.Add(castCard);
                    }
                    else if (usedAbilityCards.Contains(castCard))
                    {
                        usedAbilityCards.Remove(castCard);
                    }
                }
                scdResults.Clear();
            }
        }

        private IEnumerator ActivateCast(Card card)
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