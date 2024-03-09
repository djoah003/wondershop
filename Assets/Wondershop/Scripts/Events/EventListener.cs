using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class EventListener : Attribute
{
    public EventListener()
    {
    }
}