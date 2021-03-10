using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Greyhat
{
    public class InstantFirewallCardController : GreyhatSharedDeviceCardController
    {

        public InstantFirewallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DealFireDamageResponse, new TriggerType[] { TriggerType.DealDamage });
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int[] powerNumerals = new int[] {
                this.GetPowerNumeral(0,1),
                this.GetPowerNumeral(1,3)
            };

            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), powerNumerals[1], DamageType.Fire, powerNumerals[0], false, powerNumerals[0], cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        private IEnumerator DealFireDamageResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            coroutine = this.GameController.DealDamage(this.DecisionMaker, this.CharacterCard, (Card c) => !c.IsHero, 1, DamageType.Fire, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}