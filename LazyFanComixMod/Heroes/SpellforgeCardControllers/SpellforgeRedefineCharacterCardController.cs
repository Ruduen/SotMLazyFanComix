using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Spellforge
{
    public class SpellforgeRedefineCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public SpellforgeRedefineCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            List<SpellforgeModifierSharedCardController> modifierCardControllers = new List<SpellforgeModifierSharedCardController>();
            string spacedPrefixTitle = "";
            string spacedSuffixTitle = "";

            if (this.HeroTurnTakerController != null)
            {
                // Discard prefix or suffix.
                coroutine = this.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 1, storedResults, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                if (storedResults.Count > 0 && storedResults.FirstOrDefault().IsSuccessful)
                {
                    Card c = storedResults.FirstOrDefault().CardToDiscard;
                    CardController cc = this.FindCardController(c);
                    if (cc is SpellforgeModifierSharedCardController)
                    {
                        SpellforgeModifierSharedCardController wcc = (SpellforgeModifierSharedCardController)this.FindCardController(c);
                        this.AddToTemporaryTriggerList(wcc.AddModifierTrigger(this, null));
                        this.AddToTemporaryTriggerList(wcc.AddDesignatePlayTrigger(this));
                        modifierCardControllers.Add(wcc);
                        if (c.DoKeywordsContain("prefix"))
                        {
                            spacedPrefixTitle = c.Definition.AlternateTitle + " ";
                        }
                        else if (c.DoKeywordsContain("suffix"))
                        {
                            spacedSuffixTitle = " " + c.Definition.AlternateTitle;
                        }
                    }
                }
            }

            // Yes, this extra logic is necessary for actually selecting a hero, since no method exists to restrict card plays after. 
            IEnumerable<TurnTaker> source = this.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero);
            if (source.All((TurnTaker tt) => tt != this.TurnTaker && this.GameController.DoAnyCardsPreventAction<PlayCardAction>(this.FindTurnTakerController(tt)).Any()))
            {
                IEnumerable<Card> cardsPreventingPlay = source.SelectMany((TurnTaker tt) => this.GameController.DoAnyCardsPreventAction<PlayCardAction>(this.FindTurnTakerController(tt))).Distinct<Card>();
                string cardTitles = (from p in cardsPreventingPlay select p.Title).ToCommaList(true);
                string connector = (cardsPreventingPlay.Count() == 1) ? "is" : "are";
                coroutine = this.GameController.SendMessageAction(cardTitles + " " + connector + " preventing heroes from playing cards.", Priority.Medium, this.GetCardSource(), cardsPreventingPlay, false);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                List<SelectTurnTakerDecision> sttd = new List<SelectTurnTakerDecision>();
                LinqTurnTakerCriteria heroCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker && tt.IsHero && (this.GameController.CanPerformAction<PlayCardAction>(this.FindTurnTakerController(tt), this.GetCardSource())) && tt.ToHero().HasCardsInHand);

                coroutine = this.GameController.SelectHeroTurnTaker(this.DecisionMaker, SelectionType.PlayCard, false, false, sttd, heroCriteria, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                TurnTaker tt = (from d in sttd where d.Completed select d.SelectedTurnTaker).FirstOrDefault();
                if (tt != null)
                {
                    coroutine = this.GameController.SelectAndPlayCardFromHand(this.FindTurnTakerController(tt).ToHero(), true, cardCriteria: new LinqCardCriteria((Card c) => c.IsOneShot), cardSource: this.GetCardSource());
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }

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