Feature: ResourceAccessRule

Scenario: Resource access rules with identical properties are equal
	Given two resource access rules have identical properties
	When the resource access rules are compared
	Then the resource access rule comparison result should be true

Scenario: Resource access rules with differing access types are not equal
	Given two resource access rules have differing access types
	When the resource access rules are compared
	Then the resource access rule comparison result should be false

Scenario: Resource access rules with differing resources are not equal
	Given two resource access rules have differing resources
	When the resource access rules are compared
	Then the resource access rule comparison result should be false

Scenario: Resource access rules with differing permissions are not equal
	Given two resource access rules have differing permissions
	When the resource access rules are compared
	Then the resource access rule comparison result should be false

Scenario Outline: Resource access rule matching without patterns
	Given I have a resource access rule for a resource with name '<resourceName>' and display name '<resourceDisplayName>', with an access type '<accessType>', and permission '<permission>'
	When I check if the resource access rule is a match for a target with resource name '<targetResourceName>' and target '<targetAccessType>'
	Then the match result should be '<result>'

	Examples: 
	| resourceName | resourceDisplayName | accessType | permission | targetResourceName | targetAccessType | result |
	| foo          | Foo                 | GET        | Allow      | foo                | GET              | true   |
	| FOO          | Foo                 | get        | Allow      | foo                | GET              | true   |
	| foo          | Foo                 | GET        | Allow      | FOO                | get              | true   |
	| foo          | Foo                 | GET        | Deny       | foo                | GET              | true   |
	| foo          | Foo                 | GET        | Allow      | bar                | GET              | false  |
	| foo          | Foo                 | GET        | Allow      | foo                | PUT              | false  |
	| foo          | Foo                 | GET        | Allow      | foo/bar            | PUT              | false  |
	| foo          | Foo                 | GET        | Allow      | foo/bar            | GET/a            | false  |
	| foo/bar      | Foo                 | GET        | Allow      | foo                | GET              | false  |
	| foo          | Foo                 | GET/a      | Allow      | foo                | GET              | false  |
	| pages/home   | Home page           | Edit       | Allow      | pages/home         | Edit             | true   |
	| pages/home   | Home page           | Read       | Allow      | pages/home         | Edit             | false  |
	| pages/home   | Home page           | Read       | Allow      | pages/admin        | Read             | false  |


Scenario Outline: Resource access rule matching with access type patterns
	Given I have a resource access rule for a resource with name '<resourceName>' and display name '<resourceDisplayName>', with an access type '<accessType>', and permission '<permission>'
	When I check if the resource access rule is a match for a target with resource name '<targetResourceName>' and target '<targetAccessType>'
	Then the match result should be '<result>'

	Examples: 
	| resourceName | resourceDisplayName | accessType | permission | targetResourceName | targetAccessType | result |
	| foo          | Foo                 | *          | Allow      | foo                | GET              | true   |
	| foo          | Foo                 | *          | Allow      | foo                | GET/a            | false  |
	| foo          | Foo                 | **         | Allow      | foo                | GET/a            | true   |
	| foo          | Foo                 | GET/*      | Allow      | foo                | GET              | false  |
	| foo          | Foo                 | GET/*      | Allow      | foo                | GET/a            | true   |
	| foo          | Foo                 | GET/*      | Allow      | foo                | GET/a/b          | false  |
	| foo          | Foo                 | GET/**     | Allow      | foo                | GET/a/b          | true   |
	| books/**     | All books           | Read       | Allow      | books/1984         | Read             | true   |
	| books/**     | All books           | Read       | Allow      | books/1984         | Update           | false  |
	| books/**     | All books           | **         | Allow      | books/1984         | Update           | true   |


Scenario Outline: Resource access rule matching with resource name patterns
	Given I have a resource access rule for a resource with name '<resourceName>' and display name '<resourceDisplayName>', with an access type '<accessType>', and permission '<permission>'
	When I check if the resource access rule is a match for a target with resource name '<targetResourceName>' and target '<targetAccessType>'
	Then the match result should be '<result>'

	Examples: 
	| resourceName                                                | resourceDisplayName | accessType | permission | targetResourceName                                                   | targetAccessType | result |
	| *                                                           | All                 | GET        | Allow      | foo                                                                  | GET              | true   |
	| *                                                           | All                 | GET        | Allow      | foo/bar                                                              | GET              | false  |
	| **                                                          | All                 | GET        | Allow      | foo/bar                                                              | GET              | true   |
	| foo/*                                                       | All                 | GET        | Allow      | foo                                                                  | GET              | false  |
	| foo/*                                                       | All                 | GET        | Allow      | foo/bar                                                              | GET              | true   |
	| foo/*                                                       | All                 | GET        | Allow      | foo/bar/baz                                                          | GET              | false  |
	| foo/**                                                      | All                 | GET        | Allow      | foo/bar/baz                                                          | GET              | true   |
	| foo/*/quux                                                  | All                 | GET        | Allow      | foo/bar/quux                                                         | GET              | true   |
	| foo/*/quux                                                  | All                 | GET        | Allow      | foo/123/quux                                                         | GET              | true   |
	| foo/*/quux                                                  | All                 | GET        | Allow      | foo/quux                                                             | GET              | false  |
	| foo/*/quux                                                  | All                 | GET        | Allow      | foo/123/baz                                                          | GET              | false  |
	| aul/api/cases/*/quotes/*/underwriting                       | All                 | GET        | Allow      | foo/123/baz                                                          | GET              | false  |
	| aul/api/cases/*/quotes/*/underwriting                       | All                 | GET        | Allow      | aul/api/cases/1/quotes/2/underwriting                                | GET              | true   |
	| aul/api/cases/*/quotes/*/binding-quote-*/actuarial-review/* | All                 | GET        | Allow      | aul/api/cases/1/quotes/2/underwriting                                | GET              | false  |
	| aul/api/cases/*/quotes/*/binding-quote-*/actuarial-review/* | All                 | GET        | Allow      | aul/api/cases/1/quotes/2/binding-quote-v1/actuarial-review/something | GET              | true   |
