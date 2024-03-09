public class PlayerModel
{
	public string ID;

	public string DisplayName { get; set; } = "";

	public string Emote { get; set; } = "";

	public string Customization { get; set; } = "";

	public string GroupReference { get; set; }

	public override string ToString() => $"PlayerModel(Id: {ID}, displayName: {DisplayName}, customization: {Customization})";
}