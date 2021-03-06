﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.EAVExtensions;

namespace ToSic.SexyContent
{
    public class DynamicEntity : DynamicObject
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public IEntity Entity { get; set; }
        public HtmlString Toolbar {
            get
            {
                if (SexyContext == null || PortalSettings.Current == null)
                    return new HtmlString("");

                if (Entity is IHasEditingData)
                    return new HtmlString("<ul class='sc-menu' data-toolbar='" + Newtonsoft.Json.JsonConvert.SerializeObject(new { sortOrder = ((IHasEditingData) Entity).SortOrder, useModuleList = true, isPublished = Entity.IsPublished }) + "'></ul>");

                return new HtmlString("<ul class='sc-menu' data-toolbar='" + Newtonsoft.Json.JsonConvert.SerializeObject(new { entityId = Entity.EntityId, isPublished = Entity.IsPublished, attributeSetName = Entity.Type.Name }) + "'></ul>");
            }
        }
        private readonly string[] _dimensions;
        private SexyContent SexyContext { get; set; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(IEntity entityModel, string[] dimensions, SexyContent sexy)
        {
            this.Entity = entityModel;
            this._dimensions = dimensions;
            SexyContext = sexy;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
        }

        public bool TryGetMember(string memberName, out object result)
        {
            var propertyNotFound = false;
            result = GetEntityValue(memberName, out propertyNotFound);

            if (propertyNotFound)
                result = null; // String.Format(Configuration.ErrorKeyMissing, memberName);

            return true;
        }

        public object GetEntityValue(string attributeName, out bool propertyNotFound)
        {
            propertyNotFound = false;
            object result;
            
            
            if (Entity.Attributes.ContainsKey(attributeName))
            {
                var attribute = Entity.Attributes[attributeName];
                result = attribute[_dimensions];

                if (attribute.Type == "Hyperlink" && result is string)
                {
                    result = SexyContent.ResolveHyperlinkValues((string) result, SexyContext == null ? PortalSettings.Current : SexyContext.OwnerPS);
                }
                else if (attribute.Type == "Entity" && result is EntityRelationshipModel)
                {
                    // Convert related entities to Dynamics
                    result = ((ToSic.Eav.EntityRelationshipModel) result).Select(
                        p => new DynamicEntity(p, _dimensions, this.SexyContext)
                        ).ToList();
                }
            }
            else
            {
                switch (attributeName)
                {
                    case "EntityTitle":
                        result = EntityTitle;
                        break;
                    case "EntityId":
                        result = EntityId;
                        break;
                    case "Toolbar":
                        result = Toolbar.ToString();
                        break;
                    case "IsPublished":
                        result = Entity.IsPublished;
                        break;
                    case "Modified":
                        result = Entity.Modified;
                        break;
                    default:
                        result = null;
                        propertyNotFound = true;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Configuration class for this expando
        /// </summary>
        public class ContentConfiguration
        {
            public string ErrorKeyMissing {
                get { return null; }
                set
                {
                    throw new Exception("Obsolete: Do not use ErrorKeyMissing anymore. Check if the value is null instead.");
                }
            }
        }

        public int EntityId
        {
            get { return Entity.EntityId; }
        }

        public Guid EntityGuid
        {
            get { return Entity.EntityGuid; }
        }

        public object EntityTitle
        {
            get { return Entity.Title[_dimensions]; }
        }

        public dynamic GetDraft()
        {
            return new DynamicEntity(Entity.GetDraft(), _dimensions, this.SexyContext);
        }

        public dynamic GetPublished()
        {
            return new DynamicEntity(Entity.GetPublished(), _dimensions, this.SexyContext);
        }

    }
}