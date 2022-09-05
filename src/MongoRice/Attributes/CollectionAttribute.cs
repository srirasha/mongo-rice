using System;

namespace MongoRice.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public CollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}