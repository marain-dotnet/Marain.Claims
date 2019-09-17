# Claims Setup

To secure an Open API service using the
`RoleBasedOpenApiAccessControlPolicy`, you must fulfil these
prerequisites:

* Deploy the Claims Service
  * The Azure resources deployed by your ARM template(s) must create a
    CosmosDB instance provisioned for shared throughput. The Claims
    Service will create two collections in this instance (one for
    "resource access sets" and one for "claim permission"). The instance
    does not need to be dedicated to the Claims Service.
  * The Azure resources deployed by your ARM template(s) must include a
    Key Vault. This does not need to be dedicated to the Claims Service.
  * The credentials for the CosmosDB instance must be stored in the Key
    Vault.
  * The Azure resources deployed by your ARM template(s) must include a
    Function to host the service.
  * The deployment process should ensure that this Claims Service
    Function receives the configuration settings it requires (e.g., the
    key vault details, and the name of the Cosmos DB).
  * You must deploy the `Marain.Claims.Host.Functions` project into that
    Function.
  * This Claims Service Function must be configured to require Azure AD
    authentication (enable Azure 'Easy Auth').
  * In the Azure AD Application being used by Easy Auth, define at
    least two application roles; one this will be used during the
    bootstrap process to indicate which user(s) and/or service
    principal(s) will be entitled to modify permissions in the
    Claims service. (Azure AD lets you choose your own GUID, and we
    normally use `7619c293-764c-437b-9a8e-698a26250efd` unless the
    customer particularly wants to specify their own for some reason.)
    You will need to add at least one principal to this;
    see the detailed description of bootstrapping later.
    The second role is to be used by clients that just need to be able
    to evaluate permissions. We normally use `be84cc43-d5fb-4e37-91f2-1d46fc353c69`
  * The Claims Service must have the Managed Service Identity enabled.
    (The MSI Service Principal may be associated with the same Azure AD
    Application that the Claims Service is also using for Easy Auth, but
    it does not need to be.)
  * The Service Principal being used as the Claims Service's MSI must
    have permission to read from the Key Vault (to be able to retrieve
    the CosmosDB key).
* Bootstrap the Claims service so that it recognizes the application role
  you defined in its Azure AD Application. (See below for details.)
* For each service that wishes to use the
  `RoleBasedOpenApiAccessControlPolicy` do the following:
  * Enable Azure Easy Auth, requiring Azure AD authentication
  * In the Azure AD Application being used by Easy Auth, define as many
    application roles as your system requires for that service. (The
    roles will be determined by your particular application domain and
    design.)
  * In the DI container setup, calls these `ServiceCollection` extension
    methods:
    * `AddClaimsClient` (You will need to pass it the Claims service
      base URL, which must include the `/api/` on the end. So you will
      need to ensure that the service receives this base URL through its
      configuration, which typically means adding it in your ARM
      template.)
    * `AddRoleBasedOpenApiAccessControl` (You must pass this a string
      argument which acts as a namespace for your service within the
      Claims Service. Since access control in this system is based on the
      URI of the endpoint being protected, we need to be able to
      distinguish between `/api/foo` in one service and another. If you
      are writing a service that is reusable endjin IP, use a string such
      as `marain/my-service`. E.g., the Claims Service uses
      `marain/claims/`. If you are writing a service specific to a
      customer, use something like `our-customer/some-service`.)
    * Configure the policy you require in the Claims Service. (See below
      for details.)
