﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Spellforge
{
    public abstract class SpellforgeSharedEssenceCardController : CardController
    {
        public SpellforgeSharedEssenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected virtual IEnumerator PerformModifiedAction(CardSource cardSource)
        {
            // Deal 1 target 1 infernal (as a base).
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Projectile, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, cardSource);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            string spacedPrefixTitle = "";
            string spacedSuffixTitle = "";

            // Discard prefix.
            coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("prefix"), "prefix"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
            {
                Card c = storedResults.FirstOrDefault().CardToDiscard;
                CardController cc = this.FindCardController(c);
                if (cc is SpellforgeSharedModifierCardController)
                {
                    // Type matches, everything should be implemented now!
                    SpellforgeSharedModifierCardController wcc = (SpellforgeSharedModifierCardController)this.FindCardController(c);
                    this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(this.GetCardSource()));
                    spacedPrefixTitle = c.Definition.AlternateTitle + " ";
                }
            }

            // Discard suffix.
            storedResults.Clear();
            coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("suffix"), "suffix"), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
            {
                Card c = storedResults.FirstOrDefault().CardToDiscard;
                CardController cc = this.FindCardController(c);
                if (cc is SpellforgeSharedModifierCardController)
                {
                    // Type matches, everything should be implemented now!
                    SpellforgeSharedModifierCardController wcc = (SpellforgeSharedModifierCardController)this.FindCardController(c);
                    this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(this.GetCardSource()));
                    spacedSuffixTitle = " " + c.Definition.AlternateTitle;
                }
            }

            if (spacedPrefixTitle.Length > 0 || spacedSuffixTitle.Length > 0)
            {
                coroutine = this.GameController.SendMessageAction("{Spellforge} uses " + spacedPrefixTitle + this.Card.AlternateTitleOrTitle + spacedSuffixTitle + "!", Priority.Low, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            coroutine = this.PerformModifiedAction(this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Clear all temporary triggers created by this card.
            this.RemoveTemporaryTriggers();
        }
    }
}