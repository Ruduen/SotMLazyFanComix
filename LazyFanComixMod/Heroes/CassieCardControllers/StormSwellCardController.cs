using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Cassie
{
    // TODO: TEST!
    public class StormSwellCardController : CassieRiverSharedCardController
    {
        public StormSwellCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Up to 2 targets 4 cold.
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, this.CharacterCard), 4, DamageType.Cold, 2, false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}