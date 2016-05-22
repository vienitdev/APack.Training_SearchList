using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Archpack.Training.ArchUnits.Validations.V1
{
    public class StringByteLengthAttribute : ValidationAttribute
    {

        private const string DefaultEncodingName = "utf-8";

        public int MaximumLength { get; private set; }

        public string EncodingName { get; set; }

        public StringByteLengthAttribute(int maximumLength)
        {
            this.MaximumLength = maximumLength;
            this.EncodingName = DefaultEncodingName;
        }

        public override bool IsValid(object value)
        {
            string valueString = value as string;

            if (!string.IsNullOrEmpty(valueString))
            {
                Encoding encoding = System.Text.Encoding.GetEncoding(this.EncodingName);
                int length = encoding.GetByteCount(valueString);
                return (length <= this.MaximumLength);
            }

            return true;
        }
    }
}