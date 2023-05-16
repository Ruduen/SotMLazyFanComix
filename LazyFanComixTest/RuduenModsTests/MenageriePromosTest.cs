using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.HeroPromos;
using Menagerie.Radiance;
using NUnit.Framework;
using System.Reflection;

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

        [Test()]
        public void TestCharadePromo()
        {
            SetupGameController("MissInformation", "Legacy", "Menagerie.Charade/LazyFanComix.CharadeRecklessPlans", "Megalopolis");

            HeroTurnTakerController Charade = FindHero("Charade");

            Assert.IsTrue(Charade.CharacterCard.IsPromoCard);

            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHandStorage(Charade);
            UsePower(Charade);
            QuickHandCheck(1);
            AssertNumberOfCardsUnderCard(Charade.CharacterCard, 2);

            MoveAllCards(Charade, Charade.TurnTaker.Deck, Charade.TurnTaker.Trash, leaveSomeCards: 1);

            QuickHandStorage(Charade);
            UsePower(Charade);
            QuickHandCheck(1);
            AssertNumberOfCardsUnderCard(Charade.CharacterCard, 2);

            QuickHandStorage(Charade);
            UsePower(Charade);
            QuickHandCheck(1);
            AssertNumberOfCardsUnderCard(Charade.CharacterCard, 1);
        }

        #endregion Homebrew Tests
    }
}