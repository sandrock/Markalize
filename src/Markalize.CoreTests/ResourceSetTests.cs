
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

            var expectedKeys = new string[]
            {
                "HowAreYou",
                "Key1", "Key1AfterTitles", "Key2",
                "MultiLine2DQ2", "MultiLine3DQ1", "MultiLineBs2", "MultiLineBs3", "MultiLineDQ1", "MultiLineDQ2", "MultiLineDQ3",
                "SecondTitle1_Key2UnderT1", "SecondTitle1_SecondTitle2_Key2UnderT2", "SecondTitle1_ThirdTitle2_Key3UnderT2",
                "ThirdTitle1_FourthTitle2_Key4UnderT2", "ThirdTitle1_Key4UnderT1", 
                "Title1_Key1UnderT1", "Title1_Title2_Key1UnderT2",
            };

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

        [Fact]
        public void LoadFromAssembly_Source1_Titles()
        {
            var set = new ResourceSet();
            set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Source1");

            var localizer = set.GetLocalizer();
            string value;

            value = localizer.Localize("Title1_Key1UnderT1");
            value.ShouldEqual("This should should be prefixed with the titles' key.");

            value = localizer.Localize("Title1_Title2_Key1UnderT2");
            value.ShouldEqual("This should should be prefixed with both titles' key.");

            value = localizer.Localize("SecondTitle1_Key2UnderT1");
            value.ShouldEqual("This should should be prefixed with the titles' key.");

            value = localizer.Localize("SecondTitle1_SecondTitle2_Key2UnderT2");
            value.ShouldEqual("This should should be prefixed with both titles' key.");

            value = localizer.Localize("SecondTitle1_ThirdTitle2_Key3UnderT2");
            value.ShouldEqual("This should should be prefixed with both titles' key.");

            value = localizer.Localize("Key1AfterTitles");
            value.ShouldEqual("This should not be prefixed.");

            value = localizer.Localize("ThirdTitle1_Key4UnderT1");
            value.ShouldEqual("This should should be prefixed with the titles' key.");

            value = localizer.Localize("ThirdTitle1_FourthTitle2_Key4UnderT2");
            value.ShouldEqual("This should should be prefixed with both titles' key.");
        }
    }
}
