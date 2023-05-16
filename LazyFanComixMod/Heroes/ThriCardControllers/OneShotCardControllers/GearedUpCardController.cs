using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class GearedUpCardController : CardController
    {
        public GearedUpCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            SelectFunctionDecision sfd;
            Func<HeroTurnTakerController, IEnumerable<Function>> funcs = (HeroTurnTakerController httc) => new Function[]
            {
                new Function(httc, "Play 1 Equipment Card", SelectionType.PlayCard,
                    () => this.GameController.SelectAndPlayCardFromHand(httc, false, cardCriteria: new LinqCardCriteria((Card c) => this.IsEquipment(c), "equipment"), cardSource: this.GetCardSource()), this.CanPlayCardsFromHand(httc) && httc.HeroTurnTaker.Hand.Cards.Any((Card c) => this.IsEquipment(c)), httc.TurnTaker.Name + " cannot draw any cards, so they must play an equipment card"),
                new Function(httc, "Draw 1 Card", SelectionType.DrawCard,
                    () => this.GameController.DrawCards(httc, 1, cardSource: this.GetCardSource()), this.CanDrawCards(httc), httc.TurnTaker.Name + " cannot draw any cards, so they must play an equipment card.")
            };

            // Players may draw or play. 
            coroutine = this.EachPlayerSelectsFunction((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame, funcs, 0, null, (HeroTurnTakerController httc) => httc.Name + " cannot draw any cards or play any equipment cards.");
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Thri deals up to 6 targets 1 damage.
            coroutine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Projectile, 6, false, 0, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}