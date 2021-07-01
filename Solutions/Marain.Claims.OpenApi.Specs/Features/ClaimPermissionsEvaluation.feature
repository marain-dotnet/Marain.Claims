@perFeatureContainer
@useClaimsApi

Feature: ClaimPermissionsEvaluation
    Service endpoints that evaluate permissions based on the configured rules

Background:
    Given a unique ClaimsPermission id named 'id-none'
    And a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    And the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And a unique ClaimsPermission id named 'id2'
    And a new ClaimsPermission with id named 'id2'
    And the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | quux        | Quux         | Deny       |
        | POST       | spong       | Spong        | Allow      |
        | GET        | spong       | Spong        | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint

Scenario: Get resource access rules for unknown ClaimPermission id
    When the effective rules for ClaimsPermission with id named 'id-none' are fetched via the getClaimPermissionsResourceAccessRules endpoint
    Then the HTTP status returned by the Claims service is 404

Scenario: Evaluate single permission for unknown ClaimPermission id
    When permissions are evaluated via the getClaimPermissionsPermission endpoint ClaimsPermission id 'id-none', resource 'foo' and access type 'GET'
    Then the HTTP status returned by the Claims service is 404

Scenario: Evaluate permissions batch including unknown ClaimPermission id
    When these permissions are evaluated via the getClaimPermissionsPermissionBatch endpoint
        | ClaimsPermissionsId | ResourceUri | AccessType |
        | id-none             | foo         | GET        |
    Then the HTTP status returned by the Claims service is 200
    And the permissions batch response items are
        | ClaimsPermissionId | ResourceUri | AccessType | ResponseCode | Permission |
        | id-none            | foo         | GET        | 404          | deny       |

Scenario: Get resource access rules for known ClaimPermission id
    When the effective rules for ClaimsPermission with id named 'id1' are fetched via the getClaimPermissionsResourceAccessRules endpoint
    Then the HTTP status returned by the Claims service is 200
    And the effective rules returns by the Claims service are
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |

Scenario Outline: Evaluate single permission for known ClaimPermission id
    When permissions are evaluated via the getClaimPermissionsPermission endpoint ClaimsPermission id '<ClaimsPermissionsId>', resource '<ResourceUri>' and access type '<AccessType>'
    Then the HTTP status returned by the Claims service is 200
    And the permission returned by the Claims service is '<Permission>'

    Examples:
        | ClaimsPermissionsId | ResourceUri | AccessType | Permission |
        | id1                 | foo         | GET        | Allow      |
        | id1                 | foo         | POST       | Deny       |
        | id1                 | foo         | PATCH      | Deny       |
        | id1                 | unknown     | GET        | Deny       |


Scenario: Evaluate permissions batch for known ClaimPermissions
    When these permissions are evaluated via the getClaimPermissionsPermissionBatch endpoint
        | ClaimsPermissionsId | ResourceUri | AccessType |
        | id1                 | foo         | GET        |
        | id1                 | foo         | POST       |
        | id1                 | foo         | DELETE     |
        | id1                 | unknown     | GET        |
        | id2                 | spong       | POST       |
    Then the HTTP status returned by the Claims service is 200
    And the permissions batch response items are
        | ClaimsPermissionId | ResourceUri | AccessType | ResponseCode | Permission |
        | id1                | foo         | GET        | 200          | allow      |
        | id1                | foo         | POST       | 200          | deny       |
        | id1                | foo         | DELETE     | 200          | deny       |
        | id1                | unknown     | GET        | 200          | deny       |
        | id2                | spong       | POST       | 200          | allow      |
