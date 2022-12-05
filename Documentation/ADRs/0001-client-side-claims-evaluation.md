# Implementation of client-side Claims Evaluation

## Status

Agreed

## Background

Since the initial implementation of the current incarnation of the Marain Claims API, we have had issues with performance. A common use case we encounter when building hypermedia APIs requires evaluating two sets of claims per API request:
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
- Clients could take advantage of the new features with minimal changes to  existing code.

#### Disadvantages
- Would remain tied to the existing interface.
- Various methods on the existing interface are irrelevent to the client side evaluation functionality. However, the fact the our new functionality shares an interface with methods to add, update and delete the underlying data would likely mean there was an expectation that any caching functionality we implement would handle changes to the underlying data, increasing the complexity of the implementation.

### 3. Implement as an independent feature built on top of the existing `IClaimsService`

In this option we'd define a new interface for our client side evaluation library, and implement it independently of the existing interfaces. We would still depend on the existing `IClaimsService` implementation to communicate with the API.

### Advantages
- Not constrained by existing `IClaimsService` interface

### Disadvantages
- Clients would need to make code changes to opt into the new functionality

## Decision

Option 3 will be implemented.

As part of this, we will implement a new `IResourceAccessEvaluator` which uses our new local-evaluation client. We'll provide additional extension methods in `OpenApiClaimsServiceCollectionExtensions` and `OpenApiAccessControlServiceCollectionExtensions` to allow configuring Menes and Marain Claims integration with the new features, meaning that many use cases will only need to change a single line of code to take advantage of the new functionality.

## Implementation notes

This section records decisions that were made during implementation.

### Tokenization of URI and AccessTypes

Question: Should we tokenize all of the URIs and AccessTypes in `ResourceAccessRules` at the point we retrieve a set of `ClaimPermissions` from the API, or do we allow the globbing library to tokenize on each evaluation.

Tokenization is part of the glob matching process, but we have the choice of doing this up front and storing the tokenization data alongside the globs, or allowing the globbing library to tokenize globs during the evaluation.

Both approaches were coded and benchmarked with the following results. Note: in the following tables, "WithTokenization" means using the code paths where URI and AccessMethod globs are tokenized up front.

Firstly, the impact on processing the retrieved `ClaimPermissions` document. As expected, tokenization up front takes longer and results in more memory allocation. Note that no attempt was made at this point to optimize memory allocation.

|                                     Method |      Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------------------------------------- |----------:|----------:|----------:|-------:|-------:|----------:|
|    ProcessClaimPermissionsWithTokenization | 11.841 us | 0.2002 us | 0.1872 us | 1.3580 | 0.0458 |  22.33 KB |
| ProcessClaimPermissionsWithoutTokenization |  5.181 us | 0.0798 us | 0.0747 us | 0.5951 | 0.0076 |   9.75 KB |

Secondly, the impact on claim evaluation. Again, as expected, evaluation is faster when the globs have already been tokenized.

|                                           Method |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------------------------------- |---------:|----------:|----------:|-------:|----------:|
|    EvaluateSingleClaimPermissionWithTokenization | 2.195 us | 0.0221 us | 0.0207 us | 0.0153 |     312 B |
| EvaluateSingleClaimPermissionWithoutTokenization | 4.711 us | 0.0461 us | 0.0432 us | 0.0153 |     312 B |

Based on these results, we decided to stick with up-front tokenization.

### Caching processed `ResourceAccessRuleset`s for reuse

Question: Since `ResourceAccessRuleset`s are often reused in multiple `ClaimPermissions`, should we keep a cache of the processed rulesets from each `ClaimPermissions` and reuse these for any future `ClaimPermissions` that reference the same rulesets?

This essentially comes down to determining whether the cost of attempting to look up every ruleset in the cache before processing it is greater than the cost of processing/tokenizing all of the rules in the `ResourceAccessRuleset` again.

The following benchmarks show the impact of this caching when processing the same `ClaimPermissions` twice - meaning that for the second `ClaimPermissions`, all of the `ResourceAccessRuleset`s will be in the cache.

|                                               Method |     Mean |    Error |   StdDev | Allocated |
|----------------------------------------------------- |---------:|---------:|---------:|----------:|
|   ProcessSingleClaimPermissionsWithoutRulesetCaching | 29.83 us | 0.590 us | 0.808 us |  23.05 KB |
| ProcessMultipleClaimPermissionsWithoutRulesetCaching | 49.75 us | 0.993 us | 1.104 us |  45.39 KB |
|      ProcessSingleClaimPermissionsWithRulesetCaching | 41.74 us | 0.812 us | 1.056 us |  38.64 KB |
|    ProcessMultipleClaimPermissionsWithRulesetCaching | 53.28 us | 1.012 us | 1.125 us |   46.5 KB |

