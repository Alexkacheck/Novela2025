using System;

public interface ITickService
{
    event Action OnTick;
}