using System;
using System.Collections.Generic;
using FreeIpaClient.Models;

namespace FreeIpaClient.RequestOptions
{
    public class FreeIpaRequestOptionsAttr : FreeIpaRequestOptions
    {      
        public List<string> Setattr { get; private set; }       

        protected void AddSetAttr(string key, object value)
        {
            if (Setattr == null)
            {
                Setattr = new List<string>();
            }
            Setattr.Add(BuildAttr(key, value));
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
    }
}