@perFeatureContainer
@useClaimsApi

Feature: ClaimPermissions
    Service endpoints for creating and getting claim permissions

Scenario: GET non-existent ClaimPermissions
    Given a unique ClaimsPermission id named 'id1'
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 404

Scenario: POST new ClaimPermissions with no rules or rulesets
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    When the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    # TODO: arguably this should be 201 (Created), but the current implementation returns 200, and the new ClaimsPermission in the result body, so that could be a breaking change for some clients.
    Then the HTTP status returned by the Claims service is 200

Scenario: Get existing ClaimPermissions with no rules or rulesets
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'

Scenario: Get existing ClaimPermissions that was created with rules
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    And the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |

Scenario: Get existing ClaimPermissions that was created with rulesets
    Given an existing ruleset with id 'rs1' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    Given an existing ruleset with id 'rs2' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    Given an existing ruleset with id 'rs3' named 'Ruleset 3' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    And the new ClaimsPermission has these ruleset IDs
        | ID  |
        | rs1 |
        | rs2 |
        | rs3 |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs1' named 'Ruleset 1' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs2' named 'Ruleset 2' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs3' named 'Ruleset 3' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
        | PATCH      | r2a         | R2a          | Allow      |

Scenario: Get existing ClaimPermissions that was empty when first fetched but has since had rules added
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    And these rules are POSTed to the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
# Variations on the above:
# multiple adds
# add vs update (POST vs PUT to /{tenantId}/marain/claims/claimPermissions/{claimPermissionsId}/resourceAccessRules)
# mix of adds and updates.
# Initially empty vs non-empty but still adding
# Initially non-empty, then PUT empty permissions
# Conditional PUT: matching and non-matching
# What should happen if we POST a rule that's exactly the same as one that's already in there?


Scenario: Get existing ClaimPermissions that was empty when first fetched but has since had rulesets added

Scenario: Get existing ClaimPermissions that was created with rulesets and since the first fetch, the ClaimPermissions has not changed but the rulesets have

Scenario: POST ClaimPermissions with existing ID

# TODO: what's the expected behaviour if we pass in a fully-formed ruleset inline in the ClaimPermissions
# instead of one with just an ID?
