namespace PolymindGames
{
    public interface IHungerManager : ICharacterModule
    {
        float Hunger { get; set; }
        float MaxHunger { get; set;  }
    }
}