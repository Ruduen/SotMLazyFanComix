using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.TheEtherealExecutionerTeam
{
    public class PreliminaryMeasuresCardController : TheEtherealExecutionerSharedCardController
    {
        public PreliminaryMeasuresCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Trigger healing by 1 per card.
            ITrigger tempIncrease = this.AddTrigger<GainHPAction>((GainHPAction ghpa) => ghpa.CardSource.CardController == this, (GainHPAction ghpa) => this.GameController.IncreaseHPGain(ghpa, this.CountObservationCards(), this.GetCardSource()), new TriggerType[] { TriggerType.IncreaseHPGain, TriggerType.ModifyHPGain }, TriggerTiming.Before);

            // Heal 3.
            coroutine = this.GameController.GainHP(this.CharacterCard, 2, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.RemoveTrigger(tempIncrease);
        }
    }
}