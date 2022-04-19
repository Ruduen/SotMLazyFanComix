using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
        public void TestTribunalCompletionistIsUnderActive()
        {
            SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "Legacy", "TheCelestialTribunal");

            StartGame();
            SelectFromBoxForNextDecision("YoungLegacyCharacter", "Legacy");
            UsePower(guise);

            ResetDecisions();
            SelectCardsForNextDecision(legacy.CharacterCard);
            UsePower(guise);

            Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);
        }

        [Test()]
        public void TestTribunalCompletionistANemesisIsActiveMaybe()
        {
            SetupGameController("FrightTrainTeam", "Guise/CompletionistGuiseCharacter", "ErmineTeam", "TheWraith", "TheCelestialTribunal");

            StartGame();

            Card nemesis = PlayCard("ManGrove");
            SetHitPoints(nemesis, 5);
            QuickHPStorage(nemesis);
            GoToEndOfTurn(frightTeam);
            QuickHPCheck(0);

            SelectFromBoxForNextDecision("NightMistCharacter", "NightMist");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("NightMistCharacter");
            AssertIsInPlay(representative);

            SelectCardsForNextDecision(representative, guise.CharacterCard);
            SelectFromBoxForNextDecision("DarkWatchNightMistCharacter", "NightMist");

            UsePower(guise);
            Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);

            ResetDecisions();

            GoToEndOfTurn(frightTeam);
            QuickHPCheck(1);

            DestroyCard("RepresentativeOfEarth");

            GoToEndOfTurn(frightTeam);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestTribunalCompletionistMedicoWhat()
        {
            SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "TheSentinels", "TheFinalWasteland");

            StartGame();
            SelectFromBoxForNextDecision("AdamantDrMedico", "TheSentinels");
            SelectCardsForNextDecision(medico, guise.CharacterCard);

            UsePower(guise);
            Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);

            ResetDecisions();

            // PlayCard("UnforgivingWasteland");
            DestroyCard(medico);
            DestroyCard(mainstay);
            DestroyCard(idealist);
            DestroyCard(writhe);

            PlayCard("HorrifyingDichotomy");

            // I am invincible! Also, wait for Handelabra to adjust logic. 

            //AssertIncapacitated(sentinels);

            //DestroyCard(guise);
            //AssertGameOver();
        }


        [Test()]
        public void TestMissInformationDealingDamage()
        {
            SetupGameController("MissInformation", "TheWraith", "Legacy", "Guise", "RookCity");

            StartGame();

            PlayCard("ScumAndVillainy");
            GoToStartOfTurn(env);
        }

        [Test()]
        public void TestBunkerF6Tribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            SelectFromBoxForNextDecision("BunkerFreedomSixCharacter", "Bunker");
            PlayCard("CalledToJudgement");

            GoToStartOfTurn(legacy);
            DecisionSelectCard = FindCardInPlay("BunkerCharacter");
            UsePower(legacy);
        }


        [Test()]
        public void TestLifelineOffturnUpheaval()
        {
            SetupGameController("BaronBlade", "Lifeline", "TheWraith", "TheCelestialTribunal");

            StartGame();

            GoToUsePowerPhase(wraith);

            DestroyNonCharacterVillainCards();
            PlayCard("LivingForceField");
            QuickHPStorage(baron);
            DiscardAllCards(lifeline);
            PutInHand("UnnaturalUpheaval");
            PlayCard("CalculatedAction");
            QuickHPCheck(-3);

        }



        [Test()]
        public void TestLifelineIsIncapDestroy()
        {
            SetupGameController("BaronBlade", "Lifeline", "TheWraith", "TheCelestialTribunal");

            StartGame();

            GoToUsePowerPhase(wraith);

            DestroyNonCharacterVillainCards();
            QuickHPStorage(baron);
            DestroyCard(wraith);
            PlayCard("UnnaturalUpheaval");
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestWraithRedirect()
        {
            SetupGameController("BaronBlade", "MrFixer", "TheWraith", "TheCelestialTribunal");

            StartGame();

            Card reduce = PlayCard("StunBolt");
            PlayCard("DrivingMantis");

            DecisionSelectCards = new Card[] { fixer.CharacterCard, baron.CharacterCard };

            UsePower(reduce);
        }

        [Test()]
        public void TestChronoRangerRedirect()
        {
            SetupGameController("BaronBlade", "MrFixer", "ChronoRanger", "WagnerMarsBase");

            StartGame();

            PlayCard("MeteorStorm");

            Card reduce = PlayCard("NeuroToxinDartThrower");
            PlayCard("DrivingMantis");

            DecisionSelectCards = new Card[] { fixer.CharacterCard, baron.CharacterCard };

            UsePower(reduce);
        }

        #endregion Official Tests
    }
}