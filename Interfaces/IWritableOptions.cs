using Microsoft.Extensions.Options;

namespace AircraftTracker.Interfaces;
internal interface IWritableOptions<out T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
}
