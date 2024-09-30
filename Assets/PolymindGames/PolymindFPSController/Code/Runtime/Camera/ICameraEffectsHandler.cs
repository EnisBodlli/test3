namespace PolymindGames
{
    public interface ICameraEffectsHandler : ICharacterModule
    {
        void DoAnimationEffect(CameraEffectSettings effect);
    }
}