using Newtonsoft.Json;

namespace FileUploadService.Utils;

public class UtcPlus7Converter : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException("Expected string type");

        var dateTimeStr = reader.Value?.ToString();
        if (DateTime.TryParse(dateTimeStr, out DateTime dateTime))
        {
            // Check if the DateTime is in UTC before applying the offset conversion
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime.AddHours(7);
            return dateTime;  // Return the dateTime without any modification if it's not UTC
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