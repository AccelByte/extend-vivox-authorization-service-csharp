// Copyright (c) 2024 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Grpc.Core;
using AccelByte.Sdk.Api;
using AccelByte.Extend.ServiceExtension;
using AccelByte.Extend.Vivox.Authentication.Server.Vivox;

namespace AccelByte.Extend.Vivox.Authentication.Server.Services
{
    public class VivoxServiceImplementation : Service.ServiceBase
    {
        private readonly ILogger<VivoxServiceImplementation> _Logger;

        private readonly IVivoxConfigProvider _VivoxConfig;

        private readonly ITokenGenerator _TokenGenerator;

        public VivoxServiceImplementation(
            ILogger<VivoxServiceImplementation> logger,
            IVivoxConfigProvider vivoxConfig,
            ITokenGenerator tokenGenerator)
        {
            _Logger = logger;
            _VivoxConfig = vivoxConfig;
            _TokenGenerator = tokenGenerator;
        }

        protected string GetChannelType(GenerateVivoxTokenRequest request)
        {
            string channelType = "";
            switch (request.ChannelType)
            {
                case GenerateVivoxTokenRequestChannelType.Echo:
                    channelType = "e";
                    break;
                case GenerateVivoxTokenRequestChannelType.Positional:
                    channelType = "p";
                    break;
                case GenerateVivoxTokenRequestChannelType.Nonpositional:
                    channelType = "g";
                    break;
            }

            if (channelType == "")
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Channel type is required but invalid."));

            return channelType;
        }

        public override Task<GenerateVivoxTokenResponse> GenerateVivoxToken(GenerateVivoxTokenRequest request, ServerCallContext context)
        {
            TokenData? token = null;
            switch (request.Type)
            {
                case GenerateVivoxTokenRequestType.Login:
                    token = _TokenGenerator.Generate(_VivoxConfig, request.Username, "login");
                    break;
                case GenerateVivoxTokenRequestType.Join:
                    string joinCt = GetChannelType(request);
                    token = _TokenGenerator.Generate(_VivoxConfig, request.Username, "join", joinCt, request.ChannelId);
                    break;
                case GenerateVivoxTokenRequestType.JoinMuted:
                    string joinMuteCt = GetChannelType(request);
                    token = _TokenGenerator.Generate(_VivoxConfig, request.Username, "join_muted", joinMuteCt, request.ChannelId);
                    break;
                case GenerateVivoxTokenRequestType.Kick:
                    string kickCt = GetChannelType(request);
                    token = _TokenGenerator.Generate(_VivoxConfig, request.Username, "kick", kickCt, request.ChannelId, request.TargetUsername);
                    break;
                default:
                    break;
            }

            if (token == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request type"));

            GenerateVivoxTokenResponse response = new GenerateVivoxTokenResponse()
            {
                AccessToken = token.Value
            };

            if (token.Claims.To != null)
            {
                response.Uri = token.Claims.To;
            }

            return Task.FromResult(response);
        }
    }
}
