using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Reviews.Core
{
    public class EventTypeMapper
    {
        private readonly Dictionary<string,Type> typeByName     = new Dictionary<string, Type>();
        private readonly Dictionary<Type,string> nameByTypes     = new Dictionary<Type, string>();

        public EventTypeMapper Map(Type type, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = type.FullName;
            
            if(typeByName.ContainsKey(name))
                throw new InvalidOperationException($"'{type}' is already mapped with name: '{name}'");

            typeByName[name] = type;
            nameByTypes[type] = name;
            
            return this;
        }

        public bool TryGetEventType(string name, out Type type) => typeByName.TryGetValue(name, out type);
        public bool TryGetEventName(Type type,out string name) => nameByTypes.TryGetValue(type, out name);
    }

    public static class EventTypeMapperExtentions
    {
        public static EventTypeMapper Map<T>(this EventTypeMapper mapper, string name) => mapper.Map(typeof(T), name);
        
        public static Type GetEventType(this EventTypeMapper mapper, string name)
        {
            if (!mapper.TryGetEventType(name, out var type))
                throw new Exception($"Failed to find type mapped with '{name}'");

            return type;
        }

        public static string GetEventName(this EventTypeMapper mapper, Type type)
        {
            if(!mapper.TryGetEventName(type,out var name))
                throw new Exception($"Failed to find name mapped with '{type}'");

            return name;
        }
    }
    
    public class InvalidExpectedStreamVersionException : Exception
    {
        public InvalidExpectedStreamVersionException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}