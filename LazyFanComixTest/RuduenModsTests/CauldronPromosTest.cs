using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Collections.Generic;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class CauldronPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            ModHelper.AddAssembly("Cauldron", Assembly.GetAssembly(typeof(Cauldron.ExtensionMethods)));
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController)), true, true);
        }

        [Test()]
        public void TestBaccaratCombatReady()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/LazyFanComix.BaccaratCombatReadyCharacter", "Megalopolis");
            Assert.IsTrue(GetCard("BaccaratCharacter").IsPromoCard);

            StartGame();

            AssertNumberOfCardsInTrash(GetTurnTakerController(GetCard("BaccaratCharacter")), 10);
        }


        [Test()]
        public void TestNecroCombatReady()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LazyFanComix.NecroCombatReadyCharacter", "Megalopolis");
            Assert.IsTrue(GetCard("NecroCharacter").IsPromoCard);

            StartGame();

            AssertIsInPlay("CorpseExplosion");
            //AssertIsInPlay("TaintedBlood"); // Can't actually be used due to null TurnTaker check. 
            AssertIsInPlay("NecroZombie");

            AssertNumberOfCardsInHand(GetTurnTakerController(GetCard("NecroCharacter")).ToHero(), 4);
        }

        [Test()]
        public void TestPyreCombatReady()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre/LazyFanComix.PyreCombatReadyCharacter", "Megalopolis");
            Assert.IsTrue(GetCard("PyreCharacter").IsPromoCard);

            StartGame();

            AssertIsInPlay("CherenkovDrive");
            AssertIsInPlay("ParticleCollider");

            AssertNumberOfCardsInHand(GetTurnTakerController(GetCard("PyreCharacter")).ToHero(), 4);
        }

    }
}