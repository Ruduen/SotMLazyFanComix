using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Reflection;
using CaseFiles.Tucker;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;

namespace LazyFanComixTest
{
    [TestFixture]
    public class CaseFilesPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            ModHelper.AddAssembly("CaseFiles", Assembly.GetAssembly(typeof(TuckerBaseCardController)));
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController)), true, true);
        }

        #region Homebrew Tests


        [Test()]
        public void TestTuckerPromo()
        {
            SetupGameController("BaronBlade", "Legacy", "CaseFiles.Tucker/LazyFanComix.TuckerRunAndGun", "Megalopolis");

            HeroTurnTakerController Tucker = FindHero("Tucker");

            Assert.IsTrue(Tucker.CharacterCard.IsPromoCard);

            StartGame();
            DestroyNonCharacterVillainCards();

            List<Card> discards = new List<Card>() { PutInHand("Revolver"), PutInHand("TwinPistols") };
            DecisionSelectCards = new List<Card>() { discards[0], discards[1], baron.CharacterCard };
            QuickHandStorage(Tucker);
            QuickHPStorage(baron);
            UsePower(Tucker);
            QuickHandCheck(0);
            QuickHPCheck(-2);
            AssertInTrash(discards);


        }
        #endregion
    }
}