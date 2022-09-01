using Microsoft.MixedReality.CloudLfs.Contracts.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.MixedReality.CloudLfs
{
    public class GitMessageContractConverter : JsonConverter<GitLfsMessageV1>
    {
        public override bool CanWrite => true;

        public override bool CanRead => true;

        public override GitLfsMessageV1? ReadJson(JsonReader reader,
            Type objectType,
            GitLfsMessageV1? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string? eventType = (string?)jo["event"];
            GitLfsMessageV1 message;
            switch (eventType)
            {
                case "init":
                    message = new GitLfsMessageV1();
                    break;

                default:
                    message = new GitLfsMessageV1();
                    break;
            }

            serializer.Populate(jo.CreateReader(), message);

            return message;
        }

        public override void WriteJson(JsonWriter writer, GitLfsMessageV1? value, JsonSerializer serializer)
        {
            JObject o = JObject.FromObject(value);
            o.WriteTo(writer);
        }
    }
}