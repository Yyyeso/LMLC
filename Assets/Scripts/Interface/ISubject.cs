public interface ISubject<T>
{
    void Add(T observer);
    void Delete(T observer);
}
