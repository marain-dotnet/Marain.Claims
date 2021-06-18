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
	And the ClaimsPermissions returned by the Claims service's id matches 'id1'

Scenario: POST ClaimPermissions with existing ID