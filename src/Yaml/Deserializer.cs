
using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RoboPhredDev.Shipbreaker.SixAxis.Yaml
{
    static class Deserializer
    {
        public static T Deserialize<T>(string filePath)
        {
            var deserializer = BuildDeserializer();
            return WithFileParser(filePath, parser =>
            {
                var deserializer = BuildDeserializer();
                return deserializer.Deserialize<T>(parser);
            });
        }

        public static T WithFileParser<T>(string filePath, Func<IParser, T> func)
        {
            try
            {
                var fileContents = File.ReadAllText(filePath);
                using var reader = new StringReader(fileContents);
                var parser = new Parser(reader);
                return func(parser);
            }
            catch (YamlException ex) when (!(ex is YamlFileException))
            {
                Logging.Log($"Error parsing file {filePath}: {ex.Message}");
                throw new YamlFileException(filePath, ex.Start, ex.End, ex.Message, ex);
            }
        }

        private static IDeserializer BuildDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }
    }
}