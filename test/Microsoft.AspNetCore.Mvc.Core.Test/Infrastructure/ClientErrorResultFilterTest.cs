﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Infrastructure
{
    public class ClientErrorResultFilterTest
    {
        private static readonly IActionResult Result = new EmptyResult();

        [Fact]
        public void OnResultExecuting_DoesNothing_IfActionIsNotClientErrorActionResult()
        {
            // Arrange
            var actionResult = new NotFoundObjectResult(new object());
            var context = GetContext(actionResult);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            Assert.Same(actionResult, context.Result);
        }

        [Fact]
        public void OnResultExecuting_TransformsClientErrors()
        {
            // Arrange
            var actionResult = new NotFoundResult();
            var context = GetContext(actionResult);
            var filter = GetFilter();

            // Act
            filter.OnResultExecuting(context);

            // Assert
            Assert.Same(Result, context.Result);
        }

        private static ClientErrorResultFilter GetFilter()
        {
            var factory = Mock.Of<IClientErrorFactory>(
                f => f.GetClientError(It.IsAny<ActionContext>(), It.IsAny<IClientErrorActionResult>()) == Result);

            return new ClientErrorResultFilter(factory, NullLogger<ClientErrorResultFilter>.Instance);
        }

        private static ResultExecutingContext GetContext(IActionResult actionResult)
        {
            return new ResultExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                Array.Empty<IFilterMetadata>(),
                actionResult,
                new object());
        }
    }
}
