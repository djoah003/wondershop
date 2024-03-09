public interface IProgress
{
	public void SetActive(bool value);
	public void Fill(float current = 0f, float max = 1f, bool inverse = false);
}