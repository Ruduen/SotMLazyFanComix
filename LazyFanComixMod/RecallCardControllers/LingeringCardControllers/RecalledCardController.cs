using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Recall
{
    public class RecalledCardController : CardController
    {
        public RecalledCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), 2, DamageType.Projectile, 1, false, 1, cardSource: this.GetCardSource()), TriggerType.DealDamage);
            this.AddTrigger<DestroyCardAction>(
                (DestroyCardAction dca) => dca.CardToDestroy.Card == this.CharacterCard,
                PreventDestructionResponse,
                new TriggerType[] { TriggerType.CancelAction, TriggerType.GainHP, TriggerType.DestroyCard, TriggerType.DestroySelf },
                TriggerTiming.Before
            );
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, 3, DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> scdResults = new List<SelectCardDecision>();
            int[] numerals = new int[]
            {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 2)
            };
            IEnumerator coroutine;
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.Card), numerals[1], DamageType.Energy, numerals[0], false, numerals[0], storedResultsDecisions: scdResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (scdResults.Count > 0)
            {
                coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => scdResults.Select((SelectCardDecision scd) => scd.SelectedCard).Contains(c), numerals[2], DamageType.Energy, cardSource: this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }

        private IEnumerator PreventDestructionResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> yncdResult = new List<YesNoCardDecision>();

            coroutine = this.GameController.CancelAction(dca, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SetHP(dca.CardToDestroy.Card, 7, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}