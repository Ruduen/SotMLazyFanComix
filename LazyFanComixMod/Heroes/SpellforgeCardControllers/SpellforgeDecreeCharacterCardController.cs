using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Spellforge
{
    public class SpellforgeDecreeCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public SpellforgeDecreeCharacterCardController(Card card, TurnTakerController turnTakerController)
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
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            List<SpellforgeModifierSharedCardController> modifierCardControllers = new List<SpellforgeModifierSharedCardController>();
            string spacedPrefixTitle = "";
            string spacedSuffixTitle = "";

            if (this.HeroTurnTakerController != null)
            {
                // Discard prefix.
                coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 0, storedResults, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("prefix"), "prefix"), cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
                {
                    Card c = storedResults.FirstOrDefault().CardToDiscard;
                    CardController cc = this.FindCardController(c);
                    if (cc is SpellforgeModifierSharedCardController)
                    {
                        // Type matches, everything should be implemented now!
                        SpellforgeModifierSharedCardController wcc = (SpellforgeModifierSharedCardController)this.FindCardController(c);
                        this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(this, this.Card));
                        modifierCardControllers.Add(wcc);
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
                    if (cc is SpellforgeModifierSharedCardController)
                    {
                        // Type matches, everything should be implemented now!
                        SpellforgeModifierSharedCardController wcc = (SpellforgeModifierSharedCardController)this.FindCardController(c);
                        this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(this, this.Card));
                        modifierCardControllers.Add(wcc);
                        spacedSuffixTitle = " " + c.Definition.AlternateTitle;
                    }
                }

                if (spacedPrefixTitle.Length > 0 || spacedSuffixTitle.Length > 0)
                {
                    coroutine = this.GameController.SendMessageAction("{Spellforge} uses " + spacedPrefixTitle + this.Card.Definition.Body.FirstOrDefault() + spacedSuffixTitle + "!", Priority.Low, this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }

            // Deal up to 3 targets 1 infernal (as a base).
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.Card), powerNumerals[1], DamageType.Infernal, powerNumerals[0], false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Clear all temporary triggers created by this card.
            this.RemoveTemporaryTriggers();
            foreach (SpellforgeModifierSharedCardController wcc in modifierCardControllers)
            {
                wcc.RemoveModifierTrigger();
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