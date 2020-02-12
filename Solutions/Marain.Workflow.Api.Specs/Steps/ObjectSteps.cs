// <copyright file="ObjectSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class ObjectSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext context;

        public ObjectSteps(FeatureContext featureContext, ScenarioContext context)
        {
            this.context = context;
            this.featureContext = featureContext;
        }

        [Given("I have an object of type '(.*)' called '(.*)'")]
        [Given("I have objects of type '(.*)' called '(.*)'")]
        public void GivenIHaveAnObjectOfTypeCalled(string contentType, string instanceName, Table table)
        {
            object[] instances = table.CreateSet(() => ContainerBindings.GetServiceProvider(this.featureContext).GetContent(contentType)).ToArray();
            this.context[instanceName] = instances;
        }

        [Given("I have a dictionary called '(.*)'")]
        public void GivenIHaveADictionaryCalled(string instanceName, Table table)
        {
            var dict = table.Rows.ToDictionary(x => x["Key"], x => x["Value"]);
            this.context[instanceName] = dict;
        }
    }
}
