// Copyright (c) 2024-2026 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;

namespace AccelByte.Extend.Vivox.Authentication.Server.Vivox
{
    public class VivoxConfigProvider : IVivoxConfigProvider
    {
        public string Issuer { get; set; } = "";

        public string Domain { get; set; } = "";

        public string SigningKey { get; set; } = "";

        public string ChannelPrefix { get; set; } = "confctl";

        public string Protocol { get; set; } = "sip";

        public int DefaultExpiry { get; set; } = 90;

        public void ReadEnvironmentVariables()
        {
            string? vIssuer = Environment.GetEnvironmentVariable("VIVOX_ISSUER");
            if ((vIssuer != null) && (vIssuer.Trim() != ""))
                Issuer = vIssuer.Trim();

            if (Issuer == "")
                throw new Exception("Vivox's Issuer configuration value is empty or VIVOX_ISSUER env var is missing.");

            string? vDomain = Environment.GetEnvironmentVariable("VIVOX_DOMAIN");
            if ((vDomain != null) && (vDomain.Trim() != ""))
                Domain = vDomain.Trim();

            if (Domain == "")
                throw new Exception("Vivox's Domain configuration value is empty or VIVOX_DOMAIN env var is missing.");

            string? vSigningKey = Environment.GetEnvironmentVariable("VIVOX_SIGNING_KEY");
            if ((vSigningKey != null) && (vSigningKey.Trim() != ""))
                SigningKey = vSigningKey.Trim();

            if (SigningKey == "")
                throw new Exception("Vivox's SigningKey configuration value is empty or VIVOX_SIGNING_KEY env var is missing.");

            //optional, default value is `confctl`.
            string? vChannelPrefix = Environment.GetEnvironmentVariable("VIVOX_CHANNEL_PREFIX");
            if ((vChannelPrefix != null) && (vChannelPrefix.Trim() != ""))
                ChannelPrefix = vChannelPrefix.Trim();

            //optional, default value is `sip`.
            string? vProtocol = Environment.GetEnvironmentVariable("VIVOX_PROTOCOL");
            if ((vProtocol != null) && (vProtocol.Trim() != ""))
                Protocol = vProtocol.Trim();

            //optional, default value is `90`.
            string? vDefaultExpiry = Environment.GetEnvironmentVariable("VIVOX_DEFAULT_EXPIRY");
            if ((vDefaultExpiry != null) && vDefaultExpiry.Trim() != "")
                DefaultExpiry = int.Parse(vDefaultExpiry.Trim());
        }
    }
}
