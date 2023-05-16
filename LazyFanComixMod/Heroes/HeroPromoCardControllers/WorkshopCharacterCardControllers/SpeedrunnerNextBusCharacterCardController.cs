using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;

namespace LazyFanComix.Speedrunner
{
    public class SpeedrunnerNextBusCharacterCardController : PromoDefaultCharacterCardController
    {
        public SpeedrunnerNextBusCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 1)
            };
            IEnumerator coroutine;

            coroutine = this.GameController.DrawCards(this.DecisionMaker, powerNums[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectCardsDecision scd = new SelectCardsDecision(this.GameController, this.DecisionMaker, (Card c) => c.IsInPlayAndHasGameText && !c.IsHero && !c.IsCharacter, SelectionType.None, powerNums[1], false, powerNums[1], eliminateOptions: true, cardSource: this.GetCardSource());

            coroutine = this.GameController.SelectCardsAndDoAction(scd, RemoveResponses, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator RemoveResponses(SelectCardDecision scd)
        {
            IEnumerator coroutine;

            PreventPhaseEffectStatusEffect ppeseStart = new PreventPhaseEffectStatusEffect(Phase.Start);
            ppeseStart.UntilStartOfNextTurn(this.TurnTaker);
            ppeseStart.CardCriteria.IsSpecificCard = scd.SelectedCard;

            coroutine = this.AddStatusEffect(ppeseStart, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            PreventPhaseEffectStatusEffect ppeseEnd = new PreventPhaseEffectStatusEffect(Phase.End);
            ppeseEnd.UntilStartOfNextTurn(this.TurnTaker);
            ppeseEnd.CardCriteria.IsSpecificCard = scd.SelectedCard;

            coroutine = this.AddStatusEffect(ppeseEnd, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}