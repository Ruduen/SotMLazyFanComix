﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Trailblazer
{
    // TODO: TEST!
    public class VantagePointCardController : TrailblazerPositionCardController
    {
        public VantagePointCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

        public override void AddTriggers()
        {
            this.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.DrawCard, 1);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            // Shared Destroy logic.
            coroutine = base.Play();
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Increase phase count if appropriate, based on trigger.
            coroutine = this.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == this.HeroTurnTaker, Phase.DrawCard, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<int> powerNumerals = new List<int>
            {
                this.GetPowerNumeral(0, 2)
            };

            // Another player draws 2 cards.
            coroutine = this.GameController.SelectTurnTakersAndDoAction(this.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt.IsHero && tt != this.TurnTaker), SelectionType.DrawCard, (TurnTaker tt) => this.GameController.DrawCards(this.GameController.FindHeroTurnTakerController(tt.ToHero()), powerNumerals[0], cardSource: this.GetCardSource()), 1, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == this.HeroTurnTaker;
        }
    }
}