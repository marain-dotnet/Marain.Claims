@setupTenantedCloudBlobContainer
@setupContainer

Feature: ClaimPermissionsStore

Background:
	Given I have a list of resource access rules called "rules-1"
	| AccessType | Resource              | Permission |
	| GET        | marain/test/resource1 | Allow      |
	Given I have a list of resource access rules called "rules-2"
	| AccessType | Resource              | Permission |
	| GET        | marain/test/resource2 | Allow      |
	And I have resource access rulesets called "rulesets"
	| Id         | DisplayName | Rules     |
	| rulesets-1 | Ruleset 1   | {rules-1} |
	| rulesets-2 | Ruleset 2   | {rules-2} |
	And I have claim permissions called "claimpermissions"
	| Id                 | ResourceAccessRules | ResourceAccessRulesets |
	| claimpermissions-1 |                     | {rulesets}             |
	And I have saved the resource access rulesets called "rulesets" to the resource access ruleset store
	And I have saved the claim permissions called "claimpermissions" to the claim permissions store

@useChildObjects
Scenario: Retrieving claim permissions from the repository
	When I request the claim permission with Id "claimpermissions-1" from the claim permissions store
	Then the claim permission is returned
	And the resource access rulesets on the claim permission match the rulesets "rulesets"

@useChildObjects
Scenario: Retrieving claim permissions with an invalid Id
	When I request the claim permission with Id "incorrectid" from the claim permissions store
	Then a "ClaimPermissionsNotFoundException" exception is thrown

@useChildObjects
Scenario: Retrieving claim permissions when one or more of the referenced rule sets are missing
	Given I have resource access rulesets called "rulesets-unsaved"
	| Id         | DisplayName | Rules     |
	| rulesets-3 | Ruleset 3   | {rules-1} |
	| rulesets-4 | Ruleset 4   | {rules-2} |
	And I have claim permissions called "claimpermissions-2"
	| Id                 | ResourceAccessRules | ResourceAccessRulesets |
	| claimpermissions-2 |                     | {rulesets-unsaved}     |
	And I have saved the claim permissions called "claimpermissions-2" to the claim permissions store
	When I request the claim permission with Id "claimpermissions-2" from the claim permissions store
	Then a "ResourceAccessRuleSetNotFoundException" exception is thrown
	