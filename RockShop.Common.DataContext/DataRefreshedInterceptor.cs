using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RockShop.Shared
{
    public class DataRefreshedInterceptor
        : IMaterializationInterceptor
    {
        public object InitializedInstance(MaterializationInterceptionData mData, object entity)
        {
            if (entity is IDataRefreshed entityWithRefreshed)
            {
                entityWithRefreshed.LastRefreshed = DateTimeOffset.UtcNow;
            }
            return entity;
        }
    }
}