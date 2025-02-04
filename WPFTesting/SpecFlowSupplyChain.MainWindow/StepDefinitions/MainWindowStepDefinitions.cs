using WPFTesting.Shapes;
using SpecFlowSupplyChain.MainWindow.Test_Classes;
using YourNamespace;
using System.Windows;
using NUnit.Framework;

namespace SpecFlowSupplyChain.MainWindowTest.StepDefinitions
{
    [Binding]
    public sealed class MainWindowStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;
        public MainWindowStepDefinitions(ScenarioContext sc)
        {
            _scenarioContext = sc;
        }

        [Given("a standard box is generated")]
        public void GivenAStandardBoxIsGenerated()
        {
            TestMainWindow testWindow = new TestMainWindow();
            TestDraggableBox db = new TestDraggableBox();
            db.Name = "goodbox";

            _scenarioContext.Add("Window", testWindow);
            _scenarioContext.Add("Box", db);
        }

        [Given("a box with height (.*) and width (.*) is generated")]
        public void WhenABoxWithWidthHeightIsGenerated(double height, double width)
        {
            TestMainWindow testWindow = new TestMainWindow();
            TestDraggableBox db = new TestDraggableBox();
            db.Name = "bigbox";
            db.Height = height;
            db.Width = width;

            _scenarioContext.Add("Window", testWindow);
            _scenarioContext.Add("Box", db);
        }

        [Given("the box's (.*) button is clicked")]
        public void WhenTheBoxCornerIsClicked(string corner)
        {
            var db = _scenarioContext.Get<TestDraggableBox>("Box");
            _scenarioContext.Remove("Box");
            db.CornerClicked = corner;
            _scenarioContext.Add("Box", db);
        }

        [When("the point for the line is computed")]
        public void WhenThePointForTheLineIsComputed()
        {
            var testWindow = _scenarioContext.Get<TestMainWindow>("Window");
            var db = _scenarioContext.Get<TestDraggableBox>("Box");

            Point actualpoint = testWindow.GetLineOffset(db);

            _scenarioContext.Add("actualpoint", actualpoint);
        }

        [Then("the line's Point should be (.*), (.*)")]
        public void ThenTheResultShouldBe(double x, double y)
        {
            Point expected = new Point(x, y);
            var actual = _scenarioContext.Get<Point>("actualpoint");

            actual.Should().Be(expected);
        }
    }
}
