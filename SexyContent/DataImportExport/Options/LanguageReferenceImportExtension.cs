﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class LanguageReferenceImportExtension
    {
        public static bool IsResolve(this LanguageReferenceImport option)
        {
            return option == LanguageReferenceImport.Resolve;
        }
    }
}