using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Vagabond
{
    public class TwinFangDaggerCardController : SharedEquipmentCardController
    {
        public TwinFangDaggerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNums = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2),
                this.GetPowerNumeral(2, 1)
            };

            List<DealDamageAction> ddas = new List<DealDamageAction>() { 
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController, this.CharacterCard), null, powerNums[1],DamageType.Melee),
                new DealDamageAction(this.GetCardSource(),new DamageSource(this.GameController, this.CharacterCard), null, powerNums[2],DamageType.Toxic)
            };

            return this.SelectTargetsAndDealMultipleInstancesOfDamage(ddas, null, null, powerNums[0], powerNums[0]);
        }
    }
}