using PolymindGames.ProceduralMotion;

namespace PolymindGames
{
    public interface ICameraMotionHandler : ICharacterModule
    {
        IMotionMixer MotionMixer { get; }
        IMotionDataHandler DataHandler { get; }
    }
}