@useTransientTenant
@perFeatureContainer

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
	And I have resource access rulesets called "rulesets-single"
	| Id         | DisplayName | Rules     |
	| rulesets-1 | Ruleset 1   | {rules-1} |
	And I have claim permissions called "claimpermissions"
	| Id                 | ResourceAccessRules | ResourceAccessRulesets |
	| claimpermissions-1 |                     | {rulesets}             |
	| claimpermissions-2 |                     | {rulesets-single}      |
	And I have saved the resource access rulesets called "rulesets" to the resource access ruleset store
	And I have created the claim permissions called "claimpermissions" in the claim permissions store

@useChildObjects
Scenario: Retrieving claim permissions from the repository
	When I request the claim permission with Id "claimpermissions-1" from the claim permissions store
	Then the claim permission is returned
	And the resource access rulesets on the claim permission match the rulesets "rulesets"

@useChildObjects
Scenario: Retrieving a batch of claim permissions from the repository
	When I request a batch of claim permissions by Id from the claim permissions store
	| ClaimPermissionsId |
	| claimpermissions-1 |
	| claimpermissions-2 |
	Then the claim permissions are returned
	And the resource access rulesets on the claim permissions match the expected rulesets
	| ClaimPermissionsId | ExpectedRulesets |
	| claimpermissions-1 | rulesets         |
	| claimpermissions-2 | rulesets-single  |

@useChildObjects
Scenario: Retrieving a batch of claim permissions from the repository with duplicate claim permission Ids automatically deduplicates the requests
	When I request a batch of claim permissions by Id from the claim permissions store
	| ClaimPermissionsId |
	| claimpermissions-1 |
	| claimpermissions-1 |
	Then the claim permissions are returned
	And the resource access rulesets on the claim permissions match the expected rulesets
	| ClaimPermissionsId | ExpectedRulesets |
	| claimpermissions-1 | rulesets         |

@useChildObjects
Scenario: Retrieving claim permissions with an invalid Id
	And an id exists named "incorrectid" but there is no claims permission associated with it
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
	And I have created the claim permissions called "claimpermissions-2" in the claim permissions store
	When I request the claim permission with Id "claimpermissions-2" from the claim permissions store
	Then a "ResourceAccessRuleSetNotFoundException" exception is thrown
	