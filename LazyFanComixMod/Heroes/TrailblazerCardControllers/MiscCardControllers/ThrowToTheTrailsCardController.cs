﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace LazyFanComix.Trailblazer
{
    public class ThrowToTheTrailsCardController : CardController
    {
        public ThrowToTheTrailsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }
    }
}