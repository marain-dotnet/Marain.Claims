Feature: ClaimPermissions

Scenario: Get all resource access rules with only direct resource access rules
	Given a claims permission with only direct resource access rules
	When I get all resource access rules for the claim permissions
	Then the result should contain all resource access rules that were directly assigned to the claim permissions

Scenario: Get all resource access rules with only resource access rule sets
	Given two resource access rule sets with different resource access rules
	And a claims permission with only resource access rule sets
	When I get all resource access rules for the claim permissions
	Then the result should contain all resource access rules that were in the resource access rule sets

Scenario: Get all resource access rules with only resource access rule sets, with overlapping claims
	Given two resource access rule sets with overlapping resource access rules
	And a claims permission with only resource access rule sets
	When I get all resource access rules for the claim permissions
	Then the result should contain all resource access rules that were in the resource access rule sets
	And the result should not contain any duplicate resource access rules

Scenario: Get all resource access rules with direct resource access rules and resource access rule sets
	Given two resource access rule sets with different resource access rules
	And a claims permission with resource access rule sets and direct resource access rules
	When I get all resource access rules for the claim permissions
	Then the result should contain all resource access rules that were directly assigned to the claim permissions
	And the result should contain all resource access rules that were in the resource access rule sets

Scenario: Get all resource access rules with direct resource access rules and resource access rule sets, with overlapping claims
	Given two resource access rule sets with different resource access rules
	And a claims permission with resource access rule sets and overlapping direct resource access rules
	When I get all resource access rules for the claim permissions
	Then the result should contain all resource access rules that were directly assigned to the claim permissions
	And the result should contain all resource access rules that were in the resource access rule sets
	And the result should not contain any duplicate resource access rules