In this case we can see that loading the second `ClaimPermissions` takes around 20 us without caching, and 12 us with caching. However, we pay a penalty of around 10us to load the first `ClaimPermissions` with caching enabled, which is due to the overhead in looking up each `ResourceAccessRuleset` in the cache.

This benchmark shows the same statistics but with the second `ClaimPermissions` being different from the first, with around 50% of the rules overlapping.

|                                               Method |     Mean |    Error |   StdDev | Allocated |
|----------------------------------------------------- |---------:|---------:|---------:|----------:|
|   ProcessSingleClaimPermissionsWithoutRulesetCaching | 28.75 us | 0.571 us | 0.906 us |  23.05 KB |
| ProcessMultipleClaimPermissionsWithoutRulesetCaching | 44.97 us | 0.871 us | 1.003 us |  41.02 KB |
|      ProcessSingleClaimPermissionsWithRulesetCaching | 40.46 us | 0.806 us | 0.960 us |  37.66 KB |
|    ProcessMultipleClaimPermissionsWithRulesetCaching | 50.71 us | 0.990 us | 1.252 us |  44.94 KB |

In this case, we see the same ~10us penalty when loading the first `ClaimPermissions` with caching enabled. Loading the second takes around 16us without caching and about 10 us with caching.

However, there is an additional complexity that caching `ResourceAccessRuleset`s will introduce. `ResourceAccessRuleset`s are entities that exist independently of the `ClaimPermissions` they are used by. The API denormalizes them into the `ClaimPermissions` document to save the need for making separate requests for the rulesets. However, this means that when multiple `ClaimPermissions` that include the same `ResourceAccessRuleset` are retrieved, it would be possible for the `ResourceAccessRuleset`s to be different versions of the same ruleset.

In order to avoid dealing with this complexity (at least for the first iteration of this feature), we will accept the relatively minor impact in processing time and not cache the rulesets.

### Choice of cache

Question: Should we use the .NET standard `MemoryCache` to store `ClaimPermission` documents, or implement our own using a `ConcurrentDictionary` (or similar).

|                                                    Method |     Mean |     Error |    StdDev |   Gen0 | Allocated |
|---------------------------------------------------------- |---------:|----------:|----------:|-------:|----------:|
|                             EvaluateSingleClaimPermission | 2.174 us | 0.0135 us | 0.0126 us | 0.0153 |     312 B |
|              EvaluateSingleClaimPermissionWithMemoryCache | 2.178 us | 0.0150 us | 0.0141 us | 0.0153 |     312 B |

Since the different in performance is negligible, the standard MemoryCache will be used.

### Best approach to evaluating a batch of claims

Question: Should we evaluate batches of claims in series or parallel? Given we know glob evaluation is highly performant, it may be that the overhead of parallelisation costs more than the evaluation itself.

For a small (~35) batch of evaluations:

|                                     Method |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------------------------- |----------:|----------:|----------:|-------:|----------:|
|   EvaluateMultipleClaimPermissionsInSeries | 47.415 us | 0.3260 us | 0.3049 us | 0.8545 |   15120 B |
| EvaluateMultipleClaimPermissionsInParallel | 49.658 us | 0.2860 us | 0.2675 us | 0.9766 |   16832 B |

For a "realistically large" batch of 280 evaluations:

|                                     Method |       Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------------------------------- |-----------:|----------:|----------:|-------:|----------:|
|   EvaluateMultipleClaimPermissionsInSeries | 382.955 us | 1.9772 us | 1.7527 us | 6.8359 |  120960 B |
| EvaluateMultipleClaimPermissionsInParallel | 386.091 us | 2.4062 us | 2.1331 us | 7.8125 |  132112 B |

And for an "unrealistically large" batch of  7000 evaluations:

|                                     Method |         Mean |      Error |     StdDev |     Gen0 |    Gen1 | Allocated |
|------------------------------------------- |-------------:|-----------:|-----------:|---------:|--------:|----------:|
|   EvaluateMultipleClaimPermissionsInSeries | 9,575.074 us | 65.3881 us | 57.9649 us | 171.8750 |       - | 3024010 B |
| EvaluateMultipleClaimPermissionsInParallel | 9,785.352 us | 86.6832 us | 81.0835 us | 187.5000 | 31.2500 | 3218618 B |

We can see from this that parallel execution does not provide any benefits and actually adds to the evaluation time for the batch.
