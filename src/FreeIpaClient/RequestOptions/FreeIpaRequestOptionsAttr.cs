using System;
using System.Collections.Generic;
using FreeIpaClient.Models;

namespace FreeIpaClient.RequestOptions
{
    public class FreeIpaRequestOptionsAttr : FreeIpaRequestOptions
    {      
        public List<string> Setattr { get; private set; }
        public List<string> Addattr { get; private set; }
        public List<string> Delattr { get; private set; }

        public void AddSetAttr(string key, object value)
        {
            Setattr ??= new List<string>();
            Setattr.Add(BuildAttr(key, value));
        }

        public void AddAddAttr(string key, object value)
        {
            Addattr ??= new List<string>();
            Addattr.Add(BuildAttr(key, value));
        }

        public void AddDelAttr(string key, object value)
        {
            Delattr ??= new List<string>();
            Delattr.Add(BuildAttr(key, value));
        }

        private string BuildAttr(string key, object value)
        {
            string attr;
            if (value == null)
            {
                attr = BuildNull(key);
            }
            else if (value is string)
            {
                attr = BuildAttr(key, (string) value);
            }
            else if (value is int)
            {
                attr = BuildAttr(key, (int) value);
            }
            else if (value is DateTime)
            {
                attr = BuildAttr(key, (DateTime) value);
            }
            else if (value is bool)
            {
                attr = BuildAttr(key, (bool) value);
            }
            else if (value is string[])
            {
                attr = BuildAttr(key, (string[]) value);
            }
            else
            {
                throw new NotImplementedException($"Attribute of {value.GetType()} is not implemented.");
            }
            return attr;
        }

        private string BuildNull(string key)
        {
            return $"{key}=";
        }

        private string BuildAttr(string key, string value)
        {
            return $"{key}={value}";
        }

        private string BuildAttr(string key, int value)
        {
            return $"{key}={value}";
        }

        private string BuildAttr(string key, bool value)
        {
            return $"{key}={value.ToString().ToUpper()}";
        }

        private string BuildAttr(string key, DateTime value)
        {
            return $"{key}={value.ToString(FreeIpaDateTimeConverter.Format)}";
        }

        private string BuildAttr(string key, string[] value)
        {
            return $"{key}={string.Join(",", value)}";
        }
    }
}
