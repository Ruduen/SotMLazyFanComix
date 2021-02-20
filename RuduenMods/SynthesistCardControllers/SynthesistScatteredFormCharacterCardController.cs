using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Synthesist
{
    public class SynthesistScatteredFormCharacterCardController : CharacterCardController
    {

        private string[] _synthesistRelics;

        public SynthesistScatteredFormCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                this.AddSideTrigger(this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PutRelicIntoPlayTrigger, new TriggerType[] { TriggerType.PutIntoPlay, TriggerType.FlipCard }));
            }
        }
        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveAllTriggers(true, true, true, false, false);
            this.AddSideTriggers();
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 1), // Damage Reduction
                this.GetPowerNumeral(1, 1) // Token Removal
            };
            IEnumerator coroutine;

            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(powerNumerals[0]);
            reduceDamageStatusEffect.TargetCriteria.OwnedBy = this.TurnTaker;
            reduceDamageStatusEffect.TargetCriteria.OutputString = this.TurnTaker.Name + "'s Targets";
            reduceDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
            coroutine = this.AddStatusEffect(reduceDamageStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            IEnumerable<Card> ritualTokenCards = this.GameController.FindCardsWhere((Card c) => c.FindTokenPool("RitualTokenPool") != null && c.FindTokenPool("RitualTokenPool").CurrentValue > 0);
            if (ritualTokenCards.Any())
            {
                coroutine = this.GameController.SelectCardAndDoAction(
                    new SelectCardDecision(this.GameController, this.DecisionMaker, SelectionType.RemoveTokens, ritualTokenCards, cardSource: this.GetCardSource()), (SelectCardDecision scd) => RemoveTokenFromRitual(scd, powerNumerals[1]));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no rituals with Ritual Tokens in play.", Priority.Medium, cardSource: this.GetCardSource(), showCardSource: true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator RemoveTokenFromRitual(SelectCardDecision scd, int number)
        {
            if (scd.SelectedCard != null)
            {
                IEnumerator coroutine = this.GameController.RemoveTokensFromPool(scd.SelectedCard.FindTokenPool("RitualTokenPool"), number, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            yield break;
        }

        protected IEnumerator PutRelicIntoPlayTrigger(PhaseChangeAction action)
        {
            IEnumerator coroutine;
            // If there are no targets in play and the relic deck is not empty...
            if (this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndNotUnderCard).Count() == 0)
            {
                coroutine = this.GameController.SelectAndUnincapacitateHeroCharacter(this.DecisionMaker, 9, null, false, null, new LinqCardCriteria((Card c) => c.Owner == this.HeroTurnTaker && (this.GameController.IsOblivAeonMode || SynthesistRelics.Contains(c.PromoIdentifierOrIdentifier)), "incapacitated Synthesist character"), this.GetCardSource(), true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Re-check. If it fails, incap. 
            if (this.GameController.FindCardsWhere((Card c) => c.Owner == this.HeroTurnTaker && c.IsTarget && c.IsInPlayAndNotUnderCard).Count() == 0)
            {
                coroutine = this.GameController.FlipCard(this, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

        }

        // TODO: Replace Incap with something more unique!
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
                        coroutine = this.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }

        private string[] SynthesistRelics
        {
            get
            {
                if (_synthesistRelics == null)
                {
                    _synthesistRelics = new string[] { "BoneOfIron", "HeartOfLightning", "VialOfMercury" };
                }
                return _synthesistRelics;
            }
        }
    }
}