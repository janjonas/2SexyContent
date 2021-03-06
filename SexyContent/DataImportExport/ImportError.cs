﻿namespace ToSic.SexyContent.DataImportExport
{
    public class ImportError
    {
        public int? LineNumber { get; private set; }

        public string LineDetail { get; private set; }

        public string ErrorDetail { get; private set; }

        public ImportErrorCode ErrorCode { get; private set; }

        public ImportError(ImportErrorCode errorCode, string errorDetail = null, int? lineNumber = null, string lineDetail = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorDetail = errorDetail;
            this.LineNumber = lineNumber;
            this.LineDetail = lineDetail;
        }
    }
}