﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace LazyFanComix.Cassie
{
    // TODO: TEST!
    public class PerpetualFlowCardController : CassieRiverSharedCardController
    {
        public PerpetualFlowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowSpecialString(GetRiverbankString, null, null);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Discard a card.
            coroutine = this.SelectAndDiscardCards(this.HeroTurnTakerController, 1, false, 1, null, false, null, null, null, SelectionType.DiscardCard, this.TurnTaker);
            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Select and play one of the Riverbank cards.
            // Yes, this is messy, but it's still the cleanest way of mimicing the official SelectCardAndDoAction without access to the evenIfIndestructable flag. Battle Zones shouldn't be an issue.

            coroutine = this.GameController.SelectCardAndDoAction(new SelectCardDecision(this.GameController, this.HeroTurnTakerController, SelectionType.PlayCard, this.GameController.FindCardsWhere((Card c) => c.Location == this.Riverbank().UnderLocation)),
                (SelectCardDecision d) => this.GameController.PlayCard(this.TurnTakerController, d.SelectedCard),
                false);

            if (UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}