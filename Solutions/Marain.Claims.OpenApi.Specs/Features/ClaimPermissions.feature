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
	And the new ClaimsPermission has id named 'id1'
	When the new ClaimsPermission is POSTed to the createClaimPermissions endpoint
	Then the result should be 120

Scenario: POST ClaimPermissions with existing ID