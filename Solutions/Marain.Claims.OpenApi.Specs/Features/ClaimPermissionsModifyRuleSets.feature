@perFeatureContainer
@useClaimsApi

Feature: ClaimPermissionsModifyRuleSets
    Service endpoints for modifying the rule sets of claim permissions

Background:
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'

Scenario: Get existing ClaimPermissions that was empty when first fetched but has since had rulesets added
    Given the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And an existing ruleset with id 'rs1' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs2' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And an existing ruleset with id 'rs3' named 'Ruleset 3' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the ClaimsPermission with id 'id1' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to add rulesets with these ids
        | ID  |
        | rs1 |
        | rs2 |
        | rs3 |
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

Scenario: Get existing ClaimPermissions that was created with rulesets and since the first fetch, the ClaimPermissions has not changed but the rulesets have
    Given an existing ruleset with id 'rs11' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs12' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And an existing ruleset with id 'rs13' named 'Ruleset 3' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the new ClaimsPermission has these ruleset IDs
        | ID  |
        | rs11 |
        | rs12 |
        | rs13 |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And these rules are added to the existing ruleset with id 'rs11'
        | AccessType | ResourceUri | ResourceName | Permission |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And these rules are added to the existing ruleset with id 'rs12'
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2c         | R2c          | Allow      |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs11' named 'Ruleset 1' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs12' named 'Ruleset 2' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
        | PATCH      | r2c         | R2c          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs13' named 'Ruleset 3' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
        | PATCH      | r2c         | R2c          | Allow      |
        | PATCH      | r2a         | R2a          | Allow      |

Scenario: Get existing ClaimPermissions that was created with rulesets but has since had rulesets removed
    Given an existing ruleset with id 'rs21' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs22' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And an existing ruleset with id 'rs23' named 'Ruleset 3' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the new ClaimsPermission has these ruleset IDs
        | ID  |
        | rs21 |
        | rs22 |
        | rs23 |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And the ClaimsPermission with id 'id1' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to remove rulesets with these ids
        | ID  |
        | rs21 |
        | rs23 |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
    And the ClaimPermissions returned by the Claims service has 1 ruleset
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs22' named 'Ruleset 2' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |

Scenario: Get existing ClaimPermissions that was created with rulesets but has since had rulesets replaced wholesale
    Given an existing ruleset with id 'rs31' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs32' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And an existing ruleset with id 'rs33' named 'Ruleset 3' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And an existing ruleset with id 'rs34' named 'Ruleset 4' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | POST       | r4a         | R4a          | Allow      |
        | DELETE     | r4a         | R4a          | Allow      |
    And the new ClaimsPermission has these ruleset IDs
        | ID  |
        | rs31 |
        | rs33 |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And these ruleset IDs are POSTed to the setClaimPermissionsResourceAccessRuleSets endpoint for the ClaimsPermission with id named 'id1'
        | ID  |
        | rs32 |
        | rs33 |
        | rs34 |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
    And the ClaimPermissions returned by the Claims service has 3 rulesets
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs32' named 'Ruleset 2' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs33' named 'Ruleset 3' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | PATCH      | r2a         | R2a          | Allow      |
    And the ClaimPermissions returned by the Claims service has a ruleset with id 'rs34' named 'Ruleset 4' with these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | POST       | r4a         | R4a          | Allow      |
        | DELETE     | r4a         | R4a          | Allow      |
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r2a         | R2a          | Allow      |
        | GET        | r2b         | R2b          | Allow      |
        | PATCH      | r2a         | R2a          | Allow      |
        | POST       | r4a         | R4a          | Allow      |
        | DELETE     | r4a         | R4a          | Allow      |

Scenario: Existing ClaimPermissions updated to add non-existent rulesets
    Given the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And an existing ruleset with id 'rs41' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs41' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    When the ClaimsPermission with id 'id1' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to add rulesets with these ids
        | ID  |
        | rs42 |
    Then the HTTP status returned by the Claims service is 400

Scenario: Existing ClaimPermissions rulesets replaced wholesale with set including non-existent rulesets
    Given the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And an existing ruleset with id 'rs51' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimsPermission with id 'id1' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to add rulesets with these ids
        | ID  |
        | rs51 |
    When these ruleset IDs are POSTed to the setClaimPermissionsResourceAccessRuleSets endpoint for the ClaimsPermission with id named 'id1'
        | ID  |
        | rs51 |
        | rs52 |
    Then the HTTP status returned by the Claims service is 400

Scenario Outline: Existing ClaimPermissions updated to remove rule it doesn't have
    Given an existing ruleset with id 'rs61' named 'Ruleset 1' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And an existing ruleset with id 'rs62' named 'Ruleset 2' and these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the new ClaimsPermission has these ruleset IDs
        | ID  |
        | rs61 |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When the ClaimsPermission with id 'id1' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to remove rulesets with these ids
        | ID  |
        | rs61 |
        | rs62 |
    Then the HTTP status returned by the Claims service is 400
