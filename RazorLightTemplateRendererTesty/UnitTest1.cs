using RazorLight;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RazorLightTemplateRendererTesty
{
    public class UnitTest1
    {

        [Fact]
        public async Task Test1Async()
        {
            //arrange
            var engine = new RazorLightEngineBuilder()
                // required to have a default RazorLightProject type,
                // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(UnitTest1))
                 .SetOperatingAssembly(typeof(UnitTest1).Assembly)
                .UseMemoryCachingProvider()
                .Build();

            var model = new TestModel();
            var template = @"
    @model RazorLightTemplateRendererTesty.TestModel;
    <select>
        @foreach(string option in this.Model.Options)
        {
            <option value=""@option"">@option</option>
        }
    </select>
";

            //act
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(template));
            string hash = Encoding.UTF8.GetString(await MD5.Create().ComputeHashAsync(stream));
            var rendered = await engine.CompileRenderStringAsync(hash, template, model);

            //assert
            Assert.False(string.IsNullOrWhiteSpace(rendered));
        }
    }
}