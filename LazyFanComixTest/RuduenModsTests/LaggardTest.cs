using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.Laggard;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class LaggardTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(LaggardCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Laggard
        { get { return FindHero("Laggard"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Laggard", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Laggard);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Laggard);
            Assert.IsInstanceOf(typeof(LaggardCharacterCardController), Laggard.CharacterCardController);

            Assert.AreEqual(27, Laggard.CharacterCard.HitPoints);
            AssertNumberOfCardsInDeck(Laggard, 36);
            AssertNumberOfCardsInHand(Laggard, 4);
        }

    }
}