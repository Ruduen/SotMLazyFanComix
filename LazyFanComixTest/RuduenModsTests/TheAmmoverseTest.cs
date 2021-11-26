using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.TheTurfWar;
using System.Collections.Generic;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using LazyFanComix.TheAmmoverse;

namespace LazyFanComixTest
{
    [TestFixture]
    public class TheAmmoverseTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PulseCannonCardController))); // replace with your own namespace
        }

        #region Load Tests


        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.TheAmmoverse");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsInstanceOf(typeof(TurnTakerController), env);
        }

        #endregion Load Tests

        #region Card Tests

        [Test()]
        public void TestAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.TheAmmoverse");

            StartGame();

            Card cannon=PlayCard("PulseCannon");
            PlayCard("HollowPoints");

            UsePower(cannon);

        }
        #endregion Card Tests

    }
}