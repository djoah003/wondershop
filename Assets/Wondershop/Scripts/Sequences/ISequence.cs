using System;

public interface ISequence
{
	public void Begin(Action onComplete);
}