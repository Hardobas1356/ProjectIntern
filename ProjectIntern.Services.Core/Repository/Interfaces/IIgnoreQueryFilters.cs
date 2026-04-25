namespace ProjectIntern.Services.Core.Repository.Interfaces;

public interface IIgnoreQueryFilters<T> where T : class
{
    IQueryable<T> IgnoreQueryFilters();
}
