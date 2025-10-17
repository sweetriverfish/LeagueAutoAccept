using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System;

internal static class StringExtensions
{

	public static string ToStringInvariant(this int value)
	{
		//if (value == null) return "";
		return value.ToString(CultureInfo.InvariantCulture);
	}
	public static string ToStringInvariant(this uint value)
	{
		//if (value == null) return "";
		return value.ToString(CultureInfo.InvariantCulture);
	}

}
