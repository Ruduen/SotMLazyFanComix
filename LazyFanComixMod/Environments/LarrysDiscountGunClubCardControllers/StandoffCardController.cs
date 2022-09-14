using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public class StandoffCardController : CardController
    {
        public StandoffCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayLarryAndSelfDestruct, new TriggerType[] { TriggerType.GainHP });
            this.AddTrigger<DealDamageAction>(SourceTargetMayBeHighestEnemies, CancelDamageIfSourceTargetAreHighestEnemies, TriggerType.CancelAction, TriggerTiming.Before, orderMatters: true);
        }

        private IEnumerator PlayLarryAndSelfDestruct(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            coroutine = this.PlayCardFromLocations(new Location[] { this.TurnTaker.Deck, this.TurnTaker.Trash }, "Larry", false);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
        private bool SourceTargetMayBeHighestEnemies(DealDamageAction dda)
        {
            IEnumerable<Card> highestVillains = this.GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => c.IsVillainCharacterCard, this.GetCardSource());
            IEnumerable<Card> highestHeroes = this.GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => c.IsHeroCharacterCard, this.GetCardSource());

            // Quit if any failures on targetting retrieval or self-damage.
            if (dda?.DamageSource?.Card?.IsHeroCharacterCard == true && dda?.Target?.IsVillainCharacterCard == true)
            {
                if (highestHeroes.Contains(dda.DamageSource.Card)  && highestVillains.Contains(dda.Target))
                {
                    return true;
                }
            }
            else if (dda?.DamageSource?.Card?.IsVillainCharacterCard == true && dda?.Target?.IsHeroCharacterCard == true)
            {
                if (highestVillains.Contains(dda.DamageSource.Card) && highestHeroes.Contains(dda.Target))
                {
                    return true;
                }
            }
            return false;

        }

        private IEnumerator CancelDamageIfSourceTargetAreHighestEnemies(DealDamageAction dda)
        {
            IEnumerable<Card> highestVillains = this.GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => c.IsVillainCharacterCard, this.GetCardSource());
            IEnumerable<Card> highestHeroes = this.GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) => c.IsHeroCharacterCard, this.GetCardSource());
            List<YesNoCardDecision> yncd = new List<YesNoCardDecision>();
            IEnumerator coroutine;

            if (highestHeroes.Count() > 1 || highestVillains.Count() > 1)
            {
                coroutine = this.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.Custom, this.Card, dda, yncd, new Card[] { dda.DamageSource.Card, dda.Target }, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (yncd?.FirstOrDefault()?.Answer == true)
                {
                    coroutine = this.CancelAction(dda, true, isPreventEffect: true);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                coroutine = this.CancelAction(dda, true, isPreventEffect: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Deck, true, false, false, new LinqCardCriteria((Card c) => c.IsGun), 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (decision.AssociatedCards.Count() == 2)
            {
                Card hero = decision.AssociatedCards.FirstOrDefault((Card c) => c.IsHeroCharacterCard);
                Card villain = decision.AssociatedCards.FirstOrDefault((Card c) => c.IsVillainCharacterCard);
                if (hero != null && villain != null)
                {
                    return new CustomDecisionText("Should this damage be prevented since " + hero.Title + " would be the highest HP Hero and " + villain.Title + " would be the highest HP Villain?", "Someone is determining if damage should be prevented.", "Should this damage be prevented since " + hero.Title + " would be the highest HP Hero and " + villain.Title + " would be the highest HP Villain?", "Prevent damage since targets would be highest HP Hero and Villain");
                }
            }
            string wut = "Something went wrong, so make a bug report for Lazy Fan Comix. In the meantime... Prevent a damage?";
            return new CustomDecisionText(wut, wut, wut, wut);
        }
    }
}