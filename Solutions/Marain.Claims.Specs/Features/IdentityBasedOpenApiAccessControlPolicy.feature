@identitybased
Feature: IdentityBasedOpenApiAccessControlPolicy
    In order to secure an OpenApi service
    As a developer
    I want to apply identity-based security

Scenario: The client is unauthenticated
    Given I am not authenticated
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    Then the result type should be 'NotAuthenticated'
    And the policy should not have attempted to use the claims service

Scenario: The client has no identities
    Given I have a ClaimsPrincipal with 0 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    Then the result type should be 'NotAllowed'
    And the policy should not have attempted to use the claims service

Scenario Outline: The client belongs to one identity
    Given I have a ClaimsPrincipal with 1 oid claims
    And the policy has a resource prefix of '<resourcePrefix>'
    When I invoke the policy with a path of '<path>' and a method of '<method>'
    Then the policy should pass the claim permissions id 0 to the claims service
    And the policy should pass the tenant id to the claims service in call for claims permissions ID 0
    And the policy should pass a resource URI of '<resourceUri>' to the claims service in call for claims permissions ID 0
    And the policy should pass an access type of '<method>' to the claims service in call for claims permissions ID 0

    Examples:
    | resourcePrefix | path     | method | resourceUri        |
    |                | /foo/bar | GET    | foo/bar            |
    |                | /baz/ick | GET    | baz/ick            |
    |                | /foo/bar | PUT    | foo/bar            |
    | testPrefix/    | /foo/bar | GET    | testPrefix/foo/bar |
    | testPrefix/    | /foo/bar | PUT    | testPrefix/foo/bar |

Scenario: The client's only identity grants it permission
    Given I have a ClaimsPrincipal with 1 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator returns 'Allow' for claim permissions ID 0
    Then the result should grant access

Scenario: The client's only identity denies it permission
    Given I have a ClaimsPrincipal with 1 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator returns 'Deny' for claim permissions ID 0
    Then the result type should be 'NotAllowed'

Scenario: The claims service doesn't recognize the identity
    Given I have a ClaimsPrincipal with 1 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator does not find the claim permissions ID 0
    Then the result type should be 'NotAllowed'

Scenario: The client belongs to three identities
    Given I have a ClaimsPrincipal with 3 oid claims
    When I invoke the policy with a path of 'foo/bar' and a method of 'GET'
    Then the policy should pass the claim permissions id 0 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for claims permissions ID 0
    And the policy should pass an access type of 'GET' to the claims service in call for claims permissions ID 0
    And the policy should pass the claim permissions id 1 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for claims permissions ID 1
    And the policy should pass an access type of 'GET' to the claims service in call for claims permissions ID 1
    And the policy should pass the claim permissions id 2 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for claims permissions ID 2
    And the policy should pass an access type of 'GET' to the claims service in call for claims permissions ID 2

Scenario: All of the client's identities grant it permission
    Given I have a ClaimsPrincipal with 3 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | allow  |
	| 1                  | allow  |
	| 2                  | allow  |
    Then the result should grant access

Scenario: All of the client's identities deny it permission
    Given I have a ClaimsPrincipal with 3 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | deny   |
	| 1                  | deny   |
	| 2                  | deny   |
    Then the result type should be 'NotAllowed'

Scenario: One client identity grants permission and the other two deny it in Allow If Any mode
    Given I have a ClaimsPrincipal with 3 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | allow  |
	| 1                  | deny   |
	| 2                  | deny   |
    Then the result should grant access

Scenario: Two client identities grant permission and the other denies it in Allow If Any mode
    Given I have a ClaimsPrincipal with 3 oid claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | allow  |
	| 1                  | allow  |
	| 2                  | deny   |
    Then the result should grant access

Scenario: One client identity grants permission and the other two deny it in All Only If All mode
    Given I have a ClaimsPrincipal with 3 oid claims
    And the policy is configured in allow only if all mode
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | allow  |
	| 1                  | deny   |
	| 2                  | deny   |
    Then the result type should be 'NotAllowed'

Scenario: Two client identities grant permission and the other denies it in All Only If All mode
    Given I have a ClaimsPrincipal with 3 oid claims
    And the policy is configured in allow only if all mode
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| ClaimPermissionsId | Result |
	| 0                  | allow  |
	| 1                  | allow  |
	| 2                  | deny   |
    Then the result type should be 'NotAllowed'
