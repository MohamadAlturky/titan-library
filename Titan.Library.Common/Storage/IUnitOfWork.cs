namespace Titan.Library.Common.Storage;

public interface IUnitOfWork
{
    void BeginTransaction();
    void Commit();
    void RollBack();
}