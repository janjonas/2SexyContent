﻿using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.DataSources.Tokens
{
    public class ModulePropertyAccess : ToSic.Eav.DataSources.Tokens.IPropertyAccess, DotNetNuke.Services.Tokens.IPropertyAccess
    {
        private int _moduleId;
        private readonly ModuleInfo _moduleInfo;

	    public ModulePropertyAccess(string name)
	    {
		    Name = name;
	    }

	    public string Name { get; private set; }

        public ModulePropertyAccess(int moduleId)
        {
            _moduleId = moduleId;
            var ctr = new ModuleController();
            _moduleInfo = ctr.GetModule(moduleId);
        }

        public string GetProperty(string propertyName, string format, ref bool propertyNotFound)
        {
            return GetProperty(propertyName, "", null, null, Scope.DefaultSettings, ref propertyNotFound);
        }

        #region DotNetNuke IPropertyAccess Members

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        public string GetProperty(string propertyName, string format, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo accessingUser, DotNetNuke.Services.Tokens.Scope accessLevel, ref bool propertyNotFound)
        {
            return _moduleInfo.GetProperty(propertyName, format, formatProvider, accessingUser, accessLevel, ref propertyNotFound);
        }

        #endregion
    }
}