Feature: RoleBasedOpenApiAccessControlPolicy
    In order to secure an OpenApi service
    As a developer
    I want to apply application role-based security

Scenario: The client is unauthenticated
    Given I am not authenticated
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    Then the result type should be 'NotAuthenticated'
    And the policy should not have attempted to use the claims service

Scenario: The client belongs to no roles
    Given I have a ClaimsPrincipal with 0 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    Then the result type should be 'NotAllowed'
    And the policy should not have attempted to use the claims service

Scenario Outline: The client belongs to one role
    Given I have a ClaimsPrincipal with 1 roles claims
    And the policy has a resource prefix of '<resourcePrefix>'
    When I invoke the policy with a path of '<path>' and a method of '<method>'
    Then the policy should pass the claim permissions id for role 0 to the claims service
    And the policy should pass the tenant id to the claims service in call for role 0
    And the policy should pass a resource URI of '<resourceUri>' to the claims service in call for role 0
    And the policy should pass an access type of '<method>' to the claims service in call for role 0

    Examples:
    | resourcePrefix | path     | method | resourceUri        |
    |                | /foo/bar | GET    | foo/bar            |
    |                | /baz/ick | GET    | baz/ick            |
    |                | /foo/bar | PUT    | foo/bar            |
    | testPrefix/    | /foo/bar | GET    | testPrefix/foo/bar |
    | testPrefix/    | /foo/bar | PUT    | testPrefix/foo/bar |

Scenario: The client's only role grants it permission
    Given I have a ClaimsPrincipal with 1 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator returns 'Allow' for role 0
    Then the result should grant access

Scenario: The client's only role denies it permission
    Given I have a ClaimsPrincipal with 1 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator returns 'Deny' for role 0
    Then the result type should be 'NotAllowed'

Scenario: The claims service doesn't recognize the role
    Given I have a ClaimsPrincipal with 1 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
    And the evaluator does not find the role 0
    Then the result type should be 'NotAllowed'

Scenario: The client belongs to three roles
    Given I have a ClaimsPrincipal with 3 roles claims
    When I invoke the policy with a path of 'foo/bar' and a method of 'GET'
    Then the policy should pass the claim permissions id for role 0 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for role 0
    And the policy should pass an access type of 'GET' to the claims service in call for role 0
    And the policy should pass the claim permissions id for role 1 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for role 1
    And the policy should pass an access type of 'GET' to the claims service in call for role 1
    And the policy should pass the claim permissions id for role 2 to the claims service
    And the policy should pass a resource URI of 'foo/bar' to the claims service in call for role 2
    And the policy should pass an access type of 'GET' to the claims service in call for role 2

Scenario: All of the client's roles grant it permission
    Given I have a ClaimsPrincipal with 3 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Allow  |
	| 1    | Allow  |
	| 2    | Allow  |
    Then the result should grant access

Scenario: All of the client's roles deny it permission
    Given I have a ClaimsPrincipal with 3 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Deny   |
	| 1    | Deny   |
	| 2    | Deny   |
    Then the result type should be 'NotAllowed'

Scenario: One client role grants permission and the other two deny it in Allow If Any mode
    Given I have a ClaimsPrincipal with 3 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Allow  |
	| 1    | Deny   |
	| 2    | Deny   |
    Then the result should grant access

Scenario: Two client roles grant permission and the other denies it in Allow If Any mode
    Given I have a ClaimsPrincipal with 3 roles claims
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Allow  |
	| 1    | Allow  |
	| 2    | Deny   |
    Then the result should grant access

Scenario: One client role grants permission and the other two deny it in All Only If All mode
    Given I have a ClaimsPrincipal with 3 roles claims
    And the policy is configured in allow only if all mode
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Allow  |
	| 1    | Deny   |
	| 2    | Deny   |
    Then the result type should be 'NotAllowed'

Scenario: Two client roles grant permission and the other denies it in All Only If All mode
    Given I have a ClaimsPrincipal with 3 roles claims
    And the policy is configured in allow only if all mode
    When I invoke the policy with a path of '/foo/bar' and a method of 'GET'
	And the evaluator returns the following results
	| Role | Result |
	| 0    | Allow  |
	| 1    | Allow  |
	| 2    | Deny   |
    Then the result type should be 'NotAllowed'
