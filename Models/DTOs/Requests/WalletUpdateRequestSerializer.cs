using Confluent.Kafka;
using System.Text.Json;
using Transaction_Service.Data.Entities;

namespace Transaction_Service.Models.DTOs.Requests
{
    public class WalletUpdateRequestSerializer : ISerializer<WalletUpdateRequest>
    {
        public byte[] Serialize(WalletUpdateRequest data, SerializationContext context)
        {
            // Return null if the data is null (or throw an exception if you prefer)
            return data is null ? null : JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

}
