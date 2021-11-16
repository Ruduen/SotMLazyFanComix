using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.TheTurfWar
{
    public class CalinCardController : TheTurfWarSharedCharacterCardController
    {
        public CalinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected override ITrigger AddUniqueActiveTrigger()
        {
            return this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
        }
        protected override ITrigger AddUniqueIncapacitatedTrigger()
        {
            return this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DestroyCardsResponse), TriggerType.DestroyCard, null, false);
        }
        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            yield break;
        }

        private IEnumerator DestroyCardsResponse(PhaseChangeAction pca)
        {
            yield break;
        }
    }
}
