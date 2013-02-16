﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Alias;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Piedone.HelpfulLibraries.Libraries.Contents.Tokens
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ContentTokens : ITokenProvider
    {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IAliasService _aliasService;

        private IContent _currentContent = null;
        private IContent CurrentContent
        {
            get
            {
                if (_currentContent == null)
                {
                    var itemRoute = _aliasService.Get(_workContextAccessor.GetContext().HttpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(1).Trim('/'));
                    if (itemRoute == null) _currentContent = _contentManager.New("Dummy"); // _currentContent isn't null so chained tokens don't throw a NE
                    else  _currentContent = _contentManager.Get(Convert.ToInt32(itemRoute["Id"]));
                }

                return _currentContent;
            }
        }
        
        public Localizer T { get; set; }


        public ContentTokens(
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor,
            IAliasService aliasService)
        {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _aliasService = aliasService;

            T = NullLocalizer.Instance;
        }


        public void Describe(DescribeContext context)
        {
            context.For("Content", T("Content Items"), T("Content Items"))
                .Token("Current", T("Current content"), T("If the current request is for a content item, returns the item."));
        }

        public void Evaluate(EvaluateContext context)
        {
            context.For<IContent>("Content")
                .Token("Current", content => string.Empty)
                .Chain("Current", "Content", content => CurrentContent);
        }
    }
}
