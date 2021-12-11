using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.LarrysDiscountGunClub
{
    public abstract class SharedHeroGunEarnedCardController : CardController
    {
        public SharedHeroGunEarnedCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowListOfCardsNextToCard(this.Card, new LinqCardCriteria((Card c) => c.IsAmmo, "ammo"));
        }

        public abstract override IEnumerator UsePower(int index);

        public override bool CanOtherCardGoNextToThisCard(Card card)
        {
            return !card.IsAmmo || this.Card.NextToLocation.Cards.Where((Card c) => c.IsAmmo).Count() < 1;
        }
    }
}