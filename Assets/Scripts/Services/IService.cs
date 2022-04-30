using System;

public interface IService
{
    event Action OnInited;
    event Action OnUpdated;
    event Action OnDestroed;
}