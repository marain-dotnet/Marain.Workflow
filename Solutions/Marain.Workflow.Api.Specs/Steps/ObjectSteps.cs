// <copyright file="ObjectSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions.Specs.Steps
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

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

        [Given(@"I have an object of type ""(.*)"" called ""(.*)""")]
        [Given(@"I have objects of type ""(.*)"" called ""(.*)""")]
        public void GivenIHaveAnObjectOfTypeCalled(string contentType, string instanceName, Table table)
        {
            var instances = table.CreateSet(() => ContainerBindings.GetServiceProvider(this.featureContext).GetContent(contentType)).ToArray();
            this.context[instanceName] = instances;
        }

        [Given(@"I have a POCO object of type ""(.*)"" called ""(.*)""")]
        [Given(@"I have POCO objects of type ""(.*)"" called ""(.*)""")]
        public void GivenIHaveAPocoObjectOfTypeCalled(string typeName, string instanceName, Table table)
        {
            var type = Type.GetType(typeName);
            var obj = table.CreateSet(() => Activator.CreateInstance(type));
            this.context[instanceName] = obj;
        }

        [Given(@"I have a dictionary called ""(.*)""")]
        public void GivenIHaveADictionaryCalled(string instanceName, Table table)
        {
            var dict = table.Rows.ToDictionary(x => x["Key"], x => x["Value"]);
            this.context[instanceName] = dict;
        }
    }
}
