
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

            var expectedKeys = new string[] { "HowAreYou", "Key1", "Key2", "MultiLine2DQ2", "MultiLine3DQ1", "MultiLineBs2", "MultiLineBs3", "MultiLineDQ1", "MultiLineDQ2", "MultiLineDQ3", };

            var keys = set.Keys;
            for (int i = 0, j = 0; i < keys.Length; i++)
            {
                var current = keys[i];
                var expected = expectedKeys[j++];

                current.ShouldEqual(expected, "Expected key " + i + " to equal " + expected);
            }
        }

        [Fact]
        public void LoadFromAssembly_Source1_LocalizerFrFr()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.MakeLocalizer().ForCulture("fr-FR").GetLocalizer();
            var value = localizer.Localize("Key1");
            value.ShouldEqual("Value1 fr-FR");

            value = localizer.Localize("Key2");
            value.ShouldEqual("Valeur en français");
        }

        [Fact]
        public void LoadFromAssembly_Source1_LocalizerEnUs()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.MakeLocalizer().ForCulture("en-US").GetLocalizer();
            var value = localizer.Localize("Key1");
            value.ShouldEqual("Value1 en-US");

            value = localizer.Localize("Key2");
            value.ShouldEqual("Value in english");
        }

        [Fact]
        public void LoadFromAssembly_Source1_LocalizerDefault()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("Key1");
            value.ShouldEqual("Value1 en-US");

            value = localizer.Localize("Key2");
            value.ShouldEqual("Value in english");
        }

        [Fact]
        public void LoadFromAssembly_Source1_MultiLineBackslashed2()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLineBs2");
            value.ShouldEqual(@"This is the first line. And here is the second and last one.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_MultiLineBackslashed3()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLineBs3");
            value.ShouldEqual(@"This is the first line. And here is the second one.This is the third and last line.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_DoubleQuoted1()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLineDQ1");
            value.ShouldEqual(@"This is the first line and last one.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_3DoubleQuoted1()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLine3DQ1");
            value.ShouldEqual(@"This is the first li""ne and la""""st one.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_DoubleQuoted2()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLineDQ2");
            value.ShouldEqual("This is the first line \r\nAnd here is the second and last one.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_DoubleQuoted3()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLineDQ3");
            value.ShouldEqual("This is the first line \r\nAnd here is the second one.\r\nThis is the third and last line.");
        }

        [Fact]
        public void LoadFromAssembly_Source1_2DoubleQuoted2()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            var value = localizer.Localize("MultiLine2DQ2");
            value.ShouldEqual("This is the first line \r\nAnd here is the sec\"ond and last one.");
        }
    }
}
