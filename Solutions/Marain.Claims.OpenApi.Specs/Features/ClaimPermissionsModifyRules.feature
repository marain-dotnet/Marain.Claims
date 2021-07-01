@perFeatureContainer
@useClaimsApi

Feature: ClaimPermissionsModifyRules
    Service endpoints for modifying the rules of claim permissions

Background:
    Given a unique ClaimsPermission id named 'id1'
    And a new ClaimsPermission with id named 'id1'

Scenario: Get existing ClaimPermissions that was empty when first fetched but has since had rules added
    Given the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    And these rules are added via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |

Scenario: Get existing ClaimPermissions that was created with rules but has since had rules removed
    Given the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    And these rules are removed via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName                 | Permission |
        | GET        | r1a         | R1a                          | Allow      |
        | POST       | r1a         | Same resource different name | Deny       |    # ResourceName should be irrelevant for matching
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1b         | R1b          | Allow      |

Scenario: Get existing ClaimPermissions that was created with rules and has since had more rules added
    Given the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    And these rules are added via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1b         | R1b          | Allow      |
        | DELETE     | r1b         | R1b          | Deny       |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1b         | R1b          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
        | DELETE     | r1b         | R1b          | Deny       |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1b         | R1b          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
        | DELETE     | r1b         | R1b          | Deny       |

Scenario: Get existing ClaimPermissions that was created with rules but has since had rules replaced wholesale
    Given the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | foo         | Foo          | Allow      |
        | POST       | foo         | Foo          | Deny       |
        | GET        | bar         | Bar          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    And ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    And these rules are POSTed to the setClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    When ClaimsPermission with id named 'id1' is fetched from the getClaimPermissions endpoint
    Then the HTTP status returned by the Claims service is 200
    And the ClaimPermissions returned by the Claims service's id matches 'id1'
    And the ClaimPermissions returned by the Claims service has exactly these defined rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the ClaimPermissions returned by the Claims service has no rulesets
    And the ClaimPermissions returned by the Claims service has exactly these effective rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |

Scenario Outline: Existing ClaimPermissions updated to add rule it already has
    Given the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When these rules are added via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType | ResourceUri | ResourceName | Permission |
        | <AccessType> | <ResourceUri> | <ResourceName> | <Permission> |
    Then the HTTP status returned by the Claims service is 400

    Examples:
        | AccessType | ResourceUri | ResourceName                 | Permission |
        | GET        | r1a         | R1a                          | Allow      |
        | POST       | r1a         | R1a                          | Deny       |
        | GET        | r1b         | R1b                          | Allow      |
        | GET        | r1b         | Same resource different name | Allow      |

Scenario Outline: Existing ClaimPermissions updated to remove rule it doesn't have
    Given the new ClaimsPermission has these rules
        | AccessType | ResourceUri | ResourceName | Permission |
        | GET        | r1a         | R1a          | Allow      |
        | POST       | r1a         | R1a          | Deny       |
        | GET        | r1b         | R1b          | Allow      |
    And the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
    When these rules are removed via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named 'id1'
        | AccessType   | ResourceUri   | ResourceName   | Permission   |
        | <AccessType> | <ResourceUri> | <ResourceName> | <Permission> |
    Then the HTTP status returned by the Claims service is 400

    Examples:
        | AccessType | ResourceUri | ResourceName               | Permission |
        | GET        | r2a         | R1a                        | Allow      |
        | GET        | r1a         | R1a                        | Deny       |
        | GET        | r1a         | Resource name not relevant | Deny       |

# Other tests we could add:
# multiple adds
# mix of adds and removes.
# Initially empty vs non-empty but still adding
# Initially non-empty, then PUT empty permissions
# Conditional PUT: matching and non-matching
