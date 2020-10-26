@perFeatureContainer
@useTransientTenant
@useClaimsApi
Feature: OpenAPI

Scenario: Get OpenAPI definition
	When I make a request to get the OpenAPI definition
	Then the request should succeed