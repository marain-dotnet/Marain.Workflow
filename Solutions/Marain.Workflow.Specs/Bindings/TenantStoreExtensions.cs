namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Tenancy;

    /// <summary>
    /// Extensions for <see cref="ITenantStore"/>.
    /// </summary>
    public static class TenantStoreExtensions
    {
        public static Task<ITenant> UpdateTenantPropertiesAsync(this ITenantStore tenantStore, ITenant tenant, Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>> setOrAdd, IEnumerable<string> remove = null)
        {
            return tenantStore.UpdateTenantAsync(
                tenant.Id,
                propertiesToSetOrAdd: PropertyBagValues.Build(setOrAdd),
                propertiesToRemove: remove);
        }
    }
}
