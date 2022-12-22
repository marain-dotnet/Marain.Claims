# Data Structures in Marain Claims

## Overview

There are three main data structures in Marain Claims:

1. `ResourceAccessRule`
2. `ResourceAccessRuleSet`
3. `ClaimPermissions`

`ClaimPermissions` and `ResourceAccessRuleSet` are the two top level entities; `ResourceAccessRule` items can only exist as part of one of these two entities. In addition, `ClaimPermissions` can also contain `ResourceAccessRuleSet`s.

## `ResourceAccessRule`

The ResourceAccessRule is the lowest level entity. It represents a permission for a specific resource or resources, and for a specific access type. The resource or resources to which it relates are defined by a URI; this supports globbing so, by using wildcards, it's possible to define a `ResourceAccessRule` which applies to multiple resources.

It also supports an access type; the meaning of this is determined by the consuming application. An HTTP-based API would be likely to use the standard HTTP verbs (e.g. GET, POST, DELETE) as the different access types, but consumers can define their own set of access types if required.

Finally, it contains a flag indicating whether this rule allows or denies access to the specified resource. There's more details on how this is expected to be used in the Claim Evaluation document.

An example of a `ResourceAccessRule` for an API which manages employee data is shown below:

```json
{
    "accessType": "GET",
    "resource": {
        "uri": "api/employees/*/contacts",
        "displayName": "GET employee contacts"
    },
    "permission": "allow"
}
```

This rule can be interpreted as allowing anyone it's assigned to (see below) to access contact information for all employees (due to the `*` in the URI).

A similar rule in the same API could be:

```json
{
    "accessType": "POST",
    "resource": {
        "uri": "api/employees/*/contacts",
        "displayName": "POST a new employee contact"
    },
    "permission": "deny"
}
```

This rule explicitly denies access to create new employee contact details.

## `ResourceAccessRuleSet`

The `ResourceAccessRuleSet` groups one or more `ResourceAccessRule`s for ease of use. The exact way in which the rules are grouped is determined by the consuming application; at its simplest a `ResourceAccessRuleSet` might contain a single `ResourceAccessRule`, but in a more complex application it could contain far more.

A single `ResourceAccessRuleSet` can be referenced by multiple `ClaimPermissions`.

The below example builds on those above and shows two rules grouped together which between them would allow access to add a new contact to an employee, or to update an existing one:

```json
{
    "contentType": "application/vnd.marain.claims.resourceaccessruleset",
    "id": "employeeAddOrUpdateContact",
    "displayName": "Add or update contact information for an employee",
    "rules": [
        {
            "accessType": "POST",
            "resource": {
                "uri": "api/employees/*/contacts",
                "displayName": "POST new employee contact"
            },
            "permission": "allow"
        },
        {
            "accessType": "POST",
            "resource": {
                "uri": "api/employees/*/contacts/*",
                "displayName": "POST employee contact update"
            },
            "permission": "allow"
        }
    ]
}
```

This shows how multiple rules can be grouped into sets of logically related functionality.

## `ClaimPermissions`

The `ClaimPermissions` data type is used to group a set of `ResourceAccessRuleSet` and/or `ResourceAccessRule` together into a set of permissions which are granted to identites holding a particular claim. For example, if the consuming application implemented access control based on Azure AD application role membership, it would define a ClaimPermissions for each role.

When `ClaimPermissions` which refer to `ResourceAccessRuleSet`s are first created, they look like this:

```json
{
    "contentType": "application/vnd.marain.claims.claimpermissions",
    "id": "Manager",
    "resourceAccessRules": [],
    "resourceAccessRuleSets": [
        {
            "contentType": "application/vnd.marain.claims.resourceaccessruleset",
            "id": "employeeAddOrUpdateContact",
            "rules": []
        },
        {
            "contentType": "application/vnd.marain.claims.resourceaccessruleset",
            "id": "employeeAddOrUpdateRole",
            "rules": []
        },
```

When an implementation of `IClaimPermissionStore` is used to retrieve a `ClaimPermissions`, it is expected that it will dereference any `ResourceAccessRuleSet` it contains, so the resulting document looks more like this:

```json
{
    "contentType": "application/vnd.marain.claims.claimpermissions",
    "id": "Manager",
    "resourceAccessRules": [],
    "resourceAccessRuleSets": [
    {
        "contentType": "application/vnd.marain.claims.resourceaccessruleset",
        "id": "employeeAddOrUpdateContact",
        "eTag": "\"0x8D819BF7E13D377\"",
        "displayName": "Add or update contact information for an employee",
        "rules": [
            {
                "accessType": "POST",
                "resource": {
                    "uri": "api/employees/*/contacts",
                    "displayName": "POST new employee contact"
                },
                "permission": "allow"
            },
            {
                "accessType": "POST",
                "resource": {
                    "uri": "api/employees/*/contacts/*",
                    "displayName": "POST employee contact update"
                },
                "permission": "allow"
            }
        ]
    }
    {
        "contentType": "application/vnd.marain.claims.resourceaccessruleset",
        "id": "employeeAddOrUpdateRole",
        "eTag": "\"0x8D819BF7E4FB2A5\"",
        "displayName": "Add or update role information for an employee",
        "rules": [
            {
                "accessType": "POST",
                "resource": {
                    "uri": "api/employees/*/role",
                    "displayName": "POST new employee contact"
                },
                "permission": "allow"
            }
        ]
    }]
}
```

The underlying store can choose to persist the fully expanded `ClaimPermissions` document back to the store, or re-expand it on every request. If it persists the expanded version back, it should transparently provide a mechanism for periodically ensuring the expanded `ResourceAccessRuleset` data is up to date.