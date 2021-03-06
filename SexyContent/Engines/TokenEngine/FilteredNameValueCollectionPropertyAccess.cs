﻿using System.Collections.Specialized;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Tokens;

namespace ToSic.SexyContent.Engines.TokenEngine
{
    /// <summary>
    /// Copied from Form and List Module
    /// </summary>
    public class FilteredNameValueCollectionPropertyAccess : IPropertyAccess
    {
        NameValueCollection NameValueCollection;
        public FilteredNameValueCollectionPropertyAccess(NameValueCollection list)
        {
            NameValueCollection = list;
        }

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        /// <summary>
        /// Get Property out of NameValueCollection
        /// </summary>
        /// <param name="strPropertyName"></param>
        /// <param name="strFormat"></param>
        /// <param name="formatProvider"></param>
        /// <param name="AccessingUser"></param>
        /// <param name="AccessLevel"></param>
        /// <param name="PropertyNotFound"></param>
        /// <returns></returns>
        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
        {
            if (NameValueCollection == null)
                return string.Empty;
            string value = NameValueCollection[strPropertyName];
            string OutputFormat = null;
            if (strFormat == string.Empty)
            {
                OutputFormat = "g";
            }
            else
            {
                OutputFormat = string.Empty;
            }
            if (value != null)
            {
                PortalSecurity Security = new PortalSecurity();
                value = Security.InputFilter(value, PortalSecurity.FilterFlag.NoScripting);
                return Security.InputFilter(PropertyAccess.FormatString(value, strFormat), PortalSecurity.FilterFlag.NoScripting);
            }
            else
            {
                PropertyNotFound = true;
                return string.Empty;
            }
        }
    }
}