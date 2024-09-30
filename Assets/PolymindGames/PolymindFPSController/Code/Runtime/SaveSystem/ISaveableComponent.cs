namespace PolymindGames
{
    public interface ISaveableComponent
	{
		void LoadMembers(object[] members);
		object[] SaveMembers();
	}
}