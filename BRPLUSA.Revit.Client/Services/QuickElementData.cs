﻿namespace BRPLUSA.Revit.Client.Services
{
    public class QuickElementData
    {
        public string FieldValue { get; set; }
        public string FieldName { get; set; }

        public QuickElementData(string fieldName, string value)
        {
            FieldName = fieldName;
            FieldValue = value;
        }
    }
}