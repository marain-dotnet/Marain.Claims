Feature: Resource

Scenario: Resources with identical properties are equal
	Given two resources have identical properties
	When the resources are compared
	Then the resource comparison result should be true

Scenario: Resources with differing names are not equal
	Given two resources have differing names
	When the resources are compared
	Then the resource comparison result should be false

Scenario: Resources differing only by display names are not equal
	Given two resources have differing display names
	When the resources are compared
	Then the resource comparison result should be true
