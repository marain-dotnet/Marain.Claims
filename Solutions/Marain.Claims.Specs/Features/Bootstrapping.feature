@setupTenantedCloudBlobContainer
@setupContainer

Feature: Bootstrapping
    As an initial user
    I want to initialise a tenant of the claims framework
    So that I have the ability to manage claims

@inMemoryStore
Scenario: Claims framework tenant is uninitialised service unit test
    Given the tenant is uninitialised
    When I initialise the tenant with the role id 'adminrole'
    Then the tenant is initialised
    And the service creates a claims permission with id 'adminrole' with empty resourceAccessRules and a single resourceAccessRuleSet 'marainClaimsAdministrator'
    And the service creates an access rule set with id 'marainClaimsAdministrator' with displayname 'Claims Administrator Permissions'
    And the access rule set created has the following rules
    | resourceUri                                             | resourceDisplayName          | accessType | permission |
    | RootTenant/marain/claims/api/claimPermissions/**/*      | Read Claim Permissions       | GET        | Allow      |
    | RootTenant/marain/claims/api/claimPermissions/**/*      | Modify Claim Permissions     | PUT        | Allow      |
    | RootTenant/marain/claims/api/claimPermissions           | Create Claim Permissions     | POST       | Allow      |
    | RootTenant/marain/claims/api/claimPermissions/**/*      | Add to Claim Permissions     | POST       | Allow      |
    | RootTenant/marain/claims/api/resourceAccessRuleSet/**/* | Read Resource Access Rules   | GET        | Allow      |
    | RootTenant/marain/claims/api/resourceAccessRuleSet/**/* | Modify Resource Access Rules | PUT        | Allow      |
    | RootTenant/marain/claims/api/resourceAccessRuleSet      | Create Resource Access Rules | POST       | Allow      |
    | RootTenant/marain/claims/api/resourceAccessRuleSet/**/* | Add to Resource Access Rules | POST       | Allow      |

@realStore
Scenario: Claims framework tenant is uninitialised service integration test
    Given the tenant is uninitialised
    When I initialise the tenant with the role id 'adminrole'
    Then the tenant is initialised

    # POST /claimPermissions
    # operationId: createClaimPermissions
    And a principal in the 'adminrole' role gets 'Allow' trying to create a claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to create a claim permissions

    # GET /claimPermissions/{claimPermissionsId}
    # operationId: getClaimPermissions
    And a principal in the 'adminrole' role gets 'Allow' trying to read a claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to read a claim permissions

    # GET /claimPermissions/{claimPermissionsId}/allResourceAccessRules
    # operationId: getClaimPermissionsResourceAccessRules
    And a principal in the 'adminrole' role gets 'Allow' trying to read all effective rules for a claims permission
    And a principal in the 'someotherrole' role gets 'Deny' trying to read all effective rules for a claims permission

    # POST /claimPermissions/{claimPermissionsId}/resourceAccessRules
    # operationId: updateClaimPermissionsResourceAccessRules
    And a principal in the 'adminrole' role gets 'Allow' trying to add a rule to a claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to add a rule to a claim permissions

    # PUT /claimPermissions/{claimPermissionsId}/resourceAccessRules
    # operationId: setClaimPermissionsResourceAccessRules
    And a principal in the 'adminrole' role gets 'Allow' trying to set all rules in a claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to set all rules in a claim permissions

    # POST /claimPermissions/{claimPermissionsId}/resourceAccessRuleSets
    # operationId: updateClaimPermissionsResourceAccessRuleSets
    And a principal in the 'adminrole' role gets 'Allow' trying to add a resource access rule set to the claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to add a resource access rule set to the claim permissions

    # PUT /claimPermissions/{claimPermissionsId}/resourceAccessRuleSets
    # operationId: setClaimPermissionsResourceAccessRuleSets
    And a principal in the 'adminrole' role gets 'Allow' trying to set all resource access rule sets in a claim permissions
    And a principal in the 'someotherrole' role gets 'Deny' trying to set all resource access rule sets in a claim permissions

    # POST /resourceAccessRuleSet
    # operationId: createResourceAccessRuleSet
    And a principal in the 'adminrole' role gets 'Allow' trying to create a resource access rule set
    And a principal in the 'someotherrole' role gets 'Deny' trying to create a resource access rule set

    # GET /resourceAccessRuleSet/{resourceAccessRuleSetId}
    # operationId: getResourceAccessRuleSet
    And a principal in the 'adminrole' role gets 'Allow' trying to read a resource access rule set
    And a principal in the 'someotherrole' role gets 'Deny' trying to read a resource access rule set

    # POST /resourceAccessRuleSet/{resourceAccessRuleSetId}/resourceAccessRules
    # operationId: updateResourceAccessRuleSetResourceAccessRules
    And a principal in the 'adminrole' role gets 'Allow' trying to add an access rule to the resource access rule set
    And a principal in the 'someotherrole' role gets 'Deny' trying to add an access rule to the resource access rule set

    # PUT /resourceAccessRuleSet/{resourceAccessRuleSetId}/resourceAccessRules
    # operationId: setResourceAccessRuleSetResourceAccessRules
    And a principal in the 'adminrole' role gets 'Allow' trying to set all access rules in a resource access rule set
    And a principal in the 'someotherrole' role gets 'Deny' trying to set all access rules in a resource access rule set

@inMemoryStore
Scenario: Claims framework tenant is initialised already
    Given the tenant is initialised
    When I initialise the tenant with the role id 'somedifferentrole'
    Then I am told that the tenant is already is initialised
    And no access rules sets are created
    And no claim permissions are created