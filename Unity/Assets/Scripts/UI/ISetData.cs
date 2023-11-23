namespace UI
{
    interface ISetData<T>
    {
        T Data { get; }
        void SetItem(T data);
        void Hide();
    }
}