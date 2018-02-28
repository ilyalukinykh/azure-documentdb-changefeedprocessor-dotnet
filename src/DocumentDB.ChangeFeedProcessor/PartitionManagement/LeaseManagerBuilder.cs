﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  Licensed under the MIT license.
//----------------------------------------------------------------

namespace Microsoft.Azure.Documents.ChangeFeedProcessor.PartitionManagement
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents.ChangeFeedProcessor.Adapters;
    using Microsoft.Azure.Documents.ChangeFeedProcessor.Utils;

    internal class LeaseManagerBuilder : ILeaseManagerBuilder
    {
        private DocumentCollectionInfo leaseCollectionLocation;
        private IDocumentClientEx leaseDocumentClient;
        private string leasePrefix;
        private string leaseStoreCollectionLink;

        public LeaseManagerBuilder WithLeaseCollection(DocumentCollectionInfo leaseCollectionLocation)
        {
            if (leaseCollectionLocation == null) throw new ArgumentNullException(nameof(leaseCollectionLocation));

            this.leaseCollectionLocation = leaseCollectionLocation.Canonicalize();
            return this;
        }

        public LeaseManagerBuilder WithLeaseDocumentClient(IDocumentClientEx leaseDocumentClient)
        {
            if (leaseDocumentClient == null) throw new ArgumentNullException(nameof(leaseDocumentClient));

            this.leaseDocumentClient = leaseDocumentClient;
            return this;
        }

        public LeaseManagerBuilder WithLeasePrefix(string leasePrefix)
        {
            if (leasePrefix == null) throw new ArgumentNullException(nameof(leasePrefix));

            this.leasePrefix = leasePrefix;
            return this;
        }

        public LeaseManagerBuilder WithLeaseCollectionLink(string leaseStoreCollectionLink)
        {
            if (leaseStoreCollectionLink == null) throw new ArgumentNullException(nameof(leaseStoreCollectionLink));

            this.leaseStoreCollectionLink = leaseStoreCollectionLink;
            return this;
        }

        public async Task<ILeaseManager> BuildAsync()
        {
            if (this.leaseCollectionLocation == null) throw new InvalidOperationException(nameof(this.leaseCollectionLocation) + " was not specified");

            this.leaseDocumentClient = this.leaseDocumentClient ?? this.leaseCollectionLocation.CreateDocumentClient();

            if (this.leaseStoreCollectionLink == null)
            {
                DocumentCollection documentCollection = await this.leaseDocumentClient.GetDocumentCollectionAsync(this.leaseCollectionLocation).ConfigureAwait(false);
                this.leaseStoreCollectionLink = documentCollection.SelfLink;
            }

            var updater = new DocumentServiceLeaseUpdater(this.leaseDocumentClient);
            var documentServiceLeaseManager = new DocumentServiceLeaseManager(
                this.leaseDocumentClient,
                updater,
                this.leaseCollectionLocation,
                this.leasePrefix,
                this.leaseStoreCollectionLink);
            return documentServiceLeaseManager;
        }
    }
}