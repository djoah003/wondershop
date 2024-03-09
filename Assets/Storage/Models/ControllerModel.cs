public class ControllerModel
{
	public string ID;

	public string PlayerReference { get; set; }

	public override string ToString() => $"ControllerModel(Id: {ID}, playerReference: {PlayerReference})";
}