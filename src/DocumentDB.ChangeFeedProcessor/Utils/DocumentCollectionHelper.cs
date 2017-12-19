﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  Licensed under the MIT license.
//----------------------------------------------------------------

using Microsoft.Azure.Documents.ChangeFeedProcessor.Adapters;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Documents.ChangeFeedProcessor.Utils
{
    internal static class DocumentCollectionHelper
    {
        private const string DefaultUserAgentSuffix = "changefeed-1.2.0";

        public static DocumentCollectionInfo Canonicalize(this DocumentCollectionInfo collectionInfo)
        {
            DocumentCollectionInfo result = collectionInfo;
            if (string.IsNullOrEmpty(result.ConnectionPolicy.UserAgentSuffix))
            {
                result = new DocumentCollectionInfo(collectionInfo)
                {
                    ConnectionPolicy = { UserAgentSuffix = DefaultUserAgentSuffix }
                };
            }

            return result;
        }

        internal static IDocumentClientEx CreateDocumentClient(this DocumentCollectionInfo collectionInfo)
        {
            var internalClient = new DocumentClient(collectionInfo.Uri, collectionInfo.MasterKey, collectionInfo.ConnectionPolicy);
            return new DocumentClientEx(internalClient);
        }

        internal static string GetCollectionSelfLink(this DocumentCollectionInfo collectionInfo)
        {
            return UriFactory.CreateDocumentCollectionUri(collectionInfo.DatabaseName, collectionInfo.CollectionName).ToString();
        }
    }
}