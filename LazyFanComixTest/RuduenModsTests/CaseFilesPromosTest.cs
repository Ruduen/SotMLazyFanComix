using CaseFiles.Tucker;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.HeroPromos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

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
            QuickHPCheck(-2);
            QuickHandCheck(1);
            AssertInTrash(discards);
        }

        [Test()]
        public void TestTuckerSpellforge()
        {
            SetupGameController("BaronBlade", "CaseFiles.Tucker", "LazyFanComix.Spellforge/SpellforgeRedefineCharacter", "Megalopolis");

            HeroTurnTakerController Spellforge = FindHero("Spellforge");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron);
            DecisionSelectCards = new List<Card>() { PutInHand("Inspired"), PutInHand("Bank") , baron.CharacterCard, null };
            UsePower(Spellforge);
            QuickHPCheck(-3);

        }

        #endregion Homebrew Tests
    }
}