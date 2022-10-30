using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using Angille.Theurgy;

namespace LazyFanComixTest
{
    [TestFixture]
    public class MythikalPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            ModHelper.AddAssembly("Angille", Assembly.GetAssembly(typeof(TheurgyCharacterCardController)));
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController)), true, true);
        }

        #region Homebrew Tests

        [Test()]
        public void TestTheurgy()
        {
            SetupGameController("BaronBlade", "Legacy", "Angille.Theurgy/LazyFanComix.TheurgyTieTheLines", "Megalopolis");

            HeroTurnTakerController Theurgy = FindHero("Theurgy");

            Assert.IsTrue(Theurgy.CharacterCard.IsPromoCard);

            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHandStorage(Theurgy);
            Card charm = PutOnDeck("GuideTheStrike");

            UsePower(Theurgy);
            QuickHandCheck(0); // Net 0. 
            AssertInHand(charm);
        }

        #endregion
    }
}