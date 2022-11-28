# Implementation of client-side Claims Evaluation

## Status

Draft

## Background

Since the initial implementation of the Marain Claims API, we have had issues with performance. A common use case we encounter when building hypermedia APIs requires evaluating two sets of claims per API request:
- On receiving a request, to ensure that the current user has permission to access the endpoint they have requested
- When returning a HAL document, to strip out any links or embedded documents that the user does not have permission to call

The first implementation of the API required one call per claim evaluation. When link-stripping from a HAL document, it is often necessary to evaluate many (in some cases, up to several hundred) claims at once, and performance with the call-per-evaluation model was predictably poor.

To partially remidate this, a batch evaluation endpoint was added to the API. This allowed clients to POST a batch of evaluation requests to the API and receive a single response containing the results of all evaluations. This has made the API more usable, but performance is still sub-optimal.

The nature of the Claims API is that the data it stores is slowly changing. As a result, a more optimal model is for the client library to retrieve `ClaimPermissions` documents in their entirety from the Claims API and to perform the claim evaluation on the client side. 

## Options

There are several options for implementing this new functionality. The following have been considered.

### 1. Implement behind the existing `IClaimsService` interface, replacing the existing AutoRest-generated library

In this option we would rewrite the existing Marain.Claims.Client library, keeping the existing `IClaimsService` interface to avoid breaking compatibility with existing consumers.

#### Advantages
- Clients could take advantage of the new features without needing to change their existing code.

#### Disadvantages
- Would remain tied to the existing interface.
- No way of knowing whether the new functionality would be a breaking change for some clients since it is very different from the existing implementation.

### 2. Implement behind the existing `IClaimsService` interface as a wrapper for the existing AutoRest-generated library

In this option, we would provide a new implementation of `IClaimsService` which consumers could opt into using. This would likely wrap the existing auto-generated `ClaimsService`.

#### Advantages
- Clients could take advantage of the new features without needing to change their existing code.

#### Disadvantages
- Would remain tied to the existing interface.
- Various methods on the existing interface are irrelevent to the client side evaluation functionality. However, the fact the our new functionality shares an interface with methods to add, update and delete the underlying data would likely mean there was an expectation that any caching functionality we implement would handle changes to the underlying data, increasing the complexity of the implementation.

### 3. Implement as an independent feature built on top of the existing `IClaimsService`

In this option we'd define a new interface for our client side evaluation library, and implement it independently of the existing interfaces. We would still likely depend on the existing `IClaimsService` implementation to communicate with the API.

### Advantages
- Not constrained by existing `IClaimsService` interface

### Disadvantages
- Clients would need to make code changes to opt into the new functionality

## Decision

Option 3 will be implemented.

As part of this, we will implement a new `IResourceAccessEvaluator` which uses our new local-evaluation client. We'll provide additional extension methods in `OpenApiClaimsServiceCollectionExtensions` and `OpenApiAccessControlServiceCollectionExtensions` to allow configuring Menes and Marain Claims integration with the new features, meaning that many use cases will only need to change a single line of code to take advantage of the new functionality.

## Consequences

