using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Spellforge
{
    public class SpellforgeRedefineOldCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public SpellforgeRedefineOldCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>(){
                GetPowerNumeral(0, 3),
                GetPowerNumeral(1, 1)
            };
            Card prefixCard = null;
            Card suffixCard = null;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            List<SelectTurnTakerDecision> sttdResults = new List<SelectTurnTakerDecision>();
            TurnTaker turnTaker;

            // Discard prefix.
            coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("prefix"), "prefix"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
            {
                prefixCard = storedResults.FirstOrDefault().CardToDiscard;
            }

            // Discard suffix.
            storedResults.Clear();
            coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("suffix"), "suffix"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
            {
                suffixCard = storedResults.FirstOrDefault().CardToDiscard;
            }

            //if (spacedPrefixTitle.Length > 0 || spacedSuffixTitle.Length > 0)
            //{
            //    coroutine = this.GameController.SendMessageAction("{Spellforge} uses " + spacedPrefixTitle + this.CharacterCard.Definition.Body.FirstOrDefault() + spacedSuffixTitle + "!", Priority.Low, cardSource);
            //    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            //}

            //// Deal up to 3 targets 1 infernal (as a base).
            //coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Infernal, powerNumerals[0], false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, cardSource);
            //if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Add temporary trigger before the card is played to apply modifiers.

            this.AddToTemporaryTriggerList(this.AddTrigger<PlayCardAction>(
                (PlayCardAction pca) => true,
                (PlayCardAction pca) => this.AddPrefixSuffixResponse(pca, prefixCard, suffixCard),
                new TriggerType[] { TriggerType.Hidden }, TriggerTiming.Before, null, false, true, null, false, null, null, false, false)
            );

            // Select a hero.
            coroutine = this.GameController.SelectHeroTurnTaker(this.HeroTurnTakerController, SelectionType.PlayCard, false, false, sttdResults, new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.HeroTurnTaker), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // That hero plays a oneshot.
            turnTaker = (from d in sttdResults
                         where d.Completed
                         select d.SelectedTurnTaker).FirstOrDefault<TurnTaker>();
            if (turnTaker != null)
            {
                HeroTurnTakerController heroTurnTaker = this.FindTurnTakerController(turnTaker).ToHero();
                coroutine = this.GameController.SelectAndPlayCardFromHand(heroTurnTaker, false, null, new LinqCardCriteria((Card c) => c.IsOneShot, "one-shot"), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Clear all temporary triggers created by this card and the corresponding trigger.
            this.RemoveTemporaryTriggers();
        }

        private IEnumerator AddPrefixSuffixResponse(PlayCardAction action, Card prefixCard, Card suffixCard)
        {
            if (!(action.CardSource.CardController == this)) yield break;
            Card modifiedCard = action.CardToPlay;
            CardSource modifiedCardSource = this.FindCardController(modifiedCard).GetCardSource();

            // Clear the trigger checking for the played card.
            this.RemoveTemporaryTriggers();
            if (prefixCard != null)
            {
                CardController cc = this.FindCardController(prefixCard);
                if (cc is SpellforgeSharedModifierCardController)
                {
                    // Type matches, everything should be implemented now!
                    SpellforgeSharedModifierCardController wcc = (SpellforgeSharedModifierCardController)this.FindCardController(prefixCard);
                    this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(modifiedCardSource));
                }
            }
            if (suffixCard != null)
            {
                CardController cc = this.FindCardController(suffixCard);
                if (cc is SpellforgeSharedModifierCardController)
                {
                    // Type matches, everything should be implemented now!
                    SpellforgeSharedModifierCardController wcc = (SpellforgeSharedModifierCardController)this.FindCardController(prefixCard);
                    this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(modifiedCardSource));
                }
            }
            yield break;
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