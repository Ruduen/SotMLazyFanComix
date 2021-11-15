using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Collections.Generic;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class ArbitraryTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController))); // replace with your own namespace
        }

        #region Official Tests
        [Test()]
        public void TestTribunalCompletionistTurn()
        {
            SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "TheWraith", "TheCelestialTribunal");

            StartGame();
            SelectFromBoxForNextDecision("LegacyCharacter", "Legacy");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("LegacyCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);

            SelectCardsForNextDecision(representative);
            SelectFromBoxForNextDecision("YoungLegacyCharacter", "Legacy");
            UsePower(guise);

            ResetDecisions();
            SelectCardsForNextDecision(wraith.CharacterCard);
            UsePower(guise);
            ResetDecisions();

            DestroyCard(guise);
            DecisionSelectTurnTaker = representative.Owner;

            UseIncapacitatedAbility(guise, 2);
        }

        [Test()]
        public void TestMissInformationDealingDamage()
        {
            SetupGameController("MissInformation", "TheWraith", "Legacy", "Guise", "RookCity");

            StartGame();

            PlayCard("ScumAndVillainy");
            GoToStartOfTurn(env);
        }

        #endregion Official Tests
    }
}