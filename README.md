
Markalize
===================

A better way to localize your apps in .NET.

WARNING - Project not ready for production
-------------------------------------------

I am throwing code all over the place. This is NO READY FOR PRODUCTION. Not even for you to test.

Come back later to follow the project status.


What's wrong with resource files? (resx)
-------------------------------------------

* Must* be implemented at compile-time with many dirty practices.
  * May be [loaded at run time](https://docs.microsoft.com/en-us/dotnet/framework/resources/working-with-resx-files-programmatically)
  * The sattelite assembly thing has [obscure code](https://github.com/Microsoft/msbuild/blob/e70a3159d64f9ed6ec3b60253ef863fa883a99b1/src/Tasks/CultureInfoCache.cs#L51) and [obscure issues](https://github.com/Microsoft/msbuild/issues/1454)
    * You [cannot prevent the compile time from making sattelite assemblies](https://github.com/Microsoft/msbuild/issues/3064#issuecomment-418540984)
    * And some file names [are not allowed to be used](https://github.com/aspnet/Tooling/issues/1066)
* The editor is too old
  * Keyboard shortcut often end in undesired edits
  * Some moves ends with lost keys
  * Dark theme is not supported
* Pluralization and genre are not supported
* A strange comment is autoblackmagicaly added to resx file every time you change them. 
  * This comment takes up assembly space if you have many files and assemblies in a project.
  * The comment keeps on coming back. Always.


Isn't there anything else already available?
----------------------------------------------

* pot files are not well tooled for .NET
* TODO: document more 


What can we change?
---------------------

Don't you think a simple text file can do the job? I do.

Don't you think anyone should be able to provide localization in a file that is not hard to edit? I do.

Don't you like markdown? There is support in here.

Aren't you tired of coding tricks to handle singular/plural/masculine/feminine variants? I am.


Let's dive
-------------------

Say that we are going to start with assembly-embedded files just like resx files. Don't worry, more is about to come.

### Keys and values

Localization files are key-value pairs It does not change here. Here is a markdown-compatible default resource file.

```

Hello =      Welcome on our website.
Navigation = Here you will find various links to find our stuff.
Intro        This website provides contents and ideas for developers and foxes. Note \
             that the equal sign is not required. Indentation is only decorative. You \
             can do without.

```


### Prefixes

Now say that you have many pages or many sections. You wish to prefix your keys to denote that.

```
Page1_Title First page!
Page2_Desc  It's quite boring to play with prefixes! Can we do better?

Page2_
=================

Title   Second page!
Desc    Oh yeah! The title acts as a prefix. Can't it be a bit more sexy?

Third page [Page3_]
=======================

Title  Now we've got somethin'!
Desc   What if my page has sections?

Section 1 [S1_]
-----------------

Title  Page 1 > Section 1 > Title
Desc   2 levels of title are supported. The key to access the current value is `Page3_S1_Desc`.
Desc2  And if I want to "leave the title"?

---

Horizontal bars reset the prefix stack. Note that the current line will create a key `Horizontal` that I don't need. The beauty of doing markdown is that you are quite free to mix localized values, text and formating.

```

### Lines and wraping

As you can see, each line defines a key and a value. How can we do multi-line values? Now double quotes are coming.

```
Key124 The backslash does not produce \
       a new line. It only makes the file look better.
Key125 "This value includes 
a line feed."
Key126 ""This line includes "quotes" that don't need to be escaped. ""
Key127 """"""You can use as many delimiting quotes as you wish""""""
Key128 ""The backslash is available to avoid lines\
from becoming too long. ""
```

* When using the backslash:
  * no line feed is produced
  * all lines are trimmed from white spaces (don't forget to end your line with a space)
* When using quotes:
  * a line feed is a line feed
  * lines are not trimmed from white spaces
  * using a backslash will only enhance the look


### File names (and dimensions)

You will need multiple files to handle mumtiple cultures. You will be able to handle more than culture.

If you don't know how "cultures" and localization work in .NET, see [IETF language tag](https://en.wikipedia.org/wiki/IETF_language_tag) and [CultureInfo class](https://docs.microsoft.com/en-gb/dotnet/api/system.globalization.cultureinfo?view=netframework-4.7.2).

Some languages allow two forms of the "you" pronoun, like in french. This is the [T-V distinction](https://en.wikipedia.org/wiki/T%E2%80%93V_distinction). "Tu" (thou) is familiar and is called "tutoiement". "Vous" is not and is called "vouvoiement". Say you want your users to choose between these forms...

The principle here is to think about dimensions. Cultures already have at least 2 dimensions: the language and the region. You can add more dimensions! Do you like a subculture? You can create localization variants for your users.

* Language dimension (standard): 
  * fr: french
  * en: english
  * de: german
* Region dimension (standard): 
  * FR: France
  * CA: Canada
  * US: USA
  * GB: Great Britain
  * DE: Germany
* T-V dimension (known subtelty): 
  * Vos: vouvoiement (considered default in french)
  * Tu: tutoiement
* Subculture dimension (developer created): 
  * Kaamelott: a popular french TV series
  * Star Trek: a popular sci-fi TV series

With dimensions, you can compose localizations for everyone.

* File `Website.L-en.R-US.Default.md`
  * Dimension `L` (language) is `en` (engligh)
  * Dimension `R` (region)   is `US` (USA)
  * The `Default` part signals that this is the ultimate fallback resource file. This is optional.
* File `Website.L-fr.R-FR.md`
  * Dimension `L` (language) is `fr` (french)
  * Dimension `R` (region)   is `FR` (France)
* File `Website.L-fr.R-FR.T-Tu.md`
  * Dimension `L` (language) is `fr` (french)
  * Dimension `R` (region)   is `FR` (France)
  * Dimension `T` (T-V dimension) is `Tu` (Tutoiement)
* File `Website.L-en.R-US.S-StarTrek.md`
  * Dimension `L` (language) is `en` (engligh)
  * Dimension `R` (region)   is `US` (USA)
  * Dimension `S` (TV series dimension) is `StarTrek` (refering to Star Trek)

Now your app: 

* default to english
* supports 2 forms of french
* supports a special star-trek-english with known references to this subculture

It's important to keep the 2 standard dimensions (language and region) in order to keep [formating right](https://docs.microsoft.com/en-us/globalization/locale/locale-and-culture).

So. Create those file in visual studio. Set them as "embedded resource".

```
var set = new ResourceSet();
set.LoadFromAssembly(typeof(ResourceSetTests).Assembly, "Resources/Website");
```

The `set variable will make sense of all your resource files that look like `Website.***.md`. Now, let's localize.

```
var localizer1 = set.GetLocalizer();
localizer1.Localize("Hello")    .ShouldEqual("Welcome on our website.") // defaults to english
localizer1.Localize("GoodBye")  .ShouldEqual("Good bye.")               // defaults to english

var usersCulture = new CultureInfo("fr-FR");
var localizer2 = set.GetLocalizer(usersCulture, "T-Tu");
localizer1.Localize("Hello")    .ShouldEqual("Nous te souhaitons la bienvenue.") // french tutoiement
localizer1.Localize("GoodBye")  .ShouldEqual("A bientôt !")                      // french tutoiement

var localizer3 = set.GetLocalizer("L-en", "S-StarTrek");
localizer3.Localize("Hello")    .ShouldEqual("Welcome on our website.") // defaults to english
localizer3.Localize("GoodBye")  .ShouldEqual("Live long and prosper.")  // Star Trek specific
```

For performance reasons, you should:

* Provide a finite list of supported culture to the user
  * letting the user compose its culture may lead to unexpected memory usage
  * use the inline syntax to identify cultures (L-fr.R-fr.T-Tu, L-en.R-US)
* Keep localizer objects in a static readonly field to avoid allocating too much memory


### Go runtime

What you just did with assembly-embedded files can also be done with filesystem-file or files from a database. This will allow you to create GUI for your users to localize texts. 


### Going further

See the [format specifications](FORMAT-SPEC.md).


Roadmap
--------------

### Documentation tasks

* [ ] Singulars and plurals


### Release 1

* [ ] Syntax fully working
  * [x] Simple value
  * [x] Simple value + backslash
  * [x] Quoted value
  * [ ] Quoted value + backslash
  * [ ] Quoted verbatim value
  * [ ] Quoted verbatim value + backslash
  * [ ] Title 1
  * [ ] Title 2
  * [x] Comments (fenced code blocks)
* [ ] Loading assembly   files fully working
  * [ ] Determine exact syntax
  * [ ] Loader code
  * [ ] Specialized exception type
* [ ] Loading filesystem files fully working
  * [ ] Loader code
* [ ] Loading custom     files fully working
  * [ ] Loader code
* [ ] Localizer fully working
* [ ] Inline culture ID syntax helpers
* [ ] Compatibility with CultureInfo and classic dev tasks
* [ ] Sample console app
* [ ] Great README
* [ ] Basic format specifications

### Release 2

* [ ] Sample domain library
* [ ] Sample MVC app
* [ ] Sample WPF app
* [ ] Ease load from filesystem
* [ ] Basic resx to markalize converter
* [ ] Ease caching of localizer objects
* [ ] In-document dimensions and options

### Future work

* [ ] Serialization capabilities (document object model)
  * Read file as object
  * Change object (from a GUI)
  * Write object to file (comments and titles are preserved)
* [ ] WPF GUI with translation capabilities
* [ ] Console app to auto translate a file
* [ ] Intelli-sense addon 
* [ ] Developer options to detect missing translations





