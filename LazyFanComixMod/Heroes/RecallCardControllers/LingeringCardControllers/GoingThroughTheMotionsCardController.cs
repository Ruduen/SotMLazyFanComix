using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Recall
{
    public class GoingThroughTheMotionsCardController : CardController
    {
        public GoingThroughTheMotionsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<PhaseChangeAction>((PhaseChangeAction p) => p.ToPhase.TurnTaker == this.TurnTaker && p.ToPhase.IsPlayCard, SkipPhaseResponse, TriggerType.SkipPhase, TriggerTiming.After, ActionDescription.Unspecified);
        }

        private IEnumerator SkipPhaseResponse(PhaseChangeAction p)
        {
            IEnumerator coroutine;

            List<Function> list = new List<Function>()
            {
                new Function(this.HeroTurnTakerController, "Skip your Play Phase to Draw a Card", SelectionType.DrawCard, () => SkipPlayAndDrawCard(p), this.CanDrawCards(this.HeroTurnTakerController)),
                new Function(this.HeroTurnTakerController, "Skip your Play Phase to deal 1 Target 2 Energy Damage", SelectionType.DealDamage, ()=>SkipPlayAndDealDamage(p), this.AskIfCardCanDealDamage(this.TurnTaker.CharacterCard))
            };

            SelectFunctionDecision sfd = new SelectFunctionDecision(this.GameController, this.HeroTurnTakerController, list, true, null, this.TurnTaker.Name + " cannot draw any cards or deal any damage, so " + this.Card.Title + " has no effect.", null, this.GetCardSource());

            coroutine = this.GameController.SelectAndPerformFunction(sfd, null, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SkipPlayAndDrawCard(PhaseChangeAction p)
        {
            IEnumerator coroutine;
            coroutine = this.GameController.PreventPhaseAction(p.ToPhase, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.DrawCards(this.HeroTurnTakerController, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator SkipPlayAndDealDamage(PhaseChangeAction p)
        {
            IEnumerator coroutine;
            coroutine = this.GameController.PreventPhaseAction(p.ToPhase, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}