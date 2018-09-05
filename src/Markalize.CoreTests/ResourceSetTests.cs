
namespace Markalize.CoreTests
{
    using Markalize.Core;
    using Should;
    using System;
    using Xunit;

    public class ResourceSetTests
    {
        [Fact]
        public void EmptyCtor()
        {
            var set = new ResourceSet();
        }

        [Fact]
        public void LoadFromAssembly_Source1()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");
            set.Keys.Length.ShouldEqual(2);
        }

        [Fact]
        public void LoadFromAssembly_LocalizerFrFr()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer("fr-FR");
            var value = localizer.Localize("Key1");
            value.ShouldEqual("Value1 fr-FR");
        }
    }
}
