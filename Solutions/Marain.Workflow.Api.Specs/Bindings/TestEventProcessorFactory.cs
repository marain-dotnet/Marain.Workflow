// <copyright file="TestEventProcessorFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.Specs.Bindings
{
    using Microsoft.Azure.EventHubs.Processor;
    using TechTalk.SpecFlow;

    /// <summary>
    /// <see cref="IEventProcessorFactory"/> implementation for the <see cref="TestEventProcessor"/>.
    /// </summary>
    public class TestEventProcessorFactory : IEventProcessorFactory
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEventProcessorFactory"/> class.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <param name="scenarioContext">The current <see cref="ScenarioContext"/>.</param>
        public TestEventProcessorFactory(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        /// <inheritdoc/>
        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new TestEventProcessor(this.featureContext, this.scenarioContext);
        }
    }
}