* For each service that needs to communicate with another service that
  has been secured with the `RoleBasedOpenApiAccessControlPolicy` do the
  following:
  * Ensure the service is able to authenticate as a principal in the same
    AD tenant as is being used by the Claims Service's Easy Auth
    configuration. (The easiest way to do this is typically to enable
    the service's Managed Service Identity, but if you have a reason to
    prefer some other mechanism, that's fine. E.g., if for some reason
    your service is running in a different Azure Subscription, and that
    subscription is associated with a different AD tenant from the one
    being used by the Claims Service, then unless you are able to make
    your service's MSI Service Principal an external member of the AD
    tenant in use by the Claims Service—something I'm not even sure is
    possible—then your service will not be able to rely on the MSI
    feature and you'll need to authenticate directly against the same
    tenant as the Claims Service.)
  * When you communicate with the service, ensure that your requests set
    the HTTP `Authorization` header with the `Bearer` scheme, and a
    current access token, and that when you obtained this access token
    you specified as the target 'resource' the Application ID (aka Client
    ID) of the Azure AD Application that the Claims Service is using for
    Easy Auth. If you are using an AutoRest-generated client
    class, the easiest way to do this is to use the generated constructor
    that accepts a `ServiceClientCredentials`. See the `AddClaimsClient`
    method in the `ClaimsClientServiceCollectionExtensions` class for
    an example of how to do this when using an MSI. (If you are not
    using an MSI, you'll need to replace the
    `ServiceIdentityTokenProvider` in that code with a token provider
    that is able to obtain tokens in whatever way you have chosen to
    authenticate against the AD tenant being used by the Claims Service.)

## The ClaimsSetup tool

Several of the procedures described below can be performed using the
`ClaimsSetup.exe` command line tool built by the
`Marain.Claims.SetupTool` project. This is designed to reduce the amount
of work required for each deployment of the Claims Service and of
services that rely on the Claims Service, and also to enable certain
tasks to be automated. The tool offers multiple commands, but there are
some common aspects which are described here first.

All of the tasks that the `ClaimsSetup` tool can perform require
authentication, because they involve using either Azure's Resource
Manager API (ARM), or the Azure Active Directory Graph API. The tool
supports various options for controlling this authentication process.

All commands include a mandatory option for specifying which AD Tenant
to authenticate against. This is the only mandatory authentication
option.

If you do not specify any other options, the tool uses the OAuth2
'device flow' meaning it will show a message asking you to visit a
particular URL and enter a code that the app will show you. You will
need to open a browser and do this. Note that if you're already logged
into the Azure portal, it may silently just use whatever identity you're
already logged in with, so you might want to do this step in a 'private'
browsing session to ensure that you get a chance to log in with an
identity that has whatever characteristics are required for the task you
need to perform.

If you specify the `-d` flag, the tool will use the cached token from
the `az` Azure CLI tool. You can run `az account get-access-token` to
refresh the cached token, and as long as an up-to-date token is available
the app can use that.

## Verifying Easy Auth and MSI settings

The `ClaimsSetup.exe` command line tool built by the
`Marain.Claims.SetupTool` project can display information about the Easy
Auth and Managed Service Identity configuration of Azure Functions and
Web Apps, enabling you to verify manually that they are correctly
configured.

```
ClaimsSetup show-app-identity -t <tenantId> -s <subscriptionId> -g <resourceGroupName> -n <serviceName>
```

As with all commands, add the `-d` switch to use the 'developer'
credentials, i.e. the cached ones from the `az` Azure CLI tool.

Here, _<tenantId>_ should be the ID of the Azure AD tenant associated
with your Azure Subscription, and _<subscriptionId>_ should be the id
of the subscription. You can discover both of these with Azure
PowerShell. If you run the `Login-AzureRmAccount` command, it will show
both ids for whichever Azure subscription it chooses as your current one.
And if you run `Get-AzureRmSubscription` it will list all of the
subscriptions available to the account you logged in with, showing both
the tenant id and the subscription id. (E.g., the tenant id for the
`endjin.com` domain is `0f621c67-98a0-4ed5-b5bd-31a35be41e29`.) Or if you
prefer Azure CLI, you can run `az account list`.

When the specified Function or Web App is configured with Easy Auth
enabled in Azure AD mode, and with a Managed Service Identity enabled,
this command will show output of this form:

```
Default Easy Auth: AzureActiveDirectory
 Client ID: 1a1b3d5c-37b9-4022-860d-d27287f9f529
Managed identity:
 Type:        SystemAssigned
 TenantId:    0f621c67-98a0-4ed5-b5bd-31a35be41e29
 PrincipalId: 4126ff17-5945-4469-9cd1-e0f0e2befd14
```

