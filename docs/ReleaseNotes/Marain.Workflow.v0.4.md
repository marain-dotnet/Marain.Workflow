# Release notes for Marain.Workflow v0.4.

## v0.4

Now using `Marain.TenantManagement` v3. This means we can supply a V3-style Service Manifest, and we
now use configuration keys for V3-style tenant storage configuration settings in line with conventions.

### Breaking changes

This changes the keys under which we expect V3-style storage configuration to be stored in tenant
properties.

We are not aware of any deployments that use the new V3-style storage configuration (not least
because we never created an updated service manifest, and until recently the tenant management
tooling was only able to create v2-style entries). However, since it's technically possible that
someone could manually have created suitable configuration entries, this is a breaking change.

Note that the service tenant ID remains unchanged and the service name is still Workflow V1 because
we continue to support the old-style configuration. So although upgrading from v0.3-v0.4 has the
potential to be a breaking change, upgrading from v0.2-v0.4 will be fine.