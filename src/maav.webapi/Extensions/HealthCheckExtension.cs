using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAAV.WebAPI.Extensions
{
    static public class HealthCheckExtension
    {
        public static Task WriteHealthCheck(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    WriteAssemblyInfo(writer);
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        if (entry.Value.Exception != null)
                        {
                            writer.WriteString("exception", $"{entry.Value.Exception.GetType().FullName} - {entry.Value.Exception.Message}");
                        }
                        if (entry.Value.Data.Any())
                        {
                            writer.WriteStartObject("data");
                            foreach (var item in entry.Value.Data)
                            {
                                writer.WritePropertyName(item.Key);
                                JsonSerializer.Serialize(
                                    writer, item.Value, item.Value?.GetType() ?? 
                                    typeof(object));
                            }
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }

        private static void WriteAssemblyInfo(Utf8JsonWriter writer)
        {
            writer.WriteString("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            writer.WriteString("product", Assembly.GetExecutingAssembly().GetName().Name);
        }
    }
}