If either feature is not enabled, it will show a suitable message
telling you this. (This tool does not provide any way to fix that,
because these settings are ultimately owned by the ARM template. This
just provides a straightforward way to inspect a service to verify that
it has been set up in the required way, and also to)


## Bootstrapping the Claims Service

The Claims Service uses `RoleBasedOpenApiAccessControlPolicy` to secure
access to those endpoints that can modify its settings. The endpoint
that evaluates claims is exempt—the only requirement for hitting that is
that the request must be authenticated as _some_ principal in the AD
tenant being used for Easy Auth, but we don't care who it is—but
everything else is subject to access control.

This creates a problem: when you first deploy a new instance of the
Claims Service, it has no policy defined—its CosmosDB collections will be
empty. This means the policy evaluation endpoint will deny access to
anything and everything. So you need to define some policy for the Claims
Service to be useful. But how can you do that if the endpoints for
defining policy are all secured by the Claims Service, which is currently
in a state where it denies access to everything? You can't define a
policy to grant suitable access until there is a policy that grants
suitable access!

To resolve this 'catch-22' the Claims Service provides a bootstrapping
endpoint. This endpoint is, like the permission evaluation endpoint,
exempt from the role-based policy, so you can use it when the Claims
Service is unconfigured. Moreover, the first thing that particular
endpoint does is to check whether the service is unconfigured (which it
does by looking to see if any Claim Permissions have been defined in the
CosmosDB) and if it finds that it has already been configured it will
return a 400. So you can in fact only use this endpoint on an
unconfigured Claims Service.

The way to use the bootstrapping endpoint (which, by way of an endjin
convention, is found at `/api/tenant`) is to POST it a JSON object
containing a single property, `administratorRoleClaimValue`, which must
be the id of the Application Role that you created earlier in the Azure
AD Application being used for Easy Auth on the Claims Service. This
endpoint will then create a Resource Access Rule Set with an id of
`marainClaimsAdministrator` granting full control over the Claims Service
and it will create a single Claim Permissions indicating that the
specified Application Role gets the permissions defined by that rule set.

The `ClaimsSetup.exe` command line tool built by the
`Marain.Claims.SetupTool` project can invoke this endpoint for you. You
can run it thus:

```
ClaimsSetup bootstrap-claims-service -t <tenantId> -c <claimsServiceAppId> -u <claimsServiceBaseUrl> -r <appRoleId>
```

The _<tenantId>_ must be the id for whichever Azure AD tenant the Claims
Service's Easy Auth settings have been configured to use, and the
_<claimsServiceAppId>_ must be the ID of the Azure AD Application being
used by those same Easy Auth settings. The _<claimsServiceBaseUrl>_ is
the base URL of the claims service (ending in `/api/`) and the
_<appRoleId>_ must be the ID of the Application Role you created to
represent principals with control over Claims Service configuration in
the Azure AD Application being used for Easy Auth. As mentioned earlier,
we normally use `7619c293-764c-437b-9a8e-698a26250efd` for this, unless
the customer wants to retain complete control over the AD configuration,
and has decided they want to use a different ID.

Note that the base URL of the claims service will include the tenant ID of
your _Marain tenant_ as opposed to the _tenantId_ of your Azure AD tenant.

Run this way, we've not specified any credentials, but it is necessary
to authenticate to access the Claims Service (because Easy Auth is
configured to require Azure AD authentication). So the tool will use the
same OAuth2 'device flow' described earlier in the 'Bootstrapping the
Claims Service' section. Note that the only requirement for bootstrapping
is that you use an identity that is known to the Azure AD Tenant that the
Claims Service is using for Easy Auth. (The Claims Service does not
currently check that the caller is a member of the role that it
specifies.)

In some scenarios, you might not have any user account that can
authenticate against the Azure AD Tenant being used for Claims Service
Easy Auth. For this case, we also support

## Configuring policy for a service
