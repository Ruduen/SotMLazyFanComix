using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheNaturalist
{
    public class TheNaturalistVolatileFormCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheNaturalistVolatileFormCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Based on the core display logic.
            this.SpecialStringMaker.ShowSpecialString(SpecialString, null, null);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<PlayCardAction> storedResultsPlay = new List<PlayCardAction>();

            // Draw a card.
            coroutine = this.DrawCards(this.HeroTurnTakerController, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Play a card.
            coroutine = this.GameController.SelectAndPlayCardFromHand(this.HeroTurnTakerController, false, storedResults: storedResultsPlay, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // A successful play means performing check logic for each of the three symbols on text.
            if (storedResultsPlay.Count > 0 && storedResultsPlay.FirstOrDefault().IsSuccessful)
            {
                Card card = storedResultsPlay.FirstOrDefault().CardToPlay;
                if (card != null)
                {
                    List<string> options = new string[] { "{gazelle}", "{rhinoceros}", "{crocodile}" }.Where((string i) => this.CardHasIconText(card, i)).ToList();
                    string selectedWord = null;
                    if (options.Count > 1)
                    {
                        List<SelectWordDecision> storedResultsWord = new List<SelectWordDecision>();
                        coroutine = this.GameController.SelectWord(this.HeroTurnTakerController, options, SelectionType.NaturalistForm, storedResultsWord, false, null, this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                        selectedWord = this.GetSelectedWord(storedResultsWord);
                    }
                    else if (options.Count == 1)
                    {
                        selectedWord = options.FirstOrDefault();
                    }

                    if (selectedWord != null)
                    {
                        // If we reach this, we've searched and confirm we have the icon, so run the add icon status!
                        ActivateEffectStatusEffect activateEffectStatusEffect = new ActivateEffectStatusEffect(this.HeroTurnTaker, card, selectedWord);
                        // Lasts until next turn!
                        activateEffectStatusEffect.UntilEndOfNextTurn(this.HeroTurnTaker);

                        coroutine = this.AddStatusEffect(activateEffectStatusEffect, true);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                    else
                    {
                        this.GameController.SendMessageAction("The played card does not contain {gazelle}, {rhinoceros}, or {crocodile}.", Priority.Medium, this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                    }
                }
            }
        }

        private bool CardHasIconText(Card card, string icon)
        {
            if ((card.Definition.Body.Where((string bodyText) => bodyText.Contains(icon)).Count() > 0) ||
                (card.Definition.Powers.Where((string powerText) => powerText.Contains(icon)).Count() > 0))
            {
                return true;
            }
            return false;
        }

        private string SpecialString()
        {
            List<string> list = new List<string>();
            if (this.TurnTaker.IsHero)
            {
                if (this.CanActivateEffect(base.TurnTakerController, "{gazelle}"))
                {
                    list.Add("{gazelle}");
                }
                if (this.CanActivateEffect(base.TurnTakerController, "{rhinoceros}"))
                {
                    list.Add("{rhinoceros}");
                }
                if (this.CanActivateEffect(base.TurnTakerController, "{crocodile}"))
                {
                    list.Add("{crocodile}");
                }
            }
            else
            {
                if (this.CanActivateEffect(base.Card, "{gazelle}"))
                {
                    list.Add("{gazelle}");
                }
                if (this.CanActivateEffect(base.Card, "{rhinoceros}"))
                {
                    list.Add("{rhinoceros}");
                }
                if (this.CanActivateEffect(base.Card, "{crocodile}"))
                {
                    list.Add("{crocodile}");
                }
            }
            if (list.Count<string>() > 0)
            {
                return string.Concat(new string[]
                {
                        this.TurnTaker.Name,
                        "'s current ",
                        list.Count<string>().ToString_SingularOrPlural("form", "forms"),
                        ": ",
                        list.ToCommaList(false, false, null, null)
                });
            }
            return this.TurnTaker.Name + " does not have any form cards in play.";
        }
    }
}