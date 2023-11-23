namespace UI
{
    interface IGetData<T>
    {
        T Data { get; }
        void SetItem(T data);
        void Hide();
    }
}