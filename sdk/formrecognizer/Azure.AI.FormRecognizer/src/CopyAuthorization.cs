﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;

namespace Azure.AI.FormRecognizer.Training
{
    /// <summary>
    /// Authorization for copying a custom model into the target Form Recognizer resource.
    /// </summary>
    public class CopyAuthorization
    {
        /// <summary>Model identifier in the target Form Recognizer Resource. </summary>
        public string ModelId { get; }
        /// <summary> The time when the access token expires. The date is represented as the number of seconds from 1970-01-01T0:0:0Z UTC until the expiration time. </summary>
        //public DateTimeOffset ExpiresOn { get; set; }
        public long ExpiresOn { get; }
        /// <summary> Token claim used to authorize the request. </summary>
        internal string _accessToken { get; }
        /// <summary> Azure Resource Id of the target Form Recognizer resource where the model is copied to. </summary>
        internal string _resourceId { get; }
        /// <summary> Location of the target Form Recognizer resource. A valid Azure region name supported by Cognitive Services. </summary>
        internal string _region { get; }

        internal CopyAuthorization(string modelId, string accessToken, long expirationDateTimeTicks, string resourceId, string region)
        {
            ModelId = modelId;
            _accessToken = accessToken;
            //ExpiresOn = DateTimeOffset.FromUnixTimeSeconds(expirationDateTimeTicks);
            ExpiresOn = expirationDateTimeTicks;
            _resourceId = resourceId;
            _region = region;
        }

        internal CopyAuthorization(CopyAuthorizationResult copyAuth, string resourceId, string region)
            : this(copyAuth.ModelId, copyAuth.AccessToken, copyAuth.ExpirationDateTimeTicks, resourceId, region) { }

        /// <summary>
        /// Deserializes an opaque string into a <see cref="CopyAuthorization"/> object.
        /// </summary>
        /// <param name="accessToken">Opaque string with the access token information for a specific model.</param>
        public static CopyAuthorization FromJson(string accessToken)
        {
            CopyAuthorizationParse parse = JsonSerializer.Deserialize<CopyAuthorizationParse>(accessToken);
            return new CopyAuthorization(
                parse.modelId,
                parse.accessToken,
                //DateTimeOffset.FromUnixTimeSeconds(parse.expirationDateTimeTicks),
                parse.expirationDateTimeTicks,
                parse.resourceId,
                parse.resourceRegion);
        }

        /// <summary>
        /// Converts the CopyAuthorization object to its equivalent json representation.
        /// </summary>
        public string ToJson()
        {
            var toParse = new CopyAuthorizationParse(this);
            return JsonSerializer.Serialize(toParse);
        }

        private class CopyAuthorizationParse
        {
            public string modelId { get; set; }
            public string accessToken { get; set; }
            public long expirationDateTimeTicks { get; set; }
            public string resourceId { get; set; }
            public string resourceRegion { get; set; }

            public CopyAuthorizationParse() { }

            public CopyAuthorizationParse(CopyAuthorization target)
            {
                modelId = target.ModelId;
                accessToken = target._accessToken;
                //expirationDateTimeTicks = target.ExpiresOn.ToUnixTimeSeconds();
                expirationDateTimeTicks = target.ExpiresOn;
                resourceId = target._resourceId;
                resourceRegion = target._region;
            }
        }
    }
}
