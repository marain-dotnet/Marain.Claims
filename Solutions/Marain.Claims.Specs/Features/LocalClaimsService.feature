Feature: LocalClaimsService
    In order to secure access to the Claims service
    As a developer of the Claims service
    I want the RoleBasedOpenApiAccessControlPolicy to be able to evaluate claim permissions without calling into itself over HTTP

Scenario Outline: Claim forwards arguments to service
    When I have passed a claim permissions id of '<claimPermissionsId>', a resource URI of '<resourceUri>', an access type of '<accessType>', and a tenant id of '<tenantId>'
    Then the service should have been passed a claim permissions id of '<claimPermissionsId>'
    Then the service should have been passed a resource URI of '<resourceUri>'
    Then the service should have been passed an access type of '<accessType>'
    Then the service should have been passed a tenant id of '<tenantId>'

    Examples: 
    | claimPermissionsId | resourceUri | accessType | tenantId |
    | cid1               | r/1         | GET        | t1       |
    | cid2               | r/2         | PUT        | t2       |
   

Scenario: Evaluation grants permission 
    When I have passed a claim permissions id of 'c1', a resource URI of 'r1', an access type of 'GET', and a tenant id of 't1'
    And the service returns an OK result of 'Deny'
    Then the response wrapper should have a status code of 200
    And the response body should contain a single permissions batch response item containing 'deny'

Scenario: Evaluation denies permission 
    When I have passed a claim permissions id of 'c1', a resource URI of 'r1', an access type of 'GET', and a tenant id of 't1'
    And the service returns an OK result of 'Allow'
    Then the response wrapper should have a status code of 200
    And the response body should contain a single permissions batch response item containing 'allow'

Scenario: Evaluation does not find claim
    When I have passed a claim permissions id of 'c1', a resource URI of 'r1', an access type of 'GET', and a tenant id of 't1'
    And the service returns a Not Found result
    Then the response wrapper should have a status code of 404
    And the response should not have a body
