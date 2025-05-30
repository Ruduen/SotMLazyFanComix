﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.BreachMage
{
    public class StaffCharmCardController : CardController
    {
        public StaffCharmCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Add damage boost if the direct source of the damage trigger was this card.
            bool criteria(DealDamageAction dd)
            {
                // Increase damage if any trigger of the damage was this card.
                return (from acs in dd.CardSource.AssociatedCardSources
                        where acs.Card != null
                        select acs.Card).Any((Card c) => c == this.Card);
            }
            this.AddIncreaseDamageTrigger(criteria, 2, null, null, false);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<ActivateAbilityDecision> storedResults = new List<ActivateAbilityDecision>();

            // Use a Cast.
            coroutine = this.GameController.SelectAndActivateAbility(this.HeroTurnTakerController, "cast", null, storedResults, false, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            if (storedResults.Count > 0)
            {
                // Destroy the cast card.
                coroutine = this.GameController.DestroyCard(this.HeroTurnTakerController, storedResults.FirstOrDefault().SelectedCard);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}