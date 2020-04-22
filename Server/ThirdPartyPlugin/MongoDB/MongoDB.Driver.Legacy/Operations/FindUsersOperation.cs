/* Copyright 2010-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;

namespace MongoDB.Driver.Operations
{
    internal class FindUsersOperation : IReadOperation<IEnumerable<BsonDocument>>
    {
        // fields
        private readonly DatabaseNamespace _databaseNamespace;
        private readonly MessageEncoderSettings _messageEncoderSettings;
        private readonly string _username;

        // constructors
        public FindUsersOperation(
            DatabaseNamespace databaseNamespace,
            string username,
            MessageEncoderSettings messageEncoderSettings)
        {
            _databaseNamespace = databaseNamespace;
            _username = username;
            _messageEncoderSettings = messageEncoderSettings;
        }

        // methods
        public IEnumerable<BsonDocument> Execute(IReadBinding binding, CancellationToken cancellationToken)
        {
            using (var channelSource = binding.GetReadChannelSource(cancellationToken))
            using (var channel = channelSource.GetChannel(cancellationToken))
            using (var channelBinding = new ChannelReadBinding(channelSource.Server, channel, binding.ReadPreference, binding.Session.Fork()))
            {
                var operation = new FindUsersUsingUserManagementCommandsOperation(
                    _databaseNamespace,
                    _username,
                    _messageEncoderSettings);

                return operation.Execute(channelBinding, cancellationToken);
            }
        }

        public Task<IEnumerable<BsonDocument>> ExecuteAsync(IReadBinding binding, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
