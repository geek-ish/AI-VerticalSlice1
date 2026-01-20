using System.Collections.Generic;

namespace CsvUpdateDemo.Features.ApplyUpdates
{
    public interface IApplyUpdatesHandler
    {
        ApplyUpdatesResult Handle(IEnumerable<ProductUpdate> updates);
    }
}
