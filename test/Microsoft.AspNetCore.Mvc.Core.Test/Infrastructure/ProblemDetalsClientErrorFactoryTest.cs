﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Infrastructure
{
    public class ProblemDetalsClientErrorFactoryTest
    {
        [Fact]
        public void GetClientError_ReturnsProblemDetails_IfNoMappingWasFound()
        {
            // Arrange
            var clientError = new UnsupportedMediaTypeResult();
            var factory = new ProblemDetailsClientErrorFactory(Options.Create(new ApiBehaviorOptions
            {
                ClientErrorMapping =
                {
                    [405] = new ClientErrorData { Link = "Some link", Summary = "Summary" },
                },
            }));

            // Act
            var result = factory.GetClientError(new ActionContext(), clientError);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(new[] { "application/problem+json", "application/problem+xml" }, objectResult.ContentTypes);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(415, problemDetails.Status);
            Assert.Equal("about:blank", problemDetails.Type);
            Assert.Null(problemDetails.Title);
            Assert.Null(problemDetails.Detail);
            Assert.Null(problemDetails.Instance);
        }

        [Fact]
        public void GetClientError_ReturnsProblemDetails()
        {
            // Arrange
            var clientError = new UnsupportedMediaTypeResult();
            var factory = new ProblemDetailsClientErrorFactory(Options.Create(new ApiBehaviorOptions
            {
                ClientErrorMapping =
                {
                    [415] = new ClientErrorData { Link = "Some link", Summary = "Summary" },
                },
            }));

            // Act
            var result = factory.GetClientError(new ActionContext(), clientError);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(new[] { "application/problem+json", "application/problem+xml" }, objectResult.ContentTypes);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(415, problemDetails.Status);
            Assert.Equal("Some link", problemDetails.Type);
            Assert.Equal("Summary", problemDetails.Title);
            Assert.Null(problemDetails.Detail);
            Assert.Null(problemDetails.Instance);
        }
    }
}
