using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.HeroPromos;
using NUnit.Framework;
using SpookyGhostwriter.Tsukiko;
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
            ModHelper.AddAssembly("SpookyGhostwriter", Assembly.GetAssembly(typeof(TsukikoCharacterCardController)));
        }

        #region Official Tests

        //[Test()]
        //public void TestTribunalCompletionistTurn()
        //{
        //    SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "TheWraith", "TheCelestialTribunal");

        //    StartGame();
        //    SelectFromBoxForNextDecision("LegacyCharacter", "Legacy");

        //    PlayCard("RepresentativeOfEarth");

        //    Card representative = FindCardInPlay("LegacyCharacter");
        //    AssertIsInPlay(representative);

        //    UsePower(representative);

        //    SelectCardsForNextDecision(representative);
        //    SelectFromBoxForNextDecision("YoungLegacyCharacter", "Legacy");
        //    UsePower(guise);

        //    ResetDecisions();
        //    SelectCardsForNextDecision(wraith.CharacterCard);
        //    UsePower(guise);
        //    ResetDecisions();

        //    DestroyCard(guise);
        //    DecisionSelectTurnTaker = representative.Owner;

        //    UseIncapacitatedAbility(guise, 2);
        //}

        //[Test()]
        //public void TestTribunalCompletionistIsUnderActive()
        //{
        //    SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "Legacy", "TheCelestialTribunal");

        //    StartGame();
        //    SelectFromBoxForNextDecision("YoungLegacyCharacter", "Legacy");
        //    UsePower(guise);

        //    ResetDecisions();
        //    SelectCardsForNextDecision(legacy.CharacterCard);
        //    UsePower(guise);

        //    Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);
        //}

        //[Test()]
        //public void TestTribunalCompletionistANemesisIsActiveMaybe()
        //{
        //    SetupGameController("FrightTrainTeam", "Guise/CompletionistGuiseCharacter", "ErmineTeam", "TheWraith", "TheCelestialTribunal");

        //    StartGame();

        //    Card nemesis = PlayCard("ManGrove");
        //    SetHitPoints(nemesis, 5);
        //    QuickHPStorage(nemesis);
        //    GoToEndOfTurn(frightTeam);
        //    QuickHPCheck(0);

        //    SelectFromBoxForNextDecision("NightMistCharacter", "NightMist");

        //    PlayCard("RepresentativeOfEarth");

        //    Card representative = FindCardInPlay("NightMistCharacter");
        //    AssertIsInPlay(representative);

        //    SelectCardsForNextDecision(representative, guise.CharacterCard);
        //    SelectFromBoxForNextDecision("DarkWatchNightMistCharacter", "NightMist");

        //    UsePower(guise);
        //    Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);

        //    ResetDecisions();

        //    GoToEndOfTurn(frightTeam);
        //    QuickHPCheck(1);

        //    DestroyCard("RepresentativeOfEarth");

        //    GoToEndOfTurn(frightTeam);
        //    QuickHPCheck(1);
        //}

        //[Test()]
        //public void TestTribunalCompletionistMedicoWhat()
        //{
        //    SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "TheSentinels", "TheFinalWasteland");

        //    StartGame();
        //    SelectFromBoxForNextDecision("AdamantDrMedico", "TheSentinels");
        //    SelectCardsForNextDecision(medico, guise.CharacterCard);

        //    UsePower(guise);
        //    Assert.IsTrue(guise.CharacterCard.UnderLocation.Cards.First().IsActive);

        //    ResetDecisions();

        //    // PlayCard("UnforgivingWasteland");
        //    DestroyCard(medico);
        //    DestroyCard(mainstay);
        //    DestroyCard(idealist);
        //    DestroyCard(writhe);

        //    PlayCard("HorrifyingDichotomy");

        //    // I am invincible! Also, wait for Handelabra to adjust logic.

        //    //AssertIncapacitated(sentinels);

        //    //DestroyCard(guise);
        //    //AssertGameOver();
        //}

        //[Test()]
        //public void TestMissInformationDealingDamage()
        //{
        //    SetupGameController("MissInformation", "TheWraith", "Legacy", "Guise", "RookCity");

        //    StartGame();

        //    PlayCard("ScumAndVillainy");
        //    GoToStartOfTurn(env);
        //}

        //[Test()]
        //public void TestBunkerF6Tribunal()
        //{
        //    IEnumerable<string> setupItems = new List<string>()
        //    {
        //        "BaronBlade", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
        //    };
        //    SetupGameController(setupItems);

        //    StartGame();

        //    SelectFromBoxForNextDecision("BunkerFreedomSixCharacter", "Bunker");
        //    PlayCard("CalledToJudgement");

        //    GoToStartOfTurn(legacy);
        //    DecisionSelectCard = FindCardInPlay("BunkerCharacter");
        //    UsePower(legacy);
        //}

        //[Test()]
        //public void TestLifelineOffturnUpheaval()
        //{
        //    SetupGameController("BaronBlade", "Lifeline", "TheWraith", "TheCelestialTribunal");

        //    StartGame();

        //    GoToUsePowerPhase(wraith);

        //    DestroyNonCharacterVillainCards();
        //    PlayCard("LivingForceField");
        //    QuickHPStorage(baron);
        //    DiscardAllCards(lifeline);
        //    PutInHand("UnnaturalUpheaval");
        //    PlayCard("CalculatedAction");
        //    QuickHPCheck(-3);

        //}

        //[Test()]
        //public void TestLifelineIsIncapDestroy()
        //{
        //    SetupGameController("BaronBlade", "Lifeline", "TheWraith", "TheCelestialTribunal");

        //    StartGame();

        //    GoToUsePowerPhase(wraith);

        //    DestroyNonCharacterVillainCards();
        //    QuickHPStorage(baron);
        //    DestroyCard(wraith);
        //    PlayCard("UnnaturalUpheaval");
        //    QuickHPCheck(-2);

        //}

        //[Test()]
        //public void TestWraithRedirect()
        //{
        //    SetupGameController("BaronBlade", "MrFixer", "TheWraith", "TheCelestialTribunal");

        //    StartGame();

        //    Card reduce = PlayCard("StunBolt");
        //    PlayCard("DrivingMantis");

        //    DecisionSelectCards = new Card[] { fixer.CharacterCard, baron.CharacterCard };

        //    UsePower(reduce);
        //}

        //[Test()]
        //public void TestChronoRangerRedirect()
        //{
        //    SetupGameController("BaronBlade", "MrFixer", "ChronoRanger", "WagnerMarsBase");

        //    StartGame();

        //    PlayCard("MeteorStorm");

        //    Card reduce = PlayCard("NeuroToxinDartThrower");
        //    PlayCard("DrivingMantis");

        //    DecisionSelectCards = new Card[] { fixer.CharacterCard, baron.CharacterCard };

        //    UsePower(reduce);
        //}

        //[Test()]
        //public void TestDestroyedWhileDestroyed()
        //{
        //    SetupGameController("BaronBlade", "Unity", "Fanatic", "TheArgentAdept", "Megalopolis");

        //    StartGame();

        //    PlayCard("BeeBot");
        //    PlayCard("AlacritousSubdominant");
        //    PutInHand("FinalDive");

        //    DecisionNextToCard = adept.CharacterCard;
        //    //PlayCard("DynamicSiphon");

        //    // TO DO: Incomplete. Figure out if Bee Bot is destroyed twice, what happens.
        //}

        //[Test()]
        //public void TestRedirectMine()
        //{
        //    SetupGameController("Ambuscade", "Unity", "Fanatic", "TheArgentAdept", "Megalopolis");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    GoToStartOfTurn(ambuscade);

        //    QuickHPStorage(fanatic);
        //    PlayCard("SonicMine");
        //    PlayCard("DivineSacrifice");
        //    QuickHPCheck(-6);

        //}

        //[Test()]
        //public void TestLadyLuck()
        //{
        //    SetupGameController("Kismet", "Unity", "Fanatic", "TheArgentAdept", "RealmOfDiscord");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();

        //    PlayCard("ImbuedVitality");
        //    PlayCard("LadyLuck");
        //    Card saved = PlayCard("ScatteredMind");

        //    PutOnDeck("UnluckyBreak");
        //    DealDamage(saved, saved, 10, DamageType.Melee);

        //    AssertIsInPlay(saved);
        //}

        //[Test()]
        //public void TestLadyLuckB()
        //{
        //    SetupGameController("Kismet", "Unity", "Fanatic", "TheArgentAdept", "RealmOfDiscord");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();

        //    PlayCard("ImbuedVitality");
        //    Card saved = PlayCard("LadyLuck");

        //    PutOnDeck("UnluckyBreak");
        //    DealDamage(saved, saved, 10, DamageType.Melee);

        //    AssertInTrash(saved);
        //}

        //[Test()]
        //public void TestMovingTarget()
        //{
        //    SetupGameController("BaronBlade", "Legacy", "Stuntman", "TheArgentAdept", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();

        //    PlayCard("MovingTarget");
        //    PlayCard("MeteorStorm");

        //    DealDamage(baron, stunt, 2, DamageType.Fire);
        //}

        //[Test()]
        //public void TestEnneadNum()
        //{
        //    SetupGameController("TheEnnead", "Legacy", "Stuntman", "TheArgentAdept", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();

        //    AssertNumberOfCardsInPlay(ennead, 10);
        //}

        //[Test()]
        //public void TestUnderCardEntersPlay()
        //{
        //    SetupGameController("BaronBlade", "VoidGuardTheIdealist", "Guise/SantaGuise", "TimeCataclysm");

        //    StartGame();

        //    QuickHPStorage(voidIdealist);
        //    PlayCard("FocusingTiara");
        //    PlayCard("FlyingStabbyKnives");
        //    DecisionSelectFunction = 1;
        //    PlayCard("SurpriseShoppingTrip");
        //    QuickHPCheck(0);
        //    UsePower(guise);
        //    QuickHPCheck(-2);

        //    GoToStartOfTurn(voidIdealist);

        //}

        //[Test()]
        //public void TestMissInfoAndTrial()
        //{
        //    SetupGameController("MissInformation", "VoidGuardTheIdealist", "Guise/SantaGuise", "TheCelestialTribunal");
        //    StartGame();

        //    PlayCard("CalledToJudgement");

        //    PlayCard("IsolatedHero");
        //}

        //[Test()]
        //public void TestMissInfoIsolatedRedirect()
        //{
        //    SetupGameController("MissInformation", "Guise", "Ra", "Legacy", "DokThorathCapital");
        //    StartGame();

        //    Card redirect = PlayCard("AbjectRefugees");
        //    PlayCard("IsolatedHero");

        //    DecisionSelectTargets = new Card[] { redirect, guise.CharacterCard };

        //    UsePower(ra);
        //}

        //[Test()]
        //public void TestGuiseBangBang()
        //{
        //    SetupGameController("Kismet", "Guise", "Expatriette", "Legacy", "DokThorathCapital");
        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    SetHitPoints(guise, 10);
        //    PlayCard("WeakHeart");

        //    PlayCard("Prejudice");
        //    Card power = PlayCard("Pride");

        //    DecisionSelectCards = new Card[] { power, kismet.CharacterCard };
        //    DecisionSelectPower = power;
        //    DecisionYesNo = true;

        //    PlayCard("LemmeSeeThat");
        //    SelectAndUsePower(guise, out _);
        //}

        //[Test()]
        //public void TestMissInfoIsolatedDoesLifelineSee()
        //{
        //    SetupGameController("MissInformation", "Lifeline", "Fanatic", "DokThorathCapital");
        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    PlayCard("CatStuckInATree");

        //    SetHitPoints(fanatic, 10);
        //    PlayCard("Embolden");
        //    //            PlayCard("IsolatedHero"); // Isolate here to isolate Fanatic.
        //    Card power=PlayCard("CrypticAlignment");
        //    PlayCard("IsolatedHero"); // Isolate here to isolate Lifeline.
        //    UsePower(power);
        //}

        //[Test()]
        //public void TestDrawTiming()
        //{
        //    SetupGameController("Omnitron", "Tachyon", "Fanatic", "DokThorathCapital");
        //    StartGame();

        //    PlayCard("InterpolationBeam");
        //    PlayCard("QuickInsight");
        //}

        //[Test()]
        //public void ChokepointLootingOneShots()
        //{
        //    SetupGameController("Chokepoint", "Tachyon", "Fanatic", "DokThorathCapital");
        //    StartGame();

        //    PlayCard("KineticLooter");
        //    PlayCard("SuckerPunch");
        //}

        [Test()]
        public void LingeringEndOfTurn()
        {
            SetupGameController("Chokepoint", "OmnitronX/OmnitronUCharacter", "Fanatic", "DokThorathCapital");
            StartGame();

            DiscardAllCards(omnix);
            UsePower(omnix);
            GoToPlayCardPhaseAndPlayCard(omnix, "SlipThroughTime");

            GoToEndOfTurn(omnix);
        }

        #endregion Official Tests

        #region Numerology Text

        //[Test()]
        //public void TestNumerology()
        //{
        //    SetupGameController("Omnitron", "SpookyGhostwriter.Tsukiko", "Megalopolis");

        //    StartGame();

        //    HeroTurnTakerController Tsukiko = FindHero("Tsukiko");
        //    Card card = PlayCard("BoxerShorts");

        //    //string description = card.GetPowerDescription(0);
        //    string description = "{Tsukiko} deals 1 target 2 Melee damage. If she dealt any damage this way, you may destroy a non-target Environment card.";
        //    List<string> numeralString = ExtractPowerNumeralsFromPowerDescription(description).ToList();

        //    Assert.IsTrue(numeralString.Count > 1);
        //    return;

        //}

        //public IEnumerable<string> ExtractPowerNumeralsFromPowerDescription(string powerDescription)
        //{
        //    List<string> list = new List<string>();
        //    List<string> list2 = (from Match m in Regex.Matches(powerDescription, "\\d+.[a-zA-Z\\- '{}]*")
        //                          select m.Value).ToList<string>();
        //    int num = 0;
        //    using (List<string>.Enumerator enumerator = list2.GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            string numText = enumerator.Current;
        //            string text = numText;
        //            if (numText.Trim().IndexOf(" ", StringComparison.CurrentCulture) <= 0)
        //            {
        //                text = (from Match m in Regex.Matches(powerDescription, "[a-zA-Z {}]+ \\d+\\.")
        //                        select m.Value).ToList<string>().FirstOrDefault<string>();
        //            }
        //            if (!string.IsNullOrEmpty(text))
        //            {
        //                int num2 = text.IndexOf(" and ", StringComparison.CurrentCulture);
        //                if (num2 > 0)
        //                {
        //                    text = text.Substring(0, num2);
        //                }
        //                int num3 = text.IndexOf(" ", StringComparison.CurrentCulture);
        //                int num4 = text.IndexOf(" or ", StringComparison.CurrentCulture);
        //                if (num4 > num3 + 1)
        //                {
        //                    text = text.Substring(0, num4);
        //                }
        //            }
        //            IEnumerable<string> source = from s in list2
        //                                         where s == numText
        //                                         select s;
        //            if (source.Count<string>() > 1 || (!string.IsNullOrEmpty(text) && Regex.IsMatch(text.Trim(), "^[0-9]+ cards*$")))
        //            {
        //                string lookbackText = numText;
        //                List<string> source2;
        //                do
        //                {
        //                    source2 = (from Match m in Regex.Matches(powerDescription, "[a-zA-Z]* " + lookbackText)
        //                               select m.Value).ToList<string>();
        //                    lookbackText = source2.ElementAtOrDefault(num);
        //                    if (lookbackText == null)
        //                    {
        //                        goto Block_12;
        //                    }
        //                }
        //                while ((from s in source2
        //                        where s == lookbackText
        //                        select s).Count<string>() > 1);
        //            IL_205:
        //                text = lookbackText;
        //                if (source.Count<string>() > 1)
        //                {
        //                    num++;
        //                    goto IL_21C;
        //                }
        //                goto IL_21C;
        //            Block_12:
        //                lookbackText = numText;
        //                goto IL_205;
        //            }
        //        IL_21C:
        //            if (!string.IsNullOrEmpty(text))
        //            {
        //                int num5 = text.IndexOf(" and ", StringComparison.CurrentCulture);
        //                if (num5 > 0)
        //                {
        //                    text = text.Substring(0, num5);
        //                }
        //                int num6 = text.IndexOf(" ", StringComparison.CurrentCulture);
        //                int num7 = text.IndexOf(" or ", StringComparison.CurrentCulture);
        //                if (num7 > num6 + 1)
        //                {
        //                    text = text.Substring(0, num7);
        //                }
        //                if (text.Last<char>() == '{')
        //                {
        //                    text = text.Substring(0, text.Length - 2);
        //                }
        //                list.Add(text.Trim());
        //            }
        //        }
        //    }
        //    return list;
        //}

        #endregion Numerology Text
    }
}