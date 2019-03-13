
namespace Markalize.CoreTests
{
    using Markalize.Core;
    using Should;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Xunit;

    public class ParserTests
    {
        [Fact]
        public void ReadSimpleKeyWithEqual()
        {
            var reader = new StringReader("Hello =      Welcome on our website.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website.");
        }

        [Fact]
        public void ReadSimpleKeyWithoutEqual()
        {
            var reader = new StringReader("Hello Welcome on our website.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website.");
        }

        [Fact]
        public void ReadBackslashedLine_Base()
        {
            var reader = new StringReader("Hello =      Welcome on our website.\\\r\nAnd have fun.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website.And have fun.");
        }

        [Fact]
        public void ReadBackslashedLine_WhiteAtEndOfLine()
        {
            var reader = new StringReader("Hello =      Welcome on our website. \\\r\nAnd have fun.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website. And have fun.");
        }

        [Fact]
        public void ReadBackslashedLine_WhiteAtStartOfLine()
        {
            var reader = new StringReader("Hello =      Welcome on our website.\\\r\n    And have fun.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website.And have fun.");
        }

        [Fact]
        public void ReadBackslashedLine_OneMoreLine()
        {
            var reader = new StringReader("Hello =      Welcome on our website.\\\r\nAnd have fun.\\\nPS: I'm me.");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our website.And have fun.PS: I'm me.");
        }

        [Fact]
        public void Titles_Base()
        {
            var reader = new StringReader("T1_\n===\n\nV1 Vv\n\nS1_\n---\n\nV2 Vvv\n\n---\n\nV3 Vvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T1_S1_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            resource.Keys.Length.ShouldEqual(3);
        }

        [Fact]
        public void Titles_TwoTitle1()
        {
            var reader = new StringReader("T1_\n===\n\nV1 Vv\n\nT2_\n===\n\nV2 Vvv\n\n---\n\nV3 Vvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T2_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            resource.Keys.Length.ShouldEqual(3);
        }

        [Fact]
        public void Titles_TwoTitle2()
        {
            var reader = new StringReader("T1_\n---\n\nV1 Vv\n\nT2_\n---\n\nV2 Vvv\n\n---\n\nV3 Vvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T2_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            resource.Keys.Length.ShouldEqual(3);
        }

        [Fact]
        public void Titles_Title2_Title1_Title2()
        {

            var reader = new StringReader("T1_\n---\n\nV1 Vv\n\nT2_\n===\n\nV2 Vvv\n\nT3_\n---\n\nV3 Vvvv\n\n---\n\nV4 Vvvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T2_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["T2_T3_V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            entity = resource["V4"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvvv");

            resource.Keys.Length.ShouldEqual(4);
        }

        [Fact]
        public void Titles_WithPrefix_Base()
        {
            var reader = new StringReader("Title 1 [T1_]\n===\n\nV1 Vv\n\nSubtitle 1[S1_]\n---\n\nV2 Vvv\n\n---\n\nV3 Vvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T1_S1_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            resource.Keys.Length.ShouldEqual(3);
        }

        [Fact]
        public void Titles_WithPrefix_TwoTitle1()
        {
            var reader = new StringReader("Title 1 [T1_]\n===\n\nV1 Vv\n\nTitle 2[T2_]\n===\n\nV2 Vvv\n\n---\n\nV3 Vvvv");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            Entity entity;

            entity = resource["T1_V1"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vv");

            entity = resource["T2_V2"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvv");

            entity = resource["V3"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Vvvv");

            resource.Keys.Length.ShouldEqual(3);
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote()
        {
            var reader = new StringReader("Hello \"Welcome\non our website. \"");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome\non our website. ");
        }

        [Fact]
        public void ReadMultiline_DoubleDoubleQuote()
        {
            var reader = new StringReader("Hello \"\"Welcome\non our \"website\". \"\"");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome\non our \"website\". ");
        }

        [Fact]
        public void ReadMultiline_TripleDoubleQuote()
        {
            var reader = new StringReader("Hello \"\"\"Welcome\non our \"\"website\"\". \"\"\"");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome\non our \"\"website\"\". ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_Trims()
        {
            var reader = new StringReader("Hello  \"Welcome\n   on our website. \" ");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome\non our website. ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_NoTrims()
        {
            var reader = new StringReader("Hello \" Welcome  \non our website.  \"");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual(" Welcome  \non our website.  ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_DontTrimThisSpace1()
        {
            var reader = new StringReader("Hello \"Welcome on our \nwebsite. \"");
            // don't trim this please:                           ^
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our \nwebsite. ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_DontTrimThisSpace2()
        {
            var reader = new StringReader("Hello \"Welcome \non our \nwebsite. \"");
            // don't trim this please:                             ^
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome \non our \nwebsite. ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_BackslashOnFirstLine()
        {
            var reader = new StringReader("Hello \"Welcome \\\non our \nwebsite. \"");
            // don't trim this please:                               ^
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome on our \nwebsite. ");
        }

        [Fact]
        public void ReadMultiline_SingleDoubleQuote_BackslashOnSecond()
        {
            var reader = new StringReader("Hello \"Welcome\non our \\\n  website. \"");
            var resource = new ResourceFile();
            var target = new Parser();
            target.Parse(reader, resource);
            var entity = resource["Hello"];
            entity.ShouldNotBeNull();
            entity.Value.ShouldEqual("Welcome\non our website. ");
        }














    }
}
