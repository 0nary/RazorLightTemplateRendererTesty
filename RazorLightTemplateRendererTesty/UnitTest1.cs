using FluentAssertions;
using RazorLight;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RazorLightTemplateRendererTesty
{

    public class TestModel
    {

        public List<string> Options { get; set; } = new() { "option1", "option2" };

    }

    public class RazorLightTemplateRendererTests
    {

        [Fact]
        public async Task RenderTemplate()
        {
            //arrange
            var engine = new RazorLightEngineBuilder()
            // required to have a default RazorLightProject type,
            // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(RazorLightTemplateRendererTests))
                 .SetOperatingAssembly(typeof(RazorLightTemplateRendererTests).Assembly)
                .UseMemoryCachingProvider()
                .Build();

            var model = new TestModel();
            var template = @"
    @model Neuroglia.UnitTests.Cases.Data.TestModel;

    <select>
        @foreach(string option in this.Model.Options)
        {
            <option value=""@option"">@option</option>
        }
    </select>
";

            //act
            //var rendered = await this.TemplateRenderer.RenderTemplateAsync(template, model);
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(template));
            string hash = Encoding.UTF8.GetString(await MD5.Create().ComputeHashAsync(stream));
            var rendered = await engine.CompileRenderStringAsync(hash, template, model);

            //assert
            rendered.Should().NotBeNullOrWhiteSpace();
            rendered.Should().Contain("<select>");
            rendered.Should().Contain($"<option value=\"{model.Options[0]}\">{model.Options[0]}</option>");
            rendered.Should().Contain($"<option value=\"{model.Options[1]}\">{model.Options[1]}</option>");
            rendered.Should().Contain("</select>");
        }

    }

}
