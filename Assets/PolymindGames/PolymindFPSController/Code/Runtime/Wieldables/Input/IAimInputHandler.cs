namespace PolymindGames.WieldableSystem
{
    public interface IAimInputHandler
    {
        bool IsAiming { get; }
        ActionBlockHandler AimBlocker { get; }

        void StartAiming();
        void EndAiming();
    }
}