using System.Reflection;
using ArtifactComics.DuskveilFalls;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace LazyFanComixTest
{
  [TestFixture]
  public class ModArtifactTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
      // Tell the engine about our mod assembly so it can load up our code.
      // It doesn't matter which type as long as it comes from the mod's assembly.
      ModHelper.AddAssembly("ArtifactComics", Assembly.GetAssembly(typeof(DuskveilFallsTurnTakerController)));
    }

    #region Homebrew Tests

    //[Test()]
    //public void TestDuskInfiniteLoop()
    //{
    //  SetupGameController("BaronBlade", "LaComodora", "ArtifactComics.DuskveilFalls");

    //  StartGame();
    //  DestroyNonCharacterVillainCards();

    //  PutInTrash(comodora, (Card c) => c.Location == comodora.TurnTaker.Deck);
    //  DiscardAllCards(comodora);
    //  PlayCard("ButcherBoulevardBackAlleys");
    //  DecisionSelectCard = PlayCard("LivingForceField");
    //  PlayCard("TakeTime");
    //}


    #endregion Homebrew Tests
  }
}