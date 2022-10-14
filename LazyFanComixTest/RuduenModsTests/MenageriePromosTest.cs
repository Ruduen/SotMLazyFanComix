using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Reflection;
using Menagerie.Radiance;
using Handelabra.Sentinels.Engine.Controller;

namespace LazyFanComixTest
{
    [TestFixture]
    public class MenageriePromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            ModHelper.AddAssembly("Menagerie", Assembly.GetAssembly(typeof(RadianceCharacterCardController)));
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController)), true, true);
        }

        #region Homebrew Tests

        [Test()]
        public void TestRadianceIsolated()
        {
            SetupGameController("MissInformation", "Legacy", "Menagerie.Radiance", "Megalopolis");

            HeroTurnTakerController Radiance = FindHero("Radiance");

            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("IsolatedHero");
            PlayCard("LightTheWay");
        }
        #endregion
    }
}