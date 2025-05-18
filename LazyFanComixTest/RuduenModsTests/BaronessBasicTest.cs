using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.TheBaroness;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
  [TestFixture]
  public class BaronessBasicTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TheBaronessBasicCharacterCardController)));
      ModHelper.AddAssembly("VainFacadePlaytest", Assembly.GetAssembly(typeof(VainFacadePlaytest.TheBaroness.TheBaronessTurnTakerController)));

    }
    protected TurnTakerController Baroness
    { get { return FindVillain("TheBaroness"); } }


    #region Load Tests

    [Test()]
    public void TestModWorks()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

      Assert.IsNotNull(Baroness);
      Assert.IsInstanceOf(typeof(TurnTakerController), Baroness);
      Assert.IsInstanceOf(typeof(TheBaronessBasicCharacterCardController), Baroness.CharacterCardController);
    }

    [Test()]
    public void TestSetupWorks()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      AssertIsInPlay("Vampirism");
    }

    #endregion Load Tests

    #region Victory Tests

    [Test()]
    public void TestVictory()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      FlipCard(Baroness);
      AssertNotGameOver();
      FlipCard(Baroness);
      AssertNotGameOver();

      DestroyCard(Baroness.CharacterCard);
      AssertGameOver(EndingResult.VillainDestroyedVictory);

    }

    #endregion Victory Tests

    #region Effect Tests

    [Test()]
    public void TestBaseStartOfTurn()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame(); // Game starts by resolving SoT.
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 1);

      GoToStartOfTurn(Baroness);

      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 2);
    }

    [Test()]
    public void TestBaseStartOfTurnFlip()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "Megalopolis");

      StartGame();
      DestroyNonCharacterVillainCards();
      PutInTrash(Baroness, FindCardsWhere((Card c) => c.Location == Baroness.TurnTaker.Deck));

      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 0);
      AssertFlipped(Baroness);


      GoToStartOfTurn(Baroness);
      AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("scheme"), 0);
    }

    [Test()]
    public void TestBaseEndOfTurn()
    {
      SetupGameController("VainFacadePlaytest.TheBaroness/LazyFanComix.TheBaronessBasic", "Legacy", "TheWraith", "Haka", "Unity", "Megalopolis");

      StartGame(); // Game starts by resolving SoT.
      FlipCard(Baroness);
      Card target = PutIntoPlay("SwiftBot");
      QuickHPStorage(legacy.CharacterCard, wraith.CharacterCard, haka.CharacterCard, unity.CharacterCard, target);

      GoToEndOfTurn(Baroness); // Should be increased?
      QuickHPCheck(-2, -2, 0, -2, -2);
      AssertFlipped(Baroness);

      DestroyNonCharacterVillainCards();
      GoToEndOfTurn(Baroness);
      QuickHPCheck(-1, -1, 0, -1, -1);
      AssertNotFlipped(Baroness);
    }

    #endregion General Tests
  }
}