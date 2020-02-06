// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Marain.Workflows.Api.Specs.EndToEnd
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("StartNewInstance")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    public partial class StartNewInstanceFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "StartNewInstance.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "StartNewInstance", "\tIn order manage a new thing through a workflow\r\n\tAs an external user of the work" +
                    "flow engine\r\n\tI want to be able to start a new instance of a workflow", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Start a new instance with a specified instance id")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void StartANewInstanceWithASpecifiedInstanceId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Start a new instance with a specified instance id", null, new string[] {
                        "useChildObjects"});
#line 8
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 9
 testRunner.Given("I start a functions instance for the local project \'Endjin.Operations.Functions.O" +
                    "perationsControlHost\' on port 7078", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 10
 testRunner.And("I start a functions instance for the local project \'Endjin.Workflow.Functions.Mes" +
                    "sageIngestionHost\' on port 7071", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.And("I start a functions instance for the local project \'Endjin.Workflow.Functions.Eng" +
                    "ineHost\' on port 7075", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
 testRunner.And("I start a functions instance for the local project \'Endjin.Workflow.Functions.Mes" +
                    "sagePreProcessingHost\' on port 7073", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.And("I have added the workflow \"SimpleExpensesWorkflow\" to the workflow store with Id " +
                    "\"simple-expenses-workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 14
 testRunner.And("I have cleared down the workflow instance store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table3.AddRow(new string[] {
                        "Claimant",
                        "J George"});
            table3.AddRow(new string[] {
                        "CostCenter",
                        "GD3724"});
#line 15
 testRunner.And("I have a dictionary called \"context\"", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId",
                        "WorkflowInstanceId",
                        "Context"});
            table4.AddRow(new string[] {
                        "simple-expenses-workflow",
                        "instance",
                        "{context}"});
#line 19
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"request\"", ((string)(null)), table4, "And ");
#line 22
 testRunner.When("I post the object called \"request\" to the endpoint \"http://localhost:7071/startne" +
                    "wworkflowinstancerequests\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
 testRunner.Then("I should have received a 202 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 24
 testRunner.And("there should be a workflow instance with the id \"instance\" in the workflow instan" +
                    "ce store within 300 seconds", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 25
 testRunner.And("the workflow instance with id \"instance\" should be an instance of the workflow wi" +
                    "th id \"simple-expenses-workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 26
 testRunner.And("the workflow instance with id \"instance\" should have a context dictionary that ma" +
                    "tches \"context\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
