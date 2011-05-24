﻿using System;
using System.Collections.Generic;
using System.Linq;
using mesoBoard.Common;
using mesoBoard.Data;
using mesoBoard.Services;
using NUnit.Framework;
using Moq;
using mesoBoard.Data.Repositories;
using System.Linq.Expressions;

namespace mesoBoard.Tests.Services
{
    [TestFixture]
    public class ParseServicesTests
    {
        ParseServices _parseServices;
        
        IQueryable<BBCode> _bbCodes = new List<BBCode>()
        {
            new BBCode(){ Tag = "i", Parse = "<i>{1}</i>" },
            new BBCode(){ Tag = "b", Parse = "<b>{1}</b>" },
            new BBCode(){ Tag = "url", Parse = "<a href=\"{1}\">{2}</a>" },
            new BBCode(){ Tag = "code", Parse = "<pre class=\"{1}\">{2}</pre>" }
        }.AsQueryable();

        IQueryable<Smiley> _smilies = new List<Smiley>()
        {
            new Smiley() { Code = ":)", ImageURL = "smile.png", Title = "smile" },
            new Smiley() { Code = ":(", ImageURL = "frown.png", Title = "frown" }
        }.AsQueryable();


        [SetUp]
        public void Initialize()
        {
            var smileyRepository = new Mock<IRepository<Smiley>>();
            smileyRepository.Setup(x => x.First(TestHelpers.RepositoryIsAny<Smiley>()))
                .Returns<Expression<Func<Smiley, bool>>>(predicate => _smilies.First(predicate));
            smileyRepository.Setup(x => x.Get()).Returns(_smilies);

            var bbCodeRepository = new Mock<IRepository<BBCode>>();
            bbCodeRepository.Setup(x => x.First(TestHelpers.RepositoryIsAny<BBCode>()))
                .Returns<Expression<Func<BBCode, bool>>>(predicate => _bbCodes.First(predicate));

            _parseServices = new ParseServices(
                bbCodeRepository.Object, 
                smileyRepository.Object);
        }

        [Test]
        public void ParseBBCodeText_Multiple_Capture_Groups()
        {
            string input = "[url=http://google.com]Google[/url]";
            string exptectedOutput = "<a href=\"http://google.com\">Google</a>";

            string output = this._parseServices.ParseBBCodeText(input);
            Assert.AreEqual(exptectedOutput, output);
        }

        [Test]
        public void ParseBBCodeText_Nested_Tags()
        {
            string input = "[url=http://google.com][b]Google[/b][/url]";
            string expectedOutput = "<a href=\"http://google.com\"><b>Google</b></a>";

            string output = this._parseServices.ParseBBCodeText(input);
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void ParseBBCodeText_Smile_Code()
        {
            string input = ":)";
            string expectedOutput = "<img alt=\"smile\" src=\"/Images/Smilies/smile.png\" title=\"smile\" />";

            string output = this._parseServices.ParseBBCodeText(input);
            Assert.AreEqual(expectedOutput, output);
        }
    }
}
