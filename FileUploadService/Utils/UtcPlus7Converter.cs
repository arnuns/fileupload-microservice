using Newtonsoft.Json;
using System;

namespace FileUploadService.Utils;

public class UtcPlus7Converter : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException("Expected string type");

        if (DateTime.TryParse((string)reader.Value, out DateTime parsedDateTime))
        {
            return parsedDateTime.AddHours(-7).ToUniversalTime();
        }

        throw new JsonSerializationException("Invalid date format");
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        // Convert UTC DateTime to UTC+7 for serialization
        DateTime dateTimeWithOffset = value.ToUniversalTime().AddHours(7);
        writer.WriteValue(dateTimeWithOffset);
    }
}