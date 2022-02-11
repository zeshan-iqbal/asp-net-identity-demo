using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


public class Program
{

    public struct MaskingToken
    {
        public MaskingCategory Category { get; private set; }
        public Func<string, string>? Mask { get; private set; }
        public string Token { get; private set; }

        public MaskingToken(string token, Func<string, string>? mask)
        {
            Mask = mask;
            Token = token;
            Category = MaskingCategory.Full;
        }

        public MaskingToken(string token, MaskingCategory category = MaskingCategory.Full)
        {
            Category = category;
            Token = token;
            Mask = null;
        }

        public string MaskData(IList<MaskingToken> tokens, object data)
        {
            if (Mask != null)
            {
                return Mask("");
            }

            //
            return data;
        }
    }


	public class Request
	{
		[Mask(MaskingCategory.PersonalData)]
		public string Name { get; set; }

		public string OrderReference { get; set; }
	}

	public enum MaskingCategory
	{
		CardNmber,
		CardSecurityCode,
		PersonalData,
		Full
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class MaskAttribute : Attribute
	{
		public MaskingCategory Category { get; private set; }

		public MaskAttribute(MaskingCategory maskingCategory = MaskingCategory.Full)
		{
			Category = maskingCategory;
		}


		public string Mask(string value)
		{
			//check for null or empty cases.
			//resolve strategy for masking data.
			//In this approach masking code is comanized out of the box, Howerver, the draw back is that we can't customize masking logic per integrator/type.
			return "xxxxxxxx";
		}
    }

	public static void Main()
	{

		var request = new Request()
		{
			Name = "Zeeshan Iqbal",
			OrderReference = "1235879"
		};

		//This code will also go into common as masking utility,
        MaskData(request);
	}


    public static void MaskData<T>(T data)
    {
        string umasked = JsonSerializer.Serialize(data);

        Console.WriteLine($"UnMasked: {umasked}");
        //diff between isdefined and getcustomattributes??

        var memebers = typeof(T).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(MaskAttribute)));

        foreach (var member in memebers)
        {
            var attribute = (MaskAttribute)Attribute.GetCustomAttribute(member, typeof(MaskAttribute));

            var value = attribute.Mask(member.GetValue(data).ToString());
			
            member.SetValue(data, value);

            Console.WriteLine(attribute.Category);
        }

        string jsonString = JsonSerializer.Serialize(data);

        Console.WriteLine($"Masked: {jsonString}");
	}
}