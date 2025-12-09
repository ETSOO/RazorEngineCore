namespace RazorEngineCore.Tests.Models
{
    public class TestTemplate2 : RazorEngineTemplateBase<TestModel>
    {
        public void Initialize(TestModel model)
        {
            Model = model;
        }
    }
}