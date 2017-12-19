﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  Licensed under the MIT license.
//----------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Documents.ChangeFeedProcessor.DocDBErrors
{
    internal static class ExceptionClassifier
    {
        public static DocDbError ClassifyClientException(DocumentClientException clientException)
        {
            SubStatusCode subStatusCode = clientException.GetSubStatusCode();

            if (clientException.StatusCode == HttpStatusCode.NotFound && SubStatusCode.ReadSessionNotAvailable != subStatusCode)
                return DocDbError.PartitionNotFound;

            if (clientException.StatusCode == HttpStatusCode.Gone && (SubStatusCode.PartitionKeyRangeGone == subStatusCode || SubStatusCode.Splitting == subStatusCode))
                return DocDbError.PartitionSplit;

            if (clientException.StatusCode == (HttpStatusCode)429 || clientException.StatusCode >= HttpStatusCode.InternalServerError)
                return DocDbError.TransientError;

            return DocDbError.Undefined;
        }
    }
}